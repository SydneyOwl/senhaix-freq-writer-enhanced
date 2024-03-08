using System;

namespace BF_H802_Import_Picture_tools;

public class ComInfoEventArgs : EventArgs
{
    public ComInfoEventArgs(int progressValue)
    {
        ProgressValue = progressValue;
    }

    public int ProgressValue { get; private set; }
}