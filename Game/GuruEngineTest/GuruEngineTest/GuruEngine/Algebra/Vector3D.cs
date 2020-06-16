using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Runtime.Serialization;
using System.Text.RegularExpressions;
using System.Xml.Serialization;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;


namespace GuruEngine.Algebra
{
    /// <summary>
    /// Defines a 3-dimensional vector (double-precision).
    /// </summary>
    /// <remarks>
    /// The three components (x, y, z) are stored with double-precision.
    /// </remarks>
    public struct Vector3D : IEquatable<Vector3D>
    {
        //--------------------------------------------------------------
        #region Constants
        //--------------------------------------------------------------

        /// <summary>
        /// Returns a <see cref="Vector3D"/> with all of its components set to zero.
        /// </summary>
        public static readonly Vector3D Zero = new Vector3D(0, 0, 0);


        /// <summary>
        /// Returns a <see cref="Vector3D"/> with all of its components set to one.
        /// </summary>
        public static readonly Vector3D One = new Vector3D(1, 1, 1);


        /// <summary>
        /// Returns the x unit <see cref="Vector3D"/> (1, 0, 0).
        /// </summary>
        public static readonly Vector3D UnitX = new Vector3D(1, 0, 0);


        /// <summary>
        /// Returns the value2 unit <see cref="Vector3D"/> (0, 1, 0).
        /// </summary>
        public static readonly Vector3D UnitY = new Vector3D(0, 1, 0);


        /// <summary>
        /// Returns the z unit <see cref="Vector3D"/> (0, 0, 1).
        /// </summary>
        public static readonly Vector3D UnitZ = new Vector3D(0, 0, 1);



    /// <summary>
    /// Returns a unit <see cref="Vector3D"/> pointing forward (0, 0, −1).
    /// (Only available in the XNA-compatible build.)
    /// </summary>
    /// <remarks>
    /// <para>
    /// This property is available only in the XNA-compatible build of the 
    /// DigitalRune.Mathematics.dll.
    /// </para>
    /// <para>
    /// <strong>DigitalRune</strong> uses the same coordinate systems as the
    /// <strong>XNA Framework:</strong> model space (object space, local space), world space 
    /// and view space are right-handed coordinate systems where, by default, the positive 
    /// x-axis points to the right, the positive y-axis points up, and the positive z-axis 
    /// points towards the viewer.
    /// </para>
    /// </remarks>
    public static readonly Vector3D Forward = new Vector3D(0, 0, -1);


    /// <summary>
    /// Returns a unit <see cref="Vector3D"/> pointing backward (0, 0, 1).
    /// (Only available in the XNA-compatible build.)
    /// </summary>
    /// <remarks>
    /// <para>
    /// This property is available only in the XNA-compatible build of the 
    /// DigitalRune.Mathematics.dll.
    /// </para>
    /// <para>
    /// <strong>DigitalRune</strong> uses the same coordinate systems as the
    /// <strong>XNA Framework:</strong> model space (object space, local space), world space 
    /// and view space are right-handed coordinate systems where, by default, the positive 
    /// x-axis points to the right, the positive y-axis points up, and the positive z-axis 
    /// points towards the viewer.
    /// </para>
    /// </remarks>
    public static readonly Vector3D Backward = new Vector3D(0, 0, 1);


    /// <summary>
    /// Returns a unit <see cref="Vector3D"/> pointing left (-1, 0, 0).
    /// (Only available in the XNA-compatible build.)
    /// </summary>
    /// <remarks>
    /// <para>
    /// This property is available only in the XNA-compatible build of the 
    /// DigitalRune.Mathematics.dll.
    /// </para>
    /// <para>
    /// <strong>DigitalRune</strong> uses the same coordinate systems as the
    /// <strong>XNA Framework:</strong> model space (object space, local space), world space 
    /// and view space are right-handed coordinate systems where, by default, the positive 
    /// x-axis points to the right, the positive y-axis points up, and the positive z-axis 
    /// points towards the viewer.
    /// </para>
    /// </remarks>
    public static readonly Vector3D Left = new Vector3D(-1, 0, 0);


    /// <summary>
    /// Returns a unit <see cref="Vector3D"/> pointing right (1, 0, 0).
    /// (Only available in the XNA-compatible build.)
    /// </summary>
    /// <remarks>
    /// <para>
    /// This property is available only in the XNA-compatible build of the 
    /// DigitalRune.Mathematics.dll.
    /// </para>
    /// <para>
    /// <strong>DigitalRune</strong> uses the same coordinate systems as the
    /// <strong>XNA Framework:</strong> model space (object space, local space), world space 
    /// and view space are right-handed coordinate systems where, by default, the positive 
    /// x-axis points to the right, the positive y-axis points up, and the positive z-axis 
    /// points towards the viewer.
    /// </para>
    /// </remarks>
    public static readonly Vector3D Right = new Vector3D(1, 0, 0);


    /// <summary>
    /// Returns a unit <see cref="Vector3D"/> pointing up (0, 1, 0).
    /// (Only available in the XNA-compatible build.)
    /// </summary>
    /// <remarks>
    /// <para>
    /// This property is available only in the XNA-compatible build of the 
    /// DigitalRune.Mathematics.dll.
    /// </para>
    /// <para>
    /// <strong>DigitalRune</strong> uses the same coordinate systems as the
    /// <strong>XNA Framework:</strong> model space (object space, local space), world space 
    /// and view space are right-handed coordinate systems where, by default, the positive 
    /// x-axis points to the right, the positive y-axis points up, and the positive z-axis 
    /// points towards the viewer.
    /// </para>
    /// </remarks>
    public static readonly Vector3D Up = new Vector3D(0, 1, 0);


    /// <summary>
    /// Returns a unit <see cref="Vector3D"/> pointing down (0, −1, 0).
    /// (Only available in the XNA-compatible build.)
    /// </summary>
    /// <remarks>
    /// <para>
    /// This property is available only in the XNA-compatible build of the 
    /// DigitalRune.Mathematics.dll.
    /// </para>
    /// <para>
    /// <strong>DigitalRune</strong> uses the same coordinate systems as the
    /// <strong>XNA Framework:</strong> model space (object space, local space), world space 
    /// and view space are right-handed coordinate systems where, by default, the positive 
    /// x-axis points to the right, the positive y-axis points up, and the positive z-axis 
    /// points towards the viewer.
    /// </para>
    /// </remarks>
    public static readonly Vector3D Down = new Vector3D(0, -1, 0);

        #endregion


