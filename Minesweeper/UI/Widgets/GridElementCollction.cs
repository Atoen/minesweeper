using Minesweeper.Utils;

namespace Minesweeper.UI.Widgets;

public sealed class GridElementCollection<T> : ObservableList<T> where T : class, IGridLayoutElement
{
    private int _padding = 1;

    public int Padding
    {
        get => _padding;
        set
        {
            _padding = value;
            OnCollectionChanged();
        }
    }

    public int PaddingCount => Count > 0 ? Count - 1 : 0;
    public int TotalPadding => PaddingCount * Padding;
    public int Size => this.Sum(e => e.Size) + TotalPadding;

    public int GetOffset(int index)
    {
        if (index >= Count || index < 0) throw new IndexOutOfRangeException();

        var offset = 0;
        for (var i = 0; i < index; i++)
        {
            offset += this[i].Size + Padding;
        }

        return offset;
    }
}

public sealed class Column : IGridLayoutElement
{
    public int Size { get; set; }
}

public sealed class Row : IGridLayoutElement
{
    public int Size { get; set; }
}

public interface IGridLayoutElement
{
    public int Size { get; set; }
}

public enum GridUnitType
{
    Auto,
    Pixel,
    Weighted
}