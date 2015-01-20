#include "stdafx.h"

#include "AIAdapterSelector.h"
#include "Wrapper.h"

using namespace std;

void *CreateAIAdapter(const wchar_t* file)
{
	wstring fileName(file);
	size_t index = fileName.find_last_of(L'.');
	wstring ext = move(index == wstring::npos ? wstring(L"") : fileName.substr(index));
	if (ext == L".class")
		return new JavaImplWrapper(fileName);
	else if (ext == L".dll")
		return new CImplWrapper(fileName);
	else if (ext == L".ss")
	{

	}
	// Unsupport
	return nullptr;
}