        //--------------------------------------------------------------
        #region Fields
        //--------------------------------------------------------------

        /// <summary>
        /// The x component.
        /// </summary>
        public double X;

        /// <summary>
        /// The y component.
        /// </summary>
        public double Y;

        /// <summary>
        /// The z component.
        /// </summary>
        public double Z;
        #endregion


        //--------------------------------------------------------------
        #region Properties
        //--------------------------------------------------------------

        /// <summary>
        /// Gets or sets the component at the specified index.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <value>The component at <paramref name="index"/>.</value>
        /// <remarks>
        /// The index is zero based: x = vector[0], y = vector[1], z = vector[2].
        /// </remarks>
        /// <exception cref="ArgumentOutOfRangeException">
        /// The <paramref name="index"/> is out of range.
        /// </exception>
        public double this[int index]
        {
            get
            {
                switch (index)
                {
                    case 0: return X;
                    case 1: return Y;
                    case 2: return Z;
                    default: throw new ArgumentOutOfRangeException("index", "The index is out of range. Allowed values are 0, 1, or 2.");
                }
            }
            set
            {
                switch (index)
                {
                    case 0: X = value; break;
                    case 1: Y = value; break;
                    case 2: Z = value; break;
                    default: throw new ArgumentOutOfRangeException("index", "The index is out of range. Allowed values are 0, 1, or 2.");
                }
            }
        }


        /// <summary>
        /// Gets a value indicating whether a component of the vector is <see cref="double.NaN"/>.
        /// </summary>
        /// <value>
        /// <see langword="true"/> if a component of the vector is <see cref="double.NaN"/>; otherwise, 
        /// <see langword="false"/>.
        /// </value>
        public bool IsNaN
        {
            get { return Double.IsNaN(X) || Double.IsNaN(Y) || Double.IsNaN(Z); }
        }


        /// <summary>
        /// Returns a value indicating whether this vector is normalized (the length is numerically
        /// equal to 1).
        /// </summary>
        /// <value>
        /// <see langword="true"/> if this <see cref="Vector3D"/> is normalized; otherwise, 
        /// <see langword="false"/>.
        /// </value>
        /// <remarks>
        /// <see cref="IsNumericallyNormalized"/> compares the length of this vector against 1.0 using
        /// the default tolerance value (see <see cref="Numeric.EpsilonD"/>).
        /// </remarks>
        public bool IsNumericallyNormalized
        {
            get { return Double.Equals(LengthSquared, 1.0); }
        }


        /// <summary>
        /// Returns a value indicating whether this vector has zero size (the length is numerically
        /// equal to 0).
        /// </summary>
        /// <value>
        /// <see langword="true"/> if this vector is numerically zero; otherwise, 
        /// <see langword="false"/>.
        /// </value>
        /// <remarks>
        /// The length of this vector is compared to 0 using the default tolerance value (see 
        /// <see cref="Numeric.EpsilonD"/>).
        /// </remarks>
        public bool IsNumericallyZero
        {
            get {
                return DoubleClassExtension.IsZero(LengthSquared, (Double.Epsilon * Double.Epsilon));
            }
        }


        /// <summary>
        /// Gets or sets the length of this vector.
        /// </summary>
        /// <returns>The length of the this vector.</returns>
        /// <exception cref="MathematicsException">
        /// The vector has a length of 0. The length cannot be changed.
        /// </exception>
        public double Length
        {
            get
            {
                return Math.Sqrt(X * X + Y * Y + Z * Z);
            }
            set
            {
                double length = Length;
                if (DoubleClassExtension.IsZero(length))
                    throw new Exception("Cannot change length of a vector with length == 0.");

                double scale = value / length;
                X *= scale;
                Y *= scale;
                Z *= scale;
            }
        }


        /// <summary>
        /// Returns the squared length of this vector.
        /// </summary>
        /// <returns>The squared length of this vector.</returns>
        public double LengthSquared
        {
            get
            {
                return X * X + Y * Y + Z * Z;
            }
        }


        /// <summary>
        /// Returns the normalized vector.
        /// </summary>
        /// <value>The normalized vector.</value>
        /// <remarks>
        /// The property does not change this instance. To normalize this instance you need to call 
        /// <see cref="Normalize"/>.
        /// </remarks>
        /// <exception cref="DivideByZeroException">
        /// The length of the vector is zero. The quaternion cannot be normalized.
        /// </exception>
        public Vector3D Normalized
        {
            get
            {
                Vector3D v = this;
                v.Normalize();
                return v;
            }
        }


        /// <summary>
        /// Returns an arbitrary normalized <see cref="Vector3D"/> that is orthogonal to this vector.
        /// </summary>
        /// <value>An arbitrary normalized orthogonal <see cref="Vector3D"/>.</value>
        public Vector3D Orthonormal1
        {
            get
            {
                Vector3D v;
                if (DoubleClassExtension.IsZero(Z) == false)
                {
                    // Orthonormal = (1, 0, 0) x (X, Y, Z)
                    v.X = 0;
                    v.Y = -Z;
                    v.Z = Y;
                }
                else
                {
                    // Orthonormal = (0, 0, 1) x (X, Y, Z)
                    v.X = -Y;
                    v.Y = X;
                    v.Z = 0;
                }
                v.Normalize();
                return v;
            }
        }


        /// <summary>
        /// Gets a normalized orthogonal <see cref="Vector3D"/> that is orthogonal to this 
        /// <see cref="Vector3D"/> and to <see cref="Orthonormal1"/>.
        /// </summary>
        /// <value>
        /// A normalized orthogonal <see cref="Vector3D"/> which is orthogonal to this 
        /// <see cref="Vector3D"/> and to <see cref="Orthonormal1"/>.
        /// </value>
        public Vector3D Orthonormal2
        {
            get
            {
                Vector3D v = Cross(this, Orthonormal1);
                v.Normalize();
                return v;
            }
        }


        /// <summary>
        /// Gets the value of the largest component.
        /// </summary>
        /// <value>The value of the largest component.</value>
        public double LargestComponent
        {
            get
            {
                if (X >= Y && X >= Z)
                    return X;

                if (Y >= Z)
                    return Y;

                return Z;
            }
        }


        /// <summary>
        /// Gets the index (zero-based) of the largest component.
        /// </summary>
        /// <value>The index (zero-based) of the largest component.</value>
        /// <remarks>
        /// <para>
        /// This method returns the index of the component (X, Y or Z) which has the largest value. The 
        /// index is zero-based, i.e. the index of X is 0. 
        /// </para>
        /// <para>
        /// If there are several components with equally large values, the smallest index of these is 
        /// returned.
        /// </para>
        /// </remarks>
        public int IndexOfLargestComponent
        {
            get
            {
                if (X >= Y && X >= Z)
                    return 0;

                if (Y >= Z)
                    return 1;

                return 2;
            }
        }


