#pragma once
#include "SCDefines.h"
#include "SCStructure.h"

class API AIAdapter
{
private:
public:
	AIAdapter(){}
	virtual ~AIAdapter(){}
	virtual struct SCHeroAction Think(const struct AIThinkData*) = 0;
};