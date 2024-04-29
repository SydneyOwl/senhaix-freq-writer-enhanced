using System;
using CommunityToolkit.Mvvm.ComponentModel;

namespace SenhaixFreqWriter.DataModels.Shx8x00;


//Notice: 请勿随意变更本文件参数！
[Serializable]
public partial  class OtherImfData : ObservableObject
{[ObservableProperty] 
    private bool enableTxOver480M;
    [ObservableProperty] 
    private bool enableTxUHF = true;
    [ObservableProperty] 
    private bool enableTxVHF = true;
    [ObservableProperty] 
    private string powerUpChar1 = "BAOFENG";
    [ObservableProperty] 
    private string powerUpChar2 = "UV-5R";
    [ObservableProperty] 
    private string theMaxFreqOfUHF = "520";
    [ObservableProperty] 
    private string theMaxFreqOfVHF = "174";
    [ObservableProperty] 
    private string theMinFreqOfUHF = "400";
    [ObservableProperty] 
    private string theMinFreqOfVHF = "136";
    [ObservableProperty] 
    private int theRangeOfUHF;
    [ObservableProperty] 
    private int theRangeOfVHF;
}