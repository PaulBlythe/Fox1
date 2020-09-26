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

//( Class CVTAnimatorComponent )
//( Group Animation )
//( Type CVTAnimatorComponent )
//( Parameter String Mesh )
//( Parameter String Control )
//( Parameter Float Minimum )
//( Parameter Float Maximum )
//( Parameter Float Start )
//( Parameter Float Finish )
//( Parameter Int Flags )

namespace GuruEngine.ECS.Components.Animators.Aircraft.Standard
{
    public class CVTAnimatorComponent : ECSGameComponent
    {
        MultiMeshComponent Host;
        AircraftStateComponent State;

        public float Minimum;
        public float Maximum;
        public float Start;
        public float Finish;
        public int Flags = 0;

        public String TargetMesh;
        public String ControlValue;


        #region ECS Game component methods
        public override ECSGameComponent Clone()
        {
            CVTAnimatorComponent other = new CVTAnimatorComponent();
            other.Maximum = Maximum;
            other.Minimum = Minimum;
            other.Start = Start;
            other.Finish = Finish;
            other.TargetMesh = TargetMesh;
            other.ControlValue = ControlValue;
            other.Flags = Flags;
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

        public override void ReplaceComponent(ECSGameComponent old, ECSGameComponent replacement)
        {
            if (old is AircraftStateComponent)
            {
                State = (AircraftStateComponent)replacement;
            }
        }

        public override void Load(ContentManager content)
        {
            Host = (MultiMeshComponent)Parent.FindGameComponentByName(TargetMesh);
            State = (AircraftStateComponent)Parent.FindGameComponentByName("AircraftStateComponent_1");
        }

        public override void ReConnect(GameObject other)
        {
            CVTAnimatorComponent otherTank = (CVTAnimatorComponent)other.FindGameComponentByName(Name);
            otherTank.Maximum = Maximum;
            otherTank.Minimum = Minimum;
            otherTank.Start = Start;
            otherTank.Finish = Finish;
            otherTank.TargetMesh = TargetMesh;
            otherTank.ControlValue = ControlValue;
            otherTank.Flags = Flags;
        }

        public override void RenderOffscreenRenderTargets()
        {
        }

        public override void SetParameter(string Name, string Value)
        {
            if (Name == "Maximum")
            {
                Maximum = float.Parse(Value);
            }
            if (Name == "Minimum")
            {
                Minimum = float.Parse(Value);
            }
            if (Name == "Start")
            {
                Start = float.Parse(Value);
            }
            if (Name == "Finish")
            {
                Finish = float.Parse(Value);
            }
            if (Name == "Flags")
            {
                Flags = int.Parse(Value);
            }
            if (Name == "TargetMesh")
            {
                TargetMesh = Value;
            }
            if (Name == "ControlValue")
            {
                ControlValue = Value;
            }
        }

        public override void Update(float dt)
        {
            double Vator = State.GetVar(ControlValue, 0);
            
            Vator = Maths.MathUtils.Cvt((float)Vator, Minimum, Maximum, Start, Finish);

            switch (Flags)
            {
                case 1:             
                    {
                        Matrix m = Matrix.CreateRotationX(MathHelper.ToRadians((float)-Vator));
                        Host.MatrixAnimate(m);
                    }
                    break;
                case 2:
                    {
                        Matrix m = Matrix.CreateRotationZ(MathHelper.ToRadians((float)Vator));
                        Host.MatrixAnimate(m);
                    }
                    break;
                default:
                    {
                        Matrix m = Matrix.CreateRotationY(MathHelper.ToRadians((float)Vator));
                        Host.MatrixAnimate(m);
                    }
                    break;
            }
        }
        #endregion

    }
}
