from xmlrpc.server import SimpleXMLRPCServer
import queue
import tkinter as tk
from tkinter import messagebox
from tkinter import scrolledtext
import threading
import sys
import getopt
import asyncio
import threading

"""
From SenhaixFreqWriter->Const
For gt12 and shx8800
DEPRECATED! USE BLEPlugin.go INSTEAD!
"""
RwServiceUuid = "ffe0"
RwCharacteristicUuid = "ffe1"
BtnameShx8800 = "walkie-talkie"

rpc_address = "127.0.0.1"
rpc_port = 8563
use_gui_m = False

peripherals = None
peripheral = None
service_uuid = None
service = None
characteristic_uuid = None
characteristic = None
server = None

mtu = 23

isPyBleAvailable = True

dataQueue = queue.Queue()

try: 
    import bleak
except Exception:
    isPyBleAvailable = False


def GetBleAvailability():
    global scanner
    scanner = bleak.BleakScanner()
    return True

async def ScanForShx():
    global peripherals
    while not dataQueue.empty():
        dataQueue.get()
    await DisposeBluetooth()
    peripherals = await scanner.discover(timeout=5)
    devList = []
    for i, per in enumerate(peripherals):
        devList.append({"DeviceName": per.name if per.name is not None else "", "DeviceMacAddr": per.address,"DeviceID":str(i)})
    insert_log(devList)
    return str(devList)

def SetDevice(seq):
    global peripheral
    peripheral = bleak.BleakClient(peripherals[int(seq)])

async def ConnectShxDevice():
    if peripheral is None:
        return False
    try:
        insert_log(f"Connecting to: [{peripheral.address}]")
        await peripheral.connect()
        # mtu = peripheral.mtu()
        insert_log("Successfully connected")
        return True
    except Exception as e:
        insert_log(repr(e))
        return False
    
async def ConnectShxRwService():
    global service_uuid, service
    if peripheral is None: return False
    services = peripheral.services
    for serviced in services:
        serid = serviced.uuid
        if RwServiceUuid in str(serid):
            service_uuid = serid
            service = serviced
            insert_log(f"Service Connected")
            return True
    return False

async def ConnectShxRwCharacteristic():
    global characteristic_uuid, characteristic
    if service is None: return False
    for characteristicd in service.characteristics:
        chrid = characteristicd.uuid
        if RwCharacteristicUuid in str(chrid):
            characteristic_uuid = chrid
            characteristic = characteristicd
            # Register Callback here
            insert_log("Registering notify...")
            await peripheral.start_notify(chrid, callback=CallbackOnDataReceived)
            insert_log("Registeredd notify")
            return True
    return False 

async def ReadCachedData():
    try:
        if (dataQueue.empty()):return None
        dt = dataQueue.get()
        insert_log("read:"+str(dt))
        return dt
    except Exception as e:
        insert_log(repr(e))
        return None

async def WriteData(data):
    try:
        await peripheral.write_gatt_char(characteristic_uuid, data.data, False)
        insert_log("Write:"+str(data))
        return True
    except Exception as e:
        insert_log(repr(e))
        return False

async def DisposeBluetooth():
    global peripheral,service_uuid,service,characteristic_uuid,characteristic
    if peripheral is not None:
        await peripheral.disconnect()
    service_uuid,service,characteristic_uuid,characteristic,peripheral = None, None, None, None,None

async def CallbackOnDataReceived(sender,data:bytearray):
    insert_log(f"recv: {data}")
    dataQueue.put(bytes(data))


def start_rpc_server(rpc_address):
    global server
    try:
        loop = asyncio.get_event_loop()
        server = SimpleXMLRPCServer((rpc_address.split(':')[0], int(rpc_address.split(':')[1])), allow_none=True,logRequests=False)
        server.register_function(GetBleAvailability, "GetBleAvailability")
        server.register_function(lambda: loop.run_until_complete(ScanForShx()), "ScanForShx")
        server.register_function(lambda: loop.run_until_complete(ConnectShxDevice()), "ConnectShxDevice")
        server.register_function(lambda: loop.run_until_complete(ConnectShxRwService()), "ConnectShxRwService")
        server.register_function(lambda: loop.run_until_complete(ConnectShxRwCharacteristic()), "ConnectShxRwCharacteristic")
        server.register_function(ReadCachedData, "ReadCachedData")
        server.register_function(SetDevice, "SetDevice")
        server.register_function(lambda data: loop.run_until_complete(WriteData(data)), "WriteData")
        server.register_function(lambda : loop.run_until_complete(DisposeBluetooth), "DisposeBluetooth")
        server_thread = threading.Thread(target=server.serve_forever)
        server_thread.daemon = True
        server_thread.start()
        insert_log( "RPC server started.")
        start_button.config(state='disabled')
        stop_button.config(state='normal')
    except Exception as e:
        messagebox.showerror("Error", f"Failed to start RPC server: {str(e)}")

