using System;
using System.Threading;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Threading;
using MsBox.Avalonia;
using SenhaixFreqWriter.Constants.Gt12;
using SenhaixFreqWriter.DataModels.Gt12;
using SenhaixFreqWriter.Utils.HID;
using SenhaixFreqWriter.Views.Common;

namespace SenhaixFreqWriter.Views.Gt12;

public partial class ProgressBarWindow : Window
{
    private CancellationTokenSource _cancelSource;

    private readonly HidCommunication _com;

    private readonly OpType _operation;

    private bool _opRes;
    private Thread _threadCommunication;

    private Thread _threadProgress;

    public ProgressBarWindow(OpType op)
    {
        _operation = op;
        InitializeComponent();
        _com = new HidCommunication(_operation);
    }

    private async void StartButton_OnClick(object? sender, RoutedEventArgs e)
    {
        if (!_opRes && !HidTools.GetInstance().IsDeviceConnected && HidTools.GetInstance().WriteBle == null)
        {
            await MessageBoxManager.GetMessageBoxStandard("注意", "请连接写频线！").ShowWindowDialogAsync(this);
            return;
        }

        if (_opRes)
        {
            _opRes = false;
            Close();
            return;
        }

        _cancelSource = new CancellationTokenSource();
        StartButton.IsEnabled = false;
        CloseButton.IsEnabled = true;
        progressBar.Value = 0;
        _threadCommunication = new Thread(() => Task_Communication(_cancelSource.Token));
        _threadCommunication.Start();
        _threadProgress = new Thread(() => Task_Progress(_cancelSource.Token));
        _threadProgress.Start();
    }

    private void Task_Communication(CancellationToken token)
    {
        DebugWindow.GetInstance().updateDebugContent("Start WriFreq Thread: StartWriteGt12");
        var flag = false;
        try
        {
            flag = _com.DoIt(token);
        }
        catch (Exception a)
        {
            DebugWindow.GetInstance().updateDebugContent(a.Message);
            // Console.Write(a);
        }

        // DebugWindow.GetInstance().updateDebugContent("We've done write!");
        Dispatcher.UIThread.Invoke(() => HandleResult(flag));
        DebugWindow.GetInstance().updateDebugContent("Terminate WriFreq Thread: StartWriteGt12");
    }

    private void Task_Progress(CancellationToken token)
    {
        DebugWindow.GetInstance().updateDebugContent("Start GetProcess Thread: GetProcessGt12");
        while (!token.IsCancellationRequested)
        {
            // Thread.Sleep(10);
            ProgressBarValue pgv;
            if (!_com.StatusQueue.TryDequeue(out pgv)) continue;
            Dispatcher.UIThread.Post(() => statusLabel.Content = pgv.Content);
            Dispatcher.UIThread.Post(() => progressBar.Value = pgv.Value);
        }

        DebugWindow.GetInstance().updateDebugContent("Terminate GetProcess Thread: GetProcessGt12");
    }

    private void HandleResult(bool result)
    {
        try
        {
            _cancelSource.Cancel();
        }
        catch
        {
            // ignored
        }

        if (result)
        {
            statusLabel.Content = "完成！";
            _opRes = true;
        }
        else
        {
            statusLabel.Content = "失败!请插拔写频线或重启设备后点击重试！";
            _opRes = false;
        }

        StartButton.IsEnabled = true;
        if (_opRes)
        {
            StartButton.Content = "关闭";
            CloseButton.IsEnabled = false;
        }
        else
        {
            StartButton.Content = "重试";
            CloseButton.IsEnabled = true;
        }
    }

    private void Cancel_OnClick(object? sender, RoutedEventArgs e)
    {
        Task.Run(() =>
        {
            try
            {
                _cancelSource.Cancel();
            }
            catch
            {
                //ignored
            }

            Dispatcher.UIThread.Invoke(() => CloseButton.IsEnabled = false);
            if ((_threadProgress != null || _threadCommunication != null) && _operation == OpType.Read)
            {
                Dispatcher.UIThread.Invoke(() => statusLabel.Content = "等待进程结束...");
                _threadProgress.Join();
                _threadCommunication.Join();
                Dispatcher.UIThread.Invoke(() => AppData.ForceNewInstance());
            }

            Dispatcher.UIThread.Invoke(Close);
        });
    }
}