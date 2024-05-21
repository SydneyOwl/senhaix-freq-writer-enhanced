using System;
using System.Collections.Generic;
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
using Avalonia.Platform.Storage;
using MsBox.Avalonia;
using SenhaixFreqWriter.Controls;
using SenhaixFreqWriter.DataModels.Gt12;
using SenhaixFreqWriter.Views.Common;

namespace SenhaixFreqWriter.Views.Plugin;

public partial class BootImageCreatorWindow : Window
{
    public SKBitmap CreatedBitmap;
    
    public Bitmap CreatedAvaloniaBitmap;
    
    private SHX_DEVICE _dev;
    public int BootImgWidth { get; set; }
    public int BootImgHeight { get; set; }
    public int WindowHeight { get; set; }
    public float defaultY { get; set; }

    private bool _stopUpdate;

    private int currentRow = 1;

    private int lastRowCount = 1;

    public ObservableCollection<string> fontList { get; set; } = new();

    public ObservableCollection<string> fontStyleList { get; set; } = new();

    private List<BootImgCreatorFontComponent> controls = new();

    public BootImageCreatorWindow(SHX_DEVICE device)
    {
        _dev = device;
        switch (_dev)
        {
            case SHX_DEVICE.SHX8X00:
                BootImgWidth = Constants.Shx8x00.OTHERS.BOOT_IMG_WIDTH;
                BootImgHeight = Constants.Shx8x00.OTHERS.BOOT_IMG_HEIGHT;
                WindowHeight = 500;
                break;
            case SHX_DEVICE.GT12:
                BootImgWidth = Constants.Gt12.OTHERS.BOOT_IMG_WIDTH;
                BootImgHeight = Constants.Gt12.OTHERS.BOOT_IMG_HEIGHT;
                WindowHeight = 390;
                break;
        }
        DebugWindow.GetInstance().updateDebugContent($"尺寸：{BootImgWidth}*{BootImgHeight}");
        InitializeFont();
        InitializeComponent();
        //TMPLLA
        DebugWindow.GetInstance().updateDebugContent($"添加控件");
        var compControl = this.FindControl<BootImgCreatorFontComponent>("CreatorComponent");
        compControl.AddHandler(BootImgCreatorFontComponent.UpdateEvent, (a, b) => UpdatePreview(compControl),
            RoutingStrategies.Bubble);
        compControl.AddHandler(BootImgCreatorFontComponent.ResetEvent, (a, b) => UpdatePreview(compControl, true),
            RoutingStrategies.Bubble);
        compControl.AddHandler(BootImgCreatorFontComponent.AddTextEvent, (a, b) => AddText(compControl),
            RoutingStrategies.Bubble);
        controls.Add(compControl);
        DataContext = this;
    }

    // Designer
    public BootImageCreatorWindow()
    {
        BootImgWidth = Constants.Gt12.OTHERS.BOOT_IMG_WIDTH;
        BootImgHeight = Constants.Gt12.OTHERS.BOOT_IMG_HEIGHT;
        WindowHeight = 500;
        InitializeFont();
        InitializeComponent();
        DataContext = this;
        // fontStyleComboBox.SelectedValue = "加粗";
        // fontComboBox.SelectedValue = "Arial";
    }

    private void InitializeFont()
    {
        //手动加入中文字体...
        fontList.Add("宋体");
        fontList.Add("黑体");
        fontList.Add("楷体");
        fontList.Add("仿宋");
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            DebugWindow.GetInstance().updateDebugContent($"Windows: ->InstalledFontCollection");
            var fonts = new InstalledFontCollection();
            foreach (var family in fonts.Families) fontList.Add(family.Name);
        }

