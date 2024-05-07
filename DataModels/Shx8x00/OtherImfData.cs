using System;
using CommunityToolkit.Mvvm.ComponentModel;

namespace SenhaixFreqWriter.DataModels.Shx8x00;

//Notice: 请勿随意变更本文件参数！
[Serializable]
public partial class OtherImfData : ObservableObject
{
    [ObservableProperty] private bool _enableTxOver480M;
    [ObservableProperty] private bool _enableTxUhf = true;
    [ObservableProperty] private bool _enableTxVhf = true;
    [ObservableProperty] private string _powerUpChar1 = "BAOFENG";
    [ObservableProperty] private string _powerUpChar2 = "UV-5R";
    [ObservableProperty] private string _theMaxFreqOfUhf = "520";
    [ObservableProperty] private string _theMaxFreqOfVhf = "174";
    [ObservableProperty] private string _theMinFreqOfUhf = "400";
    [ObservableProperty] private string _theMinFreqOfVhf = "136";
    [ObservableProperty] private int _theRangeOfUhf;
    [ObservableProperty] private int _theRangeOfVhf;
}