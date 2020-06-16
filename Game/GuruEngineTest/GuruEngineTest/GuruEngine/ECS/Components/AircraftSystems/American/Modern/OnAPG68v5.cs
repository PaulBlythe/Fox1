using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;


using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

using GuruEngine.ECS;
using GuruEngine.ECS.Components;
using GuruEngine.ECS.Components.World;
using GuruEngine.ECS.Components.AircraftSystems.General;
using GuruEngine.Simulation.Components.Radar.Airbourne;
using GuruEngine.Simulation.Components.Radar.Airbourne.Modern;
using GuruEngine.Simulation.Components.Radar.CrossSections;
using GuruEngine.Data.Code.Instruments;
using GuruEngine.Simulation.Components.Radar;
using GuruEngine.Simulation.Systems;

//( Class OnAPG68v5 )
//( Type Radar )
//( Group Sensors )
//( ConnectionList Radar Instruments )
//( Connection Transform Position )

namespace GuruEngine.ECS.Components.AircraftSystems.American.Modern
{
    public class OnAPG68v5 : ECSGameComponent, AirbourneRadarInterface
    {
        #region Fire control radar mode
        public enum FireControlRadarMode
        {
            RangeWhileScan,
            TrackWhileScan,
            VelocitySearch,
            AirCombatManoeuvering,
            SituationalAwareness,
            SingleTargetTrack,
            GroundMapping,
            GroundMovingTarget,
            SeaBorneTargetScan,
        }
        public FireControlRadarMode fireControlRadarMode = FireControlRadarMode.RangeWhileScan;

        public enum RadarMode
        {
            CombinedRadarMode,
            Inoperative,
        }
        public RadarMode radarMode;
        #endregion

        #region Emitter variables
        float emitter_angle = 0;
        float emitter_bar = 0;
        float timer = 0;
        int submode = 0;
        #endregion

        GameObject parent = null;
        WorldTransform position;

        int Bars = 4;
        int Azimuth = 60;    // in degrees
        int Range = 120;     // in miles
        int History = 4;
        float Offset = 0;    // in degrees
        float Tilt = 0;      // in degrees
        float ApparentAntennaAperture = 3;      // (from er13081) effective antenna size in square metres
        float TransmitterPower = 12900;         // in watts
        float AntennaGain = 20;                 // from interweb
        double DetectionLimit = 1.18186E-16;    // from experimentation

        // Notes
        // Requires 5.6K watts power

        List<ECSGameComponent> AttachedMFDs = new List<ECSGameComponent>();

        Dictionary<int, AirbourneRadarTarget> TrackedTargets = new Dictionary<int, AirbourneRadarTarget>();
        List<int> TrackedThisFrame = new List<int>();
        int BuggedTarget = -1;

        public OnAPG68v5()
        {
            UpdateStage = 3;

            //for (float gain = 1.0f; gain<1000.0f; gain+=1.0f)
            //{
            //    float res = RadarSimulation.GetReturn(TransmitterPower, gain, ApparentAntennaAperture, 5, 120000, 1);
            //    Debug.WriteLine(String.Format("Radar test gain {0} return {1}", gain, res));
            //}

        }