        /// <summary>
        /// Gets the value of the smallest component.
        /// </summary>
        /// <value>The value of the smallest component.</value>
        public double SmallestComponent
        {
            get
            {
                if (X <= Y && X <= Z)
                    return X;

                if (Y <= Z)
                    return Y;

                return Z;
            }
        }


        /// <summary>
        /// Gets the index (zero-based) of the largest component.
        /// </summary>
        /// <value>The index (zero-based) of the largest component.</value>
        /// <remarks>
        /// <para>
        /// This method returns the index of the component (X, Y or Z) which has the smallest value. The 
        /// index is zero-based, i.e. the index of X is 0. 
        /// </para>
        /// <para>
        /// If there are several components with equally small values, the smallest index of these is 
        /// returned.
        /// </para>
        /// </remarks>
        public int IndexOfSmallestComponent
        {
            get
            {
                if (X <= Y && X <= Z)
                    return 0;

                if (Y <= Z)
                    return 1;

                return 2;
            }
        }
        #endregion


        //--------------------------------------------------------------
        #region Creation & Cleanup
        //--------------------------------------------------------------

        /// <overloads>
        /// <summary>
        /// Initializes a new instance of <see cref="Vector3D"/>.
        /// </summary>
        /// </overloads>
        /// 
        /// <summary>
        /// Initializes a new instance of <see cref="Vector3D"/>.
        /// </summary>
        /// <param name="x">Initial value for the x component.</param>
        /// <param name="y">Initial value for the y component.</param>
        /// <param name="z">Initial value for the z component.</param>
        public Vector3D(double x, double y, double z)
        {
            X = x;
            Y = y;
            Z = z;
        }


        /// <summary>
        /// Initializes a new instance of <see cref="Vector3D"/>.
        /// </summary>
        /// <param name="componentValue">The initial value for 3 the vector components.</param>
        /// <remarks>
        /// All components are set to <paramref name="componentValue"/>.
        /// </remarks>
        public Vector3D(double componentValue)
        {
            X = componentValue;
            Y = componentValue;
            Z = componentValue;
        }


        /// <summary>
        /// Initializes a new instance of <see cref="Vector3D"/>.
        /// </summary>
        /// <param name="components">
        /// Array with the initial values for the components x, y and z.
        /// </param>
        /// <exception cref="IndexOutOfRangeException">
        /// <paramref name="components"/> has less than 3 elements.
        /// </exception>
        /// <exception cref="NullReferenceException">
        /// <paramref name="components"/> must not be <see langword="null"/>.
        /// </exception>
        public Vector3D(double[] components)
        {
            X = components[0];
            Y = components[1];
            Z = components[2];
        }


        /// <summary>
        /// Initializes a new instance of the <see cref="Vector3D"/> class.
        /// </summary>
        /// <param name="components">
        /// List with the initial values for the components x, y and z.
        /// </param>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="components"/> has less than 3 elements.
        /// </exception>
        /// <exception cref="NullReferenceException">
        /// <paramref name="components"/> must not be <see langword="null"/>.
        /// </exception>
        public Vector3D(IList<double> components)
        {
            X = components[0];
            Y = components[1];
            Z = components[2];
        }
        #endregion


        //--------------------------------------------------------------
        #region Interfaces and Overrides
        //--------------------------------------------------------------

        /// <summary>
        /// Returns the hash code for this instance.
        /// </summary>
        /// <returns>A 32-bit signed integer that is the hash code for this instance.</returns>
        public override int GetHashCode()
        {
            // ReSharper disable NonReadonlyFieldInGetHashCode
            unchecked
            {
                int hashCode = X.GetHashCode();
                hashCode = (hashCode * 397) ^ Y.GetHashCode();
                hashCode = (hashCode * 397) ^ Z.GetHashCode();
                return hashCode;
            }
            // ReSharper restore NonReadonlyFieldInGetHashCode
        }


        /// <overloads>
        /// <summary>
        /// Indicates whether a vector and a another object are equal.
        /// </summary>
        /// </overloads>
        /// 
        /// <summary>
        /// Indicates whether this instance and a specified object are equal.
        /// </summary>
        /// <param name="obj">Another object to compare to.</param>
        /// <returns>
        /// <see langword="true"/> if <paramref name="obj"/> and this instance are the same type and
        /// represent the same value; otherwise, <see langword="false"/>.
        /// </returns>
        public override bool Equals(object obj)
        {
            return obj is Vector3D && this == (Vector3D)obj;
        }


        #region IEquatable<Vector3D> Members
        /// <summary>
        /// Indicates whether the current object is equal to another object of the same type.
        /// </summary>
        /// <param name="other">An object to compare with this object.</param>
        /// <returns>
        /// <see langword="true"/> if the current object is equal to the other parameter; otherwise, 
        /// <see langword="false"/>.
        /// </returns>
        public bool Equals(Vector3D other)
        {
            return this == other;
        }
        #endregion


        /// <overloads>
        /// <summary>
        /// Returns the string representation of a vector.
        /// </summary>
        /// </overloads>
        /// 
        /// <summary>
        /// Returns the string representation of this vector.
        /// </summary>
        /// <returns>The string representation of this vector.</returns>
        public override string ToString()
        {
            return ToString(CultureInfo.CurrentCulture);
        }


        /// <summary>
        /// Returns the string representation of this vector using the specified culture-specific format
        /// information.
        /// </summary>
        /// <param name="provider">
        /// An <see cref="IFormatProvider"/> that supplies culture-specific formatting information.
        /// </param>
        /// <returns>The string representation of this vector.</returns>
        public string ToString(IFormatProvider provider)
        {
            return string.Format(provider, "({0}; {1}; {2})", X, Y, Z);
        }
        #endregion


        //--------------------------------------------------------------
        #region Overloaded Operators
        //--------------------------------------------------------------

        /// <summary>
        /// Negates a vector.
        /// </summary>
        /// <param name="vector">The vector.</param>
        /// <returns>The negated vector.</returns>
        public static Vector3D operator -(Vector3D vector)
        {
            vector.X = -vector.X;
            vector.Y = -vector.Y;
            vector.Z = -vector.Z;
            return vector;
        }


