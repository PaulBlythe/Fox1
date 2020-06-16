using System;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;


namespace GuruEngine.Helpers
{
    public enum MatrixAxis
    {
        /// <summary>
        /// The X-axis of a <see cref="Microsoft.Xna.Framework.Matrix" />.
        /// </summary>
        X,

        /// <summary>
        /// The X-axis of a <see cref="Microsoft.Xna.Framework.Matrix" />.
        /// </summary>
        Y,

        /// <summary>
        /// The X-axis of a <see cref="Microsoft.Xna.Framework.Matrix" />.
        /// </summary>
        Z,

        /// <summary>
        /// The translation axis of a <see cref="Microsoft.Xna.Framework.Matrix" />.
        /// </summary>
        Translation
    }

    /// <summary>
	/// Represents a component of a <see cref="Microsoft.Xna.Framework.Vector3" />.
	/// </summary>
	public enum VectorIndex
    {
        /// <summary>
        /// Not the x, y or z-component of a <see cref="Microsoft.Xna.Framework.Vector3" />.
        /// </summary>
        None = -1,

        /// <summary>
        /// The X-component of a <see cref="Microsoft.Xna.Framework.Vector3" />.
        /// </summary>
        X,

        /// <summary>
        /// The Y-component of a <see cref="Microsoft.Xna.Framework.Vector3" />. 
        /// </summary>
        Y,

        /// <summary>
        /// The Z-component of a <see cref="Microsoft.Xna.Framework.Vector3" />.
        /// </summary>
        Z
    }

    /// <summary>
	/// Specifies the corner type of a box shape.
	/// </summary>
	public enum BoxCornerType
    {
        /// <summary>
        /// The left side of a box.  Use as a mask.
        /// </summary>
        Left = 0,

        /// <summary>
        /// The right side of a box.  Use as a mask.
        /// </summary>
        Right = 1,

        /// <summary>
        /// The top side of a box.  Use as a mask.
        /// </summary>
        Top = 0,

        /// <summary>
        /// The bottom side of a box.  Use as a mask.
        /// </summary>
        Bottom = 1,

        /// <summary>
        /// The front side of a box.  Use as a mask.
        /// </summary>
        Front = 0,

        /// <summary>
        /// The back size of a box.  Use as a mask.
        /// </summary>
        Back = 1,

        /// <summary>
        /// The left-upper-front corner of a box.
        /// </summary>
        LeftTopFront = (Left << 2) + (Top << 1) + Front,

        /// <summary>
        /// The left-upper-back corner of a box.
        /// </summary>
        LeftTopBack = (Left << 2) + (Top << 1) + Back,

        /// <summary>
        /// The left-lower-front corner of a box.
        /// </summary>
        LeftBottomFront = (Left << 2) + (Bottom << 1) + Front,

        /// <summary>
        /// The left-lower-back corner of a box.
        /// </summary>
        LeftBottomBack = (Left << 2) + (Bottom << 1) + Back,

        /// <summary>
        /// The right-upper-front corner of a box.
        /// </summary>
        RightTopFront = (Right << 2) + (Top << 1) + Front,

        /// <summary>
        /// The right-upper-back corner of a box.
        /// </summary>
        RightTopBack = (Right << 2) + (Top << 1) + Back,

        /// <summary>
        /// The right-lower-front corner of a box.
        /// </summary>
        RightBottomFront = (Right << 2) + (Bottom << 1) + Front,

        /// <summary>
        /// The right-lower-back corner of a box
        /// </summary>
        RightBottomBack = (Right << 2) + (Bottom << 1) + Back,
    }

    public class MathsHelper
    {
        #region Matrix maths
        public static readonly Matrix Zero = new Matrix();

