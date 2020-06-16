using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.IO;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

using GuruEngine.Text;
using GuruEngine.Rendering;
using GuruEngine.DebugHelpers;
using GuruEngine.Rendering.Particles;

namespace GuruEngine.Assets
{
    public class MutableHolder<T>
    {
        public T Value;
        public MutableHolder(T value)
        {
            this.Value = value;
        }
    }

    public class AssetManager : ContentManager
    {
        public static AssetManager Instance;

        Texture2D defaultTexture;
        public SpriteFont debugFont;
        public Texture2D white;
        public Texture2D black;
        public Texture2D gaze;
        public Texture2D single_pixel;
        public TextureCube environment;

        /// <summary>
        /// Thread on which asyncronous loading will occur
        /// </summary>
        Thread loadThread;

        /// <summary>
        /// Reset event so load thread can wait once the queue is empty
        /// </summary>
        AutoResetEvent loadResetEvent;

        /// <summary>
        ///  A flag to request the thread to return from its loop
        /// </summary>
        private volatile bool mCloseRequested;

        Queue<AssetRecord> loadItemsQueue = new Queue<AssetRecord>();
        Dictionary<int, MutableHolder<AssetRecord>> loadedAssets = new Dictionary<int, MutableHolder<AssetRecord>>();

        #region Constructor
        public AssetManager(GraphicsDevice device, IServiceProvider serviceProvider, string rootDirectory)
            : base(serviceProvider, rootDirectory)
        {
            Instance = this;

            Color[] pixels = new Color[4];
            pixels[0] = Color.Purple;
            pixels[1] = Color.Purple;
            pixels[2] = Color.Purple;
            pixels[3] = Color.Purple;

            defaultTexture = new Texture2D(device, 2, 2);
            defaultTexture.SetData<Color>(pixels);

            pixels[0] = Color.White;
            pixels[1] = Color.White;
            pixels[2] = Color.White;
            pixels[3] = Color.White;

            white = new Texture2D(device, 2, 2);
            white.SetData<Color>(pixels);

           

            pixels[0] = Color.Black;
            pixels[1] = Color.Black;
            pixels[2] = Color.Black;
            pixels[3] = Color.Black;

            black = new Texture2D(device, 2, 2);
            black.SetData<Color>(pixels);

            debugFont = Load<SpriteFont>(@"Fonts\DebugFont");
            gaze = Load<Texture2D>(@"Textures\gaze");

            single_pixel = new Texture2D(device, 1, 1);
            pixels = new Color[1];
            pixels[0] = Color.Black;
            single_pixel.SetData<Color>(pixels);

            environment = new TextureCube(device, 256, false, SurfaceFormat.Color);

            // First time LoadAsync has been called so 
            // initialise thread, reset event and queue
            loadThread = new Thread(new ThreadStart(LoadingThreadWorker));
            loadThread.Name = "File Loading Worker";

            loadResetEvent = new AutoResetEvent(false);

            //reset the request flag to close the thread
            mCloseRequested = false;

            // Start thread. It will wait once queue is empty
            loadThread.Start();
        }
        #endregion

        #region Add asset to manager
        public void AddAsset(String name, AssetRecordType type)
        {
#if PROFILE
            Profiler.Start("Asset adding");
#endif
            int quid = name.GetHashCode();
            if (loadedAssets.ContainsKey(quid))
            {
                loadedAssets[quid].Value.ReferenceCount++;
                return;
            }

            AssetRecord ar = new AssetRecord();
            ar.Asset = null;
            ar.Path = name;
            ar.ReferenceCount = 1;
            ar.Type = type;

            lock (loadItemsQueue)
            {
                loadItemsQueue.Enqueue(ar);
            }

            // Tell loading thread to stop waiting
            loadResetEvent.Set();
#if PROFILE
            Profiler.End("Asset adding");
#endif
        }

        public void AddTexture(int ID, Texture2D asset)
        {
            
            AssetRecord ar = new AssetRecord();
            ar.Asset = asset;
            ar.Type = AssetRecordType.Texture;
            MutableHolder<AssetRecord> mar = new MutableHolder<AssetRecord>(ar);
            loadedAssets.Add(ID, mar);
        }
        #endregion

