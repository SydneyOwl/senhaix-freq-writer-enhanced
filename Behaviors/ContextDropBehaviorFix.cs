using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Xaml.Interactions.DragAndDrop;
using Avalonia.Xaml.Interactivity;

namespace SenhaixFreqWriter.Behaviors;

/// <summary>
///     A quick fix from https://github.com/AvaloniaUI/Avalonia.Xaml.Behaviors/pull/176/files
/// </summary>
public class ContextDropBehaviorFix : Behavior<Control>
{
    public static string DataFormat = nameof(Context);

    public static readonly StyledProperty<object?> ContextProperty =
        AvaloniaProperty.Register<ContextDropBehavior, object>(nameof(Context));

    public static readonly StyledProperty<IDropHandler?> HandlerProperty =
        AvaloniaProperty.Register<ContextDropBehavior, IDropHandler>(nameof(Handler));


    public object? Context
    {
        get => GetValue<object>(ContextDropBehavior.ContextProperty);
        set => SetValue<object>(ContextDropBehavior.ContextProperty, value);
    }

    public IDropHandler? Handler
    {
        get => GetValue<IDropHandler>(ContextDropBehavior.HandlerProperty);
        set => SetValue<IDropHandler>(ContextDropBehavior.HandlerProperty, value);
    }


    protected override void OnAttachedToVisualTree()
    {
        if (AssociatedObject != null)
            DragDrop.SetAllowDrop(AssociatedObject, true);
        AssociatedObject?.AddHandler(DragDrop.DragEnterEvent, DragEnter);
        AssociatedObject?.AddHandler(DragDrop.DragLeaveEvent, DragLeave);
        AssociatedObject?.AddHandler(DragDrop.DragOverEvent, DragOver);
        AssociatedObject?.AddHandler(DragDrop.DropEvent, Drop);
    }


    protected override void OnDetachedFromVisualTree()
    {
        if (AssociatedObject != null)
            DragDrop.SetAllowDrop(AssociatedObject, false);
        AssociatedObject?.RemoveHandler(DragDrop.DragEnterEvent, DragEnter);
        AssociatedObject?.RemoveHandler(DragDrop.DragLeaveEvent, DragLeave);
        AssociatedObject?.RemoveHandler(DragDrop.DragOverEvent, DragOver);
        AssociatedObject?.RemoveHandler(DragDrop.DropEvent, Drop);
    }

    private void DragEnter(object? sender, DragEventArgs e)
    {
        if (!IsExpectedFormatAvailable(e)) return;

        var sourceContext = e.Data.Get(ContextDropBehavior.DataFormat);
        var targetContext = Context ?? AssociatedObject?.DataContext;
        Handler?.Enter(sender, e, sourceContext, targetContext);
    }

    private void DragLeave(object? sender, RoutedEventArgs e)
    {
        Handler?.Leave(sender, e);
    }

    private void DragOver(object? sender, DragEventArgs e)
    {
        if (!IsExpectedFormatAvailable(e)) return;

        var sourceContext = e.Data.Get(ContextDropBehavior.DataFormat);
        var targetContext = Context ?? AssociatedObject?.DataContext;
        Handler?.Over(sender, e, sourceContext, targetContext);
    }

    private void Drop(object? sender, DragEventArgs e)
    {
        if (!IsExpectedFormatAvailable(e)) return;

        var sourceContext = e.Data.Get(ContextDropBehavior.DataFormat);
        var targetContext = Context ?? AssociatedObject?.DataContext;
        Handler?.Drop(sender, e, sourceContext, targetContext);
    }

    private static bool IsExpectedFormatAvailable(DragEventArgs e)
    {
        var availableFormats = e.Data.GetDataFormats();
        if (availableFormats.Contains(ContextDropBehavior.DataFormat)) return true;

        e.Handled = true;
        e.DragEffects = DragDropEffects.None;
        return false;
    }
}