        /// <summary>
        /// Negates a vector.
        /// </summary>
        /// <param name="vector">The vector.</param>
        /// <returns>The negated vector.</returns>
        public static Vector3D Negate(Vector3D vector)
        {
            vector.X = -vector.X;
            vector.Y = -vector.Y;
            vector.Z = -vector.Z;
            return vector;
        }


        /// <summary>
        /// Adds two vectors.
        /// </summary>
        /// <param name="vector1">The first vector.</param>
        /// <param name="vector2">The second vector.</param>
        /// <returns>The sum of the two vectors.</returns>
        public static Vector3D operator +(Vector3D vector1, Vector3D vector2)
        {
            vector1.X += vector2.X;
            vector1.Y += vector2.Y;
            vector1.Z += vector2.Z;
            return vector1;
        }


        /// <summary>
        /// Adds two vectors.
        /// </summary>
        /// <param name="vector1">The first vector.</param>
        /// <param name="vector2">The second vector.</param>
        /// <returns>The sum of the two vectors.</returns>
        public static Vector3D Add(Vector3D vector1, Vector3D vector2)
        {
            vector1.X += vector2.X;
            vector1.Y += vector2.Y;
            vector1.Z += vector2.Z;
            return vector1;
        }


        /// <summary>
        /// Subtracts a vector from a vector.
        /// </summary>
        /// <param name="minuend">The first vector (minuend).</param>
        /// <param name="subtrahend">The second vector (subtrahend).</param>
        /// <returns>The difference of the two vectors.</returns>
        public static Vector3D operator -(Vector3D minuend, Vector3D subtrahend)
        {
            minuend.X -= subtrahend.X;
            minuend.Y -= subtrahend.Y;
            minuend.Z -= subtrahend.Z;
            return minuend;
        }


        /// <summary>
        /// Subtracts a vector from a vector.
        /// </summary>
        /// <param name="minuend">The first vector (minuend).</param>
        /// <param name="subtrahend">The second vector (subtrahend).</param>
        /// <returns>The difference of the two vectors.</returns>
        public static Vector3D Subtract(Vector3D minuend, Vector3D subtrahend)
        {
            minuend.X -= subtrahend.X;
            minuend.Y -= subtrahend.Y;
            minuend.Z -= subtrahend.Z;
            return minuend;
        }


        /// <overloads>
        /// <summary>
        /// Multiplies a vector by a scalar or a vector.
        /// </summary>
        /// </overloads>
        /// 
        /// <summary>
        /// Multiplies a vector by a scalar.
        /// </summary>
        /// <param name="vector">The vector.</param>
        /// <param name="scalar">The scalar.</param>
        /// <returns>The vector with each component multiplied by <paramref name="scalar"/>.</returns>
        public static Vector3D operator *(Vector3D vector, double scalar)
        {
            vector.X *= scalar;
            vector.Y *= scalar;
            vector.Z *= scalar;
            return vector;
        }


        /// <summary>
        /// Multiplies a vector by a scalar.
        /// </summary>
        /// <param name="vector">The vector.</param>
        /// <param name="scalar">The scalar.</param>
        /// <returns>The vector with each component multiplied by <paramref name="scalar"/>.</returns>
        public static Vector3D operator *(double scalar, Vector3D vector)
        {
            vector.X *= scalar;
            vector.Y *= scalar;
            vector.Z *= scalar;
            return vector;
        }


        /// <overloads>
        /// <summary>
        /// Multiplies a vector by a scalar or a vector.
        /// </summary>
        /// </overloads>
        /// 
        /// <summary>
        /// Multiplies a vector by a scalar.
        /// </summary>
        /// <param name="vector">The vector.</param>
        /// <param name="scalar">The scalar.</param>
        /// <returns>The vector with each component multiplied by <paramref name="scalar"/>.</returns>
        public static Vector3D Multiply(double scalar, Vector3D vector)
        {
            vector.X *= scalar;
            vector.Y *= scalar;
            vector.Z *= scalar;
            return vector;
        }


        /// <summary>
        /// Multiplies the components of two vectors by each other.
        /// </summary>
        /// <param name="vector1">The first vector.</param>
        /// <param name="vector2">The second vector.</param>
        /// <returns>The component-wise product of the two vectors.</returns>
        public static Vector3D operator *(Vector3D vector1, Vector3D vector2)
        {
            vector1.X *= vector2.X;
            vector1.Y *= vector2.Y;
            vector1.Z *= vector2.Z;
            return vector1;
        }


        /// <summary>
        /// Multiplies the components of two vectors by each other.
        /// </summary>
        /// <param name="vector1">The first vector.</param>
        /// <param name="vector2">The second vector.</param>
        /// <returns>The component-wise product of the two vectors.</returns>
        public static Vector3D Multiply(Vector3D vector1, Vector3D vector2)
        {
            vector1.X *= vector2.X;
            vector1.Y *= vector2.Y;
            vector1.Z *= vector2.Z;
            return vector1;
        }


        /// <overloads>
        /// <summary>
        /// Divides the vector by a scalar or a vector.
        /// </summary>
        /// </overloads>
        /// 
        /// <summary>
        /// Divides a vector by a scalar.
        /// </summary>
        /// <param name="vector">The vector.</param>
        /// <param name="scalar">The scalar.</param>
        /// <returns>The vector with each component divided by <paramref name="scalar"/>.</returns>
        public static Vector3D operator /(Vector3D vector, double scalar)
        {
            double f = 1 / scalar;
            vector.X *= f;
            vector.Y *= f;
            vector.Z *= f;
            return vector;
        }


        /// <overloads>
        /// <summary>
        /// Divides the vector by a scalar or a vector.
        /// </summary>
        /// </overloads>
        /// 
        /// <summary>
        /// Divides a vector by a scalar.
        /// </summary>
        /// <param name="vector">The vector.</param>
        /// <param name="scalar">The scalar.</param>
        /// <returns>The vector with each component divided by <paramref name="scalar"/>.</returns>
        public static Vector3D Divide(Vector3D vector, double scalar)
        {
            double f = 1 / scalar;
            vector.X *= f;
            vector.Y *= f;
            vector.Z *= f;
            return vector;
        }


        /// <summary>
        /// Divides the components of a vector by the components of another vector.
        /// </summary>
        /// <param name="dividend">The first vector (dividend).</param>
        /// <param name="divisor">The second vector (divisor).</param>
        /// <returns>The component-wise product of the two vectors.</returns>
        public static Vector3D operator /(Vector3D dividend, Vector3D divisor)
        {
            dividend.X /= divisor.X;
            dividend.Y /= divisor.Y;
            dividend.Z /= divisor.Z;
            return dividend;
        }


