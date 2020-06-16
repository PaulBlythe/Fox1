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

//( Class ShipStateComponent )
//( Group Naval )
//( Type ShipStateComponent )
//( ConnectionList ShipGunComponent Guns )
namespace GuruEngine.ECS.Components.Ships
{
    public class ShipStateComponent:ECSGameComponent
    {
        List<ShipGunComponent> Guns = new List<ShipGunComponent>();
        List<MultiMeshComponent> Meshes = new List<MultiMeshComponent>();
        WorldTransform Transform;
        Dictionary<int, double> DoubleVariables = new Dictionary<int, double>();
        Dictionary<int, bool> BoolVariables = new Dictionary<int, bool>();

        #region ECS Game component methods
        public override ECSGameComponent Clone()
        {
            ShipStateComponent other = new ShipStateComponent();

            other.Transform = Transform;
            other.Meshes = Meshes;
            other.Guns = Guns;
            return (ECSGameComponent)other;
        }

        public override void Connect(string components, bool isList)
        {
            string[] parts = components.Split('#');
            if (isList)
            {
                switch (parts[1])
                {
                    case "Guns":
                        {
                            string[] mfds = parts[1].Split(',');
                            for (int i = 0; i < mfds.Length; i++)
                            {
                                string[] objs = mfds[i].Split(':');
                                if (objs.Length == 2)
                                {
                                    Guns.Add((ShipGunComponent)Parent.FindGameComponentByName(objs[0]));
                                }
                            }
                        }
                        break;
                    default:
                        throw new Exception("GameComponent::FuelTank:: Unknown list connection request to " + parts[0]);
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
                        //throw new Exception("GameComponent::FuelTank:: Unknown direct connection request to " + parts[0]);
                        break;
                }
            }
        }

        public override void DisConnect()
        {
            Transform = null;
            Meshes.Clear();
            Guns.Clear();
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
            ShipStateComponent otherT = (ShipStateComponent)other.FindGameComponentByName(Name);
            otherT.Meshes = Meshes;
            otherT.Guns = Guns;
            otherT.Transform = Transform;

           
        }

        public override void RenderOffscreenRenderTargets()
        {
        }

        public override void SetParameter(string Name, string Value)
        {

        }

        /// <summary>
        /// </summary>
        /// <param name="dt"></param>
        public override void Update(float dt)
        {
            
        }
        #endregion


        #region ShipStateComponent Interface
        public bool GetVar(int id, bool Default)
        {
            if (BoolVariables.ContainsKey(id))
            {
                return BoolVariables[id];
            }
            BoolVariables.Add(id, Default);
            return Default;
        }

        public double GetVar(int id, double Default)
        {
            double res = Default;
            lock (DoubleVariables)
            {
                if (DoubleVariables.ContainsKey(id))
                    res = DoubleVariables[id];
                else
                    DoubleVariables.Add(id, Default);
            }
            return res;
        }
        #endregion
    }
}
