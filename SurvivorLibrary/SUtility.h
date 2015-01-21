#pragma once
#include <Windows.h>
#include "SDefines.h"

namespace SurvivorLibrary
{
	template < typename T >
	void SearchHandleFiles(const wchar_t* folder, const wchar_t* option, T phandler)
	{
		if (CreateDirectory(folder, NULL) || ERROR_ALREADY_EXISTS == GetLastError())
		{
			WIN32_FIND_DATA fd;
			HANDLE hFind = FindFirstFile(option, &fd);
			if (hFind != INVALID_HANDLE_VALUE)
			{
				do
					if (!(fd.dwFileAttributes & FILE_ATTRIBUTE_DIRECTORY))
						phandler(fd.cFileName);
				while (FindNextFile(hFind, &fd));
				FindClose(hFind);
			}
		}
	}
}