        /// <summary>
        /// Divides the components of a vector by the components of another vector.
        /// </summary>
        /// <param name="dividend">The first vector (dividend).</param>
        /// <param name="divisor">The second vector (divisor).</param>
        /// <returns>The component-wise division of the two vectors.</returns>
        public static Vector3D Divide(Vector3D dividend, Vector3D divisor)
        {
            dividend.X /= divisor.X;
            dividend.Y /= divisor.Y;
            dividend.Z /= divisor.Z;
            return dividend;
        }


        /// <summary>
        /// Tests if two vectors are equal.
        /// </summary>
        /// <param name="vector1">The first vector.</param>
        /// <param name="vector2">The second vector.</param>
        /// <returns>
        /// <see langword="true"/> if the vectors are equal; otherwise <see langword="false"/>.
        /// </returns>
        /// <remarks>
        /// For the test the corresponding components of the vectors are compared.
        /// </remarks>
        public static bool operator ==(Vector3D vector1, Vector3D vector2)
        {
            return vector1.X == vector2.X
                && vector1.Y == vector2.Y
                && vector1.Z == vector2.Z;
        }


        /// <summary>
        /// Tests if two vectors are not equal.
        /// </summary>
        /// <param name="vector1">The first vector.</param>
        /// <param name="vector2">The second vector.</param>
        /// <returns>
        /// <see langword="true"/> if the vectors are different; otherwise <see langword="false"/>.
        /// </returns>
        /// <remarks>
        /// For the test the corresponding components of the vectors are compared.
        /// </remarks>
        public static bool operator !=(Vector3D vector1, Vector3D vector2)
        {
            return vector1.X != vector2.X
                || vector1.Y != vector2.Y
                || vector1.Z != vector2.Z;
        }


        /// <summary>
        /// Tests if each component of a vector is greater than the corresponding component of another
        /// vector.
        /// </summary>
        /// <param name="vector1">The first vector.</param>
        /// <param name="vector2">The second vector.</param>
        /// <returns>
        /// <see langword="true"/> if each component of <paramref name="vector1"/> is greater than its
        /// counterpart in <paramref name="vector2"/>; otherwise, <see langword="false"/>.
        /// </returns>
        public static bool operator >(Vector3D vector1, Vector3D vector2)
        {
            return vector1.X > vector2.X
                && vector1.Y > vector2.Y
                && vector1.Z > vector2.Z;
        }


        /// <summary>
        /// Tests if each component of a vector is greater or equal than the corresponding component of
        /// another vector.
        /// </summary>
        /// <param name="vector1">The first vector.</param>
        /// <param name="vector2">The second vector.</param>
        /// <returns>
        /// <see langword="true"/> if each component of <paramref name="vector1"/> is greater or equal
        /// than its counterpart in <paramref name="vector2"/>; otherwise, <see langword="false"/>.
        /// </returns>
        public static bool operator >=(Vector3D vector1, Vector3D vector2)
        {
            return vector1.X >= vector2.X
                && vector1.Y >= vector2.Y
                && vector1.Z >= vector2.Z;
        }


        /// <summary>
        /// Tests if each component of a vector is less than the corresponding component of another
        /// vector.
        /// </summary>
        /// <param name="vector1">The first vector.</param>
        /// <param name="vector2">The second vector.</param>
        /// <returns>
        /// <see langword="true"/> if each component of <paramref name="vector1"/> is less than its 
        /// counterpart in <paramref name="vector2"/>; otherwise, <see langword="false"/>.
        /// </returns>
        public static bool operator <(Vector3D vector1, Vector3D vector2)
        {
            return vector1.X < vector2.X
                && vector1.Y < vector2.Y
                && vector1.Z < vector2.Z;
        }


        /// <summary>
        /// Tests if each component of a vector is less or equal than the corresponding component of
        /// another vector.
        /// </summary>
        /// <param name="vector1">The first vector.</param>
        /// <param name="vector2">The second vector.</param>
        /// <returns>
        /// <see langword="true"/> if each component of <paramref name="vector1"/> is less or equal than
        /// its counterpart in <paramref name="vector2"/>; otherwise, <see langword="false"/>.
        /// </returns>
        public static bool operator <=(Vector3D vector1, Vector3D vector2)
        {
            return vector1.X <= vector2.X
                && vector1.Y <= vector2.Y
                && vector1.Z <= vector2.Z;
        }


        /// <overloads>
        /// <summary>
        /// Converts a vector to another data type.
        /// </summary>
        /// </overloads>
        /// 
        /// <summary>
        /// Converts a vector to an array of 3 <see langword="double"/> values.
        /// </summary>
        /// <param name="vector">The vector.</param>
        /// <returns>
        /// The array with 3 <see langword="double"/> values. The order of the elements is: x, y, z
        /// </returns>
        public static explicit operator double[] (Vector3D vector)
        {
            return new[] { vector.X, vector.Y, vector.Z };
        }


        /// <summary>
        /// Converts this vector to an array of 3 <see langword="double"/> values.
        /// </summary>
        /// <returns>
        /// The array with 3 <see langword="double"/> values. The order of the elements is: x, y, z
        /// </returns>
        public double[] ToArray()
        {
            return (double[])this;
        }


        /// <summary>
        /// Converts a vector to a list of 3 <see langword="double"/> values.
        /// </summary>
        /// <param name="vector">The vector.</param>
        /// <returns>The result of the conversion. The order of the elements is: x, y, z</returns>
        public static explicit operator List<double>(Vector3D vector)
        {
            List<double> result = new List<double>(3) { vector.X, vector.Y, vector.Z };
            return result;
        }


        /// <summary>
        /// Converts this vector to a list of 3 <see langword="double"/> values.
        /// </summary>
        /// <returns>The result of the conversion. The order of the elements is: x, y, z</returns>
        public List<double> ToList()
        {
            return (List<double>)this;
        }


        /// <summary>
        /// Performs an explicit conversion from <see cref="Vector3D"/> to <see cref="Vector3F"/>.
        /// </summary>
        /// <param name="vector">The DigitalRune <see cref="Vector3D"/>.</param>
        /// <returns>The result of the conversion.</returns>
        public static explicit operator Vector3(Vector3D vector)
        {
            return new Vector3((float)vector.X, (float)vector.Y, (float)vector.Z);
        }


