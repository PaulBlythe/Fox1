using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;

using GuruEngine.ECS.Components.World;
using GuruEngine.Cameras;
using GuruEngine.World;
using GuruEngine.ECS.Components.Mesh;
using GuruEngine.ECS.Components.Settings;
using GuruEngine.InputDevices;
using GuruEngine.Player;
using GuruEngine.ECS.Components.AircraftSystems.General;

using GuruEngine.Player.HumanFactors;
using GuruEngine.Rendering.EffectPasses;
using GuruEngine.Rendering;

namespace GuruEngine.ECS.Components.Game
{
    public class LocalPlayerComponent
    {
        public static LocalPlayerComponent Instance;
        public bool CockpitLights = false;
        GameObject Mesh = null;
        GameObject Fuselage = null;
        Vector3 CockpitOffset = new Vector3(-1.350f, 0.0f, 0.600482f);
       
        WorldTransform transform;
        WorldTransform transform2;
        AircraftStateComponent hoststate;
        AircraftSettingsComponent hostsettings;
        OxygenSupplyComponent osc = null;
        float gear_direction = 0;
        float cock_direction = 0;

        #region Player world factors
        Hypoxia hypoxia = new Hypoxia();
        RedOutEffectPass redout = new RedOutEffectPass();
        BlackoutEffectPass blackout = new BlackoutEffectPass();
        #endregion

        #region Head position animation
        
        Vector3 head_position_delta = new Vector3(0, 0, 0);
        Vector3 head_position_target = new Vector3(0, 0, 0);
        Vector3 head_position_current = new Vector3(0, 0, 0);

        Quaternion target_quat = new Quaternion(0, 0, 0, 1);
        Quaternion current_quat = new Quaternion(0, 0, 0, 1);
        Quaternion delta_quat = new Quaternion(0, 0, 0, 1);
        float lft = 0;

        int head_rotation = 3;
        int head_pitch = 3;
        float[] head_rot_angles = new float[] { -120, -90, -45 , 0.0f, 45.0f, 90.0f, 120.0f };
        float[] head_pitch_angles = new float[] { -45, -33, -11, 0.0f, 11.0f, 33.0f, 45.0f };
        #endregion

        public LocalPlayerComponent()
        {
            Instance = this;
        }

