using CSharpUI2DImpl.Core;
using System;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;

namespace CSharpUI2DImpl
{
    public class UIAdapter
    {
        int survivorState = 1;
        SurvivorForm mainForm;
        Thread formThread;
        public UIAdapter()
        {
            mainForm = new SurvivorForm();
            mainForm.FormClosing += (object s, FormClosingEventArgs e) => { survivorState = 0; };
            formThread = new Thread(() =>
            {
                Application.Run(mainForm);
            });
            formThread.Start();
        }

        ~UIAdapter()
        {
            Application.Exit();
        }

        public int Display(IntPtr pmap, IntPtr pcompetitors)
        {
            SCCollection competitors = new SCCollection(pcompetitors);
            mainForm.Validate();
            return survivorState;
        }
    }
}
