using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using GuruEngine.Helpers;
using GuruEngine.ECS.Components.World;
using GuruEngine.ECS.Components.Weapons.Bullets;
using GuruEngine.ECS.Components.Configuration;
using GuruEngine.ECS.Components.Mesh;

//( Class GunComponent )
//( Group Weapons )
//( Type GunComponent )
//( Connection BulletPropertiesComponent Bullets )
//( Parameter Bool Aimer )
//( Parameter Int weaponType )
//( Parameter Bool Cannon )
//( Parameter Bool UseHookAsRel )
//( Parameter String fireMesh )
//( Parameter String fire )
//( Parameter String sprite )
//( Parameter String fireMeshDay )
//( Parameter String fireDay )
//( Parameter String spriteDay )
//( Parameter String smoke )
//( Parameter String shells )
//( Parameter String sound )
//( Parameter String hook )
//( Parameter Colour emitColor )
//( Parameter Float emitI )
//( Parameter Float emitR )
//( Parameter Float emitTime )
//( Parameter Float aimMinDist )
//( Parameter Float aimMaxDist )
//( Parameter Float maxDeltaAngle )
//( Parameter Float shotFreq )
//( Parameter Float shotFreqDeviation )
//( Parameter Int traceFreq )
//( Parameter Bool EnablePause )
//( Parameter Int bullets )
//( Parameter Int bulletsCluster )
//( Preset Browning303 false 1 false true GunFire/7mm GunFire/7mm/GunFlare GunFire/7mm null GunFire/7mm/GunFlare null Smokes/MachineGunSmall GunShells/GunShells mgun_browning303 ADDTHIS ff884020 10 3 0.03 10 1000 0.32 15 0 3 false 500 2 )



namespace GuruEngine.ECS.Components.Weapons.Guns
{
    public class GunComponent : ECSGameComponent
    {
        public bool Aimer;
        public int weaponType;
        public bool Cannon;
        public bool UseHookAsRel;
        public String fireMesh;
        public String fire;
        public String sprite;
        public String fireMeshDay;
        public String fireDay;
        public String spriteDay;
        public String smoke;
        public String shells;
        public String sound;
        public String hook;
        public Color emitColor;
        public float emitI;
        public float emitR;
        public float emitTime;
        public float aimMinDist;
        public float aimMaxDist;
        public float maxDeltaAngle;
        public float shotFreq;
        public float shotFreqDeviation;
        public int traceFreq;
        public bool EnablePause;
        public int bullets;
        public int bulletsCluster;

        public BulletPropertiesComponent bulletproperties;

        #region ECS game component interface
        public override ECSGameComponent Clone()
        {
            GunComponent other = new GunComponent();
            other.bulletproperties = bulletproperties;
            other.Aimer = Aimer;
            other.weaponType = weaponType;
            other.Cannon = Cannon;
            other.UseHookAsRel = UseHookAsRel;
            other.fireMesh = fireMesh;
            other.fire = fire;
            other.sprite = sprite;
            other.fireMeshDay = fireMeshDay;
            other.fireDay = fireDay;
            other.spriteDay = spriteDay;
            other.smoke = smoke;
            other.shells = shells;
            other.sound = sound;
            other.hook = hook;
            other.emitColor = emitColor;
            other.emitI = emitI;
            other.emitR = emitR;
            other.emitTime = emitTime;
            other.aimMinDist = aimMinDist;
            other.aimMaxDist = aimMaxDist;
            other.maxDeltaAngle = maxDeltaAngle;
            other.shotFreq = shotFreq;
            other.shotFreqDeviation = shotFreqDeviation;
            other.traceFreq = traceFreq;
            other.EnablePause = EnablePause;
            other.bullets = bullets;
            other.bulletsCluster = bulletsCluster;
            return other;
        }

        public override void Connect(string components, bool isList)
        {
            string[] parts = components.Split('#');
            if (isList)
            {

            }
            else
            {
                string[] objects = parts[1].Split(':');
                switch (parts[0])
                {
                    case "Root":
                        {
                            Parent = GameObjectManager.Instance.FindGameObjectByName(objects[0]);
                        }
                        break;

                    case "Bullets":
                        {
                            bulletproperties = (BulletPropertiesComponent)Parent.FindGameComponentByName(objects[0]);
                        }
                        break;
                    default:
                        throw new Exception("GameComponent::AngleAnimator:: Unknown direct connection request to " + parts[0]);
                }
            }
        }

