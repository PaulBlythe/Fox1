using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace GuruEngine.Simulation.Weapons.AAA
{
    public class WeaponAAA
    {
        public float AimMaxDistance;            // Metres
        public float FireDelay;                 // Seconds. Convert from rounds per minute by 1.0/(RPM/60)
        public float MuzzleVelocity;            // Metres per second
        public int Bullets;                     // Number of bullets fired per shot. IE. the number of barrels on the gun
        public int Sound;                       // Hash of sound effect to use
        public int Round;                       // Hash that identifies the round 


        public WeaponAAA(float aim, float delay, float velocity, int bullets, int sound, int roundtype)
        {
            AimMaxDistance = aim;
            FireDelay = delay;
            MuzzleVelocity = velocity;
            Bullets = bullets;
            Sound = sound;
            Round = roundtype;
        }
        public WeaponAAA()
        {

        }
    }
}
