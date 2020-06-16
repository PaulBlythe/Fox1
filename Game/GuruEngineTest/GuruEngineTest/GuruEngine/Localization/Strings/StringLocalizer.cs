using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Threading.Tasks;
using GuruEngine.Localization;

namespace GuruEngine.Localization.Strings
{
    public class StringLocalizer
    {
        public static StringLocalizer Instance;

        Dictionary<StringIDS, String> Strings = new Dictionary<StringIDS, string>();

        public StringLocalizer(SupportedLanguages language)
        {
            Instance = this;
            switch (language)
            {
                case SupportedLanguages.English:
                    Load(Path.Combine(FilePaths.DataPath, @"Localisation\english.xml"));
                    break;
            }

        }

        void Load(String file)
        {
            XmlDocument xdoc = new XmlDocument();
            xdoc.Load(file);
            XmlNodeList list = xdoc.SelectNodes("//string");
            var n = list.Count;
            for (int i=0; i<n; i++)
            {
                StringIDS id = GetStringID(list[i].Attributes["ID"].Value);
                Strings.Add(id, list[i].Attributes["Text"].Value);
            }
        }


        public static StringIDS GetStringID(string s)
        {
            return (StringIDS)Enum.Parse(typeof(StringIDS), s);
        }

        public static String GetString(StringIDS s)
        {
            return Instance.Strings[s];
        }
    }
}
