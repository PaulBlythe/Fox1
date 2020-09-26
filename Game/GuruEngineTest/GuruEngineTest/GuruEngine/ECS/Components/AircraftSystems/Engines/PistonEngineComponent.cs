using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

using GuruEngine.World;
using GuruEngine.Maths;
using GuruEngine.ECS.Components.AircraftSystems.General;
using GuruEngine.ECS.Components.AircraftSystems.Thrusters;

//( Class PistonEngineComponent )
//( Group Flight )
//( Type PistonEngine )
//( ConnectionList FuelTank Tanks )
//( Connection AircraftStateComponent State )
//( Connection PropellerComponent Prop )
//( Parameter Float MinManifoldPressure_inHg )
//( Parameter Float MaxManifoldPressure_inHg )  
//( Parameter Float MaxManifoldPressure_Percent )
//( Parameter Float Displacement )               
//( Parameter Float displacement_SI )            
//( Parameter Float MaxHP )                      
//( Parameter Float SparkFailDrop )              
//( Parameter Float Cycles )             
//( Parameter Float IdleRPM )                    
//( Parameter Float MaxRPM )                   
//( Parameter Float Bore )                       
//( Parameter Float Stroke )                     
//( Parameter Float Cylinders )            
//( Parameter Float CompressionRatio )           
//( Parameter Float StarterHP )
//( Parameter Int BoostSpeeds )
//( Parameter Int BoostSpeed )    
//( Parameter Bool Boosted )
//( Parameter Bool BoostOverride )
//( Parameter Bool TakeoffBoost )  
//( Parameter Float TakeoffBoostPSI )
//( Parameter ArrayDouble RatedBoost )
//( Parameter ArrayDouble RatedAltitude )
//( Parameter ArrayDouble RatedRPM )
//( Parameter ArrayDouble RatedPower )
//( Parameter ArrayDouble BoostSwitchAltitude )
//( Parameter ArrayDouble BoostSwitchPressure )
//( Parameter ArrayDouble BoostMul )
//( Parameter ArrayDouble RatedMAP )
//( Parameter ArrayDouble TakeoffMAP )
//( Parameter Float BoostSwitchHysteresis )
//( Parameter Float minMAP )
//( Parameter Float maxMAP )
//( Parameter Float MAP )
//( Parameter Float TMAP )
//( Parameter Float ISFC )
//( Parameter Float Oil_Press_Relief_Valve )
//( Parameter Float Oil_Press_RPM_Max_Ratio )
//( Parameter Float Design_Oil_Temp )
//( Parameter Float Oil_Cool_Efficiency )
//( Parameter Float VolumetricEfficiency )
//( Parameter Int EngineNumber )
//( Parameter ArrayDouble ICE_Index )
//( Parameter ArrayDouble ICE_Values )
//( Parameter ArrayDouble MCE_Index )
//( Parameter ArrayDouble MCE_Vlaues )


namespace GuruEngine.ECS.Components.AircraftSystems.Engines
{

    public class PistonEngineComponent : ECSGameComponent
    {
        #region Configuration

        public double MinManifoldPressure_inHg;     // Inches Hg                                Merlin 12
        public double MaxManifoldPressure_inHg;     // Inches Hg                                Merlin 29.9
        public double MaxManifoldPressure_Percent;  // MaxManifoldPressure / 29.92
        public double Displacement;                 // cubic inches                             Merlin 1650
        public double displacement_SI;              // cubic meters
        public double MaxHP;                        // horsepower                               Merlin 1700
        public double SparkFailDrop;                // drop of power due to spark failure       Merlin 0.9
        public double Cycles;                       // cycles/power stroke                      Merlin 4
        public double IdleRPM;                      // revolutions per minute                   Merlin 800
        public double MaxRPM;                       // revolutions per minute                   Merlin 3000
        public double Bore;                         // inches                                   Merlin 5.4
        public double Stroke;                       // inches                                   Merlin 6.0
        public double Cylinders;                    // number                                   Merlin 12
        public double CompressionRatio;             // number                                   Merlin 6

        public double StarterHP;                    // initial horsepower of starter motor
        public int BoostSpeeds;	                    // Number of super/turbocharger boost speeds - zero implies no turbo/supercharging.                           Merlin 2
        public int BoostSpeed;	                    // The current boost-speed (zero-based).
        public bool Boosted;		                // Set true for boosted engine.
        public bool BoostOverride;	                // Set true if pilot override of the boost regulator was fitted. (Typically called 'war emergency power').    Merlin false
        public bool TakeoffBoost;	                // Set true if extra takeoff / emergency boost above rated boost could be attained. (Typically by extra throttle movement past a mechanical 'gate').  Merlin 45
        public double TakeoffBoostPSI;	            // Sea-level takeoff boost in psi. (if fitted).

