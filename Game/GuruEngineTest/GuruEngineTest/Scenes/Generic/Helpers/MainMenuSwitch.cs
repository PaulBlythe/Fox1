using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using GuruEngine.Rendering;
using GuruEngine.Audio;
using GuruEngine.Rendering.Deferred;


namespace GuruEngineTest.Scenes.Generic.Helpers
{
    public class MainMenuSwitch:MainMenuWidget
    {
        String Choice1;
        String Choice2;
        Rectangle r1;
        Rectangle r2;
        Vector2 p1;
        Vector2 p2;

        bool First = true;

        public MainMenuSwitch(String l1, String l2, int id, Vector2 pos)
        {
            Choice1 = l1;
            Choice2 = l2;
            Position = pos;
            SetRegion();
            Text = "";
            OwnerDrawn = true;

            switch(id)
            {
                case -1:
                    {
                        First = !Renderer.GetSettings().Forward;
                    }
                    break;
            }
        }

        public override void Check(int x, int y, bool pressed)
        {
            if ((pressed) && (Region.Contains(x, y)))
            {
                r1 = Region;
                r1.Width = 200;
                if (r1.Contains(x,y))
                {
                    First = true;
                }
                else
                {
                    First = false;
                }
                switch (ID)
                {
                    case -1:
                        Renderer.GetSettings().Forward = !First;
                        break;
                }
                
            }
        }

        public override void Draw(SpriteBatch batch, int x)
        {
            r1 = Region;
            r1.X += x;
            batch.FillRectangle(r1, Color.DarkSlateGray);
            batch.DrawRectangle(r1, Color.White);

            if (First)
            {
                r1.X += 1;
                r1.Width = 198;
                r1.Y += 1;
                r1.Height -= 2;

                batch.DrawString(MainMenu.Instance.font, Choice2, new Vector2(p2.X + x, p2.Y), Color.Gray);
                batch.FillRectangle(r1, Color.SlateBlue);
                batch.DrawRectangle(r1, Color.White);

                batch.DrawString(MainMenu.Instance.font, Choice1, new Vector2(p1.X + x, p1.Y), Color.White);
            }
            else
            {
                r1.X += 201;
                r1.Width = 198;
                r1.Y += 1;
                r1.Height -= 2;

                batch.DrawString(MainMenu.Instance.font, Choice1, new Vector2(p1.X + x, p1.Y), Color.Gray);
                batch.FillRectangle(r1, Color.SlateBlue);
                batch.DrawRectangle(r1, Color.White);

                batch.DrawString(MainMenu.Instance.font, Choice2, new Vector2(p2.X + x, p2.Y), Color.White);
            }
        }

        public override void SetRegion()
        {
            Region = new Rectangle();
            Region.X = (int)Position.X - 3;
            Region.Y = (int)Position.Y - 3;
            Region.Height = 36;
            Region.Width = 400;

            r2 = new Rectangle(Region.X + 3, Region.Y + 3, 300, 36);
            p1 = new Vector2(Region.X + 3, Region.Y + 6);
            p2 = new Vector2(Region.X + 203, Region.Y + 6);
        }
    }
}
