using System;

namespace BF_H802_Import_Picture_tools;

public class ComInfoIssue
{
    public event EventHandler<ComInfoEventArgs> ComInfo;

    public void IssueProgressValue(int val)
    {
        RaiseComInfo(val);
    }

    protected virtual void RaiseComInfo(int val)
    {
        ComInfo?.Invoke(this, new ComInfoEventArgs(val));
    }
}