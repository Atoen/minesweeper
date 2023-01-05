using Minesweeper.Utils;

namespace Minesweeper.UI.Widgets;

public class Panel : Control, IContainer
{
    public ObservableList<Control> Children { get; } = new();
    
}
