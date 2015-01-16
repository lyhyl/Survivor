using System;
using System.Runtime.InteropServices;

namespace CSSurvivorLibrary
{
    public unsafe class Memory
    {
        // Handle for the process heap. This handle is used in all calls to the
        // HeapXXX APIs in the methods below.
        static int ph = GetProcessHeap();
        // Private instance constructor to prevent instantiation.
        private Memory() { }
        // Allocates a memory block of the given size. The allocated memory is
        // automatically initialized to zero.
        public static IntPtr Alloc(int size)
        {
            IntPtr result = HeapAlloc(ph, HEAP_ZERO_MEMORY, size);
            if (result == null) throw new OutOfMemoryException();
            return result;
        }
        public static IntPtr Alloc<T>()
        {
            T t = default(T);
            IntPtr result = HeapAlloc(ph, HEAP_ZERO_MEMORY, Marshal.SizeOf(t));
            if (result == null) throw new OutOfMemoryException();
            return result;
        }
        // Copies count bytes from src to dst. The source and destination
        // blocks are permitted to overlap.
        public static void Copy(IntPtr src, IntPtr dst, int count)
        {
            byte* ps = (byte*)src;
            byte* pd = (byte*)dst;
            if (ps > pd)
            {
                for (; count != 0; count--) *pd++ = *ps++;
            }
            else if (ps < pd)
            {
                for (ps += count, pd += count; count != 0; count--) *--pd = *--ps;
            }
        }
        // Frees a memory block.
        public static void Free(IntPtr block)
        {
            if (!HeapFree(ph, 0, block)) throw new InvalidOperationException();
        }
        // Re-allocates a memory block. If the reallocation request is for a
        // larger size, the additional region of memory is automatically
        // initialized to zero.
        public static IntPtr ReAlloc(IntPtr block, int size)
        {
            IntPtr result = HeapReAlloc(ph, HEAP_ZERO_MEMORY, block, size);
            if (result == null) throw new OutOfMemoryException();
            return result;
        }
        // Returns the size of a memory block.
        public static int SizeOf(IntPtr block)
        {
            int result = HeapSize(ph, 0, block);
            if (result == -1) throw new InvalidOperationException();
            return result;
        }
        // Heap API flags
        const int HEAP_ZERO_MEMORY = 0x00000008;
        // Heap API functions
        [DllImport("kernel32")]
        static extern int GetProcessHeap();
        [DllImport("kernel32")]
        static extern IntPtr HeapAlloc(int hHeap, int flags, int size);
        [DllImport("kernel32")]
        static extern bool HeapFree(int hHeap, int flags, IntPtr block);
        [DllImport("kernel32")]
        static extern IntPtr HeapReAlloc(int hHeap, int flags, IntPtr block, int size);
        [DllImport("kernel32")]
        static extern int HeapSize(int hHeap, int flags, IntPtr block);
    }
}