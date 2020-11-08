using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;


namespace GUITestbed.Rendering._3D
{
    public class FixedCamera:Camera
    {
        public FixedCamera(Vector3 Position, Vector3 Target, Vector3 Up, float nearPlane, float farPlane, GraphicsDevice graphicsDevide)
            : base(graphicsDevide, nearPlane, farPlane)
        {
            View = Matrix.CreateLookAt(Position, Target, Up);
            this.Position = Position;
        }

        public override void Update()
        {
            base.Update();
        }
    }
}
