using System.IO.Ports;
#if NET462
using System.Linq;
using SQ5R.View;
#endif

namespace WF_FRAM_KDH;

internal class MySerialPort : SerialPort
{
#if NET462
    private BleCore bleCore = BleCore.BleInstance();
#endif


    public int BytesToReadFromCache
    {
        get
        {
#if NET462
            if (bleCore.CurrentDevice != null) return bleCore.rxData.Count;
#endif
            return BytesToRead;
        }
    }

    public void OpenSerial()
    {
#if NET462
        bleCore = BleCore.BleInstance();
        if (bleCore.CurrentDevice != null) return;
#endif
        Open();
    }

    public void CloseSerial()
    {
#if NET462
        if (bleCore.CurrentDevice != null) return;
        // bleCore.Dispose();
#endif
        Close();
    }

    public void WriteByte(byte dat)
    {
#if NET462
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
#if NET462
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
#if NET462
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