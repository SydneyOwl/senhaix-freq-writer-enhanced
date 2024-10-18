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
    FramHeader = 165,
    CmdWrite = 87,
    CmdHandshake = 2,
    CmdSetaddress = 3,
    CmdErase = 4,
    CmdOver = 6
}