using System;
using System.Collections.Generic;

namespace LD53;

public enum Alignment
{
    Start,
    End,
    Center,
    Strech,
    Fill
}

public class Bounds
{
    public int Width { get; set; }
    public int Height { get; set; }
    public int X { get; set; }
    public int Y { get; set; }
    public Alignment HorizontalAlignment { get; set; }
    public Alignment VerticalAlignment { get; set; }
}

public abstract class View
{
    public virtual ConsoleRender ConsoleRender => Parent.ConsoleRender;

    public abstract void Render();
    protected List<View> ChildViews { get; set; } = new List<View>();
    public Bounds Bound { get; set; } = new Bounds();
    public View Parent { get; set; }

    public void SetBound(int width, int height, int x, int y, Alignment horizontalAlignment, Alignment verticalAlignment)
    {
        Bound.Height = height;
        Bound.Width = width;
        Bound.X = x;
        Bound.Y = y;
        Bound.HorizontalAlignment = horizontalAlignment;
        Bound.VerticalAlignment = verticalAlignment;
    }

    public virtual bool PreviewKeyPressed(ConsoleKeyInfo key)
    {
        for (int i = 0; i < ChildViews.Count; i++)
        {
            var childView = ChildViews[i];
            if (childView.PreviewKeyPressed(key))
                return true;
        }

        return false;
    }
    
    public virtual bool KeyPressed(ConsoleKeyInfo key)
    {
        for (int i = 0; i < ChildViews.Count; i++)
        {
            var childView = ChildViews[i];
            if (childView.KeyPressed(key))
                return true;
        }

        return false;
    }
    public virtual void RenderAll()
    {
        Render();
        for (int i = 0; i < ChildViews.Count; i++)
        {
            var childView = ChildViews[i];
            childView.RenderAll();
        }
    }
    
    public virtual void CalculateBounds()
    {
        CalculateBound();
        for (int i = 0; i < ChildViews.Count; i++)
        {
            var childView = ChildViews[i];
            childView.CalculateBounds();
        }
    }

    protected virtual void CalculateBound()
    {
        Bound.Width = Bound.HorizontalAlignment switch
        {
            Alignment.Strech => Parent.Bound.Width - 2,
            Alignment.Fill => Parent.Bound.Width,
            _ => Bound.Width
        };
        Bound.X = Parent.Bound.X + Bound.HorizontalAlignment switch
        {
            Alignment.Strech => 1,
            Alignment.Fill => 0,
            Alignment.Center => (Parent.Bound.Width - Bound.Width) / 2,
            Alignment.End => Parent.Bound.Width - Bound.Width,
            _ => Bound.X + 1
        };
        Bound.Height = Bound.VerticalAlignment switch
        {
            Alignment.Strech => Parent.Bound.Height - 2,
            Alignment.Fill => Parent.Bound.Height,
            _ => Bound.Height
        };
        Bound.Y = Parent.Bound.Y + Bound.VerticalAlignment switch
        {
            Alignment.Strech => 1,
            Alignment.Fill => 0,
            Alignment.Center => (Parent.Bound.Height - Bound.Height) / 2,
            Alignment.End => Parent.Bound.Height - Bound.Height - 1,
            _ => Bound.Y + 1
        };
    }

    public void AddChildView(View view)
    {
        view.Parent = this;
        ChildViews.Add(view);
    }

    public void Move(int x, int y)
    {
        ConsoleRender.Move(Bound.X + x, Bound.Y + y);
    }
    public void Draw(char c)
    {
        ConsoleRender.Draw(c);
    }
    public void DrawString(int x, int y, string s)
    {
        for (int i = 0; i < s.Length; i++)
        {
            ConsoleRender.Move(Bound.X + x + i, Bound.Y + y);
            ConsoleRender.Draw(s[i]);
        }
    }
}