using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;


//( Class MultiMeshComponent )
//( Type Mesh )
//!!!( Connection LODGroup LODGroup )
//( Connection CollisionMesh Collision )
//( Connection HookListComponent Hooks )
//( Connection AircraftComponent Aircraft )
//( Connection MultiMeshComponent Father )
//( ConnectionList MultiMeshComponent Children )
//( Parameter String Filename )
//( Parameter Bool Hidden )

using GuruEngine.Assets;
using GuruEngine.ECS.Components.World;
using GuruEngine.Rendering;
using GuruEngine.DebugHelpers;
using GuruEngine.Physics.Aircraft;
using GuruEngine.Rendering.RenderCommands;


namespace GuruEngine.ECS.Components.Mesh
{
    public class MultiMeshComponent : ECSGameComponent
    {
        LODGroupComponent lods = null;
        CollisionMeshComponent coll = null;
        public MeshPart mesh = null;
        MeshMaterialLibrary lib = null;
        HookListComponent hooks = null;
        AircraftComponent aircraft = null;
        List<MultiMeshComponent> children = new List<MultiMeshComponent>();
        MultiMeshComponent father = null;
        String meshname;
        int meshGUID;
        public bool Hidden;
        public bool Active = false;

        RenderCommandSet GeometrySet = null;
        RenderCommandSet GlassSet = null;
        RenderCommandSet SortedSet = null;
        RenderCommandSet DoubleSided = null;
        RenderCommandSet Transparent = null;

        public Matrix world;
        public Matrix Animation;

        #region ECS game component interface
        public override ECSGameComponent Clone()
        {
            MultiMeshComponent other = new MultiMeshComponent();
            other.lods = lods;
            other.mesh = mesh;
            other.coll = coll;
            other.hooks = hooks;
            other.lib = lib;
            other.aircraft = aircraft;
            return (ECSGameComponent)other;
        }

        public override void Connect(string components, bool isList)
        {
            string[] parts = components.Split('#');
            if (isList)
            {
                switch (parts[0])
                {
                    case "Children":
                        {
                            string[] mfds = parts[1].Split(',');
                            for (int i = 0; i < mfds.Length; i++)
                            {
                                string[] objs = mfds[i].Split(':');
                                if (objs.Length == 2)
                                {
                                    children.Add((MultiMeshComponent)Parent.FindGameComponentByName(objs[0]));
                                }
                            }
                        }
                        break;
                    default:
                        throw new Exception("GameComponent::FuelTank:: Unknown list connection request to " + parts[0]);
                }
            }
            else
            {
                switch (parts[1])
                {
                    case "Root":
                        {
                            string[] objects = parts[2].Split(':');
                            Parent = GameObjectManager.Instance.FindGameObjectByName(objects[0]);
                        }
                        break;

                    case "LODGroup":
                        {
                            string[] objects = parts[2].Split(':');
                            if (objects[0] != "")
                                lods = (LODGroupComponent)Parent.FindGameComponentByName(objects[0]);
                        }
                        break;
                    case "Collision":
                        {
                            string[] objects = parts[2].Split(':');
                            if (objects[0] != "")
                                coll = (CollisionMeshComponent)Parent.FindGameComponentByName(objects[0]);
                        }
                        break;
                    case "Aircraft":
                        {
                            string[] objects = parts[2].Split(':');
                            if (objects[0] != "")
                                aircraft = (AircraftComponent)Parent.FindGameComponentByName(objects[0]);
                        }
                        break;
                    case "Hooks":
                        {
                            string[] objects = parts[2].Split(':');
                            if (objects[0] != "")
                                hooks = (HookListComponent)Parent.FindGameComponentByName(objects[0]);
                        }
                        break;
                    case "Father":
                        {
                            string[] objects = parts[2].Split(':');
                            if (objects[0] != "")
                                father = (MultiMeshComponent)Parent.FindGameComponentByName(objects[0]);
                        }
                        break;


                    default:
                        throw new Exception("GameComponent::MultiMeshComponent:: Unknown direct connection request to " + parts[1]);
                }
            }
        }

