using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GuruEngine.Characters;

namespace GuruEngine.Characters.Ranks.RAF
{
    public class RAFRankTable 
    {
        public static Dictionary<int, String> Ranks = new Dictionary<int, string>();
        public static Dictionary<String, String> ShortForm = new Dictionary<string, string>();

        /// <summary>
        /// Subcodes 
        ///    1  part rank
        ///    2  technician
        ///    10 aircrew
        /// </summary>
        public RAFRankTable()
        {
            Ranks.Add(Rank.MakeCode(RankCodes.QR1, 0, false), "Aircraftman");
            Ranks.Add(Rank.MakeCode(RankCodes.QR1, 0, true), "Aircraftwoman");
            Ranks.Add(Rank.MakeCode(RankCodes.QR2, 0, false), "Leading Aircraftman");
            Ranks.Add(Rank.MakeCode(RankCodes.QR2, 0, true), "Leading Aircraftwoman");
            Ranks.Add(Rank.MakeCode(RankCodes.QR2, 1, false), "Senior Aircraftman");
            Ranks.Add(Rank.MakeCode(RankCodes.QR2, 1, true), "Senior Aircraftwoman");
            Ranks.Add(Rank.MakeCode(RankCodes.QR2, 2, false), "Senior Aircraftman Technician");
            Ranks.Add(Rank.MakeCode(RankCodes.QR2, 2, true), "Senior Aircraftwoman Technician");
            Ranks.Add(Rank.MakeCode(RankCodes.QR3, 0, false), "Lance Corporal");
            Ranks.Add(Rank.MakeCode(RankCodes.QR3, 0, true), "Lance Corporal");
            Ranks.Add(Rank.MakeCode(RankCodes.QR4, 0, false), "Corporal");
            Ranks.Add(Rank.MakeCode(RankCodes.QR4, 0, true), "Corporal");
            Ranks.Add(Rank.MakeCode(RankCodes.QR6, 0, false), "Sergeant");
            Ranks.Add(Rank.MakeCode(RankCodes.QR6, 0, true), "Sergeant");

            Ranks.Add(Rank.MakeCode(RankCodes.QR6, 10, false), "Sergeant Aircrew");
            Ranks.Add(Rank.MakeCode(RankCodes.QR7, 2, false), "Chief Technician");
            Ranks.Add(Rank.MakeCode(RankCodes.QR7, 0, false), "Flight Sergeant");
            Ranks.Add(Rank.MakeCode(RankCodes.QR7, 10, false), "Flight Sergeant Aircrew");

            Ranks.Add(Rank.MakeCode(RankCodes.QR9, 0, false), "Warrant Officer");
            Ranks.Add(Rank.MakeCode(RankCodes.QR9, 10, false), "Master Aircrew");

            Ranks.Add(Rank.MakeCode(RankCodes.OF0, 0, false), "Officer Cadet");
            Ranks.Add(Rank.MakeCode(RankCodes.OF0, 0, true), "Officer Cadet");

            Ranks.Add(Rank.MakeCode(RankCodes.OF1, 0, false), "Flying Officer");
            Ranks.Add(Rank.MakeCode(RankCodes.OF1, 0, true), "Flying Officer");
            Ranks.Add(Rank.MakeCode(RankCodes.OF1, 10, false), "Pilot Officer");
            Ranks.Add(Rank.MakeCode(RankCodes.OF1, 10, true), "Pilot Officer");

            Ranks.Add(Rank.MakeCode(RankCodes.OF2, 0, false), "Flight lieutenant");
            Ranks.Add(Rank.MakeCode(RankCodes.OF2, 0, true), "Flight lieutenant");
            Ranks.Add(Rank.MakeCode(RankCodes.OF2, 10, false), "Flight lieutenant");
            Ranks.Add(Rank.MakeCode(RankCodes.OF2, 10, true), "Flight lieutenant");

            Ranks.Add(Rank.MakeCode(RankCodes.OF3, 0, false), "Squadron Leader");
            Ranks.Add(Rank.MakeCode(RankCodes.OF3, 0, true), "Squadron Leader");
            Ranks.Add(Rank.MakeCode(RankCodes.OF3, 10, false), "Squadron Leader");
            Ranks.Add(Rank.MakeCode(RankCodes.OF3, 10, true), "Squadron Leader");

            Ranks.Add(Rank.MakeCode(RankCodes.OF4, 0, false), "Wing Commander");
            Ranks.Add(Rank.MakeCode(RankCodes.OF4, 0, true), "Wing Commander");
            Ranks.Add(Rank.MakeCode(RankCodes.OF4, 10, false), "Wing Commander");
            Ranks.Add(Rank.MakeCode(RankCodes.OF4, 10, true), "Wing Commander");

            Ranks.Add(Rank.MakeCode(RankCodes.OF5, 0, false), "Group Captain");
            Ranks.Add(Rank.MakeCode(RankCodes.OF5, 0, true), "Group Captain");
            Ranks.Add(Rank.MakeCode(RankCodes.OF5, 10, false), "Group Captain");
            Ranks.Add(Rank.MakeCode(RankCodes.OF5, 10, true), "Group Captain");

            Ranks.Add(Rank.MakeCode(RankCodes.OF6, 0, false), "Air commodore");
            Ranks.Add(Rank.MakeCode(RankCodes.OF6, 0, true), "Air commodore");
            Ranks.Add(Rank.MakeCode(RankCodes.OF6, 10, false), "Air commodore");
            Ranks.Add(Rank.MakeCode(RankCodes.OF6, 10, true), "Air commodore");

            Ranks.Add(Rank.MakeCode(RankCodes.OF7, 0, false), "Air vice-marshal");
            Ranks.Add(Rank.MakeCode(RankCodes.OF7, 0, true), "Air vice-marshal");
            Ranks.Add(Rank.MakeCode(RankCodes.OF7, 10, false), "Air vice-marshal");
            Ranks.Add(Rank.MakeCode(RankCodes.OF7, 10, true), "Air vice-marshal");

            Ranks.Add(Rank.MakeCode(RankCodes.OF8, 0, false), "Air marshal");
            Ranks.Add(Rank.MakeCode(RankCodes.OF8, 0, true), "Air marshal");
            Ranks.Add(Rank.MakeCode(RankCodes.OF8, 10, false), "Air marshal");
            Ranks.Add(Rank.MakeCode(RankCodes.OF8, 10, true), "Air marshal");

            Ranks.Add(Rank.MakeCode(RankCodes.OF9, 0, false), "Air chief marshal");
            Ranks.Add(Rank.MakeCode(RankCodes.OF9, 0, true), "Air chief marshal");
            Ranks.Add(Rank.MakeCode(RankCodes.OF9, 10, false), "Air chief marshal");
            Ranks.Add(Rank.MakeCode(RankCodes.OF9, 10, true), "Air chief marshal");

            Ranks.Add(Rank.MakeCode(RankCodes.OF10, 0, false), "Marshal of the RAF");
            Ranks.Add(Rank.MakeCode(RankCodes.OF10, 0, true), "Marshal of the RAF");
            Ranks.Add(Rank.MakeCode(RankCodes.OF10, 10, false), "Marshal of the RAF");
            Ranks.Add(Rank.MakeCode(RankCodes.OF10, 10, true), "Marshal of the RAF");

            ShortForm.Add("Marshal of the RAF", "MRAF");
            ShortForm.Add("Air chief marshal", "Air Chf Mshl");
            ShortForm.Add("Air marshal", "Air Mshl");
            ShortForm.Add("Air vice-marshal", "AVM");
            ShortForm.Add("Air commodore", "Air Cdre");
            ShortForm.Add("Group Captain", "Gp Capt");
            ShortForm.Add("Wing Commander", "Wg Cdr");
            ShortForm.Add("Squadron Leader", "Sqn Ldr");
            ShortForm.Add("Flight lieutenant", "Flt Lt");
            ShortForm.Add("Pilot Officer", "Plt Off");
            ShortForm.Add("Flying Officer", "Fg Off");
            ShortForm.Add("Officer Cadet", "Off Cdt");
            ShortForm.Add("Master Aircrew", "MAcr");
            ShortForm.Add("Warrant Officer", "WO");
            ShortForm.Add("Flight Sergeant Aircrew", "FS");
            ShortForm.Add("Flight Sergeant", "FS");
            ShortForm.Add("Chief Technician", "Chf Tech");
            ShortForm.Add("Sergeant Aircrew", "Sgt");
            ShortForm.Add("Sergeant", "Sgt");
            ShortForm.Add("Corporal", "Cpl");
            ShortForm.Add("Lance Corporal", "L/Cpl");
            ShortForm.Add("Senior Aircraftwoman Technician", "SAC Tech");
            ShortForm.Add("Senior Aircraftman Technician", "SAC Tech");
            ShortForm.Add("Senior Aircraftwoman", "SAC");
            ShortForm.Add("Senior Aircraftman Technician", "SAC");
            ShortForm.Add("Senior Aircraftwoman", "SAC");
            ShortForm.Add("Senior Aircraftman", "SAC");
            ShortForm.Add("Leading Aircraftwoman", "LAF");
            ShortForm.Add("Leading Aircraftman", "LAF");
            ShortForm.Add("Aircraftwoman", "AC");
            ShortForm.Add("Aircraftman", "AC");

        }
    }
}
