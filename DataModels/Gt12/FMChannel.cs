using CommunityToolkit.Mvvm.ComponentModel;

namespace SenhaixFreqWriter.DataModels.Gt12;

public partial class FMChannel : ObservableObject
{
    [ObservableProperty] private int[] channels = new int[15];
    [ObservableProperty] private int curFreq = 904;
}

public partial class FMObject : ObservableObject
{
    [ObservableProperty] private int id;
    [ObservableProperty] private string freq = "";
}