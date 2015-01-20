#include "SObject.h"

namespace SurvivorLibrary
{
	ssize SObject::UniversalID = 0;
	SObject::SObject()
	{
		id = UniversalID++;
	}
	void SObject::ResetIDCounter()
	{
		UniversalID = 0;
	}
	ssize SObject::ID()
	{
		return id;
	}
}