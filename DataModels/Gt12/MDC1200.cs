using CommunityToolkit.Mvvm.ComponentModel;

namespace SenhaixFreqWriter.DataModels.Gt12;

public partial class MDC1200 : ObservableObject
{
    [ObservableProperty] private string callID = "";
    [ObservableProperty] private string group = "111";
    [ObservableProperty] private string id = "1111";
}