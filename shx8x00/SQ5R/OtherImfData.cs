using System;

namespace SQ5R;

[Serializable]
public class OtherImfData
{
    private bool enableTxOver480M;

    private bool enableTxUHF = true;

    private bool enableTxVHF = true;

    private string powerUpChar1 = "BAOFENG";

    private string powerUpChar2 = "UV-5R";

    private string theMaxFreqOfUHF = "520";

    private string theMaxFreqOfVHF = "174";

    private string theMinFreqOfUHF = "400";

    private string theMinFreqOfVHF = "136";

    private int theRangeOfUHF;
    private int theRangeOfVHF;

    public string TheMinFreqOfVHF
    {
        get => theMinFreqOfVHF;
        set => theMinFreqOfVHF = value;
    }

    public string TheMaxFreqOfVHF
    {
        get => theMaxFreqOfVHF;
        set => theMaxFreqOfVHF = value;
    }

    public int TheRangeOfUHF
    {
        get => theRangeOfUHF;
        set => theRangeOfUHF = value;
    }

    public string TheMinFreqOfUHF
    {
        get => theMinFreqOfUHF;
        set => theMinFreqOfUHF = value;
    }

    public string TheMaxFreqOfUHF
    {
        get => theMaxFreqOfUHF;
        set => theMaxFreqOfUHF = value;
    }

    public bool EnableTxUHF
    {
        get => enableTxUHF;
        set => enableTxUHF = value;
    }

    public bool EnableTxVHF
    {
        get => enableTxVHF;
        set => enableTxVHF = value;
    }

    public string PowerUpChar1
    {
        get => powerUpChar1;
        set => powerUpChar1 = value;
    }

    public string PowerUpChar2
    {
        get => powerUpChar2;
        set => powerUpChar2 = value;
    }

    public bool EnableTxOver480M
    {
        get => enableTxOver480M;
        set => enableTxOver480M = value;
    }

    public int TheRangeOfVHF
    {
        get => theRangeOfVHF;
        set => theRangeOfVHF = value;
    }
}