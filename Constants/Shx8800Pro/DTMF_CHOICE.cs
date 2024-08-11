using System.Collections.ObjectModel;

namespace SenhaixFreqWriter.Constants.Shx8800Pro;

public class DtmfChoice
{
    public static ObservableCollection<string> Time = new()
    {
        "50 ms", "100 ms", "200 ms", "300 ms", "500 ms"
    };
    
    public static ObservableCollection<string> SendID = new()
    {
        "关", "按下PPT", "松开PPT", "两者"
    };
}