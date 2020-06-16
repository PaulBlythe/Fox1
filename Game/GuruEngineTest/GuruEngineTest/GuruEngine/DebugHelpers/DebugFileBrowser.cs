using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;

using GuruEngine.Rendering;
using GuruEngine.Assets;

namespace GuruEngine.DebugHelpers
{
    public class Entry
    {
        public String ID;
        public String Path;
        public Rectangle Rect;
        public bool IsDir = false;
    }

    public class DebugFileBrowser
    {
        public int ResultCode = 0;

        String CurrentDirectory;
        String Extension;
        public Entry SelectedEntry = null;

        Rectangle CancelButton = new Rectangle(470, 665, 100, 30);
        Rectangle OKButton = new Rectangle(1350, 665, 100, 30);
        MouseState oldmousestate;

        List<Entry> entries = new List<Entry>();

        public DebugFileBrowser(String startdir, String extension)
        {
            CurrentDirectory = startdir;
            Extension = extension;

            int y = 133;
            foreach (var drive in DriveInfo.GetDrives())
            {
                Entry e = new Entry();
                e.ID = drive.Name;
                e.Rect = new Rectangle(463, y, 35, 30);
                entries.Add(e);
            }
            Scan();
        }

        private String GetDir(String s)
        {
            int pos = s.LastIndexOf('\\');
            return s.Substring(pos+1);
        }

        private void Scan()
        {
            entries.Clear();

            string[] files = Directory.GetFiles(CurrentDirectory, Extension);
            string[] dirs = Directory.GetDirectories(CurrentDirectory);

            int x = 505;
            int y = 135;

            Entry ee = new Entry();
            ee.Path = "..";
            ee.ID = "..";
            ee.Rect = new Rectangle(x, y, 200, 28);
            ee.IsDir = true;
            entries.Add(ee);

            x += 210;

            for (int i=0; i<dirs.Length; i++)
            {
                Entry e = new Entry();
                e.Path = dirs[i];
                e.ID = GetDir(dirs[i]);
                e.Rect = new Rectangle(x, y, 200, 28);
                e.IsDir = true;
                entries.Add(e);

                x += 210;
                if ((x + 200) >= 1460)
                {
                    x = 505;
                    y += 30;
                }
            }

            for (int i=0; i<files.Length; i++)
            {
                Entry e = new Entry();
                e.Path = files[i];
                e.ID = Path.GetFileNameWithoutExtension(files[i]);
                e.Rect = new Rectangle(x, y, 200, 28);
                entries.Add(e);

                x += 210;
                if ((x+200) >= 1460)
                {
                    x = 505;
                    y += 30;
                }
            }
        }

        public void Draw(SpriteBatch batch)
        {
            bool rescan = false;

            MouseState ms = Mouse.GetState();
            bool click = ((ms.LeftButton == ButtonState.Released) && (oldmousestate.LeftButton == ButtonState.Pressed));

            batch.FillRectangle(new Rectangle(460, 100, 1000, 600), Color.DarkGray);
            batch.DrawRectangle(new Rectangle(460, 100, 1000, 600), Color.White);

            batch.FillRectangle(new Rectangle(461, 101, 998, 28), Color.DarkSlateGray);
            batch.DrawRectangle(new Rectangle(460, 100, 1000, 30), Color.White);

            batch.FillRectangle(new Rectangle(462, 132, 40, 528), Color.DarkSlateGray);
            batch.DrawRectangle(new Rectangle(461, 130, 40, 530), Color.White);

            batch.FillRectangle(new Rectangle(502, 132, 958, 528), Color.DarkSlateGray);
            batch.DrawRectangle(new Rectangle(501, 130, 959, 530), Color.White);

            batch.FillRectangle(new Rectangle(1002, 665, 280, 30), Color.DarkSlateGray);
            batch.DrawRectangle(new Rectangle(1001, 665, 280, 30), Color.White);

            batch.DrawString(AssetManager.GetDebugFont(), CurrentDirectory, new Vector2(463, 101), Color.White);

            if (CancelButton.Contains(ms.X,ms.Y))
            {
                batch.FillRectangle(CancelButton, Color.DarkGray);
                if (click)
                    ResultCode = 1;
            }
            else
            {
                batch.FillRectangle(CancelButton, Color.DarkSlateGray);
            }
            batch.DrawRectangle(CancelButton, Color.White);
            batch.DrawString(AssetManager.GetDebugFont(), "Cancel", new Vector2(480, 666), Color.White);

            if (OKButton.Contains(ms.X, ms.Y))
            {
                batch.FillRectangle(OKButton, Color.DarkGray);
                if (click)
                    ResultCode = 2;
            }
            else
            {
                batch.FillRectangle(OKButton, Color.DarkSlateGray);
            }
            batch.DrawRectangle(OKButton, Color.White);
            batch.DrawString(AssetManager.GetDebugFont(), "OK", new Vector2(1380, 666), Color.White);

            foreach(Entry e in entries)
            {
                if (e.IsDir)
                {
                    if (e.Rect.Contains(ms.X, ms.Y))
                    {
                        batch.FillRectangle(e.Rect, Color.DarkKhaki);
                        if (click)
                        {
                            if (e.ID == "..")
                            {
                                int ll = CurrentDirectory.LastIndexOf('\\');
                                CurrentDirectory = CurrentDirectory.Substring(0, ll);
                            }
                            else
                            {
                                CurrentDirectory = e.Path;
                            }
                            rescan = true;
                        }
                    }
                    else
                    {
                        batch.FillRectangle(e.Rect, Color.DarkOrange);
                    }
                }
                else
                {
                    if (e.Rect.Contains(ms.X, ms.Y))
                    {
                        batch.FillRectangle(e.Rect, Color.DarkGray);
                        if (click)
                            SelectedEntry = e;
                    }
                    else
                    {
                        batch.FillRectangle(e.Rect, Color.DarkSlateGray);
                    }
                }
                
                //batch.DrawRectangle(e.Rect, Color.White);
                batch.DrawString(AssetManager.GetDebugFont(), e.ID, new Vector2(e.Rect.X + 2, e.Rect.Y + 2), Color.White);
            }
            if (SelectedEntry != null)
            {
                batch.DrawString(AssetManager.GetDebugFont(), SelectedEntry.ID, new Vector2(1005, 666), Color.White);
            }

            if (rescan)
            {
                Scan();
            }
            oldmousestate = ms;
        }
    }
}
