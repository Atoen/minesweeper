using System.Collections.ObjectModel;
using Minesweeper.Utils;

namespace Minesweeper.UI;

public interface IContainer
{
    public ObservableList<VisualComponent> Children { get; }
}