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

using namespace std;

class Competitor
{
private:
	static int HeroID;
	class SCGame *game;
	AIAdapter *ai;
	double fairRatio;
public:
	Competitor(SCGame *g, AIAdapter *a, double fr);
	SCHero* CreateHero();
};

class SCGame
{
	friend void HeroThinkThread(SCGame *game, SCHero *hero, double penaltyRatio);

	typedef UIAdapter*(__cdecl *GetUIAdapterFunc)(const wchar_t*);
	typedef AIAdapter*(__cdecl *GetAIAdapterFunc)(const wchar_t*);
public:
	SCGame();
	~SCGame();

	void BeginGame(istream &in);
	void BeginGame();
	void BeginGame(wstring &UIOption);
	bool Run();
	void Present();
	void EndGame();

	bool SwitchUI(wstring &UIOption);

	void GetStatistics() const;

	void *GetLog();
private:
	void GetThinkData(AIThinkData *data) const;
	void ApplyAction();

	GetUIAdapterFunc getUIAdapter;
	GetAIAdapterFunc getAIAdapter;

	HMODULE uiSelectorDll;
	wstring uiOption;
	UIAdapter *uiAdapter = nullptr;
	int uiState = 1;

	HMODULE aiSelectorDll;
	vector<Competitor*> Competitors;
	SCCollectionX<SCHero*> Heroes;

	mutex writeMtx;
	hash_map<SCHero*, SCHeroAction> actionLog;
	hash_map<SCHero*, SCHeroAction> actionLogApplying;

	SCMap Map;

	bool running = true;
};
