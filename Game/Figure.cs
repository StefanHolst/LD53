using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using LD53.AssetLogic;

namespace LD53;

public class Person : View
{
    private string figure;
    private string message = null;
    private bool bagLeft = false;

    private void LoadFigure(string type)
    {
        using var stream = Assembly.GetEntryAssembly()!.GetManifestResourceStream("LD53.Game.Figures." + type);
        using var reader = new StreamReader(stream!);
        this.figure = reader.ReadToEnd();
    }

    public Person()
    {
        Bound.Width = 7;
        Bound.Height = 3;
        LoadFigure("NORMAL");
    }
    
    public override bool KeyPressed(ConsoleKeyInfo key)
    {
        switch (key.Key)
        {
            case ConsoleKey.LeftArrow:
                Bound.X--;
                if (CheckCollision())
                    Bound.X++;
                // Move bag to right side
                if (bagLeft && GameState.Bag != null)
                    bagLeft = false;
                break;
            case ConsoleKey.RightArrow:
                Bound.X++;
                if (CheckCollision())
                    Bound.X--;
                // Move bag to left side
                if (bagLeft == false && GameState.Bag != null)
                    bagLeft = true;
                break;
            case ConsoleKey.UpArrow:
                Bound.Y--;
                if (CheckCollision())
                    Bound.Y++;
                break;
            case ConsoleKey.DownArrow:
                Bound.Y++;
                if (CheckCollision())
                    Bound.Y--;
                break;
            case ConsoleKey.Spacebar:
                var assets = getCollidedAssets(1);
                if (assets.Any())
                {
                    // Check for picking up a box
                    var box = assets.FirstOrDefault(a => a.Name != null && a.Name.StartsWith("Box") && a.Name.Length == 4);
                    if (box != null)
                        putInBag(box);
                    
                    // Check unlocking door
                    var door = assets.FirstOrDefault(a => a.Name != null && a.Name.EndsWith("Door"));
                    if (door != null)
                        UnlockDoor(door);
                    
                    // Check chest
                    var chest = assets.FirstOrDefault(a => a.Name != null && a.Name.EndsWith("Chest") && a.Name.Length > 5);
                    if (chest != null)
                        CheckChest(chest);
                    
                    // Check for key
                    var _key = assets.FirstOrDefault(a => a.Name != null && a.Name.EndsWith("Key"));
                    if (_key != null)
                        CheckKey(_key);

                    if (GameState.Bag != null)
                    {
                        assets = getCollidedAssets(4);
                        // Check for placing box in slot
                        var boxSlot = assets.FirstOrDefault(a => a.Name != null && a.Name.StartsWith("BoxSlot") && a.Name.Length == 8);
                        if (boxSlot != null)
                        {
                            CheckBox(boxSlot);
                            return false;
                        }
                    }
                }
                else if (GameState.Bag != null)
                {
                    var asset = GameState.Bag;
                    var bagXOffset = bagLeft ? - GameState.Bag.Width - 1 : Bound.Width;

                    foreach (var point in asset.Points)
                    {
                        point.X += Bound.X + bagXOffset;
                        point.Y += Bound.Y;
                    }
                    GameState.CurrentMap.Assets.Add(asset);
                    GameState.CurrentMap.UpdateLookup();
                    GameState.Bag = null;
                    
                    ShowMessage("Too heavy...");
                }
                else
                    ShowMessage("hello...");
                break;
        }

        return false;
    }

