using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

using GuruEngine.ECS.Components.World;

//( Class LODGroupComponent )
//( Type LODGroup )
//( ConnectionList Mesh Meshes )
//( Parameter String Filename )


namespace GuruEngine.ECS.Components.Mesh
{
    public class LODGroupComponent : ECSGameComponent
    {
        int nDistances = 0;
        int[] Distances;
        List<BasicMeshComponent> LODs = new List<BasicMeshComponent>();
        GameObject parent = null;


        #region ECS game component interface
        public override ECSGameComponent Clone()
        {
            LODGroupComponent other = new LODGroupComponent();
            other.nDistances = nDistances;
            other.LODs = LODs;
            other.Distances = Distances;

            return (ECSGameComponent)other;
        }

        public override void Connect(string components, bool isList)
        {
            string[] parts = components.Split('#');
            if (isList)
            {
                switch (parts[0])
                {
                    case "Meshes":
                        {
                            string[] meshes = parts[1].Split(',');
                            for (int i = 0; i < meshes.Length; i++)
                            {
                                string[] objs = meshes[i].Split(':');
                                if (objs.Length == 2)
                                {
                                    LODs.Add((BasicMeshComponent)parent.FindGameComponentByName(objs[0]));
                                }
                            }
                        }
                        break;
                }
            }
            else
            {
                switch (parts[0])
                {
                    case "Root":
                        {
                            string[] objects = parts[1].Split(':');
                            parent = GameObjectManager.Instance.FindGameObjectByName(objects[0]);
                        }
                        break;
                    default:
                        throw new Exception("GameComponent::OnAPG68v5:: Unknown direct connection request to " + parts[0]);
                }
            }
        }

        public override void DisConnect()
        {
            LODs.Clear();
            parent = null;
            nDistances = 0;
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
            LODGroupComponent otherLODgroup = (LODGroupComponent)other.FindGameComponentByName(Name);
            foreach (ECSGameComponent gc in LODs)
            {
                ECSGameComponent ogc = other.FindGameComponentByName(gc.Name);
                otherLODgroup.LODs.Add((BasicMeshComponent)ogc);
            }
        }

        public override void RenderOffscreenRenderTargets()
        {
        }

        public override void ReplaceComponent(ECSGameComponent old, ECSGameComponent replacement)
        {
        }

        public override void SetParameter(string Name, string Value)
        {
            if (Name == "Filename")
            {
                using (BinaryReader b = new BinaryReader(File.Open(Value, FileMode.Open)))
                {
                    nDistances = b.ReadInt32();
                    Distances = new int[nDistances];
                    for (int i = 0; i < nDistances; i++)
                    {
                        Distances[i] = b.ReadInt32();
                    }
                }
            }
        }

        public override void Update(float dt)
        {
        }

        #endregion

        #region public methods
        public BasicMeshComponent GetMesh(int distance)
        {
            if ((nDistances == 0) || (distance < Distances[0]))
                return null;

            for (int i=1; i<nDistances; i++)
            {
                if (distance < Distances[i])
                    return LODs[i - 1];
            }
            return LODs[nDistances - 1];
        }
        #endregion
    }
}
