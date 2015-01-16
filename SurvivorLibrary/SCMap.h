#pragma once
#include <fstream>
#include "SCMath.h"
#include "SCCollection.h"
using namespace std;

struct SCMapResource;

class API SCMap
{
public:
	static const scsize DefaultSize = 5120;

	SCMap(scsize w = DefaultSize, scsize h = DefaultSize);
	SCMap(unsigned seed, scsize w = DefaultSize, scsize h = DefaultSize);
	SCMap(istream &in);
	~SCMap();

	istream &operator>>(istream &in);
	ostream &operator<<(ostream &out);
private:
	unsigned seed;
	scsize width, height;
	// warning C4251
	// Just treat it as SCCollection* outside
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
	// Trap
	Water, Stream, Lake,
	Hole, Fire, Smoke,
	// Weather
	Fog, Rain, Thunder,
	//
	TypeCount
};

struct API SCMapResource
{
public:
	SCMapResource(SCRegion &reg, SCMapResourceType t = SCMapResourceType::Rock)
	{
		type = t;
		region = reg;
	}
private:
	SCMapResourceType type;
	SCRegion region;
};
