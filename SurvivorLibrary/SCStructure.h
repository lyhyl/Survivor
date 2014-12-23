#pragma once
#include "SCUtilities.h"
#include "SCMap.h"

using namespace std;

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

struct API SCHero
{
	scint id;
	scint hp;
	scint energy;
	SCPoint position;
	SCVector2 direction;
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

struct API SCHeroAction
{
	SCHeroActionType type;
	union
	{
		// Stay Move Run Attack Climb
		struct { scfloat r0, r1, r2, r3; };
		// Turn
		struct { scfloat angle, r1, r2, r3; };
	};
};

const SCHeroAction API NoAction = { SCHeroActionType::Stay, 0 };

struct API UIDisplayData
{
	const SCMap *map;
	const SCCollection *competitors;
};

struct API AIThinkData
{
	const SCHero *target;
	const SCHeroVision *vision;
	const SCHeroHearing *hearing;
	const SCHeroTouch *touch;
};