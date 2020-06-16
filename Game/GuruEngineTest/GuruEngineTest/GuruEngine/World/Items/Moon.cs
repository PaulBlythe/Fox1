using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using GuruEngine.Rendering;
using GuruEngine.Rendering.RenderCommands;
using GuruEngine.Assets;


namespace GuruEngine.World.Items
{
    public class Moon : WorldItem
    {
        int GUID;
        int moonGUID;
        int shaderGUID;

        Effect fx;
        RenderCommandSet rendercommand;
        bool mapped = false;

        public Moon()
        {
            String mesh = @"StaticMeshes\World\sphere2";
            AssetManager.AddStaticMeshToQue(mesh);
            GUID = mesh.GetHashCode();

            String day = FilePaths.DataPath + @"Textures\2k_moon.png";
            AssetManager.AddTextureToQue(day);
            moonGUID = day.GetHashCode();

            String shader = @"Shaders\Forward\Moon";
            AssetManager.AddShaderToQue(shader);
            shaderGUID = shader.GetHashCode();

        }


        public override void Update(WorldState state)
        {
            if (!mapped)
            {
                Model mesh = AssetManager.StaticMesh(GUID);
                if (mesh == null)
                    return;

                fx = AssetManager.Shader(shaderGUID);
                if (fx == null)
                    return;

                Texture2D day = AssetManager.Texture(moonGUID);
                if (day == null)
                    return;


                foreach (ModelMesh meshp in mesh.Meshes)
                {
                    foreach (ModelMeshPart mp in meshp.MeshParts)
                    {
                        mp.Effect = fx;
                    }
                }
                rendercommand = new RenderCommandSet();
                rendercommand.IsStaticMesh = false;
                rendercommand.DS = DepthStencilState.None;
                rendercommand.mesh = mesh;
                rendercommand.RS = RasteriserStates.NoDepth;
                rendercommand.RenderPass = RenderPasses.Ephemeris;
                rendercommand.fx = fx;
                rendercommand.blend = BlendState.AlphaBlend;

                RenderMoonCommand rmc = new RenderMoonCommand(mesh,fx,day);
                rendercommand.Commands.Add(rmc);

                mapped = true;
            }
           
            Vector3 textPosition = WorldState.GetWorldState().MoonPosition.ToVector3F();
            textPosition.Normalize();
            Renderer.AddDirectionalLight(-textPosition, Color.Gray, false, true);
            Renderer.AddRenderCommand(rendercommand);
        }
    }
}
