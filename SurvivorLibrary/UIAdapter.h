#pragma once
#include "SCDefines.h"
#include "SCStructure.h"

class API UIAdapter
{
private:
public:
	UIAdapter(){}
	virtual ~UIAdapter(){}
	virtual __int32 Display(const struct UIDisplayData*) = 0;
};