        public double[] RatedBoost = new double[3];	                // Sea-level rated boost in psi.                    Merlin [37,37]
        public double[] RatedAltitude = new double[3];	            // Altitude at which full boost is reached (boost regulation ends) and at which power starts to fall with altitude [ft]. Merlin [10500,17750]
        public double[] RatedRPM = new double[3];                   // Engine speed at which the rated power for each boost speed is delivered [rpm].   Merlin [3000,3000]
        public double[] RatedPower = new double[3];	                // Power at rated throttle position at rated altitude [HP].         Merlin [1700,1555]
        public double[] BoostSwitchAltitude = new double[2];	    // Altitude at which switchover (currently assumed automatic) from one boost speed to next occurs [ft].
        public double[] BoostSwitchPressure = new double[2];        // Pressure at which boost speed switchover occurs [Pa]
        public double[] BoostMul = new double[3];	                // Pressure multipier of unregulated supercharger
        public double[] RatedMAP = new double[3];	                // Rated manifold absolute pressure [Pa] (BCV clamp)
        public double[] TakeoffMAP = new double[3];	                // Takeoff setting manifold absolute pressure [Pa] (BCV clamp)
        public double BoostSwitchHysteresis;	                    // Pa.

        public double minMAP;                       // Pa
        public double maxMAP;                       // Pa
        public double MAP;                          // Pa
        public double TMAP;                         // Pa - throttle manifold pressure e.g. before the supercharger boost
        public double ISFC;                         // Indicated specific fuel consumption lbs/horsepower*hour

        public double Oil_Press_Relief_Valve = 60;             
        public double Oil_Press_RPM_Max_Ratio = 0.75;       
        public double Design_Oil_Temp = 358;
        public double Oil_Cool_Efficiency = 0.667;

        public int EngineNumber = 1;

        public double[] ICE_Index;
        public double[] ICE_Values;
        public double[] MCE_Index;
        public double[] MCE_Values;

        #endregion

        #region Working variables
        double ThrottleAngle;
        int crank_counter;

        double IndicatedHorsePower;
        double PMEP;
        double FMEP;
        //double SpeedSlope;
        //double SpeedIntercept;
        //double AltitudeSlope;
        double PowerAvailable;

        bool Starter = false;
        bool Starved;
        bool Running;
        bool Cranking;
        bool TrimMode = false;
        bool FuelFreeze = false;
        double p_amb;                    // Pascals
        double T_amb;                    // degrees Kelvin

        double FuelFlow_pph;
        double FuelFlowRate;
        double FuelFlow_gph;
        double dt;

        String RPM_descriptor;
        String Throttle_descriptor;
        String Magnetos_descriptor;
        String Mixture_descriptor;

        #endregion

        #region Outputs

        double rho_air;
        double volumetric_efficiency = 0.85;                               // Merlin 0.435
        double map_coefficient;
        double m_dot_air;
        double equivalence_ratio;
        double m_dot_fuel;
        double HP;
        double combustion_efficiency;
        double ExhaustGasTemp_degK;
        double EGT_degC;
        double ManifoldPressure_inHg;
        double CylinderHeadTemp_degK;
        double OilPressure_psi;
        double OilTemp_degK;
        double MeanPistonSpeed_fps;
        double PctPower;
        #endregion


        List<FuelTank> Tanks = new List<FuelTank>();
        AircraftStateComponent State = null;
        PropellerComponent Prop = null;

        #region ECS Game component methods
        public override ECSGameComponent Clone()
        {
            PistonEngineComponent other = new PistonEngineComponent();
            other.Tanks = Tanks;
            other.State = State;
            other.Prop = Prop;
            return other;
        }

        public override void Connect(string components, bool isList)
        {
            string[] parts = components.Split('#');
            if (isList)
            {
                switch (parts[1])
                {
                    case "Tanks":
                        {
                            string[] mfds = parts[1].Split(',');
                            for (int i = 0; i < mfds.Length; i++)
                            {
                                string[] objs = mfds[i].Split(':');
                                if (objs.Length == 2)
                                {
                                    Tanks.Add((FuelTank)Parent.FindGameComponentByName(objs[0]));
                                }
                            }
                        }
                        break;
                    default:
                        throw new Exception("GameComponent::PistonEngineComponent:: Unknown list connection request to " + parts[0]);
                }
            }
            else
            {
                string[] objects = parts[2].Split(':');
                switch (parts[0])
                {
                    case "State":
                        {
                            State = (AircraftStateComponent)Parent.FindGameComponentByName(objects[0]);
                        }
                        break;
                    case "Prop":
                        {
                            Prop = (PropellerComponent)Parent.FindGameComponentByName(objects[0]);
                        }
                        break;

                    case "Root":
                        {
                            Parent = GameObjectManager.Instance.FindGameObjectByName(objects[0]);
                        }
                        break;

                    default:
                        throw new Exception("GameComponent::PistonEngineComponent:: Unknown direct connection request to " + parts[0]);
                }
            }
        }

