namespace SHX_GT12_CPS.Properties;

public class appData
{
    private Channel[][] channelList = new Channel[10][];

    private Function function = new Function();

    private DTMF dtmf = new DTMF();

    private FMChannel fmChannels = new FMChannel();

    public appData()
    {
        for (int i = 0; i < 10; i++)
        {
            for (int j = 0; j < 100; j++)
            {
                channelList[i][j] = new Channel();
            }
        }
    }
}