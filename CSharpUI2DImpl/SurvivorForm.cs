using CSharpUI2DImpl.Core;
using CSharpUI2DImpl.Properties;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Security.Permissions;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CSharpUI2DImpl
{
    public partial class SurvivorForm : Form
    {
        //SCCollection<int> collection = new SCCollection<int>();

        public SurvivorForm()
        {
            InitializeComponent();
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            e.Cancel = true;
            base.OnClosing(e);
            Hide();
        }

        protected override CreateParams CreateParams
        {
            get
            {
                new SecurityPermission(SecurityPermissionFlag.UnmanagedCode).Demand();
                CreateParams cp = base.CreateParams;
                if (Settings.Default.Fullscreen)
                {
                    //remove (WS_CAPTION | WS_THICKFRAME | WS_MINIMIZE | WS_MAXIMIZE | WS_SYSMENU)
                    cp.Style &= ~(0x00C00000 | 0x00040000 | 0x20000000 | 0x01000000 | 0x00080000);
                    //set (WS_MAXIMIZE)
                    cp.Style |= 0x01000000;
                }
                else
                {
                    //remove (WS_SIZEBOX | WS_MAXIMIZEBOX | WS_MINIMIZEBOX)
                    cp.Style &= ~(0x00040000 | 0x00010000 | 0x00020000);
                }
                return cp;
            }
        }
    }
}
