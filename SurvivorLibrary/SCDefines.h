#pragma once
typedef signed __int64 scint;
typedef unsigned __int64 scsize;

typedef double scfloat;
typedef __int64 scptr;
typedef __int64 sctime;

#ifdef _DLL
#	define API __declspec(dllexport)
#else
#	define API __declspec(dllimport)
#endif

API const char * GetHeroesDirA();
API const wchar_t * GetHeroesDirW();