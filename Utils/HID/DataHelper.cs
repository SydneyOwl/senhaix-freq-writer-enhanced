using SenhaixFreqWriter.Constants.Gt12;

namespace SenhaixFreqWriter.Utils.HID;

public class DataHelper
{
    private ushort _args;

    public byte Command;

    private ushort _crc;

    public HidErrors ErrorCode;
    
    private byte _header = 170;

    private byte _lenOfPackage;

    public byte[] Payload = new byte[56];

    public byte[] LoadPackage(byte cmd, ushort args, byte[] dat, byte len)
    {
        var array = new byte[64];
        var num = 0;
        if (dat == null) len = 1;

        array[0] = 1;
        array[1] = (byte)(3 + len + 2);
        array[2] = cmd;
        array[3] = (byte)(args >> 8);
        array[4] = (byte)args;
        if (dat != null)
        {
            for (var i = 0; i < len; i++) array[5 + i] = dat[i];

            num = 5 + len;
        }
        else
        {
            array[5] = 0;
            num = 6;
        }

        _crc = (ushort)CrcValidation(array, 2, array[1] - 2);
        array[num] = (byte)(_crc >> 8);
        array[num + 1] = (byte)_crc;
        return array;
    }
    
    public byte[] LoadImgDataPackage(byte cmd, ushort args, byte[] dat, byte len)
    {
        byte[] array = new byte[64];
        if (dat == null)
        {
            len = 1;
        }
        array[0] = 1;
        array[1] = (byte)(3 + len + 2);
        array[2] = cmd;
        array[3] = (byte)(args >> 8);
        array[4] = (byte)args;
        if (dat != null)
        {
            for (int i = 0; i < len; i++)
            {
                array[5 + i] = dat[i];
            }
        }
        else
        {
            array[5] = 0;
        }
        return array;
    }

    public int AnalyzePackage(byte[] dat)
    {
        try
        {
            _lenOfPackage = dat[1];
            Command = dat[2];
            _args = (ushort)((dat[3] << 8) | dat[4]);
            ErrorCode = (HidErrors)_args;
            for (var i = 0; i < _lenOfPackage - 5; i++) Payload[i] = dat[i + 5];

            var num = 2 + _lenOfPackage - 2;
            _crc = (ushort)CrcValidation(dat, 2, _lenOfPackage - 2);
            var num2 = (ushort)((dat[num] << 8) | dat[num + 1]);
            if (_crc == num2) return 1;
            return -1;
        }
        catch
        {
            // 写频完成后手台还会发几个包，。不知道为啥
            return -1;
        }
    }

    public static int CrcValidation(byte[] dat, int offset, int count)
    {
        var num = 0;
        for (var i = 0; i < count; i++)
        {
            int num2 = dat[i + offset];
            num ^= num2 << 8;
            for (var j = 0; j < 8; j++) num = (num & 0x8000) != 32768 ? num << 1 : (num << 1) ^ 0x1021;
        }

        return num;
    }
}