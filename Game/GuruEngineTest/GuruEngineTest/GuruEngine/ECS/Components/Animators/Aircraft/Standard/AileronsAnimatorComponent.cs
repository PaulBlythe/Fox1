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

//( Class AileronsAnimatorComponent )
//( Group Animation )
//( Type AileronsAnimatorComponent )
//( Parameter Int AileronNumber )
//( Parameter Float Scale )

namespace GuruEngine.ECS.Components.Animators.Aircraft.Standard
{
    public class AileronsAnimatorComponent : ECSGameComponent
    {
        #region Identifiers
        String AileronPositionVar;
        #endregion

        MultiMeshComponent HostL;
        MultiMeshComponent HostR;
        AircraftStateComponent State;

        public float Scale;
        public int Number;



        #region ECS Game component methods
        public override ECSGameComponent Clone()
        {
            AileronsAnimatorComponent other = new AileronsAnimatorComponent();
            other.Number = Number;
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
            AileronPositionVar = "AileronPosition";


            if (Number == 0)
            {
                HostL = (MultiMeshComponent)Parent.FindGameComponentByName("AroneL_D0");
                HostR = (MultiMeshComponent)Parent.FindGameComponentByName("AroneR_D0");
            }
            else
            {
                String t = String.Format("AroneL{0}_D0", Number);
                HostL = (MultiMeshComponent)Parent.FindGameComponentByName(t);
                t = String.Format("AroneR{0}_D0", Number);
                HostR = (MultiMeshComponent)Parent.FindGameComponentByName(t);
            }

            State = (AircraftStateComponent)Parent.FindGameComponentByName("AircraftStateComponent_1");
        }

        public override void ReConnect(GameObject other)
        {
            AileronsAnimatorComponent otherTank = (AileronsAnimatorComponent)other.FindGameComponentByName(Name);
            otherTank.Number = Number;
            otherTank.Scale = Scale;
        }

        public override void RenderOffscreenRenderTargets()
        {
        }

        public override void SetParameter(string Name, string Value)
        {
            if (Name == "AileronNumber")
            {
                Number = int.Parse(Value);
            }
            if (Name == "Scale")
            {
                Scale = float.Parse(Value);
            }
        }

        public override void Update(float dt)
        {
            double Vator = State.GetVar(AileronPositionVar, 0);
            Vator *= Scale;

            Matrix m = Matrix.CreateRotationY(MathHelper.ToRadians((float)Vator));
            HostL.MatrixAnimate(m);
            HostR.MatrixAnimate(m);
        }
        #endregion

    }
}
