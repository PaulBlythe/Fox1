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

//( Class PropellorAnimatorComponent )
//( Group Animation )
//( Type PropellorAnimatorComponent )
//( Parameter Int EngineNumber )
//( Parameter Int PropellorNumber )

namespace GuruEngine.ECS.Components.Animators.Aircraft.Standard
{
    public class PropellorAnimatorComponent:ECSGameComponent
    {
        #region Configuration variables
        int EngineNumber;
        int PropellorNumber;

        MultiMeshComponent Host;
        MultiMeshComponent HostRot;
        AircraftStateComponent State;
        #endregion

        #region Identifiers
        String EngineRPMVar;
        String PropellorFeatheredVAr;
        String PropellorDamageVar;
        #endregion

        #region Calculated values
        float Angle = 0;
        #endregion

        #region ECS Game component methods
        public override ECSGameComponent Clone()
        {
            PropellorAnimatorComponent other = new PropellorAnimatorComponent();
            other.EngineNumber = EngineNumber;
            other.PropellorNumber = PropellorNumber;

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
            HostRot = null;
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
            EngineRPMVar = String.Format("Propellor{0}RPM", PropellorNumber);
            PropellorFeatheredVAr = String.Format("Propellor{0}Feathered", PropellorNumber);
            PropellorDamageVar = String.Format("Propellor{0}Damage", PropellorNumber);

            String t = String.Format("Prop{0}_D0", PropellorNumber);
            Host = (MultiMeshComponent) Parent.FindGameComponentByName(t);
            t = String.Format("PropRot{0}_D0", PropellorNumber);
            HostRot = (MultiMeshComponent)Parent.FindGameComponentByName(t);
            State = (AircraftStateComponent) Parent.FindGameComponentByName("AircraftStateComponent_1");
        }

        public override void ReConnect(GameObject other)
        {
            PropellorAnimatorComponent otherTank = (PropellorAnimatorComponent)other.FindGameComponentByName(Name);
            otherTank.EngineNumber = EngineNumber;
            otherTank.PropellorNumber = PropellorNumber;
        }

        public override void RenderOffscreenRenderTargets()
        {
        }

        public override void SetParameter(string Name, string Value)
        {
            if (Name == "EngineNumber")
            {
                EngineNumber = int.Parse(Value);
            }
            if (Name == "PropellorNumber")
            {
                PropellorNumber = int.Parse(Value);
            }
        }

        public override void Update(float dt)
        {
            double Damage = State.GetVar(PropellorDamageVar, 0);
            double RPM = State.GetVar(EngineRPMVar, 0);
            MultiMeshComponent target = null;

            if (RPM<600)
            {
                // Revs so low we need the full mesh
                Host.Hidden = false;
                HostRot.Hidden = true;
                target = Host;
            }
            else
            {
                Host.Hidden = true;
                HostRot.Hidden = false;
                target = HostRot;
            }
            Angle += (float)(RPM * dt * 2 * MathHelper.Pi / 60);

            Matrix m = Matrix.CreateRotationY(Angle);
            target.MatrixAnimate(m);
        }
        #endregion


    }
}