        /// <summary>
        /// Converts this <see cref="Vector3D"/> to <see cref="Vector3F"/>.
        /// </summary>
        /// <returns>The result of the conversion.</returns>
        public Vector3 ToVector3F()
        {
            return new Vector3((float)X, (float)Y, (float)Z);
        }
        public Vector4 ToVector4()
        {
            return new Vector4((float)X, (float)Y, (float)Z, 1);
        }


        /// <summary>
        /// Performs an conversion from <see cref="Vector3"/> (XNA Framework) to <see cref="Vector3D"/>
        /// (DigitalRune Mathematics).
        /// </summary>
        /// <param name="vector">The <see cref="Vector3"/> (XNA Framework).</param>
        /// <returns>The <see cref="Vector3D"/> (DigitalRune Mathematics).</returns>
        /// <remarks>
        /// This method is available only in the XNA-compatible build of the
        /// DigitalRune.Mathematics.dll.
        /// </remarks>
        public static explicit operator Vector3D(Vector3 vector)
    {
      return new Vector3D(vector.X, vector.Y, vector.Z);
    }


    /// <summary>
    /// Converts this <see cref="Vector3D"/> (DigitalRune Mathematics) to <see cref="Vector3"/> 
    /// (XNA Framework).
    /// </summary>
    /// <param name="vector">The <see cref="Vector3"/> (XNA Framework).</param>
    /// <returns>The <see cref="Vector3D"/> (DigitalRune Mathematics).</returns>
    /// <remarks>
    /// This method is available only in the XNA-compatible build of the 
    /// DigitalRune.Mathematics.dll.
    /// </remarks>
    public static Vector3D FromXna(Vector3 vector)
    {
      return new Vector3D(vector.X, vector.Y, vector.Z);
    }


    

    /// <summary>
    /// Converts this <see cref="Vector3D"/> (DigitalRune Mathematics) to <see cref="Vector3"/> 
    /// (XNA Framework).
    /// </summary>
    /// <returns>The <see cref="Vector3"/> (XNA Framework).</returns>
    /// <remarks>
    /// This method is available only in the XNA-compatible build of the 
    /// DigitalRune.Mathematics.dll.
    /// </remarks>
   public Vector3 ToXna()
    {
      return new Vector3((float)X, (float)Y, (float)Z);
    }

        #endregion


        //--------------------------------------------------------------
        #region Methods
        //--------------------------------------------------------------

        /// <overloads>
        /// <summary>
        /// Sets each vector component to its absolute value.
        /// </summary>
        /// </overloads>
        /// 
        /// <summary>
        /// Sets each vector component to its absolute value.
        /// </summary>
        public void Absolute()
        {
            X = Math.Abs(X);
            Y = Math.Abs(Y);
            Z = Math.Abs(Z);
        }


        /// <overloads>
        /// <summary>
        /// Clamps the vector components to the range [min, max].
        /// </summary>
        /// </overloads>
        /// 
        /// <summary>
        /// Clamps the vector components to the range [min, max].
        /// </summary>
        /// <param name="min">The min limit.</param>
        /// <param name="max">The max limit.</param>
        /// <remarks>
        /// This operation is carried out per component. Component values less than 
        /// <paramref name="min"/> are set to <paramref name="min"/>. Component values greater than 
        /// <paramref name="max"/> are set to <paramref name="max"/>.
        /// </remarks>
        public void Clamp(double min, double max)
        {
            X = DoubleClassExtension.Clamp(X, min, max);
            Y = DoubleClassExtension.Clamp(Y, min, max);
            Z = DoubleClassExtension.Clamp(Z, min, max);
        }


        /// <overloads>
        /// <summary>
        /// Clamps near-zero vector components to zero.
        /// </summary>
        /// </overloads>
        /// 
        /// <summary>
        /// Clamps near-zero vector components to zero.
        /// </summary>
        /// <remarks>
        /// Each vector component is compared to zero. If the component is in the interval 
        /// [-<see cref="Numeric.EpsilonD"/>, +<see cref="Numeric.EpsilonD"/>] it is set to zero, 
        /// otherwise it remains unchanged.
        /// </remarks>
        public void ClampToZero()
        {
            X = DoubleClassExtension.ClampToZero(X);
            Y = DoubleClassExtension.ClampToZero(Y);
            Z = DoubleClassExtension.ClampToZero(Z);
        }


        /// <summary>
        /// Clamps near-zero vector components to zero.
        /// </summary>
        /// <param name="epsilon">The tolerance value.</param>
        /// <remarks>
        /// Each vector component is compared to zero. If the component is in the interval 
        /// [-<paramref name="epsilon"/>, +<paramref name="epsilon"/>] it is set to zero, otherwise it 
        /// remains unchanged.
        /// </remarks>
        public void ClampToZero(double epsilon)
        {
            X = DoubleClassExtension.ClampToZero(X, epsilon);
            Y = DoubleClassExtension.ClampToZero(Y, epsilon);
            Z = DoubleClassExtension.ClampToZero(Z, epsilon);
        }


        /// <summary>
        /// Normalizes the vector.
        /// </summary>
        /// <remarks>
        /// A vectors is normalized by dividing its components by the length of the vector.
        /// </remarks>
        /// <exception cref="DivideByZeroException">
        /// The length of this vector is zero. The vector cannot be normalized.
        /// </exception>
        public void Normalize()
        {
            double length = Length;
            if (DoubleClassExtension.IsZero(length))
                throw new DivideByZeroException("Cannot normalize a vector with length 0.");

            double scale = 1.0 / length;
            X *= scale;
            Y *= scale;
            Z *= scale;
        }


        /// <summary>
        /// Tries to normalize the vector.
        /// </summary>
        /// <returns>
        /// <see langword="true"/> if the vector was normalized; otherwise, <see langword="false"/> if 
        /// the vector could not be normalized. (The length is numerically zero.)
        /// </returns>
        public bool TryNormalize()
        {
            double lengthSquared = LengthSquared;
            if (DoubleClassExtension.IsZero(lengthSquared, Double.Epsilon * Double.Epsilon))
                return false;

            double length = Math.Sqrt(lengthSquared);

            double scale = 1.0 / length;
            X *= scale;
            Y *= scale;
            Z *= scale;

            return true;
        }


        /// <overloads>
        /// <summary>
        /// Projects a vector onto another vector.
        /// </summary>
        /// </overloads>
        /// 
        /// <summary>
        /// Sets this vector to its projection onto the axis given by the target vector.
        /// </summary>
        /// <param name="target">The target vector.</param>
        public void ProjectTo(Vector3D target)
        {
            this = Dot(this, target) / target.LengthSquared * target;
        }