        public override void DisConnect()
        {
            Parent = null;
            bulletproperties = null;
            UpdateStage = 99;
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
            PlayerOptionsComponent poc = (PlayerOptionsComponent)Parent.FindSingleComponentByType<PlayerOptionsComponent>();
            poc.RegisterVariable("GunAimingDistance", (aimMaxDist + aimMinDist) * 0.5f, aimMinDist, aimMaxDist);
        }


        public override void ReConnect(GameObject other)
        {
            GunComponent ot = (GunComponent)other.FindGameComponentByName(Name);
            ot.bulletproperties = (BulletPropertiesComponent)other.FindGameComponentByName(bulletproperties.Name);
            ot.Aimer = Aimer;
            ot.weaponType = weaponType;
            ot.Cannon = Cannon;
            ot.UseHookAsRel = UseHookAsRel;
            ot.fireMesh = fireMesh;
            ot.fire = fire;
            ot.sprite = sprite;
            ot.fireMeshDay = fireMeshDay;
            ot.fireDay = fireDay;
            ot.spriteDay = spriteDay;
            ot.smoke = smoke;
            ot.shells = shells;
            ot.sound = sound;
            ot.hook = hook;
            ot.emitColor = emitColor;
            ot.emitI = emitI;
            ot.emitR = emitR;
            ot.emitTime = emitTime;
            ot.aimMinDist = aimMinDist;
            ot.aimMaxDist = aimMaxDist;
            ot.maxDeltaAngle = maxDeltaAngle;
            ot.shotFreq = shotFreq;
            ot.shotFreqDeviation = shotFreqDeviation;
            ot.traceFreq = traceFreq;
            ot.EnablePause = EnablePause;
            ot.bullets = bullets;
            ot.bulletsCluster = bulletsCluster;
        }

        public override void RenderOffscreenRenderTargets()
        {
        }

        public override void SetParameter(string Name, string Value)
        {
            switch (Name)
            {
                case "Aimer":
                    Aimer = bool.Parse(Value);
                    break;
                case "weaponType":
                    weaponType = int.Parse(Value);
                    break;
                case "Cannon":
                    Cannon = bool.Parse(Value);
                    break;
                case "UseHookAsRel":
                    UseHookAsRel = bool.Parse(Value);
                    break;
                case "fireMesh":
                    fireMesh = Value;
                    break;
                case "fire":
                    fireMesh = Value;
                    break;
                case "sprite":
                    fire = Value;
                    break;
                case "fireMeshDay":
                    fireMeshDay = Value;
                    break;
                case "fireDay":
                    fireDay = Value;
                    break;
                case "spriteDay":
                    spriteDay = Value;
                    break;
                case "smoke":
                    smoke = Value;
                    break;
                case "shells":
                    shells = Value;
                    break;
                case "sound":
                    sound = Value;
                    break;
                case "hook":
                    hook = Value;
                    break;
                case "emitColor":
                    emitColor = IOHelper.ColourFromString(Value);
                    break;
                case "emitI":
                    emitI = float.Parse(Value);
                    break;
                case "emitR":
                    emitI = float.Parse(Value);
                    break;
                case "emitTime":
                    emitR = float.Parse(Value);
                    break;
                case "aimMinDist":
                    aimMinDist = float.Parse(Value);
                    break;
                case "aimMaxDist":
                    aimMaxDist = float.Parse(Value);
                    break;
                case "maxDeltaAngle":
                    maxDeltaAngle = float.Parse(Value);
                    break;
                case "shotFreq":
                    shotFreq = float.Parse(Value);
                    break;
                case "shotFreqDeviation":
                    shotFreqDeviation = float.Parse(Value);
                    break;
                case "traceFreq":
                    traceFreq = int.Parse(Value);
                    break;
                case "EnablePause":
                    EnablePause = bool.Parse(Value);
                    break;
                case "bullets":
                    bullets = int.Parse(Value);
                    break;
                case "bulletsCluster":
                    bulletsCluster = int.Parse(Value);
                    break;
            }
        }

        public override void Update(float dt)
        {

        }

        #endregion

    }
}
