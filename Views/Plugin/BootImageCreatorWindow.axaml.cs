using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Interactivity;
using Avalonia.Skia;
using SenhaixFreqWriter.Constants.Common;
using SkiaSharp;
using Bitmap = Avalonia.Media.Imaging.Bitmap;

using System.Drawing;
using System.Drawing.Text;
using System.Runtime.InteropServices;
using MsBox.Avalonia;

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

    public ObservableCollection<String> fontList { get; set; } = new();
    
    public ObservableCollection<String> fontStyleList { get; set; } = new();
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
        InitializeFont();
        InitializeComponent();
        DataContext = this;
        fontStyleComboBox.SelectedValue = "加粗";
        fontComboBox.SelectedValue = "Arial";
    }
    
    // Designer
    public BootImageCreatorWindow()
    {
        BootImgWidth = Constants.Shx8x00.OTHERS.BOOT_IMG_WIDTH;
        BootImgHeight = Constants.Shx8x00.OTHERS.BOOT_IMG_HEIGHT;
        WindowHeight = 300;
        InitializeFont();
        InitializeComponent();
        DataContext = this;
        fontStyleComboBox.SelectedValue = "加粗";
        fontComboBox.SelectedValue = "Arial";
    }

    private void InitializeFont()
    {
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            InstalledFontCollection fonts = new InstalledFontCollection();
            foreach (FontFamily family in fonts.Families)
            {
                fontList.Add(family.Name);
            }
        }
        if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
        {
            // can't fetch list directly from macos syscall...
            // maybe insert list directly
            foreach (var se in OSX_AVALIABLE_FONTS.OSX_FONT_LIST)
            {
                fontList.Add(se);
            }
        }
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
        {
            var process = new Process();
            process.StartInfo.FileName = "fc-list";
            process.StartInfo.Arguments = ": family"; 
            process.StartInfo.UseShellExecute = false; 
            process.StartInfo.RedirectStandardOutput = true; 
            process.StartInfo.RedirectStandardError = false;
            try
            {
                process.Start();
                using (StreamReader reader = process.StandardOutput)
                {
                    string line;
                    while ((line = reader.ReadLine()) != null)
                    {
                        fontList.Add(line.Trim());
                    }
                }
                process.WaitForExit();
                if (process.ExitCode != 0)
                {
                    MessageBoxManager.GetMessageBoxStandard("注意", "获取字体失败!").ShowWindowDialogAsync(this);
                }
            }
            catch (Exception ex)
            {
                MessageBoxManager.GetMessageBoxStandard("注意", $"获取字体失败!{ex.Message}").ShowWindowDialogAsync(this);
            }
        }
        fontStyleList.Add("正常");
        fontStyleList.Add("加粗");
        fontStyleList.Add("斜体");
        fontStyleList.Add("加粗斜体");
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
                var fontFamily = fontComboBox.SelectedValue == null ? "宋体" : fontComboBox.SelectedValue.ToString();
                var fontStyle = SKTypefaceStyle.Normal;
                if (fontStyleComboBox.SelectedValue!=null)
                {
                    switch (fontStyleComboBox.SelectedIndex)
                    {
                        case 0:
                            fontStyle = SKTypefaceStyle.Normal;
                            break;
                        case 1:
                            fontStyle = SKTypefaceStyle.Bold;
                            break;
                        case 2:
                            fontStyle = SKTypefaceStyle.Italic; 
                            break;
                        case 3:
                            fontStyle = SKTypefaceStyle.BoldItalic;
                            break;
                    }
                }
                sKPaint.Color = fontColor;
                sKPaint.TextSize = (float)sizeSlider.Value;
                sKPaint.IsAntialias = true;
                sKPaint.Typeface = SKTypeface.FromFamilyName(fontFamily, fontStyle);
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

    private void SizeSlider_OnValueChanged(object? sender, RangeBaseValueChangedEventArgs e)
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

    private void FontComboBox_OnSelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        try
        {
            UpdatePreview();
        }
        catch
        {
            // in theory only macos throws this...
            MessageBoxManager.GetMessageBoxStandard("注意", "该字体不受支持...").ShowWindowDialogAsync(this);
        }
    }
}