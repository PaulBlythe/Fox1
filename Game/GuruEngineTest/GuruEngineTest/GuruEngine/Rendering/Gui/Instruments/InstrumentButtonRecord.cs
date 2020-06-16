using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

using GuruEngine.ECS;

namespace GuruEngine.Rendering.Gui.Instruments
{
    public class InstrumentButtonRecord
    {
        public ECSGameComponent Host;
        public Rectangle Location;
        public String DownEvent;
        public String UpEvent;
        public bool DeBounce;
        public int state;

        public InstrumentButtonRecord()
        {
            state = 0;
        }

        public InstrumentButtonRecord(ECSGameComponent host, Rectangle location, String down, String up, bool deBounce)
        {
            Host = host;
            Location = location;
            DownEvent = down;
            UpEvent = up;
            DeBounce = deBounce;
            state = 0;
        }

        
    }
}
