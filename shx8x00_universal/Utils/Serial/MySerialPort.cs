using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Threading.Tasks;

namespace shx8x00.Utils.Serial;

public class MySerialPort : SerialPort
{
    private static MySerialPort sp;

    private Queue<byte> rxData = new(1024);
    
    private int btDeviceMtu  = 23;
    public delegate Task WriteValueAsync(byte[] value);

    public WriteValueAsync WriteBLE;


    public int BTDeviceMtu
    {
        get => btDeviceMtu;
        set => btDeviceMtu = value;
    }


    public int BytesToReadFromCache
    {
        get
        {
            if (WriteBLE != null) return rxData.Count;
            return BytesToRead;
        }
    }


    public Queue<byte> RxData
    {
        get => rxData;
        set => rxData = value ?? throw new ArgumentNullException(nameof(value));
    }
    

    public string TargetPort { get; set; } = "";

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

    public async Task WriteByte(byte buffer)
    {
        if (WriteBLE == null)
            Write(new byte[1] { buffer }, 0, 1);
        else
            await WriteBLE(new byte[1] { buffer });
    }

    public async Task WriteByte(byte[] buffer, int offset, int count)
    {
        if (WriteBLE == null)
        {
            Write(buffer, offset, count);
        }
        else
        {
            // 太大的话要分开发
            var tobeWrite = buffer.Skip(offset).Take(count).ToArray();
            var singleSize = BTDeviceMtu - 5;
            var sendTimes = tobeWrite.Length / singleSize;
            var tmp = 0;
            for (var i = 0; i < sendTimes + 1; i++)
            {
                if (i == sendTimes)
                {
                    await WriteBLE(tobeWrite.Skip(tmp)
                        .Take(tobeWrite.Length - sendTimes * singleSize).ToArray());
                    break;
                }

                await WriteBLE(tobeWrite.Skip(tmp).Take(singleSize).ToArray());
                tmp += singleSize;
            }
            // Console.WriteLine(tobeWrite.Length);
            // await characteristic.WriteValueWithoutResponseAsync(tobeWrite);
        }
    }

    public async Task ReadByte(byte[] buffer, int offset, int count)
    {
        if (WriteBLE == null)
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
        if (WriteBLE == null)
        {
            sp.PortName = TargetPort;
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
        if (WriteBLE == null)
        {
            var portTmp = sp.TargetPort;
            if (sp != null && sp.IsOpen) Close();
            sp = new MySerialPort();
            sp.TargetPort = portTmp;
        }
    }
}