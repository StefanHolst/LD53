using System;
using System.IO;
using System.Linq;
using System.Threading;

namespace LD53;

public class Program
{
    public static void Main(string[] args)
    {
        Console.Clear();
        var consoleRender = new ConsoleRender();
        
        var mapConstructor = new MapConstructor("Start");
        mapConstructor.SetBound(0,0,0,0, Alignment.Strech, Alignment.Strech);

        var mapViewer = new MapViewer("Morse");
        mapViewer.SetBound(0,0,0,0, Alignment.Strech, Alignment.Strech);
        
        // Add menu
        var menu = new Menu();
        menu.SetBound(0,0,0,0, Alignment.Fill, Alignment.Fill);
        menu.AddOption(mapConstructor);
        menu.AddOption(mapViewer);

        GameState.LoadedMaps.Add(mapViewer.map);
        GameState.CurrentMap = mapViewer.map;
        GameState.Viewer = mapViewer;
        
        var CancellationTokenSource = new CancellationTokenSource();
        Console.CancelKeyPress += (sender, args) => CancellationTokenSource.Cancel();
        
        consoleRender.Run(menu, CancellationTokenSource.Token);
    }
}
