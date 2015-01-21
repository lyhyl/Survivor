#pragma once

#include <vector>
#include <cliext\vector>

using namespace cliext;
using namespace System;
using namespace System::Collections::Generic;

namespace CLRSurvivorLibrary
{
	ref class CSMarshal
	{
	internal:
		template <typename T, typename U>
		static cliext::vector<T^>^ Vector(const std::vector<U> &v)
		{
			cliext::vector<T^>^ cv = gcnew vector<T^>();
			cv->reserve(v.size());
			for each (U &u in v)
				cv->push_back(gcnew T(u));
			return cv;
		}
		template <typename T, typename U>
		static cliext::vector<T^>^ Vector(const std::vector<U*> &v)
		{
			cliext::vector<T^>^ cv = gcnew vector<T^>();
			cv->reserve(static_cast<cliext::vector<T^>::size_type>(v.size()));
			for each (U *u in v)
				cv->push_back(gcnew T(u));
			return cv;
		}
	};
}
