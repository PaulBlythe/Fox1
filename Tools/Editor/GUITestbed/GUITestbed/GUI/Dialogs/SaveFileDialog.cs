using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using GUITestbed.GUI.Widgets;

namespace GUITestbed.GUI.Dialogs
{
    public delegate bool CallBack(String file);

    public class SaveFileDialog:Dialog
    {
        Rectangle FileRegion;
        String SearchPath;
        String extension;

        ListView ip;
        TextBox textBox1;
        CallBack resultHandler;

        public SaveFileDialog(String path, String title, String ext, Rectangle r, CallBack caller)
            : base(title, r)
        {
            SearchPath = path;
            resultHandler = caller;
            extension = ext;

            DriveInfo[] allDrives = DriveInfo.GetDrives();
            Rectangle r2 = new Rectangle(r.X + 10, r.Y + 40, 40, 30);
            foreach (DriveInfo d in allDrives)
            {
                Button b = new Button(r2, d.Name);
                b.Parent = this;
                Children.Add(b);
                r2.Y += 40;
            }
            FileRegion = new Rectangle(r.X + 55, r.Y + 40, r.Width - 70, r.Height - 125);
            ip = new ListView(FileRegion);
            ip.Parent = this;
            Children.Add(ip);

            Button b2 = new Button(new Rectangle(r.X + 10, r.Y + r.Height - 40, 120, 30), "Cancel");
            b2.Parent = this;
            Children.Add(b2);

            b2 = new Button(new Rectangle(r.X + r.Width - 130, r.Y + r.Height - 40, 120, 30), "Save");
            b2.Parent = this;
            Children.Add(b2);

            textBox1 = new TextBox(new Rectangle(r.X + 55, FileRegion.Y + FileRegion.Height + 5, FileRegion.Width, 30), "");
            textBox1.Parent = this;
            Children.Add(textBox1);
            Search();
        }

        void Search()
        {
            ip.Clear();
            ip.AddItem("[  ..  ]");

            string[] directories = Directory.GetDirectories(SearchPath);
            for (int i = 0; i < directories.Length; i++)
            {
                String hh = Path.GetFileName(directories[i]);
                ip.AddItem("[  " + hh + "  ]");
            }

            DirectoryInfo dd = new DirectoryInfo(SearchPath);
            FileInfo[] Files = dd.GetFiles("*" + extension);
            foreach (FileInfo fi in Files)
            {
                ip.AddItem(fi.Name);
            }
        }

        public override void Update(float dt)
        {
            foreach (Widget w in Children)
                w.Update(dt);
        }

        public override void Draw(GUIBatch b)
        {
            base.Draw(b);
        }

        public override void Draw(GuiFont b)
        {
            base.Draw(b);
        }

        /// <summary>
        /// Message handler
        /// </summary>
        /// <param name="s"></param>
        public override void Message(string s)
        {
            if (s.Contains("["))
            {
                char[] splits = new char[] {' ','\t','[',']' };
                string[] parts = s.Split(splits, StringSplitOptions.RemoveEmptyEntries);
                SearchPath = Path.Combine(SearchPath, parts[1]);
                Search();
            }
            else if (s == "Button:Cancel")
            {
                GuiManager.Instance.RemoveTopLevelDialog();
            }
            else if (s == "Button:Save")
            {
                resultHandler(Path.Combine(SearchPath,textBox1.Text));
                GuiManager.Instance.RemoveTopLevelDialog();
            }else if (s.StartsWith("SmallButton:"))
            {
                String fn = s.Substring(12);
               
                textBox1.Text = fn;
            }
            else
            {
                base.Message(s);
            }

        }
    }
}
