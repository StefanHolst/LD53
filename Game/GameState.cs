using System.Collections.Generic;
using System.Linq;

namespace LD53;

public static class GameState
{
    public static List<Map> LoadedMaps { get; set; } = new List<Map>();
    public static Map CurrentMap { get; set; }
    public static MapViewer Viewer { get; set; }

    public static void ChangeMap(MapEntrance entrance)
    {
        // Find the map
        if (LoadedMaps.Any(m => m.Name == entrance.MapName && m.Entrances.Any(e => e.Name == entrance.Name)))
        {
            CurrentMap = LoadedMaps.First(m => m.Name == entrance.MapName);
        }
        else
        {
            CurrentMap = Map.Load(entrance.MapName);
            LoadedMaps.Add(CurrentMap);
        }
        
        // Update the viewer
        Viewer.map = CurrentMap;
    }
}