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

//( Class ElevatorAnimatorComponent )
//( Group Animation )
//( Type ElevatorAnimatorComponent )
//( Parameter Int ElevatorNumber )
//( Parameter Float Scale )

namespace GuruEngine.ECS.Components.Animators.Aircraft.Standard
{
    class ElevatorAnimatorComponent : ECSGameComponent
    {
        #region Identifiers
        String ElevatorPositionVar;
        #endregion

        MultiMeshComponent HostL;
        MultiMeshComponent HostR;
        AircraftStateComponent State;

        public float Scale;
        public int ElevatorNumber;



        #region ECS Game component methods
        public override ECSGameComponent Clone()
        {
            ElevatorAnimatorComponent other = new ElevatorAnimatorComponent();
            other.ElevatorNumber = ElevatorNumber;
            other.Scale = Scale;

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
            HostR = null;
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
            ElevatorPositionVar = "ElevatorPosition";

            
            if (ElevatorNumber ==0)
            {
                HostL = (MultiMeshComponent)Parent.FindGameComponentByName("VatorL_D0");
                HostR = (MultiMeshComponent)Parent.FindGameComponentByName("VatorR_D0");
            }
            else
            {
                String t = String.Format("VatorL{0}_D0", ElevatorNumber);
                HostL = (MultiMeshComponent)Parent.FindGameComponentByName(t);
                t = String.Format("VatorR{0}_D0", ElevatorNumber);
                HostR = (MultiMeshComponent)Parent.FindGameComponentByName(t);
            }
           
            State = (AircraftStateComponent)Parent.FindGameComponentByName("AircraftStateComponent_1");
        }

        public override void ReConnect(GameObject other)
        {
            ElevatorAnimatorComponent otherTank = (ElevatorAnimatorComponent)other.FindGameComponentByName(Name);
            otherTank.ElevatorNumber = ElevatorNumber;
            otherTank.Scale = Scale;
        }

        public override void RenderOffscreenRenderTargets()
        {
        }

        public override void SetParameter(string Name, string Value)
        {
            if (Name == "ElevatorNumber")
            {
                ElevatorNumber = int.Parse(Value);
            }
            if (Name == "Scale")
            {
                Scale = float.Parse(Value);
            }
        }

        public override void Update(float dt)
        {
            double Vator = State.GetVar(ElevatorPositionVar, 0);
            Vator *= Scale;

            Matrix m = Matrix.CreateRotationY(MathHelper.ToRadians((float)Vator));
            HostL.MatrixAnimate(m);
            HostR.MatrixAnimate(m);
        }
        #endregion

    }
}
