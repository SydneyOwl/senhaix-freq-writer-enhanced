namespace SenhaixFreqWriter.DataModels.Shx8800Pro;

public class ProgressBarValue
{
    public string Content;
    public int Value;

    public ProgressBarValue(int value, string content)
    {
        Value = value;
        Content = content;
    }
}