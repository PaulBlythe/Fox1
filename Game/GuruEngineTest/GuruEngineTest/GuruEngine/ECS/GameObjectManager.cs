 using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

using GuruEngine.ECS.Components;
using GuruEngine.ECS.Components.Effects;
using GuruEngine.ECS.Components.World;
using GuruEngine.ECS.Components.AircraftSystems.General;
using GuruEngine.ECS.Components.Mesh;
using GuruEngine.ECS.Components.Input;
using GuruEngine.ECS.Components.Animators.Aircraft.Standard;
using GuruEngine.ECS.Components.Animators.Ships.Standard;
using GuruEngine.ECS.Components.Ships;
using GuruEngine.ECS.Components.AircraftSystems.American.Modern;
using GuruEngine.ECS.Components.Cockpit.American.Modern.F16;
using GuruEngine.ECS.Components.Weapons.Bullets;
using GuruEngine.ECS.Components.Debug;
using GuruEngine.ECS.Components.AircraftSystems.Aero;
using GuruEngine.ECS.Components.Physics;
using GuruEngine.ECS.Components.AircraftSystems.Thrusters;
using GuruEngine.ECS.Components.Settings;
using GuruEngine.ECS.Components.Cockpit;
using GuruEngine.ECS.Components.Artillery;
using GuruEngine.ECS.Components.Animators.Generic;

namespace GuruEngine.ECS
{
    public class GameObjectRecordParameter
    {
        public String Name;
        public String Type;
        public String Value;

        public GameObjectRecordParameter(String src)
        {
            string[] parts = src.Split(' ');
            Name = parts[0];
            Type = parts[1];
            Value = parts[2];
        }
    }

    public class GameObjectRecord
    {
        public String Name;
        public String Type;
        public int nDirectConnections;
        public List<String> DirectConnections = new List<string>();
        public int nListConnections;
        public List<String> ListConnections = new List<string>();
        public int nParameters;
        public List<GameObjectRecordParameter> Parameters = new List<GameObjectRecordParameter>();

        public GameObjectRecord(TextReader reader)
        {
            Name = reader.ReadLine();
            Type = reader.ReadLine();
            nDirectConnections = int.Parse(reader.ReadLine());
            for (int i=0; i<nDirectConnections; i++)
            {
                DirectConnections.Add(reader.ReadLine());
            }
            nListConnections = int.Parse(reader.ReadLine());
            for (int i = 0; i < nListConnections; i++)
            {
                ListConnections.Add(reader.ReadLine());
            }
            nParameters = int.Parse(reader.ReadLine());
            for (int i = 0; i < nParameters; i++)
            {
                Parameters.Add(new GameObjectRecordParameter(reader.ReadLine()));
            }
            
        }
    }

    public class GameObjectManager
    {
        public static GameObjectManager Instance;

        public Dictionary<String, GameObject> GameObjects = new Dictionary<string, GameObject>();
        public Dictionary<int, GameObject> ActiveGameObjects = new Dictionary<int, GameObject>();
        public List<GameObject> AirbourneTargets = new List<GameObject>();

        public GameObjectManager()
        {
            Instance = this;
        }

        public GameObject CreateInstance(String name)
        {
            GameObject result = new GameObject();
            GameObject src = GameObjects[name];

            result.Name = src.Name + ActiveGameObjects.Count.ToString();
            result.FullPath = src.FullPath;
            result.UID = ActiveGameObjects.Count;

            // Create all the components
            foreach(string s in src.Components.Keys)
            {
                ECSGameComponent gc = src.Components[s].Clone();
                gc.Parent = result;
                gc.Name = s;
                result.Components.Add(gc.Name, gc);
            }
            // Connect all the components
            foreach (string s in result.Components.Keys)
            {
                ECSGameComponent ngc = src.Components[s];
                ngc.ReConnect(result);
            }
            ActiveGameObjects.Add(result.UID, result);
            return result;
        }

        /// <summary>
        /// Create a game object at a location
        /// </summary>
        /// <param name="name"></param>
        /// <param name="position"></param>
        /// <returns></returns>
        public GameObject CreateInstanceAt(String name, Vector3 position)
        {
            GameObject result = CreateInstance(name);
            WorldTransform wt = (WorldTransform)result.FindGameComponentByName("WorldTransform_1");
            wt.LocalPosition = position;
            return result;
        }

