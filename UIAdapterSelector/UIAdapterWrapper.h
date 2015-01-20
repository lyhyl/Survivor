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
	gcroot<MethodInfo^> initializeFunction;
	gcroot<MethodInfo^> updateFunction;
public:
	CSharpImplWrapper(Object ^obj, MethodInfo ^initfunc, MethodInfo ^updfunc)
	{
		object = obj;
		initializeFunction = initfunc;
		updateFunction = updfunc;
	}
	virtual __int32 Initialize(SInitializeData* pmap)
	{
		auto arg = gcnew array < Object^ > { IntPtr(pmap) };
		return Convert::ToInt32(initializeFunction->Invoke(object, arg));
	}
	virtual __int32 Update(SUpdateData* pupdd)
	{
		auto arg = gcnew array < Object^ > { IntPtr(pupdd) };
		return Convert::ToInt32(updateFunction->Invoke(object, arg));
	}
};