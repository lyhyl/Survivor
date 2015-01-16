#pragma once
#include <Windows.h>
#include "AIAdapter.h"

class CImplWrapper :AIAdapter
{
private:
	typedef AIAdapter*(*pNewAdapter)();
	AIAdapter *pAI = nullptr;
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
			TCHAR msg[50] = L"";
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
	virtual SCHeroAction Think(const AIThinkData *data)
	{
		return pAI ? pAI->Think(data) : NoAction;
	}
};