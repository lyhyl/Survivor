using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Forms;

namespace SceneObjectEditor
{
    public partial class SceneObjectEditor : Form
    {
        public SceneObjectEditor()
        {
            InitializeComponent();
        }

        private void fileFToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Survivor Scene Object Editor\nCopyright (c) Wingkou Loeng", "About", MessageBoxButtons.OK);
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                using (BinaryReader reader = new BinaryReader(openFileDialog1.OpenFile()))
                {

                }
            }
        }

        private void newToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }
    }
}
