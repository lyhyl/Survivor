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

        public int Display(IntPtr pdata)
        {
            if (threadExited)
                return 0;
            if (survivorGame != null)
                return survivorGame.Display(pdata);
            return 1;
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
