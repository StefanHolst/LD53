using System;
using System.IO;
using System.Linq;
using System.Text.Json;

namespace LD53;

public class MapConstructor : View
{
    private readonly string _name;
    private Map map = new Map();
    private int x = 1;
    private int y = 1;
    private bool drawing = false;

    private static char[] chars = new[]
    {
        ConsoleChars.VLine,
        ConsoleChars.HLine,
        ConsoleChars.CornerBottomLeft,
        ConsoleChars.CornerBottomRight,
        ConsoleChars.CornerTopRight,
        ConsoleChars.CornerTopLeft,
        ConsoleChars.LeftTee,
        ConsoleChars.RightTee,
        ConsoleChars.TopTee,
        ConsoleChars.BottomTee,
        '/',
        '\\',
        
    };

    public MapConstructor(string name)
    {
        _name = name;
        if (File.Exists(_name))
        {
            var json = File.ReadAllText(_name);
            map = JsonSerializer.Deserialize<Map>(json);
        }
    }

    private int selectedChar = 0;
    
    public override bool KeyPressed(ConsoleKeyInfo key)
    {
        var selected = map.Walls.FirstOrDefault(c => c.X == Bound.X + x && c.Y == Bound.Y + y);

        switch (key.Key)
        {
            case ConsoleKey.LeftArrow:
                if (x > Bound.X)
                    x--;
                break;
            case ConsoleKey.RightArrow:
                if (x < Bound.Width - 2)
                    x++;
                break;
            case ConsoleKey.UpArrow:
                if (y > 0)
                    y--;
                break;
            case ConsoleKey.DownArrow:
                if (y < Bound.Height - 1)
                   y++;
                break;
            case ConsoleKey.Spacebar:
                drawing = !drawing;
                break;
            case ConsoleKey.Backspace:
                if (selected == null)
                    break;
                map.Walls.Remove(selected);
                break;
            case ConsoleKey.D1:
                if (selectedChar > 0)
                    selectedChar--;
                break;
            case ConsoleKey.D2:
                if (selectedChar < chars.Length - 1)
                    selectedChar++;
                break;
        }

        if ((key.Modifiers & ConsoleModifiers.Control) != 0 && key.Key == ConsoleKey.S)
        {
            // Save
            var json  = JsonSerializer.Serialize(map);
            File.WriteAllText(_name, json);
            Dialog.Show("Save", "Saved map");
        }

        if (drawing)
        {
            if (key.Key >= ConsoleKey.A && key.Key <= ConsoleKey.Z)
            {
                if (selected != null)
                    selected.C = key.KeyChar;
                else
                    map.Walls.Add(new (Bound.X + x, Bound.Y + y, key.KeyChar));
                if (x < Bound.Width - 2)
                    x++;
            }
            else
            {
                selected = map.Walls.FirstOrDefault(c => c.X == Bound.X + x && c.Y == Bound.Y + y);
                if (selected != null)
                    selected.C = chars[selectedChar];
                else
                    map.Walls.Add(new (Bound.X + x, Bound.Y + y, chars[selectedChar]));
            }
        }

        return false;
    }
    
    public override void Render()
    {
        // Draw walls
        foreach (var point in map.Walls)
        {
            Move(point.X, point.Y);
            Draw(point.C);
        }

        // Draw cursor
        Move(Bound.X + x, Bound.Y + y);
        // if (DateTime.Now.Second % 2 == 0)
            Draw(chars[selectedChar]);
        // else
        //     Draw('#');
    }
}