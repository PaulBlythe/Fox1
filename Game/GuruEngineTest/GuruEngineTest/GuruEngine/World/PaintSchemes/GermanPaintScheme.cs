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
    class GermanPaintScheme : PaintScheme
    {
        public Guid InstanceID { get; private set; }

        /// <summary>
        /// A paint scheme for a German aircraft
        /// </summary>
        /// <param name="squadron">Squadron code eg WX</param>
        /// <param name="aircraft">Aircraft code eg A</param>
        /// <param name="year">Year of aircraft paintscheme</param>
        /// <param name="doTail">Add roundel to tail</param>
        public GermanPaintScheme(String squadron, String aircraft, int year, bool doTail)
        {
            DoBase(squadron, aircraft, year, doTail);
        }


        /// <summary>
        /// A paint scheme for a German aircraft with nose art
        /// </summary>
        /// <param name="squadron">Squadron code eg WX</param>
        /// <param name="aircraft">Aircraft code eg A</param>
        /// <param name="year">Year of aircraft paintscheme</param>
        /// <param name="doTail">Add roundel to tail</param>
        /// <param name="logo">Nose art</param>
        public GermanPaintScheme(String squadron, String aircraft, int year, bool doTail, String logo)
        {
            DoBase(squadron, aircraft, year, doTail);
            String noseart = Path.Combine(Settings.PaintSchemeDirectory, logo);
            TextureOverrides.Add("Overlay5", noseart);
        }

        private void DoBase(String squadron, String aircraft, int year, bool doTail)
        {
            String roundel = @"decals\German\balken1";
            InstanceID = Guid.NewGuid();
            roundel = Path.Combine(Settings.PaintSchemeDirectory, roundel);

            if (doTail)
            {
                TextureOverrides.Add("Overlay8", roundel);
            }
            else
            {
                TextureOverrides.Add("Overlay8", @"E:\Research\XNA\MultiThreadedRenderer\MultiThreadedRenderer\Content\HIM\CommonTextures\null");
            }
        

            TextureOverrides.Add("Overlay7", roundel);
            TextureOverrides.Add("Overlay6", roundel);

            SpriteBatch batch = new SpriteBatch(Renderer.GetGraphicsDevice());

            /// Do the squadron code
            {
                Texture2D tenstex = null;
                Texture2D unittex = null;


                String srcpath = Path.Combine(Settings.PaintSchemeDirectory, @"decals\German");
                int ss = 0;
                if (squadron.Length > 1)
                {
                    string tpath = Path.Combine(srcpath, Convert(squadron, 0, 1));
                    tpath += ".png";
                    Stream read = new FileStream(tpath, FileMode.Open);
                    tenstex = Texture2D.FromStream(Renderer.GetGraphicsDevice(), read);
                    read.Close();
                    ss++;
                }
                {
                    string tpath = Path.Combine(srcpath, Convert(squadron, ss, 1));
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
                batch.Draw(unittex, new Vector2(62, 0), Color.White);

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

                String srcpath = Path.Combine(Settings.PaintSchemeDirectory, @"decals\German");
                int ss = 0;
                if (aircraft.Length > 1)
                {
                    string tpath = Path.Combine(srcpath, Convert(aircraft,0,1));
                    tpath += ".png";
                    Stream read = new FileStream(tpath, FileMode.Open);
                    tenstex = Texture2D.FromStream(Renderer.GetGraphicsDevice(), read);
                    read.Close();
                    ss++;
                }
                {
                    string tpath = Path.Combine(srcpath, Convert(aircraft, ss, 1));
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
                batch.Draw(unittex, new Vector2(62, 0), Color.White);

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

        String Convert(String i, int s, int e)
        {
            String res = i.Substring(s, e);
            switch (res)
            {
                case "0":
                case "1":
                case "2":
                case "3":
                case "4":
                case "5":
                case "6":
                case "7":
                case "8":
                case "9":
                    res = "11" + res;
                    break;
            }
            return res;
        }

    }
}
