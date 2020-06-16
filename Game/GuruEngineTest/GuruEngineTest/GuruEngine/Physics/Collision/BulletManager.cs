using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace GuruEngine.Physics.Collision
{
    public class BulletManager
    {
        public static BulletManager Instance;

        public List<BulletRecord> activeBullets = new List<BulletRecord>();

        public BulletManager()
        {
            Instance = this;
        }

        public void AddBullet(BulletRecord rec)
        {
            activeBullets.Add(rec);
        }

        public void Update(float dt)
        {
            for (int i= activeBullets.Count-1; i>=0; i--)
            {
                if (activeBullets[i].Update(dt))
                    activeBullets.RemoveAt(i);
            }
        }
    }
}