        /// <summary>
        /// Returns the cross product matrix (skew matrix) of this vector.
        /// </summary>
        /// <returns>The cross product matrix of this vector.</returns>
        /// <remarks>
        /// <c>Vector3D.Cross(v, w)</c> is the same as <c>v.ToCrossProductMatrix() * w</c>.
        /// </remarks>
        public Matrix33D ToCrossProductMatrix()
        {
            return new Matrix33D(0, -Z, Y,
                                 Z, 0, -X,
                                 -Y, X, 0);
        }
        #endregion


        //--------------------------------------------------------------
        #region Static Methods
        //--------------------------------------------------------------

        /// <summary>
        /// Returns a vector with the absolute values of the elements of the given vector.
        /// </summary>
        /// <param name="vector">The vector.</param>
        /// <returns>A vector with the absolute values of the elements of the given vector.</returns>
        public static Vector3D Absolute(Vector3D vector)
        {
            return new Vector3D(Math.Abs(vector.X), Math.Abs(vector.Y), Math.Abs(vector.Z));
        }


        /// <overloads>
        /// <summary>
        /// Determines whether two vectors are equal (regarding a given tolerance).
        /// </summary>
        /// </overloads>
        /// 
        /// <summary>
        /// Determines whether two vectors are equal (regarding the tolerance 
        /// <see cref="Numeric.EpsilonD"/>).
        /// </summary>
        /// <param name="vector1">The first vector.</param>
        /// <param name="vector2">The second vector.</param>
        /// <returns>
        /// <see langword="true"/> if the vectors are equal (within the tolerance 
        /// <see cref="Numeric.EpsilonD"/>); otherwise, <see langword="false"/>.
        /// </returns>
        /// <remarks>
        /// The two vectors are compared component-wise. If the differences of the components are less
        /// than <see cref="Numeric.EpsilonD"/> the vectors are considered as being equal.
        /// </remarks>
        public static bool AreNumericallyEqual(Vector3D vector1, Vector3D vector2)
        {
            return DoubleClassExtension.AreEqual(vector1.X, vector2.X)
                && DoubleClassExtension.AreEqual(vector1.Y, vector2.Y)
                && DoubleClassExtension.AreEqual(vector1.Z, vector2.Z);
        }


        /// <summary>
        /// Determines whether two vectors are equal (regarding a specific tolerance).
        /// </summary>
        /// <param name="vector1">The first vector.</param>
        /// <param name="vector2">The second vector.</param>
        /// <param name="epsilon">The tolerance value.</param>
        /// <returns>
        /// <see langword="true"/> if the vectors are equal (within the tolerance 
        /// <paramref name="epsilon"/>); otherwise, <see langword="false"/>.
        /// </returns>
        /// <remarks>
        /// The two vectors are compared component-wise. If the differences of the components are less
        /// than <paramref name="epsilon"/> the vectors are considered as being equal.
        /// </remarks>
        public static bool AreNumericallyEqual(Vector3D vector1, Vector3D vector2, double epsilon)
        {
            return DoubleClassExtension.AreEqual(vector1.X, vector2.X, epsilon)
                && DoubleClassExtension.AreEqual(vector1.Y, vector2.Y, epsilon)
                && DoubleClassExtension.AreEqual(vector1.Z, vector2.Z, epsilon);
        }


        /// <summary>
        /// Returns a vector with the vector components clamped to the range [min, max].
        /// </summary>
        /// <param name="vector">The vector.</param>
        /// <param name="min">The min limit.</param>
        /// <param name="max">The max limit.</param>
        /// <returns>A vector with clamped components.</returns>
        /// <remarks>
        /// This operation is carried out per component. Component values less than 
        /// <paramref name="min"/> are set to <paramref name="min"/>. Component values greater than 
        /// <paramref name="max"/> are set to <paramref name="max"/>.
        /// </remarks>
        public static Vector3D Clamp(Vector3D vector, double min, double max)
        {
            return new Vector3D(DoubleClassExtension.Clamp(vector.X, min, max),
                                DoubleClassExtension.Clamp(vector.Y, min, max),
                                DoubleClassExtension.Clamp(vector.Z, min, max));
        }


        /// <summary>
        /// Returns a vector with near-zero vector components clamped to 0.
        /// </summary>
        /// <param name="vector">The vector.</param>
        /// <returns>The vector with small components clamped to zero.</returns>
        /// <remarks>
        /// Each vector component (X, Y and Z) is compared to zero. If the component is in the interval 
        /// [-<see cref="Numeric.EpsilonD"/>, +<see cref="Numeric.EpsilonD"/>] it is set to zero, 
        /// otherwise it remains unchanged.
        /// </remarks>
        public static Vector3D ClampToZero(Vector3D vector)
        {
            vector.X = DoubleClassExtension.ClampToZero(vector.X);
            vector.Y = DoubleClassExtension.ClampToZero(vector.Y);
            vector.Z = DoubleClassExtension.ClampToZero(vector.Z);
            return vector;
        }


        /// <summary>
        /// Returns a vector with near-zero vector components clamped to 0.
        /// </summary>
        /// <param name="vector">The vector.</param>
        /// <param name="epsilon">The tolerance value.</param>
        /// <returns>The vector with small components clamped to zero.</returns>
        /// <remarks>
        /// Each vector component (X, Y and Z) is compared to zero. If the component is in the interval 
        /// [-<paramref name="epsilon"/>, +<paramref name="epsilon"/>] it is set to zero, otherwise it 
        /// remains unchanged.
        /// </remarks>
        public static Vector3D ClampToZero(Vector3D vector, double epsilon)
        {
            vector.X = DoubleClassExtension.ClampToZero(vector.X, epsilon);
            vector.Y = DoubleClassExtension.ClampToZero(vector.Y, epsilon);
            vector.Z = DoubleClassExtension.ClampToZero(vector.Z, epsilon);
            return vector;
        }


        /// <summary>
        /// Calculates the dot product of two vectors.
        /// </summary>
        /// <param name="vector1">The first vector.</param>
        /// <param name="vector2">The second vector.</param>
        /// <returns>The dot product.</returns>
        /// <remarks>
        /// The method calculates the dot product (also known as scalar product or inner product).
        /// </remarks>
        public static double Dot(Vector3D vector1, Vector3D vector2)
        {
            return vector1.X * vector2.X + vector1.Y * vector2.Y + vector1.Z * vector2.Z;
        }


