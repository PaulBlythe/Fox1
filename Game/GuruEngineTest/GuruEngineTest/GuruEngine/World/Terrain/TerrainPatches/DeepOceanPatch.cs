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
                    // Within range , need to generate structures
                     
                    //myVertices = new VertexPositionTexture[myWidth * myHeight];
                    //
                    //for (int x = 0; x < myWidth; x++)
                    //    for (int y = 0; y < myHeight; y++)
                    //    {
                    //        myVertices[x + y * myWidth].Position = new Vector3(y, 0, x);
                    //      
                    //        myVertices[x + y * myWidth].TextureCoordinate.X = (float)x / 10.0f;
                    //        myVertices[x + y * myWidth].TextureCoordinate.Y = (float)y / 10.0f;
                    //      
                    //    }
                    //
                    //vb = new VertexBuffer(MRenderer.Renderer.GetGraphicsDevice(), VertexPositionTexture.VertexDeclaration, myWidth*myHeight, BufferUsage.WriteOnly);
                    //vb.SetData(myVertices);
                    //
                    ////Index 
                    //short[] terrainIndices = new short[(myWidth - 1) * (myHeight - 1) * 6];
                    //for (short x = 0; x < myWidth - 1; x++)
                    //{
                    //    for (short y = 0; y < myHeight - 1; y++)
                    //    {
                    //        terrainIndices[(x + y * (myWidth - 1)) * 6] = (short)((x + 1) + (y + 1) * myWidth);
                    //        terrainIndices[(x + y * (myWidth - 1)) * 6 + 1] = (short)((x + 1) + y * myWidth);
                    //        terrainIndices[(x + y * (myWidth - 1)) * 6 + 2] = (short)(x + y * myWidth);
                    //
                    //        terrainIndices[(x + y * (myWidth - 1)) * 6 + 3] = (short)((x + 1) + (y + 1) * myWidth);
                    //        terrainIndices[(x + y * (myWidth - 1)) * 6 + 4] = (short)(x + y * myWidth);
                    //        terrainIndices[(x + y * (myWidth - 1)) * 6 + 5] = (short)(x + (y + 1) * myWidth);
                    //    }
                    //}
                    //
                    //ib = new IndexBuffer(MRenderer.Renderer.GetGraphicsDevice(), typeof(short), (myWidth - 1) * (myHeight - 1) * 6, BufferUsage.WriteOnly);
                    //ib.SetData(terrainIndices);

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
                Renderer.AddRenderCommand(rcs);
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