        /// <summary>
        /// Update 
        /// 4.9 degrees vertical scan per pass 
        /// 2.5 seconds to scan one bar (assumes 120 degree azimuth)
        /// 0.5 seconds to move antenna vertically
        /// </summary>
        /// <param name="dt"></param>
        /// <param name="dataPacket"></param>
        public override void Update(float dt)
        {
            ECSGameComponent pm = Parent.FindSingleComponentByType<PowerManagementComponent>();
            if (pm != null)
            {
                PowerManagementComponent pmc = (PowerManagementComponent)pm;
                if (! pmc.HasPower(5600.0f/14.0f))
                {
                    // cannot run without power
                    return;
                }
            }


            timer += dt;
            float limit = 2.5f * (Azimuth * 2) / 120.0f;
            float arc = (60.0f / 2.5f) * dt;
            float cosArc = (float)Math.Cos(arc);

            switch (fireControlRadarMode)
            {
                case FireControlRadarMode.SituationalAwareness:
                case FireControlRadarMode.RangeWhileScan:
                    {
                        switch (submode)
                        {
                            case 0:
                                {
                                    if (timer > limit)
                                    {
                                        timer = 0;
                                        // reached the end of one bar
                                        if (Bars > 1)
                                        {
                                            // extra bars so move antenna vertically
                                            submode = 1;
                                        }else
                                        {

                                        }
                                    }
                                    emitter_angle = ((timer/limit) * Azimuth * 2) - Azimuth;
                                }
                                break;

                            case 1:
                                {
                                    if (timer>=0.5f)
                                    {
                                        submode = 0;
                                        emitter_bar++;
                                        if (emitter_bar == Bars)
                                        {
                                            emitter_bar = 0;
                                        }
                                        timer = 0;
                                    }
                                }
                                break;
                        }
                    }
                    break;
            }
            Matrix world = Matrix.CreateFromQuaternion(position.GetOrientation());
            Vector3 noseDirection = world.Forward;
            float yaw = MathHelper.ToRadians(emitter_angle + Offset);
            float pitch = 0;
            switch (Bars)
            {
                case 1:
                    break;
                case 2:
                    pitch = MathHelper.ToRadians((emitter_bar - 1) * 2.2f);
                    break;
                case 3:
                    pitch = MathHelper.ToRadians((emitter_bar - 1.5f) * 2.2f);
                    break;
                case 4:
                    pitch = MathHelper.ToRadians((emitter_bar - 2) * 2.2f);
                    break;
            }
            Matrix temp = Matrix.CreateFromYawPitchRoll(yaw, pitch, 0);
            Vector3 beamDirection = Vector3.Transform(noseDirection, temp);
            Vector3 myposition = position.GetLocalPosition();

           
            noseDirection.Y = 0;
            noseDirection.Normalize();

            foreach (GameObject go in GameObjectManager.Instance.AirbourneTargets)
            {
                if (go != parent)
                {
                    WorldTransform wt = (WorldTransform) go.FindGameComponentByName("WorldTransform_1");
                    Vector3 direction = wt.GetLocalPosition() - myposition;
                    float range = direction.Length();

                    direction.Normalize();
                    float dot = Vector3.Dot(beamDirection, direction);
                    if (dot<cosArc)
                    {
                        if (Settings.GetInstance().Difficulty < 5)
                        {
                            AddTarget(wt, go, direction, noseDirection);
                        }else
                        {
                            RadarCrossSectionComponent rcsc = (RadarCrossSectionComponent)go.FindGameComponentByName("RadarCrossSectionComponent_1");
                            RadarCrossSectionDefinition rcsd = (RadarCrossSectionDefinition)rcsc.GetContainedObject("RadarCrossSection");
                            Matrix t = Matrix.CreateFromQuaternion(wt.GetOrientation());
                            float angle = (float)Math.Acos(Vector3.Dot(t.Forward, direction));
                            int iangle = (int)MathHelper.ToDegrees(angle);
                            if (iangle < 0)
                                iangle += 360;
                            float rcs = rcsd.CalculateRCS(iangle);
                            float receivedPower = RadarSimulation.GetReturn(TransmitterPower, AntennaGain, ApparentAntennaAperture, rcs, range, 1);
                            if (receivedPower>=DetectionLimit)
                            {
                                AddTarget(wt, go, direction, noseDirection);
                            }
                        }
                        
                    }
                }
            }
            if (BuggedTarget!=-1)
            {
                if (TrackedTargets.ContainsKey(BuggedTarget))
                {

                }else
                {
                    BuggedTarget = -1;
                }
            }
            
        }

        private void AddTarget(WorldTransform wt, GameObject go, Vector3 direction, Vector3 noseDirection)
        {
            // we have painted a target
            if (TrackedTargets.Keys.Contains(go.UID))
            {
                // painted this one before
                for (int i = 3; i > 0; i--)
                {
                    TrackedTargets[go.UID].LocalPosition[i] = TrackedTargets[go.UID].LocalPosition[i - 1];
                }

                TrackedTargets[go.UID].LocalPosition[0] = wt.GetLocalPosition();
                TrackedTargets[go.UID].Orientation = wt.GetOrientation();
                direction.Y = 0;
                direction.Normalize();
                TrackedTargets[go.UID].AngleOffTheNose = (float)Math.Acos(Vector3.Dot(noseDirection, direction));
                if (!TrackedThisFrame.Contains(go.UID))
                {
                    TrackedThisFrame.Add(go.UID);
                }
            }
            else
            {
                // new target
                AirbourneRadarTarget art = new AirbourneRadarTarget();
                art.UID = go.UID;
                art.LocalPosition[0] = wt.GetLocalPosition();
                art.IFF = 0;
                art.Jamming = false;
                art.Emiting = false;
                art.Orientation = wt.GetOrientation();
                art.Velocity = 400;
                TrackedTargets.Add(go.UID, art);
                if (!TrackedThisFrame.Contains(go.UID))
                {
                    TrackedThisFrame.Add(go.UID);
                }
            }
        }

        private void CleanList()
        {
            for (int i=TrackedTargets.Count-1; i>=0; i++)
            {
                if (!TrackedThisFrame.Contains(TrackedTargets[i].UID))
                {
                    TrackedTargets.Remove(i);
                }
            }
            TrackedThisFrame.Clear();
        }

        public override void Connect(string Comps, bool isList)
        {
            string[] parts = Comps.Split('#');
            if (isList)
            {
                switch (parts[1])
                {
                    case "Instruments":
                        {
                            string[] mfds = parts[1].Split(',');
                            for (int i=0; i<mfds.Length; i++)
                            {
                                string[] objs = mfds[i].Split(':');
                                if (objs.Length == 2)
                                {
                                    AttachedMFDs.Add(parent.FindGameComponentByName(objs[0]));
                                }
                            }
                        }
                        break;
                    default:
                        throw new Exception("GameComponent::OnAPG68v5:: Unknown list connection request to " + parts[0]);
                }
            }else
            {
                switch(parts[1])
                {
                    case "Root":
                        {
                            string[] objects = parts[2].Split(':');
                            parent = GameObjectManager.Instance.FindGameObjectByName(objects[0]);
                        }
                        break;
                    case "Position":
                        {
                            string[] objects = parts[2].Split(':');
                            position = (WorldTransform)parent.FindGameComponentByName(objects[0]);
                        }
                        break;
                    default:
                        throw new Exception("GameComponent::OnAPG68v5:: Unknown direct connection request to " + parts[0]);
                }
            }
        }

