using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework;

using GuruEngine.Assets;

namespace GuruEngine.Audio
{
    public class AudioManager
    {
        public static AudioManager Instance;
        public float MusicVolume = 1;
        public float SFXVolume = 1;
        public ContentManager content;
        public AudioListener listener;

        Song[] songs;

        public Dictionary<String, SoundEffect> effects = new Dictionary<string, SoundEffect>();
        Dictionary<int, SoundEffect> dynamic_effects = new Dictionary<int, SoundEffect>();
        Dictionary<int, ActiveSoundEffect3D> active_effects = new Dictionary<int, ActiveSoundEffect3D>();

        List<int> pending = new List<int>();

        public AudioManager(ContentManager manager)
        {
            Instance = this;
            songs = new Song[1];
            songs[0] = manager.Load<Song>(@"Music\music1");
            content = manager;
            listener = new AudioListener();
        }

        public void Update()
        {
            for (int i = pending.Count - 1; i >= 0; i--)
            {
                SoundEffect sf = AssetManager.GetSoundEffect(pending[i]);
                if (sf !=null)
                {
                    dynamic_effects.Add(pending[i], sf);
                    pending.RemoveAt(i);
                }
            }
        }

        public void PlayMusic(int ID)
        {
            MediaPlayer.Play(songs[ID]);
            MediaPlayer.IsRepeating = true;
        }

        public static void SetMusicVolume(float val)
        {
            val = Math.Min(val, 1);
            val = Math.Max(val, 0);

            Instance.MusicVolume = val;
            MediaPlayer.Volume = val;
        }

        public static void SetSfxVolume(float val)
        {
            val = Math.Min(val, 1);
            val = Math.Max(val, 0);

            Instance.SFXVolume = val;
            SoundEffect.MasterVolume = val;
        }

        public static float GetMusicVolume()
        {
            return Instance.MusicVolume;
        }

        public static float GetSFXVolume()
        {
            return Instance.SFXVolume;
        }

        public static void PlaySong(int id)
        {
            Instance.PlayMusic(id);
        }

        public static void RegisterSoundEffect(String s, String t)
        {
            SoundEffect sfx = Instance.content.Load<SoundEffect>(t);
            Instance.effects.Add(s, sfx);
            
        }

        public static void PlaySFXOnce(String s)
        {
            Instance.effects[s].Play();
        }

        #region Add dynamic noises
        public int AddSFX(String relative)
        {
            String path = FilePaths.DataPath + "SoundEffectPacks\\" + relative;

            int ID = path.GetHashCode();
            if ((pending.Contains(ID)) || (dynamic_effects.ContainsKey(ID)))
                return ID;

            AssetManager.AddSoundEffectToQue(path);
            pending.Add(ID);
            return ID;
        }

        /// <summary>
        /// Throw a sound effect at a postion, let C# deal with garbage collection
        /// </summary>
        /// <param name="ID"></param>
        /// <param name="position"></param>
        public void PlaySoundOnceAt(int ID, Vector3 position)
        {
            if (dynamic_effects.ContainsKey(ID))
            {
                SoundEffectInstance sfi = dynamic_effects[ID].CreateInstance();
                sfi.IsLooped = false;
                AudioEmitter em = new AudioEmitter();
                em.Position = position;
                sfi.Apply3D(listener, em);
                sfi.Play();
            }
        }
        #endregion

        #region Static methods
        public static int AddSoundEffect(String relativepath)
        {
            return Instance.AddSFX(relativepath);
        }

        public static void PlayDynamicEffectOnceAt(int ID, Vector3 pos)
        {
            Instance.PlaySoundOnceAt(ID, pos);
        }

        public static void MoveListener(Matrix pos)
        {
            Instance.listener.Position = pos.Translation;
            Instance.listener.Up = pos.Up;
            Instance.listener.Forward = pos.Forward;
        }
        #endregion
    }
}
