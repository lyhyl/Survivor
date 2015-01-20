using CLRSurvivorLibrary;
using System;
using System.Runtime.InteropServices;
using System.Threading;

namespace XNAUI3DImpl
{
    public class UIAdapter
    {
        Thread formThread;
        SurvivorGame survivorGame = null;
        bool threadExited = false;

        public UIAdapter()
        {
            formThread = new Thread(() =>
            {
                survivorGame = new SurvivorGame();
                survivorGame.Run();
                FormThreadExited();
            });
            formThread.Name = "Survivor XNA 3D Impl";
            formThread.Start();
        }

        public int Initialize(IntPtr pinitd)
        {
            while (survivorGame == null)
                Thread.Yield();
            return survivorGame.InitializeSurvivor(new CSInitializeData(pinitd));
        }

        public int Update(IntPtr pupdd)
        {
            if (threadExited)
                return (int)CSGState.UIExited;
            if (survivorGame != null)
                return survivorGame.UpdateSurvivor(new CSUpdateData(pupdd));
            return (int)CSGState.OK;
        }

        private void FormThreadExited()
        {
            threadExited = true;
        }

        ~UIAdapter()
        {
            if (formThread != null && formThread.ThreadState == ThreadState.Running)
                formThread.Join();
        }
    }
}
