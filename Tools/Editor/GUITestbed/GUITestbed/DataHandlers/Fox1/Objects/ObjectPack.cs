using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace GUITestbed.DataHandlers.Fox1.Objects
{
    public class ObjectPack
    {
        public Dictionary<Guid, PackObject> PackedObjects = new Dictionary<Guid, PackObject>();
        public String Directory;

        public ObjectPack(String filename)
        {
            Directory = Path.GetDirectoryName(filename);

            using (TextReader reader = File.OpenText(filename))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    if (line.StartsWith("#"))
                    {

                    }
                    else
                    {
                        if (line.Length >3)
                        {
                            if (line.Contains("-"))
                            {
                                string[] parts = line.Split(' ');
                                Guid g = new Guid(parts[0]);
                                //System.Console.WriteLine(g.ToString());
                                PackObject p = new PackObject(g, parts[1]);
                                PackedObjects.Add(g, p);
                            }
                            else
                            {
                                string[] parts = line.Split(' ');

                                String m1 = parts[0].Substring(0, 8).ToLower();
                                String m2 = parts[0].Substring(8, 4).ToLower();
                                String m3 = parts[0].Substring(12, 4).ToLower();
                                String m4 = parts[0].Substring(16, 2).ToLower();
                                String m5 = parts[0].Substring(18, 2).ToLower();
                                String m6 = parts[0].Substring(20, 2).ToLower();
                                String m7 = parts[0].Substring(22, 2).ToLower();
                                String m8 = parts[0].Substring(24, 2).ToLower();
                                String m9 = parts[0].Substring(26, 2).ToLower();
                                String m10 = parts[0].Substring(28, 2).ToLower();
                                String m11 = parts[0].Substring(30, 2).ToLower();

                                Guid g = new Guid(m1 + m3 + m2 + m7 + m6 + m5 + m4 + m11 + m10 + m9 + m8);
                                //System.Console.WriteLine(g.ToString());
                                PackObject p = new PackObject(g, parts[1]);
                                PackedObjects.Add(g, p);
                            }
                        }
                    }
                }
            }
        }

        public String GetPath(Guid n)
        {
            String file = PackedObjects[n].Filename + ".obj";
            return Path.Combine(Directory, file);
        }
    }
}
