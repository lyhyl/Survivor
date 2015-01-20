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
		virtual __int32 Initialize(struct SInitializeData*) = 0;
		virtual __int32 Update(struct SUpdateData*) = 0;
	};
}