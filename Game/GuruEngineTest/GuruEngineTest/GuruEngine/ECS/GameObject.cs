using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;

using GuruEngine.Physics.World;
using GuruEngine.ECS.Components.World;
using GuruEngine.Helpers;
using GuruEngine.Physics.Aircraft;
using GuruEngine.World.PaintSchemes;
using GuruEngine.ECS.Components.Mesh;

namespace GuruEngine.ECS
{
    public class GameObject
    {
        public String Name;
        public String FullPath;
        public int UID;
        public Dictionary<String, String> TextureOverrides = new Dictionary<string, string>();

        public bool IsAirbourneTarget = true;
        public bool IsGroundTarget = false;
        public int IFF;

        public Dictionary<String, ECSGameComponent> Components = new Dictionary<string, ECSGameComponent>();

        public void DestroyGameObject()
        {
            Parallel.ForEach(Components.Keys, (s) =>
            {
                Components[s].DisConnect();
            });
        }

        public void Update(int stage, float dt)
        {
            Parallel.ForEach(Components.Keys, (s) =>
            {
                if (Components[s].UpdateStage == stage)
                {
                    Components[s].Update(dt);
                }
            });
        }

        public ECSGameComponent FindGameComponentByName(String name)
        {
            if (Components.ContainsKey(name))
                return Components[name];
            return null;
        }

        /// <summary>
        /// Find all components which match the required type
        /// </summary>
        /// <typeparam name="TClass">Class type to search for</typeparam>
        /// <returns></returns>
        public List<ECSGameComponent> FindGameComponentByType<TClass>()
        {
            List<ECSGameComponent> results = new List<ECSGameComponent>();
            foreach (String s in Components.Keys)
            {
                if (Components[s] is TClass)
                {
                    results.Add(Components[s]);
                }
            }

            return results;
        }

        /// <summary>
        /// Find a single component by it's type.
        /// Returns the first it finds
        /// </summary>
        /// <typeparam name="TClass">Class type to search for</typeparam>
        /// <returns></returns>
        public ECSGameComponent FindSingleComponentByType<TClass>()
        {
            foreach (String s in Components.Keys)
            {
                if (Components[s] is TClass)
                {
                    return Components[s];
                }
            }
            return null;
        }

        public void LoadContent(ContentManager content)
        {
            foreach(String s in Components.Keys)
            {
                Components[s].Load(content);
            }
        }

        public void RenderOffscreenRenderTargets()
        {
            foreach (String s in Components.Keys)
            {
                Components[s].RenderOffscreenRenderTargets();
            }
        }

        #region Physics helpers

        public AtmosphericModelResults atmosphereAltitude;
        public AtmosphericModelResults atmosphereSeaLevel;
        public AuxiliaryStateData auxStateData = new AuxiliaryStateData();

        public void UpdatePhysicsState()
        {
            WorldTransform wt = (WorldTransform)FindGameComponentByName("WorldTransform_1");

            ///
            /// TODO get correct sea level based on latitude/longitude
            /// 
            atmosphereAltitude = World.World.Instance.atmosphericModel.Update(wt.LocalPosition.Y);
            atmosphereSeaLevel = World.World.Instance.atmosphericModel.Update(0);

            // TODO replace this with physics data
            auxStateData.Velocity = wt.Velocity;
            auxStateData.AltitudeASL = MathsHelper.MetresToFeet(wt.LocalPosition.Y);
            auxStateData.Update(this);

        }
        #endregion

        public void ApplyPaintScheme(PaintScheme scheme)
        {
            foreach (string s in scheme.TextureOverrides.Keys)
            {
                TextureOverrides.Add(s, scheme.TextureOverrides[s]);
            }
        }

        /// <summary>
        /// IFF = Identification Friend or Foe
        /// Each group is defined by a block of 8 bits.
        /// So you could have English == 1, American == 2, German == 256, Italian == 257
        /// Subgroups are supported in 4 bit boundaries so German == 256 and Japanese == 320 is valid
        /// </summary>
        /// <param name="iff"></param>
        public void SetIFF(int iff)
        {
            IFF = iff;
        }

        public Vector3 GetWorldPosition()
        {
            WorldTransform wt = (WorldTransform)FindSingleComponentByType<WorldTransform>();
            return wt.GetLocalPosition();
        }

        public Matrix GetWorldTransform()
        {
            WorldTransform wt = (WorldTransform)FindSingleComponentByType<WorldTransform>();
            return wt.GetMatrix();
        }

        public MultiMeshComponent FindHookOwner(String hookname)
        {
            foreach (String s in Components.Keys)
            {
                if (Components[s] is MultiMeshComponent)
                {
                    MultiMeshComponent mm = (MultiMeshComponent)Components[s];
                    if (mm.HasHook(hookname))
                        return mm;
                }
            }
            return null;
        }

        public void ReplaceComponent(ECSGameComponent old, ECSGameComponent replacement)
        {
            Components.Remove(old.Name);

            foreach (String s in Components.Keys)
            {
                Components[s].ReplaceComponent(old, replacement);
            }
        }

    }
}
