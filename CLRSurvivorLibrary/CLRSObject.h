#pragma once

#include <cliext\vector>

#include <SDefines.h>
#include <SObject.h>

#include "CLRSurvivorLibrary.h"
#include "CLRSMath.h"

using namespace cliext;
using namespace System;
using namespace System::Collections::Generic;
using namespace SurvivorLibrary;

namespace CLRSurvivorLibrary
{
	public ref class CSObject
	{
	public:
		property ssize ID { virtual ssize get() { return id; } }
		property CSVector2^ Position { virtual CSVector2^ get() { return position; } }
	protected:
		CSObject(SObject *p)
		{
			id = p->ID();
			position = gcnew CSVector2(p->position);
		}
	private:
		ssize id;
		CSVector2 ^position;
	};

	public enum class CSHeroState :int
	{
		Stay,
		Move,
		Run,
		Turn,
		Attack,
		Climb
	};

	public ref class CSHero :CSObject
	{
	public:
		CSHero(IntPtr p) :CSHero((SHero*)p.ToPointer()) { }
		CSHero(SHero *p) :CSObject(p)
		{
			type = p->type;
			hp = p->hp;
			energy = p->energy;
			direction = gcnew CSVector2(p->direction);
			state = (CSHeroState)p->state;
			prvTime = p->prvTime;
			ai = gcnew IntPtr(p->ai);
			aiThread = gcnew IntPtr(p->aiThread);
		}

		property sint Type { sint get() { return type; } }
		property sint HP { sint get() { return hp; } }
		property sint Energy { sint get() { return energy; } }
		property CSVector2^ Direction { CSVector2^ get() { return direction; } }
		property CSHeroState State { CSHeroState get() { return state; } }

	private:
		sint type;
		sint hp;
		sint energy;
		CSVector2 ^direction;
		CSHeroState state;
		stime prvTime;
		IntPtr ^ai;
		IntPtr ^aiThread;
	};

	public ref class CSAnimal :CSObject
	{
	public:
		CSAnimal(IntPtr p) :CSAnimal((SAnimal*)p.ToPointer()) { }
		CSAnimal(SAnimal *p) :CSObject(p)
		{
		}
	};

	public enum class CSStillObjectType :int
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

	public ref class CSStillObject :CSObject
	{
	public:
		CSStillObject(IntPtr p) :CSStillObject((SStillObject*)p.ToPointer()) { }
		CSStillObject(SStillObject *p) :CSObject(p)
		{
			type = (CSStillObjectType)p->type;
		}

		property CSStillObjectType Type { CSStillObjectType get() { return type; } }

	private:
		CSStillObjectType type;
	};
}
