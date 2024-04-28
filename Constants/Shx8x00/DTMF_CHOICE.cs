using System.Collections.ObjectModel;

namespace SenhaixFreqWriter.Constants.Shx8x00;

public class DTMF_CHOICE
{
    public static ObservableCollection<string> stopOrLastTime = new()
    {
        "50ms", "100ms", "200ms", "300ms", "500ms"
    };
}