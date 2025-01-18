namespace SenhaixFreqWriter.Utils.Serial;

public class DataHelper
{
    private ushort _args;
    private byte _command;

    public byte[] Payload = new byte[68];

    public byte[] LoadPackage(byte cmd, ushort address, byte[] dat, byte len)
    {
        var array = new byte[68];
        for (var i = 0; i < 64; i++) array[4 + i] = 0;
        array[0] = cmd;
        array[1] = (byte)(address >> 8);
        array[2] = (byte)address;
        array[3] = len;
        for (var j = 0; j < len; j++) array[4 + j] = dat[j];
        return array;
    }
}