// This is the main DLL file.

#include "stdafx.h"
#include <set>
#include <string>
#include "UIAdapterSelector.h"
#include "UIAdapterWapper.h"

using namespace System;
using namespace System::IO;
using namespace System::Reflection;
using namespace System::Runtime::InteropServices;

static bool Initialized = false;

static gcroot<Assembly^> CSharpAssembly;
static CSharpImplWapper *_CSharpImpl;

static HMODULE *_CImpl;

gcroot<String^> UIRootPath = Path::Combine(Path::GetDirectoryName(Assembly::GetExecutingAssembly()->Location), "UI");

void InitializeCPP(String ^assName)
{
	// TODO
	throw gcnew Exception(L"Not Impl");
	Initialized = true;
}

void InitializeCSharp(String ^assName)
{
	String ^csClassName = assName + ".UIAdapter";
	String ^assPath = Path::Combine(UIRootPath, assName + ".dll");
	CSharpAssembly = Assembly::LoadFile(assPath);
	Object ^instance = CSharpAssembly->CreateInstance(csClassName);
	MethodInfo ^method = CSharpAssembly->GetType(csClassName)->GetMethod("Display");
	_CSharpImpl = new CSharpImplWapper(instance, method);
	Initialized = true;
}

void *CreateUIAdapter(const wchar_t* option)
{
	String ^decoratedOption = gcnew String(option);
	decoratedOption += L"Impl";
	try // C/C++
	{
		if (!Initialized)
			InitializeCPP(decoratedOption);
		return _CImpl;
	}
	catch (...){}
	try // C#
	{
		if (!Initialized)
			InitializeCSharp(decoratedOption);
		return _CSharpImpl;
	}
	catch (...){}
	return 0;
}

const wchar_t** EnumUIAdapters(int *count)
{
	wstring folder((wchar_t*)(void*)Marshal::StringToHGlobalAuto(UIRootPath));
	set<wstring> options;

	if (CreateDirectory((folder+L"\\").c_str(), NULL) || ERROR_ALREADY_EXISTS == GetLastError())
	{
		WIN32_FIND_DATA fd;
		HANDLE hFind = FindFirstFile((folder + L"\\*Impl.dll").c_str(), &fd);
		if (hFind != INVALID_HANDLE_VALUE)
		{
			do
				if (!(fd.dwFileAttributes & FILE_ATTRIBUTE_DIRECTORY))
				{
					wstring file(fd.cFileName);
					// remove `Impl.dll`
					options.emplace(move(file.substr(0, file.length() - 8)));
				}
			while (FindNextFile(hFind, &fd));
			FindClose(hFind);
		}
	}

	wchar_t ** opt = new wchar_t*[options.size()];
	wchar_t ** popt = opt;
	for each (auto str in options)
	{
		int len = str.length() + 1;
		*popt = new wchar_t[len];
		int memsize = sizeof(wchar_t) * len;
		memcpy_s(*popt, memsize, str.c_str(), memsize);
		popt++;
	}

	*count = options.size();
	return const_cast<const wchar_t**>(opt);
}