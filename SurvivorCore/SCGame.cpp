#include <hash_map>
#include <future>
#include <chrono>
#include "SCGame.h"

using namespace std;
using namespace std::chrono;

SCGame::SCGame(AIAdapter *ais[])
{
	AIAdapter **ppAI = ais;
	while (*ppAI) Competitors.Add({ *ppAI++ });
}

SCGame::SCGame(istream &in) :Map(in)
{
}

SCGame::~SCGame()
{
}

void SCGame::ApplyAction(AIAdapter *const com, SCCompetitorAction act) const
{
	// TODO
}

SCCompetitorAction SCGame::TimeLimitThink(AIAdapter *const com, SCCompetitorVision vis) const
{
	future<SCCompetitorAction> result = async( [com, vis]{ return com->Think(0, vis); });
	switch (result.wait_for(milliseconds(500)))
	{
	case future_status::ready:
		return result.get();
	default:
		return{ 0 };
	}
}

void SCGame::Run() const
{
	hash_map<AIAdapter*, SCCompetitorAction> actionLog;
	scsize comc = Competitors.Size();
	SCCompetitorVision v = { 0 };
	for (scsize i = 0; i < comc; i++)
	{
		AIAdapter *com = (AIAdapter*)Competitors[i]->ptr;
		actionLog[com] = TimeLimitThink(com, v);
	}
	for (auto p : actionLog)
		ApplyAction(p.first, p.second);
}

SCMap &SCGame::GetMap()
{
	return Map;
}

SCCollection &SCGame::GetCompetitors()
{
	return Competitors;
}

void *SCGame::GetLog()
{
	return 0;
}
