using System;
using System.Drawing;
using System.Runtime.InteropServices;

namespace CSharpUI2DImpl.Core
{
    [StructLayout(LayoutKind.Sequential)]
    public struct _SCPoint
    {
        public double x, y;
    }

    [StructLayout(LayoutKind.Sequential)]
    struct _SCTriangle
    {
        _SCPoint a, b, c;
    }

    [StructLayout(LayoutKind.Sequential)]
    struct _SCRegion
    {
        UInt64 triangleCount;
        IntPtr triangles;
        UInt64 vertexCount;
        IntPtr verties;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct _SCHero
    {
       public Int64 id, hp, energy;
       public _SCPoint position;
       public _SCPoint direction;
    }

    public class SCHero
    {
        private _SCHero hero;

        public PointF Position
        {
            get { return new PointF((float)hero.position.x, (float)hero.position.y); }
        }

        public SCHero(IntPtr phero)
        {
            hero = (_SCHero)Marshal.PtrToStructure(phero, typeof(_SCHero));
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct _UIDisplayData
    {
        public IntPtr map;
        public IntPtr competitors;
    }

    public class UIDisplayData
    {
        private _UIDisplayData data;
        private SCMap map;
        private SCCollection<SCHero> heroes;

        public UIDisplayData(IntPtr d)
        {
            if (d != IntPtr.Zero)
            {
                data = (_UIDisplayData)Marshal.PtrToStructure(d, typeof(_UIDisplayData));
                map = new SCMap(data.map);
                heroes = new SCCollection<SCHero>(new SCCollection(data.competitors));
            }
        }

        public SCMap Map { get { return map; } }
        public SCCollection<SCHero> Heroes { get { return heroes; } }
    }

    [StructLayout(LayoutKind.Sequential)]
    struct _AIThinkData
    {
        IntPtr target;
        IntPtr vision;
        IntPtr hearing;
        IntPtr touch;
    }
}
