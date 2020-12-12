using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Threading.Tasks;
using GUITestbed.Scenes;
using GUITestbed.Rendering._3D;

namespace GUITestbed.Rendering.Lighting.Photon
{
    public class Radiosity
    {
        Scene scene;

        int num_faces;
        Raytracer raytracer;
        PhotonMapper photon_mapping;


        double[] formfactors;

        // length n vectors
        double[] area;
        Vector3[] undistributed;
        Vector3[] absorbed;
        Vector3[] radiance;

        int max_undistributed_patch;
        double total_undistributed;
        double total_area;


        public Radiosity(Scene s)
        {
            scene = s;
            num_faces = -1;
            max_undistributed_patch = -1;
            total_area = -1;
            Reset();
        }

        void Reset()
        {
            // create and fill the data structures
            num_faces = scene.CountFaces();
            area = new double[num_faces];
            undistributed = new Vector3[num_faces];
            absorbed = new Vector3[num_faces];
            radiance = new Vector3[num_faces];
            for (int i = 0; i < num_faces; i++)
            {
                Face f = scene.GetFace(i);
                f.RadiosityPatchIndex = i;

                setArea(i, f.getArea());
                Vector3 emit = f.EmittedColor;
                setUndistributed(i, emit);
                setAbsorbed(i, new Vector3(0, 0, 0));
                setRadiance(i, emit);
            }

            findMaxUndistributed();
        }

        double getFormFactor(int i, int j)
        {
            return formfactors[i * num_faces + j];
        }

        double getArea(int i)
        {
            return area[i];
        }

        Vector3 getUndistributed(int i)
        {
            return undistributed[i];
        }

        Vector3 getAbsorbed(int i)
        {
            return absorbed[i];
        }

        Vector3 getRadiance(int i)
        {
            return radiance[i];
        }

        void setFormFactor(int i, int j, double value)
        {
            formfactors[i * num_faces + j] = value;
        }

        void normalizeFormFactors(int i)
        {
            double sum = 0;
            int j;
            for (j = 0; j < num_faces; j++)
            {
                sum += getFormFactor(i, j);
            }
            if (sum == 0) return;
            for (j = 0; j < num_faces; j++)
            {
                setFormFactor(i, j, getFormFactor(i, j) / sum);
            }
        }

        void setArea(int i, double value)
        {
            area[i] = value;
        }

        void setUndistributed(int i, Vector3 value)
        {
            undistributed[i] = value;
        }

        void setAbsorbed(int i, Vector3 value)
        {
            absorbed[i] = value;
        }

        void setRadiance(int i, Vector3 value)
        {
            radiance[i] = value;
        }

        void findMaxUndistributed()
        {
            max_undistributed_patch = -1;
            total_undistributed = 0;
            total_area = 0;
            double max = -1;
            for (int i = 0; i < num_faces; i++)
            {
                double m = getUndistributed(i).Length() * getArea(i);
                total_undistributed += m;
                total_area += getArea(i);
                if (max < m)
                {
                    max = m;
                    max_undistributed_patch = i;
                }
            }
        }

        void ComputeFormFactors()
        {
            formfactors = new double[num_faces * num_faces];

            for (int i = 0; i < num_faces; ++i)
            {
                for (int j = i; j < num_faces; ++j)
                {
                    if (i == j)
                    {
                        formfactors[i * num_faces + j] = 0;
                        continue;
                    }

                    Face f1 = scene.GetFace(i);
                    Face f2 = scene.GetFace(j);

                    double formfactor = 0.0;

                    Vector3 p1 = f1.computeCentroid();
                    Vector3 p2 = f2.computeCentroid();

                    // Get the vec for p1-->p2
                    Vector3 line_p1p2 = p2 - p1;
                    double R2 = line_p1p2.Length() * line_p1p2.Length();

                    Vector3 p1Normal = f1.Normal;
                    line_p1p2.Normalize();

                    double cos_p1 = Vector3.Dot(p1Normal, line_p1p2);

                    Vector3 p2Normal = f2.Normal;
                    double cos_p2 = Vector3.Dot(p2Normal, -line_p1p2);

                    double ff = 0.0;

                    if (cos_p1 > float.Epsilon && cos_p2 > float.Epsilon)
                    {
                        ff = cos_p1 * cos_p2 / (MathHelper.Pi * R2);
                    }

                    formfactor += ff;

                    formfactors[i * num_faces + j] = formfactor * getArea(i) / getArea(j);
                    formfactors[j * num_faces + i] = formfactor * getArea(j) / getArea(i);
                }

                normalizeFormFactors(i);
            }
        }

