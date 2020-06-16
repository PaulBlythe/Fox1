using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

using GuruEngine.ECS;
using GuruEngine.ECS.Components.AircraftSystems.General;
using GuruEngine.Helpers;
using GuruEngine.World;

namespace GuruEngine.Physics.Propulsion.Engines
{
    public class Turbine : AircraftEngine
    {
        #region Engine definition

        public double milthrust;
        public double maxthrust;
        public double bypassratio;
        public double idlen1;
        public double idlen2;
        public double maxn1;
        public double maxn2;
        public bool augmented;
        public int augmethod;
        public bool injected;
        public double tsfc;                 // Thrust Specific Fuel Consumption (lbm/hr/lbf)
        public double atsfc;                // Augmented TSFC (lbm/hr/lbf)
        public double MaxExhaustTemperature;
        public double GeneratorPower;       // KVA 
        public double StarterCurrent;       // A

        #region Idle thrust
        public double[] IdleColumnValues;
        public double[] IdleRowValues;
        public double[,] IdleDataValues;
        public double IdleScalar;
        #endregion

        #region Mil thrust
        public double[] MilColumnValues;
        public double[] MilRowValues;
        public double[,] MilDataValues;
        public double MilScalar;
        #endregion

        #region Augmented thrust
        public double[] AugColumnValues;
        public double[] AugRowValues;
        public double[,] AugDataValues;
        public double AugScalar;
        #endregion

        #region Injected thrust
        public double[] InjColumnValues;
        public double[] InjRowValues;
        public double[,] InjDataValues;
        public double InjScalar;
        #endregion

        #endregion

        #region Input values
        public bool HasPower = false;      // is the electrical system connected
        #endregion

        #region Computed values
        public double output_thrust;
        double N1;               ///< N1
        double N2;               ///< N2
        public double N2norm;    ///< N2 normalized (0=idle, 1=max)
        double IdleFF;           ///< Idle Fuel Flow (lbm/hr)
        double delay;            ///< Inverse spool-up time from idle to 100% (seconds)
        double N1_factor;        ///< factor to tie N1 and throttle
        double N2_factor;        ///< factor to tie N2 and throttle
        double ThrottlePos;      ///< FCS-supplied throttle position
        double AugmentCmd;       ///< modulated afterburner command (0.0 to 1.0)
        double TAT;              ///< total air temperature (deg C)
        double N1_spinup;        ///< N1 spin up rate from starter (per second)
        double N2_spinup;        ///< N2 spin up rate from starter (per second)
        

        bool Stalled;            ///< true if engine is compressor-stalled
        bool Seized;             ///< true if inner spool is seized
        bool Overtemp;           ///< true if EGT exceeds limits
        bool Fire;               ///< true if engine fire detected
        bool Injection;
        bool Augmentation;
        bool Cutoff;
        bool Running;
        bool Starved;
        bool Starter;
        bool Cranking;
        bool TrimMode;
        bool FuelFreeze;
        bool PlayingSpinup = false;

        double EGT_degC;
        double EPR;
        double OilPressure_psi;
        double OilTemp_degK;
        double BleedDemand;
        double InletPosition;
        double NozzlePosition;
        double correctedTSFC;
        double InjectionTimer;
        double InjectionTime;
        double Throttle;
        double PctPower;
        double FuelExpended;
        double FuelFlow_pph;
        double FuelFlowRate;

        double Mach;
        double Altitude;
        double OvertempTimer;

        #endregion

        #region State variables
        private enum Phase { Off, Run, SpinUp, Start, Stall, Seize, Trim };

        Phase phase = Phase.Off;

        #endregion

