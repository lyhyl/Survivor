#include <chrono>
#include <future>
#include "SCGame.h"

using namespace std;
using namespace std::chrono;

typedef UIAdapter*(__cdecl *GetUIAdapterFunc)(const wchar_t*);
typedef AIAdapter*(__cdecl *GetAIAdapterFunc)(const wchar_t*);

wstring GetExePath()
{
	TCHAR buffer[MAX_PATH];
	GetModuleFileName(NULL, buffer, MAX_PATH);
	wstring path(buffer);
	wstring::size_type pos = path.find_last_of(__TEXT("\\/"));
	return path.substr(0, pos);
}

void HeroThinkThread(SCGame *game, AIAdapter *ai, SCHero *hero)
{
	// Think
	AIThinkData data = { hero, 0 };
	game->GetThinkData(&data);
	future<SCHeroAction> result = async([=]{ return ai->Think(&data); });
	SCHeroAction action = result.wait_for(milliseconds(ThinkTimeLimit)) == future_status::ready ? result.get() : NoAction;

	// Write
	game->writeMtx.lock();
	game->actionLog[hero] = action;
	game->actionThinkedCount++;
	game->writeMtx.unlock();

	game->doneCV.notify_one();
}

SCGame::SCGame()
{
	threadPool = new ThreadPool(256);
}

SCGame::SCGame(istream &in) :Map(in)
{
	throw exception("Not Impl");
}

SCGame::~SCGame()
{
	delete threadPool;

	scsize count = Competitors.Size();
	while (count--)
		delete Competitors[count];
	delete uiAdapter;
	FreeLibrary(uiSelectorDll);
	FreeLibrary(aiSelectorDll);
}

void SCGame::BeginGame(const wchar_t *UIOption)
{
	wstring currentWPath = GetExePath();

	if (UIOption == nullptr)
		UIOption = __TEXT("C#2D");

	uiSelectorDll = LoadLibrary(__TEXT(".\\UIAdapterSelector.dll"));
	GetUIAdapterFunc getUIAdapter = (GetUIAdapterFunc)GetProcAddress(uiSelectorDll, "CreateUIAdapter");
	uiAdapter = getUIAdapter(UIOption);

	aiSelectorDll = LoadLibrary(__TEXT(".\\AIAdapterSelector.dll"));
	GetAIAdapterFunc getAIAdapter = (GetAIAdapterFunc)GetProcAddress(aiSelectorDll, "CreateAIAdapter");
	wstring aiFolder = currentWPath + __TEXT("\\") + GetHeroesDirW() + __TEXT("\\");

	if (CreateDirectory(aiFolder.c_str(), NULL) || ERROR_ALREADY_EXISTS == GetLastError())
	{
		WIN32_FIND_DATA fd;
		HANDLE hFind = FindFirstFile((aiFolder + __TEXT("*.*")).c_str(), &fd);
		if (hFind != INVALID_HANDLE_VALUE)
		{
			do
				if (!(fd.dwFileAttributes & FILE_ATTRIBUTE_DIRECTORY))
					Competitors.Add(new Competitor(this, getAIAdapter((aiFolder + fd.cFileName).c_str())));
			while (FindNextFile(hFind, &fd));
			FindClose(hFind);
		}
	}
	else
	{
		// Wait for AIs
	}

	scsize comc = Competitors.Size();
	for (scsize i = 0; i < comc; i++)
	{
		Competitors[i]->CreateHero();
		HeroCount++;
	}
}

int SCGame::Present() const
{
	UIDisplayData data = GetDisplayData();
	return uiAdapter->Display(&data);
}

void SCGame::GetThinkData(AIThinkData *data) const
{
	data->vision = 0;
}

UIDisplayData SCGame::GetDisplayData() const
{
	UIDisplayData data;
	data.map = &Map;
	data.competitors = (SCCollection*)&Competitors;
	return data;
}

void SCGame::ApplyAction()
{
	for (auto p : actionLog)
	{
		// TODO
	}
	actionLog.clear();
}

void SCGame::Run()
{
	actionThinkedCount = 0;
	scsize comc = Competitors.Size();
	for (scsize i = 0; i < comc; i++)
	{
		Competitor *com = Competitors[i];
		for (auto her : com->heroes)
			threadPool->enqueue(HeroThinkThread, this, com->ai, her);
	}

	while (actionThinkedCount < HeroCount)
		this_thread::sleep_for(milliseconds(1));

	// 
	ApplyAction();
	actionLog.clear();
}

bool SCGame::UIClosed() const
{
	return false;
}

void SCGame::EndGame()
{
}

void SCGame::GetStatistics() const
{

}

void *SCGame::GetLog()
{
	return 0;
}
