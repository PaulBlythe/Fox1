using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Runtime.InteropServices;



namespace GuruEngine.DebugHelpers
{
    

    public class Profiler
    {
        [DllImport("kernel32.dll")]
        static extern int GetCurrentProcessorNumber();

        private static Dictionary<string, ProfileData> _profiles;

        static Profiler()
        {
            _profiles = new Dictionary<string, ProfileData>();
        }

        public static void Start(string profilingName)
        {
            ProfileData profileData;
            if (_profiles.TryGetValue(profilingName, out profileData) == false)
            {
                profileData = new ProfileData { Stopwatch = new Stopwatch(), Name = profilingName };

                Process Proc = Process.GetCurrentProcess();
                profileData.Core = GetCurrentProcessorNumber();

                _profiles.Add(profilingName, profileData);
            }
            profileData.Ticks++;
            profileData.Stopwatch.Start();
        }

        public static void End(string profilingName)
        {
            if (_profiles.ContainsKey(profilingName))
            {
                var profileData = _profiles[profilingName];
                profileData.Stopwatch.Stop();
                profileData.ElapsedTime += profileData.Stopwatch.ElapsedTicks;
            }
        }

        public static void Clear()
        {
            foreach (var profileData in _profiles)
            {
                profileData.Value.ElapsedTime = 0;
                profileData.Value.Ticks = 0;
                profileData.Value.Stopwatch.Reset();
            }
        }

        public static ProfileReport CreateReport()
        {
            ProfileReport report = new ProfileReport();

            foreach (var profileData in _profiles)
            {
                ProfileReportEntry entry = new ProfileReportEntry();
                entry.ExecutionTime = profileData.Value.ElapsedTime;
                entry.Core = profileData.Value.Core;
                entry.Name = profileData.Value.Name;
                report.Entries.Add(entry);
            }
            return report;
        }
    }
}
