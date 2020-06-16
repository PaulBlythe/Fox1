using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;


namespace GuruEngine.Rendering
{
    public class RenderCommandSet
    {
        public RasteriserStates RS;
        public DepthStencilState DS;
      
        public List<RenderCommand> Commands = new List<RenderCommand>();

        public bool IsStaticMesh = false;
        public Model mesh;
        public Effect fx;
        public int RenderPass = 0;
        public BlendState blend = BlendState.NonPremultiplied;      // ONLY USED FOR STATIC MESHES
        public Matrix World;                                        // ONLY USED FOR STATIC MESHES
        public Matrix View;                                         // ONLY USED FOR STATIC MESHES

        public void Clear()
        {
            mesh = null;
            fx = null;
            Commands.Clear();
        }
    }
}
