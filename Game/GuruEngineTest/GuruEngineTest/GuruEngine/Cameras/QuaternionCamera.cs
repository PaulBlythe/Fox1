using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

using GuruEngine.Audio;

namespace GuruEngine.Cameras
{
    public class QuaternionCamera: Camera
    {
        Quaternion Rotation;
        MouseState OldState;
        Matrix World;
        Vector3 Up;

        public Quaternion ViewAdjust = Quaternion.Identity;

        public QuaternionCamera(float aspectratio)
        {
            FarClip = 60000.0f;
            Position = new Vector3(-100, 135, 50);
            Projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(60), aspectratio, 0.15f, 60000.0f);
            Up = Vector3.Up;
            Forward = Vector3.Forward;
            View = Matrix.CreateLookAt(Position, Position + Forward, Up);
            Right = Vector3.Right;
            Rotation = Quaternion.CreateFromYawPitchRoll(0, 0, 0);
        }

        public override void Update(float dt)
        {
            float updownRotation = 0.0f;
            float leftrightRotation = 0.0f;
            float roll = 0.0f;

            KeyboardState ks = Keyboard.GetState();
            float f = 0;
            float l = 0;

            if (ks.IsKeyDown(Keys.W))
                f = 1;
            if (ks.IsKeyDown(Keys.S))
                f = -1;
            if (ks.IsKeyDown(Keys.D))
                l = 1;
            if (ks.IsKeyDown(Keys.A))
                l = -1;
            if (ks.IsKeyDown(Keys.LeftShift))
            {
                l *= 10;
                f *= 10;
            }
            if (ks.IsKeyDown(Keys.LeftControl))
            {
                l *= 100;
                f *= 100;
            }

            MouseState ms = Mouse.GetState();
            float dx = ms.Position.X - OldState.Position.X;
            float dy = ms.Position.Y - OldState.Position.Y;

            if (ms.LeftButton == ButtonState.Pressed)
            {
                leftrightRotation = dx* dt;
                updownRotation = dy * dt;
            }
            if (ms.RightButton == ButtonState.Pressed)
            {
               roll = dx * dt;
            }

            Quaternion additionalRotation = Quaternion.CreateFromAxisAngle(Vector3.UnitX, -updownRotation) * 
                                            Quaternion.CreateFromAxisAngle(Vector3.UnitY, leftrightRotation) *
                                            Quaternion.CreateFromAxisAngle(Vector3.UnitZ, -roll);

            Rotation *= additionalRotation;
            Rotation.Normalize();

            Quaternion temp = Rotation * ViewAdjust;

            Matrix orient = Matrix.CreateFromQuaternion(temp);
            Forward = orient.Forward;
            Right = orient.Right;
            Up = orient.Up;

            Position += Forward * f * dt;
            Position += Right * l * dt;

            OldState = ms;
            View = Matrix.CreateLookAt(Position, Position + Forward, Up);
            World = Matrix.CreateTranslation(Position) * orient;

            AudioManager.MoveListener(View);

            Matrix viewProjection = View * Projection;
            Frustum = new BoundingFrustum(viewProjection);
        }

        public override Matrix GetWorld()
        {
            return World;
        }

        public Quaternion GetOrientation()
        {
            return Rotation;
        }

        public Vector3 GetLocalPosition()
        {
            return Position;
        }

        public override void Yaw(float angle)
        {
            Rotation *= Quaternion.CreateFromAxisAngle(Vector3.UnitY, angle);
            Rotation.Normalize();
        }

        public override void SetPosition(Vector3 location)
        {
            Position = location;
        }
    }
}
