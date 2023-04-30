using System.Collections.Generic;
using System.Linq;
using LD53.AssetLogic;

namespace LD53;

public static class GameState
{
    public static List<Map> LoadedMaps { get; set; } = new List<Map>();
    public static Map CurrentMap { get; set; }
    public static MapViewer Viewer { get; set; }
    public static MapAsset Bag { get; set; }
    public static List<MapAsset> Keys { get; set; } = new();
    public static Person Person { get; set; }
    public static List<Direction> TakenDirections { get; set; } = new();

    public static int DoorsOpened { get; set; }
    public static int ChestOpened { get; set; }
    public static int PackagesDelivered { get; set; }


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

    public static void CheckStats()
    {
        if (DoorsOpened == 6)
        {
            // Update box
            var map = LoadedMaps.FirstOrDefault(m => m.Name == "Start");
            var box = map.Assets.FirstOrDefault(a => a.Name == "BoxKey1");
            var text = box.Points.FirstOrDefault(p => p.C == '?');
            if (text != null)
                text.C = '6';
        }

        if (ChestOpened == 3)
        {
            // Update box
            var map = LoadedMaps.FirstOrDefault(m => m.Name == "Start");
            var box = map.Assets.FirstOrDefault(a => a.Name == "BoxKey2");
            var text = box.Points.FirstOrDefault(p => p.C == '?');
            if (text != null)
                text.C = '3';
        }

        if (PackagesDelivered == 5)
        {
            // Update box
            var map = LoadedMaps.FirstOrDefault(m => m.Name == "Start");
            var box = map.Assets.FirstOrDefault(a => a.Name == "BoxKey3");
            var text = box.Points.FirstOrDefault(p => p.C == '?');
            if (text != null)
                text.C = '5';
        }
    }
}