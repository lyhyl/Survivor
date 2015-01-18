#pragma once
#include "SCommunication.h"

namespace SurvivorLibrary
{
	class API SUIAdapter
	{
	private:
	public:
		SUIAdapter(){}
		virtual ~SUIAdapter(){}
		virtual __int32 Display(struct SUIDisplayData*) = 0;
	};
}