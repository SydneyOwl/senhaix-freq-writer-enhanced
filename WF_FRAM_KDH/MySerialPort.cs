using System.IO.Ports;
#if NET461
using System.Linq;
using SQ5R.View;
#endif

namespace WF_FRAM_KDH;

internal class MySerialPort : SerialPort
{
#if NET461
    private readonly BleCore bleCore = BleCore.BleInstance();
#endif
    public int BytesToReadFromCache
    {
        get
        {
#if NET461
            if (bleCore.CurrentDevice != null) return bleCore.rxData.Count;
#endif
            return BytesToRead;
        }
    }

    public void CloseSerial()
    {
#if NET461
        if (bleCore.CurrentDevice == null)
        {
#endif
        Close();
#if NET461
        }
#endif
    }

    public void WriteByte(byte dat)
    {
#if NET461
        if (bleCore.CurrentDevice != null)
        {
            bleCore.Write(new byte[1] { dat });
            return;
        }
#endif
        Write(new byte[1] { dat }, 0, 1);
    }

    public void WriteByte(byte[] dat, int offset, int count)
    {
#if NET461
        if (bleCore.CurrentDevice != null)
        {
            var tobeWrite = dat.Skip(offset).Take(count).ToArray();
            bleCore.Write(tobeWrite);
            return;
        }
#endif
        Write(dat, offset, count);
    }

    public void ReadByte(byte[] buffer, int offset, int count)
    {
#if NET461
        if (bleCore.CurrentDevice != null)
        {
            var tmp = new byte[count];
            for (var z = 0; z < count; z++) tmp[z] = bleCore.rxData.Dequeue();

            tmp.CopyTo(buffer, 0);
            return;
        }
#endif
        Read(buffer, offset, count);
    }
}