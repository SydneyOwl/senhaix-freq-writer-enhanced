using System.Collections.Generic;
using System.Threading;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Media.Imaging;
using Avalonia.Platform.Storage;
using Avalonia.Threading;
using MsBox.Avalonia;
using SenhaixFreqWriter.Constants.Common;
using SenhaixFreqWriter.Utils.HID;
using SenhaixFreqWriter.Utils.Serial;
using SkiaSharp;

namespace SenhaixFreqWriter.Views.Plugin;

public partial class BootImageImportWindow : Window
{
    private SKBitmap _bitmap;
    private CancellationTokenSource _ctx;
    private WriBootImage _bootWri;
    private HIDBootImage _bootHid;
    private SHX_DEVICE _device = SHX_DEVICE.SHX8X00;
    public int bootImgWidth { get; set; }
    public int bootImgHeight { get; set; }
    
    public int windowHeight { get; set; }
    
    public string hint { get; set; }

    public BootImageImportWindow(SHX_DEVICE dev)
    {
        _device = dev;
        switch (dev)
        {
            case SHX_DEVICE.SHX8X00:
                bootImgWidth = Constants.Shx8x00.OTHERS.BOOT_IMG_WIDTH;
                bootImgHeight = Constants.Shx8x00.OTHERS.BOOT_IMG_HEIGHT;
                hint = $"尺寸限制：{bootImgWidth}x{bootImgHeight}，建议bmp格式";
                windowHeight = 200;
                break;
            case SHX_DEVICE.GT12:
                bootImgWidth = Constants.Gt12.OTHERS.BOOT_IMG_WIDTH;
                bootImgHeight = Constants.Gt12.OTHERS.BOOT_IMG_HEIGHT;
                hint = $"尺寸限制：{bootImgWidth}x{bootImgHeight}，建议bmp格式";
                windowHeight = 390;
                break;
        }
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
        if (files.Count == 0) return;
        var bitmap = SKBitmap.Decode(files[0].Path.AbsolutePath);

        if (!bitmap.ColorType.Equals(SKColorType.Bgra8888) && !bitmap.ColorType.Equals(SKColorType.Rgba8888))
        {
            MessageBoxManager.GetMessageBoxStandard("注意", "图片像素格式不符合要求！").ShowWindowDialogAsync(this);
            return;
        }
        
        if ((bitmap.Width != Constants.Shx8x00.OTHERS.BOOT_IMG_WIDTH || bitmap.Height != Constants.Shx8x00.OTHERS.BOOT_IMG_HEIGHT)&&_device==SHX_DEVICE.SHX8X00)
        {
            MessageBoxManager.GetMessageBoxStandard("注意", "图片尺寸不符合要求！").ShowWindowDialogAsync(this);
            return;
        }
        
        if ((bitmap.Width != Constants.Gt12.OTHERS.BOOT_IMG_WIDTH || bitmap.Height != Constants.Gt12.OTHERS.BOOT_IMG_HEIGHT)&&_device==SHX_DEVICE.GT12)
        {
            MessageBoxManager.GetMessageBoxStandard("注意", "图片尺寸不符合要求！").ShowWindowDialogAsync(this);
            return;
        }

        bootImage.Source = new Bitmap(files[0].Path.AbsolutePath);
        this._bitmap = bitmap;
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

        if (_device == SHX_DEVICE.SHX8X00)
        {
            if (MySerialPort.GetInstance().TargetPort == "" && MySerialPort.GetInstance().WriteBle == null)
            {
                MessageBoxManager.GetMessageBoxStandard("注意", "请连接蓝牙或写频线！").ShowWindowDialogAsync(this);
                return;
            }
            start.IsEnabled = false;
            _bootWri = new WriBootImage(_bitmap);
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
        var res = await _bootWri.WriteImg();
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
    }

    private void StartGetProcess8800(CancellationToken token)
    {
        while (!token.IsCancellationRequested)
        {
            int curPct;
            if (!_bootWri.currentProg.TryDequeue(out curPct)) continue;
            Dispatcher.UIThread.Post(() => { pgBar.Value = curPct; });
            // Thread.Sleep(10);
        }
    }
    
    private async void StartWriteGt12(CancellationTokenSource source)
    {
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
    }

    private void StartGetProcessGt12(CancellationToken token)
    {
        while (!token.IsCancellationRequested)
        {
            int curPct;
            if (!_bootHid.currentProg.TryDequeue(out curPct)) continue;
            Dispatcher.UIThread.Post(() => { pgBar.Value = curPct; });
        }
    }
}