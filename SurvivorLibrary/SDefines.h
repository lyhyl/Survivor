#pragma once

namespace SurvivorLibrary
{
	typedef signed __int64 sint;
	typedef unsigned __int64 ssize;

	typedef double sfloat;
	typedef __int64 stime;

#ifdef _DLL
#	define API __declspec(dllexport)
#else
#	define API __declspec(dllimport)
#endif

	API const char * GetHeroesDirA();
	API const wchar_t * GetHeroesDirW();

	API enum class SGState :int
	{
		OK,
		UnknowError,
		AIError = 0x10000000,
		UIError = 0x20000000,
		UITooManyUpdateData,
		UIExited
	};
}
