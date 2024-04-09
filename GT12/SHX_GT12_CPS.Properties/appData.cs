namespace SHX_GT12_CPS.Properties;

public class appData
{
    private readonly Channel[][] channelList = new Channel[10][];

    private DTMF dtmf = new();

    private FMChannel fmChannels = new();

    private Function function = new();

    public appData()
    {
        for (var i = 0; i < 10; i++)
        for (var j = 0; j < 100; j++)
            channelList[i][j] = new Channel();
    }
}