// This is the main DLL file.

#include "stdafx.h"
#include <string>
#include "UIAdapterSelector.h"
#include "UIAdapterWapper.h"

using namespace System;
using namespace System::IO;
using namespace System::Reflection;

static bool Initialized = false;
static gcroot<Assembly^> CSharpAssembly;

static CSharpImplWapper *_CSharpImpl;
static CSharpImplWapper *_XNAImpl;
static HMODULE *_CImpl;

void InitializeCSharp2D()
{
	String ^csClassName = "CSharpUI2DImpl.UIAdapter";
	String ^assPath = Path::Combine(Path::GetDirectoryName(Assembly::GetExecutingAssembly()->Location), "CSharpUI2DImpl.dll");
	CSharpAssembly = Assembly::LoadFile(assPath);
	_CSharpImpl = new CSharpImplWapper(CSharpAssembly->CreateInstance(csClassName),
		CSharpAssembly->GetType(csClassName)->GetMethod("Display"));
	Initialized = true;
}

void InitializeXNA3D()
{
	String ^csClassName = "XNAUI3DImpl.UIAdapter";
	String ^assPath = Path::Combine(Path::GetDirectoryName(Assembly::GetExecutingAssembly()->Location), "XNAUI3DImpl.dll");
	CSharpAssembly = Assembly::LoadFile(assPath);
	_XNAImpl = new CSharpImplWapper(CSharpAssembly->CreateInstance(csClassName),
		CSharpAssembly->GetType(csClassName)->GetMethod("Display"));
	Initialized = true;
}

void *CreateUIAdapter(const wchar_t* option)
{
	if (wcscmp(option, L"C#2D") == 0)
	{
		if (!Initialized)
			InitializeCSharp2D();
		return _CSharpImpl;
	}
	else if (wcscmp(option, L"C#3D") == 0)
	{
		if (!Initialized)
			InitializeXNA3D();
		return _XNAImpl;
	}

	return 0;
}