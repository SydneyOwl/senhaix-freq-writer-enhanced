using System;
using System.Collections.Generic;
using System.Threading;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using Avalonia.Platform.Storage;
using Avalonia.Threading;
using MsBox.Avalonia;
using SenhaixFreqWriter.Utils.Serial;
using SkiaSharp;

namespace SenhaixFreqWriter.Views.Shx8x00.Plugin;

public partial class BootImageImportWindow : Window
{

    private SKBitmap bitmap;
    private CancellationTokenSource ctx;
    private WriBootImage boot;
    public BootImageImportWindow()
    {
        InitializeComponent();
        DataContext = this;
    }

    private async void OpenImageButton_OnClick(object? sender, RoutedEventArgs e)
    {
        var topLevel = GetTopLevel(this);
        var fileType = new FilePickerFileType("image");
        fileType.Patterns = new[] { "*.bmp", "*.jpg" };
        var files = await topLevel.StorageProvider.OpenFilePickerAsync(new FilePickerOpenOptions
        {
            Title = "打开备份",
            AllowMultiple = false,
            FileTypeFilter = new List<FilePickerFileType>()
            {
                fileType
            }
        });
        if (files.Count == 0)
        {
            return;
        }
        var bitmap = SKBitmap.Decode(files[0].Path.AbsolutePath);
        if (bitmap.Width!=128 || bitmap.Height!=128)
        {
            MessageBoxManager.GetMessageBoxStandard("注意", "图片尺寸不符合要求！").ShowWindowDialogAsync(this);
            return;
        }
        if (!bitmap.ColorType.Equals(SKColorType.Bgra8888))
        {
            MessageBoxManager.GetMessageBoxStandard("注意", "图片像素格式不符合要求！").ShowWindowDialogAsync(this);
            return;
        }
        this.bitmap = bitmap;
        bootImage.Source = new Bitmap(files[0].Path.AbsolutePath);
    }

    private async void ImportButton_OnClick(object? sender, RoutedEventArgs e)
    {
        ctx = new CancellationTokenSource();
        pgBar.Value = 0;
        start.IsEnabled = false;
        if (bitmap == null)
        {
            MessageBoxManager.GetMessageBoxStandard("注意", "请选择图片！").ShowWindowDialogAsync(this);
            start.IsEnabled = true;
            return;
        }
        if (MySerialPort.GetInstance().TargetPort == "" && MySerialPort.GetInstance().WriteBle == null)
        {
            MessageBoxManager.GetMessageBoxStandard("注意", "请连接蓝牙或写频线！").ShowWindowDialogAsync(this);
            start.IsEnabled = true;
            return;
        }

        boot = new WriBootImage(bitmap);
        new Thread(()=>
        {
            startWrite(ctx);
        }).Start();
        new Thread(()=>
        {
            startGetProcess(ctx.Token);
        }).Start();
    }

    private async void startWrite(CancellationTokenSource source)
    {
        var res = await boot.WriteImg();
        Dispatcher.UIThread.Invoke(() =>
        {
            start.IsEnabled = true;
        });
        source.Cancel();
        Dispatcher.UIThread.Invoke(() =>
        {
            if (res)
            {
                MessageBoxManager.GetMessageBoxStandard("注意", "成功！").ShowWindowDialogAsync(this);
            }
            else
            {
                MessageBoxManager.GetMessageBoxStandard("注意", "失败！").ShowWindowDialogAsync(this);
            }
            start.IsEnabled = true;
        });
    }

    private void startGetProcess(CancellationToken token)
    {
        while (!token.IsCancellationRequested)
        {
            int curPct;
            if (!boot.currentProg.TryDequeue(out curPct))
            {
                continue;
            }
            Dispatcher.UIThread.Post(() =>
            {
                pgBar.Value = curPct;
            });
            Thread.Sleep(10);
        }
    }
}