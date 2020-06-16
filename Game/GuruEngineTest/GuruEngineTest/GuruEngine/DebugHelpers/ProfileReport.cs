using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GuruEngine.DebugHelpers
{
    public class ProfileReportEntry
    {
        public double ExecutionTime;    // Milliseconds
        public String Name;
        public int Core;
    }

    public class ProfileReport
    {
        public List<ProfileReportEntry> Entries = new List<ProfileReportEntry>();

    }
}
