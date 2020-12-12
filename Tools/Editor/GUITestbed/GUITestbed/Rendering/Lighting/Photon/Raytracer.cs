using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using System.Threading.Tasks;
using GUITestbed.Scenes;
using GUITestbed.Rendering._3D;

namespace GUITestbed.Rendering.Lighting.Photon
{
    public class Raytracer
    {
        Radiosity radiosity;
        PhotonMapper photon_mapping;
        Scene scene;

        public Raytracer(Scene sc)
        {
            scene = sc;
        }

        // set access to the other modules for hybrid rendering options
        public void setRadiosity(Radiosity r)
        {
            radiosity = r;
        }

        public void setPhotonMapping(PhotonMapper pm)
        {
            photon_mapping = pm;
        }

        public bool CastRay(Ray ray, Hit h, bool use_sphere_patches)
        {
            for (int i = 0; i < scene.GetFaceCount(); i++)
            {
                Face f = scene.GetFace(i);
                if (f.intersect(ray, ref h, PhotonMapperSettings.intersect_backfacing))
                    return true;
            }
            return false;
        }

        double srgb_to_linear(double x)
        {
            double answer;
            if (x <= 0.04045)
                answer = x / 12.92;
            else
                answer = Math.Pow((x + 0.055) / (1 + 0.055), 2.4);
            return answer;
        }

        public Vector3 TraceRay(Ray ray, Hit hit, int bounce_count = 0)
        {
            hit = new Hit();
            bool intersect = CastRay(ray, hit, false);

            // if there is no intersection, simply return the background color
            if (intersect == false)
            {

                return new Vector3((float)srgb_to_linear(0), (float)srgb_to_linear(0), (float)srgb_to_linear(0));
            }


            // rays coming from the light source are set to white, don't bother to ray trace further.
            if (hit.prim.EmittedColor.Length() > 0.001)
            {
                return hit.prim.EmittedColor;
            }

            Vector3 normal = hit.normal;
            Vector3 point = ray.Position + ray.Direction * (float)hit.t;
            Vector3 answer;

            // ----------------------------------------------
            // start with the indirect light (ambient light)
            Vector3 diffuse_color = hit.prim.Diffuse;
            if (PhotonMapperSettings.gather_indirect)
            {
                // photon mapping for more accurate indirect light
                answer = diffuse_color * photon_mapping.GatherIndirect(point, normal, ray.Direction);
            }
            else
            {
                // the usual ray tracing hack for indirect light
                answer = diffuse_color * PhotonMapperSettings.ambient_light;
            }

            // ----------------------------------------------
            // add contributions from each light that is not in shadow
            int num_lights = scene.GetLightCount();
            for (int i = 0; i < num_lights; i++)
            {
                PhotonEmitter f = scene.GetLight(i);
                Vector3 pointOnLight = f.Centre;
                Vector3 lightColor = f.EmittedColor * (float)f.getArea();
                Vector3 dirToLight = pointOnLight - point;
                double dist = dirToLight.Length();
                dirToLight.Normalize();
                lightColor /= (float)(MathHelper.Pi * dist * dist);

                if (PhotonMapperSettings.num_shadow_samples == 1)
                {
                    Ray lightRay =  new Ray(point, dirToLight);

                    bool allClear = true;

                    int num_primitives = scene.GetFaceCount(); 
                    for (int j = 0; j < num_primitives; ++j)
                    {
                        Face p = scene.GetFace(j);
                        Hit lightHit = new Hit();
                        if (p.intersect(lightRay, ref lightHit, false))
                        {
                            Vector3 shadowRayPoint = lightRay.Position + lightRay.Direction * (float)lightHit.t;
                            Vector3 shadowRayVec = pointOnLight - shadowRayPoint;
                            double x = shadowRayVec.Length();
                            if (x < dist)
                            {
                                allClear = false;
                                shadowRayVec.Normalize();
                                RayTree.AddShadowSegment(new Ray(shadowRayPoint, shadowRayVec), 0, dist);
                                break;
                            }
                        }
                    }

                    if (allClear)
                    {
                        answer += Shade(ray, hit, dirToLight, lightColor, f.EmittedColor, Vector3.One);
                    }
                }
                else if (PhotonMapperSettings.num_shadow_samples > 1)
                {
                    Vector3 tempAnswer = new Vector3(0, 0, 0);
                    for (int s = 0; s < PhotonMapperSettings.num_shadow_samples; ++s)
                    {
                        Vector3 newPoint = f.RandomPoint();
                        Vector3 dir = newPoint - point;
                        dist = dir.Length();
                        dir.Normalize();
                        lightColor = f.EmittedColor * (float)f.getArea();
                        lightColor /= (float)(MathHelper.Pi * dist * dist);
                        Ray lightRay =new Ray(point, dir);

                        bool allClear = true;

                        int num_primitives = scene.GetFaceCount();
                        for (int j = 0; j < num_primitives; ++j)
                        {
                            Face p = scene.GetFace(j);
                            Hit lightHit = new Hit() ;
                            if (p.intersect(lightRay, ref lightHit, false))
                            {
                                Vector3 shadowRayPoint = lightRay.Position = lightRay.Direction * (float)lightHit.t;
                                Vector3 shadowRayVec = pointOnLight - shadowRayPoint;
                                double x = shadowRayVec.Length();
                                if (x < dist)
                                {
                                    allClear = false;
                                    shadowRayVec.Normalize();
                                    RayTree.AddShadowSegment(new Ray(shadowRayPoint, shadowRayVec), 0, dist);

                                }
                            }
                        }

                        if (allClear)
                        {
                            tempAnswer += Shade(ray, hit, dirToLight, lightColor, f.EmittedColor, Vector3.One);
                        }
                    }
                    tempAnswer /= PhotonMapperSettings.num_shadow_samples;
                    answer += tempAnswer;
                }
                else
                {
                    answer += Shade(ray, hit, dirToLight, lightColor,f.EmittedColor, Vector3.One);
                }


            }


            Vector3 reflectiveColor = Vector3.One;


            if (PhotonMapperSettings.num_bounces > bounce_count)
            {
                Vector3 V = ray.Direction;
                Vector3 R_dir = V - 2 * Vector3.Dot(V,normal) * normal;
                R_dir.Normalize();

                Ray R = new Ray(point, R_dir);
                Hit nHit = new Hit() ;
                answer += TraceRay(R, nHit, bounce_count + 1) * reflectiveColor;
                RayTree.AddReflectedSegment(R, 0, nHit.t);
            }

            return answer;
        }

        Vector3 Shade(Ray ray, Hit hit, Vector3 dirToLight, Vector3 lightColor, Vector3 emit, Vector3 diffuse)
        {
            Vector3 point = ray.Position + ray.Direction * (float)hit.t;
            Vector3 n = hit.normal;
            Vector3 e = ray.Direction * -1.0f;
            Vector3 l = dirToLight;

            Vector3 answer = new Vector3(0, 0, 0);

            answer += emit;

            double dot_nl = Vector3.Dot(n , l);
            if (dot_nl< 0) dot_nl = 0;
            answer += lightColor * diffuse * (float)dot_nl;

            Vector3 specularColor = lightColor * diffuse;
            double exponent = 100;

            // compute ideal reflection angle
            Vector3 r = (l * -1.0f) + n * (float)(2 * dot_nl);
            r.Normalize();
            float dot_er = Vector3.Dot(e , r);
            if (dot_er< 0) dot_er = 0;
            answer += lightColor * specularColor*(float)Math.Pow(dot_er, exponent)* (float)dot_nl;

            return answer;
        }

    }
}
