
using System;
using System.Collections.Generic;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

using GuruEngine.ECS;
using GuruEngine.Physics.World;
using GuruEngine.Physics.Collision;
using GuruEngine.ECS.Components.World;
using GuruEngine.AI.Scripting;
using GuruEngine.World;
using GuruEngine.Maths;
using GuruEngine.Rendering;
using GuruEngine.ECS.Components.Mesh;
using GuruEngine.Simulation.Weapons.AAA;

//( Class AntiAircraftArtilleryComponent )
//( Group AAA )
//( Type AntiAircraftArtilleryComponent )
//( ConnectionList MultiMeshComponent Meshes )
//( Connection Transform WorldTransform )
//( Parameter Float FireTime )
//( Parameter Float RecoilDistance )
//( Parameter Float MinYaw )
//( Parameter Float MaxYaw )
//( Parameter Float MinPitch )
//( Parameter Float MaxPitch )
//( Parameter Int AnimMode )
//( Parameter String Round )

namespace GuruEngine.ECS.Components.Artillery
{
    public class AntiAircraftArtilleryComponent : ECSGameComponent
    {
        public Dictionary<String, double> DoubleVariables = new Dictionary<string, double>();
        public Dictionary<String, bool> BoolVariables = new Dictionary<string, bool>();
        List<MultiMeshComponent> Meshes = new List<MultiMeshComponent>();
        WorldTransform Transform;
        float time = 0;
        public float FireTime;
        public float RecoilDistance;
        public float MinYaw;
        public float MaxYaw;
        public float MinPitch;
        public float MaxPitch;
        public int AnimMode;
        public String Round;
        MultiMeshComponent Gun;
        MultiMeshComponent Head;

        public AntiAircraftArtilleryComponent()
        {
            UpdateStage = 4;
        }

        #region ECS Game component methods
        public override ECSGameComponent Clone()
        {
            AntiAircraftArtilleryComponent other = new AntiAircraftArtilleryComponent();
            other.FireTime = FireTime;
            other.RecoilDistance = RecoilDistance;
            other.Transform = Transform;
            other.MinYaw = MinYaw;
            other.MaxYaw = MaxYaw;
            other.MinPitch = MinPitch;
            other.MaxPitch = MaxPitch;
            other.AnimMode = AnimMode;
            other.Round = Round;
            return other;
        }

        public override void Connect(string components, bool isList)
        {
            string[] parts = components.Split('#');
            if (isList)
            {
                switch (parts[0])
                {
                    case "MultiMeshComponent":
                        {
                            string[] mfds = parts[2].Split(',');
                            for (int i = 0; i < mfds.Length; i++)
                            {
                                string[] objs = mfds[i].Split(':');
                                if (objs.Length == 2)
                                {
                                    Meshes.Add((MultiMeshComponent)Parent.FindGameComponentByName(objs[0]));
                                }
                            }
                        }
                        break;
                    case "Meshes":
                        {
                            string[] mfds = parts[1].Split(',');
                            for (int i = 0; i < mfds.Length; i++)
                            {
                                string[] objs = mfds[i].Split(':');
                                if (objs.Length == 2)
                                {
                                    Meshes.Add((MultiMeshComponent)Parent.FindGameComponentByName(objs[0]));
                                }
                            }
                        }
                        break;
                    default:
                        throw new Exception("GameComponent::AntiAircraftArtilleryComponent:: Unknown list connection request to " + parts[0]);
                }
            }
            else
            {
                string[] objects = parts[2].Split(':');
                switch (parts[0])
                {
                    case "Transform":
                        {
                            Transform = (WorldTransform)Parent.FindGameComponentByName(objects[0]);
                        }
                        break;

                    case "Root":
                        {
                            Parent = GameObjectManager.Instance.FindGameObjectByName(objects[0]);
                        }
                        break;

                    default:
                        //throw new Exception("GameComponent::FuelTank:: Unknown direct connection request to " + parts[0]);
                        break;
                }
            }
        }



