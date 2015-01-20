#pragma once

#include <cliext\vector>

#include <SAIAdapter.h>
#include <SCommunication.h>
#include <SDefines.h>
#include <SUIAdapter.h>

#include "CLRSurvivorLibrary.h"
#include "CLRSMath.h"
#include "CLRSObject.h"
#include "CLRSMap.h"

using namespace cliext;
using namespace System;
using namespace System::Collections::Generic;
using namespace SurvivorLibrary;

namespace CLRSurvivorLibrary
{
	public enum class CSGState :int
	{
		OK = (int)SGState::OK,
		UnknowError = (int)SGState::UnknowError,
		AIError = (int)SGState::AIError,
		UIError = (int)SGState::UIError,
		UITooManyUpdateData = (int)SGState::UITooManyUpdateData,
		UIExited = (int)SGState::UIExited
	};

	public ref struct CSUpdateData
	{
	public:
		CSUpdateData(IntPtr p) :CSUpdateData((SUpdateData*)p.ToPointer()) { }

		CSUpdateData(SUpdateData *p)
		{
			stillObjects = CSMarshal::Vector<CSStillObject>(p->stillObjects);
			heroes = CSMarshal::Vector<CSHero>(p->heroes);
			animals = CSMarshal::Vector<CSAnimal>(p->animals);
		}

		property ICollection<CSAnimal^>^ Animals
		{ ICollection<CSAnimal^>^ get() { return animals; } }
		property ICollection<CSHero^>^ Heroes
		{ ICollection<CSHero^>^ get() { return heroes; } }
		property ICollection<CSStillObject^>^ StillObjects
		{ ICollection<CSStillObject^>^ get() { return stillObjects; } }

	private:
		ICollection<CSAnimal^>^ animals = nullptr;
		ICollection<CSHero^>^ heroes = nullptr;
		ICollection<CSStillObject^>^ stillObjects = nullptr;
	};

	public ref class CSInitializeData
	{
	public:
		CSInitializeData(IntPtr p) :CSInitializeData((SInitializeData*)p.ToPointer()) { }

		CSInitializeData(SInitializeData *p)
		{
			map = gcnew CSMap(p->map);
			heroes = CSMarshal::Vector<CSHero>(p->heroes);
		}

		property CSMap^ Map
		{ CSMap^ get() { return map; } }
		property ICollection<CSHero^>^ Heroes
		{ ICollection<CSHero^>^ get() { return heroes; } }

	private:
		CSMap^ map = nullptr;
		ICollection<CSHero^>^ heroes = nullptr;
	};
}
