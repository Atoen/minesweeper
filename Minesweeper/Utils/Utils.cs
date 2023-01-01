namespace Minesweeper.Utils;

public static class EnumExtensions
{
    public static bool HasValue<T>(this T value, T flag) where T : Enum
    {
        var f = Convert.ToInt32(flag);
        return (Convert.ToInt32(value) & f) == f;
    }
}

public sealed class ObservableList<T> : List<T>
{
    public event EventHandler<CollectionChangedEventArgs<T>>? Changed;

    public new void Add(T item)
    {
        base.Add(item);

        var args = new CollectionChangedEventArgs<T>(ChangeType.Add, item);
        Changed?.Invoke(this, args);
    }

    public new bool Remove(T item)
    {
        var result = base.Remove(item);

        if (result)
        {
            var args = new CollectionChangedEventArgs<T>(ChangeType.Remove, item);
            Changed?.Invoke(this, args);
        }

        return result;
    }

    public new void AddRange(IEnumerable<T> collection)
    {
        var items = collection.ToList();
        base.AddRange(items);

        foreach (var item in items)
        {
            var args = new CollectionChangedEventArgs<T>(ChangeType.Add, item);
            Changed?.Invoke(this, args);
        }
    }

    public new void Clear()
    {
        var items = this as List<T>;
        base.Clear();

        foreach (var item in items)
        {
            var args = new CollectionChangedEventArgs<T>(ChangeType.Remove, item);
            Changed?.Invoke(this, args);
        }
    }
}

public class CollectionChangedEventArgs<T> : EventArgs
{
    public ChangeType ChangeType { get; }
    public T Element { get; }

    public CollectionChangedEventArgs(ChangeType changeType, T element)
    {
        ChangeType = changeType;
        Element = element;
    }
}

public enum ChangeType
{
    Add,
    Remove
}