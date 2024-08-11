using CommunityToolkit.Mvvm.ComponentModel;

namespace SenhaixFreqWriter.DataModels.Shx8800Pro;

public partial class FmChannel : ObservableObject
{
    [ObservableProperty] private int[] _channels = new int[15];
    [ObservableProperty] private int _curFreq = 904;
}

public partial class FmObject : ObservableObject
{
    [ObservableProperty] private string _freq = "";
    [ObservableProperty] private int _id;
}