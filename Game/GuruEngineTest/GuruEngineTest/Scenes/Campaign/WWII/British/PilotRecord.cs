using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;

using GuruEngine.Text;
using GuruEngine.SceneManagement;
using GuruEngine.Rendering;
using GuruEngine.Player.Records.Display.Pages.British.Form414;
using GuruEngine.Player.Records.Display.Pages;

namespace GuruEngineTest.Scenes.Campaign.WWII.British
{
    public class PilotRecord : Scene
    {
        Page currentPage;
       

        public override void Draw(GameTime gt)
        {
            Renderer.GetCurrentRenderer().Draw(null, gt);
        }

        public override void Init()
        {
            //currentPage = new FrontPage();
            currentPage = new Page1();
            //currentPage = new Page2();
            currentPage.SetupRenderCommands();
        }

        public override void Load(ContentManager Content)
        {

        }

        public override void Update(float dt)
        {
            currentPage.Update();
        }
    }
}