        public void Update(float dt)
        {
            QuaternionCamera camera = (QuaternionCamera)WorldState.GetWorldState().camera;
            if (camera != null)
            {
                camera.ViewAdjust = current_quat;
                camera.Update(dt);
                
                if (transform != null)
                {
                    Quaternion q1 = camera.GetOrientation();
                    transform.SetOrientation(q1);
                    transform2.SetOrientation(q1);

                    Matrix lw = Matrix.CreateFromYawPitchRoll(0, MathHelper.ToRadians(-90), MathHelper.ToRadians(90)) * Matrix.CreateFromQuaternion(q1);

                    //MultiMeshComponent body = Mesh.FindHookOwner("_CAMERA_<BASE>");
                    //Matrix m = body.GetOriginalHookMatrix("_CAMERA_<BASE>");
                    //if (m.Forward != Vector3.Zero)
                    //{
                    //    Vector3 test = Vector3.Transform(m.Translation, lw);
                    //    System.Console.WriteLine(test.ToString());
                    //}
                    Vector3 dp = CockpitOffset + head_position_current;
                    Vector3 temp = Vector3.Transform(dp, lw);

                    transform.SetPosition(camera.GetLocalPosition() - temp);
                    transform.Update(dt);

                    transform2.SetPosition(camera.GetLocalPosition() - temp);
                    transform2.Update(dt);

                    #region Lean animation
                    if (lft == 1)
                    {
                        int old_h = head_rotation;
                        int old_p = head_pitch;

                        bool leanf = InputDeviceManager.GetPlayerButton("LeanForward");
                        bool leanb = InputDeviceManager.GetPlayerButton("LeanBack");
                        bool leanl = InputDeviceManager.GetPlayerButton("LeanLeft");
                        bool leanr = InputDeviceManager.GetPlayerButton("LeanRight");

                        if (leanl)
                            head_rotation++;
                        if (leanr)
                            head_rotation--;

                        if (head_rotation < 0)
                            head_rotation = 0;

                        if (head_rotation > 6)
                            head_rotation = 6;

                        if (leanf)
                            head_pitch--;
                        if (leanb)
                            head_pitch++;

                        if (head_pitch > 6)
                            head_pitch = 6;
                        if (head_pitch < 0)
                            head_pitch = 0;

                        if ((old_h != head_rotation)||(old_p != head_pitch))
                        {
                            float yaw = head_rot_angles[head_rotation];
                            float pitch = head_pitch_angles[head_pitch];

                            head_position_delta = head_position_current;
                            target_quat = Quaternion.CreateFromYawPitchRoll(MathHelper.ToRadians(yaw), MathHelper.ToRadians(pitch), 0);
                            delta_quat = current_quat;
                            lft = 0;
                        }
                    }
                    
                    if (lft < 1)
                    {
                        lft += PlayerSettings.CockpitPositionAnimationRate * dt;
                        lft = Math.Min(1, lft);
                        
                        head_position_current = Vector3.Lerp(head_position_delta, head_position_target, lft);
                        current_quat = Quaternion.Slerp(delta_quat, target_quat, lft);
                    }

                    #endregion

                }
            }


            #region Update cockpit
            if (Mesh != null)
            {
                if ((hoststate != null) && (hostsettings != null))
                {
                    #region Elevator trim
                    bool etrimup = InputDeviceManager.GetPlayerButton("ElevatorTrimUp");
                    bool etrimdown = InputDeviceManager.GetPlayerButton("ElevatorTrimDown");
                    double etrim = hoststate.GetVar("ElevatorTrim", 0.0);
                    if ((etrimdown) || (etrimup))
                    {
                        double min = hostsettings.Variables["ElevatorTrimMin"];
                        double max = hostsettings.Variables["ElevatorTrimMax"];

                        double delta = (max - min) / 100.0;
                        if (etrimdown)
                            etrim -= delta;
                        if (etrimup)
                            etrim += delta;
                        etrim = Math.Min(max, etrim);
                        etrim = Math.Max(min, etrim);
                        hoststate.SetVar("ElevatorTrim", etrim);
                    }
                    #endregion

                    #region Rudder trim
                    etrimup = InputDeviceManager.GetPlayerButton("RudderTrimRight");
                    etrimdown = InputDeviceManager.GetPlayerButton("RudderTrimLeft");
                    etrim = hoststate.GetVar("RudderTrim", 0.0);
                    if ((etrimdown) || (etrimup))
                    {
                        double min = hostsettings.Variables["RudderTrimMin"];
                        double max = hostsettings.Variables["RudderTrimMax"];

                        double delta = (max - min) / 100.0;
                        if (etrimdown)
                            etrim -= delta;
                        if (etrimup)
                            etrim += delta;
                        etrim = Math.Min(max, etrim);
                        etrim = Math.Max(min, etrim);
                        hoststate.SetVar("RudderTrim", etrim);
                    }
                    #endregion

                    #region Propeller pitch
                    etrimup = InputDeviceManager.GetPlayerButton("Prop0PitchUp");
                    etrimdown = InputDeviceManager.GetPlayerButton("Prop0PitchDown");
                    etrim = hoststate.GetVar("Prop0Pitch", 0.0);
                    if ((etrimdown) || (etrimup))
                    {
                        double min = 0;
                        double max = 1;

                        double delta = (max - min) / 100.0;
                        if (etrimdown)
                            etrim -= delta;
                        if (etrimup)
                            etrim += delta;
                        etrim = Math.Min(max, etrim);
                        etrim = Math.Max(min, etrim);
                        hoststate.SetVar("Prop0Pitch", etrim);
                    }
                    #endregion

                    #region Elevator
                    bool eup = InputDeviceManager.GetPlayerButton("ElevatorUp");
                    bool edown = InputDeviceManager.GetPlayerButton("ElevatorDown");
                    double e = hoststate.GetVar("ElevatorPosition", 0.0);
                    if ((edown) || (eup))
                    {
                        double min = -1;
                        double max = 1;

                        double delta = (max - min) / 100.0;
                        if (edown)
                            e -= delta;
                        if (eup)
                            e += delta;
                        e = Math.Min(max, e);
                        e = Math.Max(min, e);
                        hoststate.SetVar("ElevatorPosition", e);
                    }
                    #endregion

                    #region Aileron
                    bool aup = InputDeviceManager.GetPlayerButton("AileronLeft");
                    bool adown = InputDeviceManager.GetPlayerButton("AileronRight");
                    double a = hoststate.GetVar("AileronPosition", 0.0);
                    if ((adown) || (aup))
                    {
                        double min = -1;
                        double max = 1;

                        double delta = (max - min) / 100.0;
                        if (adown)
                            a -= delta;
                        if (aup)
                            a += delta;
                        a = Math.Min(max, a);
                        a = Math.Max(min, a);
                        hoststate.SetVar("AileronPosition", a);
                    }
                    #endregion

                    #region Throttle
                    aup = InputDeviceManager.GetPlayerButton("ThrottleUp");
                    adown = InputDeviceManager.GetPlayerButton("ThrottleDown");
                    a = hoststate.GetVar("ThrottleSetting", 0.0);
                    if ((adown) || (aup))
                    {
                        double min = -1;
                        double max = hostsettings.Variables["ThrottleMax"];

                        double delta = (max - min) / 100.0;
                        if (adown)
                            a -= delta;
                        if (aup)
                            a += delta;
                        a = Math.Min(max, a);
                        a = Math.Max(min, a);
                        hoststate.SetVar("ThrottleSetting", a);
                    }
                    #endregion

                    #region Brake
                    bool brake = InputDeviceManager.GetPlayerButton("BrakeControl");
                    hoststate.SetVar("BrakeControl", brake ? 1 : 0);
                    #endregion

                    #region Clock
                    DateTime d = WorldState.GetWorldState().GameTime;
                    hoststate.SetVar("TimeHour", d.Hour);
                    hoststate.SetVar("TimeMinute", d.Minute);
                    hoststate.SetVar("TimeSecond", d.Second);
                    #endregion

                    #region Rudder
                    eup = InputDeviceManager.GetPlayerButton("RudderRight");
                    edown = InputDeviceManager.GetPlayerButton("RudderLeft");
                    e = hoststate.GetVar("RudderPosition", 0.0);
                    if ((edown) || (eup))
                    {
                        double min = -1;
                        double max = 1;

                        double delta = (max - min) / 100.0;
                        if (edown)
                            e -= delta;
                        if (eup)
                            e += delta;
                        e = Math.Min(max, e);
                        e = Math.Max(min, e);
                        hoststate.SetVar("RudderPosition", e);
                    }
                    #endregion

                    #region Cockpit lights
                    bool cp = InputDeviceManager.GetPlayerButton("CockpitLightToggle");
                    if (cp)
                    {
                        CockpitLights = !CockpitLights;
                    }
                    #endregion

                    #region Gear
                    bool pressed = InputDeviceManager.GetPlayerButton("GearToggle");
                    double gp = hoststate.GetVar("GearPosition", 0.0);
                    if (pressed)
                    {

                        if ((gear_direction == 0) && (gp == 0))
                        {
                            gear_direction = 1;
                        }
                        else
                        {
                            if ((gear_direction == 1) && (gp == 1))
                            {
                                gear_direction = -1;
                            }
                        }
                    }
                    if (gear_direction != 0)
                    {
                        double t = gp + gear_direction * dt;
                        if (t <= 0)
                        {
                            t = 0;
                            gear_direction = 0;
                        }
                        if (t >= 1)
                        {
                            t = 1;
                            gear_direction = 1;
                        }
                        hoststate.SetVar("GearPosition", t);
                    }
                    #endregion

                    #region Cockpit door
                    bool cppressed = InputDeviceManager.GetPlayerButton("CockpitToggle");
                    double cgp = hoststate.GetVar("CockpitPosition", 0.0);
                    if (pressed)
                    {
                        if ((cock_direction == 0) && (cgp == 0))
                        {
                            cock_direction = 1;
                        }
                        else
                        {
                            if ((cock_direction == 1) && (cgp == 1))
                            {
                                cock_direction = -1;
                            }
                        }
                    }
                    if (cock_direction != 0)
                    {
                        double t = cgp + cock_direction * dt;
                        if (t <= 0)
                        {
                            t = 0;
                            cock_direction = 0;
                        }
                        if (t >= 1)
                        {
                            t = 1;
                            cock_direction = 1;
                        }
                        hoststate.SetVar("CockpitPosition", t);
                    }
                    #endregion

                    #region Update human factors
                    if (osc == null)
                    {
                        osc = (OxygenSupplyComponent)Mesh.FindSingleComponentByType<OxygenSupplyComponent>();
                    }
                    if (osc != null)
                    {
                        if (osc.Contents == 0)
                        {
                            hypoxia.Update((float)hoststate.GetVar("Altitude", 0), dt);
                        }
                    }
                    else
                    {
                        hypoxia.Update((float)hoststate.GetVar("Altitude", 0), dt);
                    }


                    float pg = (float)hoststate.GetVar("PilotG", 1);
                    float pga = (float)Math.Abs(pg);
                    float limit = 6;
                    if (hostsettings.Variables["GSuit"] > 0)
                        limit = 9;
                    if (pga > limit)
                    {
                        float amount = (pga - limit) / 3.0f;
                        amount = Math.Min(1, amount);
                        if (pg < 0)
                        {
                            redout.Value = amount;
                            Renderer.AddEffectPass(redout);
                        }
                        else
                        {

                            blackout.Value = amount;
                            Renderer.AddEffectPass(blackout);
                        }
                    }


                    #endregion
                }

                for (int i = 0; i < 5; i++)
                {
                    Mesh.Update(i, dt);
                }
            }
            #endregion

            if (Fuselage!=null)
            {
                for (int i = 0; i < 5; i++)
                {
                    Fuselage.Update(i, dt);
                }
            }
        }