        /// <summary>
        /// Load the game object from text file
        /// </summary>
        /// <param name="name"></param>
        public void LoadGameObject(String name)
        {
            List<GameObjectRecord> recs = new List<GameObjectRecord>();
            GameObject result = new GameObject();
        
            result.Name = Path.GetFileNameWithoutExtension(name);
            result.FullPath = Path.GetDirectoryName(name);

            using (TextReader tr = File.OpenText(name))
            {
                while (tr.Peek() != -1)
                {
                    recs.Add(new GameObjectRecord(tr));
                }
            }

            foreach (GameObjectRecord gor in recs)
            {
                ECSGameComponent newobj = null;

                switch (gor.Type)
                {
                    #region Aero
                    case "GearComponent":
                        newobj = new GearComponent();
                        break;
                    case "BallastComponent":
                        newobj = new BallastComponent();
                        break;
                    case "HStabComponent":
                        newobj = new HStabComponent();
                        break;
                    case "VStabComponent":
                        newobj = new VStabComponent();
                        break;
                    case "FlapComponent":
                        newobj = new FlapComponent();
                        break;
                    case "StallComponent":
                        newobj = new StallComponent();
                        break;
                    #endregion

                    #region Physics
                    case "DynamicPhysicsComponent":
                        newobj = new DynamicPhysicsComponent();
                        break;
                    case "PropellerComponent":
                        newobj = new PropellerComponent();
                        break;
                    #endregion

                    #region Artillery
                    case "ArtilleryComponent":
                        newobj = new ArtilleryComponent();
                        break;
                    case "AntiAircraftArtilleryComponent":
                        newobj = new AntiAircraftArtilleryComponent();
                        break;
                    case "FlakGunnerComponent":
                        newobj = new FlakGunnerComponent();
                        break;
                    case "ArtilleryGunnerComponent":
                        newobj = new ArtilleryGunnerComponent();
                        break;
                    #endregion

                    #region Animators
                    case "PitchAnimatorComponent":
                        newobj = new PitchAnimatorComponent();
                        break;
                    #endregion

                    case "DebugGunneryTargetDrone":
                        newobj = new DebugGunneryTargetDrone();
                        break;
                    case "BulletPropertiesComponent":
                        newobj = new BulletPropertiesComponent();
                        break;
                    case "TurretComponent":
                        newobj = new TurretComponent();
                        break;
                    case "ParticleEmitterComponent":
                        newobj = new ParticleEmitterComponent();
                        break;
                    case "AnimatedMeshComponent":
                        newobj = new AnimatedMeshComponent();
                        break;
                    case "RadarAnimatorComponent":
                        newobj = new RadarAnimatorComponent();
                        break;
                    case "ShipGunComponent":
                        newobj = new ShipGunComponent();
                        break;
                    case "ShipGunFOFComponent":
                        newobj = new ShipGunFOFComponent();
                        break;
                    case "ShipStateComponent":
                        newobj = new ShipStateComponent();
                        break;
                    case "SmoothedAngleAnimatorComponent":
                        newobj = new SmoothedAngleAnimatorComponent();
                        break;
                    case "TranslateAnimatorComponent":
                        newobj = new TranslateAnimatorComponent();
                        break;
                    case "CVTAnimatorComponent":
                        newobj = new CVTAnimatorComponent();
                        break;
                    case "FlapAnimatorComponent":
                        newobj = new FlapAnimatorComponent();
                        break;
                    case "AileronsAnimatorComponent":
                        newobj = new AileronsAnimatorComponent();
                        break;
                    case "RudderAnimatorComponent":
                        newobj = new RudderAnimatorComponent();
                        break;
                    case "ElevatorAnimatorComponent":
                        newobj = new ElevatorAnimatorComponent();
                        break;
                    case "AircraftComponent":
                        newobj = new AircraftComponent();
                        break;
                    case "PowerManagementComponent":
                        newobj = new PowerManagementComponent();
                        break;
                    case "CollisionMeshComponent":
                        newobj = new CollisionMeshComponent();
                        break;
                    case "InputEventComponent":
                        newobj = new InputEventComponent();
                        break;
                    case "FuelTankComponent":
                        newobj = new FuelTank();
                        break;

                    case "EngineComponent":
                        newobj = new EngineComponent();
                        break;
                    case "F16Hud":
                        newobj = new F16HudComponent();
                        break;

                    case "MultiMeshComponent":
                        newobj = new MultiMeshComponent();
                        break;

                    case "HookListComponent":
                        newobj = new HookListComponent();
                        break;

                    case "LODGroupComponent":
                        newobj = new LODGroupComponent();
                        break;

                    case "WorldTransform":
                        newobj = new WorldTransform();
                        break;

                    case "OnAPG68v5":
                        newobj = new OnAPG68v5();                       // Radar system APG 68 v5
                        break;

                    case "ColourMFD":
                        newobj = new ColourMFD();                       // Modern full colour MFD
                        break;

                    case "ElectronicECMComponent":                      // ECM system
                        newobj = new ElectronicECMComponent();
                        break;

                    case "RadarCrossSectionComponent":
                        newobj = new RadarCrossSectionComponent();
                        break;
                    case "PropellorAnimatorComponent":
                        newobj = new PropellorAnimatorComponent();
                        break;
                    case "AircraftStateComponent":
                        newobj = new AircraftStateComponent();
                        break;

                    case "WindsockComponent":
                        newobj = new WindsockComponent();
                        break;

                    case "AircraftSettingsComponent":
                        newobj = new AircraftSettingsComponent();
                        break;

                    case "NightMaterialListComponent":
                        newobj = new NightMaterialListComponent();
                        break;
                    case "CurveCvtAnimatorComponent":
                        newobj = new CurveCvtAnimatorComponent();
                        break;
                    case "MeshConditionalVisibilityComponent":
                        newobj = new MeshConditionalVisibilityComponent();
                        break;
                    case "CvtTranslateAnimatorComponent":
                        newobj = new CvtTranslateAnimatorComponent();
                        break;


                    case "OxygenSupplyComponent":
                        newobj = new OxygenSupplyComponent();
                        break;

                    default:
                        throw new Exception("Game component " + gor.Type + " Not added to the game object manager");
                }
                newobj.Name = gor.Name;
                result.Components.Add(gor.Name, newobj);
            }
            GameObjects.Add(result.Name, result);

            foreach (GameObjectRecord gor in recs)
            {
                for (int i=0; i<gor.nDirectConnections; i++)
                {
                    result.FindGameComponentByName(gor.Name).Connect(gor.DirectConnections[i], false);
                }
                for (int i = 0; i < gor.nListConnections; i++)
                {
                    result.FindGameComponentByName(gor.Name).Connect(gor.ListConnections[i], true);
                }
                for (int i = 0; i < gor.nParameters; i++)
                {
                    result.FindGameComponentByName(gor.Name).SetParameter(gor.Parameters[i].Name, gor.Parameters[i].Value);
                }
            }

        }
        
