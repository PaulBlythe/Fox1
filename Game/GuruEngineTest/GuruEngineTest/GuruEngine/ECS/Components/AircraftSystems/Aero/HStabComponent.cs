using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

using GuruEngine.Physics.World;


//( Class HStabComponent )
//( Group Aero )
//( Type HStabComponent )
//( Connection FlapComponent Flap )
//( Connection StallComponent Stall )
//( Parameter Float Length )
//( Parameter Float Twist )
//( Parameter Float Chord )
//( Parameter Float Incidence )
//( Parameter Float Sweep )
//( Parameter Float Taper )
//( Parameter Float Dihedral )
//( Parameter Float Camber )
//( Parameter Float IDrag )
//( Parameter Float MinIncidence )
//( Parameter Float MaxIncidence )
//( Parameter Float SectionDrag )
//( Parameter Vector3 Position )
//( Parameter String Control )

namespace GuruEngine.ECS.Components.AircraftSystems.Aero
{
    /// <summary>
    /// Horizontal stabaliser
    /// </summary>
    public class HStabComponent : ECSGameComponent
    {
        public float Length;
        public float Twist;
        public float Chord;
        public float Incidence;
        public float Sweep = 0;
        public float Taper = 1;
        public float Dihedral = 0;
        public float Camber = 0;
        public float IDrag = 0.7f;
        public float Incidence_max;
        public float Incidence_min;
        public float SectionDrag;
        public String Control;

        public Vector3 Position;

        public FlapComponent Flap;
        public StallComponent Stall;

        public HStabComponent()
        {
            UpdateStage = 1;
        }

        #region ECS Game component methods
        public override ECSGameComponent Clone()
        {
            HStabComponent other = new HStabComponent();
            other.Length = Length;
            other.Twist = Twist;
            other.Chord = Chord;
            other.Incidence = Incidence;
            other.Sweep = Sweep;
            other.Taper = Taper;
            other.Dihedral = Dihedral;
            other.Camber = Camber;
            other.IDrag = IDrag;
            other.Incidence_max = Incidence_max;
            other.Incidence_min = Incidence_min;
            other.SectionDrag = SectionDrag;
            other.Position = Position;
            other.Flap = Flap;
            other.Stall = Stall;
            other.Control = Control;
            return other;
        }

        public override void Connect(string components, bool isList)
        {
            string[] parts = components.Split('#');
            if (isList)
            {

                        throw new Exception("GameComponent::HStabComponent:: Unknown list connection request to " + parts[0]);
                
            }
            else
            {
                string[] objects = parts[2].Split(':');
                switch (parts[0])
                {
                    case "Stall":
                        {
                            Stall = (StallComponent)Parent.FindGameComponentByName(objects[0]);
                        }
                        break;
                    case "Flap":
                        {
                            Flap = (FlapComponent) Parent.FindGameComponentByName(objects[0]);
                        }
                        break;
                    case "Root":
                        {
                            Parent = GameObjectManager.Instance.FindGameObjectByName(objects[0]);
                        }
                        break;

                    default:
                        throw new Exception("GameComponent::HStabComponent:: Unknown direct connection request to " + parts[0]);

                }
            }
        }

        public override void DisConnect()
        {

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

        }

        public override void ReConnect(GameObject tother)
        {
            HStabComponent other = (HStabComponent)tother.FindGameComponentByName(Name);
            other.Length = Length;
            other.Twist = Twist;
            other.Chord = Chord;
            other.Incidence = Incidence;
            other.Sweep = Sweep;
            other.Taper = Taper;
            other.Dihedral = Dihedral;
            other.Camber = Camber;
            other.IDrag = IDrag;
            other.Incidence_max = Incidence_max;
            other.Incidence_min = Incidence_min;
            other.SectionDrag = SectionDrag;
            other.Position = Position;
            other.Control = Control;
            other.Flap = (FlapComponent)tother.FindGameComponentByName(Flap.Name);
            other.Stall = (StallComponent)tother.FindGameComponentByName(Stall.Name);
        }

        public override void RenderOffscreenRenderTargets()
        {
        }

        public override void SetParameter(string Name, string Value)
        {
            switch (Name)
            {
                case "Control":
                    Control = Value;
                    break;
                case "Length":
                    Length = float.Parse(Value);
                    break;
                case "Twist":
                    Twist = (float)(Constants.degtorad * float.Parse(Value));
                    break;
                case "Chord":
                    Chord = float.Parse(Value);
                    break;
                case "Incidence":
                    Incidence = (float)(Constants.degtorad * float.Parse(Value));
                    break;
                case "Sweep":
                    Sweep = (float)(Constants.degtorad * float.Parse(Value));
                    break;
                case "Taper":
                    Taper = float.Parse(Value);
                    break;
                case "Dihedral":
                    Dihedral = (float)(Constants.degtorad * float.Parse(Value));
                    break;
                case "Camber":
                    Camber = float.Parse(Value);
                    break;
                case "IDrag":
                    IDrag = 0.7f * float.Parse(Value);
                    break;
                case "MinIncidence":
                    Incidence_min = (float)(Constants.degtorad * float.Parse(Value));
                    break;
                case "MaxIncidence":
                    Incidence_max = (float)(Constants.degtorad * float.Parse(Value));
                    break;
                case "SectionDrag":
                    SectionDrag = float.Parse(Value);
                    break;

                case "Position":
                    {
                        Position = new Vector3();
                        string[] parts = Value.Split(',');
                        Position.X = float.Parse(parts[0]);
                        Position.Y = float.Parse(parts[1]);
                        Position.Z = float.Parse(parts[2]);
                    }
                    break;
            }
        }

        public override void Update(float dt)
        {
            
        }
        #endregion
    }
}
