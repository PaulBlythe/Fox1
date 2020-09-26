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

//( Class RudderAnimatorComponent )
//( Group Animation )
//( Type RudderAnimatorComponent )
//( Parameter Int RudderNumber )
//( Parameter Float Scale )

namespace GuruEngine.ECS.Components.Animators.Aircraft.Standard
{
    public class RudderAnimatorComponent : ECSGameComponent
    {
        #region Identifiers
        String RudderPositionVar;
        #endregion

        MultiMeshComponent HostL;
        AircraftStateComponent State;

        public float Scale;
        public int RudderNumber;

        #region ECS Game component methods
        public override ECSGameComponent Clone()
        {
            RudderAnimatorComponent other = new RudderAnimatorComponent();
            other.RudderNumber = RudderNumber;
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
            RudderPositionVar = "RudderPosition";


            if (RudderNumber == 0)
            {
                HostL = (MultiMeshComponent)Parent.FindGameComponentByName("Rudder_D0");
 
            }
            else
            {
                String t = String.Format("Rudder{0}_D0", RudderNumber);
                HostL = (MultiMeshComponent)Parent.FindGameComponentByName(t);
            }

            State = (AircraftStateComponent)Parent.FindGameComponentByName("AircraftStateComponent_1");
        }

        public override void ReplaceComponent(ECSGameComponent old, ECSGameComponent replacement)
        {
            if (old is AircraftStateComponent)
            {
                State = (AircraftStateComponent)replacement;
            }
        }

        public override void ReConnect(GameObject other)
        {
            RudderAnimatorComponent otherTank = (RudderAnimatorComponent)other.FindGameComponentByName(Name);
            otherTank.RudderNumber = RudderNumber;
            otherTank.Scale = Scale;
        }

        public override void RenderOffscreenRenderTargets()
        {
        }

        public override void SetParameter(string Name, string Value)
        {
            if (Name == "RudderNumber")
            {
                RudderNumber = int.Parse(Value);
            }
            if (Name == "Scale")
            {
                Scale = float.Parse(Value);
            }
        }

        public override void Update(float dt)
        {
            double Vator = State.GetVar(RudderPositionVar, 0);
            Vator *= Scale;

            Matrix m = Matrix.CreateRotationY(MathHelper.ToRadians((float)Vator));
            HostL.MatrixAnimate(m);
        }
        #endregion

    }
}
