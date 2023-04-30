using System;
using System.Threading;

namespace LD53;

public class Dialog : Frame
{
    private string Message;
    private Action Ok;
    private Action Cancel;

    public static void Show(string title, string message)
    {
        var dialog = new Dialog(title, message);
        dialog.Bound.Width = message.Length + 6;
        dialog.Bound.Height = 3;
        dialog.Bound.VerticalAlignment = Alignment.Center;
        dialog.Bound.HorizontalAlignment = Alignment.Center;
        
        var render = new ConsoleRender();
        var tokenSource = new CancellationTokenSource();
        
        // wait for input
        dialog.Ok = () =>
        {
            tokenSource.Cancel();
        };
        dialog.Cancel = () =>
        {
            tokenSource.Cancel();
        };
        
        render.Run(dialog, tokenSource.Token);
        ConsoleRender.RedrawEverything();
    }
    public static string ShowInput(string title, string message)
    {
        var dialog = new Dialog(title, message);
        dialog.Bound.Width = message.Length + 6;
        dialog.Bound.Height = 8;
        dialog.Bound.VerticalAlignment = Alignment.Center;
        dialog.Bound.HorizontalAlignment = Alignment.Center;

        var textBox = new TextBox();
        textBox.SetBound(0, 3, 0, 3, Alignment.Strech, Alignment.End);
        dialog.AddChildView(textBox);
        
        var render = new ConsoleRender();
        var tokenSource = new CancellationTokenSource();
        
        // wait for input
        dialog.Ok = () =>
        {
            tokenSource.Cancel();
        };
        dialog.Cancel = () =>
        {
            tokenSource.Cancel();
        };
        
        render.Run(dialog, tokenSource.Token);
        ConsoleRender.RedrawEverything();
        
        return textBox.Content;
    }
    public static void ShowView(string title, string message, View view)
    {
        var dialog = new Dialog(title, message);
        dialog.AddChildView(view);
        dialog.Bound.Width = Math.Max(message.Length, view.Bound.Width) + 6;
        dialog.Bound.Height = Math.Max(8, view.Bound.Height);
        dialog.Bound.VerticalAlignment = Alignment.Center;
        dialog.Bound.HorizontalAlignment = Alignment.Center;
        
        var render = new ConsoleRender();
        var tokenSource = new CancellationTokenSource();
        
        // wait for input
        dialog.Ok = () =>
        {
            tokenSource.Cancel();
        };
        dialog.Cancel = () =>
        {
            tokenSource.Cancel();
        };
        
        render.Run(dialog, tokenSource.Token);
        ConsoleRender.RedrawEverything();
    }

    public override bool PreviewKeyPressed(ConsoleKeyInfo key)
    {
        if (key.Key == ConsoleKey.Enter)
        {
            Ok?.Invoke();
            return true;
        }
        if (key.Key == ConsoleKey.Escape)
        {
            Cancel?.Invoke();
            return true;
        }

        return false;
    }

    public override void Render()
    {
        base.Render();

        if (Message.Length == 0)
            return;
        for (int i = 0; i < Message.Length; i++)
        {
            Move(i + 2, 1);
            Draw(Message[i]);
        }
    }

    public Dialog(string title, string message) : base(title)
    {
        Message = message;
    }
}