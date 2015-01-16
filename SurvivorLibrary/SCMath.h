#pragma once
#include <initializer_list>
#include <memory>
#include <cmath>
#include "SCDefines.h"
using namespace std;

struct API SCPoint
{
	scfloat x, y;
};

typedef SCPoint SCVector2;

inline scfloat SCDot(SCVector2 a, SCVector2 b){ return a.x*b.x + a.y*b.y; }
inline scfloat SCCross(SCVector2 a, SCVector2 b){ return a.x*b.y - a.y*b.x; }
inline scfloat SCLengthSq(SCVector2 v){ return v.x*v.x + v.y*v.y; }
inline scfloat SCLength(SCVector2 v){ return sqrt(SCLengthSq(v)); }
inline void SCNormalize(SCVector2 &v){ scfloat len = SCLength(v); v.x /= len; v.y /= len; }

inline SCVector2 operator+(SCVector2 a, SCVector2 b){ return{ a.x + b.x, a.y + b.y }; }
inline SCVector2 operator-(SCVector2 a, SCVector2 b){ return{ a.x - b.x, a.y - b.y }; }
inline SCVector2 operator*(SCVector2 v, scfloat f){ return{ v.x * f, v.y * f }; }
inline SCVector2 operator*(scfloat f, SCVector2 v){ return{ v.x * f, v.y * f }; }
inline SCVector2 operator/(SCVector2 v, scfloat f){ return{ v.x / f, v.y / f }; }

inline SCVector2 operator+=(SCVector2 &a, SCVector2 &b){ a.x += b.x, a.y += b.y; return a; }
inline SCVector2 operator-=(SCVector2 &a, SCVector2 &b){ a.x -= b.x, a.y -= b.y; return a; }
inline SCVector2 operator*=(SCVector2 &v, scfloat f){ v.x *= f, v.y *= f; return v; }
inline SCVector2 operator/=(SCVector2 &v, scfloat f){ v.x /= f, v.y /= f; return v; }

inline void SCRotate(SCVector2 &v, scfloat a)
{
	scfloat s = sin(a), c = cos(a);
	v = { v.x*c - v.y*s, v.x*s + v.y*c };
}

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
	SCRegion(initializer_list<SCPoint> vs) :SCRegion(vs.begin(), vs.size())
	{
	}
	SCRegion(const SCPoint *vs, scsize size)
	{
		vertexCount = size;
		rsize_t memsiz = (rsize_t)vertexCount * sizeof(SCPoint);
		verties = new SCPoint[(size_t)vertexCount];
		memcpy_s(verties, memsiz, vs, memsiz);
	}
	SCRegion& operator=(SCRegion &r)
	{
		vertexCount = r.vertexCount;
		rsize_t memsiz = (rsize_t)vertexCount * sizeof(SCPoint);
		verties = new SCPoint[(size_t)vertexCount];
		memcpy_s(verties, memsiz, r.verties, memsiz);
		return *this;
	}
	SCRegion& operator=(SCRegion &&r)
	{
		vertexCount = r.vertexCount;
		rsize_t memsiz = (rsize_t)vertexCount * sizeof(SCPoint);
		verties = new SCPoint[(size_t)vertexCount];
		memcpy_s(verties, memsiz, r.verties, memsiz);
		return *this;
	}
	~SCRegion()
	{
		if (verties)
			delete[] verties;
	}
	bool Empty()const{ return vertexCount == 0; }
	bool Contain(SCPoint p)const
	{
		//TODO
		return false;
	}
private:
	scsize vertexCount = 0;
	SCPoint *verties = nullptr;

	scsize triangleCount = 0;
	SCTriangle *triangles = nullptr;

	void DelaunayTriangulation(SCPoint *verties, scsize count)
	{
		//TODO
	}
};