        public override void DisConnect()
        {
            State = null;
            Tanks.Clear();
            Prop = null;
        }

        public override void ReplaceComponent(ECSGameComponent old, ECSGameComponent replacement)
        {
            if (old is AircraftStateComponent)
            {
                State = (AircraftStateComponent)replacement;
            }
        }

        public override object GetContainedObject(string type)
        {
            return null;
        }

        public override Texture2D GetOffscreenTexture()
        {
            return null;
        }

        public override void HandleEvent(string evt)
        {

        }

        public override void Load(ContentManager content)
        {
            // Setup state variables these are all IO variable which need to be visible to all things in the aircraft
            RPM_descriptor = "Engine" + EngineNumber.ToString() + "RPM";
            State.DoubleVariables.Add(RPM_descriptor, 0);

            Throttle_descriptor = "Engine" + EngineNumber.ToString() + "ThrottlePCT";
            State.DoubleVariables.Add(Throttle_descriptor, 0);

            Mixture_descriptor = "Engine" + EngineNumber.ToString() + "Mixture";
            State.DoubleVariables.Add(Mixture_descriptor, 0);

            Magnetos_descriptor = "Engine" + EngineNumber.ToString() + "Magnetos";
            State.BoolVariables.Add(Magnetos_descriptor, false);

            
        }

        public override void ReConnect(GameObject other)
        {
            PistonEngineComponent otherTank = (PistonEngineComponent)other.FindGameComponentByName(Name);
            foreach (ECSGameComponent gc in Tanks)
            {
                ECSGameComponent ogc = other.FindGameComponentByName(gc.Name);
                otherTank.Tanks.Add((FuelTank)ogc);
            }
            if (State != null)
                otherTank.State = (AircraftStateComponent)other.FindGameComponentByName(State.Name);
            if (Prop != null)
                otherTank.Prop = (PropellerComponent)other.FindGameComponentByName(Prop.Name);
        }

        public override void RenderOffscreenRenderTargets()
        {
        }

