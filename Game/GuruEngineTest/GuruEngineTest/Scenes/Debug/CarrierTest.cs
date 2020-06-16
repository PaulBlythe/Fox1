using System;
using System.Collections.Generic;
using GuruEngine.InputDevices;
using GuruEngine.World;
using GuruEngine.Simulation.Weapons.AAA;
using GuruEngine.Simulation.Weapons.Ammunition;
using GuruEngine.AI;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using GuruEngine.Rendering;
using GuruEngine.World.Items;
using GuruEngine.SceneManagement;

namespace GuruEngineTest.Scenes.Gebug
{
    public class CarrierTest : Scene
    {
        WorldState worldState;
        World gameWorld;

        public override void Init()
        {
            worldState = new WorldState();
            gameWorld = new World(@"Debug/CarrierTest.txt");

        }

        public override void Update(float dt)
        {
            if (worldState == null)
                return;

            worldState.Update(dt);
            gameWorld.Update(dt);
        }

        public override void Load(ContentManager Content)
        {
            gameWorld.Initialise(Content);

            switch (Renderer.GetSkyType())
            {
                case Skies.Traced:
                    {
                        WorldItem sky = new TracedSky();
                        worldState.AddWorldItem(sky);
                    }
                    break;
                default:
                    {
                        WorldItem sky = new Sky();
                        worldState.AddWorldItem(sky);
                    }
                    break;
            }
            WorldItem Stars = new Stars();
            worldState.AddWorldItem(Stars);

            WorldItem Planets = new Planets();
            worldState.AddWorldItem(Planets);

            WorldItem Moon = new Moon();
            worldState.AddWorldItem(Moon);
        }


        public override void Draw(GameTime gt)
        {
            Renderer.GetCurrentRenderer().Draw(worldState, gt);
        }
    }
}
