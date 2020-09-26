using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

using GuruEngine.Helpers;

//( Class TwoDArrayComponent )
//( Group Undefined )
//( Type TwoDArrayComponent )
//( Parameter Array1D Column )
//( Parameter Array1D Row )
//( Parameter Array2D Data )

namespace GuruEngine.ECS.Components.Generic
{
    public class TwoDArrayComponent:ECSGameComponent
    {
        public double[] ColumnValues;
        public double[] RowValues;
        public double[,] DataValues;
        int cols;
        int rows;

        #region ECSGameComponent methods
        public override void SetParameter(string Name, string Value)
        {
            String[] parts = Name.Split(':');
            switch (parts[0])
            {
                case "Column":
                    {
                        cols = int.Parse(parts[1]);
                        ColumnValues = new double[cols];
                        for(int i=0; i<cols; i++)
                        {
                            ColumnValues[i] = double.Parse(parts[i + 2]);
                        }
                    }
                    break;

                case "Row":
                    {
                        rows = int.Parse(parts[1]);
                        RowValues = new double[rows];
                        for (int i = 0; i < rows; i++)
                        {
                            RowValues[i] = double.Parse(parts[i + 2]);
                        }
                    }
                    break;

                case "Data":
                    {
                        DataValues = new double[rows, cols];
                        int k = 1;
                        for (int i=0; i<cols; i++)
                        {
                            for (int j=0; j<rows; j++)
                            {
                                DataValues[j, i] = double.Parse(parts[k]);
                                k++;
                            }
                        }
                    }
                    break;
            }
        }

        /// <summary>
        /// Create a new instance
        /// </summary>
        /// <returns></returns>
        public override ECSGameComponent Clone()
        {
            TwoDArrayComponent res = new TwoDArrayComponent();
            res.ColumnValues = ColumnValues;
            res.RowValues = RowValues;
            res.DataValues = DataValues;
            return res;
        }

        /// <summary>
        /// Connect up the component
        /// Only connects to root
        /// </summary>
        /// <param name="components"></param>
        /// <param name="isList"></param>
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
                        throw new Exception("GameComponent::RadarCrossSectionComponent:: Unknown direct connection request to " + parts[0]);
                }
            }

        }

        public override void DisConnect()
        {
        }

        /// <summary>
        /// The only contained object is the radar cross srction
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
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

        public override void ReConnect(GameObject other)
        {
        }

        public override void RenderOffscreenRenderTargets()
        {
        }
        public override void ReplaceComponent(ECSGameComponent old, ECSGameComponent replacement)
        {
        }
        public override void Update(float dt)
        {
        }
        #endregion

        #region TwoDArrayComponent methods
        public double GetValue(double colkey, double rowkey)
        {
            return MathsHelper.TableLookup2D(rowkey, colkey, ColumnValues, RowValues, DataValues);
        }
        #endregion
    }
}
