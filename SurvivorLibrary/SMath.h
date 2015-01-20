#pragma once
#include <initializer_list>
#include <memory>
#include <cmath>
#include "SDefines.h"

namespace SurvivorLibrary
{
	struct API SVector2
	{
		sfloat x, y;
	};

	inline sfloat SDot(SVector2 a, SVector2 b){ return a.x*b.x + a.y*b.y; }
	inline sfloat SCross(SVector2 a, SVector2 b){ return a.x*b.y - a.y*b.x; }
	inline sfloat SLengthSq(SVector2 v){ return v.x*v.x + v.y*v.y; }
	inline sfloat SLength(SVector2 v){ return sqrt(SLengthSq(v)); }
	inline void SNormalize(SVector2 &v){ sfloat len = SLength(v); v.x /= len; v.y /= len; }

	inline SVector2 operator+(SVector2 a, SVector2 b){ return{ a.x + b.x, a.y + b.y }; }
	inline SVector2 operator-(SVector2 a, SVector2 b){ return{ a.x - b.x, a.y - b.y }; }
	inline SVector2 operator*(SVector2 v, sfloat f){ return{ v.x * f, v.y * f }; }
	inline SVector2 operator*(sfloat f, SVector2 v){ return{ v.x * f, v.y * f }; }
	inline SVector2 operator/(SVector2 v, sfloat f){ return{ v.x / f, v.y / f }; }

	inline SVector2 operator+=(SVector2 &a, SVector2 &b){ a.x += b.x, a.y += b.y; return a; }
	inline SVector2 operator-=(SVector2 &a, SVector2 &b){ a.x -= b.x, a.y -= b.y; return a; }
	inline SVector2 operator*=(SVector2 &v, sfloat f){ v.x *= f, v.y *= f; return v; }
	inline SVector2 operator/=(SVector2 &v, sfloat f){ v.x /= f, v.y /= f; return v; }

	inline void SRotate(SVector2 &v, sfloat a)
	{
		sfloat s = sin(a), c = cos(a);
		v = { v.x*c - v.y*s, v.x*s + v.y*c };
	}

	struct API STriangle
	{
		SVector2 a, b, c;
	};

	struct API SRegion
	{
	public:
		SRegion()
		{
			vertexCount = 0;
			verties = nullptr;
		}
		SRegion(std::initializer_list<SVector2> vs) :SRegion(vs.begin(), vs.size())
		{
		}
		SRegion(const SVector2 *vs, ssize size)
		{
			vertexCount = size;
			rsize_t memsiz = (rsize_t)vertexCount * sizeof(SVector2);
			verties = new SVector2[(size_t)vertexCount];
			memcpy_s(verties, memsiz, vs, memsiz);
		}
		SRegion& operator=(SRegion &r)
		{
			vertexCount = r.vertexCount;
			rsize_t memsiz = (rsize_t)vertexCount * sizeof(SVector2);
			verties = new SVector2[(size_t)vertexCount];
			memcpy_s(verties, memsiz, r.verties, memsiz);
			return *this;
		}
		SRegion& operator=(SRegion &&r)
		{
			vertexCount = r.vertexCount;
			rsize_t memsiz = (rsize_t)vertexCount * sizeof(SVector2);
			verties = new SVector2[(size_t)vertexCount];
			memcpy_s(verties, memsiz, r.verties, memsiz);
			return *this;
		}
		~SRegion()
		{
			if (verties)
				delete[] verties;
		}
		bool Empty()const{ return vertexCount == 0; }
		bool Contain(SVector2 p)const
		{
			//TODO
			return false;
		}
	private:
		ssize vertexCount = 0;
		SVector2 *verties = nullptr;

		ssize triangleCount = 0;
		STriangle *triangles = nullptr;

		void DelaunayTriangulation(SVector2 *verties, ssize count)
		{
			//TODO
		}
	};
}
