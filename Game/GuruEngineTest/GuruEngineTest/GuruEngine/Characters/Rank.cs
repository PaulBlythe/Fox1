using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GuruEngine.Characters
{
    public class Rank
    {
        public AirforceCode Force;
        public int Code;

        public static int MakeCode(int RankCode, int Subcode, bool isFemale)
        {
            int code = (RankCode << 16) + (Subcode << 8);
            if (isFemale)
                code++;
            return code;
        }

        #region Comparisons

        public bool IsEqual(Rank other)
        {
            int c1 = (Code & 0xff00);
            int c2 = (other.Code & 0xff00);
            return c1 == c2;
        }

        public bool IsGreater(Rank other)
        {
            int c1 = (Code & 0xff00);
            int c2 = (other.Code & 0xff00);
            return c1 > c2;
        }

        public bool IsLowerr(Rank other)
        {
            int c1 = (Code & 0xff00);
            int c2 = (other.Code & 0xff00);
            return c1 < c2;
        }

        public static bool operator ==(Rank lhs, Rank rhs)
        {
            return lhs.IsEqual(rhs);
        }
        public static bool operator !=(Rank lhs, Rank rhs)
        {
            return !lhs.IsEqual(rhs);
        }
        public static bool operator <(Rank lhs, Rank rhs)
        {
            return lhs.IsLowerr(rhs);
        }
        public static bool operator >(Rank lhs, Rank rhs)
        {
            return lhs.IsGreater(rhs);
        }
        public static bool operator <=(Rank lhs, Rank rhs)
        {
            return (lhs.IsLowerr(rhs)) || (lhs.IsEqual(rhs));
        }
        public static bool operator >=(Rank lhs, Rank rhs)
        {
            return (lhs.IsGreater(rhs)) || (lhs.IsEqual(rhs));
        }

        public override bool Equals(object obj)
        {
            return base.Equals(obj);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
        #endregion

    }

}
