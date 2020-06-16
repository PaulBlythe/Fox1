using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

using GuruEngine.ECS;
using GuruEngine.Physics.World;
using GuruEngine.Physics.Collision;
using GuruEngine.ECS.Components.World;
using GuruEngine.AI.Scripting;
using GuruEngine.World;

//( Class AircraftStateComponent )
//( Group Flight )
//( Type AircraftStateComponent )
//( Parameter String Script )

namespace GuruEngine.ECS.Components
{
    public class AircraftStateComponent: ECSGameComponent
    {
        public double IndicatedAirSpeed;               // in KNOTS
        public double AmbientAirTemperature;
        public double FuelCalorificValue = 47.3e6;
        public AtmosphericModelResults Atmosphere;

        public double Cp_air = 1005;                  // Specific heat (constant pressure) J/Kg/K
        public double Cp_fuel = 1700;
        public double fttom = 0.3048;
        public double R_air = 287.3;                  // Gas constant for air J/Kg/K
        public double inhgtopa = 3386.38;
        public double psftoinhg = 0.014138;
        public double psftopa = 47.88;
        public double hptoftlbssec = 550.0;

        public Dictionary<String, double> DoubleVariables = new Dictionary<string, double>();
        public Dictionary<String, bool> BoolVariables = new Dictionary<string, bool>();

        public String ScriptName = "";
        public Script script = null;

        public AircraftStateComponent()
        {
            UpdateStage = 4;
        }

        #region Debug only variables 
        float time = 0;
        #endregion

        #region ECS Game component methods
        public override ECSGameComponent Clone()
        {
            AircraftStateComponent other = new AircraftStateComponent();
            other.ScriptName = ScriptName;
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

            
        }

        public override void Load(ContentManager content)
        {
            DoubleVariables.Add("ElevatorPosition", 0);
            DoubleVariables.Add("RudderPosition", 0);
            DoubleVariables.Add("AileronPosition", 0);
            DoubleVariables.Add("FlapPosition", 0);
            DoubleVariables.Add("GearPosition", 0);
            DoubleVariables.Add("CockpitPosition", 0);
            DoubleVariables.Add("RadiatorPosition", 0);
            DoubleVariables.Add("BombBayPosition", 0);
            DoubleVariables.Add("Altitude", 0);

            if (ScriptName !="")
            {
                script = GuruEngine.World.World.Instance.scriptManager.GetScript(ScriptClasses.Aircraft, ScriptName);
                script.Host = this;
                script.CreateObjects();
            }
        }

        public override void ReConnect(GameObject other)
        {
            AircraftStateComponent otherTank = (AircraftStateComponent)other.FindGameComponentByName(Name);
            otherTank.ScriptName = ScriptName;
        }

        public override void RenderOffscreenRenderTargets()
        {
        }

        public override void SetParameter(string Name, string Value)
        {
            switch (Name)
            {
                case "ScriptName":
                    ScriptName = Value;
                    break;
            }
        }

        public override void Update(float dt)
        {
            time += 0.016667f;

            #region Debug only code

            //double ep = Math.Sin(time);
            //DoubleVariables["ElevatorPosition"] = ep;
            //DoubleVariables["AileronPosition"] = ep;
            //
            //ep = Math.Cos(time);
            //DoubleVariables["RudderPosition"] = ep;
            //DoubleVariables["FlapPosition"] = Math.Abs(ep);
            //DoubleVariables["CockpitPosition"] = Math.Abs(ep);
            //
            //
            //ep = Math.Sin(time * 0.1f);
            //DoubleVariables["GearPosition"] = Math.Abs(ep);
            //DoubleVariables["BombBayPosition"] = Math.Abs(ep);
            //
            //if (Math.Abs(ep)>0.5)
            //{
            //    DoubleVariables["RadiatorPosition"] = 1;
            //}
            //else
            //{
            //    DoubleVariables["RadiatorPosition"] = 0;
            //
            //}

            #endregion
           
            Atmosphere =  GuruEngine.World.World.GetAtmos().Update(DoubleVariables["Altitude"]);
            if (script != null)
            {
                script.UpdateScript(dt);
            }

        }
        #endregion

        #region AircraftStateComponent Interface
        public bool GetVar(String id, bool Default)
        {
            if (BoolVariables.ContainsKey(id))
            {
                return BoolVariables[id];
            }
            BoolVariables.Add(id, Default);
            return Default;
        }

        public void SetVar(String id, double value)
        {
            if (DoubleVariables.ContainsKey(id))
            {
                DoubleVariables[id] = value;
            }
        }

        public double GetVar(String id, double Default)
        {
            double res = Default;
            lock (DoubleVariables)
            {
                if (DoubleVariables.ContainsKey(id))
                    res =  DoubleVariables[id];
                else
                    DoubleVariables.Add(id, Default);
            }
            return res;
        }

        public void RunScript(String s)
        {
            if (script != null)
            {
                script.StartAction(s);
            }
        }

        public bool IsPlayer()
        {
            return true;
        }
        #endregion

    }
}
