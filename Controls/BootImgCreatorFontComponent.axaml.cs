using System;
using System.Collections.ObjectModel;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

namespace SenhaixFreqWriter.Controls;

public partial class BootImgCreatorFontComponent : UserControl
{
    public BootImgCreatorFontComponent()
    {
        InitializeComponent();
    }
    public static readonly RoutedEvent<RoutedEventArgs> UpdateEvent =
        RoutedEvent.Register<BootImgCreatorFontComponent, RoutedEventArgs>("UpdateEvent", RoutingStrategies.Bubble);
    
    public static readonly RoutedEvent<RoutedEventArgs> ResetEvent =
        RoutedEvent.Register<BootImgCreatorFontComponent, RoutedEventArgs>("ResetEvent", RoutingStrategies.Bubble);
    
    public static readonly RoutedEvent<RoutedEventArgs> AddTextEvent =
        RoutedEvent.Register<BootImgCreatorFontComponent, RoutedEventArgs>("AddTextEvent", RoutingStrategies.Bubble);

    public void RaiseUpdateEvent()
    {
        RaiseEvent(new RoutedEventArgs(UpdateEvent));
    }

    public void RaiseResetEvent()
    {
        RaiseEvent(new RoutedEventArgs(ResetEvent));
    }
    
    public void RaiseAddTextEvent()
    {
        RaiseEvent(new RoutedEventArgs(AddTextEvent));
    }
    private void Call_OnTextChanged(object? sender, TextChangedEventArgs e)
    {
        RaiseUpdateEvent();
    }

    private void Font_OnPropertyChanged(object? sender, AvaloniaPropertyChangedEventArgs e)
    {
        RaiseUpdateEvent();
    }
    private void FontComboBox_OnSelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        RaiseUpdateEvent();
    }

    private void FontStyleComboBox_OnSelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        RaiseUpdateEvent();
    }

    private void SizeSlider_OnValueChanged(object? sender, RangeBaseValueChangedEventArgs e)
    {
        RaiseUpdateEvent();
    }

    private void SizeSliderX_OnValueChanged(object? sender, RangeBaseValueChangedEventArgs e)
    {
        RaiseUpdateEvent();
    }

    private void SizeSliderY_OnValueChanged(object? sender, RangeBaseValueChangedEventArgs e)
    {
        RaiseUpdateEvent();
    }

    private void ResetButton_OnClick(object? sender, RoutedEventArgs e)
    {
        RaiseResetEvent();
    }

    private void AddTextButton_OnClick(object? sender, RoutedEventArgs e)
    {
        RaiseAddTextEvent();
    }
}