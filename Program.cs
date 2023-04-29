using System;
using System.Threading;

namespace LD53;

public class Program
{
    public static void Main(string[] args)
    {
        var consoleRender = new ConsoleRender();
        
        var mapConstructor = new MapConstructor("Maps/Start.map");
        mapConstructor.SetBound(0,0,0,0, Alignment.Strech, Alignment.Strech);

        var map = new MapViewer("Maps/Start.map");
        map.SetBound(0,0,0,0, Alignment.Strech, Alignment.Strech);
        
        // Add menu
        var menu = new Menu();
        menu.SetBound(0,0,0,0, Alignment.Fill, Alignment.Fill);
        menu.AddOption(map);
        menu.AddOption(mapConstructor);
        
        
        var CancellationTokenSource = new CancellationTokenSource();
        Console.CancelKeyPress += (sender, args) => CancellationTokenSource.Cancel();
        
        consoleRender.Run(menu, CancellationTokenSource.Token);
    }
}