        public Turbine()
        {
            N1_factor = maxn1 - idlen1;
            N2_factor = maxn2 - idlen2;
            OilTemp_degK = 366;

            phase = Phase.Off;
            Stalled = false;
            Seized = false;
            Overtemp = false;
            Fire = false;
            N1_spinup = 1.0;
            N2_spinup = 3.0;
            N1 = N2 = 0;
            Running = false;
            Starved = false;
            Cutoff = true;
            BleedDemand = 0.0;
            delay = 90.0 / (bypassratio + 3.0);

            Throttle = 0.0;
            Starter = false;
            FuelExpended = 0.0;
            Cranking = false;
            PctPower = 0.0;
            TrimMode = false;
            FuelFlow_pph = 0.0;
            FuelFreeze = false;
            OilTemp_degK = -491.69 * 0.5555556 + 273.0;
            InjectionTime = 0;
            MaxExhaustTemperature = 4100;
            OvertempTimer = 0;
            N2norm = 0;
        }

        #region Thrust calculations
        private double IdleThrust()
        {
            return IdleScalar * MathsHelper.TableLookup2D(Mach, Altitude, IdleColumnValues, IdleRowValues, IdleDataValues);
        }

        private double MilThrust()
        {
            return MilScalar * MathsHelper.TableLookup2D(Mach, Altitude, MilColumnValues, MilRowValues, MilDataValues);
        }

        private double MaxThrust()
        {
            return AugScalar * MathsHelper.TableLookup2D(Mach, Altitude, AugColumnValues, AugRowValues, AugDataValues);
        }

        private double InjectionThrust()
        {
            return InjScalar * MathsHelper.TableLookup2D(Mach, Altitude, InjColumnValues, InjRowValues, InjDataValues);
        }
        #endregion

        #region State handlers
        private double Off(float dt, double qbar)
        {
            Running = false;
            FuelFlow_pph = MathsHelper.Seek(ref FuelFlow_pph, 0, 1000.0, 10000.0, dt);
            N1 = MathsHelper.Seek(ref N1, qbar / 10.0, N1 / 2.0, N1 / 2.0, dt);
            N2 = MathsHelper.Seek(ref N2, qbar / 15.0, N2 / 2.0, N2 / 2.0, dt);
            EGT_degC = MathsHelper.Seek(ref EGT_degC, TAT, 11.7, 7.3, dt);
            OilTemp_degK = MathsHelper.Seek(ref OilTemp_degK, TAT + 273.0, 0.2, 0.2, dt);
            OilPressure_psi = N2 * 0.62;
            NozzlePosition = MathsHelper.Seek(ref NozzlePosition, 1.0, 0.8, 0.8, dt);
            EPR = MathsHelper.Seek(ref EPR, 1.0, 0.2, 0.2, dt);
            Augmentation = false;
            return 0.0;
        }

        private double Trim()
        {
            double idlethrust, milthrustl, thrust, tdiff;

            idlethrust = milthrust * IdleThrust();
            milthrustl = (milthrust - idlethrust) * MilThrust();
            N2 = idlen2 + ThrottlePos * N2_factor;
            N2norm = (N2 - idlen2) / N2_factor;
            thrust = (idlethrust + (milthrustl * N2norm * N2norm))
                  * (1.0 - BleedDemand);

            if (augmethod == 1)
            {
                if ((ThrottlePos > 0.99) && (N2 > 97.0)) { Augmentation = true; }
                else { Augmentation = false; }
            }

            if ((augmented) && Augmentation && (augmethod < 2))
            {
                thrust = maxthrust * MaxThrust();
            }

            if (augmethod == 2)
            {
                if (AugmentCmd > 0.0)
                {
                    tdiff = (maxthrust * MaxThrust()) - thrust;
                    thrust += (tdiff * AugmentCmd);
                }
            }

            if ((injected) && Injection)
            {
                thrust = thrust * InjectionThrust();
            }

            return thrust;
        }

        private double Stall(float dt, double qbar)
        {
            EGT_degC = TAT + 903.14;
            FuelFlow_pph = IdleFF;
            N1 = MathsHelper.Seek(ref N1, qbar / 10.0, 0, N1 / 10.0, dt);
            N2 = MathsHelper.Seek(ref N2, qbar / 15.0, 0, N2 / 10.0, dt);
            if (ThrottlePos < 0.01)
                phase = Phase.Run;        // clear the stall with throttle

            return 0.0;
        }

