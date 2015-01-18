#include <random>
#include "SMap.h"

using namespace std;

namespace SurvivorLibrary
{
	sint SMap::ResID = 0;

	SMap::SMap(ssize w, ssize h) :width(w), height(h)
	{
		default_random_engine random;
		seed = random();
		GenerateRandomMap();
	}

	SMap::SMap(unsigned s, ssize w, ssize h) :seed(s), width(w), height(h)
	{
		GenerateRandomMap();
	}

	SMap::SMap(istream &in)
	{
		ReadinFile(in);
	}

	SMap::~SMap()
	{
	}

	ostream &SMap::operator<<(ostream &out)
	{
		return out;
	}

	istream &SMap::operator>>(istream &in)
	{
		Clear();
		ReadinFile(in);
		return in;
	}

	void SMap::Clear()
	{

	}

	void SMap::ReadinFile(istream &in)
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
			SRegion region{ { getPositionX(random), getPositionY(random) } };
		}
	}
}
