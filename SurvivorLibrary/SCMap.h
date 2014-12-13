#pragma once
#include <fstream>
#include "SCUtilities.h"
#include "SCCollection.h"
using namespace std;

class API SCMap
{
public:
	static const scint DefaultSize = 512;

	SCMap(int seed = 0, scint size = DefaultSize);
	SCMap(istream &in);
	~SCMap();

	istream &operator>>(istream &in);
	ostream &operator<<(ostream &out);
private:
	int seed;
	scint size;
	SCCollection resources;

	void Clear();
	void ReadinFile(istream &in);
	void GenerateRandomMap();
};

struct API SCMapResource
{
public:
	SCMapResource()
	{
	}
	SCMapResource(Region &reg)
	{
		region = reg;
	}
private:
	Region region;
};
