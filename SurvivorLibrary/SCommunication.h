#pragma once
#include "SObject.h"
#include "SMap.h"

namespace SurvivorLibrary
{
	struct API SHeroVision
	{
	};

	struct API SHeroHearing
	{

	};

	struct API SHeroTouch
	{

	};

	struct API SHeroAction
	{
		SHeroState type;
		union
		{
			// Stay Attack Climb
			struct { sfloat r0, r1, r2, r3; };
			// Turn Move Run
			struct { sfloat angle, r1, r2, r3; };
		};
	};

	const SHeroAction API NoAction = { SHeroState::Stay, 0 };

	struct API SUIDisplayData
	{
		const SMap * map = nullptr;
	};

	struct API SAIThinkData
	{
		const SHero *target;
		const SHeroVision *vision;
		const SHeroHearing *hearing;
		const SHeroTouch *touch;
	};
}
