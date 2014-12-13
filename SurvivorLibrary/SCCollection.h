#pragma once
#include "SCDefines.h"
typedef union API mixed_t
{
	/**/
	void *ptr;
	/**/
	__int8 i1;
	__int16 i2;
	__int32 i4;
	__int64 i8;
	/**/
	float f2;
	double f4;
} out_t, in_t;
class API SCCollection
{
private:
	static const scsize nop = (scsize)(-1);
	struct DataChunk *head;
	mixed_t *next;
	scsize size;
public:
	SCCollection();
	~SCCollection();
	out_t *operator[](scsize i)const;
	scsize Add(in_t e);
	bool Remove(in_t e);
	bool Remove(scsize i);
	scsize Size()const;
	void Clear();
	scsize Find(in_t e)const;
};

