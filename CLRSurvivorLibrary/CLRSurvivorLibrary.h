#pragma once

#include <cliext\vector>

#include <SAIAdapter.h>
#include <SCommunication.h>
#include <SDefines.h>
#include <SMap.h>
#include <SMath.h>
#include <SObject.h>
#include <SUIAdapter.h>

using namespace cliext;
using namespace System;
using namespace System::Collections::Generic;
using namespace SurvivorLibrary;

namespace CLRSurvivorLibrary
{
	public value struct CSPoint
	{
		sfloat x, y;
	};

	public ref struct CSObject
	{
		sint id;
		CSPoint position;
	};

	public ref struct CSHero :CSObject
	{

	};

	public ref struct CSStillObject :CSObject
	{

	};

	public ref class CSMap
	{
		const SMap *unmptr = nullptr;
	public:
		CSMap(IntPtr p) { unmptr = static_cast<const SMap*> (p.ToPointer()); }

		property ssize Width { ssize get() { return unmptr->DefaultSize; } }
		property ssize Height { ssize get() { return unmptr->DefaultSize; } }

		property ICollection<CSStillObject^>^ StillObjects
		{
			ICollection<CSStillObject^>^ get() { return gcnew vector<CSStillObject^>(); }
		}

	internal:
		CSMap(const SMap *p) { unmptr = p; }

	private:
		~CSMap() { this->!CSMap(); }
		!CSMap()	{ delete unmptr; }
	};

	public ref struct CSUIDisplayData
	{
		const SUIDisplayData *unmptr = nullptr;
	public:
		CSUIDisplayData(IntPtr p) { unmptr = static_cast<const SUIDisplayData*> (p.ToPointer()); }

		property CSMap^ Map { CSMap^ get() { return gcnew CSMap(unmptr->map); } }

	internal:
		CSUIDisplayData(const SUIDisplayData *p) { unmptr = p; }

	private:
		~CSUIDisplayData() { this->!CSUIDisplayData(); }
		!CSUIDisplayData()	{ delete unmptr; }
	};
}
