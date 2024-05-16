using System;
using System.IO;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Avalonia.Media.Imaging;
using Avalonia.Skia;
using SenhaixFreqWriter.Constants.Common;
using SkiaSharp;

namespace SenhaixFreqWriter.Views.Plugin;

public partial class BootImageCreatorWindow : Window
{
    public SKBitmap CreatedBitmap;
    public Bitmap CreatedAvaloniaBitmap;
    private SHX_DEVICE _dev;
    public int BootImgWidth { get; set; }
    public int BootImgHeight { get; set; }
    public int WindowHeight { get; set; }

    private bool _stopUpdate;
    public BootImageCreatorWindow(SHX_DEVICE device)
    {
        _dev = device;
        switch (_dev)
        {
            case SHX_DEVICE.SHX8X00:
                BootImgWidth = Constants.Shx8x00.OTHERS.BOOT_IMG_WIDTH;
                BootImgHeight = Constants.Shx8x00.OTHERS.BOOT_IMG_HEIGHT;
                WindowHeight = 300;
                break;
            case SHX_DEVICE.GT12:
                BootImgWidth = Constants.Gt12.OTHERS.BOOT_IMG_WIDTH;
                BootImgHeight = Constants.Gt12.OTHERS.BOOT_IMG_HEIGHT;
                WindowHeight = 390;
                break;
        }
        InitializeComponent();
        DataContext = this;
    }

    private void UpdatePreview()
    {
        if (_stopUpdate)
        {
            return;
        }
        var callsign = call.Text;
        var backColor = back.Color.ToSKColor();
        var fontColor = font.Color.ToSKColor();
        SKBitmap bmp = new SKBitmap(BootImgWidth, BootImgHeight);
        using (SKCanvas canvas = new SKCanvas(bmp))
        {
            canvas.DrawColor(backColor);
            using (SKPaint sKPaint = new SKPaint())
            {
                sKPaint.Color = fontColor;
                sKPaint.TextSize = 39;
                sKPaint.IsAntialias = true;
                sKPaint.Typeface = SKTypeface.FromFamilyName("宋体", SKTypefaceStyle.Bold);
                SKRect size = new SKRect();
                sKPaint.MeasureText(callsign, ref size);
                float temp = (BootImgWidth - size.Size.Width) / 2;
                float temp1 = (BootImgHeight - size.Size.Height) / 2;
                canvas.DrawText(callsign, temp, temp1 - size.Top, sKPaint);
            }
        }
        using (var stream = new MemoryStream())
        {
            bmp.Encode(stream, SKEncodedImageFormat.Png, 100);
            stream.Position = 0;
            var bm = new Bitmap(stream);
            bootImage.Source = bm;
            CreatedAvaloniaBitmap = bm;
            CreatedBitmap = bmp;
        }
    }

    private void Call_OnTextChanged(object? sender, TextChangedEventArgs e)
    {
        UpdatePreview();
    }

    private void Back_OnDataContextChanged(object? sender, EventArgs e)
    {
        UpdatePreview();
    }

    private void Font_OnDataContextChanged(object? sender, EventArgs e)
    {
        UpdatePreview();
    }

    private void Back_OnPropertyChanged(object? sender, AvaloniaPropertyChangedEventArgs e)
    {
        try
        {
            UpdatePreview();
        }
        catch
        {
            //ignore!
        }
    }
    private void Font_OnPropertyChanged(object? sender, AvaloniaPropertyChangedEventArgs e)
    {
        try
        {
            UpdatePreview();
        }
        catch
        {
            //ignore!
        }
    }

    private void ConfirmButton_OnClick(object? sender, RoutedEventArgs e)
    {
        Close();
    }

    private void AbortButton_OnClick(object? sender, RoutedEventArgs e)
    {
        _stopUpdate = true;
        CreatedAvaloniaBitmap = null;
        CreatedBitmap = null;
        Close();
    }
}