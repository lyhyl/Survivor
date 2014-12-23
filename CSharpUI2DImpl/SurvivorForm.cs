using CSCore.SoundOut;
using CSharpUI2DImpl.Core;
using CSharpUI2DImpl.Properties;
using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Threading;
using System.Windows.Forms;

namespace CSharpUI2DImpl
{
    public partial class SurvivorForm : Form
    {
        private SplashScreenForm splashForm = new SplashScreenForm();
        private CS2DResource resource = new CS2DResource();
        private string loadingMessage = "";

        private DateTime prvTime;
        private DateTime curTime;
        private TimeSpan duration;
        private float durationF;

        private float viewMoveSpeed = 100;
        private float easeFactor = .3f;

        private UIDisplayData displayData = null;

        private Image miniMapBackground;
        private bool showMiniMap = false;
        private object target;
        private PointF curTarget = new PointF();

        ISoundOut MusicPlayer = WasapiOut.IsSupportedOnCurrentPlatform ?
            (ISoundOut)new WasapiOut() : new DirectSoundOut();

        public SurvivorForm()
        {
            Thread splashThread = new Thread(() => { splashForm.ShowSplash(); });
            splashThread.IsBackground = true;
            splashThread.Start();

            resource.LoadingItem += (object o, LoadingItemEventArgs le) =>
            {
                splashForm.ReportProgress(0, le.Name);
            };
            if (!resource.Load())
                MessageBox.Show("Load resource failed!");

            splashForm.CloseSplash();

            InitializeComponent();

            TopMost = Settings.Default.Fullscreen;
            Activate();
        }

        protected override CreateParams CreateParams
        {
            get
            {
                //new SecurityPermission(SecurityPermissionFlag.UnmanagedCode).Demand();
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

        #region API
        private void Initialize(UIDisplayData data)
        {
            prvTime = DateTime.Now;

            #region generate mini map
            const float percent = .75f;

            int mmwidth = (int)(ClientSize.Width * percent);
            int mmheight = (int)(ClientSize.Height * percent);

            ulong mwidth = data.Map.Width;
            ulong mheight = data.Map.Height;

            float r = Math.Min((float)mmwidth / mwidth, (float)mmheight / mheight);

            mmwidth = (int)(r * mwidth);
            mmheight = (int)(r * mheight);

            int pwidth = resource.GroundGrass.Width;
            int pheight = resource.GroundGrass.Height;

            miniMapBackground = new Bitmap(mmwidth, mmheight);

            Graphics graphics = Graphics.FromImage(miniMapBackground);
            float wu = r * pwidth, hu = r * pheight;
            for (float w = 0; w <= mmwidth; w += wu)
                for (float h = 0; h <= mmheight; h += hu)
                    graphics.DrawImage(resource.GroundGrass, w, h, wu, hu);
            #endregion

            target = new PointF(data.Map.Width / 2.0f, data.Map.Height / 2.0f);

            #region setup music
            MusicPlayer.Initialize(resource.BackgroundMusic);
            MusicPlayer.Volume = .1f;
            MusicPlayer.Play();
            #endregion
        }

        public void SetDisplayData(UIDisplayData data)
        {
            if (displayData == null)
                Initialize(data);
            displayData = data;

            curTime = DateTime.Now;
            duration = curTime - prvTime;
            durationF = (float)duration.TotalMilliseconds;
            prvTime = curTime;

            RestrictTarget();
            EaseTarget();

            Invalidate();
        }
        #endregion

        #region Control
        protected override void OnKeyDown(KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Tab:
                    showMiniMap = true;
                    break;
                case Keys.Up:
                    if (!(target is SCHero))
                        target = ((PointF)target) - new SizeF(0, viewMoveSpeed);
                    break;
                case Keys.Down:
                    if (!(target is SCHero))
                        target = ((PointF)target) + new SizeF(0, viewMoveSpeed);
                    break;
                case Keys.Left:
                    if (!(target is SCHero))
                        target = ((PointF)target) - new SizeF(viewMoveSpeed, 0);
                    break;
                case Keys.Right:
                    if (!(target is SCHero))
                        target = ((PointF)target) + new SizeF(viewMoveSpeed, 0);
                    break;
            }
            base.OnKeyDown(e);
        }

        protected override void OnKeyUp(KeyEventArgs e)
        {
            switch(e.KeyCode)
            {
                case Keys.Tab:
                    showMiniMap = false;
                    break;
            }
            base.OnKeyUp(e);
        }

        void EaseTarget()
        {
            PointF t = target is PointF ? (PointF)target : (target as SCHero).Position;
            float dx = t.X - curTarget.X;
            float dy = t.Y - curTarget.Y;
            dx = Math.Abs(dx) <= 1 ? dx : dx * easeFactor * (durationF / 40);
            dy = Math.Abs(dy) <= 1 ? dy : dy * easeFactor * (durationF / 40);
            curTarget += new SizeF(dx, dy);
        }

        void RestrictTarget()
        {
            if (target is SCHero)
                return;
            PointF t = (PointF)target;
            float hw = (ulong)ClientSize.Width < displayData.Map.Width ? ClientSize.Width / 2.0f : 0;
            float hh = (ulong)ClientSize.Height < displayData.Map.Height ? ClientSize.Height / 2.0f : 0;
            t.X = Math.Max(Math.Min(t.X, displayData.Map.Width - hw), hw);
            t.Y = Math.Max(Math.Min(t.Y, displayData.Map.Height - hh), hh);
            target = t;
        }
        #endregion

        #region Present
        protected override void OnPaint(PaintEventArgs e)
        {
            // Paint World
            float viewLeft = curTarget.X - ClientSize.Width / 2.0f;
            float viewTop = curTarget.Y - ClientSize.Height / 2.0f;
            int groundw = resource.GroundGrass.Width;
            int groundh = resource.GroundGrass.Height;
            float offx = viewLeft - (int)(viewLeft / groundw) * groundw;
            float offy = viewTop - (int)(viewTop / groundh) * groundh;
            for (int ox = (int)-offx; ox < ClientSize.Width; ox += groundw)
                for (int oy = (int)-offy; oy < ClientSize.Height; oy += groundh)
                    e.Graphics.DrawImage(resource.GroundGrass, ox, oy, groundw, groundh);

            // Paint Info
            if (showMiniMap)
                DrawMiniMap(e.Graphics);

            // Debug
            double fps = 1000 / duration.TotalMilliseconds;
            string sfps = double.IsInfinity(fps) ? "∞" : fps.ToString();
            e.Graphics.DrawString(sfps + "fps", DefaultFont, Brushes.DarkBlue, new PointF(0, 0));
        }

        private void DrawMiniMap(Graphics g)
        {
            const int th = 2, dth = th << 1;
            Point lt = new Point((ClientSize.Width - miniMapBackground.Width) >> 1, (ClientSize.Height - miniMapBackground.Height) >> 1);
            g.FillRectangle(Brushes.Black, lt.X - th, lt.Y - th, miniMapBackground.Width + dth, miniMapBackground.Height + dth);
            g.DrawImage(miniMapBackground, lt);
        }
        #endregion
    }
}
