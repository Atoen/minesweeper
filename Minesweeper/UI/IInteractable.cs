namespace Minesweeper.UI;

internal interface IInteractable
{
    void Update();
    void CursorMove(MouseState state);
    void Click(MouseState state);
}