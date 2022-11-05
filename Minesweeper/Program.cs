using Minesweeper;

var name =  ParentProcessUtilities.GetParentProcess(ParentProcessUtilities.GetParentProcess().Id).ProcessName;

Display.Init(1);

Input.Init();

MainMenu.Display();
