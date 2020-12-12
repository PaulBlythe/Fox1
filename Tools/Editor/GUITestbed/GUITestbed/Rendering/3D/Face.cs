using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GUITestbed.Rendering.Lighting.Photon;
using Microsoft.Xna.Framework;

namespace GUITestbed.Rendering._3D
{
    public class Face
    {
        public Vector3 V1;
        public Vector3 V2;
        public Vector3 V3;

        public Vector3 Normal;
        public Vector3 Diffuse;
        public Vector3 EmittedColor = Vector3.Zero;

        public PhotonEdge Edge;

        public int RadiosityPatchIndex;

        Random rand = new Random();

        public float getArea()
        {
            Vector3 a = V1;
            Vector3 b = V2;
            Vector3 c = V3;

            return AreaOfTriangle((a - b).Length(),(a - c).Length(), (b - c).Length());
        }

        public Vector3 computeCentroid()
        {
            return (V1 + V2 + V3) / 3.0f;
        }

        float AreaOfTriangle(float a, float b, float c)
        {
            float s = (a + b + c) / (float)2;
            return (float) Math.Sqrt(s * (s - a) * (s - b) * (s - c));
        }

        public bool intersect(Ray r, ref Hit h, bool intersect_backfacing) 
        {
            Vector3 a = V1;
            Vector3 b = V2;
            Vector3 c = V3;

            return triangle_intersect(r, h, a, b, c, intersect_backfacing);
        }

        bool plane_intersect(Ray r, ref Hit h, bool intersect_backfacing)
        {

            double d = Vector3.Dot(Normal,V1);
            double numer = d - Vector3.Dot(r.Position,Normal);
            double denom = Vector3.Dot(r.Direction,Normal);

            if (denom == 0) return false;  

            if (!intersect_backfacing && Vector3.Dot(Normal,r.Direction) >= 0) 
                 return false; 

            double t = numer / denom;
            if (t > float.Epsilon && t<h.t)
            {
                h.t = t;
                h.normal = Normal;
                h.t2 = t;
   
                return true;
            }
            return false;
        }

        bool triangle_intersect(Ray r, ref Hit h,Vector3 a, Vector3 b, Vector3 c, bool intersect_backfacing)
        {
            Hit h2 = new Hit(h);
            
            if (!plane_intersect(r, h2, intersect_backfacing))
                return false;


            // Compute vectors along two edges of the triangle.
            Vector3 edge1, edge2;

            Vector3.Subtract(ref b, ref a, out edge1);
            Vector3.Subtract(ref c, ref a, out edge2);

            // Compute the determinant.
            Vector3 directionCrossEdge2;
            Vector3.Cross(ref r.Direction, ref edge2, out directionCrossEdge2);

            float determinant;
            Vector3.Dot(ref edge1, ref directionCrossEdge2, out determinant);

            // If the ray is parallel to the triangle plane, there is no collision.
            if (determinant > -float.Epsilon && determinant < float.Epsilon)
            {
                return false;
            }

            float inverseDeterminant = 1.0f / determinant;

            // Calculate the U parameter of the intersection point.
            Vector3 distanceVector;
            Vector3.Subtract(ref r.Position, ref a, out distanceVector);

            float triangleU;
            Vector3.Dot(ref distanceVector, ref directionCrossEdge2, out triangleU);
            triangleU *= inverseDeterminant;

            // Make sure it is inside the triangle.
            if (triangleU < 0 || triangleU > 1)
            {
                return false;
            }

            // Calculate the V parameter of the intersection point.
            Vector3 distanceCrossEdge1;
            Vector3.Cross(ref distanceVector, ref edge1, out distanceCrossEdge1);

            float triangleV;
            Vector3.Dot(ref r.Direction, ref distanceCrossEdge1, out triangleV);
            triangleV *= inverseDeterminant;

            // Make sure it is inside the triangle.
            if (triangleV < 0 || triangleU + triangleV > 1)
            {
                return false;
            }

            // Compute the distance along the ray to the triangle.
            float rayDistance;
            Vector3.Dot(ref edge2, ref distanceCrossEdge1, out rayDistance);
            rayDistance *= inverseDeterminant;

            // Is the triangle behind the ray origin?
            if (rayDistance < 0)
            {
                return false;
            }
            return true;
        }


        public Vector3 RandomPoint()
        {
            Vector3 a = V1;
            Vector3 b = V2;
            Vector3 c = V3;

            float s = (float)rand.NextDouble(); 
            float t = (float)rand.NextDouble(); 

            Vector3 answer = (s * t * a) + (s * (1 - t) * b)  + (1 - s) * (1 - t) * c;
            return answer;
        }
    }
}
