#include <chrono>
#include <future>
#include "SCGame.h"

using namespace std;
using namespace std::chrono;

const sfloat HeroProperty::MoveSpeed = .5;
const sfloat HeroProperty::RunSpeed = .9;
const sfloat HeroProperty::MinTurnAngle = -.1;
const sfloat HeroProperty::MaxTurnAngle = .1;

wstring GetExePath()
{
	TCHAR buffer[MAX_PATH];
	GetModuleFileName(NULL, buffer, MAX_PATH);
	wstring path(buffer);
	wstring::size_type pos = path.find_last_of(L"\\/");
	return path.substr(0, pos);
}

void HeroThinkThread(SCGame *game, SHero *hero, sfloat fairRatio)
{
	while (game->running)
	{
		SAIThinkData data;
		data.target = hero;
		game->GetThinkData(&data);
		auto begin = steady_clock::now();
		SHeroAction action = hero->ai->Think(&data);
		auto end = steady_clock::now();
		long long d = (long long)(fairRatio * (begin - end).count());
		this_thread::sleep_for(milliseconds(max(10, d)));
		game->writeMtx.lock();
		game->actionLog[hero] = action;
		game->writeMtx.unlock();
	}
}

Competitor::Competitor(SCGame *g, SAIAdapter *a, sfloat fr) :ai(a), game(g), fairRatio(fr) { }

SHero* Competitor::CreateHero()
{
	SHero *hero = new SHero;
	hero->position = { 0, 0 };
	hero->type = 0;
	hero->hp = 100;
	hero->energy = 10;
	hero->direction = { 1, 0 };
	hero->state = SHeroState::Stay;
	hero->prvTime = system_clock::to_time_t(steady_clock::now());
	hero->ai = ai;
	hero->aiThread = new thread(HeroThinkThread, game, hero, fairRatio);
	return hero;
}

SCGame::SCGame()
{
	uiSelectorDll = LoadLibrary(L".\\UIAdapterSelector.dll");
	enumUIAdpaters = (EnumUIAdapterFunc)GetProcAddress(uiSelectorDll, "EnumUIAdapters");
	getUIAdapter = (GetUIAdapterFunc)GetProcAddress(uiSelectorDll, "CreateUIAdapter");
	aiSelectorDll = LoadLibrary(L".\\AIAdapterSelector.dll");
	getAIAdapter = (GetAIAdapterFunc)GetProcAddress(aiSelectorDll, "CreateAIAdapter");
}

SCGame::~SCGame()
{
	delete map;
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
	// TODO
	int count;
	auto options = enumUIAdpaters(&count);
	BeginGame(wstring(options[count - 1]));
	delete options;
}

void SCGame::BeginGame(wstring &UIOption)
{
	uiOption = UIOption;
	uiAdapter = getUIAdapter(uiOption.c_str());

	wstring aiFolder = GetExePath() + L"\\" + GetHeroesDirW() + L"\\";
	if (CreateDirectory(aiFolder.c_str(), NULL) || ERROR_ALREADY_EXISTS == GetLastError())
	{
		WIN32_FIND_DATA fd;
		HANDLE hFind = FindFirstFile((aiFolder + L"*.*").c_str(), &fd);
		if (hFind != INVALID_HANDLE_VALUE)
		{
			do
				if (!(fd.dwFileAttributes & FILE_ATTRIBUTE_DIRECTORY))
				{
					auto fullName = aiFolder + fd.cFileName;
					Competitors.emplace_back(new Competitor(this, getAIAdapter(fullName.c_str()), 0));
				}
			while (FindNextFile(hFind, &fd));
			FindClose(hFind);
		}
	}
	else
	{
		// Wait for AIs
	}

	SObject::ResetIDCounter();

	map = new SMap();

	for (auto com : Competitors)
		Heroes.emplace_back(com->CreateHero());

	SInitializeData data{ map, Heroes };
	if (uiAdapter->Initialize(&data) != (int)SGState::OK)
	{
		// TODO
		MessageBox(0, L"UI Adapter Initialize Fault", L"Error", 0);
	}
}

void SCGame::Present()
{
	/*Skip Empty Data*/
	if (UpdatedHeroes.size() || UpdatedStillObjects.size() || UpdatedAnimals.size())
	{
		SUpdateData data{ UpdatedHeroes, UpdatedStillObjects, UpdatedAnimals };
		uiState = uiAdapter->Update(&data);
	}
}

void SCGame::GetThinkData(SAIThinkData *data) const
{
	const SHero *hero = data->target;
	data->vision = nullptr;
	data->hearing = nullptr;
	data->touch = nullptr;
}

inline void TurnDirection(SVector2 &v, sfloat a)
{
	SRotate(v, max(min(a, HeroProperty::MaxTurnAngle), HeroProperty::MinTurnAngle));
	SNormalize(v);
}

inline void MoveDirection(SVector2 &v, SVector2 &d, sfloat speed)
{
	v += d * speed;
}

void SCGame::ApplyAction()
{
	UpdatedHeroes.clear();
	UpdatedStillObjects.clear();
	UpdatedAnimals.clear();

	for (auto p : actionLogApplying)
	{
		__time64_t now = system_clock::to_time_t(steady_clock::now());
		__time64_t d = now - p.first->prvTime;
		p.first->prvTime = now;

		switch (p.second.type)
		{
		case SHeroState::Move:
			TurnDirection(p.first->direction, p.second.angle);
			MoveDirection(p.first->position, p.first->direction, HeroProperty::MoveSpeed);
			break;
		case SHeroState::Run:
			TurnDirection(p.first->direction, p.second.angle);
			MoveDirection(p.first->position, p.first->direction, HeroProperty::RunSpeed);
			break;
		case SHeroState::Turn:
			TurnDirection(p.first->direction, p.second.angle);
			break;
		case SHeroState::Attack:
			break;
		case SHeroState::Climb:
			break;
		default:break;
		}

		UpdatedHeroes.push_back(p.first);
	}
}

bool SCGame::Run()
{
	writeMtx.lock();
	actionLog.swap(actionLogApplying);
	writeMtx.unlock();
	ApplyAction();
	actionLogApplying.clear();
	if (uiState == (int)SGState::UITooManyUpdateData)
		MessageBox(0, L"Too Many UDD", L"", 0);
	return uiState == (int)SGState::OK;
}

void SCGame::EndGame()
{
	// stop all thread
	running = false;
	size_t HeroesSize = Heroes.size();
	for (size_t i = 0; i < HeroesSize; i++)
	{
		SHero *hero = Heroes[i];
		if (hero->aiThread)
		{
			thread* t = (thread*)hero->aiThread;
			t->join();
			delete t;
		}
	}

	// generate statistics
	// TODO

	// clear up
	Heroes.clear();
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
