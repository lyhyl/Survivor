using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace CSharpUI2DImpl.Core
{
    public class SCSSCollection<T> : IEnumerable<T> where T : struct
    {
        private SCCollection collection = null;

        public SCSSCollection() { collection = new SCCollection(); }
        public SCSSCollection(SCCollection c) { collection = c; }

        public static implicit operator SCCollection(SCSSCollection<T> c)
        { return c.collection; }
        public T this[ulong index]
        {
            get
            {
                return (T)Marshal.PtrToStructure(((_SCmixed_t)collection[index]).ptr, typeof(T));
            }
        }
        public ulong Size
        { get { return collection.Size; } }
        public ulong Find(T e)
        { return collection.Find(SCmixed_tConverter.Form(e)); }

        public class Enumerator : IEnumerator<T>
        {
            private SCSSCollection<T> collection;
            private long index;

            public Enumerator(SCSSCollection<T> c)
            {
                collection = c;
                index = -1;
            }
            public Enumerator(SCSSCollection<T> c, long i)
            {
                collection = c;
                index = i;
            }

            public T Current
            {
                get { return collection[(ulong)index]; }
            }

            public void Dispose()
            {
            }

            object System.Collections.IEnumerator.Current
            {
                get { return collection[(ulong)index]; }
            }

            public bool MoveNext()
            {
                return (ulong)(++index) < collection.Size;
            }

            public void Reset()
            {
                index = -1;
            }
        }

        public IEnumerator<T> GetEnumerator()
        {
            return new SCSSCollection<T>.Enumerator(this);
        }
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return new SCSSCollection<T>.Enumerator(this);
        }
    }

    public class SCCollection<T> : IEnumerable<T>
    {
        private SCCollection collection = null;

        public SCCollection() { collection = new SCCollection(); }
        public SCCollection(SCCollection c) { collection = c; }

        public static implicit operator SCCollection(SCCollection<T> c)
        { return c.collection; }

        public T this[ulong index]
        {
            get
            {
                return SCmixed_tConverter.To<T>((_SCmixed_t)collection[index]);
            }
        }
        public ulong Size
        { get { return collection.Size; } }
        public ulong Find(T e)
        { return collection.Find(SCmixed_tConverter.Form(e)); }

        public class Enumerator : IEnumerator<T>
        {
            private SCCollection<T> collection;
            private long index;

            public Enumerator(SCCollection<T> c)
            {
                collection = c;
                index = -1;
            }
            public Enumerator(SCCollection<T> c, long i)
            {
                collection = c;
                index = i;
            }

            public T Current
            {
                get { return collection[(ulong)index]; }
            }

            public void Dispose()
            {
            }

            object System.Collections.IEnumerator.Current
            {
                get { return collection[(ulong)index]; }
            }

            public bool MoveNext()
            {
                return (ulong)(++index) < collection.Size;
            }

            public void Reset()
            {
                index = -1;
            }
        }

        public IEnumerator<T> GetEnumerator()
        {
            return new SCCollection<T>.Enumerator(this);
        }
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return new SCCollection<T>.Enumerator(this);
        }
    }

    public class SCCollection : SurvivorStructure
    {
        [DllImport("SurvivorLibrary.dll", EntryPoint = "??0SCCollection@@QAE@XZ", CallingConvention = CallingConvention.ThisCall)]
        private static extern IntPtr Constructor(IntPtr c);

        [DllImport("SurvivorLibrary.dll", EntryPoint = "??1SCCollection@@QAE@XZ", CallingConvention = CallingConvention.ThisCall)]
        private static extern IntPtr Destructor(IntPtr c);

        [DllImport("SurvivorLibrary.dll", EntryPoint = "?Add@SCCollection@@QAE_KTmixed_t@@@Z", CallingConvention = CallingConvention.ThisCall)]
        private static extern UInt64 CollectionAdd(IntPtr c, _SCmixed_t cell);

        [DllImport("SurvivorLibrary.dll", EntryPoint = "?Clear@SCCollection@@QAEXXZ", CallingConvention = CallingConvention.ThisCall)]
        private static extern void CollectionClear(IntPtr c);

        [DllImport("SurvivorLibrary.dll", EntryPoint = "?Find@SCCollection@@QBE_KTmixed_t@@@Z", CallingConvention = CallingConvention.ThisCall)]
        private static extern UInt64 CollectionFind(IntPtr c, _SCmixed_t e);

        [DllImport("SurvivorLibrary.dll", EntryPoint = "?Remove@SCCollection@@QAE_N_K@Z", CallingConvention = CallingConvention.ThisCall)]
        private static extern bool CollectionRemoveAt(IntPtr c, UInt64 i);

        [DllImport("SurvivorLibrary.dll", EntryPoint = "?Remove@SCCollection@@QAE_NTmixed_t@@@Z", CallingConvention = CallingConvention.ThisCall)]
        private static extern bool CollectionRemove(IntPtr c, _SCmixed_t e);

        [DllImport("SurvivorLibrary.dll", EntryPoint = "?Size@SCCollection@@QBE_KXZ", CallingConvention = CallingConvention.ThisCall)]
        private static extern UInt64 CollectionSize(IntPtr c);

        //[return: MarshalAs(UnmanagedType.LPStruct)]
        [DllImport("SurvivorLibrary.dll", EntryPoint = "??ASCCollection@@QBEPATmixed_t@@_K@Z", CallingConvention = CallingConvention.ThisCall)]
        private static extern IntPtr CollectionIndexer(IntPtr c, UInt64 i);

        private bool isMarshal = false;

        public SCCollection()
            : base(Memory.Alloc<_SCCollection>())
        {
            Constructor(unmanagedPointer);
        }

        public SCCollection(IntPtr pcollection)
            : base(pcollection)
        {
            isMarshal = true;
        }

        ~SCCollection()
        {
            if (!isMarshal)
            {
                Destructor(unmanagedPointer);
                Memory.Free(unmanagedPointer);
            }
            unmanagedPointer = IntPtr.Zero;
        }

        public object this[ulong index]
        {
            get
            {
                return Marshal.PtrToStructure(CollectionIndexer(unmanagedPointer, index), typeof(_SCmixed_t));
            }
        }

        public ulong Add(_SCmixed_t e)
        { return CollectionAdd(unmanagedPointer, e); }
        #region Add
        public ulong Add(IntPtr e)
        { return CollectionAdd(unmanagedPointer, SCmixed_tConverter.Form(e)); }
        public ulong Add(Byte e)
        { return CollectionAdd(unmanagedPointer, SCmixed_tConverter.Form(e)); }
        public ulong Add(Int16 e)
        { return CollectionAdd(unmanagedPointer, SCmixed_tConverter.Form(e)); }
        public ulong Add(Int32 e)
        { return CollectionAdd(unmanagedPointer, SCmixed_tConverter.Form(e)); }
        public ulong Add(Int64 e)
        { return CollectionAdd(unmanagedPointer, SCmixed_tConverter.Form(e)); }
        public ulong Add(Single e)
        { return CollectionAdd(unmanagedPointer, SCmixed_tConverter.Form(e)); }
        public ulong Add(Double e)
        { return CollectionAdd(unmanagedPointer, SCmixed_tConverter.Form(e)); }
        #endregion

        public bool Remove(_SCmixed_t e)
        { return CollectionRemove(unmanagedPointer, e); }
        #region Remove Element
        public bool Remove(IntPtr e)
        { return CollectionRemove(unmanagedPointer, SCmixed_tConverter.Form(e)); }
        public bool Remove(Byte e)
        { return CollectionRemove(unmanagedPointer, SCmixed_tConverter.Form(e)); }
        public bool Remove(Int16 e)
        { return CollectionRemove(unmanagedPointer, SCmixed_tConverter.Form(e)); }
        public bool Remove(Int32 e)
        { return CollectionRemove(unmanagedPointer, SCmixed_tConverter.Form(e)); }
        public bool Remove(Int64 e)
        { return CollectionRemove(unmanagedPointer, SCmixed_tConverter.Form(e)); }
        public bool Remove(Single e)
        { return CollectionRemove(unmanagedPointer, SCmixed_tConverter.Form(e)); }
        public bool Remove(Double e)
        { return CollectionRemove(unmanagedPointer, SCmixed_tConverter.Form(e)); }
        #endregion

        public bool RemoveAt(ulong i)
        {
            return CollectionRemoveAt(unmanagedPointer, (UInt32)i);
        }

        public ulong Size
        {
            get
            {
                return CollectionSize(unmanagedPointer);
            }
        }

        public void Clear()
        {
            CollectionClear(unmanagedPointer);
        }

        public ulong Find(_SCmixed_t e)
        { return CollectionFind(unmanagedPointer, e); }
        #region Find
        public ulong Find(IntPtr e)
        { return CollectionFind(unmanagedPointer, SCmixed_tConverter.Form(e)); }
        public ulong Find(Byte e)
        { return CollectionFind(unmanagedPointer, SCmixed_tConverter.Form(e)); }
        public ulong Find(Int16 e)
        { return CollectionFind(unmanagedPointer, SCmixed_tConverter.Form(e)); }
        public ulong Find(Int32 e)
        { return CollectionFind(unmanagedPointer, SCmixed_tConverter.Form(e)); }
        public ulong Find(Int64 e)
        { return CollectionFind(unmanagedPointer, SCmixed_tConverter.Form(e)); }
        public ulong Find(Single e)
        { return CollectionFind(unmanagedPointer, SCmixed_tConverter.Form(e)); }
        public ulong Find(Double e)
        { return CollectionFind(unmanagedPointer, SCmixed_tConverter.Form(e)); }
        #endregion
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct _SCCollection
    {
        public static UInt64 nop = UInt64.MaxValue;
        public IntPtr head;
        public IntPtr next;
        public UInt64 size;
    }

    [StructLayout(LayoutKind.Explicit)]
    public struct _SCmixed_t
    {
        /**/
        [FieldOffset(0)]
        public Byte i1;
        [FieldOffset(0)]
        public Int16 i2;
        [FieldOffset(0)]
        public Int32 i4;
        [FieldOffset(0)]
        public Int64 i8;
        /**/
        [FieldOffset(0)]
        public IntPtr ptr;
        /**/
        [FieldOffset(0)]
        public Single f2;
        [FieldOffset(0)]
        public Double f4;
    }

    internal static class SCmixed_tConverter
    {
        /*public static SCmixed_t Form(IntPtr e) { SCmixed_t t = new SCmixed_t(); t.ptr = e; return t; }
        public static SCmixed_t Form(Byte e) { SCmixed_t t = new SCmixed_t(); t.i1 = e; return t; }
        public static SCmixed_t Form(Int16 e) { SCmixed_t t = new SCmixed_t(); t.i2 = e; return t; }
        public static SCmixed_t Form(Int32 e) { SCmixed_t t = new SCmixed_t(); t.i4 = e; return t; }
        public static SCmixed_t Form(Int64 e) { SCmixed_t t = new SCmixed_t(); t.i8 = e; return t; }
        public static SCmixed_t Form(Single e) { SCmixed_t t = new SCmixed_t(); t.f2 = e; return t; }
        public static SCmixed_t Form(Double e) { SCmixed_t t = new SCmixed_t(); t.f4 = e; return t; }*/
        public static _SCmixed_t Form<T>(T e)
        {
            _SCmixed_t v = new _SCmixed_t();
            Type t = typeof(T);
            if (t == typeof(IntPtr))
                v.ptr = (IntPtr)(object)e;
            else if (t == typeof(Int64))
                v.i8 = (Int64)(object)e;
            else if (t == typeof(Int32))
                v.i4 = (Int32)(object)e;
            else if (t == typeof(Int16))
                v.i2 = (Int16)(object)e;
            else if (t == typeof(Byte))
                v.i1 = (Byte)(object)e;
            else if (t == typeof(Double))
                v.f4 = (Double)(object)e;
            else if (t == typeof(Single))
                v.f2 = (Single)(object)e;
            else
                throw new ArgumentException("Unable convert form " + t.Name);
            return v;
        }

        public static IntPtr ToPtr(_SCmixed_t e) { return e.ptr; }
        public static Byte ToI1(_SCmixed_t e) { return e.i1; }
        public static Int16 ToI2(_SCmixed_t e) { return e.i2; }
        public static Int32 ToI4(_SCmixed_t e) { return e.i4; }
        public static Int64 ToI8(_SCmixed_t e) { return e.i8; }
        public static Single ToF2(_SCmixed_t e) { return e.f2; }
        public static Double ToF4(_SCmixed_t e) { return e.f4; }

        public static T To<T>(_SCmixed_t e)
        {
            Type t = typeof(T);
            if (t == typeof(IntPtr))
                return (T)(object)ToPtr(e);
            else if (t == typeof(Int64))
                return (T)(object)ToI8(e);
            else if (t == typeof(Int32))
                return (T)(object)ToI4(e);
            else if (t == typeof(Int16))
                return (T)(object)ToI2(e);
            else if (t == typeof(Byte))
                return (T)(object)ToI1(e);
            else if (t == typeof(Double))
                return (T)(object)ToF4(e);
            else if (t == typeof(Single))
                return (T)(object)ToF2(e);
            throw new ArgumentException("Unable convert to " + t.Name);
        }
    }
}
