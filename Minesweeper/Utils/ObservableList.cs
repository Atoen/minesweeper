namespace Minesweeper.Utils;

public class ObservableList<T> : List<T>
{
    public event EventHandler<CollectionChangedEventArgs<T>>? ElementChanged;
    public event EventHandler? CollectionChanged; 

    public new void Add(T item)
    {
        base.Add(item);

        var args = new CollectionChangedEventArgs<T>(ChangeType.Add, item);
        ElementChanged?.Invoke(this, args);
        CollectionChanged?.Invoke(this, EventArgs.Empty);
    }

    public new bool Remove(T item)
    {
        var result = base.Remove(item);

        if (result)
        {
            var args = new CollectionChangedEventArgs<T>(ChangeType.Remove, item);
            ElementChanged?.Invoke(this, args);
            CollectionChanged?.Invoke(this, EventArgs.Empty);
        }

        return result;
    }

    public new void AddRange(IEnumerable<T> collection)
    {
        var newItems = collection.ToList();
        base.AddRange(newItems);

        if (newItems.Count > 0)
        {
            CollectionChanged?.Invoke(this, EventArgs.Empty);
        }

        foreach (var item in newItems)
        {
            var args = new CollectionChangedEventArgs<T>(ChangeType.Add, item);
            ElementChanged?.Invoke(this, args);
        }
    }

    public new void Clear()
    {
        var items = this as List<T>;

        if (Count > 0)
        {
            CollectionChanged?.Invoke(this, EventArgs.Empty);
        }
        
        base.Clear();

        foreach (var item in items)
        {
            var args = new CollectionChangedEventArgs<T>(ChangeType.Remove, item);
            ElementChanged?.Invoke(this, args);
        }
    }

    protected void OnCollectionChanged()
    {
        CollectionChanged?.Invoke(this, EventArgs.Empty);
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