        #region Getters
        public Texture2D GetTexture(int id)
        {
            if (loadedAssets.ContainsKey(id))
            {
                return loadedAssets[id].Value.Asset as Texture2D;
            }
            return defaultTexture;
        }

        public ParticleSettings GetPSettings(int id)
        {
            if (loadedAssets.ContainsKey(id))
            {
                return loadedAssets[id].Value.Asset as ParticleSettings;
            }
            return null;
        }

        public MeshPart GetMeshPart(int id)
        {
            if (loadedAssets.ContainsKey(id))
            {
                return loadedAssets[id].Value.Asset as MeshPart;
            }
            return null;
        }

        public MeshMaterialLibrary GetMeshMaterialLibrary(int id)
        {
            if (loadedAssets.ContainsKey(id))
            {
                return loadedAssets[id].Value.Asset as MeshMaterialLibrary;
            }
            return null;
        }

        public Model GetStaticMesh(int id)
        {
            if (loadedAssets.ContainsKey(id))
            {
                return loadedAssets[id].Value.Asset as Model;
            }
            return null;
        }

        public Effect GetShader(int id)
        {
            if (loadedAssets.ContainsKey(id))
            {
                return loadedAssets[id].Value.Asset as Effect;
            }
            return null;
        }

        public Effect CloneEffect(int id)
        {
            if (loadedAssets.ContainsKey(id))
            {
                return (loadedAssets[id].Value.Asset as Effect).Clone();
            }
            return null;
        }

        public SDFFont GetSDF(int id)
        {
            if (loadedAssets.ContainsKey(id))
            {
                return loadedAssets[id].Value.Asset as SDFFont;
            }
            return null;
        }

        public MSDFFont GetMSDF(int id)
        {
            if (loadedAssets.ContainsKey(id))
            {
                return loadedAssets[id].Value.Asset as MSDFFont;
            }
            return null;
        }

        public SoundEffect GetNoise(int id)
        {
            if (loadedAssets.ContainsKey(id))
            {
                return loadedAssets[id].Value.Asset as SoundEffect;
            }
            return null;
        }
        #endregion

        #region Disposal
        public void Shutdown()
        {

            try
            {
                mCloseRequested = true;
                loadResetEvent.Set();
                loadThread.Join();
                loadThread = null;
            }
            catch { }

            DisposeAll();

        }

        public void DisposeAll()
        { 
            foreach (int g in loadedAssets.Keys)
            {
                AssetRecord ar = loadedAssets[g].Value;
                Dispose(ar);
            }
            LoadedAssets.Clear();
        }

        public void RemoveParticleSettings(int id)
        {
            if (loadedAssets.ContainsKey(id))
            {
                loadedAssets[id].Value.ReferenceCount--;
                if (loadedAssets[id].Value.ReferenceCount == 0)
                {
                    MutableHolder<AssetRecord> ar = loadedAssets[id];
                    loadedAssets.Remove(id);
                    Dispose(ar.Value);

                }
            }
        }

        public void RemoveTexture(int id)
        {
            if (loadedAssets.ContainsKey(id))
            {
                loadedAssets[id].Value.ReferenceCount--;
                if (loadedAssets[id].Value.ReferenceCount == 0)
                {
                    MutableHolder<AssetRecord> ar = loadedAssets[id];
                    loadedAssets.Remove(id);
                    Dispose(ar.Value);

                }
            }
        }

        public void RemoveSDF(int id)
        {
            if (loadedAssets.ContainsKey(id))
            {
                loadedAssets[id].Value.ReferenceCount--;
                if (loadedAssets[id].Value.ReferenceCount == 0)
                {
                    MutableHolder<AssetRecord> ar = loadedAssets[id];
                    loadedAssets.Remove(id);
                    Dispose(ar.Value);

                }
            }
        }

        public void RemoveMSDF(int id)
        {
            if (loadedAssets.ContainsKey(id))
            {
                loadedAssets[id].Value.ReferenceCount--;
                if (loadedAssets[id].Value.ReferenceCount == 0)
                {
                    MutableHolder<AssetRecord> ar = loadedAssets[id];
                    loadedAssets.Remove(id);
                    Dispose(ar.Value);

                }
            }
        }

