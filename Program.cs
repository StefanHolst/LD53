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
        
        var mapConstructor = new MapConstructor("Maps/Morse.map");
        mapConstructor.SetBound(0,0,0,0, Alignment.Strech, Alignment.Strech);

        if (args.Length > 0 && File.Exists(args.FirstOrDefault()) == false)
            throw new FileNotFoundException(args.FirstOrDefault());
        var map = new MapViewer(args.FirstOrDefault() ?? "Maps/Morse.map");
        map.SetBound(0,0,0,0, Alignment.Strech, Alignment.Strech);
        
        // Add menu
        var menu = new Menu();
        menu.SetBound(0,0,0,0, Alignment.Fill, Alignment.Fill);
        menu.AddOption(map);
        menu.AddOption(mapConstructor);

        GameState.CurrentMap = map.map;
        
        var CancellationTokenSource = new CancellationTokenSource();
        Console.CancelKeyPress += (sender, args) => CancellationTokenSource.Cancel();
        
        consoleRender.Run(mapConstructor, CancellationTokenSource.Token);
    }
}
