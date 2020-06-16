using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using GuruEngine.Simulation.Weapons.AAA;
using GuruEngine.Simulation.Weapons.Ammunition;
using GuruEngine.ECS.Components.Mesh;
using GuruEngine.ECS.Components.World;
using GuruEngine.AI;
using GuruEngine.DebugHelpers;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

//( Class DebugGunneryTargetDrone )
//( Group Debug )
//( Type DebugGunneryTargetDrone )
//( Parameter String Pattern )
//( Parameter Float Speed )

namespace GuruEngine.ECS.Components.Debug
{
    public enum PatternCommand
    {
        Setup,
        Straight,
        Bank,
        Left,
        Right
    };

    public class PatternStep
    {
        public float Time;
        public PatternCommand Command;
        public Vector3 Variables;

        public PatternStep(float t, PatternCommand l, Vector3 a)
        {
            Time = t;
            Command = l;
            Variables = a;
        }
    }
    public class DebugGunneryTargetDrone : ECSGameComponent
    {
        String Pattern;
        float Speed;

        List<PatternStep> Steps = new List<PatternStep>();
        int current_step = 0;
        int loop_step = 0;
        float step_time = 0;
        Vector3 current_angles = new Vector3(0, 0, 0);
        Vector3 old_angles = new Vector3(0, 0, 0);
        int targetid;

        WorldTransform host;

        #region ECS Game component methods
        public override ECSGameComponent Clone()
        {
            DebugGunneryTargetDrone other = new DebugGunneryTargetDrone();

            other.Pattern = Pattern;
            other.Speed = Speed;
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
            host = (WorldTransform) Parent.FindGameComponentByName("WorldTransform_1");
            targetid = TargetManager.AddTarget(1, 1, host.GetLocalPosition(), Vector3.Zero);

            switch (Pattern)
            {
                case "FigureOfEight":
                    {
                        Steps.Add(new PatternStep(0, PatternCommand.Setup, new Vector3(MathHelper.ToRadians(45), 0, 0)));

                        Steps.Add(new PatternStep(2, PatternCommand.Straight, Vector3.Zero));
                        Steps.Add(new PatternStep(1, PatternCommand.Bank, new Vector3(MathHelper.ToRadians(-45), 0, 0)));
                        Steps.Add(new PatternStep(2, PatternCommand.Left, new Vector3(MathHelper.ToRadians(90), 0, 0)));
                        Steps.Add(new PatternStep(1, PatternCommand.Bank, new Vector3(0, MathHelper.ToRadians(-45), 0)));
                        Steps.Add(new PatternStep(4, PatternCommand.Straight, Vector3.Zero));
                        Steps.Add(new PatternStep(1, PatternCommand.Bank, new Vector3(MathHelper.ToRadians(-45), 0, 0)));
                        Steps.Add(new PatternStep(4, PatternCommand.Left, new Vector3(MathHelper.ToRadians(90), 0, 0)));
                        Steps.Add(new PatternStep(1, PatternCommand.Bank, new Vector3(0, MathHelper.ToRadians(-45), 0)));
                        Steps.Add(new PatternStep(4, PatternCommand.Straight, Vector3.Zero));
                        Steps.Add(new PatternStep(1, PatternCommand.Bank, new Vector3(MathHelper.ToRadians(-45), 0, 0)));
                        Steps.Add(new PatternStep(4, PatternCommand.Left, new Vector3(MathHelper.ToRadians(90), 0, 0)));
                        Steps.Add(new PatternStep(1, PatternCommand.Bank, new Vector3(0, MathHelper.ToRadians(-45), 0)));
                        Steps.Add(new PatternStep(4, PatternCommand.Straight, Vector3.Zero));
                        Steps.Add(new PatternStep(1, PatternCommand.Bank, new Vector3(MathHelper.ToRadians(-45), 0, 0)));
                        Steps.Add(new PatternStep(4, PatternCommand.Left, new Vector3(MathHelper.ToRadians(90), 0, 0)));
                        Steps.Add(new PatternStep(1, PatternCommand.Bank, new Vector3(0, MathHelper.ToRadians(-45), 0)));
                        Steps.Add(new PatternStep(2, PatternCommand.Straight, Vector3.Zero));
                    }
                    break;
            }

        }

        public override void ReConnect(GameObject other)
        {
            DebugGunneryTargetDrone otherT = (DebugGunneryTargetDrone)other.FindGameComponentByName(Name);
            otherT.Speed = Speed;
            otherT.Pattern = Pattern;
        }

        public override void RenderOffscreenRenderTargets()
        {
        }

        public override void SetParameter(string Name, string Value)
        {
            switch (Name)
            {
                case "Pattern":
                    {
                        Pattern = Value;

                    }
                    break;

                case "Speed":
                    {
                        Speed = float.Parse(Value);
                    }
                    break;
               

            }
        }

        /// <summary>
        /// </summary>
        /// <param name="dt"></param>
        public override void Update(float dt)
        {
            step_time += dt;
            
            switch(Steps[current_step].Command)
            {
                case PatternCommand.Setup:
                    current_angles = Steps[current_step].Variables;
                    step_time = 0;
                    current_step++;
                    break;

                case PatternCommand.Straight:
                    if (step_time>=Steps[current_step].Time)
                    {
                        step_time = 0;
                        current_step++;
                    }
                    break;

                case PatternCommand.Bank:
                    {
                        float delta = Math.Min(1, step_time / Steps[current_step].Time);
                        current_angles.Z = MathHelper.SmoothStep(Steps[current_step].Variables.Y, Steps[current_step].Variables.X, delta);
                        if (delta>=1)
                        {
                            current_step++;
                            step_time = 0;
                        }
                    }
                    break;

                case PatternCommand.Left:
                    {
                        float da = Steps[current_step].Variables.X / Steps[current_step].Time;

                        current_angles.X -= da * dt;
                        if (step_time >= Steps[current_step].Time)
                        {
                            current_step++;
                            step_time = 0;
                        }
                    }
                    break;

                case PatternCommand.Right:
                    {
                        float delta = Math.Min(1, step_time / Steps[current_step].Time);
                        float da = Steps[current_step].Variables.X / Steps[current_step].Time;

                        current_angles.X += da * dt;
                        if (delta >= 1)
                        {
                            current_step++;
                            step_time = 0;
                        }
                    }
                    break;

            }
            if (current_step == Steps.Count)
                current_step = 1;

            host.Orientation = Quaternion.CreateFromYawPitchRoll(current_angles.X, current_angles.Y, current_angles.Z);
            host.Velocity = Speed;
            host.LocalPosition += Speed * dt * host.world.Forward;

            TargetManager.UpdateTarget(targetid, host.GetLocalPosition(), Speed * host.world.Forward);

        }
        #endregion


    }
}
