using System;
using System.Runtime.InteropServices;

namespace CSharpUI2DImpl
{
    internal class SurvivorCoreInvoker
    {
        private static SurvivorCoreInvoker singleton = null;

        #region System API
        [DllImport("kernel32", SetLastError = true, CharSet = CharSet.Ansi)]
        static extern IntPtr LoadLibrary([MarshalAs(UnmanagedType.LPStr)]string lpFileName);
        [DllImport("kernel32", CharSet = CharSet.Ansi, ExactSpelling = true, SetLastError = true)]
        static extern IntPtr GetProcAddress(IntPtr hModule, [MarshalAs(UnmanagedType.LPStr)]string procName);
        [DllImport("kernel32.dll", SetLastError = true)]
        static extern bool FreeLibrary(IntPtr hModule);
        #endregion

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate double CalcculateDelegate([MarshalAs(UnmanagedType.LPStr)]string expression);
        /*private IntPtr CoreInitPtr;
        static private CalcculateDelegate CoreInit;*/

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate int TestDelegate();
        private IntPtr TestPtr;
        static public TestDelegate Test;

        private IntPtr CalcCoreDLL = IntPtr.Zero;

        static public void Initialize()
        {
            singleton = new SurvivorCoreInvoker();
        }

        private SurvivorCoreInvoker()
        {
            /*CalcCoreDLL = LoadLibrary(Settings.Default.CoreName);
            if (CalcCoreDLL == IntPtr.Zero)
                throw new DllNotFoundException(Settings.Default.CoreName);*/
            LoadCoreFunctions();  
        }

        private void LoadCoreFunctions()
        {
            //LoadFunction("Test");
        }

        private void LoadFunction(string name)
        {
            TestPtr = GetProcAddress(CalcCoreDLL, name);
            if (TestPtr == IntPtr.Zero)
                throw new MissingMethodException(name);
            Test = (TestDelegate)Marshal.GetDelegateForFunctionPointer(TestPtr, typeof(TestDelegate));
        }

        ~SurvivorCoreInvoker()
        {
            FreeLibrary(CalcCoreDLL);
        }
    }
}
