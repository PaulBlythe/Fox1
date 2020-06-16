using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

using GuruEngine.Localization;
using GuruEngine.Audio;

namespace GuruEngine
{
    public class Settings
    {
        #region Setup
        static Settings instance;

        public static Settings GetInstance()
        {
            return instance;
        }

        public Settings()
        {
            instance = this;
            Default();
            Load();
        }

        public void Default()
        {
            DisplayHeight = 1080;
            DisplayWidth = 2560;
            FullScreen = false;
            GameObjectDirectory = FilePaths.DataPath;
            GameRootDirectory = FilePaths.DataPath; 
            PaintSchemeDirectory = FilePaths.DataPath + @"/PaintSchemes/";
            Language = SupportedLanguages.English;
        }
        #endregion

        #region Variables
        private const String SettingsFile = "Settings.txt";
        public const int Version = 8;
        public int DisplayWidth;
        public int DisplayHeight;
        public bool FullScreen;
        public String GameObjectDirectory;
        public String GameRootDirectory;
        public static String PaintSchemeDirectory;
        public static SupportedLanguages Language;
        

        /// <summary>
        /// 0
        /// 1
        /// 2
        /// 3
        /// 4
        /// 5   Radar starts to use tranmission power
        /// 6
        /// 7
        /// 8
        /// 9
        /// 10
        /// </summary>
        public int Difficulty = 10;


        #endregion

        #region Serialisation
        public void Save()
        {
            String settingsfile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, SettingsFile);
            using (TextWriter writer = File.CreateText(settingsfile))
            {
                writer.WriteLine(String.Format("{0}", Version));

                writer.WriteLine(AudioManager.GetMusicVolume().ToString());
                writer.WriteLine(AudioManager.GetSFXVolume().ToString());
                string s = Enum.GetName(typeof(SupportedLanguages), Language);
                writer.WriteLine(s);
                writer.WriteLine(PaintSchemeDirectory);
                writer.WriteLine(String.Format("{0}", Difficulty));
                writer.WriteLine(GameRootDirectory);
                writer.WriteLine(GameObjectDirectory);
                writer.WriteLine(String.Format("{0}", FullScreen));
                writer.WriteLine(String.Format("{0}", DisplayWidth));
                writer.WriteLine(String.Format("{0}", DisplayHeight));
               
            }
        }

        public void Load()
        {
            String settingsfile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, SettingsFile);
            if (File.Exists(settingsfile))
            {
                try
                {
                    using (TextReader reader = File.OpenText(settingsfile))
                    {
                        String line = reader.ReadLine();
                        int version = int.Parse(line);
                        switch (version)
                        {
                            case 8:
                                {
                                    AudioManager.SetMusicVolume(float.Parse(reader.ReadLine()));
                                    AudioManager.SetSfxVolume(float.Parse(reader.ReadLine()));
                                }
                                goto case 7;
                            case 7:
                                {
                                    Language = (SupportedLanguages)Enum.Parse(typeof(SupportedLanguages), reader.ReadLine());
                                }
                                goto case 6;
                            case 6:
                                {
                                    PaintSchemeDirectory = reader.ReadLine();
                                }
                                goto case 5;
                            case 5:
                                {
                                    Difficulty = int.Parse(reader.ReadLine());
                                }
                                goto case 4;
                            case 4:
                                {
                                    GameRootDirectory = reader.ReadLine();
                                }
                                goto case 3;
                            case 3:
                                {
                                    GameObjectDirectory = reader.ReadLine();
                                }
                                goto case 2;
                            case 2:
                                {
                                    line = reader.ReadLine();
                                    FullScreen = bool.Parse(line);
                                }
                                goto case 1;
                            case 1:
                                {
                                    line = reader.ReadLine();
                                    DisplayWidth = int.Parse(line);
                                    line = reader.ReadLine();
                                    DisplayHeight = int.Parse(line);
                                }
                                break;

                            default:
                                {
                                    
                                }
                                break;
                        }
                    }
                    //RenderSettings.Instance.FullScreen = new Microsoft.Xna.Framework.Rectangle(0, 0, DisplayWidth, DisplayHeight);
                }catch (Exception e)
                {
                    throw new Exception("Error reading settings file :-" + e);
                }
            }else
            {
                Default();
            }
        }
        #endregion

    }
}
