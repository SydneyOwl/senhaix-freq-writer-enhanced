﻿using System.Collections.Generic;
using System.Threading;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Media.Imaging;
using Avalonia.Platform.Storage;
using Avalonia.Threading;
using MsBox.Avalonia;
using SenhaixFreqWriter.Constants.Common;
using SenhaixFreqWriter.Constants.Shx8x00;
using SenhaixFreqWriter.Utils.HID;
using SenhaixFreqWriter.Utils.Serial;
using SenhaixFreqWriter.Views.Common;
using SkiaSharp;

namespace SenhaixFreqWriter.Views.Plugin;

public partial class BootImageImportWindow : Window
{
    private SKBitmap _bitmap;
    private HIDBootImage _bootHid;
    private WriBootImage _bootWri;
    private CancellationTokenSource _ctx;
    private SHX_DEVICE _device = SHX_DEVICE.SHX8600;

    public BootImageImportWindow(SHX_DEVICE dev)
    {
        _device = dev;
        switch (dev)
        {
            case SHX_DEVICE.SHX8600:
            case SHX_DEVICE.SHX8600PRO:
            case SHX_DEVICE.SHX8800:
                BootImgWidth = OTHERS.BOOT_IMG_WIDTH;
                BootImgHeight = OTHERS.BOOT_IMG_HEIGHT;
                Hint = $"尺寸限制：{BootImgWidth}x{BootImgHeight}，建议bmp格式";
                WindowHeight = 200;
                break;
            case SHX_DEVICE.GT12:
                BootImgWidth = Constants.Gt12.OTHERS.BOOT_IMG_WIDTH;
                BootImgHeight = Constants.Gt12.OTHERS.BOOT_IMG_HEIGHT;
                Hint = $"尺寸限制：{BootImgWidth}x{BootImgHeight}，建议bmp格式";
                WindowHeight = 390;
                break;
        }

        DebugWindow.GetInstance().updateDebugContent($"尺寸：{BootImgWidth}*{BootImgHeight}");
        InitializeComponent();
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
        var bitmap = SKBitmap.Decode(files[0].Path.AbsolutePath);
        if (bitmap == null)
        {
            MessageBoxManager.GetMessageBoxStandard("注意", "出错，请检查您的路径，应当为纯英文，或软件对图片无读权限！").ShowWindowDialogAsync(this);
            return;
        }

        DebugWindow.GetInstance().updateDebugContent($"PicFormat：{bitmap.ColorType}");
        if (!bitmap.ColorType.Equals(SKColorType.Bgra8888) && !bitmap.ColorType.Equals(SKColorType.Rgba8888))
        {
            MessageBoxManager.GetMessageBoxStandard("注意", "图片像素格式不符合要求！").ShowWindowDialogAsync(this);
            return;
        }

        DebugWindow.GetInstance().updateDebugContent($"图片尺寸：{bitmap.Width}*{bitmap.Height}");
        if ((bitmap.Width != OTHERS.BOOT_IMG_WIDTH ||
             bitmap.Height != OTHERS.BOOT_IMG_HEIGHT) && (_device != SHX_DEVICE.GT12))
        {
            MessageBoxManager.GetMessageBoxStandard("注意", "图片尺寸不符合要求！").ShowWindowDialogAsync(this);
            return;
        }

        if ((bitmap.Width != Constants.Gt12.OTHERS.BOOT_IMG_WIDTH ||
             bitmap.Height != Constants.Gt12.OTHERS.BOOT_IMG_HEIGHT) && _device == SHX_DEVICE.GT12)
        {
            MessageBoxManager.GetMessageBoxStandard("注意", "图片尺寸不符合要求！").ShowWindowDialogAsync(this);
            return;
        }

        bootImage.Source = new Bitmap(files[0].Path.AbsolutePath);
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

        if (_device !=SHX_DEVICE.GT12)
        {
            if (MySerialPort.GetInstance().TargetPort == "" && MySerialPort.GetInstance().WriteBle == null)
            {
                MessageBoxManager.GetMessageBoxStandard("注意", "请连接蓝牙或写频线！").ShowWindowDialogAsync(this);
                return;
            }

            start.IsEnabled = false;
            _bootWri = new WriBootImage(_device,_bitmap);
            new Thread(() => { StartWrite8800(_ctx); }).Start();
            new Thread(() => { StartGetProcess8800(_ctx.Token); }).Start();
        }

        if (_device == SHX_DEVICE.GT12)
        {
            if (!HidTools.GetInstance().IsDeviceConnected && HidTools.GetInstance().WriteBle == null)
            {
                MessageBoxManager.GetMessageBoxStandard("注意", "请连接蓝牙或写频线！").ShowWindowDialogAsync(this);
                start.IsEnabled = true;
                return;
            }

            _bootHid = new HIDBootImage(_bitmap);
            new Thread(() => { StartWriteGt12(_ctx); }).Start();
            new Thread(() => { StartGetProcessGt12(_ctx.Token); }).Start();
        }
    }

