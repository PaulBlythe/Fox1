using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace GuruEngine.DebugHelpers
{
    public class ProfileData
    {
        public Stopwatch Stopwatch { get; set; }
        public double ElapsedTime { get; set; }           // in mS
        public string Name { get; set; }
        public long Ticks { get; set; }                 // number of times it was accessed
        public int Core { get; set; }
    }
}
