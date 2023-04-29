using System;
using System.Threading.Tasks;

namespace LD53;

public class Person : View
{
    const string NORMAL = @" /\_/\
⟨_o_o_⟩
 ⟨u_u⟩
";
    const string SAD = @" /\_/\
⟨_ó_ò_⟩
 ⟨u_u⟩
";
    const string MAD = @" /\_/\
⟨_ò_ó_⟩
 ⟨u_u⟩
";

    private string figure = NORMAL;

    private string message = null;

    public override bool KeyPressed(ConsoleKeyInfo key)
    {
        switch (key.Key)
        {
            case ConsoleKey.LeftArrow:
                Bound.X--;
                figure = MAD;
                break;
            case ConsoleKey.RightArrow:
                Bound.X++;
                figure = NORMAL;
                break;
            case ConsoleKey.UpArrow:
                Bound.Y--;
                figure = SAD;
                break;
            case ConsoleKey.DownArrow:
                Bound.Y++;
                figure = NORMAL;
                break;
            case ConsoleKey.Spacebar:
                ShowMessage("heelllo");
                break;
        }

        return false;
    }
    public override void Render()
    {
        // Draw figure
        int x = 0;
        int y = 0;
        for (int i = 0; i < figure.Length; i++)
        {
            if (figure[i] == '\r')
                continue;
            if (figure[i] == '\n')
            {
                x = 0;
                y++;
                continue;
            }
            Move(x, y);
            Draw(figure[i]);
            x++;
        }

        if (message == null)
            return;

        bool msgTop = Bound.Y > 4;
        bool msgLeft = Bound.X + message.Length + 5 < Parent.Bound.Width;

        int xOffset = msgLeft ? 0 : -message.Length + 3;
        int yOffset = msgTop ? -4 : 4;
        
        // Draw Bubble
        Move(0 + xOffset, 0 + yOffset);
        Draw('╭');
        Move(message.Length + 3 + xOffset, 0 + yOffset);
        Draw('╮');
        Move(0 + xOffset, 2 + yOffset);
        Draw('╰');
        Move(message.Length + 3 + xOffset, 2 + yOffset);
        Draw('╯');
        Move(message.Length + 3 + xOffset, 1 + yOffset);
        Draw('│');
        Move(0 + xOffset, 1 + yOffset);
        Draw('│');

        DrawString(1 + xOffset, (msgTop ? 0 : 2) + yOffset, new string('◠', message.Length + 2));
        DrawString((msgLeft ? 1 : message.Length + 1) + xOffset, (msgTop ? 2 : 0) + yOffset, new string('◠', 2));
        DrawString((msgLeft ? 5 : 1) + xOffset, (msgTop ? 2 : 0) + yOffset, new string('◠', message.Length - 2));
        
        Move((msgLeft ? 3 : message.Length - 1) + xOffset, (msgTop ? 2 : 0) + yOffset);
        Draw(msgTop ? '\\' : '/');
        Move((msgLeft ? 4 : message.Length) + xOffset, (msgTop ? 2 : 0) + yOffset);
        Draw(msgTop ? '/' : '\\');
        
        DrawString(2 + xOffset, 1 + yOffset, message);
    }

    public void ShowMessage(string message)
    {
        this.message = message;
        Task.Run(async () =>
        {
            await Task.Delay(5000);
            this.message = null;
        });
    }
}