        if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            // can't fetch list directly from macos syscall...
            // maybe insert list directly
        {
            DebugWindow.GetInstance().updateDebugContent($"macOS: ->OSX_AVAILABLE_FONTS.OSX_FONT_LIST");
            foreach (var se in OSX_OPTIONS.OSX_FONT_LIST)
                fontList.Add(se);
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
                using (var reader = process.StandardOutput)
                {
                    string line;
                    while ((line = reader.ReadLine()) != null) fontList.Add(line.Trim());
                }

                process.WaitForExit();
                if (process.ExitCode != 0)
                {
                    DebugWindow.GetInstance().updateDebugContent($"Linux获取字体失败,输出：{fontList[0]}");
                    MessageBoxManager.GetMessageBoxStandard("注意", "获取字体失败!").ShowWindowDialogAsync(this);
                    fontList.Clear();
                }
            }
            catch (Exception ex)
            {
                DebugWindow.GetInstance().updateDebugContent($"Linux获取字体失败：{ex.Message}");
                MessageBoxManager.GetMessageBoxStandard("注意", $"获取字体失败!{ex.Message}").ShowWindowDialogAsync(this);
            }
        }

        fontStyleList.Add("正常");
        fontStyleList.Add("加粗");
        fontStyleList.Add("斜体");
        fontStyleList.Add("加粗斜体");

        defaultY = BootImgHeight / 2;
    }

    private void UpdatePreview(BootImgCreatorFontComponent comp, bool resetCenter = false)
    {
        // DebugWindow.GetInstance().updateDebugContent($"触发更新");
        if (_stopUpdate) return;
        var bmp = new SKBitmap(BootImgWidth, BootImgHeight);
        var backColor = back.Color.ToSKColor();
        using (var canvas = new SKCanvas(bmp))
        {
            canvas.DrawColor(backColor);
            using (var sKPaint = new SKPaint())
            {
                foreach (var cmp in controls)
                {
                    var inputed = cmp.call.Text;
                    var fontColor = cmp.font.Color.ToSKColor();
                    var fontFamily = cmp.fontComboBox.SelectedValue == null
                        ? "宋体"
                        : cmp.fontComboBox.SelectedValue.ToString();
                    var fontStyle = SKTypefaceStyle.Normal;
                    if (cmp.fontStyleComboBox.SelectedValue != null)
                        switch (cmp.fontStyleComboBox.SelectedIndex)
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

                    sKPaint.Color = fontColor;
                    sKPaint.TextSize = (float)cmp.sizeSlider.Value;
                    sKPaint.IsAntialias = true;
                    sKPaint.Typeface = SKTypeface.FromFamilyName(fontFamily, fontStyle);
                    if (!resetCenter)
                    {
                        canvas.DrawText(inputed, (float)cmp.sizeSliderX.Value, (float)cmp.sizeSliderY.Value, sKPaint);
                    }
                    else
                    {
                        if (comp.Equals(cmp))
                        {
                            var size = new SKRect();
                            sKPaint.MeasureText(inputed, ref size);
                            var temp = (BootImgWidth - size.Size.Width) / 2;
                            var temp1 = (BootImgHeight - size.Size.Height) / 2;
                            cmp.sizeSliderX.Value = temp;
                            cmp.sizeSliderY.Value = temp1 - size.Top;
                            canvas.DrawText(inputed, temp, temp1 - size.Top, sKPaint);
                        }
                        else
                        {
                            canvas.DrawText(inputed, (float)cmp.sizeSliderX.Value, (float)cmp.sizeSliderY.Value,
                                sKPaint);
                        }
                    }
                }
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

    // private void Call_OnTextChanged(object? sender, TextChangedEventArgs e)
    // {
    //     UpdatePreview();
    // }

    private void Back_OnPropertyChanged(object? sender, AvaloniaPropertyChangedEventArgs e)
    {
        try
        {
            UpdatePreview(null);
        }
        catch
        {
            //ignore!
        }
    }
    // private void Font_OnPropertyChanged(object? sender, AvaloniaPropertyChangedEventArgs e)
    // {
    //     try
    //     {
    //         UpdatePreview();
    //     }
    //     catch
    //     {
    //         //ignore!
    //     }
    // }

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

    // private void SizeSlider_OnValueChanged(object? sender, RangeBaseValueChangedEventArgs e)
    // {
    //     try
    //     {
    //         UpdatePreview();
    //     }
    //     catch
    //     {
    //         //ignore!
    //     }
    // }
    //
    // private void FontComboBox_OnSelectionChanged(object? sender, SelectionChangedEventArgs e)
    // {
    //     try
    //     {
    //         UpdatePreview();
    //     }
    //     catch
    //     {
    //         // in theory only macos throws this...
    //         MessageBoxManager.GetMessageBoxStandard("注意", "该字体不受支持...").ShowWindowDialogAsync(this);
    //     }
    // }
    //
    // private void SetCenterButton_OnClick(object? sender, RoutedEventArgs e)
    // {
    //     try
    //     {
    //         UpdatePreview(true);
    //     }
    //     catch
    //     {
    //         //
    //     }
    // }

    private async void SaveFileToButton_OnClick(object? sender, RoutedEventArgs e)
    {
        var topLevel = GetTopLevel(this);
        var file = await topLevel.StorageProvider.SaveFilePickerAsync(new FilePickerSaveOptions
        {
            Title = "保存配置文件",
            SuggestedFileName = "boot_image_" + _dev + ".png"
        });
        if (file is not null)
        {
            try
            {
                await using var stream = await file.OpenWriteAsync();
                stream.Seek(0L, SeekOrigin.Begin);
                stream.SetLength(0L);
                CreatedBitmap.Encode(stream, SKEncodedImageFormat.Png, 100);
                stream.Close();
            }
            catch (UnauthorizedAccessException)
            {
                DebugWindow.GetInstance().updateDebugContent($"UNAUTHORIAZED");
                MessageBoxManager.GetMessageBoxStandard("注意", "目标目录无写权限，无法写入！").ShowWindowDialogAsync(this);
            }
            catch (Exception f)
            {
                DebugWindow.GetInstance().updateDebugContent($"{f.Message}");
                MessageBoxManager.GetMessageBoxStandard("注意", "出错！").ShowWindowDialogAsync(this);
            }
        }
    }

    private void AddText(BootImgCreatorFontComponent bt)
    {
        bt.addButton.IsVisible = false;
        var newRowDefinition = new RowDefinition() { Height = GridLength.Auto };
        fullGrid.RowDefinitions.Add(newRowDefinition);
        var compControl = new BootImgCreatorFontComponent();
        if (++lastRowCount % 4 == 0)
        {
            lastRowCount = 1;
            currentRow += 1;
        }

        Grid.SetRow(compControl, currentRow);
        Grid.SetColumn(compControl, lastRowCount);
        fullGrid.Children.Add(compControl);
        compControl.AddHandler(BootImgCreatorFontComponent.UpdateEvent, (a, b) => UpdatePreview(null),
            RoutingStrategies.Bubble);
        compControl.AddHandler(BootImgCreatorFontComponent.ResetEvent, (a, b) => UpdatePreview(compControl, true),
            RoutingStrategies.Bubble);
        compControl.AddHandler(BootImgCreatorFontComponent.AddTextEvent, (a, b) => AddText(compControl),
            RoutingStrategies.Bubble);
        controls.Add(compControl);
        DebugWindow.GetInstance().updateDebugContent($"新增子元素：BootImgCreatorC，行{currentRow}，列{lastRowCount}");
    }
}