    private void CheckMaze(Map map, MapEntrance entrance)
    {
        if (map.Name == "Maze")
        {
            GameState.TakenDirections.Add(entrance.Name switch
            {
                "MazeTop" => Direction.Up,
                "MazeBottom" => Direction.Down,
                "MazeLeft" => Direction.Left,
                _ => Direction.Right,
            });

            for (int i = 0; i < GameState.TakenDirections.Count; i++)
            {
                if (GameState.TakenDirections[i] != Maze.ValidSequence[i])
                {
                    GameState.TakenDirections.Clear();
                    break;
                }
            }
            if (GameState.TakenDirections.Count == Maze.ValidSequence.Length)
            {
                // success
                var timeMap = Map.Load("Time");
                var timeEntrence = timeMap.Entrances.First(e => e.Destination);
                GameState.ChangeMap(timeEntrence);
                GameState.TakenDirections.Clear();
                var timeLocation = timeEntrence.Points.First();

                Bound.X = timeLocation.X;
                Bound.Y = timeLocation.Y;
            }
        }
    }
    private void CheckKey(MapAsset key)
    {
        GameState.Keys.Add(MapAsset.LoadFromAssets("Key"));
        GameState.CurrentMap.Assets.Remove(key);
        GameState.CurrentMap.UpdateLookup();
    }
    private void CheckChest(MapAsset chest)
    {
        if (chest.Name == "MorseChest")
        {
            var morseChest = new MorseChest();
            Dialog.ShowView("Pin Needed", "Please type the pin...", morseChest);
            if (morseChest.Content == morseChest.ValidPin)
            {
                // Load success chest
                var successChest = MapAsset.LoadFromAssets("Chest");
                foreach (var point in successChest.Points)
                {
                    point.X += chest.X;
                    point.Y += chest.Y;
                }
                GameState.CurrentMap.Assets.Add(successChest);
                GameState.CurrentMap.Assets.Remove(chest);
                GameState.CurrentMap.UpdateLookup();
                GameState.Keys.Add(MapAsset.LoadFromAssets("Key"));
                GameState.ChestOpened++;

                ShowMessage(".. - / .-- --- .-. -.- . -.. (It worked)");
            }
            else
                ShowMessage("Nothing happened... Must be the wrong pin\n\nThere's a PostIt on the chest...\nIt says: \"Who the hell uses morse?\"");
        }

        if (chest.Name == "JapanChest")
        {
            var textBox = new TextBox(null);
            textBox.SetBound(0,3,0,0, Alignment.Strech, Alignment.End);
            Dialog.ShowView("Pin Needed", "Please type the pin...", textBox);
            if (textBox.Content.ToLowerInvariant() == "japan")
            {
                // Load success chest
                var successChest = MapAsset.LoadFromAssets("Chest");
                foreach (var point in successChest.Points)
                {
                    point.X += chest.X;
                    point.Y += chest.Y;
                }
                GameState.CurrentMap.Assets.Add(successChest);
                GameState.CurrentMap.Assets.Remove(chest);
                GameState.CurrentMap.UpdateLookup();
                GameState.Keys.Add(MapAsset.LoadFromAssets("Key"));
                GameState.ChestOpened++;

                ShowMessage("Konichiwa!");
            }
            else
                ShowMessage("Hmm, does not seem to work...\nMust be the wrong pin\n\nWonder what language that is...");
        }

        if (chest.Name == "CountChest")
        {
            var textBox = new TextBox(null);
            textBox.SetBound(0,3,0,0, Alignment.Strech, Alignment.End);
            Dialog.ShowView("Pin Needed", "Please type the pin...", textBox);
            if (textBox.Content == "4665")
            {
                // Load success chest
                var successChest = MapAsset.LoadFromAssets("Chest");
                foreach (var point in successChest.Points)
                {
                    point.X += chest.X;
                    point.Y += chest.Y;
                }
                GameState.CurrentMap.Assets.Add(successChest);
                GameState.CurrentMap.Assets.Remove(chest);
                GameState.CurrentMap.UpdateLookup();
                GameState.Keys.Add(MapAsset.LoadFromAssets("Key"));
                GameState.ChestOpened++;

                ShowMessage("Ohh yeaah!");
            }
            else
                ShowMessage("Not the right code...\nMust be the wrong pin\n\nDid I count right?");
        }
        
        if (chest.Name == "LastChest")
        {
            var textBox = new TextBox(null);
            textBox.SetBound(0,3,0,0, Alignment.Strech, Alignment.End);
            Dialog.ShowView("Pin Needed", "Please type the pin...", textBox);
            if (textBox.Content == "635")
            {
                // Load success chest
                var successChest = MapAsset.LoadFromAssets("Chest");
                foreach (var point in successChest.Points)
                {
                    point.X += chest.X;
                    point.Y += chest.Y;
                }
                GameState.CurrentMap.Assets.Add(successChest);
                GameState.CurrentMap.Assets.Remove(chest);
                GameState.CurrentMap.UpdateLookup();
                GameState.Keys.Add(MapAsset.LoadFromAssets("Key"));
                
                // Win... nothing more..
                ShowMessage("!!! DONE !!!");
                var winEntrance = new MapEntrance()
                {
                    MapName = "Win"
                };
                GameState.ChangeMap(winEntrance);
            }
            else
                ShowMessage("Fail...\nMust be the wrong pin\n\nDid I see wrong?");
        }
        
        GameState.CheckStats();
    }
    private void UnlockDoor(MapAsset door)
    {
        // Check if we have a key
        var key = GameState.Keys.FirstOrDefault();
        if (key == null)
        {
            ShowMessage("I need a key to open this door...");
            return;
        }
        
        GameState.Keys.Remove(key);
        GameState.DoorsOpened++;
        GameState.CheckStats();
        GameState.CurrentMap.Assets.Remove(door);
        GameState.CurrentMap.UpdateLookup();
    }
    private void CheckBox(MapAsset boxSlot)
    {
        if (GameState.Bag == null)
            ShowMessage("I need a box to put in there...");
        else if (GameState.Bag.Name[3] != boxSlot.Name[7])
            ShowMessage("This box doesn't fit...");
        else
        {
            // Load success box
            var successBox = MapAsset.LoadFromAssets("Box");
            foreach (var point in successBox.Points)
            {
                point.X += boxSlot.X;
                point.Y += boxSlot.Y - 1;
            }
            GameState.CurrentMap.Assets.Add(successBox);
            GameState.CurrentMap.Assets.Remove(boxSlot);
            GameState.CurrentMap.UpdateLookup();
            GameState.Bag = null;
            GameState.Keys.Add(MapAsset.LoadFromAssets("Key"));
            GameState.PackagesDelivered++;
            GameState.CheckStats();
                            
            ShowMessage("I did it!");
        }
    }
    private void putInBag(MapAsset asset)
    {
        if (GameState.Bag != null)
            return;
        
        GameState.Bag = asset;
        GameState.CurrentMap.Assets.Remove(asset);
        GameState.CurrentMap.UpdateLookup();

        // Reset bag location
        var x = asset.X;
        var y = asset.Y;
        foreach (var point in GameState.Bag.Points)
        {
            point.X -= x;
            point.Y -= y;
        }

        ShowMessage("Lemme pick that up...");
    }
    private List<MapAsset> getCollidedAssets(int offset = 1)
    {
        // Check 
        var outBound = new Bounds()
        {
            X = Bound.X - offset,
            Y = Bound.Y - offset,
            Height = Bound.Height + offset * 2,
            Width = Bound.Width + offset * 2
        };
                
        return GameState.CurrentMap.Collides(outBound);
    }
    
