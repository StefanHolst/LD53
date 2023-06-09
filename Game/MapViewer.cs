using System.IO;

namespace LD53;

public class MapViewer : View
{
    public Map map = new Map();

    public MapViewer(string name)
    {
        map = Map.Load(name);
        GameState.Person = new Person();
        AddChildView(GameState.Person);
        GameState.Person.Bound.X = 24;
        GameState.Person.Bound.Y = 6;
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
    }
}