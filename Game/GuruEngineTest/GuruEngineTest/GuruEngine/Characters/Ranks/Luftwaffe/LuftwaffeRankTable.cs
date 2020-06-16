using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GuruEngine.Characters.Ranks.Luftwaffe
{
    /// <summary>
    /// subcodes
    ///   5 post 1944
    ///   1 subrank
    ///   2 cadet
    ///   10 Flyers
    ///   11 Paratroopers
    ///   12 Anti aircraft gunner
    ///   13 Communications
    ///   14 Air force
    /// </summary>
    public class LuftwaffeRankTable
    {
        public static Dictionary<int, String> Ranks = new Dictionary<int, string>();
        public static Dictionary<String, String> ShortForm = new Dictionary<string, string>();

        public LuftwaffeRankTable()
        {
            Ranks.Add(Rank.MakeCode(RankCodes.QR1, 0, false), "Flieger");
            Ranks.Add(Rank.MakeCode(RankCodes.QR2, 0, false), "Gefreiter");
            Ranks.Add(Rank.MakeCode(RankCodes.QR3, 0, false), "Obergefreiter");
            Ranks.Add(Rank.MakeCode(RankCodes.QR4, 0, false), "Hauptgefreiter");
            Ranks.Add(Rank.MakeCode(RankCodes.QR4, 5, false), "Stabsgefreiter");
            Ranks.Add(Rank.MakeCode(RankCodes.QR5, 0, false), "Unteroffizier");
            Ranks.Add(Rank.MakeCode(RankCodes.QR5, 2, false), "Fahnenjunker");
            Ranks.Add(Rank.MakeCode(RankCodes.QR5, 1, false), "Unterfeldwebel");
            Ranks.Add(Rank.MakeCode(RankCodes.QR5, 3, false), "Fähnrich");
            Ranks.Add(Rank.MakeCode(RankCodes.QR6, 0, false), "Feldwebel");
            Ranks.Add(Rank.MakeCode(RankCodes.QR7, 0, false), "Oberfeldwebel");
            Ranks.Add(Rank.MakeCode(RankCodes.QR7, 1, false), "Oberfähnrich");
            Ranks.Add(Rank.MakeCode(RankCodes.QR8, 0, false), "Stabsfeldwebel");

            Ranks.Add(Rank.MakeCode(RankCodes.OF0, 0, false), "Leutnant");
            Ranks.Add(Rank.MakeCode(RankCodes.OF1, 0, false), "Oberleutnant");
            Ranks.Add(Rank.MakeCode(RankCodes.OF2, 0, false), "Hauptmann");
            Ranks.Add(Rank.MakeCode(RankCodes.OF3, 0, false), "Major");
            Ranks.Add(Rank.MakeCode(RankCodes.OF4, 0, false), "Oberstleutnant");
            Ranks.Add(Rank.MakeCode(RankCodes.OF5, 0, false), "Oberst");
            Ranks.Add(Rank.MakeCode(RankCodes.OF6, 0, false), "Generalmajor");
            Ranks.Add(Rank.MakeCode(RankCodes.OF7, 0, false), "Generalleutnant");
            Ranks.Add(Rank.MakeCode(RankCodes.OF8, 10, false), "General der Flieger");
            Ranks.Add(Rank.MakeCode(RankCodes.OF8, 11, false), "General der Fallschirmtruppe");
            Ranks.Add(Rank.MakeCode(RankCodes.OF8, 13, false), "General der Luftnachrichtentruppe");
            Ranks.Add(Rank.MakeCode(RankCodes.OF8, 12, false), "General der Flakartillerie");
            Ranks.Add(Rank.MakeCode(RankCodes.OF8, 14, false), "General der Luftwaffe");
            Ranks.Add(Rank.MakeCode(RankCodes.OF9, 0, false), "Generaloberst");
            Ranks.Add(Rank.MakeCode(RankCodes.OF10, 0, false), "Generalfeldmarschall");
            Ranks.Add(Rank.MakeCode(RankCodes.OF11, 0, false), "Reichsmarschall");

            ShortForm.Add("Flieger", "Flg");
            ShortForm.Add("Gefreiter", "Gefr");
            ShortForm.Add("Obergefreiter", "Ogfr");
            ShortForm.Add("Hauptgefreiter", "Hptgefr");
            ShortForm.Add("Stabsgefreiter", "Sbsgefr");
            ShortForm.Add("Unteroffizier", "Uffz");
            ShortForm.Add("Fahnenjunker", "Fhr");
            ShortForm.Add("Unterfeldwebel", "Ufw");
            ShortForm.Add("Fähnrich", "Fhr");
            ShortForm.Add("Feldwebel", "Fw");
            ShortForm.Add("Oberfeldwebel", "Obfw");
            ShortForm.Add("Oberfähnrich", "	Obfhr");
            ShortForm.Add("Stabsfeldwebel", "Stabsfw");
            ShortForm.Add("Leutnant", "Lt");
            ShortForm.Add("Oberleutnant", "Olt");
            ShortForm.Add("Hauptmann", "Hptm");
            ShortForm.Add("Major", "Maj");
            ShortForm.Add("Oberstleutnant", "ObLt");
            ShortForm.Add("Oberst", "Ob");
            ShortForm.Add("Generalmajor", "GenMaj");
            ShortForm.Add("Generalleutnant", "GenLt");

            ShortForm.Add("General der Flieger", "GendFl");
            ShortForm.Add("General der Fallschirmtruppe", "GendFal");
            ShortForm.Add("General der Luftnachrichtentruppe", "GendLuft");
            ShortForm.Add("General der Flakartillerie", "GendFlak");
            ShortForm.Add("General der Luftwaffe", "GendL");

            ShortForm.Add("Generaloberst", "GenOb");
            ShortForm.Add("Generalfeldmarschall", "GenFeldm");
            ShortForm.Add("Reichsmarschall", "RM");
        }

    }
}
