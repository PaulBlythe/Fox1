using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GuruEngine.InputDevices
{

    public class AircraftInputMapping
    {
        // For looking up a particular input say "EngineStart"
        public Dictionary<String, int> RecordIndices = new Dictionary<string, int>();
        public List<InputMappingRecord> Records = new List<InputMappingRecord>();

        public AircraftInputMapping()
        {
            #region Debug only
            InputMappingRecord imr = new InputMappingRecord();
            imr.Description = "Test fire detection";
            imr.Device = "Keyboard";
            imr.DeviceIdentifier = "F";
            imr.ID = "FIREOHEAT";
            imr.Type = InputMappingRecord.InputType.Button;
            imr.Group = "Test";
            RecordIndices.Add(imr.ID, Records.Count);
            Records.Add(imr);

            imr = new InputMappingRecord();
            imr.Description = "Test oxygen quality";
            imr.Device = "Keyboard";
            imr.DeviceIdentifier = "O";
            imr.ID = "OXYQTY";
            imr.Type = InputMappingRecord.InputType.Button;
            imr.Group = "Test";
            RecordIndices.Add(imr.ID, Records.Count);
            Records.Add(imr);

            imr = new InputMappingRecord();
            imr.Description = "Test warning lights";
            imr.Device = "Keyboard";
            imr.DeviceIdentifier = "T";
            imr.ID = "MALINDLTS";
            imr.Type = InputMappingRecord.InputType.Button;
            imr.Group = "Test";
            RecordIndices.Add(imr.ID, Records.Count);
            Records.Add(imr);

            imr = new InputMappingRecord();
            imr.Description = "Test probe heat";
            imr.Device = "Keyboard";
            imr.DeviceIdentifier = "]";
            imr.ID = "TESTPROBE";
            imr.Type = InputMappingRecord.InputType.Button;
            imr.Group = "Test";
            RecordIndices.Add(imr.ID, Records.Count);
            Records.Add(imr);

            imr = new InputMappingRecord();
            imr.Description = "Probe heat toggle";
            imr.Device = "Keyboard";
            imr.DeviceIdentifier = "[";
            imr.ID = "PROBEHEAT";
            imr.Type = InputMappingRecord.InputType.Button;
            imr.Group = "Flight";
            RecordIndices.Add(imr.ID, Records.Count);
            Records.Add(imr);

            imr = new InputMappingRecord();
            imr.Description = "Test FLCS";
            imr.Device = "Keyboard";
            imr.DeviceIdentifier = "O";
            imr.ID = "TESTFLCS";
            imr.Type = InputMappingRecord.InputType.Button;
            imr.Group = "Test";
            RecordIndices.Add(imr.ID, Records.Count);
            Records.Add(imr);

            imr = new InputMappingRecord();
            imr.Description = "Swap to backup FLCS";
            imr.Device = "Keyboard";
            imr.DeviceIdentifier = "I";
            imr.ID = "SWAPFLCS";
            imr.Type = InputMappingRecord.InputType.Button;
            imr.Group = "Flight";
            RecordIndices.Add(imr.ID, Records.Count);
            Records.Add(imr);

            imr = new InputMappingRecord();
            imr.Description = "Toggle trailing edge flaps mode";
            imr.Device = "Keyboard";
            imr.DeviceIdentifier = "U";
            imr.ID = "ALTFLAPS";
            imr.Type = InputMappingRecord.InputType.Button;
            imr.Group = "Flight";
            RecordIndices.Add(imr.ID, Records.Count);
            Records.Add(imr);

            imr = new InputMappingRecord();
            imr.Description = "Auto terrain avoidance on/off";
            imr.Device = "Keyboard";
            imr.DeviceIdentifier = "Y";
            imr.ID = "TFFLYUP";
            imr.Type = InputMappingRecord.InputType.Button;
            imr.Group = "Flight";
            RecordIndices.Add(imr.ID, Records.Count);
            Records.Add(imr);

            imr = new InputMappingRecord();
            imr.Description = "Leading edge flaps lock";
            imr.Device = "Keyboard";
            imr.DeviceIdentifier = "T";
            imr.ID = "LEFLAPS";
            imr.Type = InputMappingRecord.InputType.Button;
            imr.Group = "Test";
            RecordIndices.Add(imr.ID, Records.Count);
            Records.Add(imr);

            imr = new InputMappingRecord();
            imr.Description = "Reset FLCS";
            imr.Device = "Keyboard";
            imr.DeviceIdentifier = "R";
            imr.ID = "FLCSRESET";
            imr.Type = InputMappingRecord.InputType.Button;
            imr.Group = "Flight";
            RecordIndices.Add(imr.ID, Records.Count);
            Records.Add(imr);

            imr = new InputMappingRecord();
            imr.Description = "FLCS built in test";
            imr.Device = "Keyboard";
            imr.DeviceIdentifier = "E";
            imr.ID = "BIT";
            imr.Type = InputMappingRecord.InputType.Button;
            imr.Group = "Test";
            RecordIndices.Add(imr.ID, Records.Count);
            Records.Add(imr);

            imr = new InputMappingRecord();
            imr.Description = "Roll trim";
            imr.Device = "Keyboard";
            imr.DeviceIdentifier = "E:D";
            imr.ID = "ROLLTRIM";
            imr.Type = InputMappingRecord.InputType.Wheel;
            imr.Group = "Trim";
            RecordIndices.Add(imr.ID, Records.Count);
            Records.Add(imr);

            imr = new InputMappingRecord();
            imr.Description = "Yaw trim";
            imr.Device = "Keyboard";
            imr.DeviceIdentifier = "W:S";
            imr.ID = "YAWTRIM";
            imr.Type = InputMappingRecord.InputType.Wheel;
            imr.Group = "Trim";
            RecordIndices.Add(imr.ID, Records.Count);
            Records.Add(imr);

            imr = new InputMappingRecord();
            imr.Description = "Pitch trim";
            imr.Device = "Keyboard";
            imr.DeviceIdentifier = "Q:A";
            imr.ID = "PITCHTRIM";
            imr.Type = InputMappingRecord.InputType.Wheel;
            imr.Group = "Trim";
            RecordIndices.Add(imr.ID, Records.Count);
            Records.Add(imr);

            imr = new InputMappingRecord();
            imr.Description = "Disable stick trim";
            imr.Device = "Keyboard";
            imr.DeviceIdentifier = "F";
            imr.ID = "TRIMDISC";
            imr.Type = InputMappingRecord.InputType.Button;
            imr.Group = "Flight";
            RecordIndices.Add(imr.ID, Records.Count);
            Records.Add(imr);

            imr = new InputMappingRecord();
            imr.Description = "Master fuel switch";
            imr.Device = "Keyboard";
            imr.DeviceIdentifier = "G";
            imr.ID = "MSTRFUEL";
            imr.Type = InputMappingRecord.InputType.Button;
            imr.Group = "Fuel";
            RecordIndices.Add(imr.ID, Records.Count);
            Records.Add(imr);

            imr = new InputMappingRecord();
            imr.Description = "Add halon to fuel tanks";
            imr.Device = "Keyboard";
            imr.DeviceIdentifier = "H";
            imr.ID = "TANKINERT";
            imr.Type = InputMappingRecord.InputType.Button;
            imr.Group = "Fuel";
            RecordIndices.Add(imr.ID, Records.Count);
            Records.Add(imr);

            imr = new InputMappingRecord();
            imr.Description = "Fuel tank select";
            imr.Device = "Keyboard";
            imr.DeviceIdentifier = "J";
            imr.ID = "ENGFEED";
            imr.Type = InputMappingRecord.InputType.Knob;
            imr.Group = "Fuel";
            RecordIndices.Add(imr.ID, Records.Count);
            Records.Add(imr);

            imr = new InputMappingRecord();
            imr.Description = "Air refuel toggle";
            imr.Device = "Keyboard";
            imr.DeviceIdentifier = "K";
            imr.ID = "AIRREFUEL";
            imr.Type = InputMappingRecord.InputType.Button;
            imr.Group = "Fuel";
            RecordIndices.Add(imr.ID, Records.Count);
            Records.Add(imr);

            #endregion
        }

        public void SetInput(String Device, String Event, String Target)
        {
            int i = RecordIndices[Target];
            Records[i].Device = Device;
            Records[i].DeviceIdentifier = Event;
        }
    }
}
