using System.Collections.ObjectModel;

namespace shx8x00.Constants;

public class DTMF_CHOICE
{
    public static ObservableCollection<string> stopOrLastTime = new()
    {
        "50ms", "100ms", "200ms", "300ms", "500ms"
    };
}