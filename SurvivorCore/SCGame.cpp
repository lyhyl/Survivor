#include <chrono>
#include <future>
#include "SCGame.h"

using namespace std;
using namespace std::chrono;

wstring GetExePath()
{
	TCHAR buffer[MAX_PATH];
	GetModuleFileName(NULL, buffer, MAX_PATH);
	wstring path(buffer);
	wstring::size_type pos = path.find_last_of(__TEXT("\\/"));
	return path.substr(0, pos);
}

void HeroThinkThread(SCGame *game, SCHero *hero, double fairRatio)
{
	while (game->running)
	{
		AIThinkData data;
		data.target = hero;
		game->GetThinkData(&data);
		auto begin = steady_clock::now();
		SCHeroAction action = hero->ai->Think(&data);
		auto end = steady_clock::now();
		this_thread::sleep_for(milliseconds((long long)(fairRatio * (begin - end).count())));
		game->writeMtx.lock();
		game->actionLog[hero] = action;
		game->writeMtx.unlock();
	}
}

int Competitor::HeroID = 0;

Competitor::Competitor(SCGame *g, AIAdapter *a, double fr) :ai(a), game(g), fairRatio(fr){}

SCHero* Competitor::CreateHero()
{
	SCHero *hero = new SCHero{ HeroID++, 100, 10, { 2560, 2560 }, { 1, 1 }, SCHeroActionType::Stay, ai };
	hero->aiThread = new thread(HeroThinkThread, game, hero, fairRatio);
	return hero;
}

SCGame::SCGame()
{
	uiSelectorDll = LoadLibrary(__TEXT(".\\UIAdapterSelector.dll"));
	getUIAdapter = (GetUIAdapterFunc)GetProcAddress(uiSelectorDll, "CreateUIAdapter");
	aiSelectorDll = LoadLibrary(__TEXT(".\\AIAdapterSelector.dll"));
	getAIAdapter = (GetAIAdapterFunc)GetProcAddress(aiSelectorDll, "CreateAIAdapter");
}

SCGame::~SCGame()
{
	delete uiAdapter;
	FreeLibrary(uiSelectorDll);
	FreeLibrary(aiSelectorDll);
}

void SCGame::BeginGame(istream &in)
{
	throw exception("Not Impl");
}

void SCGame::BeginGame()
{
	BeginGame(wstring(__TEXT("C#2D")));
}

void SCGame::BeginGame(wstring &UIOption)
{
	uiOption = UIOption;

	uiAdapter = getUIAdapter(uiOption.c_str());
	wstring aiFolder = GetExePath() + __TEXT("\\") + GetHeroesDirW() + __TEXT("\\");

	if (CreateDirectory(aiFolder.c_str(), NULL) || ERROR_ALREADY_EXISTS == GetLastError())
	{
		WIN32_FIND_DATA fd;
		HANDLE hFind = FindFirstFile((aiFolder + __TEXT("*.*")).c_str(), &fd);
		if (hFind != INVALID_HANDLE_VALUE)
		{
			do
				if (!(fd.dwFileAttributes & FILE_ATTRIBUTE_DIRECTORY))
					Competitors.emplace_back(new Competitor(this, getAIAdapter((aiFolder + fd.cFileName).c_str()), 0));
			while (FindNextFile(hFind, &fd));
			FindClose(hFind);
		}
	}
	else
	{
		// Wait for AIs
	}

	for (auto com : Competitors)
		Heroes.Add(com->CreateHero());
}

void SCGame::Present()
{
	UIDisplayData data;
	data.map = &Map;
	data.heroes = (SCCollection*)&Heroes;
	uiState = uiAdapter->Display(&data);
}

void SCGame::GetThinkData(AIThinkData *data) const
{
	const SCHero *hero = data->target;
	data->vision = nullptr;
}

void SCGame::ApplyAction()
{
	for (auto p : actionLogApplying)
	{
		switch (p.second.type)
		{
		case SCHeroActionType::Move:
			p.first->position.y += 0.001;
			break;
		default:
			break;
		}
	}
}

bool SCGame::Run()
{
	writeMtx.lock();
	actionLog.swap(actionLogApplying);
	writeMtx.unlock();
	ApplyAction();
	actionLogApplying.clear();
	return uiState == 1;
}

void SCGame::EndGame()
{
	// stop all thread
	running = false;
	scsize HeroesSize = Heroes.Size();
	for (scsize i = 0; i < HeroesSize; i++)
	{
		SCHero *hero = Heroes[i];
		if (hero->aiThread)
		{
			thread* t = (thread*)hero->aiThread;
			t->join();
			delete t;
		}
	}
	// generate statistics

	// clear up
	Heroes.Clear();
	for (auto com : Competitors)
		delete com;
	Competitors.clear();
}

bool SCGame::SwitchUI(wstring &UIOption)
{
	if (UIOption == uiOption)
		return false;

	uiOption = UIOption;
	if (uiAdapter)
		delete uiAdapter;
	uiAdapter = getUIAdapter(uiOption.c_str());
	return true;
}

void SCGame::GetStatistics() const
{

}

void *SCGame::GetLog()
{
	return 0;
}
