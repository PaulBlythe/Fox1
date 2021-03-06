﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xna.Framework;

namespace GuruEngine.Cameras
{
    public abstract class Camera
    {
        public Matrix View;
        public Matrix Projection;
        public Vector3 Position;
        public Vector3 Forward;
        public Vector3 Right;
        public float FarClip;
        public BoundingFrustum Frustum;
        public bool HasFocus = true;

        public abstract void Update(float gt);
        public abstract Matrix GetWorld();
        public abstract void Yaw(float angle);
        public abstract void SetPosition(Vector3 location);

        public bool BoundingVolumeIsInView(BoundingBox box)
        {
            if (Frustum == null)
                return true;
            return (Frustum.Contains(box) != ContainmentType.Disjoint);
        }

        public bool BoundingVolumeIsInView(BoundingSphere sphere)
        {
            if (Frustum == null)
                return true;
            return (Frustum.Contains(sphere) != ContainmentType.Disjoint);
        }
    }
}