        public override void SetParameter(string Name, string Value)
        {
            switch (Name)
            {
                case "ICE_Index":
                    {
                        string[] parts = Value.Split(':');
                        ICE_Index = new double[parts.Length];
                        for (int i = 0; i < parts.Length; i++)
                        {
                            ICE_Index[i] = double.Parse(parts[i]);
                        }
                    }
                    break;
                case "ICE_Values":
                    {
                        string[] parts = Value.Split(':');
                        ICE_Values = new double[parts.Length];
                        for (int i = 0; i < parts.Length; i++)
                        {
                            ICE_Values[i] = double.Parse(parts[i]);
                        }
                    }
                    break;
                case "MCE_Index":
                    {
                        string[] parts = Value.Split(':');
                        MCE_Index = new double[parts.Length];
                        for (int i = 0; i < parts.Length; i++)
                        {
                            MCE_Index[i] = double.Parse(parts[i]);
                        }
                    }
                    break;
                case "MCE_Values":
                    {
                        string[] parts = Value.Split(':');
                        ICE_Values = new double[parts.Length];
                        for (int i = 0; i < parts.Length; i++)
                        {
                            ICE_Values[i] = double.Parse(parts[i]);
                        }
                    }
                    break;
                case "RatedBoost":
                    {
                        string[] parts = Value.Split(':');
                        for (int i=0; i<parts.Length; i++)
                        {
                            RatedBoost[i] = double.Parse(parts[i]);
                        }
                    }
                    break;
                case "RatedAltitude":
                    {
                        string[] parts = Value.Split(':');
                        for (int i = 0; i < parts.Length; i++)
                        {
                            RatedAltitude[i] = double.Parse(parts[i]);
                        }
                    }
                    break;
                case "RatedRPM":
                    {
                        string[] parts = Value.Split(':');
                        for (int i = 0; i < parts.Length; i++)
                        {
                            RatedRPM[i] = double.Parse(parts[i]);
                        }
                    }
                    break;
                case "RatedPower":
                    {
                        string[] parts = Value.Split(':');
                        for (int i = 0; i < parts.Length; i++)
                        {
                            RatedPower[i] = double.Parse(parts[i]);
                        }
                    }
                    break;
                case "BoostSwitchAltitude":
                    {
                        string[] parts = Value.Split(':');
                        for (int i = 0; i < parts.Length; i++)
                        {
                            BoostSwitchAltitude[i] = double.Parse(parts[i]);
                        }
                    }
                    break;
                case "BoostSwitchPressure":
                    {
                        string[] parts = Value.Split(':');
                        for (int i = 0; i < parts.Length; i++)
                        {
                            BoostSwitchPressure[i] = double.Parse(parts[i]);
                        }
                    }
                    break;
                case "BoostMul":
                    {
                        string[] parts = Value.Split(':');
                        for (int i = 0; i < parts.Length; i++)
                        {
                            BoostMul[i] = double.Parse(parts[i]);
                        }
                    }
                    break;
                case "RatedMAP":
                    {
                        string[] parts = Value.Split(':');
                        for (int i = 0; i < parts.Length; i++)
                        {
                            RatedMAP[i] = double.Parse(parts[i]);
                        }
                    }
                    break;
                case "TakeoffMAP":
                    {
                        string[] parts = Value.Split(':');
                        for (int i = 0; i < parts.Length; i++)
                        {
                            TakeoffMAP[i] = double.Parse(parts[i]);
                        }
                    }
                    break;
                case "MinManifoldPressure_inHg":
                    {
                        MinManifoldPressure_inHg = double.Parse(Value);
                    }
                    break;
                case "VolumetricEfficiency":
                    {
                        volumetric_efficiency = double.Parse(Value);
                    }
                    break;
                case "MaxManifoldPressure_inHg":
                    {
                        MaxManifoldPressure_inHg = double.Parse(Value);
                    }
                    break;
                case "MaxManifoldPressure_Percent":
                    {
                        MaxManifoldPressure_Percent = double.Parse(Value);
                    }
                    break;
                case "Displacement":
                    {
                        Displacement = double.Parse(Value);
                    }
                    break;
                case "displacement_SI":
                    {
                        displacement_SI = double.Parse(Value);
                    }
                    break;
                case "MaxHP":
                    {
                        MaxHP = double.Parse(Value);
                    }
                    break;
                case "SparkFailDrop":
                    {
                        SparkFailDrop = double.Parse(Value);
                    }
                    break;
                case "Cycles":
                    {
                        Cycles = double.Parse(Value);
                    }
                    break;
                case "IdleRPM":
                    {
                        IdleRPM = double.Parse(Value);
                    }
                    break;
                case "MaxRPM":
                    {
                        MaxRPM = double.Parse(Value);
                    }
                    break;
                case "Bore":
                    {
                        Bore = double.Parse(Value);
                    }
                    break;
                case "Stroke":
                    {
                        Stroke = double.Parse(Value);
                    }
                    break;
                case "Cylinders":
                    {
                        Cylinders = double.Parse(Value);
                    }
                    break;
                case "CompressionRatio":
                    {
                        CompressionRatio = double.Parse(Value);
                    }
                    break;
                case "BoostSwitchHysteresis":
                    {
                        BoostSwitchHysteresis = double.Parse(Value);
                    }
                    break;
                case "Oil_Press_Relief_Valve":
                    {
                        Oil_Press_Relief_Valve = double.Parse(Value);
                    }
                    break;

                case "Oil_Press_RPM_Max_Ratio":
                    {
                        Oil_Press_RPM_Max_Ratio = double.Parse(Value);
                    }
                    break;

                case "Design_Oil_Temp":
                    {
                        Design_Oil_Temp = double.Parse(Value);
                    }
                    break;
                case "Oil_Cool_Efficiency":
                    {
                        Oil_Cool_Efficiency = double.Parse(Value);
                    }
                    break;
                case "EngineNumber":
                    {
                        EngineNumber = int.Parse(Value);
                    }
                    break;
            }
        }

        public override void Update(float delt)
        {
            dt = delt;

            if (FuelFlow_gph > 0.0)
                ConsumeFuel();


            // calculate the throttle plate angle.  1 unit is approx pi/2 radians.
            ThrottleAngle = State.DoubleVariables[Throttle_descriptor];

            //
            // Input values.
            //

            p_amb = State.Atmosphere.Pressure * State.psftopa;
            T_amb = MathUtils.RankineToKelvin(State.Atmosphere.Temperature);

            State.DoubleVariables[RPM_descriptor] = Prop.GetRPM() * Prop.GetGearRatio();
            MeanPistonSpeed_fps = (State.DoubleVariables[RPM_descriptor] * Stroke) / (360);            // AKA 2 * (RPM/60) * ( Stroke / 12) or 2NS

            doEngineStartup();

            if (Boosted)
                doBoostControl();

            doMAP();
            doAirFlow();
            doFuelFlow();

            //Now that the fuel flow is done check if the mixture is too lean to run the engine
            //Assume lean limit at 22 AFR for now - thats a thi of 0.668
            //This might be a bit generous, but since there's currently no audiable warning of impending
            //cutout in the form of misfiring and/or rough running its probably reasonable for now.
            //  if (equivalence_ratio < 0.668)
            //    Running = false;

            doEnginePower();
            if (IndicatedHorsePower < 0.1250) Running = false;

            doEGT();
            doCHT();
            doOilTemperature();
            doOilPressure();

            PowerAvailable = (HP * State.hptoftlbssec) - Prop.GetPowerRequired();
        }
        #endregion

