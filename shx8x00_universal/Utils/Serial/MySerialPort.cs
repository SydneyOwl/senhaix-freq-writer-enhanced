using System;
using System.IO.Ports;

namespace shx8x00.Utils.Serial;

internal class MySerialPort : SerialPort
{
    private static MySerialPort sp;

    private string targetPort = "";

    public string TargetPort
    {
        get => targetPort;
        set => targetPort = value ?? throw new ArgumentNullException(nameof(value));
    }

    public void WriteByte(byte dat)
    {
        Write(new byte[1] { dat }, 0, 1);
    }

    public void WriteByte(byte[] buffer, int offset, int count)
    {
        Write(buffer, offset, count);
    }

    public void ReadByte(byte[] buffer, int offset, int count)
    {
        Read(buffer, offset, count);
    }
    public static MySerialPort getInstance()
    {
        if (sp == null) sp = new MySerialPort();

        return sp;
    }

    public void OpenSerial()
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

    public void CloseSerial()
    {
        var portTmp = sp.targetPort;
        if (sp != null && sp.IsOpen) Close();
        sp = new MySerialPort();
        sp.targetPort = portTmp;
    }
}