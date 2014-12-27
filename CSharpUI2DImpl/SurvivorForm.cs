using CSCore.SoundOut;
using CSharpUI2DImpl.Core;
using CSharpUI2DImpl.Properties;
using System;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;

namespace CSharpUI2DImpl
{
    public partial class SurvivorForm : Form
    {
        private SplashScreenForm splashForm = new SplashScreenForm();
        private CS2DResource resource = new CS2DResource();

        private DateTime prvTime;
        private DateTime curTime;
        private TimeSpan duration;
        private float durationF;

        private float viewMoveSpeed = 100;
        private float easeFactor = .3f;

        private UIDisplayData displayData = null;

        private Image miniMapBackground;
        private float miniMapScale;
        private bool showMiniMap = false;

        private object target;
        private PointF curTarget = new PointF();

        private bool mouseDown = false;

        private ISoundOut MusicPlayer = WasapiOut.IsSupportedOnCurrentPlatform ?
            (ISoundOut)new WasapiOut() : new DirectSoundOut();

        public SurvivorForm()
        {
            Thread splashThread = new Thread(() => { splashForm.ShowSplash(); });
            splashThread.Name = "Survivor C# 2D Impl Splash";
            splashThread.IsBackground = true;
            splashThread.Start();

            resource.LoadingItem += (object o, LoadingItemEventArgs le) =>
            { splashForm.ReportProgress(le.Percentage, le.Name); };
            if (!resource.Load())
            {
                MessageBox.Show("Load resource failed!");
            }

            splashForm.CloseSplash();

            InitializeComponent();

            TopMost = Settings.Default.Fullscreen;
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
        private void FirstPresentInitialize(UIDisplayData data)
        {
            prvTime = DateTime.Now;

            #region generate mini map
            const float percent = .75f;

            int mmwidth = (int)(ClientSize.Width * percent);
            int mmheight = (int)(ClientSize.Height * percent);

            ulong mwidth = data.Map.Width;
            ulong mheight = data.Map.Height;

            miniMapScale = Math.Min((float)mmwidth / mwidth, (float)mmheight / mheight);

            mmwidth = (int)(miniMapScale * mwidth);
            mmheight = (int)(miniMapScale * mheight);

            int pwidth = resource.GroundGrass.Width;
            int pheight = resource.GroundGrass.Height;

            miniMapBackground = new Bitmap(mmwidth, mmheight);

            Graphics graphics = Graphics.FromImage(miniMapBackground);
            float wu = miniMapScale * pwidth, hu = miniMapScale * pheight;
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
                FirstPresentInitialize(data);
            displayData = data;
            UpdateFrame();
        }

        private void UpdateFrame()
        {
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
                    if (target is PointF)
                        target = ((PointF)target) - new SizeF(0, viewMoveSpeed);
                    break;
                case Keys.Down:
                    if (target is PointF)
                        target = ((PointF)target) + new SizeF(0, viewMoveSpeed);
                    break;
                case Keys.Left:
                    if (target is PointF)
                        target = ((PointF)target) - new SizeF(viewMoveSpeed, 0);
                    break;
                case Keys.Right:
                    if (target is PointF)
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

        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);
            mouseDown = true;
            FollowMouse(e);
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            base.OnMouseUp(e);
            mouseDown = false;
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);
            if (mouseDown && showMiniMap)
                FollowMouse(e);
        }

        private void FollowMouse(MouseEventArgs e)
        {
            int sw = ClientSize.Width;
            int sh = ClientSize.Height;
            if (showMiniMap)
            {
                int gbw = miniMapBackground.Width;
                int gbh = miniMapBackground.Height;

                int left = (sw - gbw) >> 1;
                int top = (sh - gbh) >> 1;

                float x = (e.X - left) / miniMapScale;
                float y = (e.Y - top) / miniMapScale;

                if (x > 0 && x < displayData.Map.Width && y > 0 && y < displayData.Map.Height)
                    target = new PointF(x, y);
            }
            else if (target is PointF)
            {
                PointF t = (PointF)target;
                t.X += e.X - (sw >> 1);
                t.Y += e.Y - (sh >> 1);
                target = t;
            }
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
            if (displayData == null)
                return;

            DrawWorld(e.Graphics);
            if (showMiniMap)
                DrawMiniMap(e.Graphics);

            PrintDebugInfo(e.Graphics);
        }

