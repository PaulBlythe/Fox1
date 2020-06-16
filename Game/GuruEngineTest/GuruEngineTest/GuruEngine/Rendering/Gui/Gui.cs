using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

using GuruEngine.Rendering.Gui.GuiItems;
using GuruEngine.Text;

namespace GuruEngine.Rendering.Gui
{
    public class Gui
    {
        public GuiManager.GuiIdentifiers ID;
        List<GuiItem> Items = new List<GuiItem>();

        public Gui()
        {

        }

        public void Update(Vector2 mousePosition, int mouseButtons, float dt, int wheelDelta)
        {
            foreach (GuiItem g in Items)
            {
                g.Update(mousePosition, mouseButtons, dt, wheelDelta);
            }
        }

        public void Draw(GlyphBatch batch)
        {
            foreach (GuiItem g in Items)
            {
                if (g.drawOrder != GuiItem.DrawOrder.Last)
                    g.Draw(batch);
            }
        }

        public void Draw()
        {
            foreach (GuiItem g in Items)
            {
                if (g.drawOrder != GuiItem.DrawOrder.First)
                    g.Draw(null);
            }
        }

        public void Load(GuiManager.GuiIdentifiers id, Vector4 position)
        {
            ID = id;
            switch (id)
            {
                #region Developer tools widget
                case GuiManager.GuiIdentifiers.DeveloperToolsWidget:
                    {
                        AddWidget(GuiItem.GuiWidgetIdentifiers.Frame, position);

                        #region Header
                        Vector4 l1pos = position;
                        l1pos.X += position.Z * 0.5f;
                        l1pos.Y += 64;
                        Label l = new Label("Developer Menu", Label.Alignment.Centre, GuiManager.FontStyle.Title, Color.White);
                        l.Load();
                        l.Position(l1pos);
                        Items.Add(l);
                        #endregion

                        #region Joystick test scene
                        Vector4 b1 = new Vector4();
                        b1.X = position.X + 128;
                        b1.Y = position.Y + 120;
                        b1.Z = 256;
                        b1.W = 48;
                        AddWidget(GuiItem.GuiWidgetIdentifiers.Button, b1, "Joystick test", "JoystickPressed");
                        #endregion

                        #region Map test
                        b1 = new Vector4();
                        b1.X = position.X + 128;
                        b1.Y = position.Y + 180;
                        b1.Z = 256;
                        b1.W = 48;
                        AddWidget(GuiItem.GuiWidgetIdentifiers.Button, b1, "Map test", "MapPressed");
                        #endregion

                        #region 3D test
                        b1 = new Vector4();
                        b1.X = position.X + 128;
                        b1.Y = position.Y + 240;
                        b1.Z = 256;
                        b1.W = 48;
                        AddWidget(GuiItem.GuiWidgetIdentifiers.Button, b1, "3D test", "3DPressed");
                        #endregion

                        #region Instrument test
                        b1 = new Vector4();
                        b1.X = position.X + 128;
                        b1.Y = position.Y + 300;
                        b1.Z = 256;
                        b1.W = 48;
                        AddWidget(GuiItem.GuiWidgetIdentifiers.Button, b1, "Instrument test", "InstrumentPressed");
                        #endregion

                        #region Return
                        b1 = new Vector4();
                        b1.X = position.X + (position.Z * 0.5f) - 128;
                        b1.Y = position.Y + 640;
                        b1.Z = 256;
                        b1.W = 48;
                        AddWidget(GuiItem.GuiWidgetIdentifiers.Button, b1, "Return", "ReturnPressed");
                        #endregion

                    }
                    break;
                #endregion

                #region Options menu
                case GuiManager.GuiIdentifiers.OptionsMenuWidget:
                    {
                        AddWidget(GuiItem.GuiWidgetIdentifiers.Frame, position);

                        #region Header
                        Vector4 l1pos = position;
                        l1pos.X += position.Z * 0.5f;
                        l1pos.Y += 64;
                        Label l = new Label("Options Menu", Label.Alignment.Centre, GuiManager.FontStyle.Title, Color.White);
                        l.Load();
                        l.Position(l1pos);
                        Items.Add(l);
                        #endregion

                        #region Game Play Options
                        Vector4 b1 = new Vector4();
                        b1.X = position.X + 128;
                        b1.Y = position.Y + 120;
                        b1.Z = 256;
                        b1.W = 48;
                        AddWidget(GuiItem.GuiWidgetIdentifiers.Button, b1, "Gameplay", "GameplayPressed");

                        b1.X += 320;
                        b1.Y += 28;
                        AddWidget(GuiItem.GuiWidgetIdentifiers.Label, b1, "Difficulty settings and the like", "left");
                        #endregion

                        #region Graphics Options
                        b1 = new Vector4();
                        b1.X = position.X + 128;
                        b1.Y = position.Y + 180;
                        b1.Z = 256;
                        b1.W = 48;
                        AddWidget(GuiItem.GuiWidgetIdentifiers.Button, b1, "Graphics", "GraphicsPressed");

                        b1.X += 320;
                        b1.Y += 28;
                        AddWidget(GuiItem.GuiWidgetIdentifiers.Label, b1, "Adjust render settings for optimal performance on your machine", "left");
                        #endregion

                        #region Audio Options
                        b1 = new Vector4();
                        b1.X = position.X + 128;
                        b1.Y = position.Y + 240;
                        b1.Z = 256;
                        b1.W = 48;
                        AddWidget(GuiItem.GuiWidgetIdentifiers.Button, b1, "Audio", "AudioPressed");

                        b1.X += 320;
                        b1.Y += 28;
                        AddWidget(GuiItem.GuiWidgetIdentifiers.Label, b1, "How much noise do you want", "left");
                        #endregion

                        #region Input Options
                        b1 = new Vector4();
                        b1.X = position.X + 128;
                        b1.Y = position.Y + 300;
                        b1.Z = 256;
                        b1.W = 48;
                        AddWidget(GuiItem.GuiWidgetIdentifiers.Button, b1, "Input", "InputPressed");

                        b1.X += 320;
                        b1.Y += 28;
                        AddWidget(GuiItem.GuiWidgetIdentifiers.Label, b1, "Input devices and settings", "left");
                        #endregion

                        #region Aircraft setup
                        b1 = new Vector4();
                        b1.X = position.X + 128;
                        b1.Y = position.Y + 360;
                        b1.Z = 256;
                        b1.W = 48;
                        AddWidget(GuiItem.GuiWidgetIdentifiers.Button, b1, "Aircraft setup", "AircraftPressed");

                        b1.X += 320;
                        b1.Y += 28;
                        AddWidget(GuiItem.GuiWidgetIdentifiers.Label, b1, "Configure input settings for a particular aircraft", "left");
                        #endregion

                        #region Developer setup
                        b1 = new Vector4();
                        b1.X = position.X + 128;
                        b1.Y = position.Y + 420;
                        b1.Z = 256;
                        b1.W = 48;
                        AddWidget(GuiItem.GuiWidgetIdentifiers.Button, b1, "Developer", "DeveloperPressed");

                        b1.X += 320;
                        b1.Y += 28;
                        AddWidget(GuiItem.GuiWidgetIdentifiers.Label, b1, "Various developer helpers. Engine testing. Mesh viewing. Physics tests.", "left");
                        #endregion

                        #region Return
                        b1 = new Vector4();
                        b1.X = position.X + 128;
                        b1.Y = position.Y + 560;
                        b1.Z = 256;
                        b1.W = 48;
                        AddWidget(GuiItem.GuiWidgetIdentifiers.Button, b1, "Return", "ReturnPressed");

                        b1.X += 320;
                        b1.Y += 28;
                        AddWidget(GuiItem.GuiWidgetIdentifiers.Label, b1, "Return to last menu", "left");
                        #endregion
                    }
                    break;
                #endregion

                #region Pause menu
                case GuiManager.GuiIdentifiers.PauseMenuWidget:
                    {
                        AddWidget(GuiItem.GuiWidgetIdentifiers.Frame, position);

                        #region Header
                        Vector4 l1pos = position;
                        l1pos.X += position.Z * 0.5f;
                        l1pos.Y += 64;
                        Label l = new Label("Pause Menu", Label.Alignment.Centre, GuiManager.FontStyle.Title, Color.White);
                        l.Load();
                        l.Position(l1pos);
                        Items.Add(l);
                        #endregion

                        #region Options
                        Vector4 b1 = new Vector4();
                        b1.X = position.X + (position.Z * 0.5f) - 128;
                        b1.Y = position.Y + 120;
                        b1.Z = 256;
                        b1.W = 48;
                        AddWidget(GuiItem.GuiWidgetIdentifiers.Button, b1, "Options", "OptionsPressed");
                        #endregion

                        #region Main menu
                        b1 = new Vector4();
                        b1.X = position.X + (position.Z * 0.5f) - 128;
                        b1.Y = position.Y + 180;
                        b1.Z = 256;
                        b1.W = 48;
                        AddWidget(GuiItem.GuiWidgetIdentifiers.Button, b1, "Main menu", "MenuMenuPressed");
                        #endregion

                        #region Return
                        b1 = new Vector4();
                        b1.X = position.X + (position.Z * 0.5f) - 128;
                        b1.Y = position.Y + 240;
                        b1.Z = 256;
                        b1.W = 48;
                        AddWidget(GuiItem.GuiWidgetIdentifiers.Button, b1, "Return to game", "ReturnPressed");
                        #endregion

                        #region Quit
                        b1 = new Vector4();
                        b1.X = position.X + (position.Z * 0.5f) - 128;
                        b1.Y = position.Y + 360;
                        b1.Z = 256;
                        b1.W = 48;
                        AddWidget(GuiItem.GuiWidgetIdentifiers.Button, b1, "Quit game", "QuitPressed");
                        #endregion

                    }
                    break;
                #endregion

                #region Map position widget
                case GuiManager.GuiIdentifiers.MapPositionWidget:
                    {
                        #region Map Position widget 
                        AddWidget(GuiItem.GuiWidgetIdentifiers.Frame, position);

                        #region Labels
                        #region Header
                        Vector4 l1pos = position;
                        l1pos.X += position.Z * 0.5f;
                        l1pos.Y += 32;
                        Label l = new Label("Map location", Label.Alignment.Centre, GuiManager.FontStyle.Medium, Color.White);
                        l.Load();
                        l.Position(l1pos);
                        Items.Add(l);
                        #endregion

                        #region Latitude
                        l1pos = position;
                        l1pos.X += 15;
                        l1pos.Y += 70;
                        l = new Label("Latitude", Label.Alignment.Left, GuiManager.FontStyle.Smaller, Color.White);
                        l.Load();
                        l.Position(l1pos);
                        Items.Add(l);
                        #endregion

                        #region Longitude
                        l1pos = position;
                        l1pos.X += 15;
                        l1pos.Y += 130;
                        l = new Label("Longitude", Label.Alignment.Left, GuiManager.FontStyle.Smaller, Color.White);
                        l.Load();
                        l.Position(l1pos);
                        Items.Add(l);
                        #endregion

                        #region Width
                        l1pos = position;
                        l1pos.X += 15;
                        l1pos.Y += 190;
                        l = new Label("Width", Label.Alignment.Left, GuiManager.FontStyle.Smaller, Color.White);
                        l.Load();
                        l.Position(l1pos);
                        Items.Add(l);
                        #endregion

                        #region Height
                        l1pos = position;
                        l1pos.X += 15;
                        l1pos.Y += 250;
                        l = new Label("Height", Label.Alignment.Left, GuiManager.FontStyle.Smaller, Color.White);
                        l.Load();
                        l.Position(l1pos);
                        Items.Add(l);
                        #endregion
                        #endregion

                        #region Small Buttons
                        Vector4 b1 = new Vector4();
                        b1.X = position.X + 15;
                        b1.Y = position.Y + 80;
                        b1.Z = 24;
                        b1.W = 24;
                        AddWidget(GuiItem.GuiWidgetIdentifiers.SmallButton, b1, "-", "StartLatitudeMinus");

                        b1 = new Vector4();
                        b1.X = position.X + 200;
                        b1.Y = position.Y + 80;
                        b1.Z = 24;
                        b1.W = 24;
                        AddWidget(GuiItem.GuiWidgetIdentifiers.SmallButton, b1, "+", "StartLatitudePlus");
                        #endregion
                        

                        #endregion

                    }
                    break;
               #endregion

            }
        }

