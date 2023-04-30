using System;
using System.Collections.Generic;

namespace LD53.AssetLogic;

public class MorseChest : TextBox
{
    public string ValidPin = "6712";
    public List<bool[]> pin = new List<bool[]>()
    {
        new[] { true, false, false, false, false },
        new[] { true, true, false, false, false },
        new[] { false, true, true, true, true },
        new[] { false, false, true, true, true },
    };

    public MorseChest() : base(null)
    {
        ShowCurser = false;
        SetBound(0,3,0,0, Alignment.Strech, Alignment.End);
    }
    
    DateTime nextCursorUpdate = DateTime.MinValue;
    private int morseIndex = 0;
    private bool cursorVisible = false;
    
    public override void Render()
    {
        base.Render();

        var index = Content.Length;
        if (index >= pin.Count)
            return;
        var morse = pin[index];
        var longMorse = morse[morseIndex];
        
        // Blink cursor
        if (nextCursorUpdate < DateTime.Now)
        {
            if (cursorVisible) // Move to next morse
            {
                morseIndex++;
                if (morseIndex == morse.Length)
                    morseIndex = 0;
                
                nextCursorUpdate = DateTime.Now.AddMilliseconds(morseIndex == 0 ? 2000 : 1000);
                cursorVisible = false;
            }
            else
            {
                nextCursorUpdate = DateTime.Now.AddMilliseconds(longMorse ? 800 : 300);
                cursorVisible = true;
            }
        }

        Move(Length + 1, 1);
        if (cursorVisible)
            Draw('|');
        else
            Draw(' ');
    }
}