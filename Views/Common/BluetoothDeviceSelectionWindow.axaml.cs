using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Drawing.Drawing2D;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Avalonia.Media;
using Avalonia.Threading;
using MsBox.Avalonia;
using SenhaixFreqWriter.Constants.BLE;
using SenhaixFreqWriter.Constants.Common;
using SenhaixFreqWriter.Utils.BLE.Interfaces;
using SenhaixFreqWriter.Utils.BLE.Platforms.RPC;
using SenhaixFreqWriter.Utils.Other;

#if WINDOWS
using SenhaixFreqWriter.Utils.BLE.Platforms.Windows;
#endif

namespace SenhaixFreqWriter.Views.Common;

public partial class BluetoothDeviceSelectionWindow : Window
{
    public ObservableCollection<GenerticBLEDeviceInfo> BleInfos { get; set; } = new();
    public IBluetooth osBLE;
    private SHX_DEVICE dev = SHX_DEVICE.SHX8X00;

    public BluetoothDeviceSelectionWindow()
    {
        InitializeComponent();
        DataContext = this;
    }

    public BluetoothDeviceSelectionWindow(SHX_DEVICE shxDevice)
    {
        InitializeComponent();
        dev = shxDevice;
#if !WINDOWS
        useRPC.IsChecked = true;
        useRPC.IsEnabled = false;
        manualRPC.IsEnabled = true;
#endif
        DataContext = this;
    }

    private void ScanButton_OnClick(object? sender, RoutedEventArgs e)
    {
        var dispStatus = manualRPC.IsChecked.Value;
        osBLE?.Dispose();
        osBLE = new WSRPCBLE(dispStatus);
        Dispatcher.UIThread.Invoke(() =>
        {
            scanButton.IsEnabled = false;
            scanStat.Text = "WAIT...";
        });
        Task.Run(() =>
        {
            try
            {
                var checkRPC = false;
                var checkDisableWeakSignalRestriction = false;
                var checkDisableSSIDRestriction = false;

                Dispatcher.UIThread.Invoke(() =>
                {
                    checkRPC = useRPC.IsChecked.Value;
                    checkDisableSSIDRestriction = disableSSIDF.IsChecked.Value;
                    checkDisableWeakSignalRestriction = disableWeakSignal.IsChecked.Value;
                    scanButton.IsEnabled = false;
                    scanStat.Text = "扫描中...";
                    ProgressRing1.IsActive = true;
                    BleInfos.Clear();
                });
#if WINDOWS
                if (!checkRPC)
                {
                    osBLE = new WindowsSHXBLE();
                }
#endif
                if (!osBLE.GetBleAvailabilityAsync())
                {
                    DebugWindow.GetInstance().updateDebugContent("Not available");
                    Dispatcher.UIThread.Invoke(() =>
                    {
                        MessageBoxManager.GetMessageBoxStandard("注意", "蓝牙错误！如果您使用RPC方式请确保服务端已打开！")
                            .ShowWindowDialogAsync(this);
                    });
                    return;
                }

                var result = osBLE.ScanForShxAsync(checkDisableWeakSignalRestriction, checkDisableSSIDRestriction);
                Dispatcher.UIThread.Invoke(() =>
                {
                    
#if WINDOWS
                    if (result.Count == 0)
                    {
                        windowsHint.Background = Brushes.Salmon;
                        windowsHint.IsVisible = true;
                    }
#endif

                    foreach (var generticBleDeviceInfo in result) BleInfos.Add(generticBleDeviceInfo);
                });
            }
            catch (Exception a)
            {
                DebugWindow.GetInstance().updateDebugContent(a.Message);
                Dispatcher.UIThread.Invoke(() =>
                {
                    MessageBoxManager.GetMessageBoxStandard("注意", "蓝牙错误！如果您使用RPC方式请确保服务端已打开！")
                        .ShowWindowDialogAsync(this);
                });
            }
            finally
            {
                Dispatcher.UIThread.Invoke(() =>
                {
                    scanButton.IsEnabled = true;
                    scanStat.Text = "扫描设备";
                    ProgressRing1.IsActive = false;
                });
            }
        });
    }


    private void UseRPC_OnIsCheckedChanged(object? sender, RoutedEventArgs e)
    {
        if (useRPC.IsChecked.Value)
        {
            manualRPC.IsEnabled = true;
        }
        else
        {
            manualRPC.IsEnabled = false;
            manualRPC.IsChecked = false;
        }
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
                connButton.IsEnabled = false;
                connStat.Text = "连接中..";
                ProgressRing2.IsActive = true;
            });
            try
            {
                osBLE.SetDevice(BleInfos[btdevice.SelectedIndex].DeviceID);
                var connDevStat = osBLE.ConnectShxDeviceAsync();
                if (!connDevStat)
                {
                    Dispatcher.UIThread.Invoke(() =>
                    {
                        MessageBoxManager.GetMessageBoxStandard("注意", "连接设备失败！").ShowWindowDialogAsync(this);
                    });
                    return;
                }

                Dispatcher.UIThread.Invoke(() => { connStat.Text = "连接服务.."; });
                var connSerStat = osBLE.ConnectShxRwServiceAsync();
                if (!connSerStat)
                {
                    Dispatcher.UIThread.Invoke(() =>
                    {
                        MessageBoxManager.GetMessageBoxStandard("注意", "连接服务失败！").ShowWindowDialogAsync(this);
                    });
                    return;
                }

                Dispatcher.UIThread.Invoke(() => { connStat.Text = "连接特征.."; });
                var connChStat = osBLE.ConnectShxRwCharacteristicAsync();
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
                if (dev.Equals(SHX_DEVICE.SHX8X00))
                    osBLE.RegisterSerial();
                else
                    osBLE.RegisterHid();
            }
            catch (Exception f)
            {
                DebugWindow.GetInstance().updateDebugContent($"dError:{f.Message}");
                Dispatcher.UIThread.Invoke(() =>
                {
                    MessageBoxManager.GetMessageBoxStandard("注意", $"出错：{f.Message}！").ShowWindowDialogAsync(this);
                });
            }
            finally
            {
                Dispatcher.UIThread.Invoke(() =>
                {
                    connButton.IsEnabled = true;
                    connStat.Text = "连接设备";
                    ProgressRing2.IsActive = false;
                });
            }
        });
    }

    private void WindowsHint_OnPointerPressed(object? sender, PointerPressedEventArgs e)
    {
        try{
            Process.Start("control", "bthprops.cpl");
        }catch{
            //
        }
    }
}