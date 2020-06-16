using System;
using System.Xml;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;


namespace GuruEngine.Rendering.Particles
{
    /// <summary>
    /// Settings class describes all the tweakable options used
    /// to control the appearance of a particle system.
    /// </summary>
    public class ParticleSettings
    {
        // Name of the texture used by this particle system.
        public string TextureName = null;
        public string ShortTextureName = null;

        public int TextureCells = 1;

        // Maximum number of particles that can be displayed at one time.
        public int MaxParticles = 100;


        // How long these particles will last.
        public TimeSpan Duration = TimeSpan.FromSeconds(1);

        public float LifeTime
        {   get { return (Duration.Seconds * 1000) + Duration.Milliseconds; }
            set { Duration = TimeSpan.FromMilliseconds(value); }
        }


        // If greater than zero, some particles will last a shorter time than others.
        public float DurationRandomness = 0;


        // Controls how much particles are influenced by the velocity of the object
        // which created them. You can see this in action with the explosion effect,
        // where the flames continue to move in the same direction as the source
        // projectile. The projectile trail particles, on the other hand, set this
        // value very low so they are less affected by the velocity of the projectile.
        public float EmitterVelocitySensitivity = 1;


        // Range of values controlling how much X and Z axis velocity to give each
        // particle. Values for individual particles are randomly chosen from somewhere
        // between these limits.
        public float MinHorizontalVelocity = 0;
        public float MaxHorizontalVelocity = 0;


        // Range of values controlling how much Y axis velocity to give each particle.
        // Values for individual particles are randomly chosen from somewhere between
        // these limits.
        public float MinVerticalVelocity = 0;
        public float MaxVerticalVelocity = 0;


        // Direction and strength of the gravity effect. Note that this can point in any
        // direction, not just down! The fire effect points it upward to make the flames
        // rise, and the smoke plume points it sideways to simulate wind.
        public Vector3 Gravity = new Vector3(0, -9.81f, 0);
        float gs = 0;
        public float GravitySensitivity
        {
            get { return gs; }
            set
            {
                gs = value;
                Gravity = gs * new Vector3(0, -9.81f, 0);
            }
        }

        // Controls how the particle velocity will change over their lifetime. If set
        // to 1, particles will keep going at the same speed as when they were created.
        // If set to 0, particles will come to a complete stop right before they die.
        // Values greater than 1 make the particles speed up over time.
        public float EndVelocity = 1;


        // Range of values controlling the particle color and alpha. Values for
        // individual particles are randomly chosen from somewhere between these limits.
        public Color MinColor = Color.White;
        public Color MaxColor = Color.White;


        // Range of values controlling how fast the particles rotate. Values for
        // individual particles are randomly chosen from somewhere between these
        // limits. If both these values are set to 0, the particle system will
        // automatically switch to an alternative shader technique that does not
        // support rotation, and thus requires significantly less GPU power. This
        // means if you don't need the rotation effect, you may get a performance
        // boost from leaving these values at 0.
        public float MinRotateSpeed = 0;
        public float MaxRotateSpeed = 0;


        // Range of values controlling how big the particles are when first created.
        // Values for individual particles are randomly chosen from somewhere between
        // these limits.
        public float MinStartSize = 100;
        public float MaxStartSize = 100;


        // Range of values controlling how big particles become at the end of their
        // life. Values for individual particles are randomly chosen from somewhere
        // between these limits.
        public float MinEndSize = 100;
        public float MaxEndSize = 100;


        // Alpha blending settings.
        public BlendState BlendState = BlendState.NonPremultiplied;