        double Iterate()
        {
            if (formfactors == null)
                ComputeFormFactors();

            for (int i = 0; i < num_faces; ++i)
            {
                Face f1 = scene.GetFace(i);
                Vector3 p_i = f1.Diffuse;

                int j = max_undistributed_patch;

                if (i == j)
                {
                    continue;
                }

                Vector3 ff = (float)(formfactors[i * num_faces + j]) * getUndistributed(j);

                setRadiance(i, getRadiance(i) + ff * p_i);
                setUndistributed(i, getUndistributed(i) + ff * p_i);
                setAbsorbed(i, getAbsorbed(i) + (new Vector3(1, 1, 1) - p_i) * ff);
            }

            setUndistributed(max_undistributed_patch, Vector3.Zero);

            findMaxUndistributed();
            return total_undistributed;
        }

        Vector3 whichVisualization(RenderModes mode, Face f, int i)
        {

            if (mode == RenderModes.RENDER_LIGHTS)
            {
                return f.EmittedColor;
            }
            else if (mode == RenderModes.RENDER_UNDISTRIBUTED)
            {
                return getUndistributed(i);
            }
            else if (mode == RenderModes.RENDER_ABSORBED)
            {
                return getAbsorbed(i);
            }
            else if (mode == RenderModes.RENDER_RADIANCE)
            {
                return getRadiance(i);
            }
            else if (mode == RenderModes.RENDER_FORM_FACTORS)
            {
                if (formfactors == null)
                    ComputeFormFactors();
                double scale = 0.2 * total_area / getArea(i);
                double factor = scale * getFormFactor(max_undistributed_patch, i);
                return new Vector3((float)factor, (float)factor, (float)factor);
            }
            else
            {
                throw new Exception("Radiosity:: Unknown render mode");
            }

        }

        void CollectFacesWithVertex(Vector3 have, Face f, List<Face> faces)
        {
            for (int i = 0; i < faces.Count; i++)
            {
                if (faces[i] == f) return;
            }
            if (have != f.V1 && have != f.V2 && have != f.V3)
                return;

            faces.Add(f);
            for (int i = 0; i < 4; i++)
            {
                PhotonEdge ea = f.Edge.getOpposite();
                PhotonEdge eb = f.Edge.getNext().getOpposite();
                PhotonEdge ec = f.Edge.getNext().getNext().getOpposite();

                if (ea != null) CollectFacesWithVertex(have, ea.getFace(), faces);
                if (eb != null) CollectFacesWithVertex(have, eb.getFace(), faces);
                if (ec != null) CollectFacesWithVertex(have, ec.getFace(), faces);

            }
        }

        Vector3 GetColor(Vector3 v)
        {
            double r = linear_to_srgb(v.X);
            double g = linear_to_srgb(v.Y);
            double b = linear_to_srgb(v.Z);
            return new Vector3((float)r, (float)g, (float)b);
        }

        Vector3 insertInterpolatedColor(int index, Face f, Vector3 v)
        {
            List<Face> faces = new List<Face>();
            CollectFacesWithVertex(v, f, faces);

            float total = 0;
            Vector3 color = new Vector3(0, 0, 0);
            Vector3 normal = f.Normal;
            for (int i = 0; i < faces.Count; i++)
            {
                Vector3 normal2 = faces[i].Normal;
                double area = faces[i].getArea();
                if (Vector3.Dot(normal, normal2) < 0.5) continue;

                total += (float)area;
                color += (float)area * whichVisualization(RenderModes.RENDER_RADIANCE, faces[i], faces[i].RadiosityPatchIndex);
            }

            color /= total;
            return color;
        }

        double linear_to_srgb(double x)
        {
            const double SRGB_ALPHA = 0.055;
            double answer;
            if (x <= 0.0031308)
                answer = 12.92 * x;
            else
                answer = (1 + SRGB_ALPHA) * (Math.Pow(x, 1 / 2.4) - SRGB_ALPHA);
            return answer;
        }

