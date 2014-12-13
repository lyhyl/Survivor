using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace UndecoratedCppFuncName
{
    public partial class UndecoratedCppFuncName : Form
    {
        public UndecoratedCppFuncName()
        {
            InitializeComponent();
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            try
            {
                textBox2.Text = "";
                if (textBox1.Text == "")
                    return;

                //textBox1.Text = textBox1.Text.Replace("  ", "");

                string[] ofuncname = textBox1.Text.Split(new char[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);
                for (int i = 0; i < ofuncname.Length; i++)
                {
                    var parts = ofuncname[i].Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                    ofuncname[i] = parts[3];
                }

                int count = 1;
                foreach (var funcn in ofuncname)
                {
                    Process cmd = new Process();
                    cmd.StartInfo.FileName = @"C:\Program Files (x86)\Microsoft Visual Studio 11.0\VC\bin\undname.exe";
                    cmd.StartInfo.Arguments = funcn;
                    cmd.StartInfo.UseShellExecute = false;
                    cmd.StartInfo.CreateNoWindow = true;
                    cmd.StartInfo.RedirectStandardOutput = true;
                    cmd.Start();

                    string output = cmd.StandardOutput.ReadToEnd();
                    var p = output.Split(new char[] { '"' }, StringSplitOptions.RemoveEmptyEntries);
                    textBox2.AppendText(count.ToString() + " ");
                    textBox2.AppendText(funcn);
                    textBox2.AppendText(Environment.NewLine);
                    textBox2.AppendText(p[p.Length - 2]);
                    textBox2.AppendText(Environment.NewLine);
                    textBox2.AppendText(Environment.NewLine);
                    cmd.WaitForExit();

                    count++;
                }
            }
            catch (Exception ex)
            {
                textBox2.Text = "Error:" + ex.Message;
            }
        }
    }
}
