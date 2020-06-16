using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using GuruEngine.DebugHelpers;

namespace GuruEngine.Algebra
{
    public class ViewVolume
    {
        //the corners of the region
        Vector3 tfl, tfr, tbl, tbr, bfl, bfr, bbl, bbr;

        #region public corner accessors
        public Vector3 TopFrontLeft { get { return tfl; } }
        public Vector3 TopFrontRight { get { return tfr; } }
        public Vector3 TopBackLeft { get { return tbl; } }
        public Vector3 TopBackRight { get { return tbr; } }
        public Vector3 BottomFrontLeft { get { return bfl; } }
        public Vector3 BottomFrontRight { get { return bfr; } }
        public Vector3 BottomBackLeft { get { return bbl; } }
        public Vector3 BottomBackRight { get { return bbr; } }
        #endregion

        public Plane[] Planes { get { return planes; } }
        //stores dot products used in collsions.
        public float[] dotProducts;

#if DEBUG
        //vertices to represent the corners for drawing
        VertexPositionColor[] vertices;
#endif

        //planes which represent the faces of the region
        Plane front, back, top, bottom, left, right;
        Plane[] planes;



        /// <summary>
        /// Creates a new Region defined by 8 arbitrary points. The faces of this region must be flat planes for it to work. Recommended for frustums.
        /// NOTE: to assign corners like "top front left" simply choose an arbitrary vantage point from which to view your region.
        /// However, it IS necessary that, from said vantage point, the corner names correspont to their positions!
        /// </summary>
        /// <param name="topFrontLeft">The corner on the top face of the region, toward the front and left</param>
        /// <param name="topFrontRight">The corner on the top face of the region, toward the front and right</param>
        /// <param name="topBackLeft">The corner on the top face of the region, toward the back and left</param>
        /// <param name="topBackRight">The corner on the top face of the region, toward the back and right</param>
        /// <param name="bottomFrontLeft">The corner on the bottom face of the region, toward the front and left</param>
        /// <param name="bottomFrontRight">The corner on the bottom face of the region, toward the front and right</param>
        /// <param name="bottomBackLeft">The corner on the bottom face of the region, toward the back and left</param>
        /// <param name="bottomBackRight">The corner on the bottom face of the region, toward the back and right</param>
        public ViewVolume(Vector3 topFrontLeft, Vector3 topFrontRight, Vector3 topBackLeft, Vector3 topBackRight, Vector3 bottomFrontLeft, Vector3 bottomFrontRight, Vector3 bottomBackLeft, Vector3 bottomBackRight)
        {
            //assign corner values
            tfl = topFrontLeft;
            tfr = topFrontRight;
            tbl = topBackLeft;
            tbr = topBackRight;
            bfl = bottomFrontLeft;
            bfr = bottomFrontRight;
            bbl = bottomBackLeft;
            bbr = bottomBackRight;

            //create planes
            front = new Plane(tfl, tfr, bfl);
            back = new Plane(tbr, tbl, bbl);
            top = new Plane(tfr, tfl, tbl);
            bottom = new Plane(bfl, bfr, bbl);
            left = new Plane(tbl, tfl, bbl);
            right = new Plane(tfr, tbr, bbr);
            planes = new Plane[6] { front, back, top, bottom, left, right };

            //the average of the corners represents the center of the region
            Vector3 avg = tfl + tfr + tbl + tbr + bfl + bfr + bbl + bbr;
            avg /= 8;

            //Fix normals to point outward
            for (int p = 0; p < planes.Length; p++)
            {
                //choose the most convenient point on the plane
                Vector3 pointOnPlane = planes[p].Normal * -planes[p].D;
                //create a vector from the plane to the center
                Vector3 planeToAvg = avg - pointOnPlane;
                //if the normal of the plane and the vector to the center point in the same direction, invert the normal
                if (Vector3.Dot(planeToAvg, planes[p].Normal) > 0)
                {
                    //normal inversion:
                    //reverse normal
                    planes[p].Normal = -planes[p].Normal;
                    //invert D
                    planes[p].D = -planes[p].D;
                    //normalize plane
                    planes[p].Normalize();
                }
            }

            dotProducts = new float[6];

        }


        /// <summary>
        /// Checks to see if the region intersects a ray. Returns the intersection depth of the ray if it does.
        /// </summary>
        /// <param name="ray">ray to intersect with this region</param>
        /// <returns>intersection depth, if any, of the ray</returns>
        public float? Intersects(Ray ray)
        {
            float? depth = null;
            int i = 0;
            for (int p = 0; p < planes.Length; p++)
            {
                Plane plane = planes[p];
                //calculate at what distance the ray intersects this plane.
                //a ray intersects a plane only once, so we only need to calc once.
                float? d = ray.Intersects(plane);
                //if the ray intersects (is not parallel), and either our best depth does not exist OR this intersection depth is less than the best so far
                if (d != null && (depth == null || d < depth))
                {
                    //calculate the point at which it intersects
                    Vector3 intersectionPoint = ray.Position + (ray.Direction * (float)d);
                    //set a flag for whether we are inside the region
                    bool isInside = true;
                    //for each other plane
                    for (int op = 0; op < planes.Length; op++)
                    {
                        //if this plane is not the same as the plane we intersected with
                        if (op != p)
                        {
                            Plane otherPlane = planes[op];
                            //calculate a point on the plane
                            Vector3 pointOnPlane = otherPlane.Normal * -otherPlane.D;
                            //generate a vector from the intersection point to the point on the plane
                            Vector3 vectorToPlane = pointOnPlane - intersectionPoint;
                            //get the dot product of the vectors
                            float intersectionDotPlane = Vector3.Dot(otherPlane.Normal, vectorToPlane);
                            //if the vectors are pointing in opposite directions, we are not inside the other planes
                            if (intersectionDotPlane <= 0)
                            {
                                //this intersection is disqualified
                                isInside = false;
                                i = 0;
                                break;
                            }
                            else
                                dotProducts[i++] = intersectionDotPlane;
                        }
                        else
                            dotProducts[i++] = 0;
                    }
                    //if the check says we are inside all other planes
                    if (isInside)
                    {
                        //set the best depth
                        depth = d;
                        if (i != 6)
                            throw new Exception(); //sanity check
                        i = 0;
                    }

                }
            }
            //return the best depth
            return depth;
        }

#if DEBUG
        // Vector3 tfl, tfr, tbl, tbr, bfl, bfr, bbl, bbr;
        public void DrawVolume(Matrix w)
        {
            DebugLineDraw.DrawLine(tfl, tfr, Color.Red);
            DebugLineDraw.DrawLine(tfr, bfr, Color.Red);
            DebugLineDraw.DrawLine(bfr, bfl, Color.Red);
            DebugLineDraw.DrawLine(bfl, tfl, Color.Red);

            DebugLineDraw.DrawLine(tbl, tbr, Color.Blue);
            DebugLineDraw.DrawLine(tbr, bbr, Color.Blue);
            DebugLineDraw.DrawLine(bbr, bbl, Color.Blue);
            DebugLineDraw.DrawLine(bbl, tbl, Color.Blue);

            DebugLineDraw.DrawLine(tfl, tbl, Color.Green);
            DebugLineDraw.DrawLine(tfr, tbr, Color.Green);
            DebugLineDraw.DrawLine(bfl, bbl, Color.Green);
            DebugLineDraw.DrawLine(bfr, bbr, Color.Green);

        }
#endif

    }
}