        public override void DisConnect()
        {
            Transform = null;
            Gun = null;
            Head = null;
            Meshes.Clear();
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
            DoubleVariables.Add("GunPitch", 0);
            DoubleVariables.Add("GunYaw", 0);
            DoubleVariables.Add("GunFireState", 0);
            DoubleVariables.Add("Recoil", 0);
            Gun = (MultiMeshComponent)Parent.FindGameComponentByName("Gun");
            Head = (MultiMeshComponent)Parent.FindGameComponentByName("Head");
            if (Round != null)
                WeaponDataBase.Load(Round);
        }

        public override void ReConnect(GameObject other)
        {
            AntiAircraftArtilleryComponent otherTank = (AntiAircraftArtilleryComponent)other.FindGameComponentByName(Name);
            otherTank.FireTime = FireTime;
            otherTank.RecoilDistance = RecoilDistance;
            otherTank.MinYaw = MinYaw;
            otherTank.MaxYaw = MaxYaw;
            otherTank.MinPitch = MinPitch;
            otherTank.MaxPitch = MaxPitch;
            otherTank.AnimMode = AnimMode;
            otherTank.Round = Round;
            foreach (ECSGameComponent gc in Meshes)
            {
                ECSGameComponent ogc = other.FindGameComponentByName(gc.Name);
                otherTank.Meshes.Add((MultiMeshComponent)ogc);
            }
            if (Transform != null)
                otherTank.Transform = (WorldTransform)other.FindGameComponentByName(Transform.Name);
        }

        public override void RenderOffscreenRenderTargets()
        {
        }

        public override void SetParameter(string Name, string Value)
        {
            switch (Name)
            {
                case "RecoilDistance":
                    RecoilDistance = float.Parse(Value);
                    break;
                case "Round":
                    Round = Value;
                    break;
                case "FireTime":
                    FireTime = float.Parse(Value);
                    break;
                case "MinYaw":
                    MinYaw = float.Parse(Value);
                    break;
                case "MaxYaw":
                    MaxYaw = float.Parse(Value);
                    break;
                case "MinPitch":
                    MinPitch = float.Parse(Value);
                    break;
                case "MaxPitch":
                    MaxPitch = float.Parse(Value);
                    break;
                case "AnimMode":
                    AnimMode = int.Parse(Value);
                    break;
            }
        }

        public override void ReplaceComponent(ECSGameComponent old, ECSGameComponent replacement)
        {
        }

        public override void Update(float dt)
        {
            time += dt;

            switch (AnimMode)
            {
                case 0:
                    {
                        Matrix gm = Matrix.CreateFromYawPitchRoll(0, 0, MathHelper.ToRadians((float)DoubleVariables["GunPitch"]));
                        Vector3 trans = Vector3.UnitX * RecoilDistance * (float)DoubleVariables["Recoil"];
                        gm = Matrix.CreateTranslation(trans) * gm;

                        Gun.MatrixAnimate(gm);

                        gm = Matrix.CreateRotationZ(MathHelper.ToRadians((float)DoubleVariables["GunYaw"]));
                        Head.MatrixAnimate(gm);
                    }
                    break;
                case 1:
                    {
                        Matrix gm = Matrix.Identity;
                        Vector3 trans = Vector3.UnitX * RecoilDistance * (float)DoubleVariables["Recoil"];
                        gm = Matrix.CreateTranslation(trans) * gm;

                        Gun.MatrixAnimate(gm);

                        gm = Matrix.CreateRotationY(MathHelper.ToRadians((float)DoubleVariables["GunPitch"]))
                            * Matrix.CreateRotationZ(MathHelper.ToRadians((float)DoubleVariables["GunYaw"]));
                        Head.MatrixAnimate(gm);
                    }
                    break;
            }
            

            Matrix mat = Matrix.CreateFromYawPitchRoll(0, MathHelper.ToRadians(-90), MathHelper.ToRadians(90)) * Transform.GetMatrix();
            foreach (MultiMeshComponent m in Meshes)
            {
                m.UpdateMatrices(mat);
            }
        }
        #endregion
    }
}