        public override void DisConnect()
        {
            hooks = null;
            lods = null;
            mesh = null;
            coll = null;
            aircraft = null;
            father = null;
            lib = null;
        }

        public override object GetContainedObject(string type)
        {
            switch (type)
            {
                case "CollisionMesh":
                    {
                        return (object)coll;
                    }
                case "LODGroup":
                    {
                        return (object)lods;
                    }
                case "HookListComponent":
                    {
                        return (object)hooks;
                    }
                case "MeshPart":
                    {
                        return (object)mesh;
                    }
                case "AircraftComponent":
                    {
                        return aircraft;
                    }
            }
            return null;
        }

        public override Texture2D GetOffscreenTexture()
        {
            return null;
        }

        public override void HandleEvent(string evt)
        {
        }

        public override void Load(ContentManager content)
        {
            String MeshName = Path.Combine(Parent.FullPath, meshname);
            MeshName += ".meshpart";
            AssetManager.AddMeshPartToQue(MeshName);
            meshGUID = MeshName.GetHashCode();

            String Libname = Path.Combine(Parent.FullPath, meshname);
            Libname += ".materials";
            FileStream readStream = new FileStream(Libname, FileMode.Open);
            BinaryReader readBinary = new BinaryReader(readStream);
            lib = new MeshMaterialLibrary(Path.GetDirectoryName(Libname), readBinary, Parent);
            readBinary.Close();
        }

        public override void ReConnect(GameObject other)
        {
            MultiMeshComponent otherC = (MultiMeshComponent)other.FindGameComponentByName(Name);

            foreach (ECSGameComponent gc in children)
            {
                ECSGameComponent ogc = other.FindGameComponentByName(gc.Name);
                otherC.children.Add((MultiMeshComponent)ogc);
            }
            if (father != null)
                otherC.father = (MultiMeshComponent)other.FindGameComponentByName(father.Name);
            otherC.aircraft = (AircraftComponent)other.FindGameComponentByName(aircraft.Name);
            //otherC.lods = (LODGroupComponent)other.FindGameComponentByName(lods.Name);
            if (coll != null)
                otherC.coll = (CollisionMeshComponent)other.FindGameComponentByName(coll.Name);
            if (hooks != null)
                otherC.hooks = (HookListComponent)other.FindGameComponentByName(hooks.Name);
            otherC.Hidden = Hidden;
            otherC.meshname = meshname;
            otherC.mesh = mesh;
            otherC.Parent = other;
        }

        public override void RenderOffscreenRenderTargets()
        {
        }

        public override void SetParameter(string Name, string Value)
        {
            // The name of the MeshPart binary file
            if (Name == "Filename")
            {
                meshname = Value;
            }
            if (Name == "Hidden")
            {
                Hidden = bool.Parse(Value);
            }
        }

        public override void Update(float dt)
        {
            if (mesh == null)
            {
                mesh = AssetManager.MeshPart(meshGUID);
                if (mesh!=null)
                {
                    world = mesh.startworld;
                    Animation = Matrix.Identity;
                }
            }
            else
            {
                Active = true;
            }
        }

