#pragma once
#include <SDefines.h>
#include <SMath.h>

using namespace SurvivorLibrary;

namespace CLRSurvivorLibrary
{
	public ref struct CSVector2
	{
	public:
		sfloat x, y;
		CSVector2(sfloat px, sfloat py) :x(px), y(py) { }
		CSVector2(const SVector2 &p) :x(p.x), y(p.y) { }
		CSVector2(const SVector2 &&p) :x(p.x), y(p.y) { }
	};

	public ref struct STriangle
	{
		CSVector2 ^a, ^b, ^c;
	};

	public ref struct SRegion
	{
	public:
	};
}
