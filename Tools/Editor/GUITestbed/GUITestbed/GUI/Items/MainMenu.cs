using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using GUITestbed.GUI.Widgets;
using GUITestbed.Tools;
using GUITestbed.SerialisableData;
using GUITestbed.GUI.Dialogs;
using GUITestbed.DataHandlers.Fox1.Objects;
using GUITestbed.DataHandlers.FG;
using GUITestbed.DataHandlers.IL2;
using GUITestbed.Scenes;

namespace GUITestbed.GUI.Items
{
    public class MainMenu : GuiItem
    {
        public static MainMenu Instance;
        List<Widget> ActiveWidgets = new List<Widget>();

        MapGeneratorData mapgen = null;
        SaveFileDialog saver = null;
        OpenFileDialog loader = null;
        TextEntryDialog textentry = null;
        StatusBar MaterialStatusBar = null;

        GUITestbed.DataHandlers.Fox1.Objects.Airport currentAirport;
        AirportDatabase currentDatabase = null;
        Button save_material;
        String basedirectory = @"c:\Data\Fox1";

        public MainMenu()
        {
            Instance = this;
            SetupWorldView();
        }

        public override void HandleEvent(string s)
        {
            switch (s)
            {
                #region Toolbar
                case "Toolbar:X":
                    Game1.Instance.Exit();
                    break;

                case "Toolbar:Airports":
                    SetupAirportView();
                    break;

                case "Toolbar:World":
                    SetupWorldView();
                    break;

                case "Toolbar:Scene":
                    SetupSceneView();
                    break;

                case "Toolbar:Object":
                    SetupObjectView();
                    break;

                case "Toolbar:Materials":
                    SetupMaterialView();
                    break;
                #endregion

                case "Button:Save material":
                    {
                        MaterialDisplayTool.SaveLibrary();
                    }
                    break;
                case "Button:Edit material":
                    {
                        loader = new OpenFileDialog(basedirectory, "Load Material", ".materials", new Rectangle((1920 - 800) / 2, 100, 800, 700), LoadMaterial);
                        GuiManager.Instance.Add(loader);
                    }
                    break;

                case "Button:Map editor":
                    SetupMapView();
                    break;

                case "Button:Object converter":
                    SetupObjectConverterView();
                    break;

                case "Button:Cancel":
                case "Button:X":
                    {
                        GuiManager.Instance.RemoveTopLevelDialog();
                    }
                    break;

                case "Button:Go to object mode":
                    SetupObjectView();
                    break;

                case "Button:Load scene":
                    loader = new OpenFileDialog(basedirectory, "Load Scene", ".scene", new Rectangle((1920 - 800) / 2, 100, 800, 700), LoadScene);
                    GuiManager.Instance.Add(loader);
                    break;

                #region Scene lighting
                case "Button:Light Volume":
                    {
                        SceneViewTool.CreateLightVolume();
                    }
                    break;
                #endregion

                #region New map
                case "Map generation:Button:Build":
                    {
                        NumericUpDown StartLatitude = (NumericUpDown)ActiveWidgets[0];
                        NumericUpDown StartLongitude = (NumericUpDown)ActiveWidgets[1];
                        NumericUpDown Size = (NumericUpDown)ActiveWidgets[2];
                        NumericUpDown Date = (NumericUpDown)ActiveWidgets[5];
                        Selector Width = (Selector)ActiveWidgets[3];
                        Selector Height = (Selector)ActiveWidgets[4];
                        CheckBox Airfields = (CheckBox)ActiveWidgets[6];
                        CheckBox Roads = (CheckBox)ActiveWidgets[7];
                        CheckBox Rivers = (CheckBox)ActiveWidgets[8];
                        CheckBox Cities = (CheckBox)ActiveWidgets[9];
                        CheckBox Countries = (CheckBox)ActiveWidgets[10];

                        mapgen = new MapGeneratorData();
                        mapgen.Airfields = Airfields.Selected;
                        mapgen.Cities = Cities.Selected;
                        mapgen.Countries = Countries.Selected;
                        mapgen.Date = Date.Value;
                        mapgen.Height = int.Parse(Height.SelectedText());
                        mapgen.Rivers = Rivers.Selected;
                        mapgen.Roads = Roads.Selected;
                        mapgen.Size = Size.Value;
                        mapgen.StartLatitude = StartLatitude.Value;
                        mapgen.StartLongitude = StartLongitude.Value;
                        mapgen.Width = int.Parse(Width.SelectedText());

                        Game1.Instance.current = new MapGenerator(mapgen);


                    }
                    break;

                case "Button:New map":
                    {
                        ActiveWidgets.Clear();

                        Rectangle r2 = new Rectangle(1920 - 320, 30, 320, 940);
                        SideBar sb = new SideBar("Map generation", r2);

                        int x1 = r2.X + 20;
                        int x2 = x1 + 40;

                        Label l1 = new Label(new Vector2(x1, 100), "Start latitude");
                        sb.Children.Add(l1);

                        NumericUpDown n = new NumericUpDown(new Vector2(x1 + 20, 110), -90, 90, 0);
                        sb.Children.Add(n);
                        ActiveWidgets.Add(n);

                        l1 = new Label(new Vector2(x1, 160), "Start longitude");
                        sb.Children.Add(l1);

                        n = new NumericUpDown(new Vector2(x1 + 20, 170), -180, 180, 0);
                        sb.Children.Add(n);
                        ActiveWidgets.Add(n);

                        l1 = new Label(new Vector2(x1, 220), "Pixel size (in metres)");
                        sb.Children.Add(l1);

                        n = new NumericUpDown(new Vector2(x1 + 20, 230), 100, 2000, 400);
                        sb.Children.Add(n);
                        ActiveWidgets.Add(n);

                        l1 = new Label(new Vector2(x1, 280), "Width (in pixels)");
                        sb.Children.Add(l1);

                        Selector se = new Selector(new Vector2(x1 + 20, 290), new String[] { "512", "1024", "2048", "4096", "8192" }, 1);
                        sb.Children.Add(se);
                        ActiveWidgets.Add(se);

                        l1 = new Label(new Vector2(x1, 340), "Height (in pixels)");
                        sb.Children.Add(l1);

                        se = new Selector(new Vector2(x1 + 20, 350), new String[] { "512", "1024", "2048", "4096", "8192" }, 1);
                        sb.Children.Add(se);
                        ActiveWidgets.Add(se);

                        l1 = new Label(new Vector2(x1, 400), "Date");
                        sb.Children.Add(l1);

                        n = new NumericUpDown(new Vector2(x1 + 20, 410), 1900, 2100, 1900);
                        sb.Children.Add(n);
                        ActiveWidgets.Add(n);

                        l1 = new Label(new Vector2(x2, 480), "Show airfields");
                        sb.Children.Add(l1);
                        l1 = new Label(new Vector2(x2, 510), "Show roads");
                        sb.Children.Add(l1);
                        l1 = new Label(new Vector2(x2, 540), "Show rivers");
                        sb.Children.Add(l1);
                        l1 = new Label(new Vector2(x2, 570), "Show major cities");
                        sb.Children.Add(l1);
                        l1 = new Label(new Vector2(x2, 600), "Show countries");
                        sb.Children.Add(l1);

                        CheckBox c = new CheckBox(new Vector2(x1, 458), true);
                        sb.Children.Add(c);
                        ActiveWidgets.Add(c);

                        c = new CheckBox(new Vector2(x1, 488), true);
                        sb.Children.Add(c);
                        ActiveWidgets.Add(c);

                        c = new CheckBox(new Vector2(x1, 518), true);
                        sb.Children.Add(c);
                        ActiveWidgets.Add(c);

                        c = new CheckBox(new Vector2(x1, 548), true);
                        sb.Children.Add(c);
                        ActiveWidgets.Add(c);

                        c = new CheckBox(new Vector2(x1, 578), true);
                        sb.Children.Add(c);
                        ActiveWidgets.Add(c);

                        Button b = new Button(new Rectangle(x2, 900, 200, 30), "Build");
                        b.Parent = sb;
                        sb.Children.Add(b);
                        sb.Host = this;

                        GuiManager.Instance.Add(sb);

                    }
                    break;
                default:
                    {
                        System.Console.WriteLine("Missing event handler " + s);
                    }
                    break;
                #endregion

                #region Load map
                case "Button:Load map":
                    {
                        loader = new OpenFileDialog(@"c:\Data\Fox1", "Load Map", ".map", new Rectangle((1920 - 800) / 2, 100, 800, 700), LoadMap);
                        GuiManager.Instance.Add(loader);
                    }
                    break;
                #endregion

                #region Load wavefront object
                case "Button:Load Wavefront":
                    {
                        loader = new OpenFileDialog(@"c:\Data\Fox1", "Load wavefront", ".obj", new Rectangle((1920 - 800) / 2, 100, 800, 700), LoadWavefront);
                        GuiManager.Instance.Add(loader);
                    }
                    break;
                #endregion

                #region Airport button handlers
                case "Button:Load object pack":
                    {
                        loader = new OpenFileDialog(@"\\NFS1\FileStore\Graphics\3D\Airports", "Load object pack", ".txt", new Rectangle((1920 - 800) / 2, 100, 800, 700), LoadObjectPack);
                        GuiManager.Instance.Add(loader);
                    }
                    break;
                case "Button:Load airport":
                    {
                        loader = new OpenFileDialog(@"\\NFS1\FileStore\Graphics\3D\Airports", "Load airport", ".xml", new Rectangle((1920 - 800) / 2, 100, 800, 700), LoadAirport);
                        GuiManager.Instance.Add(loader);
                    }
                    break;
                case "Button:Load object map":
                    {
                        loader = new OpenFileDialog(@"\\NFS1\FileStore\Graphics\3D\Airports", "Load object map", ".xml", new Rectangle((1920 - 800) / 2, 100, 800, 700), LoadAirportObjects);
                        GuiManager.Instance.Add(loader);
                    }
                    break;

                case "Button:Scan APT.DAT":
                    {
                        loader = new OpenFileDialog(Settings.APTDirectory, "Load apt.dat", ".dat", new Rectangle((1920 - 800) / 2, 100, 800, 700), ScanAPT);
                        GuiManager.Instance.Add(loader);
                    }
                    break;

                case "Button:Find ICAO":
                    {
                        textentry = new TextEntryDialog("ICAO", SearchAPTForICAO);
                        GuiManager.Instance.Add(textentry);
                    }
                    break;

                case "Button:Find name":
                    {
                        textentry = new TextEntryDialog("Name", SearchAPTForName);
                        GuiManager.Instance.Add(textentry);
                    }
                    break;

                case "DayNight":
                    {
                        Slider sl = (Slider)Widgets[8];

                        float v = 23 * sl.Value;
                        int h = (int)v;
                        int m = (int)((v - h) * 60);
                        Game1.Instance.GameDateTime = new DateTime(2020, 6, 15, h, m, 0);

                    }
                    break;

                #endregion

                #region Save map
                case "Button:Save map":
                    {
                        if (mapgen != null)
                        {
                            saver = new SaveFileDialog(@"c:\Data\Fox1", "Save Map", ".map", new Rectangle((1920 - 800) / 2, 100, 800, 700), SaveMap);
                            GuiManager.Instance.Add(saver);
                        }
                        else
                        {
                            GuiManager.Instance.Add(new MessageBox("Save map error", "No map data created"));
                        }
                    }
                    break;
                    #endregion


            }
        }

