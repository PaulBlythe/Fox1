using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using GuruEngine.ECS;
using GuruEngine.ECS.Components.Mesh;
using GuruEngine.ECS.Components.World;

//( Class SmoothedAngleAnimatorComponent )
//( Group Animation )
//( Type SmoothedAngleAnimatorComponent )
//( Parameter String Mesh )
//( Parameter String Control )
//( Parameter Float Scale )
//( Parameter Float Smoothing )
//( Parameter Int Plane )


namespace GuruEngine.ECS.Components.Animators.Aircraft.Standard
{
    class SmoothedAngleAnimatorComponent : ECSGameComponent
    {
        MultiMeshComponent HostL;
        AircraftStateComponent State;

        public float Scale;
        public int Plane;
        public float Smoothing;
        public String Mesh;
        public String Control;

        float Angle = 0;

        #region ECS Game component methods
        public override ECSGameComponent Clone()
        {
            SmoothedAngleAnimatorComponent other = new SmoothedAngleAnimatorComponent();
            other.Plane = Plane;
            other.Mesh = Mesh;
            other.Control = Control;
            other.Scale = Scale;
            other.Smoothing = Smoothing;
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
            HostL = null;
            State = null;
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
            HostL = (MultiMeshComponent)Parent.FindGameComponentByName(Mesh);
            State = (AircraftStateComponent)Parent.FindGameComponentByName("AircraftStateComponent_1");
        }

        public override void ReConnect(GameObject other)
        {
            SmoothedAngleAnimatorComponent otherTank = (SmoothedAngleAnimatorComponent)other.FindGameComponentByName(Name);
            otherTank.Plane = Plane;
            otherTank.Mesh = Mesh;
            otherTank.Control = Control;
            otherTank.Scale = Scale;
            otherTank.Smoothing = Smoothing;
        }

        public override void RenderOffscreenRenderTargets()
        {
        }

        public override void SetParameter(string Name, string Value)
        {
            if (Name == "Plane")
            {
                Plane = int.Parse(Value);
            }
            if (Name == "Scale")
            {
                Scale = float.Parse(Value);
            }
            if (Name == "Smoothing")
            {
                Smoothing = float.Parse(Value);
            }
            if (Name == "Mesh")
            {
                Mesh = Value;
            }
            if (Name == "Control")
            {
                Control = Value;
            }
        }

        public override void Update(float dt)
        {
            
            double Vator = State.GetVar(Control, 0);

            Angle = (float)((1 - Smoothing) * Angle + (Smoothing * Vator));
            
            Vator = Angle * Scale;

            Matrix m = Matrix.Identity;
            switch (Plane)
            {
                case 1:
                    m = Matrix.CreateRotationX(MathHelper.ToRadians((float)Vator));
                    break;
                case 2:
                    m = Matrix.CreateRotationY(MathHelper.ToRadians((float)Vator));
                    break;
                case 4:
                    m = Matrix.CreateRotationZ(MathHelper.ToRadians((float)Vator));
                    break;

                case 3:
                    m = Matrix.CreateFromYawPitchRoll(MathHelper.ToRadians((float)Vator), MathHelper.ToRadians((float)Vator), 0);
                    break;
                case 5:
                    m = Matrix.CreateFromYawPitchRoll(0, MathHelper.ToRadians((float)Vator), MathHelper.ToRadians((float)Vator));
                    break;
                case 6:
                    m = Matrix.CreateFromYawPitchRoll(MathHelper.ToRadians((float)Vator), 0, MathHelper.ToRadians((float)Vator));
                    break;
                case 7:
                    m = Matrix.CreateFromYawPitchRoll(MathHelper.ToRadians((float)Vator), MathHelper.ToRadians((float)Vator), MathHelper.ToRadians((float)Vator));
                    break;


                case 14:
                    m = Matrix.CreateRotationZ(MathHelper.ToRadians((float)Vator));
                    break;
            }

            HostL.MatrixAnimate(m);
        }
        #endregion

    }
}
