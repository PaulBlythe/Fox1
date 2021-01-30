using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GuruEngine.ECS.Components;
using GuruEngine.ECS.Components.Artillery;
using Microsoft.Xna.Framework;

namespace GuruEngineTest.Scenes.Developer.Helpers
{
    public class AnimatedVar
    {
        public String Name;
        public float Min;
        public float Max;
        public float Value;
        public bool Tracked = false;
        public Rectangle Region;
        public AircraftStateComponent astate = null;
        public AntiAircraftArtilleryComponent aaastate = null;
        public ArtilleryComponent arty = null;

        public bool Over(int x, int y)
        {
            return Region.Contains(x, y);
        }

        public float ScaleValue()
        {
            float range = Max - Min;
            float sv = (Value - Min) / range;
            return sv;
        }

        public void SetValueScaled(float x)
        {
            float range = Max - Min;
            Value = (x * range) + Min;
        }

        public void Broadcast()
        {
            if (astate!=null)
            {
                astate.DoubleVariables[Name] = Value;
            }
            if (aaastate != null)
            {
                aaastate.DoubleVariables[Name] = Value;
            }
            if (arty !=null)
            {
                arty.DoubleVariables[Name] = Value;
            }
        }

    }
}