        #region Callbacks
        public bool ScanAPT(String f)
        {
            if (f != "")
            {
                currentDatabase = new AirportDatabase(f);
                for (int i=0;i<Widgets.Count;i++)
                {
                    if (Widgets[i] is Button)
                    {
                        Button b = (Button)Widgets[i];
                        b.Active = true;
                    }
                }
            }
            return true;
        }

        public bool SearchAPTForName(String f)
        {
            GuiManager.Instance.RemoveTopLevelDialog();
            int indx = currentDatabase.FindName(f);
            if (indx<0)
            {
                GuiManager.Instance.Add(new MessageBox("Search for name", "No Airport node"));
                return false;
            }

            return true;
        }

        public bool SearchAPTForICAO(String f)
        {
            GuiManager.Instance.RemoveTopLevelDialog();

            int indx = currentDatabase.FindICAO(f);
            if (indx < 0)
            {
                GuiManager.Instance.Add(new MessageBox("Search for ICAO", "No Airport node"));
                return false;
            }
            
            return true;
        }

        public bool SaveMap(String f)
        {
            try
            {
                mapgen.Write(f);
                Game1.Instance.current.SaveResults(f);
            }
            catch (Exception e)
            {
                GuiManager.Instance.Add(new MessageBox("Save map error", e.ToString()));
            }

            return true;
        }

