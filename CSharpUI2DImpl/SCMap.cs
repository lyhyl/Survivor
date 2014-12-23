using CSharpUI2DImpl.Core;
using System;
using System.Drawing;
using System.Runtime.InteropServices;

namespace CSharpUI2DImpl.Core
{
    public class SCMap
    {
        private _SCMap map;
        private SCCollection<_SCMapResource> resources;

        public Size Size { get { return new Size((int)map.width, (int)map.height); } }
        public UInt64 Width { get { return map.width; } }
        public UInt64 Height { get { return map.height; } }

        public SCMap(IntPtr pmap)
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
        public static Int64 DefaultSize = 5120;
        public UInt32 seed;
        public UInt64 width, height;
        public IntPtr resources;
    }

    [StructLayout(LayoutKind.Sequential)]
    struct _SCMapResource
    {
        int type;
        _SCRegion region;
    }
}
