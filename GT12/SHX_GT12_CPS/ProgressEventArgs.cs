using System;

namespace SHX_GT12_CPS;

public class ProgressEventArgs : EventArgs
{
    public ProgressEventArgs(int value, string content)
    {
        Value = value;
        Content = content;
    }

    public string Content { get; private set; }

    public int Value { get; private set; }
}