        public bool LoadWavefront(String f)
        {
            return true;
        }

        public bool LoadMap(String f)
        {
            mapgen = new MapGeneratorData();
            mapgen.Load(f);

            ActiveWidgets.Clear();

            Rectangle r2 = new Rectangle(1920 - 320, 30, 320, 940);
            SideBar sb = new SideBar("Map generation", r2);

            int x1 = r2.X + 20;
            int x2 = x1 + 40;

            Label l1 = new Label(new Vector2(x1, 100), "Start latitude");
            sb.Children.Add(l1);

            NumericUpDown n = new NumericUpDown(new Vector2(x1 + 20, 110), -90, 90, mapgen.StartLatitude);
            sb.Children.Add(n);
            ActiveWidgets.Add(n);

            l1 = new Label(new Vector2(x1, 160), "Start longitude");
            sb.Children.Add(l1);

            n = new NumericUpDown(new Vector2(x1 + 20, 170), -180, 180, mapgen.StartLongitude);
            sb.Children.Add(n);
            ActiveWidgets.Add(n);

            l1 = new Label(new Vector2(x1, 220), "Pixel size (in metres)");
            sb.Children.Add(l1);

            n = new NumericUpDown(new Vector2(x1 + 20, 230), 100, 2000, mapgen.Size);
            sb.Children.Add(n);
            ActiveWidgets.Add(n);

            l1 = new Label(new Vector2(x1, 280), "Width (in pixels)");
            sb.Children.Add(l1);

            int k = 0;
            switch (mapgen.Width)
            {
                case 512:
                    k = 0;
                    break;
                case 1024:
                    k = 1;
                    break;
                case 2048:
                    k = 2;
                    break;
                case 4096:
                    k = 3;
                    break;
                default:
                    k = 4;
                    break;
            }
            Selector se = new Selector(new Vector2(x1 + 20, 290), new String[] { "512", "1024", "2048", "4096", "8192" }, k);
            sb.Children.Add(se);
            ActiveWidgets.Add(se);

            l1 = new Label(new Vector2(x1, 340), "Height (in pixels)");
            sb.Children.Add(l1);

            switch (mapgen.Height)
            {
                case 512:
                    k = 0;
                    break;
                case 1024:
                    k = 1;
                    break;
                case 2048:
                    k = 2;
                    break;
                case 4096:
                    k = 3;
                    break;
                default:
                    k = 4;
                    break;
            }
            se = new Selector(new Vector2(x1 + 20, 350), new String[] { "512", "1024", "2048", "4096", "8192" }, k);
            sb.Children.Add(se);
            ActiveWidgets.Add(se);

            l1 = new Label(new Vector2(x1, 400), "Date");
            sb.Children.Add(l1);

            n = new NumericUpDown(new Vector2(x1 + 20, 410), 1900, 2100, mapgen.Date);
            sb.Children.Add(n);
            ActiveWidgets.Add(n);

            l1 = new Label(new Vector2(x2, 480), "Show airfields");
            sb.Children.Add(l1);
            l1 = new Label(new Vector2(x2, 510), "Show roads");
            sb.Children.Add(l1);
            l1 = new Label(new Vector2(x2, 540), "Show rivers");
            sb.Children.Add(l1);
            l1 = new Label(new Vector2(x2, 570), "Show major cities");
            sb.Children.Add(l1);
            l1 = new Label(new Vector2(x2, 600), "Show countries");
            sb.Children.Add(l1);

            CheckBox c = new CheckBox(new Vector2(x1, 458), mapgen.Airfields);
            sb.Children.Add(c);
            ActiveWidgets.Add(c);

            c = new CheckBox(new Vector2(x1, 488), mapgen.Roads);
            sb.Children.Add(c);
            ActiveWidgets.Add(c);

            c = new CheckBox(new Vector2(x1, 518), mapgen.Rivers);
            sb.Children.Add(c);
            ActiveWidgets.Add(c);

            c = new CheckBox(new Vector2(x1, 548), mapgen.Cities);
            sb.Children.Add(c);
            ActiveWidgets.Add(c);

            c = new CheckBox(new Vector2(x1, 578), mapgen.Countries);
            sb.Children.Add(c);
            ActiveWidgets.Add(c);

            Button b = new Button(new Rectangle(x2, 900, 200, 30), "Build");
            b.Parent = sb;
            sb.Children.Add(b);
            sb.Host = this;

            GuiManager.Instance.Add(sb);
            return true;
        }

