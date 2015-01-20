#pragma once
#include <fstream>
#include "SMath.h"
#include "SObject.h"

namespace SurvivorLibrary
{
	template class API std::vector < SStillObject* >;
	template class API std::vector < SAnimal* >;
	class API SMap
	{
	public:
		static const ssize DefaultSize = 5120;

		SMap(ssize w = DefaultSize, ssize h = DefaultSize);
		SMap(unsigned seed, ssize w = DefaultSize, ssize h = DefaultSize);
		SMap(std::istream &in);
		~SMap();

		std::istream &operator>>(std::istream &in);
		std::ostream &operator<<(std::ostream &out);

		unsigned Seed() { return seed; }
		ssize Width() { return width; }
		ssize Height() { return height; }

		std::vector<SStillObject*>& StillObjects() { return this->stillObjects; }
		std::vector<SAnimal*>& Animals() { return this->animals; }

	private:
		unsigned seed;
		ssize width, height;

		std::vector<SStillObject*> stillObjects;
		std::vector<SAnimal*> animals;

		void Clear();
		void ReadinFile(std::istream &in);
		void GenerateRandomMap();
	};
}
