#include <random>
#include "SCMap.h"

using namespace std;

SCMap::SCMap(scsize w, scsize h) :width(w), height(h)
{
	default_random_engine random;
	seed = random();
	GenerateRandomMap();
}

SCMap::SCMap(int s, scsize w, scsize h) :seed(s), width(w), height(h)
{
	GenerateRandomMap();
}

SCMap::SCMap(istream &in)
{
	ReadinFile(in);
}

SCMap::~SCMap()
{
}

ostream &SCMap::operator<<(ostream &out)
{
	return out;
}

istream &SCMap::operator>>(istream &in)
{
	Clear();
	ReadinFile(in);
	return in;
}

void SCMap::Clear()
{

}

void SCMap::ReadinFile(istream &in)
{
	in >> seed >> width >> height;
}

void SCMap::GenerateRandomMap()
{
	default_random_engine random(seed);
	uniform_int_distribution<scsize> getCount(0, width * height);
	uniform_real_distribution<long double> getPosition(0, (long double)(width * height));
	auto count = getCount(random);
	while (count--)
	{
		long double x = getPosition(random);
		long double y = getPosition(random);
	}
}