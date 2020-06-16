using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

using GUITestbed.DataHandlers.Mapping.Types;

namespace GUITestbed.DataHandlers
{
    public class CSV : Database
    {
        List<DatabaseHeaderItem> Header = new List<DatabaseHeaderItem>();
        List<List<String>> DataItems = new List<List<string>>();

        public CSV(String file)
        {
            using (StreamReader sr = new StreamReader(file))
            {
                String line;

                line = sr.ReadLine();
                String[] parts = line.Split(',');

                for (int i = 0; i < parts.Length; i++)
                {
                    String[] p2 = parts[i].Split('.');

                    DatabaseHeaderItem di = new DatabaseHeaderItem();
                    di.Name = p2[0];
                    di.Type = p2[1];
                    di.Length = int.Parse(p2[2]);

                    Header.Add(di);
                }

                while ((line = sr.ReadLine()) != null)
                {
                    parts = line.Split(',');
                    List<String> record = new List<string>();
                    foreach (String s in parts)
                    {
                        record.Add(s);
                    }
                    DataItems.Add(record);
                }
            }
        }

        public override string GetRecord(int index, string name)
        {
            for (int i = 0; i < Header.Count; i++)
            {
                if (String.Equals(Header[i].Name,name,StringComparison.InvariantCultureIgnoreCase))
                {
                    return DataItems[index][i];
                }

            }
            return null;
        }

        public override int GetCount()
        {
            return DataItems.Count;
        }
    }
}
