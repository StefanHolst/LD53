using System;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;

namespace LD53;

public class Person : View
{
    private string figure;
    private string message = null;

    private void LoadFigure(string figure)
    {
        using var stream = Assembly.GetEntryAssembly().GetManifestResourceStream("LD53.Game.Figures." + figure);
        using var reader = new StreamReader(stream);
        this.figure = reader.ReadToEnd();
    }

    public Person()
    {
        Bound.Width = 7;
        Bound.Height = 3;
        LoadFigure("NORMAL");
    }
    
    public override bool KeyPressed(ConsoleKeyInfo key)
    {
        switch (key.Key)
        {
            case ConsoleKey.LeftArrow:
                Bound.X--;
                LoadFigure("MAD");
                CheckCollision();
                break;
            case ConsoleKey.RightArrow:
                Bound.X++;
                LoadFigure("NORMAL");
                CheckCollision();
                break;
            case ConsoleKey.UpArrow:
                Bound.Y--;
                LoadFigure("SAD");
                CheckCollision();
                break;
            case ConsoleKey.DownArrow:
                Bound.Y++;
                LoadFigure("NORMAL");
                CheckCollision();
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

    private void CheckCollision()
    {
        var map = GameState.CurrentMap;
        var collision = map.Collides(Bound);
        if (collision != null)
            ShowMessage($"collided {collision.GetType().Name}");
    }
}