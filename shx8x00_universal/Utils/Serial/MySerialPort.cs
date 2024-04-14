using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using InTheHand.Bluetooth;

namespace shx8x00.Utils.Serial;

internal class MySerialPort : SerialPort
{
    public int BytesToReadFromCache
    {
        get
        {
            if (characteristic != null) return rxData.Count;
            return BytesToRead;
        }
    }
    
    private static MySerialPort sp;

    private GattCharacteristic characteristic = null;

    private int BTDeviceMTU = 23;

    public int BtDeviceMtu
    {
        get => BTDeviceMTU;
        set => BTDeviceMTU = value;
    }

    private Queue<byte> rxData = new(1024);

    public Queue<byte> RxData
    {
        get => rxData;
        set => rxData = value ?? throw new ArgumentNullException(nameof(value));
    }

    public async Task preRead()
    {
        // var data = await characteristic.ReadValueAsync();
        // Console.WriteLine("Now I Read");
        // foreach (var b in data)
        // {
        //     Console.WriteLine(b);
        // }
        // foreach (var b in data)
        // {
        //     rxData.Enqueue(b);
        // }
    }


    public GattCharacteristic Characteristic
    {
        get => characteristic;
        set => characteristic = value;
    }

    private string targetPort = "";

    public string TargetPort
    {
        get => targetPort;
        set => targetPort = value;
    }

    public async Task WriteByte(byte buffer)
    {
        if (characteristic == null)
        {
            Write(new byte[1] { buffer }, 0, 1);
        }
        else
        {
            await characteristic.WriteValueWithoutResponseAsync(new byte[1]{buffer});
        }
    }

    public async Task WriteByte(byte[] buffer, int offset, int count)
    {
        if (characteristic == null)
        {
            Write(buffer, offset, count);
        }
        else
        {
            // 太大的话要分开发
            var tobeWrite = buffer.Skip(offset).Take(count).ToArray();
            var singleSize = (BTDeviceMTU - 5);
            var sendTimes = tobeWrite.Length / singleSize;
            var tmp = 0;
            for (int i = 0; i < sendTimes+1; i++)
            {
                if (i == sendTimes)
                {
                    await characteristic.WriteValueWithoutResponseAsync(tobeWrite.Skip(tmp).Take(tobeWrite.Length-sendTimes*singleSize).ToArray());
                    break;
                }
                await characteristic.WriteValueWithoutResponseAsync(tobeWrite.Skip(tmp).Take(singleSize).ToArray());
                tmp += singleSize;
            }
            // Console.WriteLine(tobeWrite.Length);
            // await characteristic.WriteValueWithoutResponseAsync(tobeWrite);
        }
    }

    public async Task ReadByte(byte[] buffer, int offset, int count)
    {
        if (characteristic == null)
        {
            Read(buffer, offset, count);
        }
        else
        {
            var tmp = new byte[count];
            for (var z = 0; z < count; z++) tmp[z] = rxData.Dequeue();
            tmp.CopyTo(buffer, 0);
        }
    }
    public static MySerialPort getInstance()
    {
        if (sp == null) sp = new MySerialPort();

        return sp;
    }

    public void OpenSerial()
    {
        if (characteristic == null)
        {
            sp.PortName = targetPort;
            sp.BaudRate = 9600;
            sp.DataBits = 8;
            sp.Parity = Parity.None;
            sp.StopBits = StopBits.One;
            sp.WriteBufferSize = 1024;
            sp.ReadBufferSize = 1024;
            sp.ReadTimeout = 4000;
            sp.WriteTimeout = 4000;
            sp.Open();
        }
    }

    public void CloseSerial()
    {
        if (characteristic == null)
        {
            var portTmp = sp.targetPort;
            if (sp != null && sp.IsOpen) Close();
            sp = new MySerialPort();
            sp.targetPort = portTmp;
        }
    }
}