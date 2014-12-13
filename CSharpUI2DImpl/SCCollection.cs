using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace CSharpUI2DImpl.Core
{
    public class SCCollection<T>
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
                return SCmixed_tConverter.To<T>((SCmixed_t)collection[index]);
            }
        }

        public ulong Add(T e)
        { return collection.Add(SCmixed_tConverter.Form(e)); }
        public bool Remove(T e)
        { return collection.Remove(SCmixed_tConverter.Form(e)); }
        public bool RemoveAt(ulong i)
        { return collection.RemoveAt(i); }
        public ulong Size
        { get { return collection.Size; } }
        public void Clear()
        { collection.Clear(); }
        public ulong Find(T e)
        { return collection.Find(SCmixed_tConverter.Form(e)); }
    }

    public class SCCollection
    {
        [DllImport("SurvivorLibrary.dll", EntryPoint = "??0SCCollection@@QAE@XZ", CallingConvention = CallingConvention.ThisCall)]
        private static extern IntPtr Constructor(IntPtr c);

        [DllImport("SurvivorLibrary.dll", EntryPoint = "??1SCCollection@@QAE@XZ", CallingConvention = CallingConvention.ThisCall)]
        private static extern IntPtr Destructor(IntPtr c);

        [DllImport("SurvivorLibrary.dll", EntryPoint = "?Add@SCCollection@@QAE_KTmixed_t@@@Z", CallingConvention = CallingConvention.ThisCall)]
        private static extern UInt64 CollectionAdd(IntPtr c, SCmixed_t cell);

        [DllImport("SurvivorLibrary.dll", EntryPoint = "?Clear@SCCollection@@QAEXXZ", CallingConvention = CallingConvention.ThisCall)]
        private static extern void CollectionClear(IntPtr c);

        [DllImport("SurvivorLibrary.dll", EntryPoint = "?Find@SCCollection@@QBE_KTmixed_t@@@Z", CallingConvention = CallingConvention.ThisCall)]
        private static extern UInt64 CollectionFind(IntPtr c, SCmixed_t e);

        [DllImport("SurvivorLibrary.dll", EntryPoint = "?Remove@SCCollection@@QAE_N_K@Z", CallingConvention = CallingConvention.ThisCall)]
        private static extern bool CollectionRemoveAt(IntPtr c, UInt64 i);

        [DllImport("SurvivorLibrary.dll", EntryPoint = "?Remove@SCCollection@@QAE_NTmixed_t@@@Z", CallingConvention = CallingConvention.ThisCall)]
        private static extern bool CollectionRemove(IntPtr c, SCmixed_t e);

        [DllImport("SurvivorLibrary.dll", EntryPoint = "?Size@SCCollection@@QBE_KXZ", CallingConvention = CallingConvention.ThisCall)]
        private static extern UInt64 CollectionSize(IntPtr c);

        //[return: MarshalAs(UnmanagedType.LPStruct)]
        [DllImport("SurvivorLibrary.dll", EntryPoint = "??ASCCollection@@QBEPATmixed_t@@_K@Z", CallingConvention = CallingConvention.ThisCall)]
        private static extern IntPtr CollectionIndexer(IntPtr c, UInt64 i);

        private IntPtr _unmanaged = IntPtr.Zero;
        private bool isCopy = false;

        public SCCollection()
        {
            _unmanaged = Memory.Alloc<_SCCollection>();
            Constructor(_unmanaged);
        }

        public SCCollection(IntPtr nativeCollection)
        {
            _unmanaged = nativeCollection;
            isCopy = true;
        }

        ~SCCollection()
        {
            if (!isCopy)
            {
                Destructor(_unmanaged);
                Memory.Free(_unmanaged);
            }
            _unmanaged = IntPtr.Zero;
        }

        public object this[ulong index]
        {
            get
            {
                return Marshal.PtrToStructure(CollectionIndexer(_unmanaged, index), typeof(SCmixed_t));
            }
        }

        public ulong Add(SCmixed_t e)
        { return CollectionAdd(_unmanaged, e); }
        #region Add
        public ulong Add(IntPtr e)
        { return CollectionAdd(_unmanaged, SCmixed_tConverter.Form(e)); }
        public ulong Add(Byte e)
        { return CollectionAdd(_unmanaged, SCmixed_tConverter.Form(e)); }
        public ulong Add(Int16 e)
        { return CollectionAdd(_unmanaged, SCmixed_tConverter.Form(e)); }
        public ulong Add(Int32 e)
        { return CollectionAdd(_unmanaged, SCmixed_tConverter.Form(e)); }
        public ulong Add(Int64 e)
        { return CollectionAdd(_unmanaged, SCmixed_tConverter.Form(e)); }
        public ulong Add(Single e)
        { return CollectionAdd(_unmanaged, SCmixed_tConverter.Form(e)); }
        public ulong Add(Double e)
        { return CollectionAdd(_unmanaged, SCmixed_tConverter.Form(e)); }
        #endregion

        public bool Remove(SCmixed_t e)
        { return CollectionRemove(_unmanaged, e); }
        #region Remove Element
        public bool Remove(IntPtr e)
        { return CollectionRemove(_unmanaged, SCmixed_tConverter.Form(e)); }
        public bool Remove(Byte e)
        { return CollectionRemove(_unmanaged, SCmixed_tConverter.Form(e)); }
        public bool Remove(Int16 e)
        { return CollectionRemove(_unmanaged, SCmixed_tConverter.Form(e)); }
        public bool Remove(Int32 e)
        { return CollectionRemove(_unmanaged, SCmixed_tConverter.Form(e)); }
        public bool Remove(Int64 e)
        { return CollectionRemove(_unmanaged, SCmixed_tConverter.Form(e)); }
        public bool Remove(Single e)
        { return CollectionRemove(_unmanaged, SCmixed_tConverter.Form(e)); }
        public bool Remove(Double e)
        { return CollectionRemove(_unmanaged, SCmixed_tConverter.Form(e)); }
        #endregion

        public bool RemoveAt(ulong i)
        {
            return CollectionRemoveAt(_unmanaged, (UInt32)i);
        }

        public ulong Size
        {
            get
            {
                return CollectionSize(_unmanaged);
            }
        }

        public void Clear()
        {
            CollectionClear(_unmanaged);
        }

        public ulong Find(SCmixed_t e)
        { return CollectionFind(_unmanaged, e); }
        #region Find
        public ulong Find(IntPtr e)
        { return CollectionFind(_unmanaged, SCmixed_tConverter.Form(e)); }
        public ulong Find(Byte e)
        { return CollectionFind(_unmanaged, SCmixed_tConverter.Form(e)); }
        public ulong Find(Int16 e)
        { return CollectionFind(_unmanaged, SCmixed_tConverter.Form(e)); }
        public ulong Find(Int32 e)
        { return CollectionFind(_unmanaged, SCmixed_tConverter.Form(e)); }
        public ulong Find(Int64 e)
        { return CollectionFind(_unmanaged, SCmixed_tConverter.Form(e)); }
        public ulong Find(Single e)
        { return CollectionFind(_unmanaged, SCmixed_tConverter.Form(e)); }
        public ulong Find(Double e)
        { return CollectionFind(_unmanaged, SCmixed_tConverter.Form(e)); }
        #endregion
    }

    [StructLayout(LayoutKind.Sequential, Size = 0x10)]
    public struct _SCCollection
    {
        public static UInt64 nop = UInt64.MaxValue;
        public IntPtr head;
        public IntPtr next;
        public UInt64 size;
    }

    [StructLayout(LayoutKind.Explicit, Size = 0x8)]
    public struct SCmixed_t
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
        public static SCmixed_t Form<T>(T e)
        {
            SCmixed_t v = new SCmixed_t();
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

        public static IntPtr ToPtr(SCmixed_t e) { return e.ptr; }
        public static Byte ToI1(SCmixed_t e) { return e.i1; }
        public static Int16 ToI2(SCmixed_t e) { return e.i2; }
        public static Int32 ToI4(SCmixed_t e) { return e.i4; }
        public static Int64 ToI8(SCmixed_t e) { return e.i8; }
        public static Single ToF2(SCmixed_t e) { return e.f2; }
        public static Double ToF4(SCmixed_t e) { return e.f4; }

        public static T To<T>(SCmixed_t e)
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
