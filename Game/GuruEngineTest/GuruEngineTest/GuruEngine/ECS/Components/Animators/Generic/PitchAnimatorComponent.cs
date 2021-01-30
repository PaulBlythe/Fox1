using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using GuruEngine.ECS;
using GuruEngine.ECS.Components.Mesh;
using GuruEngine.ECS.Components.World;
using GuruEngine.ECS.Components.Artillery;

namespace GuruEngine.ECS.Components.Animators.Generic
{
    public class PitchAnimatorComponent : ECSGameComponent
    {
        public float Scale;
        public String Control;
        public String Mesh;

        MultiMeshComponent Host;
        ECSGameComponent State;


        #region ECS Game component methods
        public override ECSGameComponent Clone()
        {
            PitchAnimatorComponent other = new PitchAnimatorComponent();
            other.Scale = Scale;
            other.Control = Control;
            other.Mesh = Mesh;
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
                State = replacement;
            }
        }

        public override void Load(ContentManager content)
        {
              Host = (MultiMeshComponent)Parent.FindGameComponentByName(Mesh);
 
              State = Parent.FindGameComponentByName("AircraftStateComponent_1");
              if (State == null)
                State = Parent.FindGameComponentByName("AntiAircraftArtilleryComponent_1");
        }

        public override void ReConnect(GameObject other)
        {
            PitchAnimatorComponent otherTank = (PitchAnimatorComponent)other.FindGameComponentByName(Name);
            otherTank.Mesh = Mesh;
            otherTank.Scale = Scale;
            otherTank.Control = Control;

        }

        public override void RenderOffscreenRenderTargets()
        {
        }

        public override void SetParameter(string Name, string Value)
        {
            if (Name == "Scale")
            {
                Scale = float.Parse(Value);
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
            double Vator = 0;
            if (State is AircraftStateComponent)
                Vator = ((AircraftStateComponent)State).GetVar(Control, 0);
            if (State is AntiAircraftArtilleryComponent)
                Vator = ((AntiAircraftArtilleryComponent)State).DoubleVariables[Control];

            Vator *= Scale;

            Matrix m = Matrix.CreateRotationZ(MathHelper.ToRadians((float)Vator));
            Host.MatrixAnimate(m);

        }
        #endregion
    }
}
