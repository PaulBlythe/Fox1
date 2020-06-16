using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using GuruEngine.Rendering;
using GuruEngine.Assets;

namespace GuruEngine.World.PaintSchemes
{
    public class BritishPaintScheme : PaintScheme
    {
        public Guid InstanceID { get; private set; }

        /// <summary>
        /// A paint scheme for a British aircraft
        /// </summary>
        /// <param name="squadron">Squadron code eg WX</param>
        /// <param name="aircraft">Aircraft code eg A</param>
        /// <param name="year">Year of aircraft paintscheme</param>
        /// <param name="doTail">Add roundel to tail</param>
        public BritishPaintScheme(String squadron, String aircraft, int year, bool doTail)
        {
            DoBase(squadron, aircraft, year, doTail);
        }


        /// <summary>
        /// A paint scheme for a British aircraft with nose art
        /// </summary>
        /// <param name="squadron">Squadron code eg WX</param>
        /// <param name="aircraft">Aircraft code eg A</param>
        /// <param name="year">Year of aircraft paintscheme</param>
        /// <param name="doTail">Add roundel to tail</param>
        /// <param name="logo">Nose art</param>
        public BritishPaintScheme(String squadron, String aircraft, int year, bool doTail, String logo)
        {
            DoBase(squadron, aircraft, year, doTail);
            String noseart = Path.Combine(Settings.PaintSchemeDirectory, logo);
            TextureOverrides.Add("Overlay5", noseart);
        }

        private void DoBase(String squadron, String aircraft, int year, bool doTail)
        {
            InstanceID = Guid.NewGuid();

            String roundel;
            if (year < 1930)
            {
                roundel = @"decals\British\RAF_Type_A_original";
            }
            else if (year < 1937)
            {
                roundel = @"decals\British\RAF_Type_A";
            }
            else if (year < 1939)
            {
                roundel = @"decals\British\roundel4c";
            }
            else if (year < 1943)
            {
                roundel = @"decals\British\roundel4cthin";
            }
            else
            {
                roundel = @"decals\British\roundel3c";
            }

            roundel = Path.Combine(Settings.PaintSchemeDirectory, roundel);

            if (doTail)
            {
                TextureOverrides.Add("Overlay8", roundel);
            }
            else
            {
                TextureOverrides.Add("Overlay8",  FilePaths.HIMTexturePath + @"\null");
            }
            TextureOverrides.Add("Overlay7", roundel);
            TextureOverrides.Add("Overlay6", roundel);

            SpriteBatch batch = new SpriteBatch(Renderer.GetGraphicsDevice());
         
            /// Do the squadron code
            {
                Texture2D tenstex = null;
                Texture2D unittex = null;


                String srcpath = Path.Combine(Settings.PaintSchemeDirectory, @"decals\British");
                int ss = 0;
                if (squadron.Length>1)
                {
                    string tpath = Path.Combine(srcpath, squadron.Substring(0,1));
                    tpath += ".png";
                    Stream read = new FileStream(tpath, FileMode.Open);
                    tenstex = Texture2D.FromStream(Renderer.GetGraphicsDevice(), read);
                    read.Close();
                    ss++;
                }
                {
                    string tpath = Path.Combine(srcpath, squadron.Substring(ss,1));
                    tpath += ".png";
                    Stream read = new FileStream(tpath, FileMode.Open);
                    unittex = Texture2D.FromStream(Renderer.GetGraphicsDevice(), read);
                    read.Close();

                }

                RenderTarget2D numbers = new RenderTarget2D(Renderer.GetGraphicsDevice(), 128, 128, false, SurfaceFormat.Color, DepthFormat.None);
                Renderer.GetGraphicsDevice().SetRenderTarget(numbers);
                Renderer.GetGraphicsDevice().Clear(Color.Transparent);
                batch.Begin();
                if (tenstex != null)
                {
                    batch.Draw(tenstex, Vector2.Zero, Color.White);
                }
                batch.Draw(unittex, new Vector2(61, 0), Color.White);
                
                batch.End();
                Renderer.GetGraphicsDevice().SetRenderTarget(null);
                String path = srcpath + squadron + InstanceID.ToString();
                TextureOverrides.Add("Overlay1", path);
                TextureOverrides.Add("Overlay4", path);
                path += ".png";
                AssetManager.AddTextureToManager(path.GetHashCode(), numbers);

                tenstex.Dispose();
                unittex.Dispose();
            }

            /// Do the aircraft code
            {
                Texture2D tenstex = null;
                Texture2D unittex = null;


                String srcpath = Path.Combine(Settings.PaintSchemeDirectory, @"decals\British");
                int ss = 0;
                if (aircraft.Length > 1)
                {
                    string tpath = Path.Combine(srcpath, aircraft.Substring(0, 1));
                    tpath += ".png";
                    Stream read = new FileStream(tpath, FileMode.Open);
                    tenstex = Texture2D.FromStream(Renderer.GetGraphicsDevice(), read);
                    read.Close();
                    ss++;
                }
                {
                    string tpath = Path.Combine(srcpath, aircraft.Substring(ss, 1));
                    tpath += ".png";
                    Stream read = new FileStream(tpath, FileMode.Open);
                    unittex = Texture2D.FromStream(Renderer.GetGraphicsDevice(), read);
                    read.Close();

                }

                RenderTarget2D numbers = new RenderTarget2D(Renderer.GetGraphicsDevice(), 128, 128, false, SurfaceFormat.Color, DepthFormat.None);
                Renderer.GetGraphicsDevice().SetRenderTarget(numbers);
                Renderer.GetGraphicsDevice().Clear(Color.Transparent);
                batch.Begin();
                if (tenstex != null)
                {
                    batch.Draw(tenstex, Vector2.Zero, Color.White);
                }
                batch.Draw(unittex, new Vector2(61, 0), Color.White);

                batch.End();
                Renderer.GetGraphicsDevice().SetRenderTarget(null);
                String path = srcpath + aircraft + InstanceID.ToString();
                TextureOverrides.Add("Overlay2", path);
                TextureOverrides.Add("Overlay3", path);
                path += ".png";
                AssetManager.AddTextureToManager(path.GetHashCode(), numbers);

                tenstex.Dispose();
                unittex.Dispose();
            }
        }

        

    }
}
