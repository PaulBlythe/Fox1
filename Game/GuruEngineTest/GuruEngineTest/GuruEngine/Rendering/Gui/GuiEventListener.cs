using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GuruEngine.Rendering.Gui
{
    public interface GuiEventListener
    {
        void HandleEvent(String message);
    }
}