        public List<VertexPositionNormalColor> Paint()
        {
            List<VertexPositionNormalColor> verts = new List<VertexPositionNormalColor>();

            switch (PhotonMapperSettings.render_mode)
            {
                case RenderModes.RENDER_MATERIALS:
                    {
                        // draw the faces with OpenGL lighting, just to understand the geometry
                        // (the GL light has nothing to do with the surfaces that emit light!)
                        for (int i = 0; i < num_faces; i++)
                        {
                            Face f = scene.getFace(i);

                            VertexPositionNormalColor v = new VertexPositionNormalColor();
                            v.Position = f.V1;
                            v.Normal = f.Normal;
                            v.Color = f.Diffuse;
                            verts.Add(v);

                            v = new VertexPositionNormalColor();
                            v.Position = f.V2;
                            v.Normal = f.Normal;
                            v.Color = f.Diffuse;
                            verts.Add(v);

                            v = new VertexPositionNormalColor();
                            v.Position = f.V3;
                            v.Normal = f.Normal;
                            v.Color = f.Diffuse;
                            verts.Add(v);
                        }
                        return verts;
                    }

                case RenderModes.RENDER_RADIANCE:
                    {
                        if (PhotonMapperSettings.interpolate)
                        {
                            for (int i = 0; i < num_faces; i++)
                            {
                                Face f = scene.getFace(i);

                                VertexPositionNormalColor v = new VertexPositionNormalColor();
                                v.Position = f.V1;
                                v.Normal = f.Normal;
                                v.Color = insertInterpolatedColor(i, f, f.V1);
                                verts.Add(v);

                                v = new VertexPositionNormalColor();
                                v.Position = f.V2;
                                v.Normal = f.Normal;
                                v.Color = insertInterpolatedColor(i, f, f.V2);
                                verts.Add(v);

                                v = new VertexPositionNormalColor();
                                v.Position = f.V3;
                                v.Normal = f.Normal;
                                v.Color = insertInterpolatedColor(i, f, f.V3);
                                verts.Add(v);
                            }
                            return verts;
                        }
                    }
                    break;

                case RenderModes.RENDER_FORM_FACTORS:
                    {
                        // draw the faces with OpenGL lighting, just to understand the geometry
                        // (the GL light has nothing to do with the surfaces that emit light!)
                        for (int i = 0; i < num_faces; i++)
                        {
                            Face f = scene.GetFace(i);

                            VertexPositionNormalColor v1 = new VertexPositionNormalColor();
                            v1.Position = f.V1;
                            v1.Normal = f.Normal;
                            v1.Color = Color.Red.ToVector3();

                            VertexPositionNormalColor v2 = new VertexPositionNormalColor();
                            v2.Position = f.V2;
                            v2.Normal = f.Normal;
                            v2.Color = Color.Red.ToVector3();

                            VertexPositionNormalColor v3 = new VertexPositionNormalColor();
                            v3.Position = f.V3;
                            v3.Normal = f.Normal;
                            v3.Color = Color.Red.ToVector3();

                            verts.Add(v1);
                            verts.Add(v2);

                            verts.Add(v2);
                            verts.Add(v3);

                            verts.Add(v1);
                            verts.Add(v3);

                        }
                        return verts;
                    }

                default:
                    {
                        for (int i = 0; i < num_faces; i++)
                        {
                            Face f = scene.GetFace(i);

                            VertexPositionNormalColor v = new VertexPositionNormalColor();
                            v.Position = f.V1;
                            v.Normal = f.Normal;
                            v.Color = whichVisualization(PhotonMapperSettings.render_mode, f, i);
                            verts.Add(v);

                            v = new VertexPositionNormalColor();
                            v.Position = f.V2;
                            v.Normal = f.Normal;
                            v.Color = whichVisualization(PhotonMapperSettings.render_mode, f, i);
                            verts.Add(v);

                            v = new VertexPositionNormalColor();
                            v.Position = f.V3;
                            v.Normal = f.Normal;
                            v.Color = whichVisualization(PhotonMapperSettings.render_mode, f, i);
                            verts.Add(v);
                        }
                        return verts;

                    }
            }
            return null;
        }
    }
}