        public bool LoadScene(String f)
        {
            Scene s = new Scene(f);
            SceneViewTool.SetScene(s);
            return true;
        }

        public bool LoadMaterial(String f)
        {
            basedirectory = Path.GetDirectoryName(f);
            MaterialDisplayTool mt = new MaterialDisplayTool(f);
            Game1.Instance.current = mt;
            ListView l = FindFirstListView();
            l.Clear();
            l.ClickEvent += MaterialDisplayTool.SelectionChanged;
            foreach (Material m in mt.lib.Materials)
            {
                l.AddItem(m.Name);
            }
            save_material.Active = true;
            MaterialStatusBar.mode = "Material mode " + Path.GetFileNameWithoutExtension(f);
            return false;
        }

        public bool LoadObjectPack(String f)
        {
            if (WorldDisplayTool.Instance == null)
            {
                WorldDisplayTool w = new WorldDisplayTool();
                Game1.Instance.current = w;
            }
            ObjectPack objp = new ObjectPack(f);
            String fn = Path.GetFileNameWithoutExtension(f);
            FindFirstListView().AddItem(fn);
            WorldDisplayTool.Instance.LoadedPacks.Add(objp);
            WorldDisplayTool.Instance.ScanObjects();
            return true;
        }

        public bool LoadAirport(String f)
        {

            if (WorldDisplayTool.Instance == null)
            {
                WorldDisplayTool w = new WorldDisplayTool();
                Game1.Instance.current = w;
            }
            currentAirport = new GUITestbed.DataHandlers.Fox1.Objects.Airport(f);
            WorldDisplayTool.Instance.AddAirport(currentAirport);
            WorldDisplayTool.Instance.ScanObjects();

            return true;
        }

