using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.Json;

namespace LD53;

public class MapViewer : View
{
    public Map map = new Map();

    public MapViewer(string path)
    {
        if (File.Exists(path))
            map = Map.Load(path);
        var person = new Person();
        AddChildView(person);
        person.Bound.X = 24;
        person.Bound.Y = 4;
    }


    public override void Render()
    {
        foreach (var point in map.Walls)
        {
            Move(point.X, point.Y);
            Draw(point.C);
        }
    }
}

public class Map
{
    public List<MapPoint> Walls { get; set; } = new List<MapPoint>();
    public List<MapPoint> Text { get; set; } = new List<MapPoint>();
    public List<MapEntrance> Entrances { get; set; } = new List<MapEntrance>();
    public List<MapAsset> Assets { get; set; } = new List<MapAsset>();

    public Dictionary<string, MapAsset> Lookup { get; set; }

    public static Map Load(string path)
    {
        var json = File.ReadAllText(path);
        var map = JsonSerializer.Deserialize<Map>(json);
        map.UpdateLookup();
        return map;
    }
    
    public void UpdateLookup()
    {
        Lookup = new Dictionary<string, MapAsset>();
        foreach (var wall in Walls)
            Lookup[$"{wall.X},{wall.Y}"] = new() { Points = new List<MapPoint> { wall }, Type = MapAssetType.Wall};
        // foreach (var entrance in Entrances)
        //     Lookup[$"{entrance.X},{entrance.Y}"] = entrance;
        foreach (var asset in Assets)
            foreach (var point in asset.Points)
                Lookup[$"{point.X},{point.Y}"] = asset;
        foreach (var point in Text)
            Lookup[$"{point.X},{point.Y}"] = new() { Points = new List<MapPoint> { point }, Type = MapAssetType.None };
    }

    public object Collides(Bounds bound)
    {
        var x1 = $"{bound.X},{bound.Y}";
        var x2 = $"{bound.X + bound.Width - 1},{bound.Y}";
        // Check corners
        if (Lookup.ContainsKey(x1))
            return Lookup[x1];
        if (Lookup.ContainsKey(x2))
            return Lookup[x2];

        return null;
    }
}
public class MapEntrance
{
    public int X { get; set; }
    public int Y { get; set; }
    public int Height { get; set; }
    public int Width { get; set; }

    public Map Map { get; set; }
}
public class MapPoint
{
    public int X { get; set; }
    public int Y { get; set; }
    public char C { get; set; }

    public MapPoint(int x, int y, char c)
    {
        X = x;
        Y = y;
        C = c;
    }
}

public enum MapAssetType
{
    Wall,
    Entrance,
    Asset,
    None
}

public class MapAsset
{
    public MapAssetType Type { get; set; }
    public List<MapPoint> Points { get; set; } = new List<MapPoint>();

    public static MapAsset LoadFromAssets(string asset)
    {
        var mapAsset = new MapAsset();
        mapAsset.Type = MapAssetType.Asset;
        
        using var stream = Assembly.GetEntryAssembly()!.GetManifestResourceStream("LD53.Game.Assets." + asset);
        using var reader = new StreamReader(stream);
        var lines = reader.ReadToEnd().Replace("\r", "").Split("\n");

        for (int y = 0; y < lines.Length; y++)
            for (int x = 0; x < lines[y].Length; x++)
                mapAsset.Points.Add(new MapPoint(x, y, lines[y][x]));

        return mapAsset;
    }
}