        public override void DisConnect()
        {
            AttachedMFDs.Clear();
        }

        public override void ReConnect(GameObject other)
        {
            OnAPG68v5 otherRadar = (OnAPG68v5) other.FindGameComponentByName(Name);
            foreach (ECSGameComponent gc in AttachedMFDs)
            {
                ECSGameComponent ogc = other.FindGameComponentByName(gc.Name);
                otherRadar.AttachedMFDs.Add(ogc);                
            }
            otherRadar.position = (WorldTransform)other.FindGameComponentByName(position.Name);
            
        }
        /// <summary>
        /// Load any disk based assets and initialise state
        /// </summary>
        public override void Load(ContentManager content)
        {
            radarMode = RadarMode.CombinedRadarMode;
            fireControlRadarMode = FireControlRadarMode.RangeWhileScan;
        }

        public override void HandleEvent(string evt)
        {
            
        }

        public override void RenderOffscreenRenderTargets()
        {
        }

        public override Texture2D GetOffscreenTexture()
        {
            return null;
        }

        public override object GetContainedObject(string type)
        {
            return null;
        }

        public override ECSGameComponent Clone()
        {
            return new OnAPG68v5();
        }

        public override void SetParameter(string Name, string Value)
        {
        }

        #region AirbourneRadarInterface section
        public String GetMode()
        {
            switch (radarMode)
            {
                case RadarMode.CombinedRadarMode:
                    return "CRM";

                default:
                    return "***";
            }
            
        }

        public Dictionary<int, AirbourneRadarTarget> GetTrackedTargets()
        {
            return TrackedTargets;
        }

        public String GetSubMode()
        {
            switch (fireControlRadarMode)
            {
                case FireControlRadarMode.AirCombatManoeuvering:
                    return "ACM";
                case FireControlRadarMode.GroundMapping:
                    return "GM";
                case FireControlRadarMode.GroundMovingTarget:
                    return "GMT";
                case FireControlRadarMode.RangeWhileScan:
                    return "RWS";
                case FireControlRadarMode.SeaBorneTargetScan:
                    return "SEA";
                case FireControlRadarMode.SingleTargetTrack:
                    return "STT";
                case FireControlRadarMode.SituationalAwareness:
                    return "SAM";
                case FireControlRadarMode.TrackWhileScan:
                    return "TWS";
                case FireControlRadarMode.VelocitySearch:
                    return "VS";
                default:
                    return "***";
            }
            
        }

        public int GetBars()
        {
            return Bars;
        }

        public int GetAzimuth()
        {
            return Azimuth;
        }

        public float GetEmitterAngle()
        {
            return emitter_angle;
        }

        public int GetRange()
        {
            return Range;
        }

        public int GetTargetHistory()
        {
            return History;
        }

        public float GetOffset()
        {
            return Offset;
        }

        public float GetTilt()
        {
            return Tilt;
        }

        public int GetBuggedTarget()
        {
            return BuggedTarget;
        }

        public void BugTarget(int id)
        {
            switch (fireControlRadarMode)
            {
                case FireControlRadarMode.RangeWhileScan:
                    fireControlRadarMode = FireControlRadarMode.SituationalAwareness;
                    BuggedTarget = id;
                    break;
                case FireControlRadarMode.SituationalAwareness:
                    fireControlRadarMode = FireControlRadarMode.SingleTargetTrack;
                    BuggedTarget = id;
                    break;
            }
        }

        public void HandleSystemInput(String evt)
        {
            switch (evt)
            {
                case "AzimuthUp":
                    {
                        switch (Azimuth)
                        {
                            case 60:
                                Azimuth = 10;
                                break;
                            case 30:
                                Azimuth = 60;
                                break;
                            case 25:
                                Azimuth = 30;
                                break;
                            case 10:
                                Azimuth = 25;
                                break;

                        }
                    }
                    break;

                case "RangeUp":
                    {
                        switch (Range)
                        {
                            case 60:
                                Range = 120;
                                break;
                            case 30:
                                Range = 60;
                                break;
                            case 15:
                                Range = 30;
                                break;

                        }
                    }
                    break;

                case "RangeDown":
                    {
                        switch (Range)
                        {
                            case 120:
                                Range = 60;
                                break;
                            case 60:
                                Range = 30;
                                break;
                            case 30:
                                Range = 15;
                                break;

                        }
                    }
                    break;

                case "BarsUp":
                    {
                        switch (Bars)
                        {
                            case 4:
                                Bars = 1;
                                break;
                            case 2:
                                Bars = 4;
                                break;
                            case 1:
                                Bars = 2;
                                break;

                        }
                    }
                    break;
            }
        }
        #endregion

    }
}
