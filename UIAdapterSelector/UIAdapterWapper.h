#pragma once
#pragma managed
#include "UIAdapter.h"

using namespace System;
using namespace System::Reflection;

class CSharpImplWapper :UIAdapter
{
private:
	gcroot<Object ^> object;
	gcroot<MethodInfo^> displayFunction;
public:
	CSharpImplWapper(Object ^obj, MethodInfo ^displayfunc)
	{
		object = obj;
		displayFunction = displayfunc;
	}
	virtual int Display(const UIDisplayData* data)
	{
		auto arg = gcnew array < Object^ > { IntPtr(const_cast<UIDisplayData*>(data)) };
		return Convert::ToInt32(displayFunction->Invoke(object, arg));
	}
};