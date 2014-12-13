#pragma once
#include "SCCollection.h"
#include "SCCompetitor.h"
#include "SCMap.h"
using namespace std;
class UIAdapter
{
private:
public:
	UIAdapter(){}
	virtual ~UIAdapter(){}
	virtual __int32 Display(SCMap &map, SCCollection &competitors) = 0;
};