using System.Collections.ObjectModel;

namespace SenhaixFreqWriter.Constants.Gt12;

public class DtmfChoice
{
    public static ObservableCollection<string> Time = new()
    {
        "50 ms", "100 ms", "200 ms", "300 ms", "500 ms"
    };
}