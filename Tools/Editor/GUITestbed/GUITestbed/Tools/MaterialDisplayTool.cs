using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using GUITestbed.DataHandlers.Fox1;
using GUITestbed.SerialisableData;
using GUITestbed.Rendering._3D;
using GUITestbed.DataHandlers.Fox1.Objects;
using GUITestbed.Rendering;
using GUITestbed.DataHandlers;
using GUITestbed.DataHandlers.IL2;
using GUITestbed.GUI.Items;
using GUITestbed.GUI.Widgets;

namespace GUITestbed.Tools
{
    public class MaterialDisplayTool : Tool
    {
        public MeshMaterialLibrary lib;
        public Material selected = null;

        public static MaterialDisplayTool Instance;
        public GameTime time;
        Effect fx;
        Matrix Projection;
        SkyDomeSystem sky;
        FreeCamera camera;
        GroundPlaneSystem ground = null;
        Model sphere;
        String sourceFile;
        float Angle = 0;
        RasterizerState state = new RasterizerState();

        VertexPositionNormalTexture[] verts = new VertexPositionNormalTexture[6];


        public MaterialDisplayTool(String file)
        {
            sourceFile = file;
            Instance = this;
            String dir = Path.GetDirectoryName(file);
            using (BinaryReader b = new BinaryReader(File.Open(file, FileMode.Open)))
            {
                lib = new MeshMaterialLibrary(dir, b);
            }
            time = new GameTime(new TimeSpan(12, 0, 0), new TimeSpan(12, 0, 0));
            fx = Game1.Instance.Content.Load<Effect>(@"Shaders/ModelMesh");
            Projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(60), Game1.Instance.GraphicsDevice.Viewport.AspectRatio, 0.5f, 65000);
            camera = new FreeCamera(Vector3.Zero, 0, 0, 0.5f, 65000, Game1.Instance.GraphicsDevice);
            sky = new SkyDomeSystem(Game1.Instance, camera, time, Game1.Instance.GraphicsDevice);
            sky.Initialize();
            sky.LoadContent();
            ground = new GroundPlaneSystem((float)(-2), 1000, 65000);
            ground.Load();

            sphere = Game1.Instance.Content.Load<Model>(@"Models/sphere");

            foreach (ModelMesh mm in sphere.Meshes)
            {
                foreach (ModelMeshPart mmp in mm.MeshParts)
                {
                    mmp.Effect = fx;
                }
            }

            VertexPositionNormalTexture v = new VertexPositionNormalTexture();
            v.Position = new Vector3(0, 15, -35);
            v.Normal = Vector3.Backward;
            v.TextureCoordinate = new Vector2(0, 0);
            verts[0] = v;

            v = new VertexPositionNormalTexture();
            v.Position = new Vector3(20, 15, -35);
            v.Normal = Vector3.Backward;
            v.TextureCoordinate = new Vector2(1, 0);
            verts[1] = v;

            v = new VertexPositionNormalTexture();
            v.Position = new Vector3(20, -5, -35);
            v.Normal = Vector3.Backward;
            v.TextureCoordinate = new Vector2(1, 1);
            verts[2] = v;

            v = new VertexPositionNormalTexture();
            v.Position = new Vector3(0, 15, -35);
            v.Normal = Vector3.Backward;
            v.TextureCoordinate = new Vector2(0, 0);
            verts[3] = v;

            v = new VertexPositionNormalTexture();
            v.Position = new Vector3(20, -5, -35);
            v.Normal = Vector3.Backward;
            v.TextureCoordinate = new Vector2(1, 1);
            verts[4] = v;

            v = new VertexPositionNormalTexture();
            v.Position = new Vector3(0, -5, -35);
            v.Normal = Vector3.Backward;
            v.TextureCoordinate = new Vector2(0, 1);
            verts[5] = v;

            state.CullMode = CullMode.CullCounterClockwiseFace;

        }

