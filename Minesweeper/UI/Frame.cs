namespace Minesweeper.UI;

public class Frame
{
    private readonly List<Widget> _widgets = new();

    public void Add(Widget widget)
    {
        _widgets.Add(widget);
    }

    public void Add(params Widget[] widgets)
    {
        _widgets.AddRange(widgets);
    }

    public void Remove(Widget widget)
    {
        _widgets.Remove(widget);
    }

    public void Clear()
    {
        foreach (var widget in _widgets)
        {
            widget.Remove();
        }
        
        _widgets.Clear();
    }
}