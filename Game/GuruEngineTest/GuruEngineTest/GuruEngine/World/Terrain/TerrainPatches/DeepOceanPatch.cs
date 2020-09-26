using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using GuruEngine.Physics.World.Geodetic;
using GuruEngine.Rendering.VertexDeclarations;
using GuruEngine.Rendering;
using GuruEngine.Rendering.RenderCommands;

namespace GuruEngine.World.Terrain.TerrainPatches
{
    public class DeepOceanPatch: TerrainPatch
    {
        double Latitude, Longitude;
       
        double xscale, yscale;
        bool visible = true;
        bool loaded = false;

        int myHeight = 128;
        int myWidth = 128;

        private Vector3 basePosition;

        RenderCommandSet rcs;

        

        public DeepOceanPatch(double x,double y)
        {
            Latitude = y;
            Longitude = x;

            basePosition = new Vector3((float)Longitude * 111200.0f, 0, (float)Latitude * 111200.0f);

            xscale = (111200.0f / 60.0f) / (myWidth - 1);
            yscale = (111200.0f / 60.0f) / (myHeight - 1);

        }
        public override void Update()
        {
            if (!loaded)
            {
                double dl = Latitude - World.Instance.cameraCoordinates.Latitude.Degrees;
                double dl2 = Longitude - World.Instance.cameraCoordinates.Longitude.Degrees;
                double dd = (dl * dl) + (dl2 * dl2);
                double testd  = 0.095f;
                if(dd<(testd*testd)) 
                {

                    rcs = new RenderCommandSet();
                    rcs.RenderPass = RenderPasses.Terrain;
                    rcs.IsStaticMesh = false;
                    rcs.RS = RasteriserStates.NormalNoCull;
                    rcs.DS = DepthStencilState.Default;

                    RenderDeepOcean rdo = new RenderDeepOcean(myWidth,myHeight);
                    rdo.vbuffer = BufferManager.GetNamedVertexBuffer("OceanVB");
                    rdo.ibuffer = BufferManager.GetNamedIndexBuffer("OceanIB");
                    rdo.World =  Matrix.CreateScale((float)xscale, 1, (float)yscale) * Matrix.CreateTranslation(basePosition);

                    rcs.Commands.Add(rdo);

                    loaded = true;
                }
            }
            if ((loaded) && (visible))
            {
                Vector3 size = new Vector3((float)xscale * 128, 1, (float)yscale * 128);

                Vector3 min = basePosition - size;
                Vector3 max = basePosition + size;
                BoundingBox b = new BoundingBox(min, max);
                if (WorldState.GetWorldState().camera.BoundingVolumeIsInView(b))
                {
                    Renderer.AddRenderCommand(rcs);
                }
                
                
            }
            
        }

        public override void Destroy()
        {
            BufferManager.RemoveNamedIndexBuffer("OceanIB");
            BufferManager.RemoveNamedVertexBuffer("OceanVB");
        }

        public Vector3 ConvertToPosition(double Latitude, double Longitude, double Altitude)
        {
            Latitude *= 0.0174533;
            Longitude *= 0.0174533;
            double a = 6378137;
            double b = 6356800;
            double N;
            double e = 1 - (Math.Pow(b, 2) / Math.Pow(a, 2));
            N = a / (Math.Sqrt(1.0 - (e * Math.Pow(Math.Sin(Latitude), 2))));
            double cosLatRad = Math.Cos(Latitude);
            double cosLongiRad = Math.Cos(Longitude);
            double sinLatRad = Math.Sin(Latitude);
            double sinLongiRad = Math.Sin(Longitude);
            double x = (N + Altitude) * cosLatRad * cosLongiRad;
            double y = (N + Altitude) * cosLatRad * sinLongiRad;
            double z = ((Math.Pow(b, 2) / Math.Pow(a, 2)) * N + Altitude) * sinLatRad;

            return new Vector3((float)x, (float)y, (float)z);
        }
    }
}
