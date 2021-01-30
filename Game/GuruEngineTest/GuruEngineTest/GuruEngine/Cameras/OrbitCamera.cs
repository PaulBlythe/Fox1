using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace GuruEngine.Cameras
{
    public class OrbitCamera:Camera
    {
        Matrix World;
        float Radius = 20;
        float Angle = 0;
        float VAngle = 30;
        bool grabbed = false;
        Vector2 gpos = new Vector2();
        int click_count = 0;

        public OrbitCamera(float aspectratio)
        {
            Projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(60), aspectratio, 0.15f, 60000.0f);
            Position = new Vector3(0, 10, -Radius);
            View = Matrix.CreateLookAt(Position, Vector3.Zero, Vector3.Up);
            World = Matrix.CreateTranslation(Position);
        }
        public override Matrix GetWorld()
        {
            return World;
        }

        public override void SetPosition(Vector3 location)
        {
            
        }

        public override void Update(float gt)
        {
            MouseState ms = Mouse.GetState();
            if (ms.RightButton == ButtonState.Pressed)
            {
                if (grabbed)
                {
                    float dx = ms.X - gpos.X;
                    float dy = ms.Y - gpos.Y;

                    Angle += dx * gt;
                    VAngle += dy * gt;
                    VAngle = Math.Max(VAngle, 0);
                    VAngle = Math.Min(VAngle, 60);
                }
                else
                {
                    grabbed = true;
                    gpos.X = ms.X;
                    gpos.Y = ms.Y;
                }
            }
            else
            {
                grabbed = false;
            }

            float dz = ms.ScrollWheelValue - click_count;
            Radius -= dz * gt;
            click_count = ms.ScrollWheelValue;

            float cy = (float)(Radius * Math.Cos(MathHelper.ToRadians(VAngle)));

            Vector3 t = new Vector3(0, cy, -Radius);
            Position = Vector3.Transform(t, Matrix.CreateRotationY(MathHelper.ToRadians(Angle)));
            View = Matrix.CreateLookAt(Position, Vector3.Zero, Vector3.Up);
            World = Matrix.CreateTranslation(Position);

            Audio.AudioManager.MoveListener(World);
        }

        public override void Yaw(float angle)
        {
            
        }
    }
}
