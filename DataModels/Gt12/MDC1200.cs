using CommunityToolkit.Mvvm.ComponentModel;

namespace SenhaixFreqWriter.DataModels.Gt12;

public partial class Mdc1200 : ObservableObject
{
    [ObservableProperty] private string _callId = "";
    [ObservableProperty] private string _group = "111";
    [ObservableProperty] private string _id = "1111";
}