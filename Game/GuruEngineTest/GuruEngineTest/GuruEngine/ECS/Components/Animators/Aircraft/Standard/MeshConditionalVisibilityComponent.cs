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

//( Class MeshConditionalVisibilityComponent )
//( Group Animation )
//( Type MeshConditionalVisibilityComponent )
//( Parameter String Mesh )
//( Parameter String Control )
//( Parameter Float Value )
//( Parameter String Compare )


namespace GuruEngine.ECS.Components.Animators.Aircraft.Standard
{
    public class MeshConditionalVisibilityComponent : ECSGameComponent
    {
        MultiMeshComponent Host;
        AircraftStateComponent State;

        public float Value;
        public String Cmp;
        public String TargetMesh;
        public String ControlValue;


        #region ECS Game component methods
        public override ECSGameComponent Clone()
        {
            MeshConditionalVisibilityComponent other = new MeshConditionalVisibilityComponent();
            other.Value = Value;
            other.Cmp = Cmp;
            other.TargetMesh = TargetMesh;
            other.ControlValue = ControlValue;

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
            MeshConditionalVisibilityComponent otherTank = (MeshConditionalVisibilityComponent)other.FindGameComponentByName(Name);
            otherTank.Value = Value;
            otherTank.ControlValue = ControlValue;
            otherTank.Cmp = Cmp;
            otherTank.TargetMesh = TargetMesh;
            
        }

        public override void RenderOffscreenRenderTargets()
        {
        }

        public override void SetParameter(string Name, string inValue)
        {
            if (Name == "Value")
            {
                Value = float.Parse(inValue);
            }
            if (Name == "Compare")
            {
                Cmp = inValue;
            }
            if (Name == "Mesh")
            {
                TargetMesh = inValue;
            }
            if (Name == "ControlValue")
            {
                ControlValue = inValue;
            }
            
        }

        public override void Update(float dt)
        {
            double Vator = State.GetVar(ControlValue, 0);
            bool Hidden = false;
            switch (Cmp)
            {
                case "<":
                    Hidden = (Vator < Value);
                    break;
                case ">":
                    Hidden = (Vator > Value);
                    break;
                case "<=":
                    Hidden = (Vator <= Value);
                    break;
                case ">=":
                    Hidden = (Vator >= Value);
                    break;
                case "==":
                    Hidden = (Vator == Value);
                    break;
                case "!=":
                    Hidden = (Vator != Value);
                    break;
            }
            Host.Hidden = Hidden;
            
        }
        #endregion

    }
}
