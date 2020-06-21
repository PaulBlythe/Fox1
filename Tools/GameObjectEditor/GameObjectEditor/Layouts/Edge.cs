using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameObjectEditor.Layouts
{
    public class Edge
    {
        public int Index1;
        public int Index2;

        public Edge(int i, int j)
        {
            Index1 = i;
            Index2 = j;
        }
    }
}
