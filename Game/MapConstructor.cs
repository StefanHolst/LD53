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
        new() { Points = new List<MapPoint> { new(0, 0, '/') }, Type = MapAssetType.Wall },
        new() { Points = new List<MapPoint> { new(0, 0, '\\') }, Type = MapAssetType.Wall },

        
        MapAsset.LoadFromAssets("Table"),
        MapAsset.LoadFromAssets("Chair"),
        MapAsset.LoadFromAssets("BoxA"),
        MapAsset.LoadFromAssets("BoxSlotA"),
        MapAsset.LoadFromAssets("BoxB"),
        MapAsset.LoadFromAssets("BoxSlotB"),
        MapAsset.LoadFromAssets("BoxC"),
        MapAsset.LoadFromAssets("BoxSlotC"),
        MapAsset.LoadFromAssets("BoxD"),
        MapAsset.LoadFromAssets("BoxSlotD"),
        MapAsset.LoadFromAssets("BoxE"),
        MapAsset.LoadFromAssets("BoxSlotE"),
        MapAsset.LoadFromAssets("BoxF"),
        MapAsset.LoadFromAssets("BoxSlotF"),
        MapAsset.LoadFromAssets("Key"),
        MapAsset.LoadFromAssets("Glass"),
        MapAsset.LoadFromAssets("Chest"),
        MapAsset.LoadFromAssets("MorseChest"),
        MapAsset.LoadFromAssets("JapanChest"),
        MapAsset.LoadFromAssets("CountChest"),
        MapAsset.LoadFromAssets("LastChest"),
        MapAsset.LoadFromAssets("HDoor"),
        MapAsset.LoadFromAssets("VDoor"),
        MapAsset.LoadFromAssets("BoxKey1"),
        MapAsset.LoadFromAssets("BoxKey2"),
        MapAsset.LoadFromAssets("BoxKey3"),
    };
    
    public MapConstructor(string name)
    {
        _name = name;
        if (File.Exists($"Maps/{_name}.map"))
            map = Map.Load(_name);
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
                        _ => map.Entrances.Remove((MapEntrance)existing),
                    };
                }
                break;
            case ConsoleKey.D1:
                if (selectedAsset > 0)
                    selectedAsset--;
                break;
            case ConsoleKey.D2:
                if (selectedAsset < assets.Count - 1)
                    selectedAsset++;
                break;
            case ConsoleKey.D3:
                // Insert entrance
                var name = Dialog.ShowInput("Entrance", "Enter entrance name");
                if (string.IsNullOrWhiteSpace(name))
                    break;
                var mapName = Dialog.ShowInput("Entrance", "Enter map name");
                if (string.IsNullOrWhiteSpace(mapName))
                    break;
                var dir = Dialog.ShowInput("Entrance", "Enter direction (D, S)");
                if (string.IsNullOrWhiteSpace(dir))
                    break;
                var entrance = new MapEntrance { Name = name, MapName = mapName, Destination = dir == "D"};
                entrance.Points.Add(new MapPoint(Bound.X + x, Bound.Y + y, 'x'));
                map.Entrances.Add(entrance);
                break;
        }

        // Save
        if ((key.Modifiers & ConsoleModifiers.Control) != 0 && key.Key == ConsoleKey.S)
        {
            var json  = JsonSerializer.Serialize(map);
            File.WriteAllText($"Maps/{_name}.map", json);
            Dialog.Show("Save", "Saved map");
            return false;
        }

        // Text
        if ((key.Key >= ConsoleKey.A && key.Key <= ConsoleKey.Z) || key.KeyChar == '?' || key.KeyChar == '.' || key.KeyChar == '!' || key.KeyChar == ',')
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
            map.UpdateLookup();
            if (map.Lookup.ContainsKey($"{Bound.X + x},{Bound.Y + y}"))
            {
                var existing = map.Lookup[$"{Bound.X + x},{Bound.Y + y}"];
                switch (existing.Type)
                {
                    case MapAssetType.Wall:
                        map.Walls.Remove(existing.Points[0]);
                        break;
                    case MapAssetType.None:
                        map.Text.Remove(existing.Points[0]);
                        break;
                    case MapAssetType.Asset:
                        map.Assets.Remove(existing);
                        break;
                    case MapAssetType.Entrance:
                        // map.Entrances.Remove((MapEntrance)existing);
                        break;
                }
            }
            
            switch (asset.Type)
            {
                case MapAssetType.Wall:
                    foreach (var point in asset.Points)
                        map.Walls.Add(new (Bound.X + x + point.X, Bound.Y + y + point.Y, point.C));
                    if (asset.Points.Count > 1)
                        drawing = false;
                    break;
                case MapAssetType.Asset:
                    var newAsset = new MapAsset()
                    {
                        Points = asset.Points.Select(p => new MapPoint(p.X + x, p.Y + y, p.C)).ToList(),
                        Type = asset.Type,
                        Name = asset.Name
                    };
                    map.Assets.Add(newAsset);
                    drawing = false;
                    break;
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
        
        // Draw entrance
        foreach (var entrance in map.Entrances)
        {
            foreach (var point in entrance.Points)
            {
                Move(point.X, point.Y);
                Draw(point.C);
            }
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