        /// <summary>
        /// Calculates the cross product of two vectors.
        /// </summary>
        /// <param name="vector1">The first vector.</param>
        /// <param name="vector2">The second vector.</param>
        /// <returns>The cross product.</returns>
        /// <remarks>
        /// The method calculates the cross product (also known as vector product or outer product).
        /// </remarks>
        public static Vector3D Cross(Vector3D vector1, Vector3D vector2)
        {
            Vector3D result;
            result.X = vector1.Y * vector2.Z - vector1.Z * vector2.Y;
            result.Y = vector1.Z * vector2.X - vector1.X * vector2.Z;
            result.Z = vector1.X * vector2.Y - vector1.Y * vector2.X;
            return result;
        }


        /// <summary>
        /// Calculates the angle between two vectors.
        /// </summary>
        /// <param name="vector1">The first vector.</param>
        /// <param name="vector2">The second vector.</param>
        /// <returns>The angle between the given vectors, such that 0 ≤ angle ≤ π.</returns>
        /// <exception cref="ArgumentException">
        /// <paramref name="vector1"/> or <paramref name="vector2"/> has a length of 0.
        /// </exception>
        public static double GetAngle(Vector3D vector1, Vector3D vector2)
        {
            if (!vector1.TryNormalize() || !vector2.TryNormalize())
                throw new ArgumentException("vector1 and vector2 must not have 0 length.");

            double α = Dot(vector1, vector2);

            // Inaccuracy in the floating-point operations can cause
            // the result be outside of the valid range.
            // Ensure that the dot product α lies in the interval [-1, 1].
            // Math.Acos() returns Double.NaN if the argument lies outside
            // of this interval.
            α = DoubleClassExtension.Clamp(α, -1.0, 1.0);

            return Math.Acos(α);
        }


        /// <summary>
        /// Returns a vector that contains the lowest value from each matching pair of components.
        /// </summary>
        /// <param name="vector1">The first vector.</param>
        /// <param name="vector2">The second vector.</param>
        /// <returns>The minimized vector.</returns>
        public static Vector3D Min(Vector3D vector1, Vector3D vector2)
        {
            vector1.X = Math.Min(vector1.X, vector2.X);
            vector1.Y = Math.Min(vector1.Y, vector2.Y);
            vector1.Z = Math.Min(vector1.Z, vector2.Z);
            return vector1;
        }


        /// <summary>
        /// Returns a vector that contains the highest value from each matching pair of components.
        /// </summary>
        /// <param name="vector1">The first vector.</param>
        /// <param name="vector2">The second vector.</param>
        /// <returns>The maximized vector.</returns>
        public static Vector3D Max(Vector3D vector1, Vector3D vector2)
        {
            vector1.X = Math.Max(vector1.X, vector2.X);
            vector1.Y = Math.Max(vector1.Y, vector2.Y);
            vector1.Z = Math.Max(vector1.Z, vector2.Z);
            return vector1;
        }


        /// <summary>
        /// Projects a vector onto an axis given by the target vector.
        /// </summary>
        /// <param name="vector">The vector.</param>
        /// <param name="target">The target vector.</param>
        /// <returns>
        /// The projection of <paramref name="vector"/> onto <paramref name="target"/>.
        /// </returns>
        public static Vector3D ProjectTo(Vector3D vector, Vector3D target)
        {
            return Dot(vector, target) / target.LengthSquared * target;
        }


        /// <overloads>
        /// <summary>
        /// Converts the string representation of a 3-dimensional vector to its <see cref="Vector3D"/>
        /// equivalent.
        /// </summary>
        /// </overloads>
        /// 
        /// <summary>
        /// Converts the string representation of a 3-dimensional vector to its <see cref="Vector3D"/>
        /// equivalent.
        /// </summary>
        /// <param name="s">A string representation of a 3-dimensional vector.</param>
        /// <returns>
        /// A <see cref="Vector3D"/> that represents the vector specified by the <paramref name="s"/>
        /// parameter.
        /// </returns>
        /// <remarks>
        /// This version of <see cref="Parse(string)"/> uses the <see cref="CultureInfo"/> associated
        /// with the current thread.
        /// </remarks>
        /// <exception cref="FormatException">
        /// <paramref name="s"/> is not a valid <see cref="Vector3D"/>.
        /// </exception>
        public static Vector3D Parse(string s)
        {
            return Parse(s, CultureInfo.CurrentCulture);
        }


        /// <summary>
        /// Converts the string representation of a 3-dimensional vector in a specified culture-specific
        /// format to its <see cref="Vector3D"/> equivalent.
        /// </summary>
        /// <param name="s">A string representation of a 3-dimensional vector.</param>
        /// <param name="provider">
        /// An <see cref="IFormatProvider"/> that supplies culture-specific formatting information about
        /// <paramref name="s"/>. 
        /// </param>
        /// <returns>
        /// A <see cref="Vector3D"/> that represents the vector specified by the <paramref name="s"/>
        /// parameter.
        /// </returns>
        /// <exception cref="FormatException">
        /// <paramref name="s"/> is not a valid <see cref="Vector3D"/>.
        /// </exception>
        public static Vector3D Parse(string s, IFormatProvider provider)
        {
            Match m = Regex.Match(s, @"\((?<x>.*);(?<y>.*);(?<z>.*)\)", RegexOptions.None);
            if (m.Success)
            {
                return new Vector3D(double.Parse(m.Groups["x"].Value, provider),
                  double.Parse(m.Groups["y"].Value, provider),
                  double.Parse(m.Groups["z"].Value, provider));
            }

            throw new FormatException("String is not a valid Vector3D.");
        }

        /// <summary>
        /// Converts polar coordinates to Cartesian coordinates.
        /// </summary>
        /// <param name="radius">The radius.</param>
        /// <param name="latitude">The latitude.</param>
        /// <param name="longitude">The longitude.</param>
        /// <returns>The position in Cartesian coordinates.</returns>
        /// <remarks>
        /// The Cartesian coordinate system is right handed; y points east; z points up.
        /// In other words: Latitude and longitude are relative to +x; z increases with latitude. 
        /// </remarks>
        public static Vector3D ToCartesian(double radius, double latitude, double longitude)
        {
            double sinLat = Math.Sin(latitude);
            double cosLat = Math.Cos(latitude);
            double sinLong = Math.Sin(longitude);
            double cosLong = Math.Cos(longitude);

            Vector3D v;
            v.X = radius * cosLong * cosLat;
            v.Y = radius * sinLong * cosLat;  // East
            v.Z = radius * sinLat;            // Up
            return v;
        }
        #endregion
    }
}
