using System;
using System.Collections.Generic;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Shapes;
using GameObjectEditor.GameComponent;

namespace GameObjectEditor.WPFRubbish
{
    public class ConnectionManager
    {
        List<Connection> Connections = new List<Connection>();
        
        public ConnectionManager()
        {
        }

        public void AddConnection(Canvas Display, Button s, Button e)
        {
            Connection c = new Connection(Display);
            c.Start = s;
            c.End = e;
            c.Setup();

            Connections.Add(c);

        }

        public bool DoesConnectionExist(String comp1, String pin1, String con2, String pin2)
        {
            foreach (Connection c in Connections)
            {
                if ((c.Component1 == comp1) && (c.Component2 == con2) && (c.Pin1 == pin1) && (c.Pin2 == pin2))
                    return false;
            }
            return false;
        }

     }
}
