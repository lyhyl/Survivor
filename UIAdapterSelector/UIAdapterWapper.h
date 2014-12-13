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
	virtual int Display(SCMap &map, SCCollection &competitors)
	{
		array<Object^> ^args = gcnew array < Object^ > { IntPtr(&map), IntPtr(&competitors) };
		Object ^ret = displayFunction->Invoke(object, args);
		return Convert::ToInt32(ret);
	}
};