        #region Piston engine code
        /// <summary>
        ///  Calculate the oil pressure.
        ///
        /// Inputs: RPM, MaxRPM, OilTemp_degK
        ///
        /// Outputs: OilPressure_psi
        /// </summary>
        void doOilPressure()
        {
            double Oil_Press_RPM_Max = MaxRPM * Oil_Press_RPM_Max_Ratio;       
            double Oil_Viscosity_Index = 0.25;

            OilPressure_psi = (Oil_Press_Relief_Valve / Oil_Press_RPM_Max) * State.DoubleVariables[RPM_descriptor];

            if (OilPressure_psi >= Oil_Press_Relief_Valve)
            {
                OilPressure_psi = Oil_Press_Relief_Valve;
            }

            OilPressure_psi += (Design_Oil_Temp - OilTemp_degK) * Oil_Viscosity_Index * OilPressure_psi / Oil_Press_Relief_Valve;
        }

        /// <summary>
        ///  Calculate the oil temperature.
        ///
        /// Inputs: CylinderHeadTemp_degK, T_amb, OilPressure_psi.
        ///
        /// Outputs: OilTemp_degK
        /// </summary>
        void doOilTemperature()
        {
            double target_oil_temp;        // Steady state oil temp at the current engine conditions
            double time_constant;          // The time constant for the differential equation

            //  Target oil temp is interpolated between ambient temperature and Cylinder Head Tempurature
            //  target_oil_temp = ( T_amb * efficiency ) + (CylinderHeadTemp_degK *(1-efficiency)) ;
            target_oil_temp = CylinderHeadTemp_degK + Oil_Cool_Efficiency * (T_amb - CylinderHeadTemp_degK);

            if (OilPressure_psi > 5.0)
            {
                time_constant = 5000 / OilPressure_psi; // Guess at a time constant for circulated oil. The higher the pressure the faster it reaches target temperature.  
                                                        // Oil pressure should be about 60 PSI yielding a TC of about 80.
            }
            else
            {
                time_constant = 1000;  // Time constant for engine-off; reflects the fact that oil is no longer getting circulated
            }

            double dOilTempdt = (target_oil_temp - OilTemp_degK) / time_constant;

            OilTemp_degK += (dOilTempdt * dt);
        }

        /// <summary>
        ///  Calculate the cylinder head temperature.
        ///
        /// Inputs: T_amb, IAS, rho_air, m_dot_fuel, calorific_value_fuel,
        ///   combustion_efficiency, RPM, MaxRPM, Displacement
        ///
        /// Outputs: CylinderHeadTemp_degK
        /// </summary>
        void doCHT()
        {
            double h1 = -95.0;
            double h2 = -3.95;
            double h3 = -140.0; // -0.05 * 2800 (default maxrpm)

            double arbitary_area = 1.0;
            double CpCylinderHead = 800.0;
            double MassCylinderHead = 8.0;

            double temperature_difference = CylinderHeadTemp_degK - T_amb;
            double v_apparent = State.IndicatedAirSpeed * 0.5144444;
            double v_dot_cooling_air = arbitary_area * v_apparent;
            double m_dot_cooling_air = v_dot_cooling_air * rho_air;
            double dqdt_from_combustion = m_dot_fuel * State.FuelCalorificValue * combustion_efficiency * 0.33;
            double dqdt_forced = (h2 * m_dot_cooling_air * temperature_difference) + (h3 * State.DoubleVariables[RPM_descriptor] * temperature_difference / MaxRPM);
            double dqdt_free = h1 * temperature_difference;
            double dqdt_cylinder_head = dqdt_from_combustion + dqdt_forced + dqdt_free;

            double HeatCapacityCylinderHead = CpCylinderHead * MassCylinderHead;

            CylinderHeadTemp_degK += (dqdt_cylinder_head / HeatCapacityCylinderHead) * dt;
        }

