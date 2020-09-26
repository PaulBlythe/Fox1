using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;



namespace GuruEngine.ECS.Components.Mesh
{
    //( Class BasicMeshComponent )
    //( Type BasicMeshComponent )
    //( Parameter String Filename )

    public class BasicMeshComponent : ECSGameComponent
    {
        public String MeshFilename;

        Matrix World = Matrix.Identity;

        #region ECS game component interface
        public override ECSGameComponent Clone()
        {
            BasicMeshComponent other = new BasicMeshComponent();
            other.MeshFilename = MeshFilename;
            other.UpdateStage = 4;
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
                switch (parts[0])
                {
                    case "Root":
                        {
                            string[] objects = parts[1].Split(':');
                            Parent = GameObjectManager.Instance.FindGameObjectByName(objects[0]);
                        }
                        break;
                    default:
                        throw new Exception("GameComponent::BasicMeshComponent:: Unknown direct connection request to " + parts[0]);
                }
            }
        }
    
        public override void DisConnect()
        {
            Parent = null;
            UpdateStage = 99;
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
            //mesh = new BasicMesh();
            //mesh.Load(MeshFilename);
            //SceneManager.Instance.currentScene.Assets.Add(MeshFilename.GetHashCode(), mesh);
        }

        public override void ReConnect(GameObject other)
        {
            BasicMeshComponent ot = (BasicMeshComponent)other.FindGameComponentByName(Name);
            MeshFilename = ot.MeshFilename;
            //mesh = ot.mesh;
        }

        public override void ReplaceComponent(ECSGameComponent old, ECSGameComponent replacement)
        {
        }

        public override void RenderOffscreenRenderTargets()
        {
        }

        public override void SetParameter(string Name, string Value)
        {
            if (Name == "Filename")
            {
                MeshFilename = Value;
            }
        }

        public override void Update(float dt)
        {
            
        }

        #endregion

        #region public methods
        public Matrix GetWorldMatrix()
        {
            return World;
        }
        #endregion

    }
}
