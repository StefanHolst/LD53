using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace LD53;

public class Menu : View
{
    public int SelectedIndex { get; set; }

    public void AddOption(View view)
    {
        AddChildView(view);
    }

    public override void RenderAll()
    {
        Render();
        ChildViews[SelectedIndex].RenderAll();
    }

    public override void Render()
    {
        // ChildViews[SelectedIndex].Render();
    }

    public override bool KeyPressed(ConsoleKeyInfo key)
    {
        if (key.Key == ConsoleKey.F1 && SelectedIndex > 0)
        {
            SelectedIndex--;
            return true;
        }
        if (key.Key == ConsoleKey.F2 && SelectedIndex < ChildViews.Count - 1)
        {
            SelectedIndex++;
            return true;
        }

        return ChildViews[SelectedIndex].KeyPressed(key);
    }
}