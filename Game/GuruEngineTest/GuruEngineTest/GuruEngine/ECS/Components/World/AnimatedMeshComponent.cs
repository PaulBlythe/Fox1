using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

using GuruEngine.ECS.Components.Mesh;
using GuruEngine.ECS.Components.World;

//( Class AnimatedMeshComponent )
//( Group World )
//( Type AnimatedMeshComponent )
//( ConnectionList MultiMeshComponent Meshes )
//( Connection Transform WorldTransform )

namespace GuruEngine.ECS.Components.World
{
    public class AnimatedMeshComponent : ECSGameComponent
    {
        List<MultiMeshComponent> Meshes = new List<MultiMeshComponent>();
        WorldTransform Transform;

        #region ECS Game component methods
        public override ECSGameComponent Clone()
        {
            AnimatedMeshComponent other = new AnimatedMeshComponent();

            other.Transform = Transform;
            return (ECSGameComponent)other;
        }

        public override void Connect(string components, bool isList)
        {
            string[] parts = components.Split('#');
            if (isList)
            {
                switch (parts[1])
                {
                    case "Meshes":
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
                    default:
                        throw new Exception("GameComponent::AircraftComponent:: Unknown list connection request to " + parts[0]);
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

        }

        public override void Load(ContentManager content)
        {

        }

        public override void ReConnect(GameObject other)
        {
            AnimatedMeshComponent otherTank = (AnimatedMeshComponent)other.FindGameComponentByName(Name);
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

        }

        /// <summary>
        /// This component only holds the top level sub meshes.
        /// Each mesh contained here has a list of children which are relative to it.
        /// </summary>
        /// <param name="dt"></param>
        public override void Update(float dt)
        {
            Matrix mat = Matrix.CreateFromYawPitchRoll(0, MathHelper.ToRadians(-90), MathHelper.ToRadians(90)) * Transform.GetMatrix();
            foreach (MultiMeshComponent m in Meshes)
            {
                m.UpdateMatrices(mat);
            }
        }
        #endregion
    }
}