        private double SpinUp(EngineComponent comp, float dt)
        {
            Running = false;
            FuelFlow_pph = 0.0;

            if (!HasPower)              // Can't spin up without the starter motor having power
                return 0;

            if (N2 < 0.0001)
            {
                //inputDeviceManager.Core.World.World.Instance.noise.PlaySound2D(comp.BaseSoundID, "EngineCrank", true);
                PlayingSpinup = false;
            }
            else if (N2 > 1)
            {
                if (!PlayingSpinup)
                {
                    //inputDeviceManager.Core.World.World.Instance.noise.StopSoundEffect(comp.BaseSoundID, "EngineCrank");
                    //StrategicAirCombat.Core.World.World.Instance.noise.PlaySound2D(comp.BaseSoundID, "EngineSpinup");
                    PlayingSpinup = true;
                }
            }

            N2 = MathsHelper.Seek(ref N2, 25.18, N2_spinup, N2 / 2.0, dt);
            N1 = MathsHelper.Seek(ref N1, 5.21, N1_spinup, N1 / 2.0, dt);
            EGT_degC = MathsHelper.Seek(ref EGT_degC, TAT, 11.7, 7.3, dt);
            OilPressure_psi = N2 * 0.62;
            OilTemp_degK = MathsHelper.Seek(ref OilTemp_degK, TAT + 273.0, 0.2, 0.2, dt);
            EPR = 1.0;
            NozzlePosition = 1.0;
            N2norm = (N2 - idlen2) / N2_factor;
            if (N2 >= 25.17)
            {
                Cutoff = false;
            }
            return 0.0;
        }

        private double Start(EngineComponent comp, float dt)
        {
            if ((N2 > 15.0) && !Starved)
            {                                       // minimum 15% N2 needed for start
                Cranking = true;                    // provided for sound effects signal
                if (N2 < idlen2)
                {
                    N2 = MathsHelper.Seek(ref N2, idlen2, 2.0, N2 / 2.0, dt);
                    N1 = MathsHelper.Seek(ref N1, idlen1, 1.4, N1 / 2.0, dt);
                    EGT_degC = MathsHelper.Seek(ref EGT_degC, TAT + 363.1, 21.3, 7.3, dt);
                    FuelFlow_pph = MathsHelper.Seek(ref FuelFlow_pph, IdleFF, 103.7, 103.7, dt);
                    OilPressure_psi = N2 * 0.62;
                }
                else
                {
                    //StrategicAirCombat.Core.World.World.Instance.noise.StopSoundEffect(comp.BaseSoundID, "EngineStart");
                    //StrategicAirCombat.Core.World.World.Instance.noise.PlaySound2D(comp.BaseSoundID, "EngineRunning", true);
                    phase = Phase.Run;
                    Running = true;
                    Starter = false;
                    Cranking = false;
                    PlayingSpinup = false;
                }
            }
            else
            {                 // no start if N2 < 15%
                phase = Phase.Off;
                Starter = false;
            }
            N2norm = (N2 - idlen2) / N2_factor;
            return 0.0;
        }

        private double Seize(float dt, double qbar)
        {
            N2 = 0.0;
            N1 = MathsHelper.Seek(ref N1, qbar / 20.0, 0, N1 / 15.0, dt);
            FuelFlow_pph = IdleFF;
            OilPressure_psi = 0.0;
            OilTemp_degK = MathsHelper.Seek(ref OilTemp_degK, TAT + 273.0, 0, 0.2, dt);
            Running = false;
            N2norm = 0.0;
            return 0.0;
        }

