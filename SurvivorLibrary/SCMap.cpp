#include "SCMap.h"

SCMap::SCMap(int seed, scint size)
{
	this->seed = seed;
	this->size = size;
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
	in >> seed >> size;
}

void SCMap::GenerateRandomMap()
{

}