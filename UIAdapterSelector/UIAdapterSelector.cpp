// This is the main DLL file.

#include "stdafx.h"
#include <set>
#include <string>
#include <functional>
#include <SUtility.h>
#include "UIAdapterSelector.h"
#include "UIAdapterWrapper.h"

using namespace std;
using namespace System;
using namespace System::IO;
using namespace System::Reflection;
using namespace System::Runtime::InteropServices;

static bool Initialized = false;

static gcroot<Assembly^> CSharpAssembly;
static CSharpImplWrapper *_CSharpImpl;

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
	MethodInfo ^initMethod = CSharpAssembly->GetType(csClassName)->GetMethod("Initialize");
	MethodInfo ^updateMethod = CSharpAssembly->GetType(csClassName)->GetMethod("Update");
	_CSharpImpl = new CSharpImplWrapper(instance, initMethod, updateMethod);
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

const wchar_t** EnumUIAdapters(ssize *count)
{
	wstring folder((wchar_t*)(void*)Marshal::StringToHGlobalAuto(UIRootPath));
	
	set<wstring> options;
	SurvivorLibrary::SearchHandleFiles((folder + L"\\").c_str(), (folder + L"\\*Impl.dll").c_str(), [&](wchar_t name[])
	{
		wstring file(name);
		options.emplace(move(file.substr(0, file.length() - 8))); // remove `Impl.dll`
	});

	wchar_t ** opt = new wchar_t*[options.size()];
	wchar_t ** popt = opt;
	for each (auto str in options)
	{
		size_t len = str.length() + 1;
		*popt = new wchar_t[len];
		rsize_t memsize = static_cast<rsize_t>(sizeof(wchar_t) * len);
		memcpy_s(*popt, memsize, str.c_str(), memsize);
		popt++;
	}

	*count = options.size();
	return const_cast<const wchar_t**>(opt);
}