        public bool LoadAirportObjects(String f)
        {
            if (WorldDisplayTool.Instance == null)
            {
                WorldDisplayTool w = new WorldDisplayTool();
                Game1.Instance.current = w;
            }
            AirportObjectList aol = new AirportObjectList(f);
            String fn = Path.GetFileNameWithoutExtension(f);
            FindListView(2).AddItem(fn);
            WorldDisplayTool.Instance.ObjectLists.Add(aol);
            WorldDisplayTool.Instance.ScanObjects();
            return true;
        }

        #endregion

        #region menu setups
        /// <summary>
        /// Add the standard tool bar 
        /// </summary>
        void AddToolbar()
        {
            ToolBar top = new ToolBar();
            top.AddButton("World", 128);
            top.AddButton("Scene", 128);
            top.AddButton("Object", 128);
            top.AddButton("Airports", 128);
            top.AddButton("Materials", 128);
            top.AddButton("Settings", 128);
            top.AddCloseButton();
            top.Finalise();
            Widgets.Add(top);
        }

        void SetupWorldView()
        {
            lock (Widgets)
            {
                Widgets.Clear();

                AddToolbar();

                Panel p = new Panel(new Microsoft.Xna.Framework.Rectangle(0, 31, 256, 940));
                Widgets.Add(p);

                Button b = new Button(new Microsoft.Xna.Framework.Rectangle(28, 50, 200, 30), "Map editor");
                Widgets.Add(b);
                b = new Button(new Microsoft.Xna.Framework.Rectangle(28, 90, 200, 30), "Terrain editor");
                Widgets.Add(b);


                StatusBar s = new StatusBar();
                s.mode = "World mode";
                Widgets.Add(s);
            }
        }

        void SetupAirportView()
        {
            lock (Widgets)
            {
                Widgets.Clear();

                AddToolbar();

                Panel p = new Panel(new Microsoft.Xna.Framework.Rectangle(0, 31, 256, 940));
                Widgets.Add(p);                         // 0

                Rectangle r2 = new Rectangle(1920 - 320, 30, 320, 940);
                SideBar sb = new SideBar("Airport generation", r2);
                Widgets.Add(sb);                        // 1

                Label l = new Label(new Vector2(1920 - 230, 93), "Object packs");
                Widgets.Add(l);                         // 2

                Rectangle r3 = new Rectangle(1638, 100, 235, 200);
                ListView lv = new ListView(r3);
                Widgets.Add(lv);                        // 3

                l = new Label(new Vector2(1920 - 230, 343), "Object maps");
                Widgets.Add(l);                         // 4

                r3 = new Rectangle(1638, 350, 235, 200);
                lv = new ListView(r3);
                Widgets.Add(lv);                        // 5

                l = new Label(new Vector2(1920 - 230, 570), "Day / Night");
                Widgets.Add(l);                         // 6

                r3 = new Rectangle(1638, 580, 235, 20);
                Slider ss = new Slider(r3, 0.5f, "DayNight");
                Widgets.Add(ss);                        // 7

                Button b = new Button(new Microsoft.Xna.Framework.Rectangle(28, 65, 200, 30), "Load object pack");
                Widgets.Add(b);
                b = new Button(new Microsoft.Xna.Framework.Rectangle(28, 105, 200, 30), "Load object map");
                Widgets.Add(b);
                b = new Button(new Microsoft.Xna.Framework.Rectangle(28, 145, 200, 30), "Load airport");
                Widgets.Add(b);

                StatusBar s = new StatusBar();
                s.mode = "Airport mode";
                Widgets.Add(s);

                GroupBox gb = new GroupBox("FSX", new Rectangle(15, 45, 226, 160));
                Widgets.Add(gb);

                gb = new GroupBox("APT", new Rectangle(15, 215, 226, 160));
                Widgets.Add(gb);

                b = new Button(new Microsoft.Xna.Framework.Rectangle(28, 235, 200, 30), "Scan APT.DAT");
                Widgets.Add(b);
                b = new Button(new Microsoft.Xna.Framework.Rectangle(28, 275, 200, 30), "Find ICAO");
                b.Active = false;
                Widgets.Add(b);
                b = new Button(new Microsoft.Xna.Framework.Rectangle(28, 315, 200, 30), "Find name");
                b.Active = false;
                Widgets.Add(b);
            }
        }

