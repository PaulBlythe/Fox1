using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using GuruEngine.SceneManagement;
using GuruEngine.DebugHelpers;
using GuruEngine.Rendering;
using GuruEngine.Assets;
using GuruEngine;

namespace GuruEngineTest.Scenes.Developer.Subs
{
    public class LoadAircraft:SubScene
    {
        Rectangle button = new Rectangle(10, 10, 320, 40);
        SpriteBatch batch;
        MouseState old;
        DebugFileBrowser fb = null;

       

        public LoadAircraft()
        {
            batch = new SpriteBatch(Renderer.GetGraphicsDevice());

        }

        public override void Update(float dt)
        {
            MouseState ms = Mouse.GetState();
            if (button.Contains(ms.X,ms.Y))
            {
                if ((ms.LeftButton == ButtonState.Released)&&(old.LeftButton == ButtonState.Pressed))
                {
                    if (fb==null)
                    {
                        fb = new DebugFileBrowser(FilePaths.DataPath + "Aircraft", "*.gameobject");
                    }
                }
            }
            old = ms;
        }

        public override void Draw()
        {
            if (fb != null)
            {
                batch.Begin();
                fb.Draw(batch);
                batch.End();
                switch (fb.ResultCode)
                {
                    case 1:
                        fb = null;
                        break;
                    case 2:
                        if (fb.SelectedEntry != null)
                        {
                            result = fb.SelectedEntry.Path;
                            return_code = 1;
                        }
                        break;
                }
            }
            else
            {
                batch.Begin();
                if (button.Contains(old.X, old.Y))
                    batch.FillRectangle(button, Color.DarkBlue);
                else
                    batch.FillRectangle(button, Color.DarkGray);

                batch.DrawString(AssetManager.GetDebugFont(), "Load Aircraft", new Vector2(20, 18), Color.White);

                batch.DrawRectangle(button, Color.White);
                batch.End();
            }
        }
    }
}
