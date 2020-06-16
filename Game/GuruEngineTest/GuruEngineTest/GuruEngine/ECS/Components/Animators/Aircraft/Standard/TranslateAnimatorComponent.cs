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

//( Class TranslateAnimatorComponent )
//( Group Animation )
//( Type TranslateAnimatorComponent )
//( Parameter String Mesh )
//( Parameter String Control )
//( Parameter Float Scale )
//( Parameter Int Plane )

namespace GuruEngine.ECS.Components.Animators.Aircraft.Standard
{
    public class TranslateAnimatorComponent : ECSGameComponent
    {

        MultiMeshComponent HostL;
        AircraftStateComponent State;

        public float Scale;
        public String Target;
        public int Plane;
        public String ControlVariable;

        #region ECS Game component methods
        public override ECSGameComponent Clone()
        {
            TranslateAnimatorComponent other = new TranslateAnimatorComponent();
            other.Target = Target;
            other.Scale = Scale;
            other.Plane = Plane;
            other.ControlVariable = ControlVariable;

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
            HostL = (MultiMeshComponent)Parent.FindGameComponentByName(Target);
 
            State = (AircraftStateComponent)Parent.FindGameComponentByName("AircraftStateComponent_1");
        }

        public override void ReConnect(GameObject other)
        {
            TranslateAnimatorComponent otherTank = (TranslateAnimatorComponent)other.FindGameComponentByName(Name);          
            otherTank.Target = Target;
            otherTank.Scale = Scale;
            otherTank.Plane = Plane;
            otherTank.ControlVariable = ControlVariable;
        }

        public override void RenderOffscreenRenderTargets()
        {
        }

        public override void SetParameter(string Name, string Value)
        {
            if (Name == "Mesh")
            {
                Target = Value;
            }
            if (Name == "Control")
            {
                ControlVariable = Value;
            }
            if (Name == "Scale")
            {
                Scale = float.Parse(Value);
            }
            if (Name == "Plane")
            {
                Plane = int.Parse(Value);
            }
        }

        public override void Update(float dt)
        {
            double Vator = State.GetVar(ControlVariable, 0);
            Vator *= Scale;

            Matrix m = Matrix.Identity;
            switch (Plane)
            {
                case 1:
                    m = Matrix.CreateTranslation((float)Vator, 0, 0);
                    break;
                case 2:
                    m = Matrix.CreateTranslation(0,(float)Vator, 0);
                    break;
                case 4:
                    m = Matrix.CreateTranslation(0,0,(float)Vator);
                    break;

                case 3:
                    m = Matrix.CreateTranslation((float)Vator, (float)Vator, 0);
                    break;
                case 5:
                    m = Matrix.CreateTranslation((float)Vator, 0, (float)Vator);
                    break;

                case 6:
                    m = Matrix.CreateTranslation(0,(float)Vator, (float)Vator);
                    break;

                case 7:
                    m = Matrix.CreateTranslation((float)Vator, (float)Vator, (float)Vator);
                    break;
            }
            HostL.MatrixAnimate(m);
        }
        #endregion

    }
}
