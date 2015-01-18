#pragma once
#include <Windows.h>

#include <condition_variable>
#include <thread>

#define _USE_MATH_DEFINES
#include <cmath>
#include <fstream>
#include <hash_map>
#include <vector>

#include <SAIAdapter.h>
#include <SUIAdapter.h>
#include <SMap.h>

using namespace std;
using namespace SurvivorLibrary;

class HeroProperty
{
public:
	static const sfloat MoveSpeed;
	static const sfloat RunSpeed;
	static const sfloat MinTurnAngle;
	static const sfloat MaxTurnAngle;
};

class Competitor
{
private:
	static int HeroID;
	class SCGame *game;
	SAIAdapter *ai;
	sfloat fairRatio;
public:
	Competitor(SCGame *g, SAIAdapter *a, sfloat fr);
	SHero* CreateHero();
};

class SCGame
{
	friend void HeroThinkThread(SCGame *game, SHero *hero, sfloat fairRatio);

	typedef SUIAdapter*(__cdecl *GetUIAdapterFunc)(const wchar_t*);
	typedef const wchar_t**(__cdecl *EnumUIAdapterFunc)(int *);
	typedef SAIAdapter*(__cdecl *GetAIAdapterFunc)(const wchar_t*);
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
	void GetThinkData(SAIThinkData *data) const;
	void ApplyAction();

	EnumUIAdapterFunc enumUIAdpaters;
	GetUIAdapterFunc getUIAdapter;
	GetAIAdapterFunc getAIAdapter;

	HMODULE uiSelectorDll;
	wstring uiOption;
	SUIAdapter *uiAdapter = nullptr;
	int uiState = 1;

	HMODULE aiSelectorDll;
	vector<Competitor*> Competitors;
	vector<SHero*> Heroes;

	mutex writeMtx;
	hash_map<SHero*, SHeroAction> actionLog;
	hash_map<SHero*, SHeroAction> actionLogApplying;

	SMap *Map = nullptr;

	bool running = true;
};
