#pragma once
#include "SMath.h"

namespace SurvivorLibrary
{
	class API SObject
	{
		static ssize UniversalID;
		ssize id;
	public:
		SObject();
		static void ResetIDCounter();
		ssize ID();

		SVector2 position;
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

	class API SHero :public SObject
	{
	public:
		sint type;

		sint hp;
		sint energy;
		SVector2 direction;
		SHeroState state;

		stime prvTime;

		class SAIAdapter *ai;
		void *aiThread;
	};

	class API SAnimal :public SObject
	{
	public:
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

	class API SStillObject :public SObject
	{
	public:
		SStillObjectType type;
	};
}
