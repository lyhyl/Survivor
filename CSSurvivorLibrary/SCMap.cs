using System;
using System.Drawing;
using System.Runtime.InteropServices;

namespace CSSurvivorLibrary
{
    public class SCMap : SurvivorStructure
    {
        private _SCMap map;
        private SCSSCollection<_SCMapResource> resources;

        public Size Size { get { return new Size((int)map.width, (int)map.height); } }
        public UInt64 Width { get { return map.width; } }
        public UInt64 Height { get { return map.height; } }

        public SCMap(IntPtr pmap)
            : base(pmap)
        {
            if (pmap != IntPtr.Zero)
            {
                map = (_SCMap)Marshal.PtrToStructure(pmap, typeof(_SCMap));
            }
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct _SCMap
    {
        public static ulong DefaultSize = 5120;
        public uint seed;
        public ulong width, height;
        public IntPtr resources;
    }

    [StructLayout(LayoutKind.Sequential)]
    struct _SCMapResource
    {
        int type;
        _SCRegion region;
    }

    class SCMapResource : SurvivorStructure
    {
        public SCMapResource(IntPtr pmr)
            : base(pmr)
        {

        }
    }
}
