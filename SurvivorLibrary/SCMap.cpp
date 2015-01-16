#include <random>
#include "SCMap.h"

using namespace std;

SCMap::SCMap(scsize w, scsize h) :width(w), height(h)
{
	default_random_engine random;
	seed = random();
	resources = new SCCollectionX<SCMapResource*>();
	GenerateRandomMap();
}

SCMap::SCMap(unsigned s, scsize w, scsize h) :seed(s), width(w), height(h)
{
	resources = new SCCollectionX<SCMapResource*>();
	GenerateRandomMap();
}

SCMap::SCMap(istream &in)
{
	resources = new SCCollectionX<SCMapResource*>();
	ReadinFile(in);
}

SCMap::~SCMap()
{
	if (resources)
	{

		delete resources;
	}
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
	uniform_real_distribution<scfloat> getPosition(0, (scfloat)(width * height));
	auto count = getCount(random);
	while (count--)
	{
		scfloat x = getPosition(random);
		scfloat y = getPosition(random);
		SCPoint position = { x, y };
		SCRegion region{ position };
		SCMapResource resource(region, SCMapResourceType::Rock);
	}
}