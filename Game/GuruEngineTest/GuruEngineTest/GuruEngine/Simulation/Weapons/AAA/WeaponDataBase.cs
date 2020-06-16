using System;
using System.Collections.Generic;
using System.IO;
using GuruEngine.Core;
using Microsoft.Xna.Framework;

namespace GuruEngine.Simulation.Weapons.AAA
{
    public class WeaponDataBase
    {
        public Dictionary<int, WeaponAAA> AAAWeapons = new Dictionary<int, WeaponAAA>();
        public static WeaponDataBase Instance;

        public WeaponDataBase()
        {
            Instance = this;
            
        }

        public static WeaponAAA GetAAAWeapon(int id)
        {
            if (Instance.AAAWeapons.ContainsKey(id))
                return Instance.AAAWeapons[id];

            throw new Exception("WeaponDataBase::Unknown key");
        }

        public static void Load(String name)
        {
            if ( (name.StartsWith("AAA")) && (Instance.AAAWeapons.ContainsKey(name.GetHashCode()))) return;

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
                if (name.StartsWith("AAA"))
                {
                    WeaponAAA w = new WeaponAAA();
                    w.AimMaxDistance = float.Parse(parts[0]);
                    w.FireDelay = float.Parse(parts[1]);
                    w.MuzzleVelocity = float.Parse(parts[2]);
                    w.Bullets = int.Parse(parts[3]);
                    w.Sound = parts[4].GetHashCode();
                    w.Round = parts[5].GetHashCode();
                    Instance.AAAWeapons.Add(name.GetHashCode(), w);
                }

            }
            else
            {
                throw new Exception("WeaponDatabase::Missing gun descriptor " + name);
            }
        }
    }
}
