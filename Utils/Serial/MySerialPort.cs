using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Threading.Tasks;
using SenhaixFreqWriter.Properties;
using SenhaixFreqWriter.Views.Common;

namespace SenhaixFreqWriter.Utils.Serial;

public class MySerialPort : SerialPort
{
    public delegate void WriteValueAsync(byte[] value);

    private static MySerialPort _sp;

    private Queue<byte> _rxData = new(1024);

    public WriteValueAsync WriteBle;

    public int BtDeviceMtu { get; set; } = 23;

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

    private void UpdateChanDebugInfo(string a)
    {
        if (!SETTINGS.DISABLE_DEBUG_CHAN_DATA_OUTPUT) DebugWindow.GetInstance().updateDebugContent(a);
    }

    public async Task PreRead()
    {
        // var data = await characteristic.ReadValueAsync();
        // DebugWindow.GetInstance().updateDebugContent("Now I Read");
        // foreach (var b in data)
        // {
        //     DebugWindow.GetInstance().updateDebugContent(b);
        // }
        // foreach (var b in data)
        // {
        //     rxData.Enqueue(b);
        // }
    }

    public void WriteByte(byte buffer)
    {
        if (WriteBle == null)
        {
            UpdateChanDebugInfo($"发送数据（长度1，使用串口）：{buffer}");
            Write(new byte[1] { buffer }, 0, 1);
        }
        else
        {
            UpdateChanDebugInfo($"发送数据（长度1，使用蓝牙）：{buffer}");
            WriteBle(new byte[1] { buffer });
        }
    }

    public void WriteByte(byte[] buffer, int offset, int count)
    {
        if (WriteBle == null)
        {
            UpdateChanDebugInfo($"发送数据（长度{buffer.Length}，使用串口）：{BitConverter.ToString(buffer)}");
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
                    UpdateChanDebugInfo($"发送数据（长度{tmpData.Length}，使用蓝牙）：{BitConverter.ToString(tmpData)}");
                    WriteBle(tmpData);
                    break;
                }

                var tmpData1 = tobeWrite.Skip(tmp).Take(singleSize).ToArray();
                UpdateChanDebugInfo($"发送数据（长度{tmpData1.Length}，使用蓝牙）：{BitConverter.ToString(tmpData1)}");
                WriteBle(tmpData1);
                tmp += singleSize;
            }
            // DebugWindow.GetInstance().updateDebugContent(tobeWrite.Length);
            // await characteristic.WriteValueWithoutResponseAsync(tobeWrite);
        }
    }

    public void ReadByte(byte[] buffer, int offset, int count)
    {
        if (WriteBle == null)
        {
            UpdateChanDebugInfo($"收到数据（串口，长度{buffer.Length}）：{BitConverter.ToString(buffer)}");
            Read(buffer, offset, count);
        }
        else
        {
            var tmp = new byte[count];
            for (var z = 0; z < count; z++) tmp[z] = _rxData.Dequeue();
            UpdateChanDebugInfo($"收到数据（蓝牙，长度{tmp.Length}）：{BitConverter.ToString(tmp)}");
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
    
    
    // 8600pro的串口
    public void OpenSerialPro()
    {
        _sp.PortName = TargetPort;
        _sp.BaudRate = 9600;
        _sp.DataBits = 8;
        _sp.StopBits = StopBits.One;
        _sp.Parity = Parity.None;
        _sp.ReadBufferSize = 10240;
        _sp.WriteBufferSize = 10240;
        _sp.DtrEnable = true;
        _sp.RtsEnable = true;
        _sp.ReadTimeout = 4000;
        _sp.WriteTimeout = 4000;
        _sp.Open();
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