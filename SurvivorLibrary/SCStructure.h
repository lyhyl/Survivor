#pragma once
#include "SCUtilities.h"
#include "SCCompetitor.h"
#include "SCMap.h"

using namespace std;

struct API SCCompetitorVision
{
	SCCollection *organisms;
	SCCollection *resources;
};

struct API SCCompetitorAction
{
	scint dirx, diry;
};