using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GuruEngine.AI.Aircraft.Formations
{
    public abstract class Formation
    {
        public List<Element> Elements = new List<Element>(); 

        public abstract void Update(float dt);
        public abstract void GetTargetPosition(int id);
        
    }
}
