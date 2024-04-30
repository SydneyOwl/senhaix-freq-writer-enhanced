using System;
using System.Threading;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Threading;
using MsBox.Avalonia;
using SenhaixFreqWriter.Constants.Shx8x00;
using SenhaixFreqWriter.DataModels.Shx8x00;
using SenhaixFreqWriter.Utils.Serial;

namespace SenhaixFreqWriter.Views.Shx8x00;

public partial class ProgressBarWindow : Window
{
    private readonly MySerialPort sP;

    private readonly OPERATION_TYPE status;

    private readonly ClassTheRadioData theRadioData;

    private readonly CancellationTokenSource tokenSource = new();

    private bool opRes;

    private Thread threadProgress;

    private Thread threadWF;

    private WriFreq wF;


    public ProgressBarWindow(OPERATION_TYPE opStatus)
    {
        status = opStatus;
        theRadioData = ClassTheRadioData.getInstance();
        sP = MySerialPort.getInstance();
        InitializeComponent();
    }

    private void Cancel_OnClick(object? sender, RoutedEventArgs e)
    {
        if (threadWF != null || threadProgress != null)
        {
            tokenSource.Cancel();
            threadWF.Join();
            threadProgress.Join();
            ClassTheRadioData.getInstance().forceNewChannel();
        }

        Close();
    }

    private async void Start_OnClick(object? sender, RoutedEventArgs e)
    {
        if (MySerialPort.getInstance().TargetPort == "" && MySerialPort.getInstance().WriteBLE == null)
        {
            await MessageBoxManager.GetMessageBoxStandard("注意", "端口还未选择，请连接蓝牙或写频线！").ShowWindowDialogAsync(this);
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
        try
        {
            sP.OpenSerial();
            threadWF = new Thread(() => Task_WriteFreq(tokenSource.Token));
            threadWF.Start();
            threadProgress = new Thread(() => Task_GetProgress(tokenSource.Token));
            threadProgress.Start();
        }
        catch (Exception ed)
        {
            MessageBoxManager.GetMessageBoxStandard("注意", "检查写频线是否正确连接:" + ed.Message).ShowWindowDialogAsync(this);
            StartButton.IsEnabled = true;
            CloseButton.IsEnabled = true;
            sP.CloseSerial();
        }
    }

    private async void Task_WriteFreq(CancellationToken cancellationToken)
    {
        var flag = false;
        wF = new WriFreq(sP, theRadioData, status);
        MySerialPort.getInstance().RxData.Clear();
        try
        {
            flag = await wF.DoIt(cancellationToken);
        }
        catch (Exception e)
        {
            // Console.WriteLine(e.Message);
            // ignored
        }

        Dispatcher.UIThread.Post(() => HandleWFResult(flag));
    }

    private void Task_GetProgress(CancellationToken cancellationToken)
    {
        var flag = false;
        var num = 3;
        while (wF == null && !cancellationToken.IsCancellationRequested) Thread.Sleep(1);
        while (!wF.flagTransmitting && !cancellationToken.IsCancellationRequested) Thread.Sleep(1);

        while (wF.flagTransmitting && !cancellationToken.IsCancellationRequested)
        {
            // Thread.Sleep(1);
            STATE curr;
            if (!wF.currentProgress.TryDequeue(out curr)) continue;
            switch (curr)
            {
                case STATE.HandShakeStep1:
                case STATE.HandShakeStep2:
                case STATE.HandShakeStep3:
                {
                    var text2 = "握手...";
                    flag = false;
                    Dispatcher.UIThread.Post(() => statusLabel.Content = text2);
                    Dispatcher.UIThread.Post(() => progressBar.Value = 0);
                    // Invoke(new getWFProgressText(UpdataWFProgressText), text2);
                    // Invoke(new getWFProgress(UpdataWFProgress), 0);
                    break;
                }
                case STATE.HandShakeStep4:
                    if (!flag)
                    {
                        var text3 = "进度...";
                        flag = true;
                        Dispatcher.UIThread.Post(() => statusLabel.Content = text3 + num + "%");
                        Dispatcher.UIThread.Post(() => progressBar.Value = num);
                        // Invoke(new getWFProgress(UpdataWFProgress), num);
                        // Invoke(new getWFProgressText(UpdataWFProgressText), text3 + num + "%");
                    }

                    break;
                case STATE.ReadStep1:
                case STATE.ReadStep3:
                case STATE.WriteStep1:
                    flag = false;
                    break;
                case STATE.ReadStep2:
                case STATE.WriteStep2:
                    if (!flag)
                    {
                        var text = "进度...";
                        flag = true;
                        if (wF.eepAddr % 64 == 0)
                        {
                            num++;
                            Dispatcher.UIThread.Post(() => statusLabel.Content = text + num + "%");
                            Dispatcher.UIThread.Post(() => progressBar.Value = num);
                            // Invoke(new getWFProgress(UpdataWFProgress), num);
                            // Invoke(new getWFProgressText(UpdataWFProgressText), text + num + "%");
                        }
                    }

                    break;
            }
        }
    }

    private void HandleWFResult(bool result)
    {
        if (result)
        {
            statusLabel.Content = "成功!";
            StartButton.Content = "关闭";
            progressBar.Value = 100;
            StartButton.IsEnabled = true;
            CloseButton.IsEnabled = false;
            opRes = true;
        }
        else
        {
            statusLabel.Content = "失败!";
            StartButton.Content = "重试";
            StartButton.IsEnabled = true;
            CloseButton.IsEnabled = true;
            opRes = false;
        }

        sP.CloseSerial();
    }
}