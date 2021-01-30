using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using GuruEngine.Core;

namespace GuruEngine.Simulation.Weapons.Ammunition
{
    public class AmmunitionDatabase
    {
        public static AmmunitionDatabase Instance;
        public Dictionary<int, AAARound> AAARounds = new Dictionary<int, AAARound>();
        public Dictionary<int, ArtilleryRound> ArtilleryRounds = new Dictionary<int, ArtilleryRound>();

        public AmmunitionDatabase()
        {
            Instance = this;
        }

        public static void Load(String name)
        {
            if (Instance.AAARounds.ContainsKey(name.GetHashCode()))
                return;

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
                            ArtilleryRound w = new ArtilleryRound();
                            w.LiveTime = float.Parse(parts[1]);
                            w.Mass = float.Parse(parts[2]);
                            w.Power = float.Parse(parts[3]);
                            w.Caliber = float.Parse(parts[4]);
                            w.Fused = bool.Parse(parts[5]);
                            w.FuseType = int.Parse(parts[6]);
                            w.Explodes = bool.Parse(parts[7]);
                            w.ExplosionRadius = float.Parse(parts[8]);

                            Instance.ArtilleryRounds.Add(name.GetHashCode(), w);
                        }
                        break;
                    case "AAA":
                        {
                            AAARound w = new AAARound();
                            w.LiveTime = float.Parse(parts[1]);
                            w.Mass = float.Parse(parts[2]);
                            w.Power = float.Parse(parts[3]);
                            w.Caliber = float.Parse(parts[4]);
                            w.Fused = bool.Parse(parts[5]);
                            w.FuseType = int.Parse(parts[6]);
                            w.Explodes = bool.Parse(parts[7]);
                            w.ExplosionRadius = float.Parse(parts[8]);

                            Instance.AAARounds.Add(name.GetHashCode(), w);
                        }
                        break;
                }

            }
            else
            {
                throw new Exception("AmmunitionDatabase::Missing ammo descriptor " + basepath);
            }
        }

        public static AAARound GetAAARound(int hashcode)
        {
            if (Instance.AAARounds.ContainsKey(hashcode))
                return Instance.AAARounds[hashcode];
            throw new Exception("AmmunitionDatabase: Missing ammo type");
        }
        public static ArtilleryRound GetArtilleryRound(int hashcode)
        {
            if (Instance.ArtilleryRounds.ContainsKey(hashcode))
                return Instance.ArtilleryRounds[hashcode];
            throw new Exception("AmmunitionDatabase: Missing ammo type");
        }
    }
}