        private double Run(float dt, double T, double sigma)
        {
            double idlethrust, milthrustl, thrust;
            double spoolup;                        // acceleration in pct/sec

            idlethrust = milthrust * IdleThrust();
            milthrustl = (milthrust - idlethrust) * MilThrust();

            Running = true;
            Starter = false;

            // adjust acceleration for N2 and atmospheric density
            double n = N2norm + 0.1;
            if (n > 1)
                n = 1;

            spoolup = delay / (1 + 3 * (1 - n) * (1 - n) * (1 - n) + (1 - sigma));

            N2 = MathsHelper.Seek(ref N2, idlen2 + ThrottlePos * N2_factor, spoolup, spoolup * 3.0, dt);
            N1 = MathsHelper.Seek(ref N1, idlen1 + ThrottlePos * N1_factor, spoolup, spoolup * 2.4, dt);
            N2norm = (N2 - idlen2) / N2_factor;
            thrust = idlethrust + (milthrustl * N2norm * N2norm);

            EGT_degC = TAT + 363.1 + ThrottlePos * 357.1;
            OilPressure_psi = N2 * 0.62;
            OilTemp_degK = MathsHelper.Seek(ref OilTemp_degK, 366.0, 1.2, 0.1, dt);

            if (!Augmentation)
            {
                correctedTSFC = tsfc * Math.Sqrt(T / 389.7) * (0.84 + (1 - N2norm) * (1 - N2norm));

                FuelFlow_pph = MathsHelper.Seek(ref FuelFlow_pph, thrust * correctedTSFC, 1000.0, 100000, dt);
                if (FuelFlow_pph < IdleFF)
                    FuelFlow_pph = IdleFF;

                NozzlePosition = MathsHelper.Seek(ref NozzlePosition, 1.0 - N2norm, 0.8, 0.8, dt);
                thrust = thrust * (1.0 - BleedDemand);
                EPR = 1.0 + thrust / milthrust;
            }

            if (augmethod == 1)
            {
                if ((AugmentCmd > 0) && (N2 > 97.0)) { Augmentation = true; }
                else { Augmentation = false; }
            }

            if ((augmented) && Augmentation && (augmethod < 2))
            {
                thrust = MaxThrust() * maxthrust;
                FuelFlow_pph = MathsHelper.Seek(ref FuelFlow_pph, thrust * atsfc, 5000.0, 10000.0, dt);
                NozzlePosition = MathsHelper.Seek(ref NozzlePosition, 1.0, 0.8, 0.8, dt);
            }

            if (augmethod == 2)
            {
                if (AugmentCmd > 0.0)
                {
                    Augmentation = true;
                    double tdiff = (maxthrust * MaxThrust()) - thrust;
                    thrust += (tdiff * AugmentCmd);
                    FuelFlow_pph = MathsHelper.Seek(ref FuelFlow_pph, thrust * atsfc, 5000.0, 10000.0, dt);
                    NozzlePosition = MathsHelper.Seek(ref NozzlePosition, 1.0, 0.8, 0.8, dt);
                }
                else
                {
                    Augmentation = false;
                }
            }

            if ((injected) && (Injection))
            {
                InjectionTimer += dt;
                if (InjectionTimer < InjectionTime)
                {
                    thrust = thrust * InjectionThrust();
                }
                else
                {
                    Injection = false;
                }
            }

            if (Cutoff) phase = Phase.Off;
            if (Starved) phase = Phase.Off;

            return thrust;
        }

        #endregion


