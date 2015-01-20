using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XNAUI3DImpl
{
    class GridStorage<T>
    {
        List<T>[,] storage;
        ulong CellSize, WorldSize;

        int gridCount;
        double xOffset, yOffset;

        public int GridCount
        {
            get { return gridCount; }
        }

        public GridStorage(ulong cellSize, ulong worldSize, double xoffset, double yoffset)
        {
            CellSize = cellSize;
            WorldSize = worldSize;
            xOffset = xoffset;
            yOffset = yoffset;

            gridCount = (int)(worldSize / cellSize) + 1;
            storage = new List<T>[gridCount, gridCount];
            for (int i = 0; i < gridCount; i++)
                for (int j = 0; j < gridCount; j++)
                    storage[i, j] = new List<T>();
        }

        public int CellPositionX(double v)
        {
            return (int)((Math.Min(Math.Max(v - xOffset, 0), WorldSize)) / CellSize);
        }
        public int CellPositionY(double v)
        {
            return (int)((Math.Min(Math.Max(v - yOffset, 0), WorldSize)) / CellSize);
        }

        public List<T> this[int x, int y]
        {
            get { return storage[x, y]; }
        }

        public List<T> this[double x, double y]
        {
            get { return storage[CellPositionX(x), CellPositionY(y)]; }
        }

        public void Add(T obj, double x, double y)
        {
            storage[CellPositionX(x), CellPositionY(y)].Add(obj);
        }

        public void Remove(T obj, double x, double y)
        {
            storage[CellPositionX(x), CellPositionY(y)].Remove(obj);
        }

        public void Replace(T ol, double x, double y, T ne, double nx, double ny)
        {
            Remove(ol, x, y);
            Add(ne, nx, ny);
        }

        public void Clear()
        {
            for (int i = 0; i < gridCount; i++)
                for (int j = 0; j < gridCount; j++)
                    storage[i, j].Clear();
        }
    }
}
