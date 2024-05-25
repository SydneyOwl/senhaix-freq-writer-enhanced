using System;
using System.Collections.ObjectModel;
using System.Drawing.Drawing2D;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Avalonia.Threading;
using MsBox.Avalonia;
using SenhaixFreqWriter.Constants.BLE;
using SenhaixFreqWriter.Utils.BLE.Interfaces;
using SenhaixFreqWriter.Utils.BLE.Platforms.RPC;

#if WINDOWS
using shx.Utils.BLE.Platforms.Windows;
#endif

namespace SenhaixFreqWriter.Views.Common;

public partial class BluetoothDeviceSelectionWindow : Window
{
    public ObservableCollection<GenerticBLEDeviceInfo> BleInfos { get; set; } = new();
    public IBluetooth osBLE;

    public BluetoothDeviceSelectionWindow()
    {
        InitializeComponent();
#if !WINDOWS
        useRPC.IsChecked = true;
        useRPC.IsEnabled = false;
#endif
        DataContext = this;
    }

    private void ScanButton_OnClick(object? sender, RoutedEventArgs e)
    {
        Dispatcher.UIThread.Invoke(() =>
        {
            scanButton.IsEnabled = false;
            scanStat.Text = "WAIT...";
        });
        Task.Run(() =>
        {
            try
            {
                osBLE?.Dispose();
                osBLE = new RPCSHXBLE();

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
                    osBLE = new WindowsShxble();
                }      
#endif
                if (!osBLE.GetBleAvailabilityAsync())
                {
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
                    foreach (var generticBleDeviceInfo in result)
                    {
                        BleInfos.Add(generticBleDeviceInfo);
                    }
                });
            }
            catch (Exception a)
            {
                Console.WriteLine(a.Message);
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
        
    }

    private void ConnButton_OnClick(object? sender, RoutedEventArgs e)
    {
        // if (btdevice.SelectedIndex)
        Task.Run(() =>
        {
            if (btdevice.SelectedIndex == -1)
            {
                Dispatcher.UIThread.Invoke(() => { MessageBoxManager.GetMessageBoxStandard("注意", "未选择设备！").ShowWindowDialogAsync(this); });
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
                osBLE.RegisterSerial();
            }
            catch (Exception f)
            {
                Console.WriteLine($"dError:{f.Message}");
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
}