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
    public class DebugCamera : Camera
    {
        public static DebugCamera Instance;

        Vector3 Up;

        MouseState OldState;

        public DebugCamera(float aspectratio)
        {
            FarClip = 60000.0f;
            Position = new Vector3(-100, 135, 50);
            Projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(60), aspectratio, 0.5f, 60000.0f);
            Up = Vector3.Up;
            Forward = Vector3.Forward;
            View = Matrix.CreateLookAt(Position, Position + Forward, Up);
            Right = Vector3.Right;
            Instance = this;
        }

        public override void Update(float dt)
        {
            if (!HasFocus)
            {
                return;
            }
            dt = 0.016f;


            MouseState ms = Mouse.GetState();
            float dx = ms.Position.X - OldState.Position.X;
            float dy = ms.Position.Y - OldState.Position.Y;

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


            if (ms.LeftButton == ButtonState.Pressed)
            {
                Yaw(dx * dt);
                Pitch(dy * dt);
            }
            if (ms.RightButton == ButtonState.Pressed)
            {
                Roll(dx * dt);
            }

            Position += Forward * f * dt;
            Position += View.Right * l * dt;
            Right = View.Right;
            OldState = ms;
            View = Matrix.CreateLookAt(Position, Position + Forward, Up);

            AudioManager.MoveListener(View);

            Matrix viewProjection = View * Projection;
            Frustum = new BoundingFrustum(viewProjection);
        }


        public override void Yaw(float radians)
        {
            Forward.Normalize();
            Forward = Vector3.Transform(Forward, Matrix.CreateFromAxisAngle(Up, radians));

        }

        public void Pitch(float radians)
        {
            Forward.Normalize();
            var left = Vector3.Cross(Up, Forward);
            left.Normalize();

            Forward = Vector3.Transform(Forward, Matrix.CreateFromAxisAngle(left, radians));
            Up = Vector3.Transform(Up, Matrix.CreateFromAxisAngle(left, radians));
        }

        public void Roll(float radians)
        {
            Up.Normalize();
            var left = Vector3.Cross(Up, Forward);
            left.Normalize();

            Up = Vector3.Transform(Up, Matrix.CreateFromAxisAngle(Forward, radians));
        }

        public override void SetPosition(Vector3 pos)
        {
            Instance.Position = pos;
        }

        public static void SetForward(Vector3 dir)
        {
            Instance.Forward = dir;
        }

        public override Matrix GetWorld()
        {
            return Matrix.CreateTranslation(Position);
        }
    }
}
