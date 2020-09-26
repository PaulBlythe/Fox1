using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

using GuruEngine.ECS.Components.Mesh;
using GuruEngine.ECS.Components.World;
using GuruEngine.Algebra;

//( Class ShipGunFOFComponent )
//( Group Naval )
//( Type ShipGunFOFComponent )
//( Connection ShipGunComponent Gun )
//( Parameter Float YawMin )
//( Parameter Float YawMax )
//( Parameter Float PitchMin )
//( Parameter Float PitchMax )
//( Parameter Float BaseHeading )

namespace GuruEngine.ECS.Components.Ships
{
    public class ShipGunFOFComponent: ECSGameComponent
    {
        public float YawMin;
        public float YawMax;
        public float PitchMin;
        public float PitchMax;
        public float BaseHeading;

        ShipGunComponent Gun;

        #region ECS Game component methods
        public override ECSGameComponent Clone()
        {
            ShipGunFOFComponent other = new ShipGunFOFComponent();

            other.Gun = Gun;
            other.YawMax = YawMax;
            other.YawMin = YawMin;
            other.PitchMax = PitchMax;
            other.PitchMin = PitchMin;
            other.BaseHeading = BaseHeading;

            return (ECSGameComponent)other;
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

                    case "Gun":
                        {
                            Gun = (ShipGunComponent)Parent.FindGameComponentByName(objects[0]);
                        }
                        break;


                }
            }
        }

        public override void DisConnect()
        {
            Gun = null;
        }

        public override object GetContainedObject(string type)
        {
            if (type == "Gun")
            {
                return Gun;
            }
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
        }

        public override void Load(ContentManager content)
        {
        }

        public override void ReConnect(GameObject other)
        {
            ShipGunFOFComponent otherT = (ShipGunFOFComponent)other.FindGameComponentByName(Name);
            otherT.Gun = Gun;
            otherT.PitchMax = PitchMax;
            otherT.PitchMin = PitchMin;
            otherT.YawMax = YawMax;
            otherT.YawMin = YawMin;
            otherT.BaseHeading = BaseHeading;
        }

        public override void RenderOffscreenRenderTargets()
        {
        }

        public override void SetParameter(string Name, string Value)
        {
            switch (Name)
            {
                case "YawMin":
                    {
                        YawMin = float.Parse(Value);
                    }
                    break;
                case "YawMax":
                    {
                        YawMax = float.Parse(Value);
                    }
                    break;
                case "PitchMin":
                    {
                        PitchMin = float.Parse(Value);
                    }
                    break;
                case "PitchMax":
                    {
                        PitchMax = float.Parse(Value);
                    }
                    break;
                case "BaseHeading":
                    {
                        BaseHeading = float.Parse(Value);
                    }
                    break;

            }
        }

        /// <summary>
        /// </summary>
        /// <param name="dt"></param>
        public override void Update(float dt)
        {

        }
        #endregion

        /// <summary>
        /// Can this gunsight point at target
        /// </summary>
        /// <param name="yaw">In degrees</param>
        /// <param name="pitch">in Degrees</param>
        /// <returns></returns>
        public bool CanBear(float yaw, float pitch)
        {
            if (pitch < PitchMin)
                return false;
            if (pitch > PitchMax)
                return false;

            float dw = YawMax - YawMin;
            if (dw>0)
            {
                if (yaw < YawMin)
                    return false;
                if (yaw > YawMax)
                    return false;
            }
            else
            {
                if (yaw < YawMax)
                    return false;
                if (yaw > YawMin)
                    return false;
            }


            return true;
        }
    }
}
