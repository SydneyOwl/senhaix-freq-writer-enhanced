using System;
using CommunityToolkit.Mvvm.ComponentModel;

namespace shx8x00.DataModels;

[Serializable]
public partial class DTMFData : ObservableObject
{
    [ObservableProperty]
    private int groupCall = 14;
    [ObservableProperty]
    private string groupOfDTMF_1 = "20202";
    [ObservableProperty]
    private string groupOfDTMF_2 = "";
    [ObservableProperty]
    private string groupOfDTMF_3 = "";
    [ObservableProperty]
    private string groupOfDTMF_4 = "";
    [ObservableProperty]
    private string groupOfDTMF_5 = "";
    [ObservableProperty]
    private string groupOfDTMF_6 = "";
    [ObservableProperty]
    private string groupOfDTMF_7 = "";
    [ObservableProperty]
    private string groupOfDTMF_8 = "";
    [ObservableProperty]
    private string groupOfDTMF_9 = "";
    [ObservableProperty]
    private string groupOfDTMF_A = "";
    [ObservableProperty]
    private string groupOfDTMF_B = "";
    [ObservableProperty]
    private string groupOfDTMF_C = "";
    [ObservableProperty]
    private string groupOfDTMF_D = "";
    [ObservableProperty]
    private string groupOfDTMF_E = "";
    [ObservableProperty]
    private string groupOfDTMF_F = "30303";
    [ObservableProperty]
    private int lastTimeSend = 1;
    [ObservableProperty]
    private int lastTimeStop = 1;
    [ObservableProperty]
    private bool sendOnPTTPressed = false;
    [ObservableProperty]
    private bool sendOnPTTReleased = true;
    [ObservableProperty]
    private string theIDOfLocalHost = "80808";
    
}