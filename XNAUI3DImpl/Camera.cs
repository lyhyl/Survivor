using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace XNAUI3DImpl
{
    class Camera
    {
        private GraphicsDevice device;
        private float _easeFactor = .95f;
        public float EaseFactor
        {
            set { _easeFactor = MathHelper.Clamp(value, 0.00001f, 1); }
            get { return _easeFactor; }
        }
        public bool EaseToTarget
        {
            set { if (!value)EaseFactor = 1; }
            get { return EaseFactor != 1; }
        }
        public Vector3 Target { set; get; }
        private Vector3 currentTarget;
        private float _distance;
        public float Distance
        {
            set { _distance = MathHelper.Clamp(value, 0.1f, 1000); }
            get { return _distance; }
        }
        public float Angle { set; get; }

        public Matrix ViewMatrix
        {
            get
            {
                Matrix rotMtx = Matrix.CreateRotationX(MathHelper.ToRadians(Angle));
                Vector3 position = Vector3.Transform(new Vector3(0, Distance, 0), rotMtx);
                return Matrix.CreateLookAt(currentTarget + position, currentTarget, Vector3.Up);
            }
        }

        public Matrix ProjectionMatrix { set; get; }

        public Camera(GraphicsDevice d)
        {
            device = d;

            currentTarget = Target = Vector3.Zero;
            Distance = 100;
            Angle = 30;

            ProjectionMatrix = Matrix.CreatePerspectiveFieldOfView(
                MathHelper.ToRadians(90),
                (float)device.Viewport.Width / (float)device.Viewport.Height,
                0.1f, 10000.0f);
        }

        public void Update(GameTime gameTime)
        {
            Vector3 d = Target - currentTarget;
            currentTarget += d * (float)gameTime.ElapsedGameTime.TotalSeconds * EaseFactor;
        }
    }
}