        /// <summary>
        ///  Reduces the fuel in the active tanks by the amount required.
        ///  This function should be called from within the
        /// removes fuel from the fuel tanks as appropriate, and sets the starved flag if necessary. 
        /// </summary>
        private void ConsumeFuel(EngineComponent comp, float dt)
        {
            if (FuelFreeze)
                return;
            if (TrimMode)
                return;
            if (phase == Phase.Off)
                return;

            int i;
            int TanksWithFuel;
            FuelTank _Tank;
            TanksWithFuel = 0;

            // count how many assigned tanks have fuel
            for (i = 0; i < comp.ConnectedTanks.Count; i++)
            {
                _Tank = comp.ConnectedTanks[i];
                if (_Tank.IsValid())
                {
                    TanksWithFuel++;
                }
            }
            if (TanksWithFuel == 0)
            {
                Starved = true;
                return;
            }

            float needed = CalcFuelNeed(dt);
            float request = (float)(needed / TanksWithFuel);
            for (i = 0; i < comp.ConnectedTanks.Count; i++)
            {
                _Tank = comp.ConnectedTanks[i];
                if (_Tank.IsValid())
                {
                    needed -= _Tank.BurnFuel(request);
                }
            }

            if (needed > 0)
            {
                // some of the tanks have run dry
                // see if we can get the fuel from another tank
                int ii = 0;
                while ((ii < comp.ConnectedTanks.Count) && (needed > 0))
                {
                    _Tank = comp.ConnectedTanks[ii];
                    if (_Tank.IsValid())
                    {
                        needed -= _Tank.BurnFuel(needed);
                    }
                    ii++;
                }

            }

            if (needed > 0.00)
                Starved = true;
            else
                Starved = false;
        }

        /// <summary>
        ///  fuel need is calculated based on power levels and flow rate for that power level. 
        ///  It is also turned from a rate into an actual amount (pounds) by multiplying it by the delta T and the rate.
        /// </summary>
        /// <returns>Total fuel requirement for this engine in pounds.</returns>
        private float CalcFuelNeed(float dt)
        {
            FuelFlowRate = FuelFlow_pph / 3600.0; // Calculates flow in lbs/sec from lbs/hr
            FuelExpended = FuelFlowRate * dt;     // Calculates fuel expended in this time step
            return (float)FuelExpended;
        }

        #region Public methods
        public override void Update(GameObject parent, EngineComponent comp, float dt)
        {
            double thrust;
            bool wasCranking = Cranking;

            HasPower = false;
            ECSGameComponent pm = parent.FindSingleComponentByType<PowerManagementComponent>();
            if (pm!=null)
            {
                PowerManagementComponent pmc = (PowerManagementComponent)pm;

                float requiredpower = 0;
                switch (phase)
                {
                    case Phase.SpinUp:
                    case Phase.Start:
                        requiredpower = (float)StarterCurrent;
                        break;
                }
                HasPower = pmc.HasPower(requiredpower);
            }

            TAT = (parent.auxStateData.TotalTemperature - 491.69) * 0.5555556;
            Mach = parent.auxStateData.Mach;
            Altitude = parent.auxStateData.AltitudeASL;

            ThrottlePos = Throttle = comp.GetThrottleSetting();
            if (ThrottlePos > 1.0)
            {
                AugmentCmd = ThrottlePos - 1.0;
                ThrottlePos -= AugmentCmd;
            }
            else
            {
                AugmentCmd = 0.0;
            }

            // When trimming is finished check if user wants engine OFF or RUNNING
            if ((phase == Phase.Trim) && (dt > 0))
            {
                if (Running && !Starved)
                {
                    phase = Phase.Run;
                    N2 = idlen2 + ThrottlePos * N2_factor;
                    N1 = idlen1 + ThrottlePos * N1_factor;
                    OilTemp_degK = 366.0;
                    Cutoff = false;
                }
                else
                {
                    phase = Phase.Off;
                    Cutoff = true;
                    EGT_degC = TAT;
                }
            }

            if (!Running && Cutoff && Starter)
            {
                if (phase == Phase.Off) phase = Phase.SpinUp;
            }
            if (!Running && !Cutoff && (N2 > 15.0) && (phase != Phase.Start))
            {
                //StrategicAirCombat.Core.World.World.Instance.noise.StopSoundEffect(comp.BaseSoundID, "EngineSpinup");
                //StrategicAirCombat.Core.World.World.Instance.noise.PlaySound2D(comp.BaseSoundID, "EngineStart",8905,10391);
                phase = Phase.Start;
            }
            if (Cutoff && (phase != Phase.SpinUp)) phase = Phase.Off;
            if (Starved) phase = Phase.Off;
            if (Stalled) phase = Phase.Stall;
            if (Seized) phase = Phase.Seize;

            switch (phase)
            {
                case Phase.Off:
                    {
                        thrust = Off(dt, parent.auxStateData.Qbar);
                    }
                    break;
                case Phase.Run:
                    {
                        thrust = Run(dt, parent.atmosphereAltitude.Temperature, parent.auxStateData.DensityRatio);
                    }
                    break;
                case Phase.SpinUp:
                    {
                        thrust = SpinUp(comp, dt);
                        if (!Cutoff)
                        {

                        }
                    }
                    break;
                case Phase.Start:
                    {
                        thrust = Start(comp, dt);
                    }
                    break;
                case Phase.Stall:
                    {
                        thrust = Stall(dt, parent.auxStateData.Qbar);
                    }
                    break;
                case Phase.Seize:
                    {
                        thrust = Seize(dt, parent.auxStateData.Qbar);
                    }
                    break;
                case Phase.Trim:
                    {
                        thrust = Trim();
                    }
                    break;
                default:
                    {
                        thrust = Off(dt, parent.auxStateData.Qbar);
                    }
                    break;
            }

            // if (Thruster != null)
            //     thrust = Thruster.Calculate(thrust); // allow thruster to modify thrust (i.e. reversing)
            output_thrust = thrust;
            PctPower = thrust / maxthrust;
            ConsumeFuel(comp, dt);

            if (EGT_degC >= MaxExhaustTemperature)
            {
                if (Overtemp)
                {
                    if (OvertempTimer > 10)
                    {
                        Fire = true;
                    }
                }
                else
                {
                    Overtemp = true;
                    OvertempTimer = dt;
                }
            }

        }

