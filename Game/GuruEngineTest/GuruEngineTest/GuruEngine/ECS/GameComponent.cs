using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Content;

namespace GuruEngine.ECS
{
    public abstract class ECSGameComponent
    {
        public String Name;
        public GameObject Parent;

        public int UpdateStage;

        public abstract void Connect(String components, bool isList);
        public abstract void DisConnect();
        public abstract void Update(float dt);
        public abstract void Load(ContentManager content);
        public abstract void HandleEvent(String evt);
        public abstract void RenderOffscreenRenderTargets();
        public abstract Texture2D GetOffscreenTexture();
        public abstract object GetContainedObject(String type);

        public abstract ECSGameComponent Clone();
        public abstract void ReConnect(GameObject other);
        public abstract void SetParameter(String Name, String Value);

        public void CopyMatrix(ref Matrix m1, ref Matrix m2)
        {
            m1.M11 = m2.M11;
            m1.M12 = m2.M12;
            m1.M13 = m2.M13;
            m1.M14 = m2.M14;

            m1.M21 = m2.M21;
            m1.M22 = m2.M22;
            m1.M23 = m2.M23;
            m1.M24 = m2.M24;

            m1.M31 = m2.M31;
            m1.M32 = m2.M32;
            m1.M33 = m2.M33;
            m1.M34 = m2.M34;

            m1.M41 = m2.M41;
            m1.M42 = m2.M42;
            m1.M43 = m2.M43;
            m1.M44 = m2.M44;

        }
    }
}
