#pragma once
#include "SCCompetitor.h"
#include "SCUtilities.h"
#include "SCStructure.h"
class AIAdapter
{
private:
public:
	AIAdapter(){}
	virtual ~AIAdapter(){}
	virtual SCCompetitorAction Think(SCCompetitor *competitor, SCCompetitorVision vision) = 0;
};