using SenhaixFreqWriter.Constants.Gt12;

namespace SenhaixFreqWriter.Utils.HID;

public class DataHelper
{
    private ushort args;

    private byte command;

    private ushort crc;

    public HID_ERRORS errorCode;
    private byte header = 170;

    private byte lenOfPackage;

    public byte[] payload = new byte[56];

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

        crc = (ushort)CrcValidation(array, 2, array[1] - 2);
        array[num] = (byte)(crc >> 8);
        array[num + 1] = (byte)crc;
        return array;
    }

    public int AnalyzePackage(byte[] dat)
    {
        try
        {
            lenOfPackage = dat[1];
            command = dat[2];
            args = (ushort)((dat[3] << 8) | dat[4]);
            errorCode = (HID_ERRORS)args;
            for (var i = 0; i < lenOfPackage - 5; i++) payload[i] = dat[i + 5];

            var num = 2 + lenOfPackage - 2;
            crc = (ushort)CrcValidation(dat, 2, lenOfPackage - 2);
            var num2 = (ushort)((dat[num] << 8) | dat[num + 1]);
            if (crc == num2) return 1;
            return -1;
        }
        catch
        {
            // 写频完成后手台还会发几个包，。不知道为啥
            return -1;
        }
        

    }

    private int CrcValidation(byte[] dat, int offset, int count)
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