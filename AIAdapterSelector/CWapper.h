#pragma once
#include <Windows.h>
#include "AIAdapter.h"

class CImplWapper :AIAdapter
{
private:
	typedef AIAdapter*(*pNewAdapter)();
	AIAdapter *pAI = nullptr;
	HMODULE hDLL;
public:
	CImplWapper(wstring &file)
	{
		hDLL = LoadLibrary(file.c_str());
		if (hDLL != INVALID_HANDLE_VALUE)
		{
			pNewAdapter newAIAdapter = (pNewAdapter)GetProcAddress(hDLL, "CreateAIAdapter");
			if (newAIAdapter != 0)
				pAI = newAIAdapter();
		}
	}
	~CImplWapper()
	{
		if (pAI)
			delete pAI;
		if (hDLL != INVALID_HANDLE_VALUE)
			FreeLibrary(hDLL);
	}
	virtual SCCompetitorAction Think(SCCompetitor *competitor, SCCompetitorVision vision)
	{
		return pAI->Think(competitor, vision);
	}
};