        public GameObject FindGameObjectByName(String name)
        {
            return GameObjects[name];
        }

        public GameObject FindActiveGameObjectByUID(int name)
        {
            return ActiveGameObjects[name];
        }

        public GameObject FindActiveGameObjectByName(String name)
        {
            foreach(int uid in ActiveGameObjects.Keys)
            {
                if (ActiveGameObjects[uid].Name == name)
                    return ActiveGameObjects[uid];
            }
            return null;
        }

        public void Destroy()
        {
            Parallel.ForEach(ActiveGameObjects.Keys, (s) =>
            {
                ActiveGameObjects[s].DestroyGameObject();
            });

            Parallel.ForEach(GameObjects.Keys, (s) =>
            {
                GameObjects[s].DestroyGameObject();
            });

            GameObjects.Clear();
            ActiveGameObjects.Clear();
        }

        public void AddGameObject(GameObject obj, String name)
        {
            ActiveGameObjects.Add(name.GetHashCode(), obj);
        }

        public void LoadContent(ContentManager content)
        {
            foreach (int s in ActiveGameObjects.Keys)
            {
                ActiveGameObjects[s].LoadContent(content);
            }
            
        }

        public void Update(int stage, float dt)
        {
            if (stage == 4)
            {
                foreach (int i in ActiveGameObjects.Keys)
                {
                    if (ActiveGameObjects[i].IsAirbourneTarget)
                    {
                        ActiveGameObjects[i].UpdatePhysicsState();
                    }
                }
            }
            //Parallel.ForEach(ActiveGameObjects.Keys, (s) =>
            foreach (int s in ActiveGameObjects.Keys)
            {
                ActiveGameObjects[s].Update(stage,dt);
            }//);
            if (stage == 4)
            {
                AirbourneTargets.Clear();
                foreach (int i in ActiveGameObjects.Keys)
                {
                    if (ActiveGameObjects[i].IsAirbourneTarget)
                        AirbourneTargets.Add(ActiveGameObjects[i]);
                }
            }
        }

        public void RenderOffscreenRenderTargets()
        {
            //Parallel.ForEach(ActiveGameObjects.Keys, (s) =>
            foreach (int s in ActiveGameObjects.Keys)
            {
                ActiveGameObjects[s].RenderOffscreenRenderTargets();
            }//);
        }
    }
}
