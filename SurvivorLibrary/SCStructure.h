#pragma once
#include <chrono>
#include "SCUtilities.h"
#include "SCMap.h"

using namespace std;
using namespace std::chrono;

struct API SCHeroVision
{
	SCCollection *resources;
};

struct API SCHeroHearing
{

};

struct API SCHeroTouch
{

};

enum class API SCHeroActionType :int
{
	Stay,
	Move,
	Run,
	Turn,
	Attack,
	Climb
};

struct API SCHero
{
	scint id;
	scint hp;
	scint energy;
	SCPoint position;
	SCVector2 direction;
	SCHeroActionType state;

	class AIAdapter *ai;
	void *aiThread;
};

struct API SCHeroAction
{
	SCHeroActionType type;
	union
	{
		// Stay Attack Climb
		struct { scfloat r0, r1, r2, r3; };
		// Turn Move Run
		struct { scfloat angle, r1, r2, r3; };
	};
};

const SCHeroAction API NoAction = { SCHeroActionType::Stay, 0 };

struct API UIDisplayData
{
	 const SCMap * map = nullptr;
	 const SCCollection * heroes = nullptr;
};

struct API AIThinkData
{
	const SCHero *target;
	const SCHeroVision *vision;
	const SCHeroHearing *hearing;
	const SCHeroTouch *touch;
};