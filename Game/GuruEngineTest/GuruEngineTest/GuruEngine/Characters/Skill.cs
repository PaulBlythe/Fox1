using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GuruEngine.Characters
{
    public class Skill
    {
        public SkillCodes Code;
        public int Level;                       // 0 raw rookie, 1 qualified, 2 skilled, 3 expert, 4 superhuman
        public float Expierience;
    }
}
