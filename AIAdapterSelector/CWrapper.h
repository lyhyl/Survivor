#pragma once
#include <Windows.h>
#include "SAIAdapter.h"

using namespace std;
using namespace SurvivorLibrary;

class CImplWrapper :SAIAdapter
{
private:
	typedef SAIAdapter*(*pNewAdapter)();
	SAIAdapter *pAI = nullptr;
	HMODULE hDLL;
public:
	CImplWrapper(wstring &file)
	{
		hDLL = LoadLibrary(file.c_str());
		if (hDLL)
		{
			pNewAdapter newAIAdapter = (pNewAdapter)GetProcAddress(hDLL, "CreateAIAdapter");
			if (newAIAdapter)
				pAI = newAIAdapter();
		}
		else
		{
			DWORD e = GetLastError();
			wchar_t msg[512] = L"";
			swprintf_s(msg, L"Error %lu\nInvail AI : %s", e, file.c_str());
			MessageBox(0, msg, L"Error", 0);
		}
	}
	~CImplWrapper()
	{
		if (pAI)
			delete pAI;
		if (hDLL)
			FreeLibrary(hDLL);
	}
	virtual SHeroAction Think(const SAIThinkData *data)
	{
		return pAI ? pAI->Think(data) : NoAction;
	}
};