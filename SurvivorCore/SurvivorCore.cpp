#include <Windows.h>
#include <string>
#include "SCGame.h"
#include "AIAdapter.h"
#include "UIAdapter.h"

using namespace std;

typedef UIAdapter*(__cdecl *GetUIAdapterFunc)(const wchar_t*);
typedef AIAdapter*(__cdecl *GetAIAdapterFunc)(const wchar_t*);

SCGame *game;

wstring GetExePath()
{
	TCHAR buffer[MAX_PATH];
	GetModuleFileName(NULL, buffer, MAX_PATH);
	wstring path(buffer);
	wstring::size_type pos = path.find_last_of(__TEXT("\\/"));
	return path.substr(0, pos);
}

//int main(int argc, char *argv[])
int CALLBACK WinMain(
	_In_  HINSTANCE hInstance,
	_In_  HINSTANCE hPrevInstance,
	_In_  LPSTR lpCmdLine,
	_In_  int nCmdShow
	)
{
	HMODULE uiSelectorDll = LoadLibrary(__TEXT(".\\UIAdapterSelector.dll"));
	GetUIAdapterFunc getUIAdapter = (GetUIAdapterFunc)GetProcAddress(uiSelectorDll, "CreateUIAdapter");
	UIAdapter *uiAdapter = getUIAdapter(__TEXT("C#2D"));

	HMODULE aiSelectorDll = LoadLibrary(__TEXT(".\\AIAdapterSelector.dll"));
	GetAIAdapterFunc getAIAdapter = (GetAIAdapterFunc)GetProcAddress(aiSelectorDll, "CreateAIAdapter");

	wstring currentWPath = GetExePath();
	wstring aiFolder = currentWPath + __TEXT("\\Heroes\\");

	// Max 256
	AIAdapter *AIs[256] = { 0 };
	unsigned AICount = 0;
	if (CreateDirectory(aiFolder.c_str(), NULL) || ERROR_ALREADY_EXISTS == GetLastError())
	{
		WIN32_FIND_DATA fd;
		HANDLE hFind = FindFirstFile((aiFolder + __TEXT("*.*")).c_str(), &fd);
		if (hFind != INVALID_HANDLE_VALUE)
		{
			do
				if (!(fd.dwFileAttributes & FILE_ATTRIBUTE_DIRECTORY))
					AIs[AICount++] = getAIAdapter((aiFolder + fd.cFileName).c_str());
			while (FindNextFile(hFind, &fd));
			FindClose(hFind);
		}
	}
	else
	{
		// Wait for AIs
	}

	game = new SCGame(AIs);
	
	int uistate;
	while ((uistate = uiAdapter->Display(game->GetMap(), game->GetCompetitors())) == 1)
	{
		//do logical thing...
		game->Run();
	}

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

	delete uiAdapter;
	FreeLibrary(uiSelectorDll);
	FreeLibrary(aiSelectorDll);
	return 0;
}
