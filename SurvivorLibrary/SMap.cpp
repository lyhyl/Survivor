#include <random>
#include "SMap.h"

using namespace std;

namespace SurvivorLibrary
{
	/*RDTSC*/
	inline unsigned __int64 GetCycleCount()
	{
		__asm _emit 0x0F
		__asm _emit 0x31
	}

	SMap::SMap(ssize w, ssize h) :width(w), height(h)
	{
		seed = (unsigned)(GetCycleCount() & 0xffffffff);
		GenerateRandomMap();
	}

	SMap::SMap(unsigned s, ssize w, ssize h) :seed(s), width(w), height(h)
	{
		GenerateRandomMap();
	}

	SMap::SMap(std::istream &in)
	{
		ReadinFile(in);
	}

	SMap::~SMap()
	{
	}

	ostream &SMap::operator<<(std::ostream &out)
	{
		return out;
	}

	istream &SMap::operator>>(std::istream &in)
	{
		Clear();
		ReadinFile(in);
		return in;
	}

	void SMap::Clear()
	{

	}

	void SMap::ReadinFile(std::istream &in)
	{
		in >> seed >> width >> height;
	}

	void SMap::GenerateRandomMap()
	{
		default_random_engine random(seed);
		uniform_int_distribution<ssize> getCount(0, static_cast<ssize>(sqrt(width * height)));
		uniform_real_distribution<sfloat> getPositionX(0, (sfloat)width);
		uniform_real_distribution<sfloat> getPositionY(0, (sfloat)height);
		auto count = getCount(random);
		while (count--)
		{
			SStillObject *so = new SStillObject;
			so->position = { getPositionX(random), getPositionY(random) };
			so->type = SStillObjectType::Tree;
			stillObjects.emplace_back(move(so));
		}
	}
}