        public void RemoveNoise(int id)
        {
            if (loadedAssets.ContainsKey(id))
            {
                loadedAssets[id].Value.ReferenceCount--;
                if (loadedAssets[id].Value.ReferenceCount == 0)
                {
                    MutableHolder<AssetRecord> ar = loadedAssets[id];
                    loadedAssets.Remove(id);
                    Dispose(ar.Value);
                }
            }
        }

        private void Dispose(AssetRecord ar)
        {
            switch (ar.Type)
            {
                case AssetRecordType.Texture:
                    {
                        Texture2D asset = ar.Asset as Texture2D;
                        asset.Dispose();
                    }
                    break;
                case AssetRecordType.Shader:
                    {
                        Effect asset = ar.Asset as Effect;
                        asset.Dispose();
                    }
                    break;
            }
        }
        #endregion

        #region Debug
        public int AssetsLoaded()
        {
            return loadedAssets.Keys.Count;
        }

        public int TexturesLoaded()
        {
            int res = 0;
            foreach (int s in loadedAssets.Keys)
            {
                if (loadedAssets[s].Value.Type == AssetRecordType.Texture)
                    res++;
            }
            return res;
        }

        public int GetReferenceCount(String name)
        {
            int guid = name.GetHashCode();
            if (loadedAssets.ContainsKey(guid))
            {
                return loadedAssets[guid].Value.ReferenceCount;
            }
            return 0;
        }
        #endregion

        #region Async loading

        /// <summary>
        /// Consume the Queue of assets to be loaded then wait 
        /// </summary>
        void LoadingThreadWorker()
        {
            AssetRecord args;

            while (!mCloseRequested)
            {
#if PROFILE
                Profiler.Start("Asset loading thread");
#endif
                while (loadItemsQueue.Count > 0)
                {

                    // Get next item to process
                    lock (loadItemsQueue)
                    {
                        args = loadItemsQueue.Peek();
                    }

                    // Process head queue entry
                    CallGenericLoad(args);

                    // Remove processed item. Can't be removed until
                    // loading complete as new async requests may need
                    // to add AssetLoaded methods to it's list
                    lock (loadItemsQueue)
                    {
                        loadItemsQueue.Dequeue();
                    }
                }
#if PROFILE
                Profiler.End("Asset loading thread");
#endif
                // Wait until next load call
                loadResetEvent.WaitOne();
            }
        }