        public override void Draw()
        {
            Game1.Instance.GraphicsDevice.DepthStencilState = DepthStencilState.Default;
            
            Angle += 0.002f;
            sky.PreDraw(time);
            sky.Draw(camera.View, Projection, camera.Transform.Translation);
            ground.Draw(camera);

            if (selected != null)
            {
                RasterizerState old = Game1.Instance.GraphicsDevice.RasterizerState;
                Game1.Instance.GraphicsDevice.RasterizerState = state;
                fx.Parameters["World"].SetValue(Matrix.CreateRotationY(Angle) * Matrix.CreateTranslation(new Vector3(-8, 3, -20)));
                fx.Parameters["Projection"].SetValue(Projection);
                fx.Parameters["WorldInverseTranspose"].SetValue(Matrix.Transpose(Matrix.Invert(Matrix.CreateTranslation(new Vector3(-8, 3, -20)))));
                fx.Parameters["View"].SetValue(camera.View);
                fx.Parameters["SunDirection"].SetValue(Vector3.Forward);

                selected.Apply(fx);
                foreach (ModelMesh m in sphere.Meshes)
                {
                    m.Draw();
                }

                Game1.Instance.GraphicsDevice.RasterizerState = old;
                fx.Parameters["World"].SetValue(Matrix.Identity);
                fx.Parameters["WorldInverseTranspose"].SetValue(Matrix.Identity);

                foreach (EffectPass pass in fx.CurrentTechnique.Passes)
                {
                    pass.Apply();

                    Game1.Instance.GraphicsDevice.DrawUserPrimitives<VertexPositionNormalTexture>(PrimitiveType.TriangleList, verts, 0, 2);
                }


                Game1.Instance.spriteBatch.Begin();
                Game1.Instance.spriteBatch.Draw(selected.Texture, new Rectangle(300, 600, 256, 256), Color.White);
                Game1.Instance.spriteBatch.End();
            }
        }

        public override void SaveResults(string path)
        {

        }

        public override void Update(float dt)
        {
            time.TotalGameTime.Add(new TimeSpan(0, 0, 0, 0, (int)(dt * 1000)));
            sky.Update(time);
            camera.Update();
            ground.UpdateLighting(sky.LightIntensity * sky.SunColor * 0.4f, sky.SunColor, sky.GetDirection());
        }

        public void SetMaterial(string s)
        {
            string[] parts = s.Split(':');
            foreach (Material m in lib.Materials)
            {
                if (m.Name == parts[1])
                {
                    selected = m;
                    MainMenu.Instance.FindSlider("Ambient").Value = selected.Ambient;
                    MainMenu.Instance.FindSlider("Diffuse").Value = selected.Diffuse;
                    MainMenu.Instance.FindSlider("Specular").Value = selected.Specular;
                    MainMenu.Instance.FindSlider("SpecularPow").Value = selected.SpecularPow;
                    MainMenu.Instance.FindSlider("Shine").Value = selected.Shine;
                    MainMenu.Instance.FindSlider("Depth offset").Value = selected.tfDepthOffset;
                    MainMenu.Instance.FindSlider("Test value").Value = selected.AlphaTestVal;
                    MainMenu.Instance.FindSlider("Red").Value = selected.Colour[0];
                    MainMenu.Instance.FindSlider("Green").Value = selected.Colour[1];
                    MainMenu.Instance.FindSlider("Blue").Value = selected.Colour[2];
                    MainMenu.Instance.FindSlider("Alpha").Value = selected.Colour[3];

                    MainMenu.Instance.FindTextbox(0).Text = selected.tname;

                    MainMenu.Instance.FindSmallCheckbox(0).Selected = selected.tfDoubleSided;
                    MainMenu.Instance.FindSmallCheckbox(1).Selected = selected.Sort;
                    MainMenu.Instance.FindSmallCheckbox(2).Selected = selected.Glass;
                    MainMenu.Instance.FindSmallCheckbox(3).Selected = selected.tfWrapX;
                    MainMenu.Instance.FindSmallCheckbox(4).Selected = selected.tfWrapY;
                    MainMenu.Instance.FindSmallCheckbox(5).Selected = selected.tfMinLinear;
                    MainMenu.Instance.FindSmallCheckbox(6).Selected = selected.tfMagLinear;
                    MainMenu.Instance.FindSmallCheckbox(7).Selected = selected.tfBlend;
                    MainMenu.Instance.FindSmallCheckbox(8).Selected = selected.tfBlendAdd;
                    MainMenu.Instance.FindSmallCheckbox(9).Selected = selected.tfNoTexture;
                    MainMenu.Instance.FindSmallCheckbox(10).Selected = selected.tfNoWriteZ;
                    MainMenu.Instance.FindSmallCheckbox(11).Selected = selected.tfTranspBorder;
                    MainMenu.Instance.FindSmallCheckbox(12).Selected = selected.tfTestA;
                    MainMenu.Instance.FindSmallCheckbox(13).Selected = selected.tfTestZ;

                    return;
                }
            }
        }

