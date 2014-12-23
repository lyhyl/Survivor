using CSharpUI2DImpl.Core;
using System;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;

namespace CSharpUI2DImpl
{
    public class UIAdapter
    {
        object stateLock = new object();
        int survivorState = 1;
        SurvivorForm mainForm;
        Thread formThread;

        public UIAdapter()
        {
            mainForm = new SurvivorForm();
            mainForm.FormClosing += mainForm_FormClosing;
            formThread = new Thread(() => { Application.Run(mainForm); });
            formThread.Name = "Survivor C# 2D Impl";
            formThread.Start();
        }

        void mainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            lock (stateLock) survivorState = 0;
        }

        ~UIAdapter()
        {
            if (formThread.ThreadState == ThreadState.Running)
                Application.Exit();
            formThread.Join();
        }

        public int Display(IntPtr pdata)
        {
            lock (stateLock)
            {
                if (survivorState != 1)
                    return survivorState;
                mainForm.SetDisplayData(new UIDisplayData(pdata));
                return 1;
            }
        }
    }
}
