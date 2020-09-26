using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using GuruEngine.Physics.World;
using GuruEngine.DebugHelpers;
using GuruEngine.Rendering.EffectPasses;
using GuruEngine.Rendering;

namespace GuruEngine.Player.HumanFactors
{
    public class Hypoxia
    {

        HypoxiaEffectPass passdata = new HypoxiaEffectPass();

        /// <summary>
        /// 0-3         0 no hypoxia, 1 hypoxia, 2 unconcious , 3 dead
        /// </summary>
        public float HypoxiaLevel;

        const float MAX_HEIGHT = 60000;

        /// <summary>
        /// Altitude bands in feet
        /// </summary>
        float[] altitudes = new float[]
        {
            15000,24000,25500,27000,31500,36000,40500,49500,MAX_HEIGHT
        };

        /// <summary>
        /// How long you can stay concious at altitude without oxygen in seconds
        /// </summary>
        float[] timings = new float[]
        {
            2.5f*60,60,30,20,15,15,12,12,12
        };


        /// <summary>
        /// Updates hypoxic state
        /// </summary>
        /// <param name="altitude"> Metres </param>
        /// <param name="dt"> Seconds </param>
        public void Update(float altitude, float dt)
        {
            float alt = (float)(altitude / Constants.fttom);
            if (alt < altitudes[0])
            {
                // no new hypoxia, chance to recover if was hypoxic
                if (HypoxiaLevel > 0)
                {
                    HypoxiaLevel -= dt / 30.0f;
                    HypoxiaLevel = Math.Max(0, HypoxiaLevel);
                }
            }
            else
            {
                float low_alt = 0;
                float high_alt = 0;
                int index_1 = 0;
                int index_2 = 0;
                if (alt>MAX_HEIGHT)
                {
                    alt = MAX_HEIGHT;
                    high_alt = MAX_HEIGHT;
                    index_1 = timings.Length - 2;
                    low_alt = altitudes[index_1];
                    index_2 = index_1 + 1;
                }
                else
                {
                    while (altitudes[index_1] > alt)
                    {
                        index_1++;
                    }
                    index_2 = index_1 + 1;
                    low_alt = altitudes[index_1];
                    high_alt = altitudes[index_2];
                }

                float delta = (alt - low_alt) / (high_alt - low_alt);
                double time = Maths.MathUtils.Lerp(timings[index_1], timings[index_2], delta);
                
                HypoxiaLevel += (float)(dt / time);
                if (HypoxiaLevel>1)
                {
                    passdata.BlurLevel = Math.Min(1, HypoxiaLevel - 1);
                    passdata.BlackoutLevel = 0.0f;
                    if (HypoxiaLevel>1.5f)
                    {
                        passdata.BlackoutLevel =   (HypoxiaLevel - 1.5f) / 2;
                        passdata.BlackoutLevel = Math.Min(1, passdata.BlackoutLevel);
                    }
                    Math.Min(Math.Max(0, HypoxiaLevel - 1.5f), 1);
                    Renderer.AddEffectPass(passdata);
                }
            }
        }
    }
}