        /// <summary>
        /// Calls the private Load<T>(string, AssetTracker) method.
        /// Reflection is needed to use an arbitrary generic type parameter
        /// </summary>
        /// <param name="loadArgs"></param>
        void CallGenericLoad(AssetRecord loadArgs)
        {
           switch (loadArgs.Type)
           {
                case AssetRecordType.SoundEffect:
                    {
                        int GUID = loadArgs.Path.GetHashCode();
                        if (!loadedAssets.ContainsKey(GUID))
                        {
                            try
                            {
                                Stream read = new FileStream(loadArgs.Path, FileMode.Open);
                                SoundEffect t = SoundEffect.FromStream(read);
                                read.Close();
                                loadArgs.Asset = t;
                            }
                            catch (Exception)
                            {
                                LogHelper.Instance.Error("Missing sound effect " + loadArgs.Path);
                            }
                            MutableHolder<AssetRecord> ar = new MutableHolder<AssetRecord>(loadArgs);
                            lock (loadedAssets)
                            {
                                loadedAssets.Add(loadArgs.Path.GetHashCode(), ar);
                            }
                        }
                    }
                    break;

                case AssetRecordType.Particlesettings:
                    {
                        int GUID = loadArgs.Path.GetHashCode();
                        if (!loadedAssets.ContainsKey(GUID))
                        {
                            ParticleSettings ps = new ParticleSettings();
                            ps.Load(loadArgs.Path);
                            loadArgs.Asset = ps;
                            MutableHolder<AssetRecord> ar = new MutableHolder<AssetRecord>(loadArgs);
                            lock (loadedAssets)
                            {
                                loadedAssets.Add(loadArgs.Path.GetHashCode(), ar);
                            }
                        }
                    }
                    break;
                case AssetRecordType.MSDFFont:
                    {
                        MSDFFont font = new MSDFFont(loadArgs.Path);
                        loadArgs.Asset = font;
                        MutableHolder<AssetRecord> ar = new MutableHolder<AssetRecord>(loadArgs);
                        lock (loadedAssets)
                        {
                            loadedAssets.Add(loadArgs.Path.GetHashCode(), ar);
                        }
                    }
                    break;
                case AssetRecordType.SDFFont:
                    {
                        SDFFont font = new SDFFont(loadArgs.Path);
                        loadArgs.Asset = font;
                        MutableHolder<AssetRecord> ar = new MutableHolder<AssetRecord>(loadArgs);
                        lock (loadedAssets)
                        {
                            loadedAssets.Add(loadArgs.Path.GetHashCode(), ar);
                        }
                    }
                    break;
                case AssetRecordType.Shader:
                    {
                        Effect fx = Load<Effect>(loadArgs.Path);
                        loadArgs.Asset = fx;
                        MutableHolder<AssetRecord> ar = new MutableHolder<AssetRecord>(loadArgs);
                        lock (loadedAssets)
                        {
                            loadedAssets.Add(loadArgs.Path.GetHashCode(), ar);
                        }
                        Renderer.GetCurrentRenderer().AddShader(loadArgs.Path, fx);
                    }
                    break;

                case AssetRecordType.StaticMesh:
                    {
                        Model mesh = Load<Model>(loadArgs.Path);
                        loadArgs.Asset = mesh;
                        MutableHolder<AssetRecord> ar = new MutableHolder<AssetRecord>(loadArgs);
                        lock (loadedAssets)
                        {
                            loadedAssets.Add(loadArgs.Path.GetHashCode(), ar);
                        }
                    }
                    break;
                case AssetRecordType.Texture:
                    {
                        int GUID = loadArgs.Path.GetHashCode();
                        if (!loadedAssets.ContainsKey(GUID))
                        {
                            Texture2D tex = null;
                            try {
                                tex = Load<Texture2D>(loadArgs.Path);
                            } catch (Exception)
                            {
                                try
                                {
                                    Stream read = new FileStream(loadArgs.Path, FileMode.Open);
                                    tex = Texture2D.FromStream(Renderer.GetGraphicsDevice(), read);
                                    read.Close();
                                }
                                catch (Exception)
                                {
                                    tex = defaultTexture;
                                    LogHelper.Instance.Error("Missing texture " + loadArgs.Path);
                                }
                            }

                            loadArgs.Asset = tex;
                            MutableHolder<AssetRecord> ar = new MutableHolder<AssetRecord>(loadArgs);
                            lock (loadedAssets)
                            {
                                loadedAssets.Add(GUID, ar);
                            }
                        }
                    }
                    break;
                case AssetRecordType.MeshPart:
                    {
                        if (!loadedAssets.ContainsKey(loadArgs.Path.GetHashCode()))
                        {
                            FileStream readStream = new FileStream(loadArgs.Path, FileMode.Open);
                            BinaryReader readBinary = new BinaryReader(readStream);
                            MeshPart mp = new MeshPart(readBinary);
                            mp.ID = loadArgs.Path.GetHashCode();

                            readBinary.Close();
                            loadArgs.Asset = mp;
                            MutableHolder<AssetRecord> ar = new MutableHolder<AssetRecord>(loadArgs);
                            lock (loadedAssets)
                            {
                                loadedAssets.Add(loadArgs.Path.GetHashCode(), ar);
                            }
                        }
                    }
                    break;
                case AssetRecordType.MeshPartMaterialLibrary:
                    {
                        if (!loadedAssets.ContainsKey(loadArgs.Path.GetHashCode()))
                        {
                            FileStream readStream = new FileStream(loadArgs.Path, FileMode.Open);
                            BinaryReader readBinary = new BinaryReader(readStream);
                            MeshMaterialLibrary mp = new MeshMaterialLibrary(Path.GetDirectoryName(loadArgs.Path), readBinary);

                            readBinary.Close();
                            loadArgs.Asset = mp;
                            MutableHolder<AssetRecord> ar = new MutableHolder<AssetRecord>(loadArgs);
                            lock (loadedAssets)
                            {
                                loadedAssets.Add(loadArgs.Path.GetHashCode(), ar);
                            }
                        }
                    }
                    break;
           }
        }

