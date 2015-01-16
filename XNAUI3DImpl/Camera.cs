using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XNAUI3DImpl
{
    class Camera
    {
        public Vector3 Target { set; get; }
        private float _distance;
        public float Distance
        {
            set
            {
                _distance = MathHelper.Clamp(value, 0.1f, 1000);
            }
            get { return _distance; }
        }
        public float Angle { set; get; }

        public Matrix ViewMatrix
        {
            get
            {
                Matrix rotMtx = Matrix.CreateRotationX(MathHelper.ToRadians(Angle));
                Vector3 position = Vector3.Transform(new Vector3(0, _distance, 0), rotMtx);
                return Matrix.CreateLookAt(Target + position, Target, Vector3.Up);
            }
        }

        public Camera()
        {
            Target = Vector3.Zero;
            Distance = 50;
            Angle = 45;
        }
    }
}