    public override void Render()
    {
        // Draw figure
        int x = 0;
        int y = 0;
        for (int i = 0; i < figure.Length; i++)
        {
            if (figure[i] == '\r')
                continue;
            if (figure[i] == '\n')
            {
                x = 0;
                y++;
                continue;
            }
            Move(x, y);
            Draw(figure[i]);
            x++;
        }

        // Draw stuff in bag
        if (GameState.Bag != null)
        {
            foreach (var point in GameState.Bag.Points)
            {
                var bagXOffset = bagLeft ? - GameState.Bag.Width - 1 : Bound.Width;
                Move(point.X + bagXOffset, point.Y);
                Draw(point.C);
            }
        }

        for (int i = 0; i < GameState.Keys.Count; i++)
        {
            var key = GameState.Keys[i];
            foreach (var point in key.Points)
            {
                Move(point.X + i * 2, point.Y - 2);
                Draw(point.C);
            }
        }
        
        // Draw Bubble
        if (message == null)
            return;
        
        var lines = message.Split("\n");
        var width = lines.Max(l => l.Length);

        bool msgTop = Bound.Y > 4 + lines.Length;
        bool msgLeft = Bound.X + width + 5 < Parent.Bound.Width;
        int xOffset = msgLeft ? 0 : -width + 3;
        int yOffset = msgTop ? -4 - lines.Length : 4;
        
        Move(0 + xOffset, 0 + yOffset);
        Draw('╭');
        Move(width + 3 + xOffset, 0 + yOffset);
        Draw('╮');
        Move(0 + xOffset, 2 + yOffset + lines.Length);
        Draw('╰');
        Move(width + 3 + xOffset, 2 + yOffset + lines.Length);
        Draw('╯');
        for (int i = 0; i <= lines.Length; i++)
        {
            Move(0 + xOffset, 1 + yOffset + i);
            Draw('│');
            Move(width + 3 + xOffset, 1 + yOffset + i);
            Draw('│');
        }

        DrawString(1 + xOffset, (msgTop ? 0 : 2 + lines.Length) + yOffset, new string('◠', width + 2));
        DrawString((msgLeft ? 1 : width + 1) + xOffset, (msgTop ? 2 + lines.Length : 0) + yOffset, new string('◠', 2));
        DrawString((msgLeft ? 5 : 1) + xOffset, (msgTop ? 2 + lines.Length : 0) + yOffset, new string('◠', width - 2));
        
        Move((msgLeft ? 3 : width - 1) + xOffset, (msgTop ? 2 + lines.Length : 0) + yOffset);
        Draw(msgTop ? '\\' : '/');
        Move((msgLeft ? 4 : width) + xOffset, (msgTop ? 2 + lines.Length : 0) + yOffset);
        Draw(msgTop ? '/' : '\\');

        for (int i = 0; i < lines.Length; i++)
        {
            DrawString(2 + xOffset, 1 + yOffset + i, lines[i]);
        }
    }

    private DateTime messageTimeout = DateTime.MinValue;
    public void ShowMessage(string _message)
    {
        message = _message;
        messageTimeout = DateTime.Now.AddSeconds(5);
        Task.Run(async () =>
        {
            while (messageTimeout > DateTime.Now)
                await Task.Delay(messageTimeout- DateTime.Now);
            message = null;
        });
    }

    private bool CheckCollision()
    {
        var map = GameState.CurrentMap;
        var collisions = map.Collides(Bound);
        if (collisions.Any())
        {
            var collision = collisions.FirstOrDefault();
            if (collision is MapEntrance entrance && entrance.Destination == false)
            {
                GameState.ChangeMap(entrance);
                
                // Set figure location
                var newEntrance = GameState.CurrentMap.Entrances.First(e => e.Name == entrance.Name);
                var location = newEntrance.Points.First();

                Bound.X = location.X;
                Bound.Y = location.Y;
                
                // Check maze
                CheckMaze(map, newEntrance);
            }

            if (collisions.Any(c => c.Type == MapAssetType.Asset || c.Type == MapAssetType.Wall))
                return true;
        }

        return false;
    }
}