        public void LoadContent(ContentManager content)
        {
            if (Mesh != null)
            {
                Mesh.LoadContent(content);
            }
            if (Fuselage != null)
            {
                Fuselage.LoadContent(content);

                AircraftStateComponent real = (AircraftStateComponent)Mesh.FindSingleComponentByType<AircraftStateComponent>();
                AircraftStateComponent old = (AircraftStateComponent)Fuselage.FindSingleComponentByType<AircraftStateComponent>();
                Fuselage.ReplaceComponent(old, real);
            }
        }

        public void RenderOffscreenRenderTargets()
        {
            if (Mesh != null)
            {
                Mesh.RenderOffscreenRenderTargets();
            }
        }

        public void UpdatePhysicsState()
        {
            if (Mesh != null)
            {
                Mesh.UpdatePhysicsState();
            }
            if (Fuselage != null)
            {
                Fuselage.UpdatePhysicsState();
            }
        }

        public void AddMesh(String file)
        {
            Mesh = GameObjectManager.Instance.CreateInstance(file);
            transform = (WorldTransform)Mesh.FindSingleComponentByType<WorldTransform>();
            hoststate = (AircraftStateComponent)Mesh.FindSingleComponentByType<AircraftStateComponent>();
            hostsettings = (AircraftSettingsComponent)Mesh.FindSingleComponentByType<AircraftSettingsComponent>();
        }

        public void AddFuselage(String file)
        {
            Fuselage = GameObjectManager.Instance.CreateInstance(file);
            transform2 = (WorldTransform)Fuselage.FindSingleComponentByType<WorldTransform>();

            MultiMeshComponent Host = (MultiMeshComponent)Fuselage.FindGameComponentByName("Head1_D0");
            Host.Hidden = true;

            Host = (MultiMeshComponent)Fuselage.FindGameComponentByName("CF_D0");
            Host.Hidden = true;

            Host = (MultiMeshComponent)Fuselage.FindGameComponentByName("Blister1_D0");
            Host.Hidden = true;

            Host = (MultiMeshComponent)Fuselage.FindGameComponentByName("Pilot1_D0");
            Host.Hidden = true;

           
        }

    }
}
