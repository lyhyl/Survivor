using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XNAUI3DImpl
{
    public class SKeyboardEventArgs : EventArgs
    {
        public Keys Key { private set; get; }
        public SKeyboardEventArgs(Keys k)
        {
            Key = k;
        }
    }
    public delegate void KeyboardActionHandler(SKeyboardEventArgs e);
    public class SKeyboard
    {
        HashSet<Keys> prvPressedKeys = new HashSet<Keys>();
        HashSet<Keys> prvPressedSubKeys = new HashSet<Keys>();
        HashSet<Keys> pressedKeys = new HashSet<Keys>();
        KeyboardState prvKeyboardState;

        public SKeyboard()
        {
        }

        public event KeyboardActionHandler KeyDown;
        public event KeyboardActionHandler KeyClick;

        public void Update(GameTime gameTime)
        {
            KeyboardState kbs = Keyboard.GetState();
            Keys[] pk = kbs.GetPressedKeys();
            pressedKeys.Clear();
            foreach (var k in pk)
                pressedKeys.Add(k);

            prvPressedSubKeys.ExceptWith(pressedKeys);

            if (KeyDown != null)
                foreach (var k in pressedKeys)
                    KeyDown(new SKeyboardEventArgs(k));
            if (KeyClick != null)
                foreach (var k in prvPressedSubKeys)
                    KeyClick(new SKeyboardEventArgs(k));

            prvKeyboardState = kbs;
            prvPressedKeys.Clear();
            prvPressedSubKeys.Clear();
            foreach (var k in pk)
            {
                prvPressedKeys.Add(k);
                prvPressedSubKeys.Add(k);
            }
        }

        public bool IsKeyDown(Keys k) { return prvKeyboardState.IsKeyDown(k); }
        public bool IsKeyUp(Keys k) { return prvKeyboardState.IsKeyUp(k); }
    }
}
