using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

using GuruEngine.ECS;
using GuruEngine.Physics.World;
using GuruEngine.Physics.Collision;
using GuruEngine.ECS.Components.World;
using GuruEngine.DebugHelpers;

//( Class DynamicPhysicsComponent )
//( Group Physics )
//( Type DynamicPhysicsComponent )
//( Parameter Bool IsStatic )

namespace GuruEngine.ECS.Components.Physics
{
    public class DynamicPhysicsComponent:ECSGameComponent
    {
        bool IsStatic;
        RigidBody body;
        WorldTransform transform;

        public DynamicPhysicsComponent()
        {
            UpdateStage = 4;
        }

        #region ECS Game component methods
        public override ECSGameComponent Clone()
        {
            DynamicPhysicsComponent other = new DynamicPhysicsComponent();
            other.IsStatic = IsStatic;
            return other;
        }

        public override void Connect(string components, bool isList)
        {
            string[] parts = components.Split('#');
            if (isList)
            {
                throw new Exception("GameComponent::DynamicPhysicsComponent:: Unknown list connection request to " + parts[0]);
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
                        throw new Exception("GameComponent::DynamicPhysicsComponent:: Unknown direct connection request to " + parts[0]);
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

        public override void ReplaceComponent(ECSGameComponent old, ECSGameComponent replacement)
        {

        }

        public override void Load(ContentManager content)
        {
            body = new RigidBody();
            body.LinearMotionThreshold = 0.8f;
            body.AngularMotionThreshold = 0.5f;
            transform = (WorldTransform)Parent.FindSingleComponentByType<WorldTransform>();
            body.UpdateTransform(ref transform.LocalPosition);
        }

        public override void ReConnect(GameObject tother)
        {
            DynamicPhysicsComponent other = (DynamicPhysicsComponent)tother.FindGameComponentByName(Name);
            other.IsStatic = IsStatic;
        }

        public override void RenderOffscreenRenderTargets()
        {
        }

        public override void SetParameter(string Name, string Value)
        {
            switch (Name)
            {
                case "IsStatic":
                    IsStatic = bool.Parse(Value);
                    break;
            }
        }

        public override void Update(float dt)
        {
            #region Do rigid body physics
            if (body.dirty)
                body.CalculateMassProperties();

            body.AttemptDeactivation(dt);
            if ((body.Flags & RigidBodyFlags.Disabled) != RigidBodyFlags.Disabled)
            {
                body.IntegrateVelocity(dt);
                body.IntegratePosition(dt);
            }

            body.ClearForces();

            #endregion

            transform.UpdateTransform(body.Position, body.Orientation);

#if DEBUG
            DebugLineDraw.DrawArrow(body.Position + body.CofG, Vector3.Up, Color.Yellow, 5);
            DebugLineDraw.DrawArrow(body.Position, body.currentAcceleration, Color.Green, body.currentAcceleration.Length());
#endif

        }
        #endregion


        /// <summary>
        /// Applies the force to the rigid body in the local frame
        /// </summary>
        /// <returns></returns>
        public void AddLocalForce(Vector3 force, Vector3 position)
        {
            body.ApplyLocalForce(force, position);
#if DEBUG
            if (DebugRenderSettings.DrawForces)
            {
                Vector3 p1 = Vector3.Transform(position, transform.world);
                Vector3 ddirection = Vector3.Normalize(force);
                DebugLineDraw.DrawArrow(p1, ddirection, Color.White, 4);

            }
#endif
        }

        /// <summary>
        /// Applies the force to the rigid body in the local frame
        /// </summary>
        /// <returns></returns>
        public void AddWorldForce(Vector3 force, Vector3 position)
        {
            body.ApplyWorldForce(force, position);
#if DEBUG
            if (DebugRenderSettings.DrawForces)
            {
                Vector3 p1 = Vector3.Transform(position, transform.world);
                Vector3 ddirection = Vector3.Normalize(force);
                DebugLineDraw.DrawArrow(p1, ddirection, Color.White, 4);

            }
#endif
        }

        /// <summary>
        /// Add a static mass
        /// </summary>
        /// <param name="position"></param>
        /// <param name="mass"></param>
        public int AddPointMass(Vector3 position, float mass)
        {
            return body.AddPointMass(mass, position);
        }

        public void AddTorque(float t)
        {
            body.ApplyWorldTorque(Vector3.UnitX * t);
        }
        /// <summary>
        /// Update dynamic masses
        /// </summary>
        /// <param name="id"></param>
        /// <param name="mass"></param>
        public void UpdatePointMass(int id, float mass)
        {
            body.UpdatePointMass(id, mass);
        }

        /// <summary>
        /// IMPLEMENT THIS
        /// </summary>
        /// <returns></returns>
        public bool IsOnWater(Vector3 position)
        {
            return false;
        }

        /// <summary>
        /// IMPLEMENT THIS
        /// </summary>
        /// <returns></returns>
        public bool IsOnGround(Vector3 position)
        {
            return true;
        }

        /// <summary>
        /// IMPLEMENT THIS CURRENTLY NO TERRAIN
        /// </summary>
        /// <returns></returns>
        public float GroundPenetrationDepth(Vector3 position)
        {
            Vector3 wp = Vector3.Transform(position, transform.world);
            return -wp.Y;
        }

        /// <summary>
        /// Get gross velocity
        /// </summary>
        /// <returns></returns>
        public float GetVelocity()
        {
            return body.Velocity.Length();
        }

        public Vector3 GetWorldVelocity()
        {
            return body.Velocity;
        }

        public Vector3 GetWorldAcceleration()
        {
            return body.currentAcceleration;
        }

        public float GetTotalMass()
        {
            return body.Mass;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public Vector3 GetDirection()
        {
            return Vector3.Normalize(transform.world.Forward);
        }

        public Vector3 GetVelocity(Vector3 position)
        {
            return body.Velocity;
        }
    }
}
