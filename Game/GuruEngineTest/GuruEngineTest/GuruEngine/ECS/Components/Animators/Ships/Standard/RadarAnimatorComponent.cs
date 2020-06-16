using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

using GuruEngine.ECS.Components.Mesh;

//( Class RadarAnimatorComponent )
//( Group Animation )
//( Type RadarAnimatorComponent )
//( Parameter String Mesh )
//( Parameter Float Rate )

namespace GuruEngine.ECS.Components.Animators.Ships.Standard
{
    public class RadarAnimatorComponent : ECSGameComponent
    {
        String Mesh;
        float Rate;
        MultiMeshComponent Host;
        float angle;

        public RadarAnimatorComponent()
        {
            Random r = new Random();
            angle = (float)(r.NextDouble() * Math.PI);
        }

        #region ECS Game component methods
        public override ECSGameComponent Clone()
        {
            RadarAnimatorComponent other = new RadarAnimatorComponent();
            other.Host = Host;
            other.Mesh = Mesh;
            other.Rate = Rate;

            return other;
        }

        public override void Connect(string components, bool isList)
        {
            string[] parts = components.Split('#');
            if (isList)
            {

            }
            else
            {
                string[] objects = parts[2].Split(':');
                switch (parts[0])
                {


                    case "Root":
                        {
                            Parent = GameObjectManager.Instance.FindGameObjectByName(objects[0]);
                        }
                        break;




                }
            }
        }

        public override void DisConnect()
        {
            Host = null;
        }

        public override object GetContainedObject(string type)
        {

            return null;
        }

        public override Texture2D GetOffscreenTexture()
        {
            return null;
        }

        public override void HandleEvent(string evt)
        {


        }

        public override void Load(ContentManager content)
        {

            Host = (MultiMeshComponent)Parent.FindGameComponentByName(Mesh);

        }

        public override void ReConnect(GameObject other)
        {
            RadarAnimatorComponent otherT = (RadarAnimatorComponent)other.FindGameComponentByName(Name);
            otherT.Host = Host;
            otherT.Mesh = Mesh;
            otherT.Rate = Rate;
        }

        public override void RenderOffscreenRenderTargets()
        {
        }

        public override void SetParameter(string Name, string Value)
        {
            if (Name == "Mesh")
            {
                Mesh = Value;
            }
            if (Name == "Rate")
            {
                Rate = float.Parse(Value);
            }
        }

        public override void Update(float dt)
        {
            
            angle += (float)(Rate * dt * 2 * MathHelper.Pi / 60);

            Matrix m = Matrix.CreateRotationZ(angle);
            Host.MatrixAnimate(m);
        }
        #endregion
    }
}
