using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Threading.Tasks;
using SenhaixFreqWriter.Views.Common;

namespace SenhaixFreqWriter.Utils.Serial;

public class MySerialPort : SerialPort
{
    public delegate Task WriteValueAsync(byte[] value);

    private static MySerialPort _sp;

    private Queue<byte> _rxData = new(1024);

    public WriteValueAsync WriteBle;

    public int BtDeviceMtu { get; set; } = 23;

    private void UpdateDebugInfo(string a)
    {
        DebugWindow.GetInstance().updateDebugContent(a);
    }

    public int BytesToReadFromCache
    {
        get
        {
            if (WriteBle != null) return _rxData.Count;
            return BytesToRead;
        }
    }


    public Queue<byte> RxData
    {
        get => _rxData;
        set => _rxData = value ?? throw new ArgumentNullException(nameof(value));
    }


    public string TargetPort { get; set; } = "";

    public async Task PreRead()
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
        if (WriteBle == null)
        {
            UpdateDebugInfo($"发送数据（长度1，使用串口）：{buffer}");
            Write(new byte[1] { buffer }, 0, 1);
        }
        else
        {
            UpdateDebugInfo($"发送数据（长度1，使用蓝牙）：{buffer}");
            await WriteBle(new byte[1] { buffer });
        }
    }

    public async Task WriteByte(byte[] buffer, int offset, int count)
    {
        if (WriteBle == null)
        {
            UpdateDebugInfo($"发送数据（长度{buffer.Length}，使用串口）：{BitConverter.ToString(buffer)}");
            Write(buffer, offset, count);
        }
        else
        {
            // 太大的话要分开发
            var tobeWrite = buffer.Skip(offset).Take(count).ToArray();
            var singleSize = BtDeviceMtu - 5;
            var sendTimes = tobeWrite.Length / singleSize;
            var tmp = 0;
            for (var i = 0; i < sendTimes + 1; i++)
            {
                if (i == sendTimes)
                {
                    var tmpData = tobeWrite.Skip(tmp)
                        .Take(tobeWrite.Length - sendTimes * singleSize).ToArray();
                    UpdateDebugInfo($"发送数据（长度{tmpData.Length}，使用蓝牙）：{BitConverter.ToString(tmpData)}");
                    await WriteBle(tmpData);
                    break;
                }
                
                var tmpData1 = tobeWrite.Skip(tmp).Take(singleSize).ToArray();
                UpdateDebugInfo($"发送数据（长度{tmpData1.Length}，使用蓝牙）：{BitConverter.ToString(tmpData1)}");
                await WriteBle(tmpData1);
                tmp += singleSize;
            }
            // Console.WriteLine(tobeWrite.Length);
            // await characteristic.WriteValueWithoutResponseAsync(tobeWrite);
        }
    }

    public async Task ReadByte(byte[] buffer, int offset, int count)
    {
        if (WriteBle == null)
        {
            UpdateDebugInfo($"收到数据（串口，长度{buffer.Length}）：{BitConverter.ToString(buffer)}");
            Read(buffer, offset, count);
        }
        else
        {
            var tmp = new byte[count];
            for (var z = 0; z < count; z++) tmp[z] = _rxData.Dequeue();
            UpdateDebugInfo($"收到数据（蓝牙，长度{tmp.Length}）：{BitConverter.ToString(tmp)}");
            tmp.CopyTo(buffer, 0);
        }
    }

    public static MySerialPort GetInstance()
    {
        if (_sp == null) _sp = new MySerialPort();

        return _sp;
    }

    public void OpenSerial()
    {
        if (WriteBle == null)
        {
            _sp.PortName = TargetPort;
            _sp.BaudRate = 9600;
            _sp.DataBits = 8;
            _sp.Parity = Parity.None;
            _sp.StopBits = StopBits.One;
            _sp.WriteBufferSize = 1024;
            _sp.ReadBufferSize = 1024;
            _sp.ReadTimeout = 4000;
            _sp.WriteTimeout = 4000;
            _sp.Open();
        }
    }

    public void CloseSerial()
    {
        if (WriteBle == null)
        {
            var portTmp = _sp.TargetPort;
            if (_sp != null && _sp.IsOpen) Close();
            _sp = new MySerialPort();
            _sp.TargetPort = portTmp;
        }
    }
}