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
static HMODULE *_CImpl;

void InitializeCSharp2D()
{
	String ^csClassName = "CSharpUI2DImpl.UIAdapter";
	String ^assPath = Path::Combine(Path::GetDirectoryName(Assembly::GetExecutingAssembly()->Location), "CSharpUI2DImpl.dll");
	CSharpAssembly = Assembly::LoadFile(assPath);
	_CSharpImpl = new CSharpImplWapper(CSharpAssembly->CreateInstance(csClassName),
		CSharpAssembly->GetType(csClassName)->GetMethod("Display"));
}

void *CreateUIAdapter(const wchar_t* option)
{
	if (wcscmp(option, __TEXT("C#2D")) == 0)
	{
		if (!Initialized)
			InitializeCSharp2D();
		return _CSharpImpl;
	}

	return 0;
}