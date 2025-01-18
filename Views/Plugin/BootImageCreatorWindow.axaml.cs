using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Drawing.Text;
using System.IO;
using System.Runtime.InteropServices;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Platform.Storage;
using Avalonia.Skia;
using MsBox.Avalonia;
using SenhaixFreqWriter.Constants.Common;
using SenhaixFreqWriter.Constants.Shx8x00;
using SenhaixFreqWriter.Controls;
using SenhaixFreqWriter.Views.Common;
using SkiaSharp;
using Bitmap = Avalonia.Media.Imaging.Bitmap;

namespace SenhaixFreqWriter.Views.Plugin;

public partial class BootImageCreatorWindow : Window
{
    private readonly List<BootImgCreatorFontComponent> _controls = new();
    private readonly ShxDevice _dev;

    private int _currentRow = 1;

    private int _lastRowCount = 1;

    private bool _stopUpdate;

    public Bitmap CreatedAvaloniaBitmap;
    public SKBitmap CreatedBitmap;

    public BootImageCreatorWindow(ShxDevice device)
    {
        _dev = device;
        switch (_dev)
        {
            case ShxDevice.Shx8600Pro:
            case ShxDevice.Shx8800Pro:
            case ShxDevice.Shx8600:
            case ShxDevice.Shx8800:
                BootImgWidth = Others.BootImgWidth;
                BootImgHeight = Others.BootImgHeight;
                WindowHeight = 500;
                break;
            case ShxDevice.Gt12:
                BootImgWidth = Constants.Gt12.Others.BootImgWidth;
                BootImgHeight = Constants.Gt12.Others.BootImgHeight;
                WindowHeight = 390;
                break;
        }

        DebugWindow.GetInstance().UpdateDebugContent($"尺寸：{BootImgWidth}*{BootImgHeight}");
        InitializeFont();
        InitializeComponent();
        //TMPLLA
        DebugWindow.GetInstance().UpdateDebugContent("添加控件");
        var compControl = this.FindControl<BootImgCreatorFontComponent>("CreatorComponent");
        compControl.AddHandler(BootImgCreatorFontComponent.UpdateEvent, (a, b) => UpdatePreview(compControl),
            RoutingStrategies.Bubble);
        compControl.AddHandler(BootImgCreatorFontComponent.ResetEvent, (a, b) => UpdatePreview(compControl, true),
            RoutingStrategies.Bubble);
        compControl.AddHandler(BootImgCreatorFontComponent.AddTextEvent, (a, b) => AddText(compControl),
            RoutingStrategies.Bubble);
        _controls.Add(compControl);
        DataContext = this;
    }

    // Designer
    public BootImageCreatorWindow()
    {
        BootImgWidth = Constants.Gt12.Others.BootImgWidth;
        BootImgHeight = Constants.Gt12.Others.BootImgHeight;
        WindowHeight = 500;
        // InitializeFont();
        InitializeComponent();
        DataContext = this;
        // fontStyleComboBox.SelectedValue = "加粗";
        // fontComboBox.SelectedValue = "Arial";
    }

    public int BootImgWidth { get; set; }
    public int BootImgHeight { get; set; }
    public int WindowHeight { get; set; }
    public float DefaultY { get; set; }

    public ObservableCollection<string> FontList { get; set; } = new();

    public ObservableCollection<string> FontStyleList { get; set; } = new();

    private void InitializeFont()
    {
        //手动加入中文字体...
        if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
        {
            FontList.Add("华文宋体");
            FontList.Add("华文黑体");
            FontList.Add("华文楷体");
            FontList.Add("华文仿宋");
        }
        else
        {
            FontList.Add("宋体");
            FontList.Add("黑体");
            FontList.Add("楷体");
            FontList.Add("仿宋");
        }

        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            DebugWindow.GetInstance().UpdateDebugContent("Windows: ->InstalledFontCollection");
            var fonts = new InstalledFontCollection();
            foreach (var family in fonts.Families) FontList.Add(family.Name);
        }

        if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            // can't fetch list directly from macos syscall...
            // maybe insert list directly
        {
            DebugWindow.GetInstance().UpdateDebugContent("macOS: ->OSX_AVAILABLE_FONTS.OSX_FONT_LIST");
            foreach (var se in OsxOptions.OsxFontList)
                FontList.Add(se);
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
                    while ((line = reader.ReadLine()) != null) FontList.Add(line.Trim());
                }

                process.WaitForExit();
                if (process.ExitCode != 0)
                {
                    DebugWindow.GetInstance().UpdateDebugContent($"Linux获取字体失败,输出：{FontList[0]}");
                    MessageBoxManager.GetMessageBoxStandard("注意", "获取字体失败!").ShowWindowDialogAsync(this);
                    FontList.Clear();
                }
            }
            catch (Exception ex)
            {
                DebugWindow.GetInstance().UpdateDebugContent($"Linux获取字体失败：{ex.Message}");
                MessageBoxManager.GetMessageBoxStandard("注意", $"获取字体失败!{ex.Message}").ShowWindowDialogAsync(this);
            }
        }

        FontStyleList.Add("正常");
        FontStyleList.Add("加粗");
        FontStyleList.Add("斜体");
        FontStyleList.Add("加粗斜体");

        DefaultY = BootImgHeight / 2;
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
                foreach (var cmp in _controls)
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
                DebugWindow.GetInstance().UpdateDebugContent("UNAUTHORIAZED");
                MessageBoxManager.GetMessageBoxStandard("注意", "目标目录无写权限，无法写入！").ShowWindowDialogAsync(this);
            }
            catch (Exception f)
            {
                DebugWindow.GetInstance().UpdateDebugContent($"{f.Message}");
                MessageBoxManager.GetMessageBoxStandard("注意", "出错！").ShowWindowDialogAsync(this);
            }
    }

    private void AddText(BootImgCreatorFontComponent bt)
    {
        bt.addButton.IsVisible = false;
        var newRowDefinition = new RowDefinition { Height = GridLength.Auto };
        fullGrid.RowDefinitions.Add(newRowDefinition);
        var compControl = new BootImgCreatorFontComponent();
        if (++_lastRowCount % 4 == 0)
        {
            _lastRowCount = 1;
            _currentRow += 1;
        }

        Grid.SetRow(compControl, _currentRow);
        Grid.SetColumn(compControl, _lastRowCount);
        fullGrid.Children.Add(compControl);
        compControl.AddHandler(BootImgCreatorFontComponent.UpdateEvent, (a, b) => UpdatePreview(null),
            RoutingStrategies.Bubble);
        compControl.AddHandler(BootImgCreatorFontComponent.ResetEvent, (a, b) => UpdatePreview(compControl, true),
            RoutingStrategies.Bubble);
        compControl.AddHandler(BootImgCreatorFontComponent.AddTextEvent, (a, b) => AddText(compControl),
            RoutingStrategies.Bubble);
        _controls.Add(compControl);
        DebugWindow.GetInstance().UpdateDebugContent($"新增子元素：BootImgCreatorC，行{_currentRow}，列{_lastRowCount}");
    }
}