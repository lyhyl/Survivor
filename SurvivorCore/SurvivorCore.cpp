#include <Windows.h>
#include <string>
#include "SCGame.h"

using namespace std;

SCGame *game;

int CALLBACK WinMain(
	_In_  HINSTANCE hInstance,
	_In_  HINSTANCE hPrevInstance,
	_In_  LPSTR lpCmdLine,
	_In_  int nCmdShow
	)
{
	game = new SCGame();
	
	int uistate;
	game->BeginGame();
	while ((uistate = game->Present()) == 1)
	{
		if (game->UIClosed())
			break;
		game->Run();
	}
	game->EndGame();

	// UI exited
	switch (uistate)
	{
	case 0:
		// exit normally
		break;
	default:
		//something happen...
		break;
	}

	delete game;

	return 0;
}
