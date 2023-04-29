using System;
using System.Threading;

namespace LD53;

public class Dialog : Frame
{
    private string Message;
    private Action Ok;

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

        return false;
    }

    public override void Render()
    {
        base.Render();

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