        public void Load(String filename)
        {
            string f = FilePaths.DataPath + filename;

            if (filename.StartsWith(FilePaths.DataPath))
                f = filename;
           
            XmlDocument xdoc = new XmlDocument();
            xdoc.Load(f);
            XmlNodeList list = xdoc.SelectNodes("//Setting");

            foreach (XmlNode node in list)
            {
                String s = (String)node.Attributes["ID"].InnerText;
                switch (s)
                {
                    case "TextureName":
                        {
                            TextureName = node.Attributes["Text"].InnerText;
                            int n = TextureName.LastIndexOf('\\');
                            if (n > 0)
                            {
                                ShortTextureName = TextureName.Substring(n + 1);
                                ShortTextureName = Path.GetFileNameWithoutExtension(ShortTextureName);
                            }
                            else
                            {
                                ShortTextureName = TextureName;
                            }
                        }
                        break;

                    case "TextureCells":
                        {
                            TextureCells = int.Parse(node.Attributes["Text"].InnerText);
                        }
                        break;

                    case "MaxParticles":
                        {
                            MaxParticles = int.Parse(node.Attributes["Text"].InnerText);
                        }
                        break;

                    case "Duration":
                        {
                            Duration = TimeSpan.FromSeconds(float.Parse(node.Attributes["Text"].InnerText));
                        }
                        break;

                    case "DurationRandomness":
                        {
                            DurationRandomness = float.Parse(node.Attributes["Text"].InnerText);
                        }
                        break;

                    case "MinRotateSpeed":
                        {
                            MinRotateSpeed = float.Parse(node.Attributes["Text"].InnerText);
                        }
                        break;

                    case "MaxRotateSpeed":
                        {
                            MaxRotateSpeed = float.Parse(node.Attributes["Text"].InnerText);
                        }
                        break;

                    case "EndVelocity":
                        {
                            EndVelocity = float.Parse(node.Attributes["Text"].InnerText);
                        }
                        break;

                    case "MinHorizontalVelocity":
                        {
                            MinHorizontalVelocity = float.Parse(node.Attributes["Text"].InnerText);
                        }
                        break;

                    case "MaxHorizontalVelocity":
                        {
                            MaxHorizontalVelocity = float.Parse(node.Attributes["Text"].InnerText);
                        }
                        break;
                    case "MinVerticalVelocity":
                        {
                            MinVerticalVelocity = float.Parse(node.Attributes["Text"].InnerText);
                        }
                        break;
                    case "MaxVerticalVelocity":
                        {
                            MaxVerticalVelocity = float.Parse(node.Attributes["Text"].InnerText);
                        }
                        break;
                    case "GravitySensitivity":
                        {
                            GravitySensitivity= float.Parse(node.Attributes["Text"].InnerText);
                            Gravity = GravitySensitivity * new Vector3(0, -9.81f, 0);
                        }
                        break;
                    case "MinStartSize":
                        {
                            MinStartSize = float.Parse(node.Attributes["Text"].InnerText);
                        }
                        break;
                    case "MaxStartSize":
                        {
                            MaxStartSize = float.Parse(node.Attributes["Text"].InnerText);
                        }
                        break;
                    case "MinColor":
                        {
                            MinColor.R = byte.Parse(node.Attributes["R"].InnerText);
                            MinColor.G = byte.Parse(node.Attributes["G"].InnerText);
                            MinColor.B = byte.Parse(node.Attributes["B"].InnerText);
                            MinColor.A = byte.Parse(node.Attributes["A"].InnerText);
                        }
                        break;

                    case "MaxColor":
                        {
                            MaxColor.R = byte.Parse(node.Attributes["R"].InnerText);
                            MaxColor.G = byte.Parse(node.Attributes["G"].InnerText);
                            MaxColor.B = byte.Parse(node.Attributes["B"].InnerText);
                            MaxColor.A = byte.Parse(node.Attributes["A"].InnerText);
                        }
                        break;
                    case "MinEndSize":
                        {
                            MinEndSize = float.Parse(node.Attributes["Text"].InnerText);
                        }
                        break;
                    case "MaxEndSize":
                        {
                            MaxEndSize = float.Parse(node.Attributes["Text"].InnerText);
                        }
                        break;
                    case "BlendState":
                        {
                            switch(node.Attributes["Text"].InnerText)
                            {
                                case "Additive":
                                    BlendState = BlendState.Additive;
                                    break;
                                case "Alpha":
                                    BlendState = BlendState.NonPremultiplied;
                                    break;
                            }
                        }
                        break;
                }
            }
        }
    }
}
