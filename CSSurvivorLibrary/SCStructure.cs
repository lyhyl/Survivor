using System;
using System.Drawing;
using System.Runtime.InteropServices;

namespace CSSurvivorLibrary
{
    public abstract class SurvivorStructure
    {
        protected IntPtr unmanagedPointer;
        public SurvivorStructure(IntPtr p) { unmanagedPointer = p; }
        public bool IsEqual(IntPtr p) { return unmanagedPointer == p; }
        public bool IsEqual(SurvivorStructure p) { return unmanagedPointer == p.unmanagedPointer; }
        public override bool Equals(object obj)
        {
            if (obj == null)
                return unmanagedPointer == null;
            SurvivorStructure ss = obj as SurvivorStructure;
            if (ss == null)
                return false;
            return unmanagedPointer == ss.unmanagedPointer;
        }
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }

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
        ulong triangleCount;
        IntPtr triangles;
        ulong vertexCount;
        IntPtr verties;
    }

    public enum SCHeroActionType : int
    {
        Stay,
        Move,
        Run,
        Turn,
        Attack,
        Climb
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct _SCHero
    {
        public ulong id, hp, energy;
        public _SCPoint position;
        public _SCPoint direction;
        public SCHeroActionType state;

        public ulong prvTime;

        private IntPtr aiPtr, threadPtr;
    }

    public class SCHero : SurvivorStructure
    {
        private _SCHero hero;

        public PointF Position
        {
            get { return new PointF((float)hero.position.x, (float)hero.position.y); }
        }

        public SCHero(IntPtr phero)
            : base(phero)
        {
            hero = (_SCHero)Marshal.PtrToStructure(phero, typeof(_SCHero));
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct _UIDisplayData
    {
        public IntPtr map;
        public IntPtr heroes;
    }

    public class UIDisplayData : SurvivorStructure
    {
        private _UIDisplayData data;
        private SCMap map;
        private SCSSCollection<_SCHero> heroes;

        public UIDisplayData(IntPtr pdata)
            : base(pdata)
        {
            if (pdata != IntPtr.Zero)
            {
                data = (_UIDisplayData)Marshal.PtrToStructure(pdata, typeof(_UIDisplayData));
                map = new SCMap(data.map);
                heroes = new SCSSCollection<_SCHero>(new SCCollection(data.heroes));
            }
        }

        public SCMap Map { get { return map; } }
        public SCSSCollection<_SCHero> Heroes { get { return heroes; } }
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
