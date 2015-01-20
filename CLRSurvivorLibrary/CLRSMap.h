#pragma once

#include <cliext\vector>

#include <SMap.h>

#include "CLRSurvivorLibrary.h"
#include "CLRSMath.h"
#include "CLRSObject.h"

using namespace cliext;
using namespace System;
using namespace System::Collections::Generic;
using namespace SurvivorLibrary;

namespace CLRSurvivorLibrary
{
	public ref class CSMap
	{
	public:
		CSMap(IntPtr p) :CSMap((SMap*)p.ToPointer()) { }

		CSMap(SMap *p)
		{
			seed = p->Seed();
			width = p->Width();
			height = p->Height();
			stillObjects = CSMarshal::Vector<CSStillObject>(p->StillObjects());
			animals = CSMarshal::Vector<CSAnimal>(p->Animals());
		}

		property ssize Width { ssize get() { return width; } }
		property ssize Height { ssize get() { return height; } }
		property ICollection<CSStillObject^>^ StillObjects
		{ ICollection<CSStillObject^>^ get() { return stillObjects; } }
		property ICollection<CSAnimal^>^ Animals
		{ ICollection<CSAnimal^>^ get() { return animals; } }

	private:
		unsigned seed;
		ssize width, height;
		ICollection<CSStillObject^>^ stillObjects;
		ICollection<CSAnimal^>^ animals;
	};
}
