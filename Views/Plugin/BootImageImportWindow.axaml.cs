using System;
using System.Collections.Generic;
using System.Threading;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Media.Imaging;
using Avalonia.Platform.Storage;
using Avalonia.Threading;
using MsBox.Avalonia;
using MsBox.Avalonia.Enums;
using SenhaixFreqWriter.Constants.Common;
using SenhaixFreqWriter.Constants.Shx8x00;
using SenhaixFreqWriter.Utils.HID;
using SenhaixFreqWriter.Utils.Serial;
using SenhaixFreqWriter.Views.Common;
using SkiaSharp;

namespace SenhaixFreqWriter.Views.Plugin;

public partial class BootImageImportWindow : Window
{
    private readonly ShxDevice _device = ShxDevice.Shx8600;
    private SKBitmap _bitmap;
    private HidBootImage _bootHid;
    private WriBootImage _bootWri;
    private CancellationTokenSource _ctx;

    public BootImageImportWindow()
    {
        BootImgWidth = Others.BootImgWidth;
        BootImgHeight = Others.BootImgHeight;
        Hint = $"尺寸限制：{BootImgWidth}x{BootImgHeight}，建议bmp格式";
        WindowHeight = 200;
        InitializeComponent();
        DataContext = this;
    }

    public BootImageImportWindow(ShxDevice dev)
    {
        _device = dev;
        switch (dev)
        {
            case ShxDevice.Shx8600:
            case ShxDevice.Shx8600Pro:
            case ShxDevice.Shx8800Pro:
            case ShxDevice.Shx8800:
                BootImgWidth = Others.BootImgWidth;
                BootImgHeight = Others.BootImgHeight;
                Hint = $"尺寸限制：{BootImgWidth}x{BootImgHeight}，建议bmp格式";
                WindowHeight = 200;
                break;
            case ShxDevice.Gt12:
                BootImgWidth = Constants.Gt12.Others.BootImgWidth;
                BootImgHeight = Constants.Gt12.Others.BootImgHeight;
                Hint = $"尺寸限制：{BootImgWidth}x{BootImgHeight}，建议bmp格式";
                WindowHeight = 390;
                break;
        }

        DebugWindow.GetInstance().UpdateDebugContent($"尺寸：{BootImgWidth}*{BootImgHeight}");
        InitializeComponent();
        Closed += (sender, args) =>
        {
            _bootWri?.CancelWriteImg();
            _bootHid?.CancelWriteImg();
        };
        DataContext = this;
    }

    public int BootImgWidth { get; set; }
    public int BootImgHeight { get; set; }

    public int WindowHeight { get; set; }

    public string Hint { get; set; }

    private async void OpenImageButton_OnClick(object? sender, RoutedEventArgs e)
    {
        var topLevel = GetTopLevel(this);
        var fileType = new FilePickerFileType("image");
        fileType.Patterns = new[] { "*.bmp", "*.jpg", "*.png" };
        var files = await topLevel.StorageProvider.OpenFilePickerAsync(new FilePickerOpenOptions
        {
            Title = "打开备份",
            AllowMultiple = false,
            FileTypeFilter = new List<FilePickerFileType>
            {
                fileType
            }
        });
        if (files.Count == 0) return;
        var bitmap = SKBitmap.Decode(files[0].Path.LocalPath);
        if (bitmap == null)
        {
            MessageBoxManager.GetMessageBoxStandard("注意", "出错，请检查您的路径，应当为纯英文，或软件对图片无读权限！").ShowWindowDialogAsync(this);
            return;
        }

        DebugWindow.GetInstance().UpdateDebugContent($"PicFormat：{bitmap.ColorType}");
        if (!bitmap.ColorType.Equals(SKColorType.Bgra8888) && !bitmap.ColorType.Equals(SKColorType.Rgba8888))
        {
            MessageBoxManager.GetMessageBoxStandard("注意", "图片像素格式不符合要求！").ShowWindowDialogAsync(this);
            return;
        }

        DebugWindow.GetInstance().UpdateDebugContent($"图片尺寸：{bitmap.Width}*{bitmap.Height}");
        if ((bitmap.Width != Others.BootImgWidth ||
             bitmap.Height != Others.BootImgHeight) && _device != ShxDevice.Gt12)
        {
            MessageBoxManager.GetMessageBoxStandard("注意", "图片尺寸不符合要求！").ShowWindowDialogAsync(this);
            return;
        }

        if ((bitmap.Width != Constants.Gt12.Others.BootImgWidth ||
             bitmap.Height != Constants.Gt12.Others.BootImgHeight) && _device == ShxDevice.Gt12)
        {
            MessageBoxManager.GetMessageBoxStandard("注意", "图片尺寸不符合要求！").ShowWindowDialogAsync(this);
            return;
        }

        bootImage.Source = new Bitmap(files[0].Path.LocalPath);
        _bitmap = bitmap;
    }

