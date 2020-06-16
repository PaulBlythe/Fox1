using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using Microsoft.Xna.Framework;
using GuruEngine.ECS.Components;
using GuruEngine.DebugHelpers;
using GuruEngine.Assets;
using GuruEngine.Rendering.Particles;
using GuruEngine.Rendering;
using GuruEngine.ECS.Components.Mesh;

// ======================================================== //
// Known actions                                            //
// =======================================================  //
// Engine start                                             //
// ======================================================== //

namespace GuruEngine.AI.Scripting.Scripts.Aircraft.British.Fighter
{
    public class SpitfireIXc :Script
    {
        AircraftStateComponent hostcomponent = null;
        List<String> activeActions = new List<string>();
        float timer = 0;

        #region Engine start particle system variables
        int EngineStartParticleSystemID;
        ParticleSettings EngineStartParticleSettings = null;
        int EngineStartParticleTextureID = -1;
        long EngineStartParticleSystem = -1;
        MultiMeshComponent engine;
        String[] EngineStartHooks = new string[]
        {
            "__Engine1EF_01_<BASE>",
            "__Engine1EF_02_<BASE>",
            "__Engine1EF_03_<BASE>",
            "__Engine1EF_04_<BASE>",
            "__Engine1EF_05_<BASE>",
            "__Engine1EF_06_<BASE>",
            "__Engine1EF_07_<BASE>",
            "__Engine1EF_08_<BASE>",
            "__Engine1EF_09_<BASE>",
            "__Engine1EF_10_<BASE>",
            "__Engine1EF_11_<BASE>",
            "__Engine1EF_12_<BASE>"
        };
        #endregion

        public override void CreateObjects()
        {
            String dir = Path.Combine(FilePaths.DataPath, "/ParticleSystems");
            String file = Path.Combine(dir, "engine_start.xml");
            AssetManager.AddParticleSystemToQue(file);
            EngineStartParticleSystemID = file.GetHashCode();

        }

        public override void AbortAction(bool all)
        {
            activeActions.Clear();
        }

        public override void StartAction(string name)
        {
            activeActions.Add(name);
        }

        public override void UpdateScript(float dt)
        {
            if (hostcomponent == null)
            {
                hostcomponent = (AircraftStateComponent)Host;
                engine = hostcomponent.Parent.FindHookOwner(EngineStartHooks[0]);
            }

            #region Async load engine start particle system
            if (EngineStartParticleSettings == null)
            {
                EngineStartParticleSettings  = AssetManager.GetParticleSettings(EngineStartParticleSystemID);
            }
            if ((EngineStartParticleTextureID == -1) && (EngineStartParticleSettings != null))
            {
                String dir = Path.Combine(FilePaths.DataPath, @"\Textures\Particles");
                String file = Path.Combine(dir, EngineStartParticleSettings.TextureName);
                AssetManager.AddTextureToQue(file);
                EngineStartParticleTextureID = file.GetHashCode();
            }
            if ((EngineStartParticleSystem == -1) &&(AssetManager.IsReady(EngineStartParticleTextureID)))
            {
                EngineStartParticleSystem = Renderer.GetCurrentRenderer().current.AddParticleSystem(EngineStartParticleSettings);
            }
            #endregion


            if (activeActions.Count>0)
            {
                timer -= dt;
                if (timer<=0)
                {
                    switch (activeActions.ElementAt(0))
                    {
                        case "Engine start":
                            {
                                activeActions.RemoveAt(0);              // throw away Engine start
                                activeActions.Insert(0, "Magneto 2");   // add all other actions    
                                activeActions.Insert(1, "Pump 1");
                                activeActions.Insert(2, "Pump 2");
                                activeActions.Insert(3, "Pump 3");
                                activeActions.Insert(4, "Pump 4");
                                activeActions.Insert(5, "Pump 5");
                                activeActions.Insert(6, "Throttle up");
                                activeActions.Insert(7, "Fire starter");

                                hostcomponent.SetVar("Magneto1", 1);

                                timer = 1;
#if DEBUG
                                if (hostcomponent.IsPlayer())
                                    DebugMessageQueue.Instance.AddDebugMessage("Starting engine");
#endif
                            }
                            break;

                        case "Magneto 2":
                            activeActions.RemoveAt(0);
                            hostcomponent.SetVar("Magneto2", 1);
#if DEBUG
                            if (hostcomponent.IsPlayer())
                                DebugMessageQueue.Instance.AddDebugMessage("Magnetos on");
#endif
                            timer = 0.5f;
                            break;

                        case "Pump 1":
                        case "Pump 2":
                        case "Pump 3":
                        case "Pump 4":
                            activeActions.RemoveAt(0);
#if DEBUG
                            if (hostcomponent.IsPlayer())
                            {
                                DebugMessageQueue.Instance.AddDebugMessage("Pumping primer");
                            }
#endif
                            timer = 0.5f;
                            break;

                        case "Pump 5":
                            activeActions.RemoveAt(0);
#if DEBUG
                            if (hostcomponent.IsPlayer())
                            {
                                DebugMessageQueue.Instance.AddDebugMessage("Pumping primer");
                                DebugMessageQueue.Instance.AddDebugMessage("Throttle to 33%");
                            }
#endif
                            timer = 1.5f;
                            break;

                        case "Throttle up":
                            {
                                double throttlepos = hostcomponent.GetVar("Engine1ThrottlePCT", 0);
                                throttlepos += 33.3333 * dt;
                                if (throttlepos>33.3333)
                                {
                                    throttlepos = 33.3333;
                                    activeActions.RemoveAt(0);
                                }
                                hostcomponent.SetVar("Engine1ThrottlePCT", throttlepos);

                            }
                            break;
                        case "Fire starter":
                            activeActions.RemoveAt(0);

                            for (int i=0; i<EngineStartHooks.Length; i++)
                            {
                                Matrix m = engine.GetHookMatrix(EngineStartHooks[i]);
                                for (int j=0; j<5; j++)
                                {
                                    Renderer.GetCurrentRenderer().current.GetParticleSystem(EngineStartParticleSystem).AddParticle(m.Translation, 4.0f * m.Forward);
                                }
                            }
#if DEBUG
                            if (hostcomponent.IsPlayer())
                                DebugMessageQueue.Instance.AddDebugMessage("Firing starter");
#endif
                            break;

                    }
                }
            }
        }

        public override bool CheckBooleanState(string name)
        {
            switch (name)
            {
                case "EngineHasSpark":
                    {
                        double t = hostcomponent.GetVar("Magneto1", 0) + hostcomponent.GetVar("Magneto2", 0);
                        return (t > 1.9);
                    }
            }
            return false;
        }

    }
}
