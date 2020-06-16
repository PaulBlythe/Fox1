using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using History.RAF.WWII;
using History.RAF.Common;
using History.Common.Geographic;

namespace History.RAF.Setups
{
    public class RAFBattleOfBritain : RAFCommandGroupWWII
    {
        public RAFBattleOfBritain()
        {
            // Fighter command
            GeographicLocation bhall = new GeographicLocation("RAF Barton Hall", 53.819167, -2.728333);
            GeographicLocation box = new GeographicLocation("RAF Box", 51.420633, -2.216861);
            GeographicLocation rux = new GeographicLocation("RAF Uxbridge", 51.542222, -0.469722);
            GeographicLocation rwat = new GeographicLocation("RAF Watnall", 53.006, -1.247);
            GeographicLocation rnew = new GeographicLocation("RAF Blakelaw", 55.0002, -1.6633);
            GeographicLocation rdrum = new GeographicLocation("Drumossie Hotel", 57.45749, -4.15967);

            Groups.Add("9 Group", new RAFGroup(RAFGroupRole.Fighter, "9 Group", bhall));
            Groups.Add("10 Group", new RAFGroup(RAFGroupRole.Fighter, "10 Group", box));
            Groups.Add("11 Group", new RAFGroup(RAFGroupRole.Fighter, "11 Group", rux));
            Groups.Add("12 Group", new RAFGroup(RAFGroupRole.Fighter, "12 Group", rwat));
            Groups.Add("13 Group", new RAFGroup(RAFGroupRole.Fighter, "13 Group", rnew));
            Groups.Add("14 Group", new RAFGroup(RAFGroupRole.Fighter, "14 Group", rdrum));


            Groups["9 Group"].Stations.Add(new RAFStation("Honiley", "", RAFStationTypes.Sector, new GeographicLocation("RAF Honiley", 52.356111, -1.665)));
            Groups["9 Group"].Stations.Add(new RAFStation("Woodvale", "", RAFStationTypes.Sector, new GeographicLocation("RAF Woodvale", 53.581667, -3.055556)));

            Groups["10 Group"].Stations.Add(new RAFStation("Filton", "", RAFStationTypes.Sector, new GeographicLocation("RAF Filton", 51.519167, -2.578611)));
            Groups["10 Group"].Stations.Add(new RAFStation("Boscombe Down", "", RAFStationTypes.Satelite, new GeographicLocation("RAF Boscombe Down", 51.1575, -1.746944)));
            Groups["10 Group"].Stations.Add(new RAFStation("Pembrey", "", RAFStationTypes.Satelite, new GeographicLocation("RAF Pembrey", 51.7075, -4.315)));
            Groups["10 Group"].Stations.Add(new RAFStation("Colerne", "", RAFStationTypes.Satelite, new GeographicLocation("RAF Colerne", 51.441111, -2.2825)));

            Groups["10 Group"].Stations.Add(new RAFStation("Middle Wallop", "STARLIGHT", RAFStationTypes.Sector, new GeographicLocation("RAF Middle Wallop", 51.148889, -1.57)));
            Groups["10 Group"].Stations.Add(new RAFStation("Boscombe Down", "STARLIGHT", RAFStationTypes.Satelite, new GeographicLocation("RAF Boscombe Down", 51.1575, -1.746944)));
            Groups["10 Group"].Stations.Add(new RAFStation("Warmwell", "STARLIGHT", RAFStationTypes.Satelite, new GeographicLocation("RAF Warmwell", 50.697222, -2.344444)));
            Groups["10 Group"].Stations.Add(new RAFStation("Exeter", "STARLIGHT", RAFStationTypes.Satelite, new GeographicLocation("RAF Exeter", 50.734444, -3.413889)));
            Groups["10 Group"].Stations.Add(new RAFStation("St Eval", "STARLIGHT", RAFStationTypes.Satelite, new GeographicLocation("RAF St Eval", 50.478056, -4.999444)));

            Groups["11 Group"].Stations.Add(new RAFStation("Tangmere", "SHORTJACK", RAFStationTypes.Sector, new GeographicLocation("RAF Tangmere", 50.845833, -0.706389)));
            Groups["11 Group"].Stations.Add(new RAFStation("Westhampnett", "SHORTJACK", RAFStationTypes.Satelite, new GeographicLocation("RAF Westhampnett", 50.861111, -0.759167)));

            Groups["11 Group"].Stations.Add(new RAFStation("Kenley", "TOPHAT", RAFStationTypes.Sector, new GeographicLocation("RAF Kenley", 51.303611, -0.095)));
            Groups["11 Group"].Stations.Add(new RAFStation("Croyden", "TOPHAT", RAFStationTypes.Satelite, new GeographicLocation("RAF Croyden", 51.356389, -0.117222)));

            Groups["11 Group"].Stations.Add(new RAFStation("Biggin Hill", "SAPPER", RAFStationTypes.Sector, new GeographicLocation("RAF Biggin Hill", 51.330833, 0.0325)));
            Groups["11 Group"].Stations.Add(new RAFStation("West Malling", "SAPPER", RAFStationTypes.Satelite, new GeographicLocation("RAF West Malling", 51.271111, 0.4025)));
            Groups["11 Group"].Stations.Add(new RAFStation("Gravesend", "SAPPER", RAFStationTypes.Satelite, new GeographicLocation("RAF Gravesend", 51.418056, 0.396389)));

            Groups["11 Group"].Stations.Add(new RAFStation("Hornchurch", "LUMBA", RAFStationTypes.Sector, new GeographicLocation("RAF Hornchurch", 51.538611, 0.204722)));
            Groups["11 Group"].Stations.Add(new RAFStation("Gravesend", "LUMBA", RAFStationTypes.Satelite, new GeographicLocation("RAF Gravesend", 51.418056, 0.396389)));
            Groups["11 Group"].Stations.Add(new RAFStation("Rochford", "LUMBA", RAFStationTypes.Satelite, new GeographicLocation("RAF Rochford", 51.570278, 0.693333)));
            Groups["11 Group"].Stations.Add(new RAFStation("Manston", "LUMBA", RAFStationTypes.Satelite, new GeographicLocation("RAF Manston", 51.348, 1.35)));
            Groups["11 Group"].Stations.Add(new RAFStation("Hawkinge", "LUMBA", RAFStationTypes.Satelite, new GeographicLocation("RAF Hawkinge", 51.1125, 1.1525)));

            Groups["11 Group"].Stations.Add(new RAFStation("North Weald", "COWSLIP", RAFStationTypes.Sector, new GeographicLocation("RAF North Weald", 51.721667, 0.154167)));
            Groups["11 Group"].Stations.Add(new RAFStation("Stapleford Tawney", "COWSLIP", RAFStationTypes.Satelite, new GeographicLocation("RAF Stapleford Tawney", 51.6525, 0.156111)));
            Groups["11 Group"].Stations.Add(new RAFStation("Martlesham Heath", "COWSLIP", RAFStationTypes.Satelite, new GeographicLocation("RAF Martlesham Heath", 52.058, 1.266)));

            Groups["11 Group"].Stations.Add(new RAFStation("Debden", "GARTER", RAFStationTypes.Sector, new GeographicLocation("RAF Debden", 51.991667, 0.270556)));
            Groups["11 Group"].Stations.Add(new RAFStation("Martlesham Heath", "GARTER", RAFStationTypes.Satelite, new GeographicLocation("RAF Martlesham Heath", 52.058, 1.266)));

            Groups["11 Group"].Stations.Add(new RAFStation("Northolt", "GARTER", RAFStationTypes.Sector, new GeographicLocation("RAF Northolt", 51.553056, -0.418333)));
            Groups["11 Group"].Stations.Add(new RAFStation("Hendon", "GARTER", RAFStationTypes.Satelite, new GeographicLocation("RAF Hendon", 51.601, -0.245)));

            Groups["12 Group"].Stations.Add(new RAFStation("Duxford", "", RAFStationTypes.Sector, new GeographicLocation("RAF Duxford", 52.090833, 0.131944)));
            Groups["12 Group"].Stations.Add(new RAFStation("Fowlmere", "", RAFStationTypes.Satelite, new GeographicLocation("RAF Fowlmere", 52.0825, 0.058611)));

            Groups["12 Group"].Stations.Add(new RAFStation("Duxford", "", RAFStationTypes.Sector, new GeographicLocation("RAF Duxford", 52.090833, 0.131944)));
            Groups["12 Group"].Stations.Add(new RAFStation("Fowlmere", "", RAFStationTypes.Satelite, new GeographicLocation("RAF Fowlmere", 52.0825, 0.058611)));

            Groups["12 Group"].Stations.Add(new RAFStation("Wittering", "", RAFStationTypes.Sector, new GeographicLocation("RAF Wittering", 52.6125, -0.476389)));
            Groups["12 Group"].Stations.Add(new RAFStation("Coltishall", "", RAFStationTypes.Satelite, new GeographicLocation("RAF Coltishall", 52.754722, 1.357222)));

            Groups["12 Group"].Stations.Add(new RAFStation("Digby", "", RAFStationTypes.Sector, new GeographicLocation("RAF Digby", 53.090833, -0.434167)));
            Groups["12 Group"].Stations.Add(new RAFStation("Ternhill", "", RAFStationTypes.Satelite, new GeographicLocation("RAF Ternhill", 52.871111, -2.533611)));

            Groups["12 Group"].Stations.Add(new RAFStation("Kirton in Lindsey", "", RAFStationTypes.Sector, new GeographicLocation("RAF Kirton in Lindsey", 53.461389, -0.578056)));

            Groups["12 Group"].Stations.Add(new RAFStation("Church Fenton", "", RAFStationTypes.Sector, new GeographicLocation("RAF Church Fenton", 53.834444, -1.195556)));
            Groups["12 Group"].Stations.Add(new RAFStation("Leconfield", "", RAFStationTypes.Satelite, new GeographicLocation("RAF Leconfield", 53.876944, -0.4375)));

            Groups["13 Group"].Stations.Add(new RAFStation("Usworth", "", RAFStationTypes.Sector, new GeographicLocation("RAF Usworth", 54.915, -1.475)));
            Groups["13 Group"].Stations.Add(new RAFStation("Catterick", "", RAFStationTypes.Satelite, new GeographicLocation("RAF Catterick", 54.365278, -1.619722)));

            Groups["13 Group"].Stations.Add(new RAFStation("Acklington", "", RAFStationTypes.Sector, new GeographicLocation("RAF Acklington", 55.296111, -1.634444)));

            Groups["13 Group"].Stations.Add(new RAFStation("Turnhouse", "", RAFStationTypes.Sector, new GeographicLocation("RAF Turnhouse", 55.95, -3.3725)));
            Groups["13 Group"].Stations.Add(new RAFStation("Drem", "", RAFStationTypes.Satelite, new GeographicLocation("RAF Drem", 56.022, -2.794)));
            Groups["13 Group"].Stations.Add(new RAFStation("Grangemouth", "", RAFStationTypes.Satelite, new GeographicLocation("RAF Grangemouth", 56.019722, -3.691667)));

            Groups["13 Group"].Stations.Add(new RAFStation("Dyce", "", RAFStationTypes.Sector, new GeographicLocation("RAF Dyce", 57.2025, -2.198056)));
            Groups["13 Group"].Stations.Add(new RAFStation("Grangemouth", "", RAFStationTypes.Satelite, new GeographicLocation("RAF Grangemouth", 56.019722, -3.691667)));

            Groups["13 Group"].Stations.Add(new RAFStation("Wick", "", RAFStationTypes.Sector, new GeographicLocation("RAF Wick", 58.458889, -3.093056)));
            Groups["13 Group"].Stations.Add(new RAFStation("Grimsetter", "", RAFStationTypes.Satelite, new GeographicLocation("RAF Grimsetter", 58.958056, -2.900556)));
            Groups["13 Group"].Stations.Add(new RAFStation("Sumburgh", "", RAFStationTypes.Satelite, new GeographicLocation("RAF Sumburgh", 59.878611, -1.296111)));



        }
    }
}