        /// <summary>
        /// Calculate the exhaust gas temperature.
        ///
        /// Inputs: equivalence_ratio, m_dot_fuel, calorific_value_fuel,
        ///   Cp_air, m_dot_air, Cp_fuel, m_dot_fuel, T_amb, PctPower
        ///
        /// Outputs: combustion_efficiency, ExhaustGasTemp_degK
        /// </summary>
        void doEGT()
        {
            double delta_T_exhaust;
            double enthalpy_exhaust;
            double heat_capacity_exhaust;
            double dEGTdt;

            if ((Running) && (m_dot_air > 0.0))
            {  // do the energy balance
                combustion_efficiency = Lookup_Combustion_Efficiency(equivalence_ratio);
                enthalpy_exhaust = m_dot_fuel * State.FuelCalorificValue * combustion_efficiency * 0.33;
                heat_capacity_exhaust = (State.Cp_air * m_dot_air) + (State.Cp_fuel * m_dot_fuel);
                delta_T_exhaust = enthalpy_exhaust / heat_capacity_exhaust;
                ExhaustGasTemp_degK = State.AmbientAirTemperature + delta_T_exhaust;
                ExhaustGasTemp_degK *= 0.444 + ((0.544 - 0.444) * PctPower);
            }
            else
            {  // Drop towards ambient - guess an appropriate time constant for now
                combustion_efficiency = 0;
                dEGTdt = (MathUtils.RankineToKelvin(T_amb) - ExhaustGasTemp_degK) / 100.0;
                delta_T_exhaust = dEGTdt * dt;
                ExhaustGasTemp_degK += delta_T_exhaust;
            }
        }


        /// <summary>
        ///  Calculate the power produced by the engine.
        ///
        /// Currently, the propellor model does not allow the
        /// engine to produce enough RPMs to get up to a high horsepower.
        /// When tested with sufficient RPM, it has no trouble reaching
        /// 200HP.
        ///
        /// Inputs: ManifoldPressure_inHg, p_amb, RPM, T_amb,
        ///   Mixture_Efficiency_Correlation, Cycles, MaxHP, PMEP,
        ///
        /// Outputs: PctPower, HP
        /// </summary>
        void doEnginePower()
        {
            IndicatedHorsePower = 0;
            FMEP = 0;
            if (Running)
            {

                double ME, percent_RPM, power;  // Convienience term for use in the calculations
                ME = Mixture_Efficiency_Correlation(m_dot_fuel / m_dot_air);

                percent_RPM = State.DoubleVariables[RPM_descriptor] / MaxRPM;
                // Guestimate engine friction as a percentage of rated HP + a percentage of rpm + a percentage of Indicted HP
                //    friction = 1 - (percent_RPM * percent_RPM * percent_RPM/10);
                FMEP = (-18400 * MeanPistonSpeed_fps * State.fttom - 46500);

                power = 1;

                if (!State.BoolVariables[Magnetos_descriptor]) power *= SparkFailDrop;


                IndicatedHorsePower = (FuelFlow_pph / ISFC) * ME * power;

            }
            else
            {
                // Power output when the engine is not running
                if (Cranking)
                {
                    if (State.DoubleVariables[RPM_descriptor] < 10)
                    {
                        IndicatedHorsePower = StarterHP;
                    }
                    else if (State.DoubleVariables[RPM_descriptor] < IdleRPM * 0.8)
                    {
                        IndicatedHorsePower = StarterHP + ((IdleRPM * 0.8 - State.DoubleVariables[RPM_descriptor]) / 8.0);
                        // This is a guess - would be nice to find a proper starter moter torque curve
                    }
                    else
                    {
                        IndicatedHorsePower = StarterHP;
                    }
                }
            }

            // Constant is (1/2) * 60 * 745.7  (1/2) convert cycles, 60 minutes to seconds, 745.7 watts to hp.
            double pumping_hp = ((PMEP + FMEP) * displacement_SI * State.DoubleVariables[RPM_descriptor]) / (Cycles * 22371);

            HP = IndicatedHorsePower + pumping_hp - 1.5; //FIXME 1.5 static friction should depend on oil temp and configuration

            PctPower = HP / MaxHP;

        }

        /// <summary>
        /// Calculate the fuel flow into the engine.
        ///
        /// Inputs: Mixture, thi_sea_level, p_amb, m_dot_air
        ///
        /// Outputs: equivalence_ratio, m_dot_fuel
        /// </summary>
        void doFuelFlow()
        {
            double thi_sea_level = 1.3 * State.DoubleVariables[Mixture_descriptor];                   // Allows an AFR of infinity:1 to 11.3075:1
            equivalence_ratio = thi_sea_level * 101325.0 / p_amb;

            //  double AFR = 10+(12*(1-Mixture));
            //  mixture 10:1 to 22:1
            //  m_dot_fuel = m_dot_air / AFR;

            m_dot_fuel = (m_dot_air * equivalence_ratio) / 14.7;
            FuelFlowRate = m_dot_fuel * 2.2046;                     // kg to lb
            FuelFlow_pph = FuelFlowRate * 3600;                     // seconds to hours
            FuelFlow_gph = FuelFlow_pph / 6.0;                      // Assumes 6 lbs / gallon
        }

