using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

using GuruEngine.Rendering;

namespace GuruEngine.World
{
    public class UpdateLoop
    {
        private readonly Stopwatch _stopwatch;
        private long _lastElapsed;
        Renderer renderer;

        public UpdateLoop(Renderer Renderer)
        {
            _stopwatch = new Stopwatch();
            renderer = Renderer;
        }

        public void Loop()
        {
            _stopwatch.Start();
            while (true)
            {
                Update();
            }
        }

        private void Update()
        {
            var elapsedMilliseconds = _stopwatch.ElapsedMilliseconds;
            var elapsed = elapsedMilliseconds - _lastElapsed;
            _lastElapsed = elapsedMilliseconds;
            Update(elapsed);
        }

        private void Update(float delta)
        {
            {
#if PROFILE
                Profiler.Start("Threaded update");
#endif
                renderer.StartFrame();
                Engine.UpdateAll(delta / 1000.0f);
                renderer.EndFrame();
                                    
#if PROFILE
                Profiler.End("Threaded update");
#endif
            }

        }

    }
}
