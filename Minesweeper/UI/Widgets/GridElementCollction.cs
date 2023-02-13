using Minesweeper.Utils;

namespace Minesweeper.UI.Widgets;

public sealed class GridElementCollection<T> : ObservableList<T> where T : class, IGridLayoutElement
{
    public int TotalPadding => Count > 0 ? Count - 1 : 0;
    public int Size => this.Sum(e => e.Size) + TotalPadding;

    public int GetOffset(int index)
    {
        if (index >= Count || index < 0) throw new IndexOutOfRangeException();

        var offset = 0;
        for (var i = 0; i < index; i++)
        {
            offset += this[i].Size + 1;
        }

        return offset;
    }

    public void SetEvenSizes(int totalSize)
    {
        var sizeToDivide = totalSize - TotalPadding;
        var size = sizeToDivide / Count;
        var remainder = sizeToDivide % Count;

        for (var i = 0; i < Count; i++)
        {
            var elementSize = size;
            if (remainder > 0)
            {
                elementSize++;
                remainder--;
            }

            this[i].Size = elementSize;
        }
    }
}

public sealed class Column : IGridLayoutElement
{
    public int Size { get; set; }
    public GridUnitType UnitType { get; set; } = GridUnitType.Pixel;
}

public sealed class Row : IGridLayoutElement
{
    public int Size { get; set; }
    public GridUnitType UnitType { get; set; } = GridUnitType.Pixel;
}

public interface IGridLayoutElement
{
    public int Size { get; set; }
    public GridUnitType UnitType { get; set; }
}

public enum GridUnitType
{
    Auto,
    Pixel,
    Weighted
}
