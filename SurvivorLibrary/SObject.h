#pragma once
#include "SMath.h"

namespace SurvivorLibrary
{
	struct API SObject
	{
		sint id;
		SPoint position;
	};

	enum class API SHeroState :int
	{
		Stay,
		Move,
		Run,
		Turn,
		Attack,
		Climb
	};

	struct API SHero :SObject
	{
		sint type;

		sint hp;
		sint energy;
		SVector2 direction;
		SHeroState state;

		stime prvTime;

		class SAIAdapter *ai;
		void *aiThread;
	};

	struct API SAnimal :SObject
	{

	};

	enum class API SStillObjectType :int
	{
		// Life dynnamic
		Hero, WildAnimal,
		Blood, Excrement, Body,
		// Life static
		Tree,
		Charcoal,
		// Barrier
		Rock, Hill,
		// Trap
		Water, Stream, Lake,
		Hole, Fire, Smoke,
		// Weather
		Fog, Rain, Thunder,
		//
		TypeCount
	};

	struct API SStillObject :SObject
	{
		SStillObjectType type;
	};
}