        /// <summary>
        /// 
        /// Calculate the air flow through the engine.
        /// Also calculates ambient air density
        /// (used in CHT calculation for air-cooled engines).
        ///
        /// Inputs: p_amb, R_air, T_amb, MAP, Displacement,
        ///   RPM, volumetric_efficiency, ThrottleAngle
        ///
        /// TODO: Model inlet manifold air temperature.
        ///
        /// Outputs: rho_air, m_dot_air
        /// </summary>
        void doAirFlow()
        {
            double gamma = 1.4; // specific heat constants

            // loss of volumentric efficiency due to difference between MAP and exhaust pressure
            double ve = ((gamma - 1) / gamma) + (CompressionRatio - (p_amb / MAP)) / (gamma * (CompressionRatio - 1));

            rho_air = p_amb / (State.R_air * T_amb);
            double swept_volume = (displacement_SI * (State.DoubleVariables[RPM_descriptor] / 60)) / 2;
            double v_dot_air = swept_volume * volumetric_efficiency * ve;

            double rho_air_manifold = MAP / (State.R_air * T_amb);
            m_dot_air = v_dot_air * rho_air_manifold;
        }

        /// <summary>
        /// 
        /// Calculate the manifold absolute pressure (MAP) in inches hg
        ///
        /// This function calculates manifold absolute pressure (MAP)
        /// from the throttle position, turbo/supercharger boost control
        /// system, engine speed and local ambient air density.
        ///
        /// Inputs: p_amb, Throttle, ThrottleAngle,
        ///         MeanPistonSpeed_fps, dt
        ///
        /// Outputs: MAP, ManifoldPressure_inHg, TMAP
        ///
        /// </summary>
        void doMAP()
        {
            // estimate throttle plate area.
            double throttle_area = ThrottleAngle * ThrottleAngle;

            // Internal Combustion Engine in Theory and Practice, Volume 2.  Charles Fayette Taylor.  Revised Edition, 1985 fig 6-13
            map_coefficient = 1 - ((MeanPistonSpeed_fps * MeanPistonSpeed_fps) / (24978 * throttle_area));

            if (map_coefficient < 0.1) map_coefficient = 0.1;

            // Add a one second lag to manifold pressure changes
            double dMAP = (TMAP - p_amb * map_coefficient) * dt;
            TMAP -= dMAP;

            // Find the mean effective pressure required to achieve this manifold pressure
            // Fixme: determine the HP consumed by the supercharger

            PMEP = TMAP - p_amb; // Fixme: p_amb should be exhaust manifold pressure

            if (Boosted)
            {
                // If takeoff boost is fitted, we currently assume the following throttle map:
                // (In throttle % - actual input is 0 -> 1)
                // 99 / 100 - Takeoff boost
                // In real life, most planes would be fitted with a mechanical 'gate' between
                // the rated boost and takeoff boost positions.

                bool bTakeoffPos = false;
                if (TakeoffBoost)
                {
                    if (State.DoubleVariables[Throttle_descriptor] > 0.98)
                    {
                        bTakeoffPos = true;
                    }
                }
                // Boost the manifold pressure.
                double boost_factor = BoostMul[BoostSpeed] * State.DoubleVariables[RPM_descriptor] / RatedRPM[BoostSpeed];
                if (boost_factor < 1.0) boost_factor = 1.0;  // boost will never reduce the MAP
                MAP = TMAP * boost_factor;
                // Now clip the manifold pressure to BCV or Wastegate setting.
                if (bTakeoffPos)
                {
                    if (MAP > TakeoffMAP[BoostSpeed]) MAP = TakeoffMAP[BoostSpeed];
                }
                else
                {
                    if (MAP > RatedMAP[BoostSpeed]) MAP = RatedMAP[BoostSpeed];
                }
            }
            else
            {
                MAP = TMAP;
            }

            // And set the value in American units as well
            ManifoldPressure_inHg = MAP / State.inhgtopa;

        }

        /// <summary>
        /// 
        /// Calculate the Current Boost Speed
        ///
        /// This function calculates the current turbo/supercharger boost speed
        /// based on altitude and the (automatic) boost-speed control valve configuration.
        ///
        /// Inputs: p_amb, BoostSwitchPressure, BoostSwitchHysteresis
        ///
        /// Outputs: BoostSpeed
        ///
        /// </summary>
        void doBoostControl()
        {
            if (BoostSpeed < BoostSpeeds - 1)
            {
                // Check if we need to change to a higher boost speed
                if (p_amb < BoostSwitchPressure[BoostSpeed] - BoostSwitchHysteresis)
                {
                    BoostSpeed++;
                }
            }
            else if (BoostSpeed > 0)
            {
                // Check if we need to change to a lower boost speed
                if (p_amb > BoostSwitchPressure[BoostSpeed - 1] + BoostSwitchHysteresis)
                {
                    BoostSpeed--;
                }
            }
        }

