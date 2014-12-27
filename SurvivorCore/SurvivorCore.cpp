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
	
	game->BeginGame();
	while (game->Run())
		game->Present();
	game->EndGame();

	delete game;

	return 0;
}
