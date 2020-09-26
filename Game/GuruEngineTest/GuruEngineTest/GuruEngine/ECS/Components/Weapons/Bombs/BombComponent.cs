using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using GuruEngine.Helpers;
using GuruEngine.ECS.Components.World;
using GuruEngine.ECS.Components.Weapons.Bullets;

//( Class BombComponent )
//( Group Weapons )
//( Type BombComponent )
//( Parameter String Hook )
//( Parameter String Mesh )
//( Parameter String Sound )
//( Parameter Int PowerType )
//( Parameter Float Power )
//( Parameter Float Kaliber )
//( Parameter Float Mass )
//( Parameter Float ArmTime )
//( Parameter Float ExplosionDelay )
//( Parameter Float DragCoefficent )
//( Parameter Float ReferenceArea )
//( Parameter Bool RotationFins )
//( Preset SD500 ADDTHIS arms/SD-500 standard_bomb 0 200 0.4445 535 1 0 0.295 0.155179 false )

namespace GuruEngine.ECS.Components.Weapons.Bombs
{
    public class BombComponent : ECSGameComponent
    {
        public String Hook;
        public String Mesh;
        public String Sound;
        public int PowerType;
        public float Power;
        public float Kaliber;
        public float Mass;
        public float ArmTime;
        public float ExplosionDelay;
        public float DragCoefficent;
        public float ReferenceArea;                     // Metres squared
        public bool RotationFins;

        #region ECS game component interface
        public override ECSGameComponent Clone()
        {
            BombComponent other = new BombComponent();
            other.Hook = Hook;
            other.Mesh = Mesh;
            other.Sound = Sound;
            other.PowerType = PowerType;
            other.Power = Power;
            other.Kaliber = Kaliber;
            other.Mass = Mass;
            other.ArmTime = ArmTime;
            other.ExplosionDelay = ExplosionDelay;
            other.DragCoefficent = DragCoefficent;
            other.ReferenceArea = ReferenceArea;
            other.RotationFins = RotationFins;
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
                string[] objects = parts[1].Split(':');
                switch (parts[0])
                {
                    case "Root":
                        {
                            Parent = GameObjectManager.Instance.FindGameObjectByName(objects[0]);
                        }
                        break;


                    default:
                        throw new Exception("GameComponent::BombComponent:: Unknown direct connection request to " + parts[0]);
                }
            }
        }

        public override void DisConnect()
        {
            Parent = null;
            UpdateStage = 99;
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

        }

        public override void ReplaceComponent(ECSGameComponent old, ECSGameComponent replacement)
        {
        }

        public override void ReConnect(GameObject other)
        {
            BombComponent ot = (BombComponent)other.FindGameComponentByName(Name);
            ot.Hook = Hook;
            ot.Mesh = Mesh;
            ot.Sound = Sound;
            ot.PowerType = PowerType;
            ot.Power = Power;
            ot.Kaliber = Kaliber;
            ot.Mass = Mass;
            ot.ArmTime = ArmTime;
            ot.ExplosionDelay = ExplosionDelay;
            ot.DragCoefficent = DragCoefficent;
            ot.ReferenceArea = ReferenceArea;
            ot.RotationFins = RotationFins;
        }

        public override void RenderOffscreenRenderTargets()
        {
        }

        public override void SetParameter(string Name, string Value)
        {
            switch (Name)
            {
                case "Hook":
                    Hook = Value;
                    break;
                case "Mesh":
                    Mesh = Value;
                    break;
                case "Sound":
                    Sound = Value;
                    break;
                case "PowerType":
                    PowerType = int.Parse(Value);
                    break;
                case "Power":
                    Power = float.Parse(Value);
                    break;
                case "Kaliber":
                    Kaliber = float.Parse(Value);
                    break;
                case "Mass":
                    Mass = float.Parse(Value);
                    break;
                case "ArmTime":
                    ArmTime = float.Parse(Value);
                    break;
                case "ExplosionDelay":
                    ExplosionDelay = float.Parse(Value);
                    break;
                case "DragCoefficent":
                    DragCoefficent = float.Parse(Value);
                    break;
                case "ReferenceArea":
                    ReferenceArea = float.Parse(Value);
                    break;
                case "RotationFins":
                    RotationFins = bool.Parse(Value);
                    break;

            }
        }

        public override void Update(float dt)
        {

        }

        #endregion
    }
}

