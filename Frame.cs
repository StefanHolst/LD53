using System;

namespace LD53;

public class Frame : View
{
    public string Title { get; set; }

    public Frame(string title)
    {
        Title = title;
    }

    public override void Render()
    {
        // Draw corners
        Move(0,0);
        Draw(ConsoleChars.CornerTopLeft);
        Move(Bound.Width-1, 0);
        Draw(ConsoleChars.CornerTopRight);
        Move(0, Bound.Height - 1);
        Draw(ConsoleChars.CornerBottomLeft);
        Move(Bound.Width - 1, Bound.Height - 1);
        Draw(ConsoleChars.CornerBottomRight);

        // Draw horizontal lines
        if (Title != null && Bound.Width - (Title.Length + 2) >= 0)
        {
            var text = $" {Title} " + new string(ConsoleChars.HLine, Bound.Width - (Title.Length + 2));
            for (int x = 0; x < Bound.Width - 2; x++)
            {
                Move(x + 1 ,0);
                Draw(text[x]);
                Move(x + 1,Bound.Height-1);
                Draw(ConsoleChars.HLine);
            }
        }
        else
        {
            for (int x = 1; x < Bound.Width - 1; x++)
            {
                Move(x ,0);
                Draw(ConsoleChars.HLine);
                Move(x ,Bound.Height-1);
                Draw(ConsoleChars.HLine);
            }
        }
        
        // Draw vertical lines
        for (int y = 1; y < Bound.Height - 1; y++)
        {
            Move(0 ,y);
            Draw(ConsoleChars.VLine);
            Move(Bound.Width - 1, y);
            Draw(ConsoleChars.VLine);
        }
    }
}