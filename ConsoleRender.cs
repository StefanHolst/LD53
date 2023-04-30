using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace LD53;

public class MainView : View
{
    private ConsoleRender _consoleRender;
    public override ConsoleRender ConsoleRender => _consoleRender; 

    public MainView(ConsoleRender consoleRender)
    {
        _consoleRender = consoleRender;
        CalculateBound();
    }
    
    public override void Render()
    {
    }

    protected override void CalculateBound()
    {
        Bound = new Bounds()
        {
            Height = ConsoleRender.Height + 2,
            Width = ConsoleRender.Width + 2,
            X = -1,
            Y = -1,
            HorizontalAlignment = Alignment.Strech,
            VerticalAlignment = Alignment.Strech
        };
    }
}

public class ConsoleRender
{
    private static List<ConsoleRender> _renders = new List<ConsoleRender>();

    public MainView MainView { get; set; }
    public int Width { get; set; }
    public int Height { get; set; }

    private char[,] ViewBuffer;
    private char[,] ViewBackBuffer;
    private int X = 0, Y = 0;

    public void Run(View view, CancellationToken cancellationToken)
    {
        _renders.Add(this);
        Console.CursorVisible = false;
        
        Height = Console.WindowHeight;
        Width = Console.WindowWidth;
        ViewBuffer = new char[Width, Height];
        ViewBackBuffer = new char[Width, Height];

        MainView = new MainView(this);
        MainView.AddChildView(view);

        MainView.CalculateBounds();
        UpdateConsole();
        
        // Watch for windows size change
        _ = Task.Run(async () =>
        {
            while (cancellationToken.IsCancellationRequested == false)
            {
                if (Console.WindowHeight != Height ||
                    Console.WindowWidth != Width)
                {
                    Height = Console.WindowHeight;
                    Width = Console.WindowWidth;
                    ViewBuffer = new char[Width, Height];
                    ViewBackBuffer = new char[Width, Height];
                    MainView.CalculateBounds();
                }

                await Task.Delay(100, cancellationToken);
            }
        }, cancellationToken);

        // Wait for keypress
        var lockObj = new object();
        _ = Task.Run( () =>
        {
            while (cancellationToken.IsCancellationRequested == false)
            {
                var key = Console.ReadKey(true);
                lock (lockObj)
                {
                    if (MainView.PreviewKeyPressed(key) == false)
                        MainView.KeyPressed(key);
                }
            }
        }, cancellationToken);

        // Render loop
        while (cancellationToken.IsCancellationRequested == false)
        {
            lock (lockObj)
            {
                MainView.RenderAll();
                UpdateConsole();
            }

            Thread.Sleep(TimeSpan.FromSeconds(1.0 / 30.0));
        }

        cancellationToken.WaitHandle.WaitOne();

        _renders.Remove(this);
    }
    
    public void Move(int x, int y)
    {
        X = x;
        Y = y;
    }
    public void Draw(char c)
    {
        try
        {
            if (X >= 0 && X < ViewBuffer.GetLength(0) && Y >= 0 && Y < ViewBuffer.GetLength(1))
                ViewBuffer[X, Y] = c;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    private void UpdateConsole()
    {
        for (int x = 0; x < Width; x++)
        {
            for (int y = 0; y < Height; y++)
            {
                Console.SetCursorPosition(x, y);
                if (ViewBuffer[x, y] == ViewBackBuffer[x, y] && redraw == false)
                    continue;
                
                // Console.ForegroundColor = (ConsoleColor) new Random().NextInt64((int)ConsoleColor.DarkBlue, (int)ConsoleColor.White);
                if (ViewBuffer[x, y] == '\0')
                    Console.Out.Write(' ');
                else
                    Console.Out.Write(ViewBuffer[x, y]);
            }
        }
        
        redraw = false;
        ViewBackBuffer = ViewBuffer;
        ViewBuffer = new char[Width, Height];
    }

    
    bool redraw = false;
    public static void RedrawEverything()
    {
        foreach (var render in _renders)
            render.redraw = true;
    }
}