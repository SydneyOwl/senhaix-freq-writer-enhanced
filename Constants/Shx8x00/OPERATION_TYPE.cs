namespace SenhaixFreqWriter.Constants.Shx8x00;

public enum OperationType
{
    Read,
    Write,
    ReadConfig,
    WriteConfig
}

// 新版8600写开机画面
public enum NCommandType
{
    FRAM_HEADER = 165,
    CMD_WRITE = 87,
    CMD_HANDSHAKE = 2,
    CMD_SETADDRESS = 3,
    CMD_ERASE = 4,
    CMD_OVER = 6
}