        #endregion

        #region Static methods
        public static Texture2D Texture(int id)
        {
            return AssetManager.Instance.GetTexture(id);
        }

        public static bool IsReady(int id)
        {
            return AssetManager.Instance.loadedAssets.ContainsKey(id);
        }
        public static ParticleSettings GetParticleSettings(int id)
        {
            return AssetManager.Instance.GetPSettings(id);
        }
        public static MeshPart MeshPart(int id)
        {
            return AssetManager.Instance.GetMeshPart(id);
        }
        public static MeshMaterialLibrary MeshMaterialLibrary(int id)
        {
            return AssetManager.Instance.GetMeshMaterialLibrary(id);
        }
        public static Model StaticMesh(int id)
        {
            return AssetManager.Instance.GetStaticMesh(id);
        }
        public static Effect Shader(int id)
        {
            return AssetManager.Instance.GetShader(id);
        }
        public static Texture2D GetWhite()
        {
            return AssetManager.Instance.white;
        }
        public static Texture2D GetSinglePixel()
        {
            return AssetManager.Instance.single_pixel;
        }
        public static Texture2D GetBlack()
        {
            return AssetManager.Instance.black;
        }
        public static SpriteFont GetDebugFont()
        {
            return AssetManager.Instance.debugFont;
        }
        public static SDFFont GetFont(int id)
        {
            return AssetManager.Instance.GetSDF(id);
        }
        public static MSDFFont GetMFont(int id)
        {
            return AssetManager.Instance.GetMSDF(id);
        }
        public static Effect GetCloneOfEffect(int id)
        {
            return AssetManager.Instance.CloneEffect(id);
        }

        public static SoundEffect GetSoundEffect(int id)
        {
            return AssetManager.Instance.GetNoise(id);
        }

        public static void AddTextureToQue(String name)
        {
            AssetManager.Instance.AddAsset(name, AssetRecordType.Texture);
        }
        public static void AddStaticMeshToQue(String name)
        {
            AssetManager.Instance.AddAsset(name, AssetRecordType.StaticMesh);
        }
        public static void AddShaderToQue(String name)
        {
            AssetManager.Instance.AddAsset(name, AssetRecordType.Shader);
        }       
        public static void AddMeshPartToQue(String name)
        {
            AssetManager.Instance.AddAsset(name, AssetRecordType.MeshPart);
        }
        public static void AddMeshMaterialLibraryToQue(String name)
        {
            AssetManager.Instance.AddAsset(name, AssetRecordType.MeshPartMaterialLibrary);
        }
        public static void AddTextureToManager(int ID, Texture2D asset )
        {
            AssetManager.Instance.AddTexture(ID, asset);
        }
        public static void AddFontToQue(String name)
        {
            AssetManager.Instance.AddAsset(name, AssetRecordType.SDFFont);
        }
        public static void AddMFontToQue(String name)
        {
            AssetManager.Instance.AddAsset(name, AssetRecordType.MSDFFont);
        }
        public static void AddParticleSystemToQue(String name)
        {
            AssetManager.Instance.AddAsset(name, AssetRecordType.Particlesettings);
        }

        public static void AddSoundEffectToQue(String name)
        {
            AssetManager.Instance.AddAsset(name, AssetRecordType.SoundEffect);
        }

        public static void RemoveParticleSettingsReference(int ID)
        {
            AssetManager.Instance.RemoveParticleSettings(ID);
        }
        public static void RemoveTextureReference(int ID)
        {
            AssetManager.Instance.RemoveTexture(ID);
        }
        public static void RemoveFontReference(int ID)
        {
            AssetManager.Instance.RemoveSDF(ID);
        }
        public static void RemoveMFontReference(int ID)
        {
            AssetManager.Instance.RemoveMSDF(ID);
        }
        public static void RemoveSoundEffectReference(int ID)
        {
            AssetManager.Instance.RemoveNoise(ID);
        }

        #endregion
    }
}
