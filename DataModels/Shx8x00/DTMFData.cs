using System;
using CommunityToolkit.Mvvm.ComponentModel;

namespace SenhaixFreqWriter.DataModels.Shx8x00;

[Serializable]
public partial class DtmfData : ObservableObject
{
    [ObservableProperty] private int _groupCall = 14;

    [ObservableProperty] private string _groupOfDtmf1 = "20202";

    [ObservableProperty] private string _groupOfDtmf2 = "";

    [ObservableProperty] private string _groupOfDtmf3 = "";

    [ObservableProperty] private string _groupOfDtmf4 = "";

    [ObservableProperty] private string _groupOfDtmf5 = "";

    [ObservableProperty] private string _groupOfDtmf6 = "";

    [ObservableProperty] private string _groupOfDtmf7 = "";

    [ObservableProperty] private string _groupOfDtmf8 = "";

    [ObservableProperty] private string _groupOfDtmf9 = "";

    [ObservableProperty] private string _groupOfDtmfA = "";

    [ObservableProperty] private string _groupOfDtmfB = "";

    [ObservableProperty] private string _groupOfDtmfC = "";

    [ObservableProperty] private string _groupOfDtmfD = "";

    [ObservableProperty] private string _groupOfDtmfE = "";

    [ObservableProperty] private string _groupOfDtmfF = "30303";

    [ObservableProperty] private int _lastTimeSend = 1;

    [ObservableProperty] private int _lastTimeStop = 1;

    [ObservableProperty] private bool _sendOnPttPressed;

    [ObservableProperty] private bool _sendOnPttReleased = true;

    [ObservableProperty] private string _theIdOfLocalHost = "80808";
}