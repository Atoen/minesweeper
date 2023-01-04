using Minesweeper.Utils;

namespace Minesweeper.UI;

public interface IContainer
{
    public ObservableList<Control> Children { get; }
}