        void doEngineStartup()
        {
            // Check parameters that may alter the operating state of the engine.
            // (spark, fuel, starter motor etc)
            bool spark;
            bool fuel;

            // Check for spark

            spark = State.script.CheckBooleanState("EngineHasSpark");

            // Assume we have fuel for now
            fuel = !Starved;

            // Check if we are turning the starter motor
            if (Cranking != Starter)
            {
                // This check saves .../cranking from getting updated every loop - they
                // only update when changed.
                Cranking = Starter;
                crank_counter = 0;
            }

            if (Cranking) crank_counter++;  //Check mode of engine operation

            if (!Running && spark && fuel)
            {  // start the engine if revs high enough
                if (Cranking)
                {
                    if ((State.DoubleVariables[RPM_descriptor] > IdleRPM * 0.8) && (crank_counter > 175)) // Add a little delay to startup on the starter
                        Running = true;                         
                }
                else
                {
                    if (State.DoubleVariables[RPM_descriptor] > IdleRPM * 0.8)                            // This allows us to in-air start when windmilling
                        Running = true;                        
                }
            }

            // Cut the engine *power* - Note: the engine may continue to
            // spin if the prop is in a moving airstream

            if (Running && (!spark || !fuel)) Running = false;

            // Check for stalling (RPM = 0).
            if (Running)
            {
                if (State.DoubleVariables[RPM_descriptor] == 0)
                {
                    Running = false;
                }
                else if ((State.DoubleVariables[RPM_descriptor] <= IdleRPM * 0.8) && (Cranking))
                {
                    Running = false;
                }
            }

        }

        public void ResetState()
        {
            ManifoldPressure_inHg = State.Atmosphere.Pressure * State.psftoinhg; // psf to in Hg
            MAP = State.Atmosphere.Pressure * State.psftopa;
            TMAP = MAP;
            double airTemperature_degK = MathUtils.RankineToKelvin(State.Atmosphere.Temperature);
            OilTemp_degK = airTemperature_degK;
            CylinderHeadTemp_degK = airTemperature_degK;
            ExhaustGasTemp_degK = airTemperature_degK;
            EGT_degC = ExhaustGasTemp_degK - 273;

            State.DoubleVariables[RPM_descriptor] = 0.0;
            OilPressure_psi = 0.0;
        }

        public double CalcFuelNeed()
        {
            double dT = WorldState.GetWorldTimestep();
            double FuelExpended = FuelFlowRate * dT;
            return FuelExpended;
        }


        /// <summary>
        ///  Reduces the fuel in the active tanks by the amount required.
        ///  This function should be called from within the
        /// removes fuel from the fuel tanks as appropriate, and sets the starved flag if necessary. 
        /// </summary>
        public void ConsumeFuel()
        {
            if (FuelFreeze)
                return;
            if (TrimMode)
                return;

            // count how many assigned tanks have fuel
            double fuelneeded = CalcFuelNeed();
            int tanksavailable = 0;
            foreach (FuelTank f in Tanks)
            {
                if (f.IsValid() && (f.HasFuel()))
                {
                    tanksavailable++;
                }
            }
            double fuelflowpertank = fuelneeded / (double)tanksavailable;

            while ((tanksavailable > 0) && (fuelneeded > 0))
            {
                foreach (FuelTank f in Tanks)
                {
                    if (f.IsValid() && f.HasFuel())
                    {
                        float fburnt = f.BurnFuel((float)fuelflowpertank);
                        if (fburnt ==0)
                        {
                            fuelneeded -= fuelflowpertank;
                        }
                        else
                        {
                            fuelneeded -= fburnt;
                            tanksavailable--;
                        }
                    }
                }
            }
            if (fuelneeded > 0.00)
                Starved = true;
            else
                Starved = false;
        }

        double Mixture_Efficiency_Correlation(double value)
        {
            return MathUtils.TableLookup1D(value, MCE_Index, MCE_Values);
        }

        double Lookup_Combustion_Efficiency(double value)
        {
            return MathUtils.TableLookup1D(value, ICE_Index, ICE_Values);
        }
        #endregion

        #region Piston engine methods
        public void SetStarter(bool value)
        {
            Starter = value;
        }

        public void SetTrimMode(bool value)
        {
            TrimMode = value;
        }
        #endregion
    }
}