        void SetupMaterialView()
        {
            lock (Widgets)
            {
                Widgets.Clear();

                AddToolbar();

                Panel p = new Panel(new Microsoft.Xna.Framework.Rectangle(0, 31, 256, 940));
                Widgets.Add(p);

                Button b = new Button(new Microsoft.Xna.Framework.Rectangle(28, 50, 200, 30), "Edit material");
                Widgets.Add(b);

                save_material = new Button(new Microsoft.Xna.Framework.Rectangle(28, 90, 200, 30), "Save material");
                save_material.Active = false;
                Widgets.Add(save_material);

                Rectangle r2 = new Rectangle(1920 - 320, 30, 320, 940);
                SideBar sb = new SideBar("Material editor", r2);
                Widgets.Add(sb);

                Rectangle r3 = new Rectangle(1638, 100, 235, 200);
                ListView lv = new ListView(r3);
                Widgets.Add(lv);

                Label l = new Label(new Vector2(1610, 320), "Double sided");
                Widgets.Add(l);
                l = new Label(new Vector2(1610, 340), "Sort");
                Widgets.Add(l);
                l = new Label(new Vector2(1610, 360), "Glass");
                Widgets.Add(l);
                l = new Label(new Vector2(1610, 380), "Ambient");
                Widgets.Add(l);
                l = new Label(new Vector2(1610, 400), "Diffuse");
                Widgets.Add(l);
                l = new Label(new Vector2(1610, 420), "Specular");
                Widgets.Add(l);
                l = new Label(new Vector2(1610, 440), "Specular power");
                Widgets.Add(l);
                l = new Label(new Vector2(1610, 460), "Shine");
                Widgets.Add(l);
                l = new Label(new Vector2(1610, 480), "Wrap X");
                Widgets.Add(l);
                l = new Label(new Vector2(1610, 500), "Wrap Y");
                Widgets.Add(l);
                l = new Label(new Vector2(1610, 520), "Min linear");
                Widgets.Add(l);
                l = new Label(new Vector2(1610, 540), "Mag linear");
                Widgets.Add(l);
                l = new Label(new Vector2(1610, 560), "Blend");
                Widgets.Add(l);
                l = new Label(new Vector2(1610, 580), "Blend Add");
                Widgets.Add(l);
                l = new Label(new Vector2(1610, 600), "No Texture");
                Widgets.Add(l);
                l = new Label(new Vector2(1610, 620), "Depth offset");
                Widgets.Add(l);
                l = new Label(new Vector2(1610, 640), "No write z");
                Widgets.Add(l);
                l = new Label(new Vector2(1610, 660), "Test value");
                Widgets.Add(l);
                l = new Label(new Vector2(1610, 680), "Transparent border");
                Widgets.Add(l);
                l = new Label(new Vector2(1610, 700), "Test A");
                Widgets.Add(l);
                l = new Label(new Vector2(1610, 720), "Test Z");
                Widgets.Add(l);
                l = new Label(new Vector2(1610, 740), "Texture");
                Widgets.Add(l);
                l = new Label(new Vector2(1610, 760), "Red");
                Widgets.Add(l);
                l = new Label(new Vector2(1610, 780), "Green");
                Widgets.Add(l);
                l = new Label(new Vector2(1610, 800), "Blue");
                Widgets.Add(l);
                l = new Label(new Vector2(1610, 820), "Alpha");
                Widgets.Add(l);

                SmallCheckbox c = new SmallCheckbox(new Vector2(1850, 305), true);
                c.ClickEvent += MaterialDisplayTool.CheckboxStateChanged;
                Widgets.Add(c);
                c = new SmallCheckbox(new Vector2(1850, 325), true);
                c.ClickEvent += MaterialDisplayTool.CheckboxStateChanged;
                Widgets.Add(c);
                c = new SmallCheckbox(new Vector2(1850, 345), true);
                c.ClickEvent += MaterialDisplayTool.CheckboxStateChanged;
                Widgets.Add(c);
                c = new SmallCheckbox(new Vector2(1850, 465), true);
                c.ClickEvent += MaterialDisplayTool.CheckboxStateChanged;
                Widgets.Add(c);
                c = new SmallCheckbox(new Vector2(1850, 485), true);
                c.ClickEvent += MaterialDisplayTool.CheckboxStateChanged;
                Widgets.Add(c);
                c = new SmallCheckbox(new Vector2(1850, 505), true);
                c.ClickEvent += MaterialDisplayTool.CheckboxStateChanged;
                Widgets.Add(c);
                c = new SmallCheckbox(new Vector2(1850, 525), true);
                c.ClickEvent += MaterialDisplayTool.CheckboxStateChanged;
                Widgets.Add(c);
                c = new SmallCheckbox(new Vector2(1850, 545), true);
                c.ClickEvent += MaterialDisplayTool.CheckboxStateChanged;
                Widgets.Add(c);
                c = new SmallCheckbox(new Vector2(1850, 565), true);
                c.ClickEvent += MaterialDisplayTool.CheckboxStateChanged;
                Widgets.Add(c);
                c = new SmallCheckbox(new Vector2(1850, 585), true);
                c.ClickEvent += MaterialDisplayTool.CheckboxStateChanged;
                Widgets.Add(c);
                c = new SmallCheckbox(new Vector2(1850, 625), true);
                c.ClickEvent += MaterialDisplayTool.CheckboxStateChanged;
                Widgets.Add(c);
                c = new SmallCheckbox(new Vector2(1850, 665), true);
                c.ClickEvent += MaterialDisplayTool.CheckboxStateChanged;
                Widgets.Add(c);
                c = new SmallCheckbox(new Vector2(1850, 685), true);
                c.ClickEvent += MaterialDisplayTool.CheckboxStateChanged;
                Widgets.Add(c);
                c = new SmallCheckbox(new Vector2(1850, 705), true);
                c.ClickEvent += MaterialDisplayTool.CheckboxStateChanged;
                Widgets.Add(c);

                Slider sl = new Slider(new Rectangle(1770, 365, 120, 18), 1, "Ambient");
                sl.ClickEvent+= MaterialDisplayTool.SliderValueChanged;
                Widgets.Add(sl);
                sl = new Slider(new Rectangle(1770, 385, 120, 18), 1, "Diffuse");
                sl.ClickEvent += MaterialDisplayTool.SliderValueChanged;
                Widgets.Add(sl);
                sl = new Slider(new Rectangle(1770, 405, 120, 18), 1, "Specular");
                sl.ClickEvent += MaterialDisplayTool.SliderValueChanged;
                Widgets.Add(sl);
                sl = new Slider(new Rectangle(1770, 425, 120, 18), 256, "SpecularPow");
                sl.ClickEvent += MaterialDisplayTool.SliderValueChanged;
                Widgets.Add(sl);
                sl = new Slider(new Rectangle(1770, 445, 120, 18), 1, "Shine");
                sl.ClickEvent += MaterialDisplayTool.SliderValueChanged;
                Widgets.Add(sl);
                sl = new Slider(new Rectangle(1770, 605, 120, 18), 0,-16,16, "Depth offset");
                sl.ClickEvent += MaterialDisplayTool.SliderValueChanged;
                Widgets.Add(sl);
                sl = new Slider(new Rectangle(1770, 645, 120, 18), 1, "Test value");
                sl.ClickEvent += MaterialDisplayTool.SliderValueChanged;
                Widgets.Add(sl);
                sl = new Slider(new Rectangle(1770, 745, 120, 18), 1, "Red");
                sl.ClickEvent += MaterialDisplayTool.SliderValueChanged;
                Widgets.Add(sl);
                sl = new Slider(new Rectangle(1770, 765, 120, 18), 1, "Green");
                sl.ClickEvent += MaterialDisplayTool.SliderValueChanged;
                Widgets.Add(sl);
                sl = new Slider(new Rectangle(1770, 785, 120, 18), 1, "Blue");
                sl.ClickEvent += MaterialDisplayTool.SliderValueChanged;
                Widgets.Add(sl);
                sl = new Slider(new Rectangle(1770, 805, 120, 18), 1, "Alpha");
                sl.ClickEvent += MaterialDisplayTool.SliderValueChanged;
                Widgets.Add(sl);

                TextBox t = new TextBox(new Rectangle(1700, 725, 210, 18), "");
                Widgets.Add(t);

                MaterialStatusBar = new StatusBar();
                MaterialStatusBar.mode = "Material mode";
                Widgets.Add(MaterialStatusBar);
            }
        }

