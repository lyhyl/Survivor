#pragma once
#include <fstream>
#include "AIAdapter.h"
#include "SCCollection.h"
#include "SCMap.h"
#include "SCCompetitor.h"
using namespace std;
class SCGame
{
public:
	SCGame(AIAdapter*[]);
	SCGame(istream &in);
	~SCGame();

	void ApplyAction(AIAdapter*const, SCCompetitorAction) const;
	SCCompetitorAction TimeLimitThink(AIAdapter*const, SCCompetitorVision) const;
	void Run() const;

	SCMap &GetMap();
	SCCollection &GetCompetitors();

	void *GetLog();
private:
	SCMap Map;
	SCCollection Competitors;
};

