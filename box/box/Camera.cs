
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;


namespace Box
{
    class Camera
    {
        //Test comment for git
        public enum Rotations
        {
            YawLeft,
            YawRight,
            PitchUp,
            PitchDown,
            RollClock,
            RollCounter
        };

        Vector3 position;
        Vector3 rotation;
        Vector3 up = Vector3.Up;
        Vector3 right = Vector3.Right;
        Vector3 look = Vector3.Forward;
        Matrix viewMatrix;


        //Another test comment
        public void Rotate(Rotations r, float angle)
        {
            switch (r)
            {
                case Rotations.YawLeft:
                    RotateAroundY(angle); break;
                case Rotations.YawRight:
                    RotateAroundY(-angle); break;
                case Rotations.PitchUp:
                    RotateAroundX(angle); break;
                case Rotations.PitchDown:
                    RotateAroundX(-angle); break;
            }

            CalculateViewMatrix();
        }

        public void Move()
        {

        }

        private void CalculateViewMatrix()
        {
            Matrix yawMatrix = Matrix.CreateFromAxisAngle(up, rotation.Y);
            look = Vector3.Transform(look, yawMatrix);
            right = Vector3.Transform(right, yawMatrix);

            Matrix pitchMatrix = Matrix.CreateFromAxisAngle(right, rotation.X);
            look = Vector3.Transform(look, pitchMatrix);
            up = Vector3.Transform(up, pitchMatrix);

            Vector3 target = position + look;
            viewMatrix = Matrix.CreateLookAt(position, target, up);
        }

        private void RotateAroundX(float angle)
        {
            rotation.X += angle;

            if (rotation.X > MathHelper.TwoPi)
                rotation.X -= MathHelper.TwoPi;
            else if (rotation.X < 0)
                rotation.X += MathHelper.TwoPi;
        }

        private void RotateAroundY(float angle)
        {
            rotation.Y += angle;

            if (rotation.Y > MathHelper.TwoPi)
                rotation.Y -= MathHelper.TwoPi;
            else if (rotation.Y < 0)
                rotation.Y += MathHelper.TwoPi;
        }
    }
}