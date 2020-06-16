using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GuruEngine.World.Items
{
    public class OrbitalElements
    {
        public double Period;
        public double EpochLongitude;
        public double PerihelionLongitude;
        public double Eccentricity;
        public double SemiMajorAxis;
        public double Inclination;
        public double LongitudeAscendingNode;
        public double AngularDiameter;
        public double VisualMagnitude;


        public OrbitalElements(double period, double epochLongitude, double perihelionLongitude,double eccentricity, double semiMajorAxis, double inclination, double longitudeAscendingNode,double angularDiameter, double visualMagnitude)
        {
            Period = period;
            EpochLongitude = epochLongitude;
            PerihelionLongitude = perihelionLongitude;
            Eccentricity = eccentricity;
            SemiMajorAxis = semiMajorAxis;
            Inclination = inclination;
            LongitudeAscendingNode = longitudeAscendingNode;
            AngularDiameter = angularDiameter;
            VisualMagnitude = visualMagnitude;
        }
    }
}
