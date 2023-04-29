using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace LD53;

public class MapViewer : View
{
    private Map map = new Map();

    public MapViewer(string path)
    {
        var json = File.ReadAllText(path);
        map = JsonSerializer.Deserialize<Map>(json);

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

public class MapEntrance
{
    public int X { get; set; }
    public int Y { get; set; }
    public int Height { get; set; }
    public int Width { get; set; }

    public Map Map { get; set; }
}

public class Map
{
    public List<MapPoint> Walls { get; set; } = new List<MapPoint>();
    public List<MapEntrance> Entrances { get; set; } = new List<MapEntrance>();
    public List<MapAsset> Assets { get; set; } = new List<MapAsset>();
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
public class MapAsset
{
    public List<MapPoint> Points { get; set; }
}