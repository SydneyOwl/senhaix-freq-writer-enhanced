using System;
using System.Collections.Generic;

namespace SenhaixFreqWriter.Utils.Other;

public class UndoRedoStack<T>
{
    private Stack<T> undoStack;
    private Stack<T> redoStack;

    public UndoRedoStack()
    {
        undoStack = new Stack<T>();
        redoStack = new Stack<T>();
    }

    // 执行操作并压入 undo 栈
    public void Execute(T item)
    {
        undoStack.Push(item);
        // 每次执行新的操作时，清空 redo 栈
        redoStack.Clear();
    }

    // 撤销操作
    public T Undo()
    {
        if (undoStack.Count == 0)
        {
            throw new InvalidOperationException("No operations to undo.");
        }

        T item = undoStack.Pop();
        redoStack.Push(item);
        return item;
    }

    // 重做操作
    public T Redo()
    {
        if (redoStack.Count == 0)
        {
            throw new InvalidOperationException("No operations to redo.");
        }

        T item = redoStack.Pop();
        undoStack.Push(item);
        return item;
    }

    // 查看 undo 栈顶部元素
    public T PeekUndo()
    {
        if (undoStack.Count == 0)
        {
            throw new InvalidOperationException("Undo stack is empty.");
        }
        return undoStack.Peek();
    }

    // 查看 redo 栈顶部元素
    public T PeekRedo()
    {
        if (redoStack.Count == 0)
        {
            throw new InvalidOperationException("Redo stack is empty.");
        }
        return redoStack.Peek();
    }

    // 返回栈的大小
    public int UndoCount => undoStack.Count;
    public int RedoCount => redoStack.Count;
}