        public void UpdateSliderValue(object sender, String s)
        {
            if (selected != null)
            {
                Slider sl = (Slider)sender;
                switch (s)
                {
                    case "Ambient":
                        selected.Ambient = sl.Value;
                        break;
                    case "Diffuse":
                        selected.Diffuse = sl.Value;
                        break;
                    case "Specular":
                        selected.Specular = sl.Value;
                        break;
                    case "SpecularPow":
                        selected.SpecularPow = sl.Value;
                        break;
                    case "Shine":
                        selected.Shine = sl.Value;
                        break;
                    case "Depth offset":
                        selected.tfDepthOffset = sl.Value;
                        break;
                    case "Test value":
                        selected.AlphaTestVal = sl.Value;
                        break;
                    case "Red":
                        selected.Colour[0] = sl.Value;
                        break;
                    case "Green":
                        selected.Colour[1] = sl.Value;
                        break;
                    case "Blue":
                        selected.Colour[2] = sl.Value;
                        break;
                    case "Alpha":
                        selected.Colour[3] = sl.Value;
                        break;
                }
            }
        }

        public void UpdateCheckBox(object sender, String s)
        {
            if (selected != null)
            {
                SmallCheckbox c = (SmallCheckbox)sender;
                for (int i = 0; i < 14; i++)
                {
                    if (MainMenu.Instance.FindSmallCheckbox(i) == c)
                    {
                        switch (i)
                        {
                            case 0:
                                selected.tfDoubleSided = c.Selected;
                                break;
                            case 1:
                                selected.Sort = c.Selected;
                                break;
                            case 2:
                                selected.Glass = c.Selected;
                                break;
                            case 3:
                                selected.tfWrapX = c.Selected;
                                break;
                            case 4:
                                selected.tfWrapY = c.Selected;
                                break;
                            case 5:
                                selected.tfMinLinear = c.Selected;
                                break;
                            case 6:
                                selected.tfMagLinear = c.Selected;
                                break;
                            case 7:
                                selected.tfBlend = c.Selected;
                                break;
                            case 8:
                                selected.tfBlendAdd = c.Selected;
                                break;
                            case 9:
                                selected.tfNoTexture = c.Selected;
                                break;
                            case 10:
                                selected.tfNoWriteZ = c.Selected;
                                break;
                            case 11:
                                selected.tfTranspBorder = c.Selected;
                                break;
                            case 12:
                                selected.tfTestA = c.Selected;
                                break;
                            case 13:
                                selected.tfTestZ = c.Selected;
                                break;
                        }
                    }
                }
            }
        }

        public static void SelectionChanged(object sender, String s)
        {
            MaterialDisplayTool.Instance.SetMaterial(s);
        }

        public static void SliderValueChanged(object sender, String s)
        {
            MaterialDisplayTool.Instance.UpdateSliderValue(sender, s);
        }

        public static void CheckboxStateChanged(object sender, String s)
        {
            MaterialDisplayTool.Instance.UpdateCheckBox(sender, s);
        }

        public static void SaveLibrary()
        {
            MaterialDisplayTool.Instance.lib.Save(MaterialDisplayTool.Instance.sourceFile);
        }
    }
}
