#pragma once
#pragma managed
#include "SUIAdapter.h"

using namespace System;
using namespace System::Reflection;

using namespace SurvivorLibrary;

class CSharpImplWrapper :SUIAdapter
{
private:
	gcroot<Object ^> object;
	gcroot<MethodInfo^> displayFunction;
public:
	CSharpImplWrapper(Object ^obj, MethodInfo ^displayfunc)
	{
		object = obj;
		displayFunction = displayfunc;
	}
	virtual int Display(SUIDisplayData* data)
	{
		auto arg = gcnew array < Object^ > { IntPtr(data) };
		return Convert::ToInt32(displayFunction->Invoke(object, arg));
	}
};