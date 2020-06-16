using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

using GuruEngine.Physics.Collision;
using GuruEngine.DebugHelpers;

//( Class CollisionMeshComponent )
//( Type CollisionMeshComponent )
//( Parameter String Filename )

namespace GuruEngine.ECS.Components.Mesh
{
    public class CollisionMeshComponent :ECSGameComponent
    {
        public CollisionMesh mesh;

        #region ECSGameComponent methods

        public override ECSGameComponent Clone()
        {
            CollisionMeshComponent cmc = new CollisionMeshComponent();
            cmc.mesh = new CollisionMesh(mesh);
            return (ECSGameComponent)cmc;
        }

        public override void Connect(string components, bool isList)
        {
            string[] parts = components.Split('#');
            if (isList)
            {

            }
           else
            {
                switch (parts[1])
                {
                    case "Root":
                        {
                            string[] objects = parts[2].Split(':');
                            Parent = GameObjectManager.Instance.FindGameObjectByName(objects[0]);
                        }
                        break;
                   
                    default:
                        throw new Exception("GameComponent::OnAPG68v5:: Unknown direct connection request to " + parts[0]);
                }
            }
        }

        public override void DisConnect()
        {
            mesh = null;
        }

        public override object GetContainedObject(string type)
        {
            switch (type)
            {
                case "CollisionMesh":
                    {
                        return (object)mesh;
                    }
            }
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
            CollisionMeshComponent otherC = (CollisionMeshComponent)other.FindGameComponentByName(Name);
            otherC.mesh = mesh;
        }

        public override void RenderOffscreenRenderTargets()
        {
        }

        public override void SetParameter(string Name, string Value)
        {
            if (Name == "Filename")
            {
                mesh = new CollisionMesh();
                mesh.Load(Path.Combine(FilePaths.DataPath, Value));
                UpdateStage = 4;
            }
        }

        public override void Update(float dt)
        {
#if DEBUG
            if (DebugRenderSettings.RenderCollisionMeshes)
            {
                if (Name.Contains("_D0"))
                {
                    mesh.DrawCollisionMesh(world);
                }
            }
#endif
        }

#if DEBUG
        Matrix world;
        public void SetMatrix(Matrix w)
        {
            world = w;
        }
#endif

#endregion
    }
}
