using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GuruEngine.World.Terrain
{
    public class TerrainManager
    {

        TerrainCell[,] cells = new TerrainCell[3, 3];

        int CentreLatitude;
        int CentreLongitude;

        public void Init(int latitude, int longitude)
        {
            CentreLatitude = latitude;
            CentreLongitude = longitude;

            cells[0, 0] = new TerrainCell(latitude - 1, longitude - 1);
            cells[1, 0] = new TerrainCell(latitude - 1, longitude);
            cells[2, 0] = new TerrainCell(latitude - 1, longitude + 1);

            cells[0, 1] = new TerrainCell(latitude, longitude - 1);
            cells[1, 1] = new TerrainCell(latitude, longitude);
            cells[2, 1] = new TerrainCell(latitude, longitude + 1);

            cells[0, 2] = new TerrainCell(latitude + 1, longitude - 1);
            cells[1, 2] = new TerrainCell(latitude + 1, longitude);
            cells[2, 2] = new TerrainCell(latitude + 1, longitude + 1);

            for (int i=0; i<3; i++)
            {
                for (int j=0; j<3; j++)
                {
                    cells[i, j].Load();
                }
            }
        }

        /// <summary>
        /// Clean up all objects
        /// </summary>
        public void Destroy()
        {
        }

        /// <summary>
        /// Update cells
        /// </summary>
        public void Update()
        {
            /// TODO
            /// Scroll map
            /// 
            for (int i=0; i<3; i++)
            {
                for (int j=0; j<3; j++)
                {
                    if (cells[i, j] != null)
                        cells[i, j].Update();
                }
            }
        }

    }
}
