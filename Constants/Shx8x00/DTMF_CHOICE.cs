using System.Collections.ObjectModel;

namespace SenhaixFreqWriter.Constants.Shx8x00;

public class DtmfChoice
{
    public static ObservableCollection<string> StopOrLastTime = new()
    {
        "50ms", "100ms", "200ms", "300ms", "500ms"
    };
}