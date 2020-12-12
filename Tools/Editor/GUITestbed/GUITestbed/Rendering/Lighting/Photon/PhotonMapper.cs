using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using GUITestbed.Scenes;
using GUITestbed.Rendering._3D;

namespace GUITestbed.Rendering.Lighting.Photon
{
    public class PhotonMapper
    {
        Scene current;
        Raytracer raytracer;
        Radiosity radiosity;
        PhotonTree kdTree;

        Random rand = new Random();

        public PhotonMapper(Scene scene)
        {
            current = scene;
            raytracer = null;
            radiosity = null;
            kdTree = null;
        }

        public void setRayTracer(Raytracer r)
        {
            raytracer = r;
        }

        public void setRadiosity(Radiosity r)
        {
            radiosity = r;
        }

        public void TracePhotons()
        {
            kdTree.Clear();
            int num_prims = current.GetFaceCount();

            // consruct a kdtree to store the photons
            BoundingBox bb = current.GetBoundingBox();
            Vector3 min = bb.Min;
            Vector3 max = bb.Max;
            Vector3 diff = max - min;
            min -= 0.001f * diff;
            max += 0.001f * diff;
            kdTree = new PhotonTree(new BoundingBox(min, max), 0);

            List<Face> lights = current.GetLights();

            // compute the total area of the lights
            double total_lights_area = 0;
            for (int i = 0; i < lights.Count(); i++)
            {
                total_lights_area += lights[i].getArea();
            }

            Vector3 global_energy = new Vector3();

            for (int i = 0; i < lights.Count; i++)
            {
                float my_area = lights[i].getArea();

                int num = (int)Math.Ceiling(PhotonMapperSettings.num_photons_to_shoot * my_area / total_lights_area);

                Vector3 energy = my_area / num * Vector3.One;
                Vector3 normal = lights[i].Normal;
                global_energy += num * energy;

                for (int j = 0; j < num; j++)
                {
                    Vector3 start = lights[i].RandomPoint();
                    Vector3 direction = RandomDiffuseDirection(normal);
                    TracePhoton(start, direction, energy, 0);
                }
            }
        }

        public Vector3 GatherIndirect(Vector3 point, Vector3 normal, Vector3 direction_from)
        {

        }

        public Vector3 CalculateEnergy(Sphere s)
        {

        }

        void TracePhoton(Vector3 position, Vector3 direction, Vector3 energy, int iter)
        {
            if (iter > 5)
            {
                return;
            }

            Ray r = new Ray(position, direction);
            Hit h = new Hit();

            if (raytracer.CastRay(r, h, false))
            {
                Vector3 v = raytracer.TraceRay(r, h, 0);
                RayPrimitive p = h.getPrimitive();
                if (p != null)
                {
                    Photon ph = new Photon(position, direction, energy, iter);
                    p.addPhoton(ph);
                }

                Vector3 pos = r.Position + r.Direction * (float)(h.t2);

                // Take care of some things here...
                Material m = h.getMaterial();

                // Multiply by material consants
                Vector3 diffuse = m.getDiffuseColor();

                diffuse = diffuse * energy;
                Vector3 reflective = m.getReflectiveColor();

                reflective = reflective * energy;
                Vector3 transmissive = m.getTransmissiveColor();
                transmissive = transmissive * energy;


                if (diffuse != Vector3.Zero)
                {

                    Vector3 R_dir = RandomDiffuseDirection(h.normal);

                    Ray R = new Ray(pos, R_dir);
                    TracePhoton(pos, R_dir, diffuse, iter + 1);
                    if (iter != 0)
                    {
                        Photon p = new Photon(pos, direction, diffuse, iter);
                        kdTree.AddPhoton(p);
                    }
                }
                if (reflective != Vector3.Zero)
                {

                    Vector3 R_dir = MirrorDirection(h.normal, r.Direction);
                    R_dir.Normalize();

                    Ray R = new Ray(pos, R_dir);
                    TracePhoton(pos, R_dir, reflective, iter + 1);
                    if (iter != 0)
                    {
                        Photon p = new Photon(pos, direction, reflective, iter);
                        kdTree.AddPhoton(p);
                    }
                }
                if (transmissive != Vector3.Zero)
                {
                    Vector3 pos2 = r.Position + r.Direction * (float)(h.t2 + float.Epsilon);

                    Ray R = new Ray(pos2, r.Direction);
                    TracePhoton(pos2, r.Direction, transmissive, iter + 1);
                    if (iter != 0)
                    {
                        Photon p = new Photon(pos, direction, transmissive, iter);
                        kdTree.AddPhoton(p);
                    }
                }
            }

        }

        void RenderPhotonPositions()
        {

        }

        void RenderPhotonDirections()
        {

        }

        public void RenderPhotons()
        {

        }

        public void RenderKDTree()
        {

        }

        public void RenderEnergy()
        {

        }

        Vector3 MirrorDirection(Vector3 normal, Vector3 incoming)
        {
            float dot = Vector3.Dot(incoming, normal);
            Vector3 r = (incoming * -1.0f) + normal * (2 * dot);
            return r * -1.0f;
        }

        Vector3 RandomDiffuseDirection(Vector3 normal)
        {
            Vector3 answer = normal + RandomUnitVector();
            answer.Normalize();
            return answer;
        }

        Vector3 RandomUnitVector()
        {
            Vector3 tmp;

            float f1 = (float)((2 * rand.NextDouble()) - 1);
            float f2 = (float)((2 * rand.NextDouble()) - 1);
            float f3 = (float)((2 * rand.NextDouble()) - 1);
            tmp = new Vector3(f1, f2, f3);

            tmp.Normalize();
            return tmp;
        }

    }
}