        void SetupObjectView()
        {
            lock (Widgets)
            {
                Widgets.Clear();

                AddToolbar();

                Panel p = new Panel(new Microsoft.Xna.Framework.Rectangle(0, 31, 256, 940));
                Widgets.Add(p);

                Button b = new Button(new Microsoft.Xna.Framework.Rectangle(28, 50, 200, 30), "Object converter");
                Widgets.Add(b);
                b = new Button(new Microsoft.Xna.Framework.Rectangle(28, 90, 200, 30), "Object animator");
                Widgets.Add(b);
                b = new Button(new Microsoft.Xna.Framework.Rectangle(28, 130, 200, 30), "Object detailer");
                Widgets.Add(b);

                StatusBar s = new StatusBar();
                s.mode = "Object mode";
                Widgets.Add(s);
            }
        }

        void SetupObjectConverterView()
        {
            lock (Widgets)
            {
                Widgets.Clear();

                AddToolbar();

                Panel p = new Panel(new Microsoft.Xna.Framework.Rectangle(0, 31, 256, 940));
                Widgets.Add(p);

                Button b = new Button(new Microsoft.Xna.Framework.Rectangle(28, 50, 200, 30), "Load Wavefront");
                Widgets.Add(b);
                b = new Button(new Microsoft.Xna.Framework.Rectangle(28, 90, 200, 30), "Load MDL");
                Widgets.Add(b);
                b = new Button(new Microsoft.Xna.Framework.Rectangle(28, 130, 200, 30), "Load MSH");
                Widgets.Add(b);
                b = new Button(new Microsoft.Xna.Framework.Rectangle(28, 920, 200, 30), "Go to object mode");
                Widgets.Add(b);

                StatusBar s = new StatusBar();
                s.mode = "Object converter mode";
                Widgets.Add(s);
            }
        }