        /// <summary>
        /// Reads in all the data from a text file
        /// </summary>
        /// <param name="filename"></param>
        public override void SetDefinition(string filename)
        {
            char[] splits = new char[] { ' ' };
            String path = Settings.GetInstance().GameRootDirectory + @"\EngineDefinitions\" + filename;
            using (TextReader reader = File.OpenText(path))
            {
                String line;
                while ((line = reader.ReadLine()) != null)
                {
                    string[] parts = line.Split(splits, StringSplitOptions.RemoveEmptyEntries);
                    if (parts.Length > 0)
                    {
                        switch (parts[0])
                        {
                            case "generator":
                                GeneratorPower = double.Parse(parts[1]);
                                break;
                            case "startcurrent":
                                StarterCurrent = double.Parse(parts[1]);
                                break;
                            case "maxexhausttemp":
                                MaxExhaustTemperature = double.Parse(parts[1]);
                                break;
                            case "milthrust":
                                milthrust = double.Parse(parts[1]);
                                break;

                            case "maxthrust":
                                maxthrust = double.Parse(parts[1]);
                                break;

                            case "bypassratio":
                                bypassratio = double.Parse(parts[1]);
                                break;

                            case "tsfc":
                                tsfc = double.Parse(parts[1]);
                                break;

                            case "atsfc":
                                atsfc = double.Parse(parts[1]);
                                break;

                            case "idlen1":
                                idlen1 = double.Parse(parts[1]);
                                break;

                            case "idlen2":
                                idlen2 = double.Parse(parts[1]);
                                break;

                            case "maxn1":
                                maxn1 = double.Parse(parts[1]);
                                break;

                            case "maxn2":
                                maxn2 = double.Parse(parts[1]);
                                break;

                            case "augmented":
                                augmented = (parts[1] == "1");
                                break;

                            case "augmethod":
                                augmethod = int.Parse(parts[1]);
                                break;

                            case "injected":
                                injected = (parts[1] == "1");
                                break;

                            case "idle_scalar":
                                IdleScalar = double.Parse(parts[1]);
                                break;

                            case "mil_scalar":
                                MilScalar = double.Parse(parts[1]);
                                break;

                            case "aug_scalar":
                                AugScalar = double.Parse(parts[1]);
                                break;

                            case "idle_columns":
                                {
                                    int cols = int.Parse(parts[1]);
                                    IdleColumnValues = new double[cols];
                                    for (int i = 0; i < cols; i++)
                                    {
                                        IdleColumnValues[i] = double.Parse(parts[i + 2]);
                                    }
                                }
                                break;

                            case "idle_rows":
                                {
                                    int cols = int.Parse(parts[1]);
                                    IdleRowValues = new double[cols];
                                    for (int i = 0; i < cols; i++)
                                    {
                                        IdleRowValues[i] = double.Parse(parts[i + 2]);
                                    }
                                }
                                break;

                            case "idle_data":
                                {
                                    IdleDataValues = new double[IdleRowValues.Length, IdleColumnValues.Length];
                                    for (int i = 0; i < IdleRowValues.Length; i++)
                                    {
                                        line = reader.ReadLine();
                                        string[] dat = line.Split(splits, StringSplitOptions.RemoveEmptyEntries);
                                        for (int j = 0; j < IdleColumnValues.Length; j++)
                                        {
                                            IdleDataValues[i, j] = double.Parse(dat[j]);
                                        }
                                    }
                                }
                                break;

                            case "mil_columns":
                                {
                                    int cols = int.Parse(parts[1]);
                                    MilColumnValues = new double[cols];
                                    for (int i = 0; i < cols; i++)
                                    {
                                        MilColumnValues[i] = double.Parse(parts[i + 2]);
                                    }
                                }
                                break;

                            case "mil_rows":
                                {
                                    int cols = int.Parse(parts[1]);
                                    MilRowValues = new double[cols];
                                    for (int i = 0; i < cols; i++)
                                    {
                                        MilRowValues[i] = double.Parse(parts[i + 2]);
                                    }
                                }
                                break;

                            case "mil_data":
                                {
                                    MilDataValues = new double[MilRowValues.Length, MilColumnValues.Length];
                                    for (int i = 0; i < MilRowValues.Length; i++)
                                    {
                                        line = reader.ReadLine();
                                        string[] dat = line.Split(splits, StringSplitOptions.RemoveEmptyEntries);
                                        for (int j = 0; j < MilColumnValues.Length; j++)
                                        {
                                            MilDataValues[i, j] = double.Parse(dat[j]);
                                        }
                                    }
                                }
                                break;

                            case "aug_columns":
                                {
                                    int cols = int.Parse(parts[1]);
                                    AugColumnValues = new double[cols];
                                    for (int i = 0; i < cols; i++)
                                    {
                                        AugColumnValues[i] = double.Parse(parts[i + 2]);
                                    }
                                }
                                break;

                            case "aug_rows":
                                {
                                    int cols = int.Parse(parts[1]);
                                    AugRowValues = new double[cols];
                                    for (int i = 0; i < cols; i++)
                                    {
                                        AugRowValues[i] = double.Parse(parts[i + 2]);
                                    }
                                }
                                break;

                            case "aug_data":
                                {
                                    AugDataValues = new double[AugRowValues.Length, AugColumnValues.Length];
                                    for (int i = 0; i < AugRowValues.Length; i++)
                                    {
                                        line = reader.ReadLine();
                                        string[] dat = line.Split(splits, StringSplitOptions.RemoveEmptyEntries);
                                        for (int j = 0; j < AugColumnValues.Length; j++)
                                        {
                                            AugDataValues[i, j] = double.Parse(dat[j]);
                                        }
                                    }
                                }
                                break;
                        }
                    }
                }
            }
            IdleFF = Math.Pow(MathsHelper.TableLookup2D(0, 0, MilColumnValues, MilRowValues, MilDataValues), 0.2) * 107.0;  // just an estimate
        }

        public override void HandleEvent(string evt)
        {
            switch (evt)
            {
                case "EngineStart":
                    {
                        Starter = true;
                    }
                    break;
            }
        }

        public override float GetThrust()
        {
            return (float)output_thrust;
        }

        public override float GetPowerProduced()
        {
            return (float)(N2norm * GeneratorPower);
        }
        #endregion



    }
}
