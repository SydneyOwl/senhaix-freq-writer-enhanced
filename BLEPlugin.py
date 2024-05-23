import simplepyble
from xmlrpc.server import SimpleXMLRPCServer
import queue

"""
From SenhaixFreqWriter->Const
For gt12 and shx8800
"""
RwServiceUuid = "ffe0"
RwCharacteristicUuid = "ffe1"
BtnameShx8800 = "walkie-talkie"

peripherals = None
peripheral = None
service_uuid = None
service = None
characteristic_uuid = None
characteristic = None

mtu = 23

dataQueue = queue.Queue()

def GetBleAvailability():
    global adapter
    adapters = simplepyble.Adapter.get_adapters()
    if len(adapters)==0:
        return False
    adapter = adapters[0]
    print(f"Selected adapter: {adapter.identifier()} [{adapter.address()}]")
    adapter.set_callback_on_scan_start(lambda: print("Scan started."))
    adapter.set_callback_on_scan_stop(lambda: print("Scan complete."))
    adapter.set_callback_on_scan_found(lambda peripheral: print(f"Found {peripheral.identifier()} [{peripheral.address()}]"))
    return True

def ScanForShx():
    global peripherals
    adapter.scan_for(5000)
    peripherals = adapter.scan_get_results()
    devList = []
    for i, per in enumerate(peripherals):
        devList.append({"DeviceName":per.identifier(),"DeviceMacAddr":per.address()})
    print(devList)
    return str(devList)

def setDevice(seq):
    global peripheral
    peripheral = peripherals[seq]
    print(seq)

def ConnectShxDevice():
    global mtu
    if peripheral is None:
        return False
    try:
        print(f"Connecting to: {peripheral.identifier()} [{peripheral.address()}]")
        peripheral.connect()
        # mtu = peripheral.mtu()
        print("Successfully connected")
        return True
    except Exception as e:
        print(repr(e))
        return False
    
def ConnectShxRwService():
    global service_uuid, service
    if peripheral is None: return False
    services = peripheral.services()
    for serviced in services:
        serid = serviced.uuid()
        print(f"Service->{serid}")
        if RwServiceUuid in str(serid):
            service_uuid = serid
            service = serviced
            return True
    return False

def ConnectShxRwCharacteristic():
    global characteristic_uuid,characteristic
    if service is None: return False
    for characteristicd in service.characteristics():
        chrid = characteristicd.uuid()
        print(f"characteristic->{chrid}")
        if RwCharacteristicUuid in str(chrid):
            characteristic_uuid = chrid
            characteristic = characteristicd
            # Register Callback here
            print("Registering notify...")
            peripheral.notify(service_uuid, characteristic_uuid, CallbackOnDataReceived)
            return True
    return False 

def ReadCachedData():
    try:
        return dataQueue.get_nowait()
    except Exception as e:
        print(repr(e))
        return None

def WriteData(data:bytes):
    try:
        peripheral.write_request(service_uuid, characteristic_uuid, data)
        return True
    except Exception as e:
        print(repr(e))
        return False

def DisposeBluetooth():
    global peripheral,service_uuid,service,characteristic_uuid,characteristic
    peripheral.disconnect()
    service_uuid,service,characteristic_uuid,characteristic = None, None, None, None


def CallbackOnDataReceived(data:bytes):
    print(f"recv: {data}")
    dataQueue.put(data)
    
# print(f"BLEAvailability: {GetBleAvailability()}")
# print(f"SHXFound: {ScanForShx()}")
# print(f"ConnShx: {ConnectShxDevice()}")
# print(f"ConnShxService: {ConnectShxRwService()}")
# print(f"ConnShxChar: {ConnectShxRwCharacteristic()}")

server = SimpleXMLRPCServer(("localhost",8563),allow_none=True)
server.register_function(GetBleAvailability,"GetBleAvailability")
server.register_function(ScanForShx,"ScanForShx")
server.register_function(ConnectShxDevice,"ConnectShxDevice")
server.register_function(ConnectShxRwService,"ConnectShxRwService")
server.register_function(ConnectShxRwCharacteristic,"ConnectShxRwCharacteristic")
server.register_function(ReadCachedData,"ReadCachedData")
server.register_function(setDevice,"setDevice")
server.register_function(WriteData,"WriteData")
server.register_function(DisposeBluetooth,"DisposeBluetooth")
server.serve_forever()




# Query the user to pick a service/characteristic pair
# print("Please select a service/characteristic pair:")
# for i, (service_uuid, characteristic) in enumerate(service_characteristic_pair):
#     print(f"{i}: {service_uuid} {characteristic}")

# choice = int(input("Enter choice: "))
# service_uuid, characteristic_uuid = service_characteristic_pair[choice]

# # Write the content to the characteristic
# contents = peripheral.read(service_uuid, characteristic_uuid)
# print(f"Contents: {contents}")

# peripheral.disconnect()
