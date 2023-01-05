namespace Minesweeper.UI.Events;

public class RoutedEventArgs : EventArgs
{
    public RoutedEventArgs(Control source)
    {
        Source = source;
        OriginalSource = source;
    }

    public bool Handled { get; set; }

    public Control Source { get; set; }
    
    public Control OriginalSource { get; }
}
