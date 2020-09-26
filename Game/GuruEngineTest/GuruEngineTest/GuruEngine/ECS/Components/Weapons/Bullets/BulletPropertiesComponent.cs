using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

using GuruEngine.Helpers;
using GuruEngine.ECS.Components.World;


//( Class BulletPropertiesComponent )
//( Group Weapons )
//( Type BulletPropertiesComponent )
//( ConnectionList GunComponent Guns )
//( Parameter Float Mass )
//( Parameter Float kaliber )
//( Parameter Float speed )
//( Parameter Float cumulativePower )
//( Parameter Float power )
//( Parameter Float powerRadius )
//( Parameter Float timeLife )
//( Parameter Bool addExplTime )
//( Parameter Bool selfDestruct )
//( Parameter String traceMesh )
//( Parameter String traceTrail )
//( Parameter Int powerType )
//( Parameter Int traceColor )
//( Preset Browning303 0.01066849 4.442131E-005F 835 0 0.0018 0 2.5 false false 20mmRed SmokeBlack_BulletTrail2 0 0xd90000ff )
//( Preset Browning50 0.0485 0.0001209675 870 0 0.0022 0 6.5 false false 20mmRed SmokeBlack_BulletTrail2 0 0xd90000ff )
//( Preset 20mmMg 0.0128 0.00000262122 760 0 0 0 1.8 false false 20mmYellow SmokeBlack_BuletteTrail2 0 0xd200ffff )

namespace GuruEngine.ECS.Components.Weapons.Bullets
{
    public class BulletPropertiesComponent : ECSGameComponent
    {
        public float mass;
        public float kaliber;
        public float speed;
        public float cumulativePower;
        public float power;
        public float powerRadius;
        public float timeLife;
        public bool addExplTime;
        public bool selfDestruct;
        public String traceMesh;
        public String traceTrail;
        public int powerType;
        public int traceColor;

        #region ECS game component interface
        public override ECSGameComponent Clone()
        {
            BulletPropertiesComponent other = new BulletPropertiesComponent();
            other.mass = mass;
            other.kaliber = kaliber;
            other.speed = speed;
            other.cumulativePower = cumulativePower;
            other.power = power;
            other.powerRadius = powerRadius;
            other.timeLife = timeLife;
            other.addExplTime = addExplTime;
            other.selfDestruct = selfDestruct;
            other.traceMesh = traceMesh;
            other.traceTrail = traceTrail;
            other.powerType = powerType;
            other.traceColor = traceColor;
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


                    default:
                        throw new Exception("GameComponent::AngleAnimator:: Unknown direct connection request to " + parts[0]);
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

        public override void ReConnect(GameObject other)
        {
            BulletPropertiesComponent ot = (BulletPropertiesComponent)other.FindGameComponentByName(Name);
            ot.mass = mass;
            ot.kaliber = kaliber;
            ot.speed = speed;
            ot.cumulativePower = cumulativePower;
            ot.power = power;
            ot.powerRadius = powerRadius;
            ot.timeLife = timeLife;
            ot.addExplTime = addExplTime;
            ot.selfDestruct = selfDestruct;
            ot.traceMesh = traceMesh;
            ot.traceTrail = traceTrail;
            ot.powerType = powerType;
            ot.traceColor = traceColor;
        }

        public override void RenderOffscreenRenderTargets()
        {
        }

        public override void SetParameter(string Name, string Value)
        {
            switch (Name)
            {
                case "Mass":
                    mass = float.Parse(Value);
                    break;
                case "kaliber":
                    kaliber = float.Parse(Value);
                    break;
                case "speed":
                    speed = float.Parse(Value);
                    break;
                case "cumulativePower":
                    cumulativePower = float.Parse(Value);
                    break;
                case "power":
                    power = float.Parse(Value);
                    break;
                case "powerRadius":
                    powerRadius = float.Parse(Value);
                    break;
                case "timeLife":
                    timeLife = float.Parse(Value);
                    break;
                case "addExplTime":
                    addExplTime = bool.Parse(Value);
                    break;
                case "selfDestruct":
                    selfDestruct = bool.Parse(Value);
                    break;
                case "traceMesh":
                    traceMesh = Value;
                    break;
                case "traceTrail":
                    traceTrail = Value;
                    break;
                case "powerType":
                    powerType = int.Parse(Value);
                    break;
                case "traceColor":
                    traceColor = int.Parse(Value);
                    break;

            }
        }

        public override void Update(float dt)
        {

        }

        public override void ReplaceComponent(ECSGameComponent old, ECSGameComponent replacement)
        {
        }

        #endregion
    }
}
