#pragma once
#include <fstream>
#include "SMath.h"
using namespace std;

namespace SurvivorLibrary
{
	class API SMap
	{
	public:
		static const ssize DefaultSize = 5120;
		static sint ResID;

		SMap(ssize w = DefaultSize, ssize h = DefaultSize);
		SMap(unsigned seed, ssize w = DefaultSize, ssize h = DefaultSize);
		SMap(istream &in);
		~SMap();

		istream &operator>>(istream &in);
		ostream &operator<<(ostream &out);
	private:
		unsigned seed;
		ssize width, height;

		void Clear();
		void ReadinFile(istream &in);
		void GenerateRandomMap();
	};
}
