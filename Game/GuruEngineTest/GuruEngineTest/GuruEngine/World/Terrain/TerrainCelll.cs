using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using GuruEngine.World.Terrain.TerrainPatches;

namespace GuruEngine.World.Terrain
{
    /// <summary>
    /// A terrain cell covers 1 degree of latitude and longitude
    /// </summary>
    public class TerrainCell
    {
        TerrainPatch [,] patches = new TerrainPatch[60,60];

        /// <summary>
        /// Type map
        /// 0 = deep ocean
        /// 1 = terrain with sea
        /// 2 = terrain
        /// 3 = terrain with lakes
        /// </summary>
        byte[,] types = new byte[60, 60];

        public int latitude, longitude;

        public TerrainCell(int Latitude, int Longitude)
        {
            latitude = Latitude;
            longitude = Longitude;
            for (int i=0; i<60; i++)
            {
                for (int j=0; j<60; j++)
                {
                    patches[i, j] = null;
                    types[i, j] = 0;            // water world for now
                }
            }
        }

        public void Load()
        {
            for (int i=0; i<60; i++)
            {
                for (int j=0; j<60; j++)
                {
                    double x = longitude + (i / 60.0);
                    double y = latitude + (j / 60.0);
                    switch (types[i,j])
                    {
                        case 0:
                            {
                                patches[i, j] = new DeepOceanPatch(x, y);
                            }
                            break;
                    }


                }
            }
        }

        public void Update()
        {
            for (int i=0; i<60; i++)
            {
                for (int j=0; j<60; j++)
                {
                    patches[i, j].Update();
                }
            }
        }

        public void Destroy()
        {
            for (int i = 0; i < 60; i++)
            {
                for (int j = 0; j < 60; j++)
                {
                    patches[i, j].Destroy();
                    patches[i, j] = null;
                }
            }
        }
    }
}
