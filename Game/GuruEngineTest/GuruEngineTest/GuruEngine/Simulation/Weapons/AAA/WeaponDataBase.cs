using System;
using System.Collections.Generic;
using System.IO;
using GuruEngine.Core;
using Microsoft.Xna.Framework;
using GuruEngine.Audio;

namespace GuruEngine.Simulation.Weapons.AAA
{
    public class WeaponDataBase
    {
        public Dictionary<int, WeaponAAA> AAAWeapons = new Dictionary<int, WeaponAAA>();
        public Dictionary<int, WeaponArtillery> ArtilleryWeapons = new Dictionary<int, WeaponArtillery>();

        public static WeaponDataBase Instance;

        public WeaponDataBase()
        {
            Instance = this;
            
        }

        public static WeaponArtillery GetArtilleryWeapon(int id)
        {
            if (Instance.ArtilleryWeapons.ContainsKey(id))
                return Instance.ArtilleryWeapons[id];
            return null;
        }

        public static WeaponAAA GetAAAWeapon(int id)
        {
            if (Instance.AAAWeapons.ContainsKey(id))
                return Instance.AAAWeapons[id];

            throw new Exception("WeaponDataBase::Unknown key");
        }

        public static void Load(String name)
        {
            if (Instance.AAAWeapons.ContainsKey(name.GetHashCode())) return;
            if (Instance.ArtilleryWeapons.ContainsKey(name.GetHashCode())) return;

            String basepath = Settings.GetInstance().GameObjectDirectory;
            basepath += @"\Weapons\";
            basepath += name;
            basepath += ".txt";

            if (File.Exists(basepath))
            {
                TextReader readFile = new StreamReader(basepath);
                String l = readFile.ReadLine();
                readFile.Close();

                string[] parts = l.Split(',');
                switch (parts[0])
                {
                    case "ARTILLERY":
                        {
                            WeaponArtillery w = new WeaponArtillery();
                            w.AimMaxDistance = float.Parse(parts[1]);
                            w.FireDelay = float.Parse(parts[2]);
                            w.MuzzleVelocity = float.Parse(parts[3]);
                            w.Bullets = int.Parse(parts[4]);
                            w.Round = parts[6].GetHashCode();
                            w.Sound = AudioManager.AddSoundEffect(parts[5]);

                            Instance.ArtilleryWeapons.Add(name.GetHashCode(), w);
                        }
                        break;
                    case "AAA":
                    {
                        WeaponAAA w = new WeaponAAA();
                        w.AimMaxDistance = float.Parse(parts[1]);
                        w.FireDelay = float.Parse(parts[2]);
                        w.MuzzleVelocity = float.Parse(parts[3]);
                        w.Bullets = int.Parse(parts[4]);
                        w.Round = parts[6].GetHashCode();
                        w.Sound = AudioManager.AddSoundEffect(parts[5]);

                        Instance.AAAWeapons.Add(name.GetHashCode(), w);
                       
                    }
                    break;
                 }
            }
            else
            {
                throw new Exception("WeaponDatabase::Missing gun descriptor " + name);
            }
        }
    }
}
