#include <memory>
#include "SCCollection.h"

#define CHUNK_SIZE (62) /* (62+1+1)*8 */

struct DataChunk
{
	mixed_t data[CHUNK_SIZE];
	DataChunk *prv, *nxt;
	scsize count;
};

SCCollection::SCCollection() :size(0)
{
	head = new DataChunk;
	if (head == nullptr)
		throw 0;
	memset(head, 0, sizeof(DataChunk));
}

SCCollection::~SCCollection()
{
	Clear();
}

out_t *SCCollection::operator[](scsize i)const
{
	DataChunk *pdata = head;
	while (pdata && i >= pdata->count)
	{
		i -= pdata->count;
		pdata = pdata->nxt;
	}
	if (pdata)
	{
		// Found
		return &pdata->data[i];
	}
	// Error
	out_t err;
	err.i8 = 0ull;
	return nullptr;
}

scsize SCCollection::Add(in_t e)
{
	DataChunk *pdata = head;
	DataChunk *plast = nullptr;
	while (pdata)
	{
		if (pdata->count < CHUNK_SIZE)
		{
			// Found empty place
			pdata->data[pdata->count++] = e;
			return size++;
		}
		plast = pdata;
		pdata = pdata->nxt;
	}
	// Full
	DataChunk *newChunk = new DataChunk;
	newChunk->count = 0;
	newChunk->prv = plast;
	newChunk->nxt = 0;
	newChunk->data[newChunk->count++] = e;
	plast->nxt = newChunk;
	return size++;
}

bool SCCollection::Remove(in_t e)
{
	return Remove(Find(e));
}

bool SCCollection::Remove(scsize i)
{
	if (i == nop)
		return false;
	DataChunk *pdata = head;
	while (pdata && i >= pdata->count)
	{
		i -= pdata->count;
		pdata = pdata->nxt;
	}
	if (pdata)
	{
		// Found
		memmove(&pdata->data[i], &pdata->data[i + 1], (size_t)(CHUNK_SIZE - i - 1));
		pdata->count--;
		size--;
		return true;
	}
	return false;
}

scsize SCCollection::Size()const
{
	return size;
}

void SCCollection::Clear()
{
	DataChunk *pdata = head;
	while (pdata)
	{
		DataChunk *pd = pdata;
		pdata = pdata->nxt;
		delete pd;
	}
	size = 0;
	head = new DataChunk;
	memset(head, 0, sizeof(DataChunk));
}

scsize SCCollection::Find(in_t e)const
{
	DataChunk *pdata = head;
	scsize p = 0;
	while (pdata)
	{
		for (scsize i = 0; i < pdata->count; i++)
			if (pdata->data[i].i8 == e.i8)
				return p + i;
		p += pdata->count;
		pdata = pdata->nxt;
	}
	return nop;
}
