#include <Windows.h>
#include "SCGame.h"

int CALLBACK WinMain(
	_In_  HINSTANCE hInstance,
	_In_  HINSTANCE hPrevInstance,
	_In_  LPSTR lpCmdLine,
	_In_  int nCmdShow
	)
{
	SCGame game;
	
	game.BeginGame();
	while (game.Run())
		game.Present();
	game.EndGame();

	return 0;
}
