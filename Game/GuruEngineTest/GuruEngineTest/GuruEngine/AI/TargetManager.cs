using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

using GuruEngine.World;

namespace GuruEngine.AI
{
    public class TargetManager
    {
        public static TargetManager Instance;

        public List<AITarget> activeTargets = new List<AITarget>();


        public TargetManager()
        {
            Instance = this;
        }

        public static int AddTarget(int IFF, int type, Vector3 position, Vector3 velocity)
        {
            AITarget ai = new AITarget();
            ai.IFF = IFF;
            ai.OldPosition = position;
            ai.Position = position;
            ai.Velocity = velocity;
            ai.OldVelocity = velocity;
            Instance.activeTargets.Add(ai);
            return Instance.activeTargets.Count - 1;
        }

        public static void UpdateTarget(int ID, Vector3 position, Vector3 velocity)
        {
            Instance.activeTargets[ID].OldPosition = Instance.activeTargets[ID].Position;
            Instance.activeTargets[ID].Position = position;

            Instance.activeTargets[ID].OldVelocity = Instance.activeTargets[ID].Velocity;
            Instance.activeTargets[ID].Velocity = velocity;
        }

        public static bool AreAnyTargetsNearby(int IFF, int Type, Vector3 Position, float range)
        {
            return true;
        }

        public static List<int> GetNearbyTargets(int IFF, int Type, Vector3 Position)
        {
            List<int> Results = new List<int>();

            // DEBUG CODE
            Results.Add(0);

            return Results;
        }

        /// <summary>
        /// Get the position and velocity of a tracked target
        /// </summary>
        /// <param name="ID"></param>
        /// <returns></returns>
        public static AITarget GetAirborneTargetDetails(int ID)
        {        
            return Instance.activeTargets[ID];
        }

        public static bool IsTargetStillValid(int ID)
        {
            return true;
        }
    }
}
