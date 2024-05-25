from xmlrpc.server import SimpleXMLRPCServer
import queue
import tkinter as tk
from tkinter import messagebox
from tkinter import scrolledtext
import threading
import sys
import getopt

"""
From SenhaixFreqWriter->Const
For gt12 and shx8800
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

dataQueue = queue.Queue()

isPyBleAvailable = True

try: 
    import simplepyble
except Exception:
    isPyBleAvailable = False

def GetBleAvailability():
    global adapter
    adapters = simplepyble.Adapter.get_adapters()
    if len(adapters) == 0:
        return False
    adapter = adapters[0]
    insert_log(f"Selected adapter: {adapter.identifier()} [{adapter.address()}]")
    adapter.set_callback_on_scan_start(lambda: insert_log("Scan started."))
    adapter.set_callback_on_scan_stop(lambda: insert_log("Scan complete."))
    adapter.set_callback_on_scan_found(lambda peripheral: insert_log(f"Found {peripheral.identifier()} [{peripheral.address()}]"))
    return True

def ScanForShx():
    global peripherals
    adapter.scan_for(5000)
    peripherals = adapter.scan_get_results()
    devList = []
    for i, per in enumerate(peripherals):
        devList.append({"DeviceName": per.identifier(), "DeviceMacAddr": per.address(),"DeviceID":str(i)})
    insert_log(devList)
    return str(devList)

def SetDevice(seq):
    global peripheral
    peripheral = peripherals[int(seq)]

def ConnectShxDevice():
    global mtu,dataQueue
    while not dataQueue.empty():
        dataQueue.get()
    DisposeBluetooth()
    if peripheral is None:
        return False
    try:
        insert_log(f"Connecting to: {peripheral.identifier()} [{peripheral.address()}]")
        peripheral.connect()
        # mtu = peripheral.mtu()
        insert_log("Successfully connected")
        return True
    except Exception as e:
        insert_log(repr(e))
        return False
    
def ConnectShxRwService():
    global service_uuid, service
    if peripheral is None: return False
    services = peripheral.services()
    for serviced in services:
        serid = serviced.uuid()
        if RwServiceUuid in str(serid):
            service_uuid = serid
            service = serviced
            insert_log(f"Service Connected")
            return True
    return False

def ConnectShxRwCharacteristic():
    global characteristic_uuid, characteristic
    if service is None: return False
    for characteristicd in service.characteristics():
        chrid = characteristicd.uuid()
        if RwCharacteristicUuid in str(chrid):
            characteristic_uuid = chrid
            characteristic = characteristicd
            # Register Callback here
            insert_log("Registering notify...")
            peripheral.notify(service_uuid, characteristic_uuid, CallbackOnDataReceived)
            insert_log("Registeredd notify")
            return True
    return False 

def ReadCachedData():
    try:
        if(dataQueue.empty()):
            return None
        return dataQueue.get()
    except Exception as e:
        insert_log(repr(e))
        return None

def WriteData(data):
    try:
        peripheral.write_command(service_uuid, characteristic_uuid, data.data)
        return True
    except Exception as e:
        insert_log(repr(e))
        return False

def DisposeBluetooth():
    global peripheral,service_uuid,service,characteristic_uuid,characteristic
    if peripheral is not None:
        peripheral.disconnect()
    service_uuid,service,characteristic_uuid,characteristic = None, None, None, None

def CallbackOnDataReceived(data:bytes):
    insert_log(f"recv: {data}")
    dataQueue.put(data)


def start_rpc_server(rpc_address):
    global server
    try:
        server = SimpleXMLRPCServer((rpc_address.split(':')[0], int(rpc_address.split(':')[1])), allow_none=True,logRequests=False)
        server.register_function(GetBleAvailability, "GetBleAvailability")
        server.register_function(ScanForShx, "ScanForShx")
        server.register_function(ConnectShxDevice, "ConnectShxDevice")
        server.register_function(ConnectShxRwService, "ConnectShxRwService")
        server.register_function(ConnectShxRwCharacteristic, "ConnectShxRwCharacteristic")
        server.register_function(ReadCachedData, "ReadCachedData")
        server.register_function(SetDevice, "SetDevice")
        server.register_function(WriteData, "WriteData")
        server.register_function(DisposeBluetooth, "DisposeBluetooth")
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
        messagebox.showinfo("注意", "请先执行pip install simplepyble!")
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
        print("请先执行pip install simplepyble安装所需蓝牙库！")
        return
    server = SimpleXMLRPCServer((rpc_address, rpc_port), allow_none=True,logRequests=False)
    server.register_function(GetBleAvailability, "GetBleAvailability")
    server.register_function(ScanForShx, "ScanForShx")
    server.register_function(ConnectShxDevice, "ConnectShxDevice")
    server.register_function(ConnectShxRwService, "ConnectShxRwService")
    server.register_function(ConnectShxRwCharacteristic, "ConnectShxRwCharacteristic")
    server.register_function(ReadCachedData, "ReadCachedData")
    server.register_function(SetDevice, "SetDevice")
    server.register_function(WriteData, "WriteData")
    server.register_function(DisposeBluetooth, "DisposeBluetooth")
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
            print("""
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
        print("----------Use cli-----------")
        use_cli(rpc_address,int(rpc_port))