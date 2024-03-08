using System;

namespace SHX_GT12_CPS;

public class ProgressBarIssue
{
    public event EventHandler<ProgressEventArgs> progressBarValue;

    public void IssueProgressValue(int value, string content)
    {
        UpdateProgressValue(value, content);
    }

    protected virtual void UpdateProgressValue(int value, string content)
    {
        progressBarValue?.Invoke(this, new ProgressEventArgs(value, content));
    }
}