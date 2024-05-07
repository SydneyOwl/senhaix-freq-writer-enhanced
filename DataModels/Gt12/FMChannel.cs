using CommunityToolkit.Mvvm.ComponentModel;

namespace SenhaixFreqWriter.DataModels.Gt12;

public partial class FmChannel : ObservableObject
{
    [ObservableProperty] private int[] _channels = new int[15];
    [ObservableProperty] private int _curFreq = 904;
}

public partial class FmObject : ObservableObject
{
    [ObservableProperty] private int _id;
    [ObservableProperty] private string _freq = "";
}