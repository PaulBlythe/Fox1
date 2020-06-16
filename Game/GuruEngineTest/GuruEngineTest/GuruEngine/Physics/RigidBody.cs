using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace GuruEngine.Physics
{
    public class RigidBody
    {
        List<PhysicsMass> Masses = new List<PhysicsMass>();
        PhysicsMass StaticMass = new PhysicsMass(0, Vector3.Zero, true);

        public float TotalMass;
        public Vector3 Cg;
        public Vector3 Gyro;

        // Inertia tensor, and its inverse. 
        public Matrix TIStatic;
        public Matrix TI;
        public Matrix InverseT;

        public Vector3 Force;
        public Vector3 Torque;
        public Vector3 Spin = Vector3.Zero;

        bool dirty = false;


        public int AddMass(float mass, Vector3 pos, bool isStatic = false)
        {
            dirty = true;
            PhysicsMass m = new PhysicsMass(mass,pos,isStatic);
            Masses.Add(m);
            return Masses.Count - 1;
        }

        public void SetMass(int index, float mass)
        {
            Masses[index].Mass = mass;
        }

        private void RecalcStaticMass()
        {
            StaticMass.Mass = 0;
            StaticMass.Position = Vector3.Zero;

            int s = 0;
            for (int i = 0; i < Masses.Count; i++)
            {
                if (Masses[i].IsStatic)
                {
                    s++;
                    float mass = Masses[i].Mass;
                    StaticMass.Mass += mass;

                    Vector3 momentum = mass * Masses[i].Position;
                    StaticMass.Position += momentum;
                }
            }
            StaticMass.Position = StaticMass.Position / StaticMass.Mass;

            Vector4 r1 = new Vector4(0, 0, 0, 0);
            Vector4 r2 = new Vector4(0, 0, 0, 0);
            Vector4 r3 = new Vector4(0, 0, 0, 0);
            Vector4 r4 = new Vector4(0, 0, 0, 1);

            for (int i = 0; i < Masses.Count; i++)
            {
                if (Masses[i].IsStatic)
                {
                    float m = Masses[i].Mass;

                    Vector3 pos = Masses[i].Position - StaticMass.Position;

                    float xy = m * pos.X * pos.Y; float yz = m * pos.Y * pos.Z; float zx = m * pos.Z * pos.X;
                    float x2 = m * pos.X * pos.X; float y2 = m * pos.Y * pos.Y; float z2 = m * pos.Z * pos.Z;

                    r1.X += y2 + z2;
                    r1.Y -= xy;
                    r1.Z -= zx;

                    r2.Y += x2 + z2;
                    r2.Z -= yz;

                    r3.Y += x2 + y2;
                }
            }
            // copy symmetric elements
            r2.X = r1.Y;
            r3.X = r1.Z;
            r3.Y = r2.Z;
            TIStatic = new Matrix(r1, r2, r3, r4);
        }

        private void CalcCofG()
        {
            Cg = StaticMass.Mass * StaticMass.Position;
            TotalMass = StaticMass.Mass;
            for (int i = 0; i < Masses.Count; i++)
            {
                if (!Masses[i].IsStatic)
                {
                    float mass = Masses[i].Mass;
                    TotalMass += mass;

                    Vector3 momentum = mass * Masses[i].Position;
                    Cg += momentum;
                }
            }
            Cg /= TotalMass;
        }

        private void CalcInertia()
        {
            Vector4 r1 = new Vector4(TIStatic.M11, TIStatic.M12, TIStatic.M13, TIStatic.M14);
            Vector4 r2 = new Vector4(TIStatic.M21, TIStatic.M22, TIStatic.M23, TIStatic.M24);
            Vector4 r3 = new Vector4(TIStatic.M31, TIStatic.M32, TIStatic.M33, TIStatic.M34);
            Vector4 r4 = new Vector4(TIStatic.M41, TIStatic.M42, TIStatic.M43, TIStatic.M44);

            for (int i = 0; i < Masses.Count; i++)
            {
                if (!Masses[i].IsStatic)
                {
                    float m = Masses[i].Mass;

                    float x = Masses[i].Position.X - Cg.X;
                    float y = Masses[i].Position.Y - Cg.Y;
                    float z = Masses[i].Position.Z - Cg.Z;
                    float mx = m * x;
                    float my = m * y;
                    float mz = m * z;

                    float xy = mx * y; float yz = my * z; float zx = mz * x;
                    float x2 = mx * x; float y2 = my * y; float z2 = mz * z;

                    r1.X += y2 + z2;
                    r1.Y -= xy;
                    r1.Z -= zx;

                    r2.Y += x2 + z2;
                    r2.Z -= yz;
                    r3.Z += x2 + y2;
                }
            }
            // copy symmetric elements 
            r2.X = r1.Y;
            r3.X = r1.Z;
            r3.Y = r2.Z;

            TI = new Matrix(r1, r2, r3, r4);
            InverseT = Matrix.Invert(TI);
        }

        public void Reset()
        {
            Force = Vector3.Zero;
            Torque = Vector3.Zero;
            Gyro = Vector3.Zero;
        }

        public void Update(float dt)
        {
            if (dirty)
            {
                dirty = false;
                RecalcStaticMass();
                CalcCofG();
                CalcInertia();
            }
        }

        public void AddImpulse(Vector3 force, Vector3 position)
        {
            Force += force;

            // For a force F at position X, the torque about the c.g C is: torque = F cross (C - X)
            Vector3 v = Cg - position;
            Vector3 t = Vector3.Cross(force, v);
            Torque += t;
        }

        public Vector3 GetAcceleration(Vector3 position)
        {
            Vector3 result = Force / TotalMass;

            // Turn the "spin" vector into a normalized spin axis "a" and a radians/sec scalar "rate".
            float rate = Spin.Length();
           
            if (rate > 0)
            {
                Vector3 v = Cg - position;
                Vector3 a = Vector3.Normalize(Spin);
                a *= Vector3.Dot(v, a);
                v += a;

                // Now v contains the vector from pos to the rotation axis.
                // Multiply by the square of the rotation rate to get the linear acceleration.
                v *= rate * rate;
                result += v;
            }
            return result;

        }

        public Vector3 GetAngularAcceleration()
        {
            // Compute "tau" as the externally applied torque, plus the counter-torque due to the internal gyro.
            Vector3 tao = Vector3.Cross(Gyro, Spin);
            tao += Torque;

            // Now work the equation of motion.  
            Vector3 result = Vector3.Transform(Spin, TI);
            result = Vector3.Cross(Spin, result);
            result += tao;
            result = Vector3.Transform(result, InverseT);

            return result;
        }

        public Vector3 GetVelocity(Vector3 position, Vector3 rotation)
        {
            Vector3 dp = position - Cg;
            return Vector3.Cross(rotation, dp);
        }

    }
}