        /// <summary>
		/// Calculates the inverse of a <see cref="Matrix.Xna.Framework.Matrix"/>, ignoring the 4th row and column.
		/// </summary>
		/// <param name="matrix">Source <see cref="Microsoft.Xna.Framework.Matrix"/>.</param>
		/// <returns>The inverse of the <paramref name="matrix"/>.</returns>
		public static Matrix Invert33(Matrix matrix)
        {
            // Calculate reusable values.
            float m11m22 = matrix.M11 * matrix.M22;
            float m11m32 = matrix.M11 * matrix.M32;
            float m21m12 = matrix.M21 * matrix.M12;
            float m31m12 = matrix.M31 * matrix.M12;
            float m21m13 = matrix.M21 * matrix.M13;
            float m31m13 = matrix.M31 * matrix.M13;

            // Calculate the determinant.
            float determinant =
                m11m22 * matrix.M33 -
                m11m32 * matrix.M23 -
                m21m12 * matrix.M33 +
                m31m12 * matrix.M23 +
                m21m13 * matrix.M32 -
                m31m13 * matrix.M22;



            // Get the determinant reciprocal.
            float inverse = 1.0f / determinant;

            // Create a 3 x 3 inverse matrix from the specified.  Use identity for 4th-dimension values.
            Matrix result = MathsHelper.Zero;
            result.M11 = (matrix.M22 * matrix.M33 - matrix.M32 * matrix.M23) * inverse;
            result.M12 = -(matrix.M12 * matrix.M33 - matrix.M32 * matrix.M13) * inverse;
            result.M13 = (matrix.M12 * matrix.M23 - matrix.M22 * matrix.M13) * inverse;
            result.M14 = 0.0f;

            result.M21 = -(matrix.M21 * matrix.M33 - matrix.M31 * matrix.M23) * inverse;
            result.M22 = (matrix.M11 * matrix.M33 - m31m13) * inverse;
            result.M23 = -(matrix.M11 * matrix.M23 - m21m13) * inverse;
            result.M24 = 0.0f;

            result.M31 = (matrix.M21 * matrix.M32 - matrix.M31 * matrix.M22) * inverse;
            result.M32 = -(m11m32 - m31m12) * inverse;
            result.M33 = (m11m22 - m21m12) * inverse;
            result.M34 = 0.0f;

            result.M41 = 0.0f;
            result.M42 = 0.0f;
            result.M43 = 0.0f;
            result.M44 = 1.0f;

            // Return the result.
            return result;
        }

        /// <summary>
		/// Calculates the inverse of a <see cref="Matrix.Xna.Framework.Matrix"/>, ignoring the 4th row and column.
		/// </summary>
		/// <param name="matrix">Source <see cref="Microsoft.Xna.Framework.Matrix"/>.</param>
		/// <param name="result">The inverse of the <paramref name="matrix"/>.</param>
		public static void Invert33(ref Matrix matrix, out Matrix result)
        {
            // Calculate reusable values.
            float m11m22 = matrix.M11 * matrix.M22;
            float m11m32 = matrix.M11 * matrix.M32;
            float m21m12 = matrix.M21 * matrix.M12;
            float m31m12 = matrix.M31 * matrix.M12;
            float m21m13 = matrix.M21 * matrix.M13;
            float m31m13 = matrix.M31 * matrix.M13;

            // Calculate the determinant.
            float determinant =
                m11m22 * matrix.M33 -
                m11m32 * matrix.M23 -
                m21m12 * matrix.M33 +
                m31m12 * matrix.M23 +
                m21m13 * matrix.M32 -
                m31m13 * matrix.M22;



            // Get the determinant reciprocal.
            float inverse = 1.0f / determinant;

            // Create a 3 x 3 inverse matrix from the specified.  Use identity for 4th-dimension values.

            result.M11 = (matrix.M22 * matrix.M33 - matrix.M32 * matrix.M23) * inverse;
            result.M12 = -(matrix.M12 * matrix.M33 - matrix.M32 * matrix.M13) * inverse;
            result.M13 = (matrix.M12 * matrix.M23 - matrix.M22 * matrix.M13) * inverse;
            result.M14 = 0.0f;

            result.M21 = -(matrix.M21 * matrix.M33 - matrix.M31 * matrix.M23) * inverse;
            result.M22 = (matrix.M11 * matrix.M33 - m31m13) * inverse;
            result.M23 = -(matrix.M11 * matrix.M23 - m21m13) * inverse;
            result.M24 = 0.0f;

            result.M31 = (matrix.M21 * matrix.M32 - matrix.M31 * matrix.M22) * inverse;
            result.M32 = -(m11m32 - m31m12) * inverse;
            result.M33 = (m11m22 - m21m12) * inverse;
            result.M34 = 0.0f;

            result.M41 = 0.0f;
            result.M42 = 0.0f;
            result.M43 = 0.0f;
            result.M44 = 1.0f;
        }

