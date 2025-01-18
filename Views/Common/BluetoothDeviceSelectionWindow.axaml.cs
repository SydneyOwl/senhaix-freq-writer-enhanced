using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Threading;
using MsBox.Avalonia;
using SenhaixFreqWriter.Constants.Common;
using SenhaixFreqWriter.Utils.BLE.Interfaces;
using SenhaixFreqWriter.Utils.BLE.Platforms.RPC;

namespace SenhaixFreqWriter.Views.Common;

public partial class BluetoothDeviceSelectionWindow : Window
{
    private readonly ShxDevice _dev = ShxDevice.Shx8800;
    public IBluetooth OsBle;

    public BluetoothDeviceSelectionWindow()
    {
        InitializeComponent();
        DataContext = this;
    }

    public BluetoothDeviceSelectionWindow(ShxDevice shxDevice)
    {
        InitializeComponent();
        _dev = shxDevice;
        DataContext = this;
    }

    public ObservableCollection<GenerticBleDeviceInfo> BleInfos { get; set; } = new();

    private void ScanButton_OnClick(object? sender, RoutedEventArgs e)
    {
        var dispStatus = manualRPC.IsChecked.Value;
        if (!dispStatus) OsBle?.Dispose();
        OsBle = new Wsrpcble(dispStatus);
        Dispatcher.UIThread.Invoke(() =>
        {
            connButton.IsEnabled = false;
            scanButton.IsEnabled = false;
            scanStat.Text = "WAIT...";
        });
        Task.Run(() =>
        {
            try
            {
                var checkDisableWeakSignalRestriction = false;
                var checkDisableSsidRestriction = false;

                Dispatcher.UIThread.Invoke(() =>
                {
                    checkDisableSsidRestriction = disableSSIDF.IsChecked.Value;
                    // checkDisableWeakSignalRestriction = disableWeakSignal.IsChecked.Value;
                    scanButton.IsEnabled = false;
                    connButton.IsEnabled = false;
                    scanStat.Text = "扫描中...";
                    ProgressRing1.IsActive = true;
                    BleInfos.Clear();
                });
                if (!OsBle.GetBleAvailabilityAsync())
                {
                    DebugWindow.GetInstance().UpdateDebugContent("Not available");
                    Dispatcher.UIThread.Invoke(() =>
                    {
                        MessageBoxManager.GetMessageBoxStandard("注意", "蓝牙错误！请确保服务端已打开！")
                            .ShowWindowDialogAsync(this);
                    });
                    return;
                }

                var result = OsBle.ScanForShxAsync(checkDisableWeakSignalRestriction, checkDisableSsidRestriction);
                Dispatcher.UIThread.Invoke(() =>
                {
#if WINDOWS
                    if (result.Count == 0)
                    {
                        // windowsHint.Background = Brushes.Salmon;
                        windowsHint.IsVisible = true;
                    }
#endif
                    foreach (var generticBleDeviceInfo in result) BleInfos.Add(generticBleDeviceInfo);
                });
            }
            catch (Exception a)
            {
                DebugWindow.GetInstance().UpdateDebugContent(a.Message);
                Dispatcher.UIThread.Invoke(() =>
                {
                    MessageBoxManager.GetMessageBoxStandard("注意", "蓝牙错误！请确保服务端已打开！")
                        .ShowWindowDialogAsync(this);
                });
            }
            finally
            {
                Dispatcher.UIThread.Invoke(() =>
                {
                    connButton.IsEnabled = true;
                    scanButton.IsEnabled = true;
                    scanStat.Text = "扫描设备";
                    ProgressRing1.IsActive = false;
                });
            }
        });
    }

    private void ConnButton_OnClick(object? sender, RoutedEventArgs e)
    {
        // if (btdevice.SelectedIndex)
        Task.Run(() =>
        {
            if (btdevice.SelectedIndex == -1)
            {
                Dispatcher.UIThread.Invoke(() =>
                {
                    MessageBoxManager.GetMessageBoxStandard("注意", "未选择设备！").ShowWindowDialogAsync(this);
                });
                return;
            }

            Dispatcher.UIThread.Invoke(() =>
            {
                scanButton.IsEnabled = false;
                connButton.IsEnabled = false;
                connStat.Text = "连接中..";
                ProgressRing2.IsActive = true;
            });
            try
            {
                OsBle.SetDevice(BleInfos[btdevice.SelectedIndex].DeviceId);
                var connDevStat = OsBle.ConnectShxDeviceAsync();
                if (!connDevStat)
                {
                    Dispatcher.UIThread.Invoke(() =>
                    {
                        MessageBoxManager.GetMessageBoxStandard("注意", "连接设备失败！").ShowWindowDialogAsync(this);
                    });
                    return;
                }

                Dispatcher.UIThread.Invoke(() => { connStat.Text = "连接服务.."; });
                var connSerStat = OsBle.ConnectShxRwServiceAsync();
                if (!connSerStat)
                {
                    Dispatcher.UIThread.Invoke(() =>
                    {
                        MessageBoxManager.GetMessageBoxStandard("注意", "连接服务失败！").ShowWindowDialogAsync(this);
                    });
                    return;
                }

                Dispatcher.UIThread.Invoke(() => { connStat.Text = "连接特征.."; });
                var connChStat = OsBle.ConnectShxRwCharacteristicAsync();
                if (!connChStat)
                {
                    Dispatcher.UIThread.Invoke(() =>
                    {
                        MessageBoxManager.GetMessageBoxStandard("注意", "连接特征失败！").ShowWindowDialogAsync(this);
                    });
                    return;
                }

                Dispatcher.UIThread.Invoke(() =>
                {
                    MessageBoxManager.GetMessageBoxStandard("注意", "连接成功！您可以开始写频了！").ShowWindowDialogAsync(this);
                });
                if (_dev is ShxDevice.Shx8800 or ShxDevice.Shx8800Pro)
                    OsBle.RegisterSerial();
                else
                    OsBle.RegisterHid();
            }
            catch (Exception f)
            {
                DebugWindow.GetInstance().UpdateDebugContent($"dError:{f.Message}");
                Dispatcher.UIThread.Invoke(() =>
                {
                    MessageBoxManager.GetMessageBoxStandard("注意", $"出错：{f.Message}！").ShowWindowDialogAsync(this);
                });
            }
            finally
            {
                Dispatcher.UIThread.Invoke(() =>
                {
                    scanButton.IsEnabled = true;
                    connButton.IsEnabled = true;
                    connStat.Text = "连接设备";
                    ProgressRing2.IsActive = false;
                });
            }
        });
    }

    private void WindowsHint_OnPointerPressed(object? sender, PointerPressedEventArgs e)
    {
        try
        {
            Process.Start("control", "bthprops.cpl");
        }
        catch
        {
            //
        }
    }

    private void ManualRPC_OnIsCheckedChanged(object? sender, RoutedEventArgs e)
    {
        OsBle?.Dispose();
    }
}