        void SetupMapView()
        {
            lock (Widgets)
            {
                Widgets.Clear();

                AddToolbar();

                Panel p = new Panel(new Microsoft.Xna.Framework.Rectangle(0, 31, 256, 940));
                Widgets.Add(p);

                Button b = new Button(new Microsoft.Xna.Framework.Rectangle(28, 50, 200, 30), "New map");
                Widgets.Add(b);
                b = new Button(new Microsoft.Xna.Framework.Rectangle(28, 90, 200, 30), "Load map");
                Widgets.Add(b);
                b = new Button(new Microsoft.Xna.Framework.Rectangle(28, 130, 200, 30), "Save map");
                Widgets.Add(b);

                StatusBar s = new StatusBar();
                s.mode = "Map mode";
                Widgets.Add(s);
            }
        }

        void SetupSceneView()
        {
            lock (Widgets)
            {
                Widgets.Clear();

                AddToolbar();

                Panel p = new Panel(new Microsoft.Xna.Framework.Rectangle(0, 31, 256, 940));
                Widgets.Add(p);

                Button b = new Button(new Microsoft.Xna.Framework.Rectangle(28, 50, 200, 30), "New scene ");
                Widgets.Add(b);
                b = new Button(new Microsoft.Xna.Framework.Rectangle(28, 90, 200, 30), "Load scene");
                Widgets.Add(b);
                b = new Button(new Microsoft.Xna.Framework.Rectangle(28, 130, 200, 30), "Save scene");
                Widgets.Add(b);

                b = new Button(new Microsoft.Xna.Framework.Rectangle(28, 530, 200, 30), "Light Volume");
                Widgets.Add(b);

                StatusBar s = new StatusBar();
                s.mode = "Scene mode";
                Widgets.Add(s);
            }
            if (SceneViewTool.Instance == null)
            {
                SceneViewTool w = new SceneViewTool();
                Game1.Instance.current = w;
            }
        }

        #endregion

        #region Helpers
        public Slider FindSlider(String id)
        {
            foreach(Widget w in Widgets)
            {
                if (w is Slider)
                {
                    Slider sl = (Slider)w;
                    if (sl.ID == id)
                        return sl;
                }
            }
            return null;
        }

        public TextBox FindTextbox(int n)
        {
            foreach (Widget w in Widgets)
            {
                if (w is TextBox)
                {
                    TextBox sl = (TextBox)w;
                    n--;
                    if (n < 0)
                        return sl;
                }
            }
            return null;
        }
        public CheckBox FindCheckbox(int n)
        {
            foreach (Widget w in Widgets)
            {
                if (w is CheckBox)
                {
                    CheckBox sl = (CheckBox)w;
                    n--;
                    if (n < 0)
                        return sl;
                }
            }
            return null;
        }
        public SmallCheckbox FindSmallCheckbox(int n)
        {
            foreach (Widget w in Widgets)
            {
                if (w is SmallCheckbox)
                {
                    SmallCheckbox sl = (SmallCheckbox)w;
                    n--;
                    if (n < 0)
                        return sl;
                }
            }
            return null;
        }
        #endregion
    }
}
