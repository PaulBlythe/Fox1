using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GuruEngine.Simulation.Components.Radar.Airbourne.Modern
{
    public interface AirbourneRadarInterface
    {
        String GetMode();           // String descriptor of general mode of operation. Usually three characters CRM = Combined Radar Mode
        String GetSubMode();        // String descriptor of specific radar mode. Usually three characters TWS = Track While Scan

        int GetBars();              // Number of vertical slices the radar is scanning
        int GetAzimuth();           // Half width of scan region in degrees 
        int GetRange();             // Scanning range in miles
        int GetTargetHistory();     // How many scans to keep information about targets
        int GetBuggedTarget();      // Returns the UID of the tracked target

        float GetEmitterAngle();    // Current position of the emitter
        float GetOffset();          // User horizontal offset applied to the emitter
        float GetTilt();            // User vertical offset applied to the emitter

        void HandleSystemInput(String evt);
        void BugTarget(int ID);

        Dictionary<int, AirbourneRadarTarget> GetTrackedTargets();
    }
}
