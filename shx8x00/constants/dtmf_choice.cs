using System.Collections.ObjectModel;

namespace shx8x00.constants;

public class dtmf_choice
{
    
    public static ObservableCollection<string> stopOrLastTime = new()
    {
        "50ms", "100ms", "200ms", "300ms", "500ms"
    };
}