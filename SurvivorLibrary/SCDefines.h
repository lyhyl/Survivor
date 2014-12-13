#pragma once
typedef signed __int64 scint;
typedef unsigned __int64 scsize;

#ifdef _DLL
#	define API __declspec(dllexport)
#else
#	define API __declspec(dllimport)
#endif