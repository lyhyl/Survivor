using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;

namespace XNAUI3DImpl
{
    public delegate void MouseActionHandler(EventArgs e);
    public class SMouse
    {
        private GraphicsDevice device;
        private MouseState prvMouseState;
        private Texture2D image;

        public int WheelDelta { private set; get; }

        public Vector2 Direction { private set; get; }

        public event MouseActionHandler MouseWheelChanged;
        public event MouseActionHandler MouseMove;

        public event MouseActionHandler MouseLeftDown;
        public event MouseActionHandler MouseLeftUp;
        public event MouseActionHandler MouseLeftPress;
        public event MouseActionHandler MouseLeftClick;

        public event MouseActionHandler MouseMiddleDown;
        public event MouseActionHandler MouseMiddleUp;
        public event MouseActionHandler MouseMiddlePress;
        public event MouseActionHandler MouseMiddleClick;

        public event MouseActionHandler MouseRightDown;
        public event MouseActionHandler MouseRightUp;
        public event MouseActionHandler MouseRightPress;
        public event MouseActionHandler MouseRightClick;

        public Vector2 Position
        {
            get { return new Vector2(prvMouseState.X, prvMouseState.Y); }
        }

        public SMouse(GraphicsDevice d) { device = d; }

        public void LoadContent(ContentManager content)
        {
            image = content.Load<Texture2D>("Element/Cursor");
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(image, Position, Color.White);
        }

        public void Update(GameTime gameTime)
        {
            MouseState ms = Mouse.GetState();

            Direction = new Vector2(ms.X, ms.Y) - Position;
            WheelDelta = prvMouseState.ScrollWheelValue - ms.ScrollWheelValue;

            if (Direction != Vector2.Zero && MouseMove != null)
                MouseMove(new EventArgs());
            if (WheelDelta != 0 && MouseWheelChanged != null)
                MouseWheelChanged(new EventArgs());

            switch ((int)ms.LeftButton | ((int)prvMouseState.LeftButton << 1))
            {
                case 0: if (MouseLeftUp != null) MouseLeftUp(new EventArgs()); break;
                case 1: if (MouseLeftPress != null) MouseLeftPress(new EventArgs()); break;
                case 2: if (MouseLeftClick != null) MouseLeftClick(new EventArgs()); break;
                case 3: if (MouseLeftDown != null) MouseLeftDown(new EventArgs()); break;
            }
            switch ((int)ms.MiddleButton | ((int)prvMouseState.MiddleButton << 1))
            {
                case 0: if (MouseMiddleUp != null) MouseMiddleUp(new EventArgs()); break;
                case 1: if (MouseMiddlePress != null) MouseMiddlePress(new EventArgs()); break;
                case 2: if (MouseMiddleClick != null) MouseMiddleClick(new EventArgs()); break;
                case 3: if (MouseMiddleDown != null) MouseMiddleDown(new EventArgs()); break;
            }
            switch ((int)ms.RightButton | ((int)prvMouseState.RightButton << 1))
            {
                case 0: if (MouseRightUp != null) MouseRightUp(new EventArgs()); break;
                case 1: if (MouseRightPress != null) MouseRightPress(new EventArgs()); break;
                case 2: if (MouseRightClick != null) MouseRightClick(new EventArgs()); break;
                case 3: if (MouseRightDown != null) MouseRightDown(new EventArgs()); break;
            }

            prvMouseState = ms;
        }

        public Vector3 SelectFloor(Matrix projectionMatrix, Matrix viewMatrix)
        {
            Ray ray = CalculateCursorRay(projectionMatrix, viewMatrix);
            float t = -ray.Position.Y / ray.Direction.Y;
            if (t >= 0)
                return ray.Position + t * ray.Direction;
            return Vector3.Zero;
        }

        private Ray CalculateCursorRay(Matrix projectionMatrix, Matrix viewMatrix)
        {
            Vector3 nearSource = new Vector3(Position, 0f);
            Vector3 farSource = new Vector3(Position, 1f);
            Vector3 nearPoint = device.Viewport.Unproject(nearSource,
                projectionMatrix, viewMatrix, Matrix.Identity);
            Vector3 farPoint = device.Viewport.Unproject(farSource,
                projectionMatrix, viewMatrix, Matrix.Identity);
            Vector3 direction = farPoint - nearPoint;
            direction.Normalize();
            return new Ray(nearPoint, direction);
        }
    }
}
