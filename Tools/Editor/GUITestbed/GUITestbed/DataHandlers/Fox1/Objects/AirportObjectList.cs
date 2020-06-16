using System;
using System.Collections.Generic;
using System.Xml;
using System.IO;
using System.Threading.Tasks;

using GUITestbed.GUI.Dialogs;
using GUITestbed.GUI;

namespace GUITestbed.DataHandlers.Fox1.Objects
{
    public class AirportObjectList
    {
        String dir;

        public List<RunwaySceneryObject> Objects = new List<RunwaySceneryObject>();

        public AirportObjectList(String f)
        {
            dir = Path.GetDirectoryName(f);

            XmlDocument doc = new XmlDocument();
            doc.Load(f);
            
            XmlNodeList objects = doc.DocumentElement.SelectNodes("SceneryObject");
            foreach (XmlNode n in objects)
            {
                RunwaySceneryObject rs = new RunwaySceneryObject(n);
                Objects.Add(rs);
            }
        }
    }
}
