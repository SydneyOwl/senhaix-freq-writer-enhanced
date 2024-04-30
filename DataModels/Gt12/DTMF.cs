using CommunityToolkit.Mvvm.ComponentModel;

namespace SenhaixFreqWriter.DataModels.Gt12;

public partial class DTMF : ObservableObject
{
    [ObservableProperty]
    private string[] group = new string[20];
    [ObservableProperty]
    private string[] groupName = new string[20];
    [ObservableProperty]
    private int idleTime = 1;
    [ObservableProperty]
    private string localID = "100";
    [ObservableProperty]
    private int wordTime = 1;

    public DTMF()
    {
        Group[0] = "101";
        GroupName[0] = "成员1";
        Group[1] = "102";
        GroupName[1] = "成员2";
        Group[2] = "103";
        GroupName[2] = "成员3";
        Group[3] = "104";
        GroupName[3] = "成员4";
        Group[4] = "105";
        GroupName[4] = "成员5";
        Group[5] = "106";
        GroupName[5] = "成员6";
        Group[6] = "107";
        GroupName[6] = "成员7";
        Group[7] = "108";
        GroupName[7] = "成员8";
        Group[8] = "109";
        GroupName[8] = "成员9";
        Group[9] = "110";
        GroupName[9] = "成员10";
        Group[10] = "111";
        GroupName[10] = "成员11";
        Group[11] = "112";
        GroupName[11] = "成员12";
        Group[12] = "113";
        GroupName[12] = "成员13";
        Group[13] = "114";
        GroupName[13] = "成员14";
        Group[14] = "115";
        GroupName[14] = "成员15";
        Group[15] = "116";
        GroupName[15] = "成员16";
        Group[16] = "117";
        GroupName[16] = "成员17";
        Group[17] = "118";
        GroupName[17] = "成员18";
        Group[18] = "119";
        GroupName[18] = "成员19";
        Group[19] = "120";
        GroupName[19] = "成员20";
    }
}

public partial class DTMPObject : ObservableObject
{
    [ObservableProperty] private string id;
    [ObservableProperty] private string group;
    [ObservableProperty] private string groupName;
}