        private void PrintDebugInfo(Graphics g)
        {
            double fps = 1000 / duration.TotalMilliseconds;
            string sfps = double.IsInfinity(fps) ? "∞" : fps.ToString();
            g.DrawString(sfps + "fps", DefaultFont, Brushes.DarkBlue, new PointF(0, 0));
            if (displayData != null)
                g.DrawString(displayData.Heroes.Size.ToString(), DefaultFont, Brushes.DarkBlue, new PointF(0, 10));
        }

        private void DrawWorld(Graphics g)
        {
            DrawWorldBase(g);
            DrawWorldElements(g);
        }

        private void DrawWorldBase(Graphics g)
        {
            float viewLeft = curTarget.X - ClientSize.Width / 2.0f;
            float viewTop = curTarget.Y - ClientSize.Height / 2.0f;
            int groundw = resource.GroundGrass.Width;
            int groundh = resource.GroundGrass.Height;
            float offx = viewLeft - (int)(viewLeft / groundw) * groundw;
            float offy = viewTop - (int)(viewTop / groundh) * groundh;
            for (int ox = (int)-offx; ox < ClientSize.Width; ox += groundw)
                for (int oy = (int)-offy; oy < ClientSize.Height; oy += groundh)
                    g.DrawImage(resource.GroundGrass, ox, oy, groundw, groundh);
        }

        private void DrawWorldElements(Graphics g)
        {
            foreach (_SCHero hero in displayData.Heroes)
                DrawHero(g, hero);
        }

        private void DrawHero(Graphics g, _SCHero hero)
        {
            float vx = (float)(hero.position.x - curTarget.X) + (ClientSize.Width >> 1);
            float vy = (float)(hero.position.y - curTarget.Y) + (ClientSize.Height >> 1);
            if (vx > 0 && vx < ClientSize.Width && vy > 0 && vy < ClientSize.Height)
                g.FillRectangle(Brushes.Red, vx , vy , 50, 50);
        }

        private void DrawMiniMap(Graphics g)
        {
            const int th = 2, dth = th << 1;

            int gbw = miniMapBackground.Width;
            int gbh = miniMapBackground.Height;

            int sw = ClientSize.Width;
            int sh = ClientSize.Height;

            int left = (sw - gbw) >> 1;
            int top = (sh - gbh) >> 1;
            g.FillRectangle(Brushes.Black, left - th, top - th, gbw + dth, gbh + dth);
            g.DrawImage(miniMapBackground, new Point(left, top));

            DrawMiniMapElements(g);

            float vw = miniMapScale * sw;
            float vh = miniMapScale * sh;
            float vl = miniMapScale * curTarget.X - vw / 2.0f;
            float vt = miniMapScale * curTarget.Y - vh / 2.0f;
            g.DrawRectangle(Pens.Black, left + vl, top + vt, vw, vh);
        }

        private void DrawMiniMapElements(Graphics g)
        {
            int gbw = miniMapBackground.Width;
            int gbh = miniMapBackground.Height;
            int sw = ClientSize.Width;
            int sh = ClientSize.Height;
            int left = (sw - gbw) >> 1;
            int top = (sh - gbh) >> 1;
            foreach (_SCHero hero in displayData.Heroes)
            {
                float x = (float)hero.position.x * miniMapScale;
                float y = (float)hero.position.y * miniMapScale;
                g.FillRectangle(Brushes.Blue, left + x, top + y, 3, 3);
            }
        }
        #endregion
    }
}