        /// <summary>
        /// Orthonormalizes the specified <see cref="Matrix"/>.
        /// </summary>
        /// <param name="matrix">The source matrix.</param>
        public static void Orthonormalize(ref Matrix matrix)
        {
            // Store matrix values.
            float m11 = matrix.M11;
            float m12 = matrix.M12;
            float m13 = matrix.M13;

            float m21 = matrix.M21;
            float m22 = matrix.M22;
            float m23 = matrix.M23;

            float m31 = matrix.M31;
            float m32 = matrix.M32;
            float m33 = matrix.M33;

            // x
            float lengthSquared0 = m11 * m11 + m12 * m12 + m13 * m13;
            float inverseLengthSquared0 = 1.0f / lengthSquared0;

            float inverseLength0 = 1.0f / (float)Math.Sqrt(lengthSquared0);

            m11 = m11 * inverseLength0;
            m12 = m12 * inverseLength0;
            m13 = m13 * inverseLength0;

            // y
            float dot0 = m11 * m21 + m12 * m22 + m13 * m23;
            m21 = m21 - dot0 * m11 * inverseLengthSquared0;
            m22 = m22 - dot0 * m12 * inverseLengthSquared0;
            m23 = m23 - dot0 * m13 * inverseLengthSquared0;

            float lengthSquared1 = m21 * m21 + m22 * m22 + m23 * m23;
            float inverseLengthSquared1 = 1.0f / lengthSquared1;

            float inverseLength1 = 1.0f / (float)Math.Sqrt(lengthSquared1);
            m21 = m21 * inverseLength1;
            m22 = m22 * inverseLength1;
            m23 = m23 * inverseLength1;

            // z
            dot0 = m11 * m31 + m12 * m32 + m13 * m33;
            float dot1 = m21 * m31 + m22 * m32 + m23 * m33;
            m31 = m31 - dot0 * m11 * inverseLengthSquared0 - dot1 * m21 * inverseLengthSquared1;
            m32 = m32 - dot0 * m12 * inverseLengthSquared0 - dot1 * m22 * inverseLengthSquared1;
            m33 = m33 - dot0 * m13 * inverseLengthSquared0 - dot1 * m23 * inverseLengthSquared1;

            float lengthSquared2 = m31 * m31 + m32 * m32 + m33 * m33;

            float inverseLength2 = 1.0f / (float)Math.Sqrt(lengthSquared2);

            m31 = m31 * inverseLength2;
            m32 = m32 * inverseLength2;
            m33 = m33 * inverseLength2;

            // Restore matrix values.
            matrix.M11 = m11;
            matrix.M12 = m12;
            matrix.M13 = m13;

            matrix.M21 = m21;
            matrix.M22 = m22;
            matrix.M23 = m23;

            matrix.M31 = m31;
            matrix.M32 = m32;
            matrix.M33 = m33;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="matrix1"></param>
        /// <param name="matrix2"></param>
        /// <param name="result"></param>
        public static void ConvertBasis33(ref Matrix matrix1, ref Matrix matrix2, out Matrix result)
        {
            float m11 = matrix2.M11 * matrix1.M11 + matrix2.M21 * matrix1.M12 + matrix2.M31 * matrix1.M13;
            float m12 = matrix2.M11 * matrix1.M21 + matrix2.M21 * matrix1.M22 + matrix2.M31 * matrix1.M23;
            float m13 = matrix2.M11 * matrix1.M31 + matrix2.M21 * matrix1.M32 + matrix2.M31 * matrix1.M33;

            float m21 = matrix2.M12 * matrix1.M11 + matrix2.M22 * matrix1.M12 + matrix2.M32 * matrix1.M13;
            float m22 = matrix2.M12 * matrix1.M21 + matrix2.M22 * matrix1.M22 + matrix2.M32 * matrix1.M23;
            float m23 = matrix2.M12 * matrix1.M31 + matrix2.M22 * matrix1.M32 + matrix2.M32 * matrix1.M33;

            float m31 = matrix2.M13 * matrix1.M11 + matrix2.M23 * matrix1.M12 + matrix2.M33 * matrix1.M13;
            float m32 = matrix2.M13 * matrix1.M21 + matrix2.M23 * matrix1.M22 + matrix2.M33 * matrix1.M23;
            float m33 = matrix2.M13 * matrix1.M31 + matrix2.M23 * matrix1.M32 + matrix2.M33 * matrix1.M33;

            result.M11 = m11 * matrix2.M11 + m12 * matrix2.M21 + m13 * matrix2.M31;
            result.M21 = m11 * matrix2.M12 + m12 * matrix2.M22 + m13 * matrix2.M32;
            result.M31 = m11 * matrix2.M13 + m12 * matrix2.M23 + m13 * matrix2.M33;
            result.M41 = 0.0f;

            result.M12 = m21 * matrix2.M11 + m22 * matrix2.M21 + m23 * matrix2.M31;
            result.M22 = m21 * matrix2.M12 + m22 * matrix2.M22 + m23 * matrix2.M32;
            result.M32 = m21 * matrix2.M13 + m22 * matrix2.M23 + m23 * matrix2.M33;
            result.M42 = 0.0f;

            result.M13 = m31 * matrix2.M11 + m32 * matrix2.M21 + m33 * matrix2.M31;
            result.M23 = m31 * matrix2.M12 + m32 * matrix2.M22 + m33 * matrix2.M32;
            result.M33 = m31 * matrix2.M13 + m32 * matrix2.M23 + m33 * matrix2.M33;
            result.M43 = 0.0f;

            result.M14 = 0.0f;
            result.M24 = 0.0f;
            result.M34 = 0.0f;
            result.M44 = 1.0f;
        }

        /// <summary>
		/// 
		/// </summary>
		/// <param name="matrix1"></param>
		/// <param name="matrix2"></param>
		/// <returns></returns>
		public static Matrix ConvertBasis33(Matrix matrix1, Matrix matrix2)
        {
            float m11 = matrix2.M11 * matrix1.M11 + matrix2.M21 * matrix1.M12 + matrix2.M31 * matrix1.M13;
            float m12 = matrix2.M11 * matrix1.M21 + matrix2.M21 * matrix1.M22 + matrix2.M31 * matrix1.M23;
            float m13 = matrix2.M11 * matrix1.M31 + matrix2.M21 * matrix1.M32 + matrix2.M31 * matrix1.M33;

            float m21 = matrix2.M12 * matrix1.M11 + matrix2.M22 * matrix1.M12 + matrix2.M32 * matrix1.M13;
            float m22 = matrix2.M12 * matrix1.M21 + matrix2.M22 * matrix1.M22 + matrix2.M32 * matrix1.M23;
            float m23 = matrix2.M12 * matrix1.M31 + matrix2.M22 * matrix1.M32 + matrix2.M32 * matrix1.M33;

            float m31 = matrix2.M13 * matrix1.M11 + matrix2.M23 * matrix1.M12 + matrix2.M33 * matrix1.M13;
            float m32 = matrix2.M13 * matrix1.M21 + matrix2.M23 * matrix1.M22 + matrix2.M33 * matrix1.M23;
            float m33 = matrix2.M13 * matrix1.M31 + matrix2.M23 * matrix1.M32 + matrix2.M33 * matrix1.M33;

            Matrix result = MathsHelper.Zero;

            result.M11 = m11 * matrix2.M11 + m12 * matrix2.M21 + m13 * matrix2.M31;
            result.M21 = m11 * matrix2.M12 + m12 * matrix2.M22 + m13 * matrix2.M32;
            result.M31 = m11 * matrix2.M13 + m12 * matrix2.M23 + m13 * matrix2.M33;
            result.M41 = 0.0f;

            result.M12 = m21 * matrix2.M11 + m22 * matrix2.M21 + m23 * matrix2.M31;
            result.M22 = m21 * matrix2.M12 + m22 * matrix2.M22 + m23 * matrix2.M32;
            result.M32 = m21 * matrix2.M13 + m22 * matrix2.M23 + m23 * matrix2.M33;
            result.M42 = 0.0f;

            result.M13 = m31 * matrix2.M11 + m32 * matrix2.M21 + m33 * matrix2.M31;
            result.M23 = m31 * matrix2.M12 + m32 * matrix2.M22 + m33 * matrix2.M32;
            result.M33 = m31 * matrix2.M13 + m32 * matrix2.M23 + m33 * matrix2.M33;
            result.M43 = 0.0f;

            result.M14 = 0.0f;
            result.M24 = 0.0f;
            result.M34 = 0.0f;
            result.M44 = 1.0f;

            return result;
        }

        public static Vector3 GetAxis(Matrix matrix, MatrixAxis axis)
        {
            switch (axis)
            {
                case MatrixAxis.X:
                    {
                        return new Vector3(matrix.M11, matrix.M12, matrix.M13);
                    }
                case MatrixAxis.Y:
                    {
                        return new Vector3(matrix.M21, matrix.M22, matrix.M23);
                    }
                case MatrixAxis.Z:
                    {
                        return new Vector3(matrix.M31, matrix.M32, matrix.M33);
                    }
            }
            return new Vector3(matrix.M41, matrix.M42, matrix.M43);
        }

        /// <summary>
		/// 
		/// </summary>
		/// <param name="matrix"></param>
		/// <param name="offset"></param>
		/// <param name="mass"></param>
		/// <returns></returns>
		public static Matrix TransferAxis(Matrix matrix, Vector3 offset, float mass)
        {
            // Hold onto calculations used more than once.
            float m12 = matrix.M12 - mass * offset.X * offset.Y;
            float m23 = matrix.M23 - mass * offset.Y * offset.Z;
            float m31 = matrix.M31 - mass * offset.Z * offset.X;

            Matrix result = Zero;

            // Apply the transfer axis theorem.
            result.M11 = matrix.M11 + mass * (offset.Y * offset.Y + offset.Z * offset.Z);
            result.M12 = m12;
            result.M13 = m31;
            result.M14 = 0.0f;

            result.M21 = m12;
            result.M22 = matrix.M22 + mass * (offset.Z * offset.Z + offset.X * offset.X);
            result.M23 = m23;
            result.M24 = 0.0f;

            result.M31 = m31;
            result.M32 = m23;
            result.M33 = matrix.M33 + mass * (offset.X * offset.X + offset.Y * offset.Y);
            result.M34 = 0.0f;

            result.M41 = 0.0f;
            result.M42 = 0.0f;
            result.M43 = 0.0f;
            result.M44 = 1.0f;

            return result;
        }
        #endregion

        #region Bounding box
        public static BoundingBox EmptyBB = new BoundingBox(new Vector3(float.MaxValue), new Vector3(float.MinValue));
        public static BoundingBox InfiniteBB = new BoundingBox(new Vector3(float.MinValue), new Vector3(float.MaxValue));

        public static Vector3 GetBoundingBoxCentre(BoundingBox value)
        {
            return value.Min + (value.Max - value.Min) * 0.5f;
        }

        public static Vector3 GetBoundingBoxExtents(BoundingBox value)
        {
            return (value.Max - value.Min) * 0.5f;
        }

        /// <summary>
		/// Tests whether the a <see cref="Microsoft.Xna.Framework.BoundingBox"/> contains another <see cref="Microsoft.Xna.Framework.BoundingBox"/>.
		/// </summary>
		/// <param name="box0">The source <see cref="Microsoft.Xna.Framework.BoundingBox"/>.</param>
		/// <param name="box1">The <see cref="Microsoft.Xna.Framework.BoundingBox"/> to test overlap.</param>
		/// <param name="tolerance">The testing tolerance.</param>
		/// <returns>Returns an enumeration indicating the extent of overlap</returns>
        public static ContainmentType BoundingBoxContains(BoundingBox box0, BoundingBox box1, float tolerance)
        {
            if (box0.Max.X < box1.Min.X - tolerance || box0.Min.X > box1.Max.X + tolerance)
            {
                return ContainmentType.Disjoint;
            }

            if (box0.Max.Y < box1.Min.Y - tolerance || box0.Min.Y > box1.Max.Y + tolerance)
            {
                return ContainmentType.Disjoint;
            }

            if (box0.Max.Z < box1.Min.Z - tolerance || box0.Min.Z > box1.Max.Z + tolerance)
            {
                return ContainmentType.Disjoint;
            }

            if (box0.Min.X - tolerance <= box1.Min.X &&
                box0.Max.X + tolerance >= box1.Max.X &&
                box0.Min.Y - tolerance <= box1.Min.Y &&
                box0.Max.Y + tolerance >= box1.Max.Y &&
                box0.Min.Z - tolerance <= box1.Min.Z &&
                box0.Max.Z + tolerance >= box1.Max.Z)
            {
                return ContainmentType.Contains;
            }

            return ContainmentType.Intersects;
        }

        /// <summary>
        /// Gets the side lengths of a <see cref="BoundingBox"/>.
        /// </summary>
        /// <param name="value">The source <see cref="BoundingBox"/>.</param>
        /// <param name="result">The side lenghs of the specified <see cref="BoundingBox"/>.</param>
        public static void GetBoundingBoxSideLengths(ref BoundingBox value, out Vector3 result)
        {
            result = value.Max - value.Min;
        }

        #endregion

        #region Vector3
        public static float GetVector3Component(Vector3 value, VectorIndex index)
        {
            switch(index)
            {
                case VectorIndex.X:
                    return value.X;
                case VectorIndex.Y:
                    return value.Y;
                case VectorIndex.Z:
                    return value.Z;
            }
            return 0;
        }

        /// <summary>
		/// Returns the maximum component of the specified <see cref="Microsoft.Xna,Framework.Vector3"/>.
		/// </summary>
		/// <param name="vector">The source <see cref="Microsoft.Xna.Framework.Vector3"/>.</param>
		/// <param name="result">The maximum value of the x, y and z components of <paramref name="vector"/>.</param>
		public static float MaxComponent(ref Vector3 vector)
        {
            return Math.Max(vector.X, Math.Max(vector.Y, vector.Z));
        }

        /// <summary>
		/// Returns the maximum component index of the specified <see cref="Microsoft.Xna,Framework.Vector3"/>.
		/// </summary>
		/// <param name="vector">The source <see cref="Microsoft.Xna.Framework.Vector3"/>.</param>
		/// <returns>The maximum index of the x, y and z components of <paramref name="vector"/>.</returns>
		public static VectorIndex MaxComponentIndex(Vector3 vector)
        {
            float temp = vector.X;
            VectorIndex result = VectorIndex.X;

            if (vector.X < vector.Y)
            {
                temp = vector.Y;
                result = VectorIndex.Y;
            }

            if (temp < vector.Z)
            {
                result = VectorIndex.Z;
            }

            return result;
        }
        #endregion

        #region Quaternions and angles
        public static float GetHeading(Quaternion q)
        {
            return (float) Math.Atan2(2.0 * (q.Y * q.Z + q.W * q.X), q.W * q.W - q.X * q.X - q.Y * q.Y + q.Z * q.Z);
        }

        public static Vector2 Rotate2D(Vector2 v, float a)
        {
            Vector2 result = new Vector2();
            result.X = (float)(Math.Cos(a) * v.X) - (float)(Math.Sin(a) * v.Y);
            result.Y = (float)(Math.Cos(a) * v.Y) + (float)(Math.Sin(a) * v.X);
            return result;
        }

        public static Vector3 QuaternionToYawPitchRoll(Quaternion q)
        {
            const float Epsilon = 0.0009765625f;
            const float Threshold = 0.5f - Epsilon;

            float yaw;
            float pitch;
            float roll;

            float XY = q.X * q.Y;
            float ZW = q.Z * q.W;

            float TEST = XY + ZW;

            if (TEST < -Threshold || TEST > Threshold)
            {

                int sign = Math.Sign(TEST);

                yaw = sign * 2 * (float)Math.Atan2(q.X, q.W);

                pitch = sign * MathHelper.PiOver2;

                roll = 0;

            }
            else
            {

                float XX = q.X * q.X;
                float XZ = q.X * q.Z;
                float XW = q.X * q.W;

                float YY = q.Y * q.Y;
                float YW = q.Y * q.W;
                float YZ = q.Y * q.Z;

                float ZZ = q.Z * q.Z;

                yaw = (float)Math.Atan2(2 * YW - 2 * XZ, 1 - 2 * YY - 2 * ZZ);

                pitch = (float)Math.Atan2(2 * XW - 2 * YZ, 1 - 2 * XX - 2 * ZZ);

                roll = (float)Math.Asin(2 * TEST);

            }

            return new Vector3(yaw, pitch, roll);

        }

        public static float GetAnglesDegrees(float rads)
        {
            float deg = MathHelper.ToDegrees(rads);
            if (deg < 0)
                deg = 360 + deg;
            return deg;
        }
        #endregion

        #region Unit converters
        public static float MpsToKnots(float mps)
        {
            return mps * 1.94384f;
        }

        public static float KnotToMps(float knots)
        {
            return knots * 0.514444f;
        }

        public static float MetresToFeet(float d)
        {
            return d * 3.28084f;
        }

        public static float FeetToMetres(float d)
        {
            return d * 0.3048f;
        }

        public static float SlugToLbs(float v)
        {
            return v * 32.174049f;
        }

        public static float LbsToSlug(float v)
        {
            return v * (1.0f / 32.174049f);
        }

        /// <summary>
        /// Converts from degrees Rankine to degrees Kelvin.
        /// </summary>
        /// <param name="rankine">The temperature in degrees Rankine</param>
        /// <returns>The temperature in Kelvin.</returns>
        public static double RankineToKelvin(double rankine)
        {
            return rankine / 1.8;
        }

        public static double CelsiusToFahrenheit(double celsius)
        {
            return celsius * 1.8 + 32.0;
        }

        public static double FahrenheitToCelsius(double fahrenheit)
        {
            return (fahrenheit - 32.0) / 1.8;
        }

        public static double KelvinToFahrenheit(double kelvin)
        {
            return 1.8 * kelvin - 459.4;
        }

        /// <summary>
        /// Converts from degrees Rankine to degrees Celsius.
        /// </summary>
        /// <param name="rankine">The temperature in degrees Rankine.</param>
        /// <returns>The temperature in Celsius.</returns>
        public static double RankineToCelsius(double rankine)
        {
            return (rankine - 491.67) / 1.8;
        }

        #endregion

        #region Table lookup
        public static double TableLookup2D(double rowKey, double colKey, double[] column, double[] row, double[,] data)
        {
            double rFactor, cFactor, col1temp, col2temp, Value;
            int r = 1;
            int c = 1;

            while (r > 1 && row[r - 1] > rowKey) { r--; }
            while (r < row.Length - 1 && row[r] < rowKey) { r++; }

            while (c > 1 && column[c - 1] > colKey) { c--; }
            while (c < column.Length - 1 && column[c] < colKey) { c++; }


            rFactor = (rowKey - row[r - 1]) / (row[r] - row[r - 1]);
            cFactor = (colKey - column[c - 1]) / (column[c] - column[c - 1]);

            if (rFactor > 1.0) rFactor = 1.0;
            else if (rFactor < 0.0) rFactor = 0.0;

            if (cFactor > 1.0) cFactor = 1.0;
            else if (cFactor < 0.0) cFactor = 0.0;

            col1temp = rFactor * (data[r, c - 1] - data[r - 1, c - 1]) + data[r - 1, c - 1];
            col2temp = rFactor * (data[r, c] - data[r - 1, c]) + data[r - 1, c];

            Value = col1temp + cFactor * (col2temp - col1temp);

            return Value;

        }

        public static double TableLookup1D(double colkey, double[] column, double[] data)
        {
            double cFactor, col1temp, Value;
            int c = 1;


            while (c > 1 && column[c - 1] > colkey) { c--; }
            while (c < column.Length - 1 && column[c] < colkey) { c++; }

            cFactor = (colkey - column[c - 1]) / (column[c] - column[c - 1]);

            if (cFactor > 1.0) cFactor = 1.0;
            else if (cFactor < 0.0) cFactor = 0.0;

            col1temp = cFactor * (data[c] - data[c - 1]) + data[c - 1];

            Value = col1temp;

            return Value;
        }
        #endregion

        public static double square_signed(double value)
        {
            if (value < 0)
                return value * value * -1;
            else
                return value * value;
        }

        public static double Lag(double input, double output, double time, double dt)
        {
            if (input == output)
                return output;

            double rate = 1.0 / time;
            double ThisDt = Math.Abs((input - output) / rate);
            if (dt < ThisDt)
            {
                ThisDt = dt;
                if (output < input)
                    output += ThisDt * rate;
                else
                    output -= ThisDt * rate;
            }
            else
                // Handle this case separate to make sure the termination condition
                // is met even in inexact arithmetics ...
                output = input;

            return output;

        }

        /// <summary>
        /// A lag filter. Used to control the rate at which values are allowed to change.
        /// </summary>
        /// <param name="var">a pointer to a variable of type double</param>
        /// <param name="target">the desired (target) value</param>
        /// <param name="accel">the rate, per second, the value may increase</param>
        /// <param name="decel">the rate, per second, the value may decrease  </param>
        /// <param name="dt">time slice</param>
        /// <returns></returns>
        public static double Seek(ref double var, double target, double accel, double decel, double dt)
        {
            double v = var;
            if (v > target)
            {
                v -= dt * decel;
                if (v < target) v = target;
            }
            else if (v < target)
            {
                v += dt * accel;
                if (v > target) v = target;
            }
            return v;
        }

        public static double Range(double x, double r)
        {
            return x - (r * Math.Floor(x / r));
        }

        public static float BiLerp(float u, float v, float q, float r, float s, float t)
        {
            return MathHelper.Lerp(MathHelper.Lerp(u, v, s), MathHelper.Lerp(q, r, s), t);
        }

        public static int Modulo(int value, int modus)
        {
            value -= modus * (int)(value / modus);

            if (value < 0)
            {
                value += modus;
            }

            return value;
        }

        public const float Epsilon = 1.0e-6f;

        public static Vector4 Round(Vector4 value)
        {
            return new Vector4(
                (float)Math.Round(value.X),
                (float)Math.Round(value.Y),
                (float)Math.Round(value.Z),
                (float)Math.Round(value.W));
        }

        public static float Random(Random rnd, float value, float variation)
        {
            return value + (float)(rnd.NextDouble() * 2.0 - 1.0) * variation;
        }
    }
}
