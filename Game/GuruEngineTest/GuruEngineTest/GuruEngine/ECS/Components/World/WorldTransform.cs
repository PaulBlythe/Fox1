using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

using GuruEngine.Maths;
using GuruEngine.Helpers;


//( Class WorldTransform )
//( Type Transform )
//( ConnectionList All Consumers )


namespace GuruEngine.ECS.Components.World
{
    public class WorldTransform : ECSGameComponent
    {
        public Quaternion Orientation;
        public Vector3 LocalPosition;
        public Vector3 DeltaV;
        public float Velocity;
        GameObject parent;
        Vector2d GlobalPosition;
        public Matrix world;

        public Vector3 WorldVelocity = new Vector3(0, 0, 0);
        public Vector3 WorldRotationalVelocity = new Vector3(0, 0, 0);
        public Vector3 WorldAcceleration = new Vector3(0, 0, 0);
        public Vector3 WorldAngularAcceleration = new Vector3(0,0,0);

        public Vector3 LocalVelocity = new Vector3(0, 0, 0);
        public Vector3 LocalRotationalVelocity = new Vector3(0, 0, 0);
        public Vector3 LocalAngularAcceleration = new Vector3(0, 0, 0);
        public Vector3 LocalAngularVelocity = new Vector3(0, 0, 0);

        public WorldTransform()
        {
            parent = null;
            Orientation = Quaternion.Identity;
            LocalPosition = Vector3.Zero;
            GlobalPosition = new Vector2d(0, 0);
            Velocity = 0;
        }

        public Quaternion GetOrientation()
        {
            return Orientation;
        }

        public Vector3 GetLocalPosition()
        {
            return LocalPosition;
        }

        public Matrix GetMatrix()
        {
            return world;
        }

        public void UpdateTransform(Vector3 position, Matrix or)
        {
            LocalPosition = position;
            Orientation = Quaternion.CreateFromRotationMatrix(or);
        }

        #region ECSGameComponent Interface

        public override void Connect(string components, bool isList)
        {
            string[] parts = components.Split('#');
            switch (parts[0])
            {
                case "Root":
                    {
                        string[] objects = parts[2].Split(':');
                        parent = GameObjectManager.Instance.FindGameObjectByName(objects[0]);
                    }
                    break;
            }
        }

        public override void DisConnect()
        {
            parent = null;
        }

        public override void SetParameter(string Name, string Value)
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
            
        }

        public override void RenderOffscreenRenderTargets()
        {
            
        }

        public override void Update(float dt)
        {
            world = Matrix.CreateFromQuaternion(Orientation) * Matrix.CreateTranslation(LocalPosition);
           
        }

        public override ECSGameComponent Clone()
        {
            return new WorldTransform();
        }

        public override void ReConnect(GameObject other)
        {
            
        }

        public override void ReplaceComponent(ECSGameComponent old, ECSGameComponent replacement)
        {
        }

        #endregion

        public void SetOrientation(Quaternion inq)
        {
            Orientation = inq;
        }

        public void SetPosition(Vector3 inpos)
        {
            LocalPosition = inpos;
        }

    }
}
