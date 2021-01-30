using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

using GuruEngine.Assets;
using GuruEngine.Helpers;
using GuruEngine.Rendering.Particles;
using GuruEngine.Rendering;
using GuruEngine.ECS.Components.World;

//( Class ParticleEmitterComponent )
//( Type ParticleEmitterComponent )
//( Connection Transform WorldTransform )
//( Parameter String Settings )
//( Parameter Float Rate )
//( Parameter String Mode )

namespace GuruEngine.ECS.Components.Effects
{
    public class ParticleEmitterComponent : ECSGameComponent
    {
        public WorldTransform host;
        public String settings_file = null;
        public long ParticleSystemID = -1;
        public long psystem = -1;
        public ParticleSettings psettings = null;
        public String Mode;

        public float rate = 0;
        int psysid;
        int texid = -1;
        Texture2D part_tex;
        float elapsedTime = 0;

        float timeBetweenParticles;
        Vector3 previousPosition;
        float timeLeftOver;

        #region ECS game component interface
        public override ECSGameComponent Clone()
        {
            ParticleEmitterComponent other = new ParticleEmitterComponent();
            other.settings_file = settings_file;
            other.host = host;
            other.Mode = Mode;
            other.rate = rate;
            return other;
        }

        public override void Connect(string components, bool isList)
        {
            string[] parts = components.Split('#');
            if (isList)
            {

            }
            else
            {
                string[] objects = parts[2].Split(':');
                switch (parts[0])
                {
                    case "Root":
                        {
                            Parent = GameObjectManager.Instance.FindGameObjectByName(objects[0]);
                        }
                        break;

                    case "Transform":
                        {
                            host = (WorldTransform)Parent.FindGameComponentByName(objects[0]);
                        }
                        break;
                    default:
                        throw new Exception("ParticleEmitterComponent:: Unknown direct connection request to " + parts[0]);
                }
            }
        }

        public override void DisConnect()
        {
            Parent = null;
            host = null;
            UpdateStage = 99;
            ParticleSystemID = -1;
            rate = 0;
            AssetManager.RemoveParticleSettingsReference(psysid);
            AssetManager.RemoveTextureReference(texid);
            texid = -2;
            psysid = -1;
        }

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
            if (settings_file != null)
            {
                String dir = Path.Combine(FilePaths.DataPath, "/ParticleSystems");
                String file = Path.Combine(dir, settings_file);
                AssetManager.AddParticleSystemToQue(file);
                psysid = file.GetHashCode();
                texid = -2;
            }
            timeBetweenParticles = 1.0f / rate;
            UpdateStage = 3;
        }

        public override void ReConnect(GameObject other)
        {
            ParticleEmitterComponent ot = (ParticleEmitterComponent)other.FindGameComponentByName(Name);
            ot.host = (WorldTransform)other.FindGameComponentByName(host.Name);
            ot.settings_file = settings_file;
            ot.rate = rate;
            ot.Mode = Mode;
            ot.Parent = other;
            ot.psystem = psystem;
        }

        public override void RenderOffscreenRenderTargets()
        {
        }

        public override void SetParameter(string Name, string Value)
        {
            switch (Name)
            {
                case "Mode":
                    Mode = Value;
                    break;

                case "Settings":
                    settings_file = Value;
                    break;

                case "Rate":
                    rate = float.Parse(Value);
                    break;
            }
        }

        public override void ReplaceComponent(ECSGameComponent old, ECSGameComponent replacement)
        {

        }

        public override void Update(float dt)
        {
           
            if ((psysid != -1) && (texid != -2))
            {
                if (psettings == null)
                    return;

                if (part_tex == null)
                    part_tex = AssetManager.Texture(texid);
                if (part_tex == null)
                    return;

                if (psystem == -1)
                {
                    psystem = Renderer.GetCurrentRenderer().current.AddParticleSystem(psettings);
                    timeBetweenParticles = 1.0f / rate;
                    previousPosition = host.LocalPosition; 
                }
                
                switch (Mode)
                {
                    case "Continuous":
                        {
                            Vector3 newPosition = host.LocalPosition;
                            
                            // Work out how much time has passed since the previous update.
                            elapsedTime += dt;

                            if (elapsedTime > 0)
                            {
                                // Work out how fast we are moving.
                                Vector3 velocity = (newPosition - previousPosition) / elapsedTime;

                                // If we had any time left over that we didn't use during the
                                // previous update, add that to the current elapsed time.
                                float timeToSpend = timeLeftOver + elapsedTime;

                                // Counter for looping over the time interval.
                                float currentTime = -timeLeftOver;

                                // Create particles as long as we have a big enough time interval.
                                while (timeToSpend > timeBetweenParticles)
                                {
                                    currentTime += timeBetweenParticles;
                                    timeToSpend -= timeBetweenParticles;

                                    // Work out the optimal position for this particle. This will produce
                                    // evenly spaced particles regardless of the object speed, particle
                                    // creation frequency, or game update rate.
                                    float mu = currentTime / elapsedTime;

                                    Vector3 position = Vector3.Lerp(previousPosition, newPosition, mu);

                                    // Create the particle.
                                    Renderer.GetCurrentRenderer().current.GetParticleSystem(psystem).AddParticle(position, velocity);

                                    //System.Console.WriteLine("Adding particle to system " + psystem.ToString() + " At " + position.ToString());
                                }

                                // Store any time we didn't use, so it can be part of the next update.
                                timeLeftOver = timeToSpend;
                            }

                            previousPosition = newPosition;
                        }
                        break;

                    case "OnDemand":
                        break;
                }
            }
            else
            {
                if (psettings == null)
                {
                    psettings = AssetManager.GetParticleSettings(psysid);
                }
                if ((texid == -2) && (psettings != null))
                {
                    String dir = Path.Combine(FilePaths.DataPath, @"Textures\Particles");
                    String file = Path.Combine(dir, psettings.TextureName);
                    AssetManager.AddTextureToQue(file);
                    texid = file.GetHashCode();
                }
            }
        }

        #endregion

        #region Particle emmitter methods

        public void AddParticle(Vector3 position, Vector3 velocity)
        {
            if (psystem != -1)
            {
                Renderer.GetCurrentRenderer().current.GetParticleSystem(psystem).AddParticle(position, velocity);

                //DebugHelpers.DebugLineDraw.DrawLine(position, position + (100 * Vector3.Up), Color.Red);
            }
        }
        #endregion
    }
}
