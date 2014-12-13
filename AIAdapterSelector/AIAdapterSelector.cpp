#include "stdafx.h"

#include "AIAdapterSelector.h"
#include "Wapper.h"

void *CreateAIAdapter(const wchar_t* file)
{
	wstring fileName(file);
	wstring ext = fileName.substr(fileName.find_last_of(L'.'));
	if (ext == __TEXT(".class"))
		return new JavaImplWapper(fileName);
	else if (ext == __TEXT(".dll"))
		return new CImplWapper(fileName);
	else if (ext == __TEXT(".ss"))
	{

	}
	// Unsupport
	return 0;
}