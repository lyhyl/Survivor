#pragma once
#include <initializer_list>
#include "SCDefines.h"
using namespace std;

struct API SCPoint
{
	scfloat x, y;
};

typedef SCPoint SCVector2;

struct API SCTriangle
{
	SCPoint a, b, c;
};

struct API SCRegion
{
public:
	SCRegion()
	{
		vertexCount = 0;
		verties = nullptr;
	}
	SCRegion(initializer_list<SCPoint> vs)
	{
		vertexCount = vs.size();
		verties = new SCPoint[(size_t)vertexCount];
		memcpy_s(verties, (rsize_t)vertexCount, vs.begin(), (rsize_t)vertexCount);
	}
	SCRegion(SCPoint *vs, scsize size)
	{
		vertexCount = size;
		rsize_t memsiz = (rsize_t)vertexCount * sizeof(SCPoint);
		memcpy_s(verties, memsiz, vs, memsiz);
		verties = vs;
	}
	~SCRegion()
	{
		delete[] verties;
	}
	bool Empty()const{ return vertexCount == 0; }
	bool Contain(SCPoint p)const
	{
		//TODO
		return false;
	}
private:
	scsize triangleCount;
	SCTriangle *triangles;
	scsize vertexCount;
	SCPoint *verties;

	void DelaunayTriangulation(SCPoint *verties, scsize count)
	{

	}
};