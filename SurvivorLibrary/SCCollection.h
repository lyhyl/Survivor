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
	template<typename T>
	friend class SCCollectionXIterator;
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

template<typename T>
class SCCollectionXIterator
{
	struct DataChunk *chunk;
	scsize offset;
public:
	SCCollectionXIterator(SCCollection &c, scsize index)
	{
		DataChunk *dc = c.head;
		while (dc->count < index)
			if (dc->nxt)
			{
				index -= dc->count;
				dc = dc->nxt;
			}
			else
			{
				dc = 0;
				index = SCCollection::nop;
				break;
			}
		chunk = dc;
		offset = index;
	}
	inline bool operator==(SCCollectionXIterator<T> &i){ return chunk == i.chunk&&offset == i.offset; }
	inline bool operator!=(SCCollectionXIterator<T> &i){ return !this->operator==(i); }
	inline T operator*(){ return (T)chunk->data[offset]; }
	inline SCCollectionXIterator<T> operator++()
	{
		if (++offset >= chunk->count)
		{
			if (chunk->nxt)
			{
				chunk = chunk->nxt;
				offset = 0;
			}
			else
			{
				chunk = 0;
				offset = SCCollection::nop;
			}
		}
		return *this;
	}
	inline SCCollectionXIterator<T> operator--()
	{
		if (offset == 0)
		{
			if (chunk->prv)
			{
				chunk = chunk->prv;
				offset = chunk->count - 1;
			}
		}
		else
			--offset;
		return *this;
	}
	inline SCCollectionXIterator<T> operator++(int)
	{
		SCCollectionXIterator<T> i = *this;
		if (++offset >= chunk->count)
		{
			if (chunk->nxt)
			{
				chunk = chunk->nxt;
				offset = 0;
			}
			else
			{
				chunk = 0;
				offset = SCCollection::nop;
			}
		}
		return i;
	}
	inline SCCollectionXIterator<T> operator--(int)
	{
		SCCollectionXIterator<T> i = *this;
		if (offset == 0)
		{
			if (chunk->prv)
			{
				chunk = chunk->prv;
				offset = chunk->count - 1;
			}
		}
		else
			--offset;
		return i;
	}
};

template<typename T>
class SCCollectionX
{
private:
	SCCollection collection;
public:
	operator SCCollection() { return collection; }
	SCCollectionX() = default;
	~SCCollectionX() = default;
	T *operator[](scsize i) const { return collection[i]; }
	scsize Add(T e) { return collection.Add((in_t)e); }
	bool Remove(T e) { return collection.Remove((in_t)e); }
	bool Remove(scsize i) { return collection.Remove(i); }
	scsize Size() const { return collection.Size(); }
	void Clear() { collection.Clear(); }
	scsize Find(T e) const { return collection.Find((in_t)e); }
};

template<typename T>
class SCCollectionX<T*>
{
private:
	SCCollection collection;
public:
	operator SCCollection() { return collection; }
	SCCollectionX() = default;
	~SCCollectionX() = default;
	T *operator[](scsize i) const { return (T*)collection[i]->ptr; }
	scsize Add(T *e) { return collection.Add({ e }); }
	bool Remove(T *e) { return collection.Remove({ e }); }
	bool Remove(scsize i){ return collection.Remove(i); }
	scsize Size() const { return collection.Size(); }
	void Clear() { collection.Clear(); }
	scsize Find(T *e) const	{ return collection.Find({ e }); }

	SCCollectionXIterator<T*> begin()
	{
		return SCCollectionXIterator<T>(collection, 0);
	}
	SCCollectionXIterator<T*> end()
	{
		return SCCollectionXIterator<T>(collection, -1);
	}
};