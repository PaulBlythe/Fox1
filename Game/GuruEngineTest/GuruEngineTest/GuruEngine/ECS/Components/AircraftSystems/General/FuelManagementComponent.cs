using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

//( Class FuelManagementComponent )
//( Group Propulsion )
//( Type FuelManagementComponent )
//( ConnectionList FuelTank Tanks )
//( ConnectionList EngineComponent Engines )

namespace GuruEngine.ECS.Components.AircraftSystems.General
{
    public class FuelManagementComponent : ECSGameComponent
    {
        List<FuelTank> Tanks = new List<FuelTank>();
        List<EngineComponent> Engines = new List<EngineComponent>();

        #region ECS Game component methods
        public override ECSGameComponent Clone()
        {
            FuelManagementComponent other = new FuelManagementComponent();
            return other;
        }

        public override void Connect(string components, bool isList)
        {
            string[] parts = components.Split('#');
            if (isList)
            {
                switch (parts[1])
                {
                    case "Tanks":
                        {
                            string[] mfds = parts[1].Split(',');
                            for (int i = 0; i < mfds.Length; i++)
                            {
                                string[] objs = mfds[i].Split(':');
                                if (objs.Length == 2)
                                {
                                    Tanks.Add((FuelTank)Parent.FindGameComponentByName(objs[0]));
                                }
                            }
                        }
                        break;
                    case "Engines":
                        {
                            string[] mfds = parts[1].Split(',');
                            for (int i = 0; i < mfds.Length; i++)
                            {
                                string[] objs = mfds[i].Split(':');
                                if (objs.Length == 2)
                                {
                                    Engines.Add((EngineComponent)Parent.FindGameComponentByName(objs[0]));
                                }
                            }
                        }
                        break;
                    default:
                        throw new Exception("GameComponent::FuelManagementComponent:: Unknown list connection request to " + parts[0]);
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
                        throw new Exception("GameComponent::FuelManagementComponent:: Unknown direct connection request to " + parts[0]);
                }
            }
        }

        public override void DisConnect()
        {
            Tanks.Clear();
            Engines.Clear();
        }

        public override object GetContainedObject(string type)
        {
            
            return null;
        }

        public override void ReplaceComponent(ECSGameComponent old, ECSGameComponent replacement)
        {
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
                case "FuelTankToggle":
                    {
                        switch (parts[1])
                        {
                            case "QI":          // input mapping type QuadStateInt
                                {
                                    int code = int.Parse(parts[1]);
                                    switch (code)
                                    {
                                        case 0:
                                            Tanks[0].ConnectedToEngine = true;
                                            Tanks[1].ConnectedToEngine = false;
                                            break;
                                        case 1:
                                            Tanks[0].ConnectedToEngine = false;
                                            Tanks[1].ConnectedToEngine = true;
                                            break;
                                        case 2:
                                            Tanks[0].ConnectedToEngine = true;
                                            Tanks[1].ConnectedToEngine = true;
                                            break;
                                        case 3:
                                            Tanks[0].ConnectedToEngine = false;
                                            Tanks[1].ConnectedToEngine = false;
                                            break;

                                    }
                                }
                                break;
                        }
                    }
                    break;

              
            }
        }

        public override void Load(ContentManager content)
        {

        }

        public override void ReConnect(GameObject tother)
        {
            FuelManagementComponent other = (FuelManagementComponent)tother.FindGameComponentByName(Name);
            foreach (ECSGameComponent gc in Tanks)
            {
                ECSGameComponent ogc = tother.FindGameComponentByName(gc.Name);
                other.Tanks.Add((FuelTank)ogc);
            }
            foreach (ECSGameComponent gc in Engines)
            {
                ECSGameComponent ogc = tother.FindGameComponentByName(gc.Name);
                other.Engines.Add((EngineComponent)ogc);
            }
        }

        public override void RenderOffscreenRenderTargets()
        {
        }

        public override void SetParameter(string Name, string Value)
        {
            
        }

        public override void Update(float dt)
        {
            
        }
        #endregion

    }
}
