using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;

namespace LD53;

public class MapConstructor : View
{
    private readonly string _name;
    private Map map = new();
    private int x = 1;
    private int y = 1;
    private bool drawing = false;

    private static List<MapAsset> assets = new()
    {
        new() { Points = new List<MapPoint> { new(0, 0, ConsoleChars.VLine) }, Type = MapAssetType.Wall},
        new() { Points = new List<MapPoint> { new(0, 0, ConsoleChars.HLine) }, Type = MapAssetType.Wall },
        new() { Points = new List<MapPoint> { new(0, 0, ConsoleChars.CornerBottomLeft) }, Type = MapAssetType.Wall },
        new() { Points = new List<MapPoint> { new(0, 0, ConsoleChars.CornerBottomRight) }, Type = MapAssetType.Wall },
        new() { Points = new List<MapPoint> { new(0, 0, ConsoleChars.CornerTopRight) }, Type = MapAssetType.Wall },
        new() { Points = new List<MapPoint> { new(0, 0, ConsoleChars.CornerTopLeft) }, Type = MapAssetType.Wall },
        new() { Points = new List<MapPoint> { new(0, 0, ConsoleChars.LeftTee) }, Type = MapAssetType.Wall },
        new() { Points = new List<MapPoint> { new(0, 0, ConsoleChars.RightTee) }, Type = MapAssetType.Wall },
        new() { Points = new List<MapPoint> { new(0, 0, ConsoleChars.TopTee) }, Type = MapAssetType.Wall },
        new() { Points = new List<MapPoint> { new(0, 0, ConsoleChars.BottomTee) }, Type = MapAssetType.Wall },

        MapAsset.LoadFromAssets("Table"),
        MapAsset.LoadFromAssets("Box"),
        MapAsset.LoadFromAssets("Key"),
        MapAsset.LoadFromAssets("HDoor"),
        MapAsset.LoadFromAssets("VDoor"),
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

    private int selectedAsset = 0;
    
    public override bool KeyPressed(ConsoleKeyInfo key)
    {

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
                map.UpdateLookup();
                if (map.Lookup.ContainsKey($"{Bound.X + x},{Bound.Y + y}"))
                {
                    var existing = map.Lookup[$"{Bound.X + x},{Bound.Y + y}"];
                    _ = existing.Type switch
                    {
                        MapAssetType.Wall => map.Walls.Remove(existing.Points[0]),
                        MapAssetType.None => map.Text.Remove(existing.Points[0]),
                        MapAssetType.Asset => map.Assets.Remove(existing),
                        // MapAssetType.Entrance => map.Entrances.Remove(existing),
                    };
                }
                var selected = map.Walls.FirstOrDefault(c => c.X == Bound.X + x && c.Y == Bound.Y + y);
                if (selected == null)
                    break;
                map.Walls.Remove(selected);
                break;
            case ConsoleKey.D1:
                if (selectedAsset > 0)
                    selectedAsset--;
                break;
            case ConsoleKey.D2:
                if (selectedAsset < assets.Count - 1)
                    selectedAsset++;
                break;
        }

        if ((key.Modifiers & ConsoleModifiers.Control) != 0 && key.Key == ConsoleKey.S)
        {
            // Save
            var json  = JsonSerializer.Serialize(map);
            File.WriteAllText(_name, json);
            Dialog.Show("Save", "Saved map");
        }

        if (key.Key >= ConsoleKey.A && key.Key <= ConsoleKey.Z)
        {
            var selected = map.Text.FirstOrDefault(c => c.X == Bound.X + x && c.Y == Bound.Y + y);
            if (selected != null)
                selected.C = key.KeyChar;
            else
                map.Text.Add(new (Bound.X + x, Bound.Y + y, key.KeyChar));
            if (x < Bound.Width - 2)
                x++;
        }
        
        if (drawing)
        {
            var asset = assets[selectedAsset];
            switch (asset.Type)
            {
                case MapAssetType.Wall:
                    foreach (var point in asset.Points)
                        map.Walls.Add(new (Bound.X + x + point.X, Bound.Y + y + point.Y, point.C));
                    break;
                case MapAssetType.Asset:
                    var newAsset = new MapAsset()
                    {
                        Points = asset.Points.Select(p => new MapPoint(p.X + x, p.Y + y, p.C)).ToList(),
                        Type = asset.Type,
                    };
                    map.Assets.Add(newAsset);
                    break;
            }
            // selected = map.Walls.FirstOrDefault(c => c.X == Bound.X + x && c.Y == Bound.Y + y);
            // if (selected != null)
            //     selected.C = chars[selectedAsset];
            // else
            //     map.Walls.Add(new (Bound.X + x, Bound.Y + y, chars[selectedAsset]));
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
        
        // Draw assets
        foreach (var asset in map.Assets)
        {
            foreach (var point in asset.Points)
            {
                Move(point.X, point.Y);
                Draw(point.C);
            }
        }
        
        // Draw text
        foreach (var point in map.Text)
        {
            Move(point.X, point.Y);
            Draw(point.C);
        }

        // Draw cursor
        Move(Bound.X + x, Bound.Y + y);
        var selected = assets[selectedAsset];
        foreach (var point in selected.Points)
        {
            Move(Bound.X + x + point.X, Bound.Y + y + point.Y);
            Draw(point.C);
        }
    }
}