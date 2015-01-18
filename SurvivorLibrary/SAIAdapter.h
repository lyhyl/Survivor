#pragma once
#include "SCommunication.h"

namespace SurvivorLibrary
{
	class API SAIAdapter
	{
	private:
	public:
		SAIAdapter(){}
		virtual ~SAIAdapter(){}
		virtual struct SHeroAction Think(const struct SAIThinkData*) = 0;
	};
}
