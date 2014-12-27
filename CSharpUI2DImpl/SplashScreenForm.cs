using CSharpUI2DImpl.Properties;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CSharpUI2DImpl
{
    public partial class SplashScreenForm : Form
    {
        delegate void ShowCloseSplashDelegate();

        Timer timer = new Timer();
        DateTime startInTime;
        DateTime startOutTime = DateTime.MinValue;
        double fadeInTime = 500;
        double fadeOutTime = 500;

        int percentage = 0;
        int colorBlend = 0;
        string message = "";

        Bitmap beginBitmap = Resources.SurvivorB;
        Bitmap endBitmap = Resources.SurvivorR;

        public SplashScreenForm()
        {
            InitializeComponent();
        }
        public SplashScreenForm(double fadeIn, double fadeOut)
        {
            fadeInTime = fadeIn;
            fadeOutTime = fadeOut;
            InitializeComponent();
        }

        protected override CreateParams CreateParams
        {
            get
            {
                // Add the layered extended style (WS_EX_LAYERED) to this window
                CreateParams createParams = base.CreateParams;
                createParams.ExStyle |= WinAPI.WS_EX_LAYERED;
                return createParams;
            }
        }

        public void ShowSplash()
        {
            if (InvokeRequired)
            {
                // We're not in the UI thread, so we need to call BeginInvoke
                BeginInvoke(new ShowCloseSplashDelegate(ShowSplash));
                return;
            }
            Show();
            Application.Run(this);
        }

        public void CloseSplash()
        {
            while ((DateTime.Now - startInTime).TotalMilliseconds < fadeInTime)
                Application.DoEvents();
            startOutTime = DateTime.Now;
            message = "Starting Survivor...";
            while ((DateTime.Now - startOutTime).TotalMilliseconds < fadeOutTime)
                Application.DoEvents();
            KillSplash();
        }

        private void KillSplash()
        {
            if (InvokeRequired)
            {
                // We're not in the UI thread, so we need to call BeginInvoke
                BeginInvoke(new ShowCloseSplashDelegate(KillSplash));
                return;
            }
            timer.Stop();
            Close();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            int diff = percentage - colorBlend;
            colorBlend = diff > 1 ? (int)(diff * .5) + percentage : percentage;

            byte op;
            if (startOutTime == DateTime.MinValue)
            {
                double delta = (DateTime.Now - startInTime).TotalMilliseconds;
                op = (byte)(255 * (Math.Min(fadeInTime, delta) / fadeInTime));
            }
            else
            {
                double delta = (DateTime.Now - startOutTime).TotalMilliseconds;
                op = (byte)(255 * (1 - (Math.Min(fadeInTime, delta) / fadeInTime)));
            }

            Bitmap bmp = new Bitmap(beginBitmap);
            Graphics g = Graphics.FromImage(bmp);

            ColorMatrix cm = new ColorMatrix();
            cm.Matrix00 = cm.Matrix11 = cm.Matrix22 = cm.Matrix44 = 1;
            cm.Matrix33 = colorBlend / 100.0f;
            ImageAttributes ia = new ImageAttributes();
            ia.SetColorMatrix(cm);
            g.DrawImage(endBitmap, new Rectangle(0, 0, endBitmap.Width, endBitmap.Height),
                0, 0, endBitmap.Width, endBitmap.Height, GraphicsUnit.Pixel, ia);

            StringFormat sf = new StringFormat();
            sf.Alignment = StringAlignment.Center;
            g.DrawString((message == "" ? "" : "Loading : ") + message, DefaultFont, Brushes.Black, 256, 384, sf);
            PaintTransparency(bmp, op);
        }

        private void Form_Load(object sender, EventArgs e)
        {
            beginBitmap = new Bitmap(BackgroundImage);
            timer.Interval = 1;
            timer.Tick += (object s, EventArgs ea) =>
            {
                OnPaint(new PaintEventArgs(CreateGraphics(), ClientRectangle));
            };
            startInTime = DateTime.Now;
            timer.Start();
        }

        protected void PaintTransparency(Bitmap bitmap, byte opacity)
        {
            if (bitmap.PixelFormat != PixelFormat.Format32bppArgb)
                throw new ApplicationException("The bitmap must be 32ppp with alpha-channel.");

            // The ideia of this is very simple,
            // 1. Create a compatible DC with screen;
            // 2. Select the bitmap with 32bpp with alpha-channel in the compatible DC;
            // 3. Call the UpdateLayeredWindow.

            IntPtr screenDc = WinAPI.GetDC(IntPtr.Zero);
            IntPtr memDc = WinAPI.CreateCompatibleDC(screenDc);
            IntPtr hBitmap = IntPtr.Zero;
            IntPtr oldBitmap = IntPtr.Zero;

            try
            {
                hBitmap = bitmap.GetHbitmap(Color.FromArgb(0));  // grab a GDI handle from this GDI+ bitmap
                oldBitmap = WinAPI.SelectObject(memDc, hBitmap);

                WinAPI.Size size = new WinAPI.Size(bitmap.Width, bitmap.Height);
                WinAPI.Point pointSource = new WinAPI.Point(0, 0);
                WinAPI.Point topPos = new WinAPI.Point(Left, Top);
                WinAPI.BLENDFUNCTION blend = new WinAPI.BLENDFUNCTION();
                blend.BlendOp = WinAPI.AC_SRC_OVER;
                blend.BlendFlags = 0;
                blend.SourceConstantAlpha = opacity;
                blend.AlphaFormat = WinAPI.AC_SRC_ALPHA;

                WinAPI.UpdateLayeredWindow(Handle, screenDc, ref topPos, ref size, memDc, ref pointSource, 0, ref blend, WinAPI.ULW_ALPHA);
            }
            finally
            {
                WinAPI.ReleaseDC(IntPtr.Zero, screenDc);
                if (hBitmap != IntPtr.Zero)
                {
                    WinAPI.SelectObject(memDc, oldBitmap);
                    //Windows.DeleteObject(hBitmap);
                    // The documentation says that we have to use the Windows.DeleteObject...
                    // but since there is no such method I use the normal DeleteObject from Win32 GDI
                    // and it's working fine without any resource leak.
                    WinAPI.DeleteObject(hBitmap);
                }
                WinAPI.DeleteDC(memDc);
            }
        }

        public void ReportProgress(int p, string msg)
        {
            percentage = p;
            message = msg;
        }

        class WinAPI
        {
            public enum Bool
            {
                False = 0,
                True
            };

            [StructLayout(LayoutKind.Sequential)]
            public struct Point
            {
                public Int32 x;
                public Int32 y;
                public Point(Int32 x, Int32 y) { this.x = x; this.y = y; }
            }

            [StructLayout(LayoutKind.Sequential)]
            public struct Size
            {
                public Int32 cx;
                public Int32 cy;
                public Size(Int32 cx, Int32 cy) { this.cx = cx; this.cy = cy; }
            }

            [StructLayout(LayoutKind.Sequential)]
            public struct RECT
            {
                public int Left, Top, Right, Bottom;
                public RECT(int left, int top, int right, int bottom)
                {
                    Left = left;
                    Top = top;
                    Right = right;
                    Bottom = bottom;
                }
                public RECT(System.Drawing.Rectangle r) : this(r.Left, r.Top, r.Right, r.Bottom) { }
            }

            [StructLayout(LayoutKind.Sequential, Pack = 1)]
            public struct BLENDFUNCTION
            {
                public byte BlendOp;
                public byte BlendFlags;
                public byte SourceConstantAlpha;
                public byte AlphaFormat;
            }

            public const Int32 WS_EX_LAYERED = 0x80000;
            public const Int32 ULW_COLORKEY = 0x00000001;
            public const Int32 ULW_ALPHA = 0x00000002;
            public const Int32 ULW_OPAQUE = 0x00000004;

            public const byte AC_SRC_OVER = 0x00;
            public const byte AC_SRC_ALPHA = 0x01;

            [DllImport("user32.dll", ExactSpelling = true, SetLastError = true)]
            public static extern Bool UpdateLayeredWindow(IntPtr hwnd, IntPtr hdcDst, ref Point pptDst, ref Size psize, IntPtr hdcSrc, ref Point pprSrc, Int32 crKey, ref BLENDFUNCTION pblend, Int32 dwFlags);
            [DllImport("user32.dll", ExactSpelling = true, SetLastError = true)]
            public static extern IntPtr GetDC(IntPtr hWnd);
            [DllImport("user32.dll", ExactSpelling = true)]
            public static extern int ReleaseDC(IntPtr hWnd, IntPtr hDC);
            [DllImport("gdi32.dll", ExactSpelling = true, SetLastError = true)]
            public static extern IntPtr CreateCompatibleDC(IntPtr hDC);
            [DllImport("gdi32.dll", ExactSpelling = true, SetLastError = true)]
            public static extern Bool DeleteDC(IntPtr hdc);
            [DllImport("gdi32.dll", ExactSpelling = true)]
            public static extern IntPtr SelectObject(IntPtr hDC, IntPtr hObject);
            [DllImport("gdi32.dll", ExactSpelling = true, SetLastError = true)]
            public static extern Bool DeleteObject(IntPtr hObject);
        }
    }
}
