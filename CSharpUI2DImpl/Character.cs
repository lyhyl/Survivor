using CSSurvivorLibrary;
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;

namespace CSharpUI2DImpl
{
    class Character
    {
        Image img;
        Size size;
        IntPtr ptr;
        int animationTime;
        _SCHero prvHero;

        public Character(Image i, Size s, IntPtr p)
        {
            img = i;
            size = s;
            ptr = p;
            animationTime = 0;
            prvHero = (_SCHero)Marshal.PtrToStructure(ptr, typeof(_SCHero));
        }

        public void Draw(Graphics g)
        {
            _SCHero curHero = (_SCHero)Marshal.PtrToStructure(ptr, typeof(_SCHero));

            if (curHero.state != prvHero.state)
                animationTime = 0;

            animationTime %= 10000;
            int frame = animationTime / 1000;

            Rectangle dest = new Rectangle((int)curHero.position.x - size.Width / 2,
                (int)curHero.position.y - size.Height, size.Width, size.Height);
            ImageAttributes ia = new ImageAttributes();
            Color col = Color.FromArgb(10, 10, 10);
            ia.SetColorKey(Color.Black, col);
            g.DrawImage(img, dest, 0, 0, size.Width, size.Height, GraphicsUnit.Pixel, ia);

            g.DrawLine(Pens.Black, new PointF((float)curHero.position.x, (float)curHero.position.y),
                new PointF((float)(curHero.position.x + curHero.direction.x * 20),
                    (float)(curHero.position.y + curHero.direction.y * 20)));

            prvHero = curHero;
        }
    }
}
