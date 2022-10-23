namespace Minesweeper.UI;

internal interface IInteractable
{
    void CursorMove(MouseState state);
    void Click(MouseState state);
}