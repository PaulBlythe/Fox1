using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using GuruEngine.Player.Records;

namespace GuruEngine.Characters.NPC.Crew
{
    public abstract class CrewMember
    {
        public String FirstName;
        public String[] MiddleNames;
        public String LastName;
        public DateTime BirthDate;
        public Rank CurrentRank;
        public List<Skill> Skills = new List<Skill>();
    }
}
