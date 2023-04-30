using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace LD53;

public class Map
{
    public string Name { get; set; }
    public List<MapPoint> Walls { get; set; } = new List<MapPoint>();
    public List<MapPoint> Text { get; set; } = new List<MapPoint>();
    public List<MapEntrance> Entrances { get; set; } = new List<MapEntrance>();
    public List<MapAsset> Assets { get; set; } = new List<MapAsset>();

    [JsonIgnore]
    public Dictionary<string, MapAsset> Lookup { get; set; }

    public static Map Load(string name)
    {
        var json = File.ReadAllText($"Maps/{name}.map");
        var map = JsonSerializer.Deserialize<Map>(json);
        map.Name = name;
        map.UpdateLookup();
        return map;
    }
    
    public void UpdateLookup()
    {
        Lookup = new Dictionary<string, MapAsset>();
        foreach (var wall in Walls)
            Lookup[$"{wall.X},{wall.Y}"] = new() { Points = new List<MapPoint> { wall }, Type = MapAssetType.Wall};
        foreach (var entrance in Entrances)
            Lookup[$"{entrance.Points[0].X},{entrance.Points[0].Y}"] = entrance;
        foreach (var asset in Assets)
            foreach (var point in asset.Points)
                Lookup[$"{point.X},{point.Y}"] = asset;
        foreach (var point in Text)
            Lookup[$"{point.X},{point.Y}"] = new() { Points = new List<MapPoint> { point }, Type = MapAssetType.None };
    }

    public MapAsset Collides(Bounds bound)
    {
        MapAsset collides;
        // Check top
        for (int x = 0; x < bound.Width; x++)
        {
            var point = $"{bound.X + x},{bound.Y}";
            if (Lookup.TryGetValue(point, out collides))
                return collides;
        }
        
        // Check bottom
        for (int x = 0; x < bound.Width; x++)
        {
            var point = $"{bound.X + x},{bound.Y + bound.Height - 1}";
            if (Lookup.TryGetValue(point, out collides))
                return collides;
        }
        
        // Check Left
        for (int y = 0; y < bound.Height; y++)
        {
            var point = $"{bound.X},{bound.Y + y}";
            if (Lookup.TryGetValue(point, out collides))
                return collides;
        }
        
        // Check Right
        for (int y = 0; y < bound.Height; y++)
        {
            var point = $"{bound.X + bound.Width - 1},{bound.Y + y}";
            if (Lookup.TryGetValue(point, out collides))
                return collides;
        }
        
        return null;
    }
}
public class MapEntrance : MapAsset
{
    public MapEntrance()
    {
        Type = MapAssetType.Entrance;
    }
    public string Name { get; set; }
    public string MapName { get; set; }
    public bool Destination { get; set; }
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