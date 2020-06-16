using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using GuruEngine.GameLogic.Navigation;
using GuruEngine.GameLogic.SensorData;
using GuruEngine.ECS.Components.AircraftSystems.American.Modern;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace GuruEngine.Data.Code.Instruments.American
{
    public class F16MFDData : MFDStateData
    {
        #region Sensor input
        public enum SensorInput
        {
            FireControlRadar,
            ForwardLookingInfraRed,
            DataTransferEquipment,
            StoresManagementSystem,
            HorizontalSituationDisplay,
            HUD
        }
        public SensorInput sensorInput;
        #endregion

        #region Lighting mode
        public enum LightingMode
        {
            Night,
            Day,
            Override
        }
        public LightingMode lightingMode;
        #endregion

        public List<String> Messages = new List<string>();
        public List<NavPoint> Navpoints = new List<NavPoint>();
        public List<RadarContact> RadarContacts = new List<RadarContact>();

        public int HSDRange;
        public int BullseyeBearing;
        public int BullseyeRange;

        public Quaternion Orientation;
        public Matrix World;

        public OnAPG68v5 hostRadar;

        public F16MFDData()
        {
            MFDType = Type.F16mfd;
            Default();
        }

        public override void Default()
        {
            sensorInput = SensorInput.FireControlRadar;
            lightingMode = LightingMode.Day;
            HSDRange = 60;
            BullseyeBearing = 90;
            BullseyeRange = 22;
          
            Orientation = Quaternion.Identity;
            World = Matrix.Identity;
            
        }

        public override void UpdateData()
        {
            throw new NotImplementedException();
        }

        public override int GetInt(string name)
        {
            return 0;
        }

        public override void SetFloat(string name, float value)
        {
            
        }
    }
}
