#pragma once
#include <fstream>
#include "SCUtilities.h"
#include "SCCollection.h"
using namespace std;

struct SCMapResource;

class API SCMap
{
public:
	static const scsize DefaultSize = 5120;

	SCMap(scsize w = DefaultSize, scsize h = DefaultSize);
	SCMap(int seed, scsize w = DefaultSize, scsize h = DefaultSize);
	SCMap(istream &in);
	~SCMap();

	istream &operator>>(istream &in);
	ostream &operator<<(ostream &out);
private:
	unsigned seed;
	scsize width, height;
	// warning C4251
	// Just treat it as SCCollection outside
#pragma warning(disable:4251)
	SCCollectionX<SCMapResource*> *resources;
#pragma warning(default:4251)

	void Clear();
	void ReadinFile(istream &in);
	void GenerateRandomMap();
};

enum class SCMapResourceType :int
{
	// Life dynnamic
	Hero, WildAnimal,
	Blood, Excrement, Body,
	// Life static
	Tree,
	Charcoal,
	// Barrier
	Rock, Hill,
	// !
	Water, Stream, Lake,
	Hole, Fire, Smoke,
	// Weather
	Fog, Rain, Thunder
};

struct API SCMapResource
{
public:
	SCMapResource()
	{
	}
	SCMapResource(SCRegion &reg)
	{
		region = reg;
	}
private:
	SCMapResourceType type;
	SCRegion region;
};