        private void Draw()
        {
            if ((mesh != null) && (lib != null) && (!Hidden))
            {
                if (GeometrySet == null)
                {
                    GeometrySet = new RenderCommandSet();
                    GeometrySet.IsStaticMesh = false;
                    GeometrySet.RS = RasteriserStates.Normal;
                    GeometrySet.DS = DepthStencilState.Default;
                    GeometrySet.RenderPass = RenderPasses.Geometry;

                    GlassSet = new RenderCommandSet();
                    GlassSet.IsStaticMesh = false;
                    GlassSet.RS = RasteriserStates.NormalNoCull;
                    GlassSet.DS = DepthStencilState.DepthRead;
                    GlassSet.RenderPass = RenderPasses.Transparent;

                    SortedSet = new RenderCommandSet();
                    SortedSet.IsStaticMesh = false;
                    SortedSet.RS = RasteriserStates.Normal;
                    SortedSet.DS = DepthStencilState.Default;
                    SortedSet.RenderPass = RenderPasses.SortedGeometry;

                    DoubleSided = new RenderCommandSet();
                    DoubleSided.IsStaticMesh = false;
                    DoubleSided.RS = RasteriserStates.NormalNoCull;
                    DoubleSided.DS = DepthStencilState.Default;
                    DoubleSided.RenderPass = RenderPasses.SortedGeometry;

                    Transparent = new RenderCommandSet();
                    Transparent.IsStaticMesh = false;
                    Transparent.RS = RasteriserStates.CullCounterclockwise;
                    Transparent.DS = DepthStencilState.Default;
                    Transparent.RenderPass = RenderPasses.Overlays;

                    for (int i = 0; i < mesh.facegroups.Length; i++)
                    {
                        RenderMeshPart r = new RenderMeshPart();
                        switch (r.Setup(this, lib, mesh.facegroups[i]))
                        {
                            case 0:
                                GlassSet.Commands.Add(r);
                                break;
                            case 1:
                                SortedSet.Commands.Add(r);
                                break;
                            case 2:
                                GeometrySet.Commands.Add(r);
                                break;
                            case 3:
                                DoubleSided.Commands.Add(r);
                                break;
                            case 4:
                                Transparent.Commands.Add(r);
                                break;
                        }
                    }
                }
                foreach (RenderCommand r in GeometrySet.Commands)
                {
                    CopyMatrix(ref r.World, ref world);
                }
                foreach (RenderCommand r in GlassSet.Commands)
                {
                    CopyMatrix(ref r.World, ref world);
                }
                foreach (RenderCommand r in SortedSet.Commands)
                {
                    CopyMatrix(ref r.World, ref world);
                }
                foreach (RenderCommand r in DoubleSided.Commands)
                {
                    CopyMatrix(ref r.World, ref world);
                }
                foreach (RenderCommand r in Transparent.Commands)
                {
                    CopyMatrix(ref r.World, ref world);
                }
                Renderer.AddRenderCommand(GeometrySet);
                Renderer.AddRenderCommand(GlassSet);
                Renderer.AddRenderCommand(SortedSet);
                Renderer.AddRenderCommand(DoubleSided);
                Renderer.AddRenderCommand(Transparent);

                if (coll != null)
                    coll.SetMatrix(world);
            }
#if DEBUG
            if (DebugRenderSettings.RenderHooks)
            {
                if (hooks != null)
                {
                    if (hooks.Hooks.Count > 0)
                    {
                        foreach (String s in hooks.Hooks.Keys)
                        {
                            Hook h = hooks.Hooks[s];
                            //System.Console.WriteLine(h.Name);
                            if (h.Name == "__MGUN01_<BASE>")
                            {
                                Matrix mn = h.Transform * world;
                                DebugLineDraw.DrawLine(mn.Translation, mn.Translation + (mn.Forward * 10), Color.Crimson);
                                DebugLineDraw.DrawLine(mn.Translation, mn.Translation + (mn.Up * 10), Color.Green);
                            }
                        }
                    }
                }
            }
#endif

        }

        #endregion

        #region Public methods
        public void UpdateMatrices(Matrix mod)
        {
            if (mesh != null)
            {
                Animate(mod);

                Draw();
                foreach (MultiMeshComponent m in children)
                {
                    m.UpdateMatrices(world);
                }
            }
        }
        public void MatrixAnimate(Matrix m)
        {
            if (mesh != null)
            {
                ModifyMatrix(m);
            }
        }

        public Matrix GetWorldMatrix()
        {
            return world;
        }

        public Matrix GetOriginalWorldMatrix()
        {
            return mesh.startworld;
        }

        public bool IsActive()
        {
            return Active;
        }



        public Matrix GetOriginalHookMatrix(String id)
        {
            Hook h = hooks.FindHook(id);
            return h.Transform;
        }

        public Matrix GetHookMatrix(String id)
        {
            Hook h = hooks.FindHook(id);
            return h.Transform * world;
        }

        public bool HasHook(String id)
        {
            if (hooks != null)
                return (hooks.HasHook(id));
            return false;
        }

        public void Animate(Matrix mod)
        {
            world = Animation * mesh.startworld * mod;
        }

        public void ModifyMatrix(Matrix m)
        {
            Animation = m;
        }
        #endregion


    }
}
