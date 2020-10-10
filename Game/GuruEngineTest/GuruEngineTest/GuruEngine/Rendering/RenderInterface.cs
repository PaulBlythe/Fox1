using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System.Threading;

using GuruEngine.World;
using GuruEngine.Rendering.Particles;

namespace GuruEngine.Rendering
{
    public abstract class RenderInterface
    {
        public abstract void Initialise();
        public abstract void CleanUp();
        public abstract void Draw(WorldState state, GameTime gt);
        public abstract void AddShader(String name, Effect fx);
        public abstract Effect ApplyShader(WorldState state, RenderCommand r);
        public abstract void AddPointLight(Vector3 pos, Color color, float rad, float intensity);
        public abstract void AddDirectionalLight(Vector3 dir, Color col, bool isSun, bool isMoon);

        public List<RenderCommandSet> updatingRenderCommands;
        public List<RenderCommandSet> renderingRenderCommands;
        public List<RenderCommandSet> bufferedRenderCommandsA;
        public List<RenderCommandSet> bufferedRenderCommandsB;
        public LightManager lightManagerA = new LightManager();
        public LightManager lightManagerB = new LightManager();
        public LightManager currentLightManager;
        public LightManager activeLightManager;

#if MULTI_THREADED
        public ManualResetEvent renderActive;
        public ManualResetEvent renderCommandsReady;
        public ManualResetEvent renderCompleted;
#endif

        public Dictionary<long, ParticleSystem> particleSystems = new Dictionary<long, ParticleSystem>();
        public long RendererUID = 0;

        public void AddCommand(RenderCommandSet rs)
        {
            lock (updatingRenderCommands)
            {
                updatingRenderCommands.Add(rs);
            }
        }

        public long AddParticleSystem(ParticleSettings settings)
        {
            RendererUID++;

            ParticleSystem sys = new ParticleSystem(Renderer.GetGraphicsDevice(), settings);
            particleSystems.Add(RendererUID, sys);

            return RendererUID;
        }

        public ParticleSystem GetParticleSystem(long id)
        {
            return particleSystems[id];
        }

        /// <summary>
        /// Swap buffers
        /// </summary>
        public void SwapBuffers()
        {
            if (updatingRenderCommands == bufferedRenderCommandsA)
            {
                updatingRenderCommands = bufferedRenderCommandsB;
                renderingRenderCommands = bufferedRenderCommandsA;
                currentLightManager = lightManagerB;
                activeLightManager = lightManagerA;
            }
            else if (updatingRenderCommands == bufferedRenderCommandsB)
            {
                updatingRenderCommands = bufferedRenderCommandsA;
                renderingRenderCommands = bufferedRenderCommandsB;
                currentLightManager = lightManagerA;
                activeLightManager = lightManagerB;
            }
            
            currentLightManager.Reset();
            updatingRenderCommands.Clear();
        }

        public Vector3 Copy(Vector3 orig)
        {
            return new Vector3(orig.X, orig.Y, orig.Z);
        }

        public Vector4 Copy(Vector4 orig)
        {
            return new Vector4(orig.X, orig.Y, orig.Z, orig.W);
        }

        public Matrix Copy(Matrix orig)
        {
            return Matrix.Identity * orig;
        }

        public void EndFrame()
        {
#if MULTI_THREADED
            renderCompleted.WaitOne();
            renderCommandsReady.Set();
#endif
        }

        public void StartFrame()
        {
#if MULTI_THREADED
            renderActive.WaitOne();
            renderCommandsReady.Reset();
#endif

        }

        public void WaitForUpdateToComplete()
        {
#if MULTI_THREADED
            renderCommandsReady.WaitOne();          // wait for update to finish
            renderCompleted.Reset();
#endif
        }

        public void TellUpdateToContinue()
        {
#if MULTI_THREADED
            renderActive.Set();                     // tell update to continue
#endif
        }

        public void SignalRenderingComplete()
        {
#if MULTI_THREADED
            renderActive.Reset();
#endif
        }

        public void SignalRendererFinished()
        {
#if MULTI_THREADED
            renderCompleted.Set();
#endif
        }


    }
}
