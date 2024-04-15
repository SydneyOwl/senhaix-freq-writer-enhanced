using System;
using System.Threading;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Threading;
using MsBox.Avalonia;
using shx8x00.Constants;
using shx8x00.DataModels;
using shx8x00.Utils.Serial;

namespace shx8x00.Views;

public partial class ProgressBarWindow : Window
{
    private readonly MySerialPort sP;

    // 读0
    private readonly int status;

    private readonly ClassTheRadioData theRadioData;

    private Thread threadProgress;

    private Thread threadWF;

    private WriFreq wF;

    private bool opRes = false;


    public ProgressBarWindow(int opStatus)
    {
        status = opStatus;
        theRadioData = ClassTheRadioData.getInstance();
        sP = MySerialPort.getInstance();
        InitializeComponent();
    }

    private void Exit_OnClick(object? sender, RoutedEventArgs e)
    {
        Close();
    }

    private void Start_OnClick(object? sender, RoutedEventArgs e)
    {
        if (opRes)
        {
            opRes = false;
            Close();
            return;
        }
        StartButton.IsEnabled = false;
        CloseButton.IsEnabled = false;
        progressBar.Value = 0;
        try
        {
            sP.OpenSerial();
            threadWF = new Thread(Task_WriteFreq);
            threadWF.Start();
            threadProgress = new Thread(Task_GetProgress);
            threadProgress.Start();
        }
        catch (Exception ed)
        {
            MessageBoxManager.GetMessageBoxStandard("注意", "检查写频线是否正确连接:"+ed.Message).ShowWindowDialogAsync(this);
            StartButton.IsEnabled = true;
            CloseButton.IsEnabled = true;
            sP.CloseSerial();
        }
    }

    private async void Task_WriteFreq()
    {
        var flag = false;
        if (status == 0)
            wF = new WriFreq(sP, theRadioData, OPERATION_TYPE.READ);
        else
            wF = new WriFreq(sP, theRadioData, OPERATION_TYPE.WRITE);
        MySerialPort.getInstance().RxData.Clear();
        try
        {
            flag = await wF.DoIt();
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
            // ignored
        }
        Dispatcher.UIThread.Invoke(() => HandleWFResult(flag));
    }

    private void Task_GetProgress()
    {
        var flag = false;
        var num = 3;
        while (wF == null)
        {
            Thread.Sleep(1);
        }
        while (!wF.flagTransmitting)
        {
            Thread.Sleep(1);
        }

        while (wF.flagTransmitting)
        {
            // Thread.Sleep(1);
            STATE curr;
            if (!wF.currentProgress.TryDequeue(out curr)){continue;}
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
            opRes = true;
        }
        else
        {
            statusLabel.Content = "失败!";
            StartButton.Content = "重试";
            opRes = false;
        }

        sP.CloseSerial();
        StartButton.IsEnabled = true;
        CloseButton.IsEnabled = true;
    }
}