    private async void StartWrite8800(CancellationTokenSource source)
    {
        DebugWindow.GetInstance().updateDebugContent("Start WriImg Thread: StartWrite8800");
        var res = _bootWri.WriteImg();
        Dispatcher.UIThread.Invoke(() => { start.IsEnabled = true; });
        source.Cancel();
        Dispatcher.UIThread.Invoke(() =>
        {
            if (res)
                MessageBoxManager.GetMessageBoxStandard("注意", "成功！").ShowWindowDialogAsync(this);
            else
                MessageBoxManager.GetMessageBoxStandard("注意", "失败！").ShowWindowDialogAsync(this);
            // 不用ResetBLE，因为更改开机图片不变更蓝牙状态
            start.IsEnabled = true;
        });

        DebugWindow.GetInstance().updateDebugContent("Terminate WriImg Thread: StartWrite8800");
    }

    private void StartGetProcess8800(CancellationToken token)
    {
        DebugWindow.GetInstance().updateDebugContent("Start WriImg Thread: StartGetProcess8800");
        while (!token.IsCancellationRequested)
        {
            int curPct;
            if (!_bootWri.currentProg.TryDequeue(out curPct)) continue;
            Dispatcher.UIThread.Post(() => { pgBar.Value = curPct; });
            // Thread.Sleep(10);
        }

        DebugWindow.GetInstance().updateDebugContent("Terminate WriImg Thread: StartGetProcess8800");
    }

    private async void StartWriteGt12(CancellationTokenSource source)
    {
        DebugWindow.GetInstance().updateDebugContent("Start WriImg Thread: StartWriteGt12");
        var res = _bootHid.WriteImg();
        Dispatcher.UIThread.Invoke(() => { start.IsEnabled = true; });
        source.Cancel();
        Dispatcher.UIThread.Invoke(() =>
        {
            if (res)
                MessageBoxManager.GetMessageBoxStandard("注意", "成功！").ShowWindowDialogAsync(this);
            else
                MessageBoxManager.GetMessageBoxStandard("注意", "失败！").ShowWindowDialogAsync(this);
            start.IsEnabled = true;
        });
        DebugWindow.GetInstance().updateDebugContent("Terminate WriImg Thread: StartWriteGt12");
    }

    private void StartGetProcessGt12(CancellationToken token)
    {
        DebugWindow.GetInstance().updateDebugContent("Start WriImg Thread: StartGetProcessGt12");
        while (!token.IsCancellationRequested)
        {
            int curPct;
            if (!_bootHid.currentProg.TryDequeue(out curPct)) continue;
            Dispatcher.UIThread.Post(() => { pgBar.Value = curPct; });
        }

        DebugWindow.GetInstance().updateDebugContent("Terminate WriImg Thread: StartGetProcessGt12");
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
}