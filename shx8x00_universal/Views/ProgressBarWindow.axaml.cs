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


    private void Start_OnClick(object? sender, RoutedEventArgs e)
    {
        if (opRes)
        {
            opRes = false;
            Close();
        }
        StartButton.IsEnabled = false;
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
            sP.CloseSerial();
        }
    }

    private void Task_WriteFreq()
    {
        var flag = false;
        if (status == 0)
            wF = new WriFreq(sP, theRadioData, OPERATION_TYPE.READ);
        else
            wF = new WriFreq(sP, theRadioData, OPERATION_TYPE.WRITE);
        try
        {
            flag = wF.DoIt();
        }
        catch
        {
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
        }

        while (!wF.flagTransmitting)
        {
        }

        while (wF.flagTransmitting)
            switch (wF.state)
            {
                case STATE.HandShakeStep1:
                case STATE.HandShakeStep2:
                case STATE.HandShakeStep3:
                {
                    var text2 = "握手...";
                    flag = false;
                    Dispatcher.UIThread.Invoke(() => statusLabel.Content = text2);
                    Dispatcher.UIThread.Invoke(() => progressBar.Value = 0);
                    // Invoke(new getWFProgressText(UpdataWFProgressText), text2);
                    // Invoke(new getWFProgress(UpdataWFProgress), 0);
                    break;
                }
                case STATE.HandShakeStep4:
                    if (!flag)
                    {
                        var text3 = "进度...";
                        flag = true;
                        Dispatcher.UIThread.Invoke(() => statusLabel.Content = text3 + num + "%");
                        Dispatcher.UIThread.Invoke(() => progressBar.Value = num);
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
                            Dispatcher.UIThread.Invoke(() => statusLabel.Content = text + num + "%");
                            Dispatcher.UIThread.Invoke(() => progressBar.Value = num);
                            // Invoke(new getWFProgress(UpdataWFProgress), num);
                            // Invoke(new getWFProgressText(UpdataWFProgressText), text + num + "%");
                        }
                    }

                    break;
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
    }
}