using Minesweeper.Utils;

namespace Minesweeper.UI;

public class Panel : Control, IContainer
{
    public ObservableList<Control> Children { get; } = new();
    
}
