using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GuruEngine.Data.Code.Instruments
{
    
    public abstract class MFDStateData
    {
        public enum Type
        {
            F16mfd,
            Count
        }
        public Type MFDType;
        public abstract void UpdateData();
        public abstract void Default();
        public abstract int GetInt(String name);
        public abstract void SetFloat(String name, float value);
    }
}
