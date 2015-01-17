// UIAdapterSelector.h

#pragma once

#ifdef _DLL
#define SCEXPORT __declspec(dllexport)
#else
#define SCEXPORT __declspec(dllimport)
#endif

extern "C" SCEXPORT void *CreateUIAdapter(const wchar_t*);
// need to delete result
extern "C" SCEXPORT const wchar_t** EnumUIAdapters(int *count);