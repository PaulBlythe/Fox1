using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

using GuruEngine.ECS.Components.Physics;
using GuruEngine.ECS.Components.World;
using GuruEngine.DebugHelpers;

//( Class BallastComponent )
//( Group Aero )
//( Type BallastComponent )
//( Parameter Vector3 Position )
//( Parameter Float Mass )

namespace GuruEngine.ECS.Components.AircraftSystems.Aero
{
    public class BallastComponent : ECSGameComponent
    {
        public Vector3 Position;
        public float Mass;

        public BallastComponent()
        {
            UpdateStage = 1;
        }

        #region ECS Game component methods
        public override ECSGameComponent Clone()
        {
            BallastComponent other = new BallastComponent();
            other.Position = Position;
            other.Mass = Mass;
            return other;
        }

        public override void Connect(string components, bool isList)
        {
            string[] parts = components.Split('#');
            if (isList)
            {

                throw new Exception("GameComponent::HStabComponent:: Unknown list connection request to " + parts[0]);

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
                        throw new Exception("GameComponent::HStabComponent:: Unknown direct connection request to " + parts[0]);

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

        }

        public override void Load(ContentManager content)
        {
            DynamicPhysicsComponent state = (DynamicPhysicsComponent)Parent.FindSingleComponentByType<DynamicPhysicsComponent>();
            state.AddPointMass(Position, Mass);
        }

        public override void ReConnect(GameObject tother)
        {
            BallastComponent other = (BallastComponent)tother.FindGameComponentByName(Name);
            other.Position = Position;
            other.Mass = Mass;
        }

        public override void RenderOffscreenRenderTargets()
        {
        }

        public override void SetParameter(string Name, string Value)
        {
            switch (Name)
            {
                case "Mass":
                    Mass = float.Parse(Value);
                    break;

                case "Position":
                    {
                        Position = new Vector3();
                        string[] parts = Value.Split(',');
                        Position.X = float.Parse(parts[0]);
                        Position.Y = float.Parse(parts[1]);
                        Position.Z = float.Parse(parts[2]);
                    }
                    break;
            }
        }

        public override void Update(float dt)
        {
#if DEBUG
            if (DebugRenderSettings.DrawMasses)
            {
                WorldTransform t = (WorldTransform)Parent.FindSingleComponentByType<WorldTransform>();
                Vector3 p = t.GetLocalPosition() + Position;
                DebugLineDraw.DrawArrow(p, Vector3.Down, Color.Purple, 4);
                DebugLineDraw.DrawText(Name, p + Vector3.Down * 4.5f);
            }
#endif
        }
        #endregion

    }
}
