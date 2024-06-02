using System;
using System.Threading;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Threading;
using MsBox.Avalonia;
using SenhaixFreqWriter.Constants.Shx8x00;
using SenhaixFreqWriter.DataModels.Shx8x00;
using SenhaixFreqWriter.Utils.Serial;
using SenhaixFreqWriter.Views.Common;

namespace SenhaixFreqWriter.Views.Shx8x00;

public partial class ProgressBarWindow : Window
{
    private readonly MySerialPort _sP;

    private readonly OperationType _status;

    private readonly ClassTheRadioData _theRadioData;

    private CancellationTokenSource _tokenSource;

    private bool _opRes;

    private Thread _threadProgress;

    private Thread _threadWf;

    private WriFreq _wF;


    public ProgressBarWindow(OperationType opStatus)
    {
        _status = opStatus;
        _theRadioData = ClassTheRadioData.GetInstance();
        _sP = MySerialPort.GetInstance();
        InitializeComponent();
    }

    private void Cancel_OnClick(object? sender, RoutedEventArgs e)
    {
        Task.Run(() =>
        {
            try
            {
                _tokenSource.Cancel();
            }
            catch
            {
                //
            }
            Dispatcher.UIThread.Invoke(() => CloseButton.IsEnabled = false);
            if ((_threadWf != null || _threadProgress != null) && _status == OperationType.Read)
            {
                Dispatcher.UIThread.Invoke(() => statusLabel.Content = "等待进程结束...");
                _threadWf.Join();
                _threadProgress.Join();
                Dispatcher.UIThread.Invoke(() => ClassTheRadioData.GetInstance().ForceNewChannel());
            }
            Dispatcher.UIThread.Invoke(Close);
        });
    }

    private async void Start_OnClick(object? sender, RoutedEventArgs e)
    {
        if (!_opRes && MySerialPort.GetInstance().TargetPort == "" && MySerialPort.GetInstance().WriteBle == null)
        {
            await MessageBoxManager.GetMessageBoxStandard("注意", "端口还未选择，请连接蓝牙或写频线！").ShowWindowDialogAsync(this);
            return;
        }

        if (_opRes)
        {
            _opRes = false;
            Close();
            return;
        }

        _tokenSource = new CancellationTokenSource();

        StartButton.IsEnabled = false;
        CloseButton.IsEnabled = true;
        progressBar.Value = 0;
        try
        {
            _sP.OpenSerial();
            _threadWf = new Thread(() => Task_WriteFreq(_tokenSource.Token));
            _threadWf.Start();
            _threadProgress = new Thread(() => Task_GetProgress(_tokenSource.Token));
            _threadProgress.Start();
        }
        catch (Exception ed)
        {
            MessageBoxManager.GetMessageBoxStandard("注意", "检查写频线是否正确连接:" + ed.Message).ShowWindowDialogAsync(this);
            StartButton.IsEnabled = true;
            CloseButton.IsEnabled = true;
            _sP.CloseSerial();
        }
    }

    private async void Task_WriteFreq(CancellationToken cancellationToken)
    {
        DebugWindow.GetInstance().updateDebugContent($"Start WriFreq Thread: WriFreq8800");
        var flag = false;
        _wF = new WriFreq(_sP, _theRadioData, _status);
        MySerialPort.GetInstance().RxData.Clear();
        try
        {
            flag = _wF.DoIt(cancellationToken);
        }
        catch (Exception e)
        {
            DebugWindow.GetInstance().updateDebugContent(e.Message);
            // ignored
        }

        Dispatcher.UIThread.Post(() => HandleWfResult(flag));
        DebugWindow.GetInstance().updateDebugContent($"Terminate WriFreq Thread: WriFreq8800");
        DebugWindow.GetInstance().updateDebugContent($"Disposing bluetooth..");
        // MySerialPort.GetInstance().WriteBle = null;
    }

    private void Task_GetProgress(CancellationToken cancellationToken)
    {
        DebugWindow.GetInstance().updateDebugContent($"Start GetProcess Thread: GetProcess8800");
        var flag = false;
        var num = 3;
        while (_wF == null && !cancellationToken.IsCancellationRequested) Thread.Sleep(1);
        while (!_wF.FlagTransmitting && !cancellationToken.IsCancellationRequested) Thread.Sleep(1);

        while (_wF.FlagTransmitting && !cancellationToken.IsCancellationRequested)
        {
            // Thread.Sleep(10);
            State curr;
            if (!_wF.CurrentProgress.TryDequeue(out curr)) continue;
            switch (curr)
            {
                case State.HandShakeStep1:
                case State.HandShakeStep2:
                case State.HandShakeStep3:
                {
                    var text2 = "握手...";
                    flag = false;
                    Dispatcher.UIThread.Post(() => statusLabel.Content = text2);
                    Dispatcher.UIThread.Post(() => progressBar.Value = 0);
                    // Invoke(new getWFProgressText(UpdataWFProgressText), text2);
                    // Invoke(new getWFProgress(UpdataWFProgress), 0);
                    break;
                }
                case State.HandShakeStep4:
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
                case State.ReadStep1:
                case State.ReadStep3:
                case State.WriteStep1:
                    flag = false;
                    break;
                case State.ReadStep2:
                case State.WriteStep2:
                    if (!flag)
                    {
                        var text = "进度...";
                        flag = true;
                        if (_wF.EepAddr % 64 == 0)
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

        DebugWindow.GetInstance().updateDebugContent($"Terminate GetProcess Thread: GetProcess8800");
    }

    private void HandleWfResult(bool result)
    {
        if (result)
        {
            statusLabel.Content = "成功!";
            StartButton.Content = "关闭";
            progressBar.Value = 100;
            StartButton.IsEnabled = true;
            CloseButton.IsEnabled = false;
            _opRes = true;
        }
        else
        {
            statusLabel.Content = "失败!";
            StartButton.Content = "重试";
            StartButton.IsEnabled = true;
            CloseButton.IsEnabled = true;
            _opRes = false;
        }

        try
        {
            _tokenSource.Cancel();
        }
        catch
        {
            //
        }

        _sP.CloseSerial();
    }
}