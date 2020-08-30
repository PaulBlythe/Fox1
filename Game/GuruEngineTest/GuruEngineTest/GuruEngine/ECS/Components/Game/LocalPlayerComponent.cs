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
using GuruEngine.World;

namespace GuruEngine.ECS.Components.Game
{
    public class LocalPlayerComponent
    {
        public static LocalPlayerComponent Instance;
        GameObject Mesh = null;
        Vector3 CockpitOffset = new Vector3(-1.75f, 0.0f, 0.44f);
        Vector3 LeanForwardDelta = new Vector3(0.5f, 0.0f, -0.25f);
        WorldTransform transform;
        AircraftStateComponent hoststate;
        AircraftSettingsComponent hostsettings;

        bool lf = false;
        bool lb = false;
        float lft = 0;

        public LocalPlayerComponent()
        {
            Instance = this;
        }

        public void Update(float dt)
        {
            QuaternionCamera camera = (QuaternionCamera) WorldState.GetWorldState().camera;
            if (camera != null)
            {
                camera.Update(dt);

                if (transform!=null)
                {
                    Quaternion q1 = camera.GetOrientation();
                    Vector3 euler = GuruEngine.Maths.MathUtils.QuaternionToEuler(q1);
                    if (hoststate!=null)
                    {
                        //hoststate.SetVar("Yaw", MathHelper.ToDegrees(euler.Y));
                        hoststate.SetVar("Pitch", MathHelper.ToDegrees(euler.X));
                        hoststate.SetVar("Roll", MathHelper.ToDegrees(euler.Z));
                        
                    }

                    transform.SetOrientation(q1);

                    Matrix lw = Matrix.CreateFromYawPitchRoll(0, MathHelper.ToRadians(-90), MathHelper.ToRadians(90)) * Matrix.CreateFromQuaternion(q1);

                    MultiMeshComponent body = Mesh.FindHookOwner("_CAMERA_<BASE>");
                    Matrix m = body.GetOriginalHookMatrix("_CAMERA_<BASE>");
                    if (m.Forward != Vector3.Zero)
                    {

                    }
                    Vector3 dp = CockpitOffset + (lft * LeanForwardDelta);
                    Vector3 temp = Vector3.Transform(dp, lw);

                    transform.SetPosition(camera.GetLocalPosition() - temp );
                    transform.Update(dt);

                    #region Lean forward animation
                    bool leanf = InputDeviceManager.GetPlayerButton("LeanForward");
                    if (leanf)
                    {
                        lb = false;
                        if (lf)
                        {

                        }
                        else if (lft < 1)
                        {
                            lf = true;
                            lft = 0;
                        }
                    }
                    if (lf)
                    {
                        if (lft < 1)
                        {
                            lft += PlayerSettings.CockpitPositionAnimationRate * dt;
                            lft = Math.Min(1, lft);
                        }

                    }
                    #endregion

                    #region Lean back animation
                    bool leanb = InputDeviceManager.GetPlayerButton("LeanBack");
                    if (leanb)
                    {
                        lf = false;
                        if (lb)
                        {

                        }
                        else if (lft > 0)
                        {
                            lb = true;
                        }
                    }
                    if (lb)
                    {
                        if (lft > 0)
                        {
                            lft -= PlayerSettings.CockpitPositionAnimationRate * dt;
                            lft = Math.Max(0, lft);
                        }

                    }
                    #endregion

                }
            }

            if (Mesh != null)
            {
                if ((hoststate != null) && (hostsettings != null))
                {
                    #region Elevator trim
                    bool etrimup = InputDeviceManager.GetPlayerButton("ElevatorTrimUp");
                    bool etrimdown = InputDeviceManager.GetPlayerButton("ElevatorTrimDown");
                    double etrim = hoststate.GetVar("ElevatorTrim", 0.0);
                    if ((etrimdown)||(etrimup))
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
                    double a = hoststate.GetVar("AileronControl", 0.0);
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
                        hoststate.SetVar("AileronControl", a);
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


                }

                for (int i = 0; i < 5; i++)
                {
                    Mesh.Update(i, dt);
                }
            }

        }

        public void LoadContent(ContentManager content)
        {
            if (Mesh != null)
            {
                Mesh.LoadContent(content);
            }
        }

        public void RenderOffscreenRenderTargets()
        {
            if (Mesh!=null)
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
        }

        public void AddMesh(String file)
        {
            Mesh = GameObjectManager.Instance.CreateInstance(file);
            transform = (WorldTransform)Mesh.FindSingleComponentByType<WorldTransform>();
            hoststate = (AircraftStateComponent)Mesh.FindSingleComponentByType<AircraftStateComponent>();
            hostsettings = (AircraftSettingsComponent)Mesh.FindSingleComponentByType<AircraftSettingsComponent>();
        }

        
    }
}