        public void UnLoad()
        {
            Items.Clear();
        }

        public void AddWidget(GuiItem.GuiWidgetIdentifiers id, Vector4 position)
        {
            GuiItem newItem = null;

            switch (id)
            {
                case GuiItem.GuiWidgetIdentifiers.Frame:
                    {
                        newItem = new Frame();
                    }
                    break;

            }
            if (newItem != null)
            {
                newItem.Load();
                newItem.Position(position);

                Items.Add(newItem);
            }
        }

        public void AddWidget(GuiItem.GuiWidgetIdentifiers id, Vector4 position, String text, String evt)
        {
            GuiItem newItem = null;

            switch (id)
            {
                case GuiItem.GuiWidgetIdentifiers.Label:
                    {
                        switch (evt)
                        {
                            case "left":
                                newItem = new Label(text, Label.Alignment.Left, GuiManager.FontStyle.Small, Color.White);
                                break;
                            case "right":
                                newItem = new Label(text, Label.Alignment.Right, GuiManager.FontStyle.Small, Color.White);
                                break;
                            default:
                                newItem = new Label(text, Label.Alignment.Centre, GuiManager.FontStyle.Small, Color.White);
                                break;
                        }
                    }
                    break;
                case GuiItem.GuiWidgetIdentifiers.Button:
                    {
                        newItem = new Button(text, evt);
                    }
                    break;
                case GuiItem.GuiWidgetIdentifiers.SmallButton:
                    {
                        newItem = new SmallButton(text, evt);
                    }
                    break;

            }
            if (newItem != null)
            {
                newItem.Load();
                newItem.Position(position);

                Items.Add(newItem);
            }
        }


    }
}
