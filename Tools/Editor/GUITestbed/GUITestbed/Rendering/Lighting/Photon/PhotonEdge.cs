using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GUITestbed.Rendering._3D;
using Microsoft.Xna.Framework;

namespace GUITestbed.Rendering.Lighting.Photon
{
    public class PhotonEdge
    {
        Vector3 start_vertex;
        Vector3 end_vertex;

        PhotonEdge next = null;
        PhotonEdge opposite = null;

        Face face;

        public PhotonEdge(Vector3 vs, Vector3 ve, Face f)
        {
            start_vertex = vs;
            end_vertex = ve;
            face = f;
        }

        public Vector3 getStartVertex()
        {
            return start_vertex;
        }

        public Vector3 getEndVertex()
        {
            return end_vertex;
        }

        public PhotonEdge getNext()
        {
            return next;
        }
        public Face getFace()
        {
            return face;
        }
        public PhotonEdge getOpposite()
        {

            return opposite;
        }

        public double Length()
        {
            return (start_vertex - end_vertex).Length();
        }

        public void setOpposite(PhotonEdge e)
        {
            opposite = e;
            e.opposite = this;
        }

        public void clearOpposite()
        {
            if (opposite == null)
                return;

            opposite.opposite = null;
            opposite = null;
        }

        void setNext(PhotonEdge e)
        {
            next = e;
        }
    }
}
