using System;
using System.Threading;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Avalonia.Threading;
using MsBox.Avalonia;
using SenhaixFreqWriter.Constants.Gt12;
using SenhaixFreqWriter.Constants.Shx8x00;
using SenhaixFreqWriter.DataModels.Gt12;
using SenhaixFreqWriter.Utils.HID;

namespace SenhaixFreqWriter.Views.Gt12;

public partial class ProgressBarWindow : Window
{
    private Thread thread_Communication;

    private Thread thread_progress;

    private OP_TYPE operation;

    private HIDCommunication com;

    private bool stopUpdateValue = false;

    private bool opRes;

    private CancellationTokenSource cancelSource;

    public ProgressBarWindow(OP_TYPE op)
    {
        operation = op;
        InitializeComponent();
        com = new HIDCommunication(operation);
        cancelSource = new CancellationTokenSource();
    }

    private async void StartButton_OnClick(object? sender, RoutedEventArgs e)
    {
        if (!HIDTools.getInstance().isDeviceConnected)
        {
            await MessageBoxManager.GetMessageBoxStandard("注意", "请连接写频线！").ShowWindowDialogAsync(this);
            return;
        }

        if (opRes)
        {
            opRes = false;
            Close();
            return;
        }

        StartButton.IsEnabled = false;
        CloseButton.IsEnabled = true;
        progressBar.Value = 0;
        thread_Communication = new Thread(() => Task_Communication(cancelSource.Token));
        thread_Communication.Start();
        thread_progress = new Thread(() => Task_Progress(cancelSource.Token));
        thread_progress.Start();
    }

    private void Task_Communication(CancellationToken token)
    {
        var flag = com.DoIt(token);
        // Console.WriteLine("We've done write!");
        Dispatcher.UIThread.Post(() => HandleResult(flag));
    }

    private void Task_Progress(CancellationToken token)
    {
        while (!stopUpdateValue && !token.IsCancellationRequested)
        {
            ProgressBarValue pgv;
            if (!com.statusQueue.TryDequeue(out pgv)) continue;
            ;
            Dispatcher.UIThread.Post(() => statusLabel.Content = pgv.content);
            Dispatcher.UIThread.Post(() => progressBar.Value = pgv.value);
        }
    }

    private void HandleResult(bool result)
    {
        if (result)
        {
            statusLabel.Content = "完成！";
            opRes = true;
        }
        else
        {
            statusLabel.Content = "失败!";
            opRes = false;
        }

        StartButton.IsEnabled = true;
        if (opRes)
        {
            StartButton.Content = "关闭";
            CloseButton.IsEnabled = false;
        }
        else
        {
            StartButton.Content = "重试";
            CloseButton.IsEnabled = true;
        }

        stopUpdateValue = true;
    }

    private void Cancel_OnClick(object? sender, RoutedEventArgs e)
    {
        if ((thread_progress != null || thread_Communication != null)&&operation==OP_TYPE.READ)
        {
            cancelSource.Cancel();
            thread_progress.Join();
            thread_Communication.Join();
            AppData.forceNewInstance();
        }

        Close();
    }
}