    private async void ImportButton_OnClick(object? sender, RoutedEventArgs e)
    {
        _ctx = new CancellationTokenSource();
        pgBar.Value = 0;

        if (_bitmap == null)
        {
            MessageBoxManager.GetMessageBoxStandard("注意", "请选择图片！").ShowWindowDialogAsync(this);
            return;
        }

        start.IsEnabled = false;
        stop.IsEnabled = true;
        if (_device != ShxDevice.Gt12)
        {
            if (MySerialPort.GetInstance().TargetPort == "" && MySerialPort.GetInstance().WriteBle == null)
            {
                start.IsEnabled = true;
                stop.IsEnabled = false;
                MessageBoxManager.GetMessageBoxStandard("注意", "请连接蓝牙或写频线！").ShowWindowDialogAsync(this);
                return;
            }

            try
            {
                _bootWri = new WriBootImage(_device, _bitmap);
            }
            catch (Exception aa)
            {
                await MessageBoxManager.GetMessageBoxStandard("注意", $"检查手台连接：{aa.Message}").ShowWindowDialogAsync(this);
                start.IsEnabled = true;
                stop.IsEnabled = false;
                return;
            }

            new Thread(() => { StartWrite8X00(_ctx); }).Start();
            new Thread(() => { StartGetProcess8X00(_ctx.Token); }).Start();
        }

        if (_device == ShxDevice.Gt12)
        {
            if (!HidTools.GetInstance().IsDeviceConnected && HidTools.GetInstance().WriteBle == null)
            {
                MessageBoxManager.GetMessageBoxStandard("注意", "请连接蓝牙或写频线！").ShowWindowDialogAsync(this);
                start.IsEnabled = true;
                stop.IsEnabled = false;
                return;
            }

            _bootHid = new HidBootImage(_bitmap);
            new Thread(() => { StartWriteGt12(_ctx); }).Start();
            new Thread(() => { StartGetProcessGt12(_ctx.Token); }).Start();
        }
    }

    private async void StartWrite8X00(CancellationTokenSource source)
    {
        DebugWindow.GetInstance().UpdateDebugContent("Start WriImg Thread: StartWrite8x00");
        var res = _bootWri.WriteImg();
        source.Cancel();
        Dispatcher.UIThread.Invoke(() =>
        {
            if (!IsActive) return;
            if (res)
                MessageBoxManager.GetMessageBoxStandard("注意", "导入成功！").ShowWindowDialogAsync(this);
            else
                MessageBoxManager.GetMessageBoxStandard("注意", "导入失败！").ShowWindowDialogAsync(this);
            // 不用ResetBLE，因为更改开机图片不变更蓝牙状态
            start.IsEnabled = true;
            stop.IsEnabled = false;
        });

        DebugWindow.GetInstance().UpdateDebugContent("Terminate WriImg Thread: StartWrite8x00");
    }

    private void StartGetProcess8X00(CancellationToken token)
    {
        DebugWindow.GetInstance().UpdateDebugContent("Start WriImg Thread: StartGetProcess8800");
        while (!token.IsCancellationRequested)
        {
            int curPct;
            if (!_bootWri.CurrentProg.TryDequeue(out curPct)) continue;
            Dispatcher.UIThread.Post(() => { pgBar.Value = curPct; });
            // Thread.Sleep(10);
        }

        DebugWindow.GetInstance().UpdateDebugContent("Terminate WriImg Thread: StartGetProcess8800");
    }

    private async void StartWriteGt12(CancellationTokenSource source)
    {
        DebugWindow.GetInstance().UpdateDebugContent("Start WriImg Thread: StartWriteGt12");
        var res = _bootHid.WriteImg();
        source.Cancel();
        Dispatcher.UIThread.Invoke(() =>
        {
            if (!IsActive) return;
            if (res)
                MessageBoxManager.GetMessageBoxStandard("注意", "导入成功！").ShowWindowDialogAsync(this);
            else
                MessageBoxManager.GetMessageBoxStandard("注意", "导入失败！").ShowWindowDialogAsync(this);
            start.IsEnabled = true;
            stop.IsEnabled = false;
        });
        DebugWindow.GetInstance().UpdateDebugContent("Terminate WriImg Thread: StartWriteGt12");
    }

    private void StartGetProcessGt12(CancellationToken token)
    {
        DebugWindow.GetInstance().UpdateDebugContent("Start WriImg Thread: StartGetProcessGt12");
        while (!token.IsCancellationRequested)
        {
            int curPct;
            if (!_bootHid.CurrentProg.TryDequeue(out curPct)) continue;
            Dispatcher.UIThread.Post(() => { pgBar.Value = curPct; });
        }

        DebugWindow.GetInstance().UpdateDebugContent("Terminate WriImg Thread: StartGetProcessGt12");
    }

    private async void CreateImageButton_OnClick(object? sender, RoutedEventArgs e)
    {
        // fix：#21
        new BootImageCreatorWindow(_device).Show();
        // var bi = new BootImageCreatorWindow(_device);
        // await bi.ShowDialog(this);
        // if (bi.CreatedBitmap == null)
        // {
        //     return;
        // };
        // bootImage.Source = bi.CreatedAvaloniaBitmap;
        // this._bitmap = bi.CreatedBitmap;
    }

    private async void StopImportImg_OnClick(object? sender, RoutedEventArgs e)
    {
        var box = MessageBoxManager.GetMessageBoxStandard("注意", "取消写入可能造成图片显示不完整，您要继续吗？", ButtonEnum.YesNo);
        var result = await box.ShowWindowDialogAsync(this);
        if (result == ButtonResult.No) return;
        _bootWri?.CancelWriteImg();
        _bootHid?.CancelWriteImg();
    }
}