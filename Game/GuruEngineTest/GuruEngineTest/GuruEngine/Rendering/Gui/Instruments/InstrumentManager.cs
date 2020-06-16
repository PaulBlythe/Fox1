using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

using GuruEngine.ECS;

namespace GuruEngine.Rendering.Gui.Instruments
{
    public class InstrumentManagerRecord
    {
        public InstrumentManager manager;
        public Rectangle screenRegion;

        public InstrumentManagerRecord(InstrumentManager masn, Rectangle r)
        {
            manager = masn;
            screenRegion = r;
        }
    }

    public class InstrumentManager
    {
        public List<InstrumentButtonRecord> buttons = new List<InstrumentButtonRecord>();

        public void RegisterButton(ECSGameComponent instrument, Rectangle location, String downEvent, String upEvent, bool deBounce)
        {
            InstrumentButtonRecord ins = new InstrumentButtonRecord(instrument, location, downEvent, upEvent, deBounce);
            buttons.Add(ins);
        }

        public void Update(Vector2 mousePos, bool down)
        {
            foreach (InstrumentButtonRecord ibr in buttons)
            {
                if (ibr.Location.Contains(mousePos))
                {
                    if (down)
                    {
                        if (!ibr.DeBounce)
                            ibr.Host.HandleEvent(ibr.DownEvent);

                        else if (ibr.state == 0)
                        {
                            ibr.state = 1;
                            ibr.Host.HandleEvent(ibr.DownEvent);
                        }
                        return;
                    }
                    else
                    {
                        if (!ibr.DeBounce)
                            ibr.Host.HandleEvent(ibr.UpEvent);
                        else if (ibr.state == 1)
                        {
                            ibr.Host.HandleEvent(ibr.UpEvent);
                            ibr.state = 0;
                        }
                    }
                }
                else
                {
                    if (ibr.state == 1)
                    {
                        ibr.Host.HandleEvent(ibr.UpEvent);
                        ibr.state = 0;
                    }
                }
            }
        }
    }
}
