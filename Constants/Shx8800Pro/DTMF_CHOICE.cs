using System.Collections.ObjectModel;
using SenhaixFreqWriter.Utils.Other;

namespace SenhaixFreqWriter.Constants.Shx8800Pro;

public class DtmfChoice
{
    public static ObservableCollection<string> Time = new()
    {
        "50 ms", "100 ms", "200 ms", "300 ms", "500 ms"
    };

    public static ObservableCollection<string> SendId = new()
    {
        // "关", "按下PPT", "松开PPT", "两者"
        Language.GetString("off"), 
        Language.GetString("press_ptt"), 
        Language.GetString("release_ptt"), 
        Language.GetString("both"), 
    };
}