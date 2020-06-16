using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;


//( Class PowerManagementComponent )
//( Group AircraftSystem )
//( Type PowerManagement )
//( ConnectionList InputEventComponent InputEvents )
//( Parameter Float Battery )
//( Parameter Float BatteryLife )
//( Parameter Bool BatteryRechargeable )

namespace GuruEngine.ECS.Components.AircraftSystems.General
{
    public class PowerManagementComponent: ECSGameComponent
    {
        float Damage;
        float AvailablePower;
        float AuxPower;
        float ConsumedPower;
        float BatteryPower;
        float BatteryTime;
        float BatteryLife;
        bool BatteryRechargeable;

        Random rand = new Random();

        public PowerManagementComponent()
        {
            Damage = 0;
            AvailablePower = 0;
            AuxPower = 0;
            ConsumedPower = 0;
            BatteryPower = 0;
            BatteryTime = 0;
            BatteryLife = 0;
            BatteryRechargeable = false;
        }

        #region ECS Game component methods
        public override ECSGameComponent Clone()
        {
            PowerManagementComponent other = new PowerManagementComponent();
            other.Damage = Damage;
            other.AvailablePower = AvailablePower;
            other.AuxPower = AuxPower;
            other.ConsumedPower = ConsumedPower;
            other.BatteryPower = BatteryPower;
            other.BatteryTime = BatteryTime;
            other.BatteryLife = BatteryLife;
            other.BatteryRechargeable = BatteryRechargeable;
            return other;
        }

        public override void Connect(string components, bool isList)
        {
            string[] parts = components.Split('#');
            if (isList)
            {
                switch (parts[1])
                {
                    case "InputEvents":
                        {
                            // don't need to do anything with input event components
                        }
                        break;
                    default:
                        throw new Exception("GameComponent::FuelTank:: Unknown list connection request to " + parts[0]);
                }
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
                        throw new Exception("GameComponent::PowerManagementComponent:: Unknown direct connection request to " + parts[0]);
                }
            }
        }

        public override void DisConnect()
        {
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
            String[] parts = evt.Split(':');

            switch (parts[0])
            {
                case "AuxiliaryPowerOn":
                    {
                        AuxPower = 50;
                    }
                    break;

                case "AuxiliaryPowerOff":
                    {
                        AuxPower = 0;
                    }
                    break;

                case "Damage":
                    {
                        Damage += float.Parse(parts[1]);
                    }
                    break;
            }
        }

        public override void Load(ContentManager content)
        {

        }

        public override void ReConnect(GameObject other)
        {
            PowerManagementComponent otherPower = (PowerManagementComponent)other.FindGameComponentByName(Name);
            otherPower.AvailablePower = AvailablePower;
            otherPower.Damage = Damage;
            otherPower.AuxPower = AuxPower;
            otherPower.BatteryTime = BatteryTime;
            otherPower.BatteryPower = BatteryPower;
            otherPower.ConsumedPower = ConsumedPower;
            otherPower.BatteryLife = BatteryLife;
            otherPower.BatteryRechargeable = BatteryRechargeable;
        }

        public override void RenderOffscreenRenderTargets()
        {
        }

        public override void SetParameter(string Name, string Value)
        {
            switch (Name)
            {
                case "Battery":
                    {
                        BatteryPower = float.Parse(Value);
                    }
                    break;
                case "BatteryLife":
                    {
                        BatteryLife = float.Parse(Value);
                    }
                    break;
                case "BatteryRechargeable":
                    {
                        BatteryRechargeable = bool.Parse(Value);
                    }
                    break;

            }
        }

        public override void Update(float dt)
        {
            List<ECSGameComponent> engines = Parent.FindGameComponentByType<EngineComponent>();

            float epower = 0;
            foreach (ECSGameComponent ec in engines)
            {
                EngineComponent engine = (EngineComponent)ec;
                epower += engine.Engine.GetPowerProduced();
            }
            AvailablePower = AuxPower + epower;
            if (AvailablePower < 1)
            {
                if (BatteryTime < BatteryLife)
                {
                    // consume battery power
                    AvailablePower += BatteryPower;
                    BatteryTime += dt;
                }
            }else
            {
                if (BatteryRechargeable)
                {
                    if (BatteryTime < BatteryLife)
                    {
                        // recharge battery
                        BatteryTime += dt;
                    }
                }
            }
            AvailablePower *= 1000;         // convert from KVA to VA (kilowatts to watts)
            ConsumedPower = 0;

            if (Damage>1)
            {
                if (rand.Next(0,100) < Damage)
                {
                    AvailablePower = 0;
                }
            }
        }
        #endregion

        #region Power Management methods
        /// <summary>
        /// Request current from the electrical system
        /// </summary>
        /// <param name="limit">Amps required</param>
        /// <returns></returns>
        public bool HasPower(float limit)
        {
            limit /= 14;            // assumes 14 V bus

            float request = ConsumedPower + limit;
            if (request<AvailablePower)
            {
                ConsumedPower += limit;
                return true;
            }
            return false;
        }
        #endregion
    }
}
