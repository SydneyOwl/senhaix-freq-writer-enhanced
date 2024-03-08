using System;

namespace SHX_GT12_CPS;

[Serializable]
public class MDC1200
{
    private string CallID = "";

    private string group = "111";
    private string id = "1111";

    public string Id
    {
        get => id;
        set => id = value;
    }

    public string Group
    {
        get => group;
        set => group = value;
    }

    public string CallID1
    {
        get => CallID;
        set => CallID = value;
    }
}