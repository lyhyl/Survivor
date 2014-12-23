#pragma once
#include <Windows.h>

#include <atomic>
#include <condition_variable>
#include <thread>

#include <fstream>
#include <hash_map>
#include <vector>

#include <AIAdapter.h>
#include <UIAdapter.h>
#include <SCCollection.h>
#include <SCMap.h>

#include "ThreadPool.h"

using namespace std;

void HeroThinkThread(class SCGame *game, AIAdapter *ai, SCHero *hero);

static const int ThinkTimeLimit = 40;

class Competitor
{
public:
	class SCGame *game;
	AIAdapter *ai;
	vector<SCHero*> heroes;

	Competitor(class SCGame *g = nullptr, AIAdapter *a = nullptr)
	{
		game = g;
		ai = a;
	}

	~Competitor()
	{
		for (auto h : heroes)
			delete h;
	}

	void CreateHero()
	{
		SCHero *hero = new SCHero;
		heroes.push_back(hero);
	}
};

class SCGame
{
	friend void HeroThinkThread(SCGame *game, AIAdapter *ai, SCHero *hero);
public:
	SCGame();
	SCGame(istream &in);
	~SCGame();

	void BeginGame(const wchar_t * UIOption = nullptr);
	int Present() const;
	void Run();
	bool UIClosed() const;
	void EndGame();

	void GetStatistics() const;

	void *GetLog();
private:
	void GetThinkData(AIThinkData *data) const;
	UIDisplayData GetDisplayData() const;
	void ApplyAction();

	mutex writeMtx;
	volatile __int64 actionThinkedCount = 0;

	mutex doneMtx;
	condition_variable doneCV;

	HMODULE uiSelectorDll;
	HMODULE aiSelectorDll;
	UIAdapter *uiAdapter = nullptr;

	SCMap Map;
	SCCollectionX<Competitor*> Competitors;
	scint HeroCount = 0;
	hash_map<SCHero*, SCHeroAction> actionLog;

	ThreadPool *threadPool = nullptr;
};

