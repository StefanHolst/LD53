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
        
        var mapConstructorStart = new MapConstructor("Start");
        mapConstructorStart.SetBound(0,0,0,0, Alignment.Strech, Alignment.Strech);
        var mapConstructorMorse = new MapConstructor("Morse");
        mapConstructorMorse.SetBound(0,0,0,0, Alignment.Strech, Alignment.Strech);
        var mapConstructorJapan = new MapConstructor("Japan");
        mapConstructorJapan.SetBound(0,0,0,0, Alignment.Strech, Alignment.Strech);
        var mapConstructorMaze = new MapConstructor("Maze");
        mapConstructorMaze.SetBound(0,0,0,0, Alignment.Strech, Alignment.Strech);
        var mapConstructorTime = new MapConstructor("Time");
        mapConstructorTime.SetBound(0,0,0,0, Alignment.Strech, Alignment.Strech);
        var mapConstructorWin = new MapConstructor("Win");
        mapConstructorWin.SetBound(0,0,0,0, Alignment.Strech, Alignment.Strech);
        
        var mapViewer = new MapViewer("Start");
        mapViewer.SetBound(0,0,0,0, Alignment.Strech, Alignment.Strech);
        
        // Add menu
        var menu = new Menu();
        menu.SetBound(0,0,0,0, Alignment.Fill, Alignment.Fill);
        menu.AddOption(mapViewer);
        menu.AddOption(mapConstructorStart);
        menu.AddOption(mapConstructorMorse);
        menu.AddOption(mapConstructorJapan);
        menu.AddOption(mapConstructorMaze);
        menu.AddOption(mapConstructorTime);
        menu.AddOption(mapConstructorWin);

        GameState.LoadedMaps.Add(mapViewer.map);
        GameState.CurrentMap = mapViewer.map;
        GameState.Viewer = mapViewer;
        
        var CancellationTokenSource = new CancellationTokenSource();
        Console.CancelKeyPress += (sender, args) => CancellationTokenSource.Cancel();
        
        consoleRender.Run(mapViewer, CancellationTokenSource.Token);
    }
}
