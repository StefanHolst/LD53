using System;
using System.Text;
using System.Threading;

namespace LD53;

public class TextBox : Frame
{
    public bool MultiLine { get; set; }
    public string Content => content.ToString();

    private StringBuilder content = new StringBuilder();
    
    public TextBox(string title = "TextBox") : base(title)
    {
        Bound.Height = 3;
    }

    public override bool KeyPressed(ConsoleKeyInfo key)
    {
        if (key.Key == ConsoleKey.Enter && MultiLine == false)
        {
            // Exit? done event
        }
        
        if (content.Length > 0 && (key.Key == ConsoleKey.Backspace || key.Key == ConsoleKey.Delete))
            content.Length -= 1;
        else if (char.IsLetterOrDigit(key.KeyChar) || key.Key == ConsoleKey.Spacebar)
            content.Append(key.KeyChar);

        return false;
    }

    protected int Length => Math.Min(content.Length, Bound.Width - 3);
    public bool ShowCurser { get; set; } = true;
    
    public override void Render()
    {
        base.Render();

        int offset = 0;
        if (content.Length > Bound.Width - 2)
            offset = content.Length - (Bound.Width - 2);

        if (ShowCurser)
            DrawCursor();

        for (int x = 0; x < Length; x++)
        {
            Move(x + 1, 1);
            Draw(content[x + offset]);
        }
    }

    private int curserCount = 0;
    private bool showVisible = false;
    private void DrawCursor()
    {
        curserCount++;
        if (curserCount == 15)
        {
            showVisible = !showVisible;
            curserCount = 0;
        }
        if (showVisible)
        {
            Move(Length + 1, 1);
            Draw('|');
        }
    }
}