def stop_rpc_server():
    global server
    try:
        server.shutdown()
        if peripheral is not None:
            peripheral.stop_notify(characteristic_uuid)
            peripheral.disconnect()
        insert_log("RPC server stopped.")
        start_button.config(state='normal')
        stop_button.config(state='disabled')
    except Exception as e:
        messagebox.showerror("Error", f"Failed to stop RPC server: {str(e)}")

def get_rpc_address():
    rpc_address = rpc_entry.get()
    if not rpc_address:
        rpc_address = "127.0.0.1:8563"
    return rpc_address

def insert_log(data):
    if use_gui_m:
        log_text.insert(tk.END, str(data)+"\n")
        log_text.see(tk.END)
    else:
        print(data)
    
def try_import():
    if not isPyBleAvailable:
        messagebox.showinfo("注意", "请先执行pip install bleak!")
        sys.exit(0)

def use_gui():
    global log_text,start_button,stop_button,rpc_entry
    root = tk.Tk()
    root.title("BLE RPC Server")
    root.geometry("400x300")

    # RPC Address
    rpc_frame = tk.Frame(root)
    rpc_frame.pack(pady=10)

    rpc_label = tk.Label(rpc_frame, text="RPC Address:    http://")
    rpc_label.pack(side=tk.LEFT, padx=5)

    rpc_entry = tk.Entry(rpc_frame, width=30)
    rpc_entry.insert(0, "127.0.0.1:8563")
    rpc_entry.pack(side=tk.LEFT, padx=5)

    # Start and Stop Buttons
    button_frame = tk.Frame(root)
    button_frame.pack(pady=10)

    start_button = tk.Button(button_frame, text="启动RPC服务", command=lambda: start_rpc_server(get_rpc_address()))
    start_button.pack(side=tk.LEFT, padx=5)

    stop_button = tk.Button(button_frame, text="停止RPC服务", command=stop_rpc_server)
    stop_button.pack(side=tk.LEFT, padx=5)

    stop_button.config(state='disabled')

    log_text = scrolledtext.ScrolledText(root, width=40, height=10)
    log_text.pack(pady=10)

    root.after(200,try_import)

    root.mainloop()

def use_cli(rpc_address,rpc_port):
    if not isPyBleAvailable:
        insert_log("请先执行pip install simplepyble安装所需蓝牙库！")
        return
    loop = asyncio.get_event_loop()
    server = SimpleXMLRPCServer((rpc_address, rpc_port), allow_none=True,logRequests=False)
    server.register_function(GetBleAvailability, "GetBleAvailability")
    server.register_function(lambda: loop.run_until_complete(ScanForShx()), "ScanForShx")
    server.register_function(lambda: loop.run_until_complete(ConnectShxDevice()), "ConnectShxDevice")
    server.register_function(lambda: loop.run_until_complete(ConnectShxRwService()), "ConnectShxRwService")
    server.register_function(lambda: loop.run_until_complete(ConnectShxRwCharacteristic()), "ConnectShxRwCharacteristic")
    server.register_function(ReadCachedData, "ReadCachedData")
    server.register_function(SetDevice, "SetDevice")
    server.register_function(lambda data: loop.run_until_complete(WriteData(data)), "WriteData")
    server.register_function(lambda : loop.run_until_complete(DisposeBluetooth), "DisposeBluetooth")
    server.serve_forever()
    
if __name__ == "__main__":
    opts,args = getopt.getopt(sys.argv[1:],'hcga:p:',['help','cli','gui','rpc-address=','rpc-port='])
    if len(opts)==0:
        if sys.platform=="darwin":
            use_gui_m = False
            use_cli(rpc_address,int(rpc_port))
        else:
            use_gui_m = True
            use_gui()
        sys.exit(0)
    for opt_name,opt_value in opts:
        if opt_name in ('-h','--help'):
            insert_log("""
Python BLE RPC Server
-a/--rpc-address 指定绑定的ip地址(命令行)
-p/--rpc-port 指定绑定的端口(命令行)
-g/--gui 使用图形模式（macOS上有bug）
-c/--cli 使用命令行模式
-h/--help 帮助
            """)
            sys.exit(0)
        if opt_name in ('-a','--rpc-address'):
            rpc_address = opt_value
        if opt_name in ('-p','--rpc-port'):
            rpc_port = opt_value
        if opt_name in ('-c','--cli'):
            use_gui_m = False
        if opt_name in ('-g','--gui'):
            use_gui_m = True
    if use_gui_m:
        use_gui()
    else:
        insert_log("----------Use cli-----------")
        use_cli(rpc_address,int(rpc_port))