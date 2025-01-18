using CommunityToolkit.Mvvm.ComponentModel;

namespace SenhaixFreqWriter.DataModels.Shx8800Pro;

public partial class Dtmf : ObservableObject
{
    [ObservableProperty] private string[] _group = new string[15];
    [ObservableProperty] private string[] _groupName = new string[15]; //seems useless...
    [ObservableProperty] private int _idleTime = 1;
    [ObservableProperty] private string _localId = "100";
    [ObservableProperty] private int _pttid;
    [ObservableProperty] private int _wordTime = 1;

    public Dtmf()
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
    }
}

public partial class DtmpObject : ObservableObject
{
    [ObservableProperty] private string _group;
    [ObservableProperty] private string _groupName;
    [ObservableProperty] private string _id;
}