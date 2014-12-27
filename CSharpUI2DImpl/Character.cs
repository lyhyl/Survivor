using CSharpUI2DImpl.Core;
using System;
using System.Drawing;
using System.Runtime.InteropServices;

namespace CSharpUI2DImpl
{
    class Character
    {
        Image img;
        Size size;
        _SCHero hero;
        SCHeroActionType prvState = SCHeroActionType.Stay;

        public Character(Image i, Size s, IntPtr p)
        {
            img = i;
            size = s;
            hero = (_SCHero)Marshal.PtrToStructure(p, typeof(_SCHero));
        }

        void Draw(Graphics g)
        {
            RectangleF src = new RectangleF(0, 0, 79, 79);
            RectangleF dest = new RectangleF((float)hero.position.x - 20, (float)hero.position.y - 40, 40, 40);
            g.DrawImage(img, dest, src, GraphicsUnit.Pixel);

            prvState = hero.state;
        }
    }
}
