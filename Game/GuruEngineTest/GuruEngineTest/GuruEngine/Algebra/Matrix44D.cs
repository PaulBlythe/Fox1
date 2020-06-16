// DigitalRune Engine - Copyright (C) DigitalRune GmbH
// This file is subject to the terms and conditions defined in
// file 'LICENSE.TXT', which is part of this source code package.

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Xml.Serialization;
using Microsoft.Xna.Framework;



namespace GuruEngine.Algebra
{
    /// <summary>
    /// Defines a 4 x 4 matrix (double-precision).
    /// </summary>
    /// <remarks>
    /// <para>
    /// All indices are zero-based. The matrix looks like this:
    /// <code>
    /// M00 M01 M02 M03
    /// M10 M11 M12 M13
    /// M20 M21 M22 M23
    /// M30 M31 M32 M33
    /// </code>
    /// </para>
    /// </remarks>

    public struct Matrix44D : IEquatable<Matrix44D>
    {
        //--------------------------------------------------------------
        #region Constants
        //--------------------------------------------------------------

        /// <summary>
        /// Returns a <see cref="Matrix44D"/> with all of its components set to zero.
        /// </summary>
        public static readonly Matrix44D Zero = new Matrix44D(0, 0, 0, 0,
                                                              0, 0, 0, 0,
                                                              0, 0, 0, 0,
                                                              0, 0, 0, 0);

        /// <summary>
        /// Returns a <see cref="Matrix44D"/> with all of its components set to one.
        /// </summary>
        public static readonly Matrix44D One = new Matrix44D(1, 1, 1, 1,
                                                             1, 1, 1, 1,
                                                             1, 1, 1, 1,
                                                             1, 1, 1, 1);

        /// <summary>
        /// Returns the 4 x 4 identity matrix.
        /// </summary>
        public static readonly Matrix44D Identity = new Matrix44D(1, 0, 0, 0,
                                                                  0, 1, 0, 0,
                                                                  0, 0, 1, 0,
                                                                  0, 0, 0, 1);
        #endregion


        //--------------------------------------------------------------
        #region Fields
        //--------------------------------------------------------------

        /// <summary>
        /// The element in first row, first column.
        /// </summary>
        public double M00;

        /// <summary>
        /// The element in first row, second column.
        /// </summary>
        public double M01;

        /// <summary>
        /// The element in first row, third column.
        /// </summary>
        public double M02;

        /// <summary>
        /// The element in first row, fourth column.
        /// </summary>
        public double M03;

        /// <summary>
        /// The element in second row, first column.
        /// </summary>
        public double M10;

        /// <summary>
        /// The element in second row, second column.
        /// </summary>
        public double M11;

        /// <summary>
        /// The element in second row, third column.
        /// </summary>
        public double M12;

        /// <summary>
        /// The element in second row, fourth column.
        /// </summary>
        public double M13;

        /// <summary>
        /// The element in third row, first column.
        /// </summary>
        public double M20;

        /// <summary>
        /// The element in third row, second column.
        /// </summary>
        public double M21;

        /// <summary>
        /// The element in third row, third column.
        /// </summary>
        public double M22;

        /// <summary>
        /// The element in third row, fourth column.
        /// </summary>
        public double M23;

        /// <summary>
        /// The element in fourth row, first column.
        /// </summary>
        public double M30;

        /// <summary>
        /// The element in fourth row, second column.
        /// </summary>
        public double M31;

        /// <summary>
        /// The element in fourth row, third column.
        /// </summary>
        public double M32;

        /// <summary>
        /// The element in fourth row, fourth column.
        /// </summary>
        public double M33;
        #endregion


        //--------------------------------------------------------------
        #region Properties
        //--------------------------------------------------------------

        /// <overloads>
        /// <summary>
        /// Gets or sets the element at the specified index.
        /// </summary>
        /// </overloads>
        /// 
        /// <summary>
        /// Gets or sets the element at the specified index.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <value>The element at <paramref name="index"/>.</value>
        /// <remarks>
        /// The matrix elements are in row-major order.
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
                    case 0: return M00;
                    case 1: return M01;
                    case 2: return M02;
                    case 3: return M03;
                    case 4: return M10;
                    case 5: return M11;
                    case 6: return M12;
                    case 7: return M13;
                    case 8: return M20;
                    case 9: return M21;
                    case 10: return M22;
                    case 11: return M23;
                    case 12: return M30;
                    case 13: return M31;
                    case 14: return M32;
                    case 15: return M33;
                    default:
                        throw new ArgumentOutOfRangeException("index", "The index is out of range. Allowed values are 0 to 15.");
                }
            }
            set
            {
                switch (index)
                {
                    case 0: M00 = value; break;
                    case 1: M01 = value; break;
                    case 2: M02 = value; break;
                    case 3: M03 = value; break;
                    case 4: M10 = value; break;
                    case 5: M11 = value; break;
                    case 6: M12 = value; break;
                    case 7: M13 = value; break;
                    case 8: M20 = value; break;
                    case 9: M21 = value; break;
                    case 10: M22 = value; break;
                    case 11: M23 = value; break;
                    case 12: M30 = value; break;
                    case 13: M31 = value; break;
                    case 14: M32 = value; break;
                    case 15: M33 = value; break;
                    default:
                        throw new ArgumentOutOfRangeException("index", "The index is out of range. Allowed values are 0 to 15.");
                }
            }
        }


        /// <summary>
        /// Gets or sets the element at the specified index.
        /// </summary>
        /// <param name="row">The row index.</param>
        /// <param name="column">The column index.</param>
        /// <value>The element at the specified row and column.</value>
        /// <remarks>
        /// The indices are zero-based: [0,0] is the first element, [3,3] is the last element.
        /// </remarks>
        /// <exception cref="ArgumentOutOfRangeException">
        /// The index [<paramref name="row"/>, <paramref name="column"/>] is out of range.
        /// </exception>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1023:IndexersShouldNotBeMultidimensional")]
        public double this[int row, int column]
        {
            get
            {
                switch (row)
                {
                    case 0:
                        switch (column)
                        {
                            case 0: return M00;
                            case 1: return M01;
                            case 2: return M02;
                            case 3: return M03;
                            default:
                                throw new ArgumentOutOfRangeException("column", "The column index is out of range. Allowed values are 0 to 3.");
                        }
                    case 1:
                        switch (column)
                        {
                            case 0: return M10;
                            case 1: return M11;
                            case 2: return M12;
                            case 3: return M13;
                            default:
                                throw new ArgumentOutOfRangeException("column", "The column index is out of range. Allowed values are 0 to 3.");
                        }
                    case 2:
                        switch (column)
                        {
                            case 0: return M20;
                            case 1: return M21;
                            case 2: return M22;
                            case 3: return M23;
                            default:
                                throw new ArgumentOutOfRangeException("column", "The column index is out of range. Allowed values are 0 to 3.");
                        }
                    case 3:
                        switch (column)
                        {
                            case 0: return M30;
                            case 1: return M31;
                            case 2: return M32;
                            case 3: return M33;
                            default:
                                throw new ArgumentOutOfRangeException("column", "The column index is out of range. Allowed values are 0 to 3.");
                        }
                    default:
                        throw new ArgumentOutOfRangeException("row", "The row index is out of range. Allowed values are 0 to 3.");
                }
            }

            set
            {
                switch (row)
                {
                    case 0:
                        switch (column)
                        {
                            case 0: M00 = value; break;
                            case 1: M01 = value; break;
                            case 2: M02 = value; break;
                            case 3: M03 = value; break;
                            default:
                                throw new ArgumentOutOfRangeException("column", "The column index is out of range. Allowed values are 0 to 3.");
                        }
                        break;
                    case 1:
                        switch (column)
                        {
                            case 0: M10 = value; break;
                            case 1: M11 = value; break;
                            case 2: M12 = value; break;
                            case 3: M13 = value; break;
                            default:
                                throw new ArgumentOutOfRangeException("column", "The column index is out of range. Allowed values are 0 to 3.");
                        }
                        break;
                    case 2:
                        switch (column)
                        {
                            case 0: M20 = value; break;
                            case 1: M21 = value; break;
                            case 2: M22 = value; break;
                            case 3: M23 = value; break;
                            default:
                                throw new ArgumentOutOfRangeException("column", "The column index is out of range. Allowed values are 0 to 3.");
                        }
                        break;
                    case 3:
                        switch (column)
                        {
                            case 0: M30 = value; break;
                            case 1: M31 = value; break;
                            case 2: M32 = value; break;
                            case 3: M33 = value; break;
                            default:
                                throw new ArgumentOutOfRangeException("column", "The column index is out of range. Allowed values are 0 to 3.");
                        }
                        break;
                    default:
                        throw new ArgumentOutOfRangeException("row", "The row index is out of range. Allowed values are 0 to 3.");
                }
            }
        }


        /// <summary>
        /// Gets a value indicating whether an element of the matrix is <see cref="double.NaN"/>.
        /// </summary>
        /// <value>
        /// <see langword="true"/> if an element of the matrix is <see cref="double.NaN"/>; otherwise, 
        /// <see langword="false"/>.
        /// </value>
        public bool IsNaN
        {
            get
            {
                return Double.IsNaN(M00) || Double.IsNaN(M01) || Double.IsNaN(M02) || Double.IsNaN(M03)
                    || Double.IsNaN(M10) || Double.IsNaN(M11) || Double.IsNaN(M12) || Double.IsNaN(M13)
                    || Double.IsNaN(M20) || Double.IsNaN(M21) || Double.IsNaN(M22) || Double.IsNaN(M23)
                    || Double.IsNaN(M30) || Double.IsNaN(M31) || Double.IsNaN(M32) || Double.IsNaN(M33);
            }
        }


        /// <summary>
        /// Gets a value indicating whether this matrix is symmetric.
        /// </summary>
        /// <value>
        /// <see langword="true"/> if this matrix is symmetric; otherwise, <see langword="false"/>.
        /// </value>
        /// <remarks>
        /// The matrix elements are compared for equality - no tolerance value to handle numerical
        /// errors is used.
        /// </remarks>
        public bool IsSymmetric
        {
            get
            {
                return M01 == M10 && M02 == M20 && M03 == M30
                       && M12 == M21 && M13 == M31
                       && M23 == M32;
            }
        }


        /// <summary>
        /// Gets or sets the upper left 3x3 sub-matrix.
        /// </summary>
        /// <value>
        /// The 3x3 matrix that is produced by removing the last row and column of this matrix.
        /// </value>
        /// <remarks>
        /// Setting the minor matrix does not affect the elements in the fourth row or column.
        /// </remarks>
        public Matrix33D Minor
        {
            get
            {
                return new Matrix33D(M00, M01, M02, M10, M11, M12, M20, M21, M22);
            }
            set
            {
                M00 = value.M00; M01 = value.M01; M02 = value.M02;
                M10 = value.M10; M11 = value.M11; M12 = value.M12;
                M20 = value.M20; M21 = value.M21; M22 = value.M22;
            }
        }


        /// <summary>
        /// Gets the matrix trace (the sum of the diagonal elements).
        /// </summary>
        /// <value>The matrix trace.</value>
        public double Trace
        {
            get
            {
                return M00 + M11 + M22 + M33;
            }
        }


        /// <summary>
        /// Gets or sets the translation vector (<see cref="M03"/>, <see cref="M13"/>, <see cref="M23"/>)
        /// of the matrix.
        /// </summary>
        /// <value>The translation vector of the matrix.</value>
        public Vector3D Translation
        {
            get { return new Vector3D(M03, M13, M23); }
            set
            {
                M03 = value.X;
                M13 = value.Y;
                M23 = value.Z;
            }
        }


        /// <summary>
        /// Returns the determinant of this matrix.
        /// </summary>
        /// <value>The determinant of this matrix.</value>
        public double Determinant
        {
            get
            {
                double m22m33_m23m32 = M22 * M33 - M23 * M32;
                double m21m33_m23m31 = M21 * M33 - M23 * M31;
                double m21m32_m22m31 = M21 * M32 - M22 * M31;
                double m20m33_m23m30 = M20 * M33 - M23 * M30;
                double m20m32_m22m30 = M20 * M32 - M22 * M30;
                double m20m31_m21m30 = M20 * M31 - M21 * M30;

                // Develop determinant after first row:
                return M00 * (M11 * m22m33_m23m32 - M12 * m21m33_m23m31 + M13 * m21m32_m22m31)
                       - M01 * (M10 * m22m33_m23m32 - M12 * m20m33_m23m30 + M13 * m20m32_m22m30)
                       + M02 * (M10 * m21m33_m23m31 - M11 * m20m33_m23m30 + M13 * m20m31_m21m30)
                       - M03 * (M10 * m21m32_m22m31 - M11 * m20m32_m22m30 + M12 * m20m31_m21m30);
            }
        }


        /// <summary>
        /// Returns the transposed of this matrix.
        /// </summary>
        /// <returns>The transposed of this matrix.</returns>
        /// <remarks>
        /// The property does not change this instance. To transpose this instance you need to call 
        /// <see cref="Transpose"/>.
        /// </remarks>
        public Matrix44D Transposed
        {
            get
            {
                Matrix44D result = this;
                result.Transpose();
                return result;
            }
        }


        /// <summary>
        /// Returns the inverse of this matrix.
        /// </summary>
        /// <value>The inverse of this matrix.</value>
        /// <remarks>
        /// The property does not change this instance. To invert this instance you need to call 
        /// <see cref="Invert"/>.
        /// </remarks>
        /// <exception cref="MathematicsException">
        /// The matrix is singular (i.e. it is not invertible).
        /// </exception>
        /// <seealso cref="Invert"/>
        /// <seealso cref="TryInvert"/>
        public Matrix44D Inverse
        {
            get
            {
                Matrix44D result = this;
                result.Invert();
                return result;
            }
        }
        #endregion


        //--------------------------------------------------------------
        #region Creation & Cleanup
        //--------------------------------------------------------------

        /// <overloads>
        /// <summary>
        /// Initializes a new instance of the <see cref="Matrix44D"/> class.
        /// </summary>
        /// </overloads>
        /// 
        /// <summary>
        /// Initializes a new instance of the <see cref="Matrix44D"/> struct.
        /// </summary>
        /// <param name="elementValue">The initial value for the matrix elements.</param>
        /// <remarks>
        /// All matrix elements are set to <paramref name="elementValue"/>.
        /// </remarks>
        public Matrix44D(double elementValue)
          : this(elementValue, elementValue, elementValue, elementValue,
                 elementValue, elementValue, elementValue, elementValue,
                 elementValue, elementValue, elementValue, elementValue,
                 elementValue, elementValue, elementValue, elementValue)
        {
        }


        /// <summary>
        /// Initializes a new instance of the <see cref="Matrix44D"/> class.
        /// </summary>
        /// <param name="m00">The element in the first row, first column.</param>
        /// <param name="m01">The element in the first row, second column.</param>
        /// <param name="m02">The element in the first row, third column.</param>
        /// <param name="m03">The element in the first row, fourth column.</param>
        /// <param name="m10">The element in the second row, first column.</param>
        /// <param name="m11">The element in the second row, second column.</param>
        /// <param name="m12">The element in the second row, third column.</param>
        /// <param name="m13">The element in the second row, fourth column.</param>
        /// <param name="m20">The element in the third row, first column.</param>
        /// <param name="m21">The element in the third row, second column.</param>
        /// <param name="m22">The element in the third row, third column.</param>
        /// <param name="m23">The element in the third row, fourth column.</param>
        /// <param name="m30">The element in the fourth row, first column</param>
        /// <param name="m31">The element in the fourth row, second column</param>
        /// <param name="m32">The element in the fourth row, third column</param>
        /// <param name="m33">The element in the fourth row, fourth column</param>
        public Matrix44D(double m00, double m01, double m02, double m03,
                         double m10, double m11, double m12, double m13,
                         double m20, double m21, double m22, double m23,
                         double m30, double m31, double m32, double m33)
        {
            M00 = m00; M01 = m01; M02 = m02; M03 = m03;
            M10 = m10; M11 = m11; M12 = m12; M13 = m13;
            M20 = m20; M21 = m21; M22 = m22; M23 = m23;
            M30 = m30; M31 = m31; M32 = m32; M33 = m33;
        }


        /// <summary>
        /// Initializes a new instance of the <see cref="Matrix44D"/> struct.
        /// </summary>
        /// <param name="elements">The array with the initial values for the matrix elements.</param>
        /// <param name="order">The order of the matrix elements in <paramref name="elements"/>.</param>
        /// <exception cref="IndexOutOfRangeException">
        /// <paramref name="elements"/> has less than 16 elements.
        /// </exception>
        /// <exception cref="NullReferenceException">
        /// <paramref name="elements"/> must not be <see langword="null"/>.
        /// </exception>
        public Matrix44D(double[] elements, MatrixOrder order)
        {
            if (order == MatrixOrder.RowMajor)
            {
                // First row
                M00 = elements[0]; M01 = elements[1]; M02 = elements[2]; M03 = elements[3];
                // Second row
                M10 = elements[4]; M11 = elements[5]; M12 = elements[6]; M13 = elements[7];
                // Third row
                M20 = elements[8]; M21 = elements[9]; M22 = elements[10]; M23 = elements[11];
                // fourth row
                M30 = elements[12]; M31 = elements[13]; M32 = elements[14]; M33 = elements[15];
            }
            else
            {
                // First column
                M00 = elements[0]; M10 = elements[1]; M20 = elements[2]; M30 = elements[3];
                // Second column
                M01 = elements[4]; M11 = elements[5]; M21 = elements[6]; M31 = elements[7];
                // Third column
                M02 = elements[8]; M12 = elements[9]; M22 = elements[10]; M32 = elements[11];
                // Fourth column
                M03 = elements[12]; M13 = elements[13]; M23 = elements[14]; M33 = elements[15];
            }
        }


        /// <summary>
        /// Initializes a new instance of the <see cref="Matrix44D"/> struct.
        /// </summary>
        /// <param name="elements">The list with the initial values for the matrix elements.</param>
        /// <param name="order">The order of the matrix elements in <paramref name="elements"/>.</param>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="elements"/> has less than 16 elements.
        /// </exception>
        /// <exception cref="NullReferenceException">
        /// <paramref name="elements"/> must not be <see langword="null"/>.
        /// </exception>
        public Matrix44D(IList<double> elements, MatrixOrder order)
        {
            if (order == MatrixOrder.RowMajor)
            {
                // First row
                M00 = elements[0]; M01 = elements[1]; M02 = elements[2]; M03 = elements[3];
                // Second row
                M10 = elements[4]; M11 = elements[5]; M12 = elements[6]; M13 = elements[7];
                // Third row
                M20 = elements[8]; M21 = elements[9]; M22 = elements[10]; M23 = elements[11];
                // fourth row
                M30 = elements[12]; M31 = elements[13]; M32 = elements[14]; M33 = elements[15];
            }
            else
            {
                // First column
                M00 = elements[0]; M10 = elements[1]; M20 = elements[2]; M30 = elements[3];
                // Second column
                M01 = elements[4]; M11 = elements[5]; M21 = elements[6]; M31 = elements[7];
                // Third column
                M02 = elements[8]; M12 = elements[9]; M22 = elements[10]; M32 = elements[11];
                // Fourth column
                M03 = elements[12]; M13 = elements[13]; M23 = elements[14]; M33 = elements[15];
            }
        }


        /// <summary>
        /// Initializes a new instance of the <see cref="Matrix44D"/> struct.
        /// </summary>
        /// <param name="elements">The array with the initial values for the matrix elements.</param>
        /// <exception cref="IndexOutOfRangeException">
        /// <paramref name="elements"/> has less than 4x4 elements.
        /// </exception>
        /// <exception cref="NullReferenceException">
        /// <paramref name="elements"/> must not be <see langword="null"/>.
        /// </exception>
        public Matrix44D(double[,] elements)
        {
            M00 = elements[0, 0]; M01 = elements[0, 1]; M02 = elements[0, 2]; M03 = elements[0, 3];
            M10 = elements[1, 0]; M11 = elements[1, 1]; M12 = elements[1, 2]; M13 = elements[1, 3];
            M20 = elements[2, 0]; M21 = elements[2, 1]; M22 = elements[2, 2]; M23 = elements[2, 3];
            M30 = elements[3, 0]; M31 = elements[3, 1]; M32 = elements[3, 2]; M33 = elements[3, 3];
        }


        /// <summary>
        /// Initializes a new instance of the <see cref="Matrix22D"/> struct.
        /// </summary>
        /// <param name="elements">The array with the initial values for the matrix elements.</param>
        /// <exception cref="IndexOutOfRangeException">
        /// <paramref name="elements"/> has less than 4x4 elements.
        /// </exception>
        /// <exception cref="NullReferenceException">
        /// <paramref name="elements"/> or the arrays in elements[0] must not be <see langword="null"/>.
        /// </exception>
        public Matrix44D(double[][] elements)
        {
            M00 = elements[0][0]; M01 = elements[0][1]; M02 = elements[0][2]; M03 = elements[0][3];
            M10 = elements[1][0]; M11 = elements[1][1]; M12 = elements[1][2]; M13 = elements[1][3];
            M20 = elements[2][0]; M21 = elements[2][1]; M22 = elements[2][2]; M23 = elements[2][3];
            M30 = elements[3][0]; M31 = elements[3][1]; M32 = elements[3][2]; M33 = elements[3][3];
        }


        /// <summary>
        /// Initializes a new instance of the <see cref="Matrix44D"/> struct.
        /// </summary>
        /// <param name="minor">The upper left 3x3 submatrix.</param>
        /// <param name="translation">The translation vector.</param>
        /// <remarks>
        /// The upper left 3x3 submatrix is initialized with <paramref name="minor"/>. The elements 
        /// <see cref="M03"/>, <see cref="M13"/>, <see cref="M23"/> are initialized with the elements of 
        /// <paramref name="translation"/>. <see cref="M33"/> is set to 1 and all other matrix elements
        /// are set to 0.
        /// </remarks>
        public Matrix44D(Matrix33D minor, Vector3D translation)
        {
            M00 = minor.M00; M01 = minor.M01; M02 = minor.M02; M03 = translation.X;
            M10 = minor.M10; M11 = minor.M11; M12 = minor.M12; M13 = translation.Y;
            M20 = minor.M20; M21 = minor.M21; M22 = minor.M22; M23 = translation.Z;
            M30 = 0; M31 = 0; M32 = 0; M33 = 1;
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
            unchecked
            {
                int hashCode = M00.GetHashCode();
                hashCode = (hashCode * 397) ^ M01.GetHashCode();
                hashCode = (hashCode * 397) ^ M02.GetHashCode();
                hashCode = (hashCode * 397) ^ M03.GetHashCode();
                hashCode = (hashCode * 397) ^ M10.GetHashCode();
                hashCode = (hashCode * 397) ^ M11.GetHashCode();
                hashCode = (hashCode * 397) ^ M12.GetHashCode();
                hashCode = (hashCode * 397) ^ M13.GetHashCode();
                hashCode = (hashCode * 397) ^ M20.GetHashCode();
                hashCode = (hashCode * 397) ^ M21.GetHashCode();
                hashCode = (hashCode * 397) ^ M22.GetHashCode();
                hashCode = (hashCode * 397) ^ M23.GetHashCode();
                hashCode = (hashCode * 397) ^ M30.GetHashCode();
                hashCode = (hashCode * 397) ^ M31.GetHashCode();
                hashCode = (hashCode * 397) ^ M32.GetHashCode();
                hashCode = (hashCode * 397) ^ M33.GetHashCode();
                return hashCode;
            }
        }


        /// <overloads>
        /// <summary>
        /// Indicates whether the current object is equal to another object.
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
            return obj is Matrix44D && this == (Matrix44D)obj;
        }


        #region IEquatable<Matrix44D> Members
        /// <summary>
        /// Indicates whether the current object is equal to another object of the same type.
        /// </summary>
        /// <param name="other">An object to compare with this object.</param>
        /// <returns>
        /// <see langword="true"/> if the current object is equal to the other parameter; otherwise, 
        /// <see langword="false"/>.
        /// </returns>
        public bool Equals(Matrix44D other)
        {
            return this == other;
        }
        #endregion


        /// <overloads>
        /// <summary>
        /// Returns the string representation of this matrix.
        /// </summary>
        /// </overloads>
        /// 
        /// <summary>
        /// Returns the string representation of this matrix.
        /// </summary>
        /// <returns>The string representation of this matrix.</returns>
        public override string ToString()
        {
            return ToString(CultureInfo.CurrentCulture);
        }


        /// <summary>
        /// Returns the string representation of this matrix using the specified culture-specific format
        /// information.
        /// </summary>
        /// <param name="provider">
        /// An <see cref="IFormatProvider"/> that supplies culture-specific formatting information.
        /// </param>
        /// <returns>The string representation of this matrix.</returns>
        public string ToString(IFormatProvider provider)
        {
            return string.Format(provider,
              "({0}; {1}; {2}; {3})\n" +
              "({4}; {5}; {6}; {7})\n" +
              "({8}; {9}; {10}; {11})\n" +
              "({12}; {13}; {14}; {15})\n",
              M00, M01, M02, M03,
              M10, M11, M12, M13,
              M20, M21, M22, M23,
              M30, M31, M32, M33);
        }
        #endregion


        //--------------------------------------------------------------
        #region Overloaded Operators
        //--------------------------------------------------------------

        /// <summary>
        /// Negates a matrix.
        /// </summary>
        /// <param name="matrix">The matrix.</param>
        /// <returns>The negated matrix.</returns>
        /// <remarks>
        /// Each element of the matrix is negated.
        /// </remarks>
        public static Matrix44D operator -(Matrix44D matrix)
        {
            matrix.M00 = -matrix.M00; matrix.M01 = -matrix.M01; matrix.M02 = -matrix.M02; matrix.M03 = -matrix.M03;
            matrix.M10 = -matrix.M10; matrix.M11 = -matrix.M11; matrix.M12 = -matrix.M12; matrix.M13 = -matrix.M13;
            matrix.M20 = -matrix.M20; matrix.M21 = -matrix.M21; matrix.M22 = -matrix.M22; matrix.M23 = -matrix.M23;
            matrix.M30 = -matrix.M30; matrix.M31 = -matrix.M31; matrix.M32 = -matrix.M32; matrix.M33 = -matrix.M33;
            return matrix;
        }


        /// <summary>
        /// Negates a matrix.
        /// </summary>
        /// <param name="matrix">The matrix.</param>
        /// <returns>The negated matrix.</returns>
        /// <remarks>
        /// Each element of the matrix is negated.
        /// </remarks>
        public static Matrix44D Negate(Matrix44D matrix)
        {
            matrix.M00 = -matrix.M00; matrix.M01 = -matrix.M01; matrix.M02 = -matrix.M02; matrix.M03 = -matrix.M03;
            matrix.M10 = -matrix.M10; matrix.M11 = -matrix.M11; matrix.M12 = -matrix.M12; matrix.M13 = -matrix.M13;
            matrix.M20 = -matrix.M20; matrix.M21 = -matrix.M21; matrix.M22 = -matrix.M22; matrix.M23 = -matrix.M23;
            matrix.M30 = -matrix.M30; matrix.M31 = -matrix.M31; matrix.M32 = -matrix.M32; matrix.M33 = -matrix.M33;
            return matrix;
        }


        /// <summary>
        /// Adds two matrices.
        /// </summary>
        /// <param name="matrix1">The first matrix.</param>
        /// <param name="matrix2">The second Matrix.</param>
        /// <returns>The sum of the two matrices.</returns>
        public static Matrix44D operator +(Matrix44D matrix1, Matrix44D matrix2)
        {
            matrix1.M00 += matrix2.M00; matrix1.M01 += matrix2.M01; matrix1.M02 += matrix2.M02; matrix1.M03 += matrix2.M03;
            matrix1.M10 += matrix2.M10; matrix1.M11 += matrix2.M11; matrix1.M12 += matrix2.M12; matrix1.M13 += matrix2.M13;
            matrix1.M20 += matrix2.M20; matrix1.M21 += matrix2.M21; matrix1.M22 += matrix2.M22; matrix1.M23 += matrix2.M23;
            matrix1.M30 += matrix2.M30; matrix1.M31 += matrix2.M31; matrix1.M32 += matrix2.M32; matrix1.M33 += matrix2.M33;
            return matrix1;
        }


        /// <summary>
        /// Adds two matrices.
        /// </summary>
        /// <param name="matrix1">The first matrix.</param>
        /// <param name="matrix2">The second Matrix.</param>
        /// <returns>The sum of the two matrices.</returns>
        public static Matrix44D Add(Matrix44D matrix1, Matrix44D matrix2)
        {
            matrix1.M00 += matrix2.M00; matrix1.M01 += matrix2.M01; matrix1.M02 += matrix2.M02; matrix1.M03 += matrix2.M03;
            matrix1.M10 += matrix2.M10; matrix1.M11 += matrix2.M11; matrix1.M12 += matrix2.M12; matrix1.M13 += matrix2.M13;
            matrix1.M20 += matrix2.M20; matrix1.M21 += matrix2.M21; matrix1.M22 += matrix2.M22; matrix1.M23 += matrix2.M23;
            matrix1.M30 += matrix2.M30; matrix1.M31 += matrix2.M31; matrix1.M32 += matrix2.M32; matrix1.M33 += matrix2.M33;
            return matrix1;
        }


        /// <summary>
        /// Subtracts two matrices.
        /// </summary>
        /// <param name="minuend">The first matrix (minuend).</param>
        /// <param name="subtrahend">The second matrix (subtrahend).</param>
        /// <returns>The difference of the two matrices.</returns>
        public static Matrix44D operator -(Matrix44D minuend, Matrix44D subtrahend)
        {
            minuend.M00 -= subtrahend.M00;
            minuend.M01 -= subtrahend.M01;
            minuend.M02 -= subtrahend.M02;
            minuend.M03 -= subtrahend.M03;
            minuend.M10 -= subtrahend.M10;
            minuend.M11 -= subtrahend.M11;
            minuend.M12 -= subtrahend.M12;
            minuend.M13 -= subtrahend.M13;
            minuend.M20 -= subtrahend.M20;
            minuend.M21 -= subtrahend.M21;
            minuend.M22 -= subtrahend.M22;
            minuend.M23 -= subtrahend.M23;
            minuend.M30 -= subtrahend.M30;
            minuend.M31 -= subtrahend.M31;
            minuend.M32 -= subtrahend.M32;
            minuend.M33 -= subtrahend.M33;
            return minuend;
        }


        /// <summary>
        /// Subtracts two matrices.
        /// </summary>
        /// <param name="minuend">The first matrix (minuend).</param>
        /// <param name="subtrahend">The second matrix (subtrahend).</param>
        /// <returns>The difference of the two matrices.</returns>
        public static Matrix44D Subtract(Matrix44D minuend, Matrix44D subtrahend)
        {
            minuend.M00 -= subtrahend.M00;
            minuend.M01 -= subtrahend.M01;
            minuend.M02 -= subtrahend.M02;
            minuend.M03 -= subtrahend.M03;
            minuend.M10 -= subtrahend.M10;
            minuend.M11 -= subtrahend.M11;
            minuend.M12 -= subtrahend.M12;
            minuend.M13 -= subtrahend.M13;
            minuend.M20 -= subtrahend.M20;
            minuend.M21 -= subtrahend.M21;
            minuend.M22 -= subtrahend.M22;
            minuend.M23 -= subtrahend.M23;
            minuend.M30 -= subtrahend.M30;
            minuend.M31 -= subtrahend.M31;
            minuend.M32 -= subtrahend.M32;
            minuend.M33 -= subtrahend.M33;
            return minuend;
        }


        /// <overloads>
        /// <summary>
        /// Multiplies a matrix by a scalar, matrix or vector.
        /// </summary>
        /// </overloads>
        /// 
        /// <summary>
        /// Multiplies a matrix and a scalar.
        /// </summary>
        /// <param name="matrix">The matrix.</param>
        /// <param name="scalar">The scalar.</param>
        /// <returns>The matrix with each element multiplied by <paramref name="scalar"/>.</returns>
        public static Matrix44D operator *(Matrix44D matrix, double scalar)
        {
            matrix.M00 *= scalar; matrix.M01 *= scalar; matrix.M02 *= scalar; matrix.M03 *= scalar;
            matrix.M10 *= scalar; matrix.M11 *= scalar; matrix.M12 *= scalar; matrix.M13 *= scalar;
            matrix.M20 *= scalar; matrix.M21 *= scalar; matrix.M22 *= scalar; matrix.M23 *= scalar;
            matrix.M30 *= scalar; matrix.M31 *= scalar; matrix.M32 *= scalar; matrix.M33 *= scalar;
            return matrix;
        }


        /// <summary>
        /// Multiplies a matrix by a scalar.
        /// </summary>
        /// <param name="scalar">The scalar.</param>
        /// <param name="matrix">The matrix.</param>
        /// <returns>The matrix with each element multiplied by <paramref name="scalar"/>.</returns>
        public static Matrix44D operator *(double scalar, Matrix44D matrix)
        {
            matrix.M00 *= scalar; matrix.M01 *= scalar; matrix.M02 *= scalar; matrix.M03 *= scalar;
            matrix.M10 *= scalar; matrix.M11 *= scalar; matrix.M12 *= scalar; matrix.M13 *= scalar;
            matrix.M20 *= scalar; matrix.M21 *= scalar; matrix.M22 *= scalar; matrix.M23 *= scalar;
            matrix.M30 *= scalar; matrix.M31 *= scalar; matrix.M32 *= scalar; matrix.M33 *= scalar;
            return matrix;
        }


        /// <overloads>
        /// <summary>
        /// Multiplies a matrix by a scalar, matrix or vector.
        /// </summary>
        /// </overloads>
        /// 
        /// <summary>
        /// Multiplies a matrix by a scalar.
        /// </summary>
        /// <param name="matrix">The matrix.</param>
        /// <param name="scalar">The scalar.</param>
        /// <returns>The matrix with each element multiplied by <paramref name="scalar"/>.</returns>
        public static Matrix44D Multiply(double scalar, Matrix44D matrix)
        {
            matrix.M00 *= scalar; matrix.M01 *= scalar; matrix.M02 *= scalar; matrix.M03 *= scalar;
            matrix.M10 *= scalar; matrix.M11 *= scalar; matrix.M12 *= scalar; matrix.M13 *= scalar;
            matrix.M20 *= scalar; matrix.M21 *= scalar; matrix.M22 *= scalar; matrix.M23 *= scalar;
            matrix.M30 *= scalar; matrix.M31 *= scalar; matrix.M32 *= scalar; matrix.M33 *= scalar;
            return matrix;
        }


        /// <summary>
        /// Multiplies two matrices.
        /// </summary>
        /// <param name="m1">The first matrix.</param>
        /// <param name="m2">The second matrix.</param>
        /// <returns>The matrix with the product the two matrices.</returns>
        public static Matrix44D operator *(Matrix44D m1, Matrix44D m2)
        {
            Matrix44D product;

            product.M00 = m1.M00 * m2.M00 + m1.M01 * m2.M10 + m1.M02 * m2.M20 + m1.M03 * m2.M30;
            product.M01 = m1.M00 * m2.M01 + m1.M01 * m2.M11 + m1.M02 * m2.M21 + m1.M03 * m2.M31;
            product.M02 = m1.M00 * m2.M02 + m1.M01 * m2.M12 + m1.M02 * m2.M22 + m1.M03 * m2.M32;
            product.M03 = m1.M00 * m2.M03 + m1.M01 * m2.M13 + m1.M02 * m2.M23 + m1.M03 * m2.M33;

            product.M10 = m1.M10 * m2.M00 + m1.M11 * m2.M10 + m1.M12 * m2.M20 + m1.M13 * m2.M30;
            product.M11 = m1.M10 * m2.M01 + m1.M11 * m2.M11 + m1.M12 * m2.M21 + m1.M13 * m2.M31;
            product.M12 = m1.M10 * m2.M02 + m1.M11 * m2.M12 + m1.M12 * m2.M22 + m1.M13 * m2.M32;
            product.M13 = m1.M10 * m2.M03 + m1.M11 * m2.M13 + m1.M12 * m2.M23 + m1.M13 * m2.M33;

            product.M20 = m1.M20 * m2.M00 + m1.M21 * m2.M10 + m1.M22 * m2.M20 + m1.M23 * m2.M30;
            product.M21 = m1.M20 * m2.M01 + m1.M21 * m2.M11 + m1.M22 * m2.M21 + m1.M23 * m2.M31;
            product.M22 = m1.M20 * m2.M02 + m1.M21 * m2.M12 + m1.M22 * m2.M22 + m1.M23 * m2.M32;
            product.M23 = m1.M20 * m2.M03 + m1.M21 * m2.M13 + m1.M22 * m2.M23 + m1.M23 * m2.M33;

            product.M30 = m1.M30 * m2.M00 + m1.M31 * m2.M10 + m1.M32 * m2.M20 + m1.M33 * m2.M30;
            product.M31 = m1.M30 * m2.M01 + m1.M31 * m2.M11 + m1.M32 * m2.M21 + m1.M33 * m2.M31;
            product.M32 = m1.M30 * m2.M02 + m1.M31 * m2.M12 + m1.M32 * m2.M22 + m1.M33 * m2.M32;
            product.M33 = m1.M30 * m2.M03 + m1.M31 * m2.M13 + m1.M32 * m2.M23 + m1.M33 * m2.M33;

            return product;
        }


        /// <summary>
        /// Multiplies two matrices.
        /// </summary>
        /// <param name="m1">The first matrix.</param>
        /// <param name="m2">The second matrix.</param>
        /// <returns>The matrix with the product the two matrices.</returns>
        public static Matrix44D Multiply(Matrix44D m1, Matrix44D m2)
        {
            Matrix44D product;

            product.M00 = m1.M00 * m2.M00 + m1.M01 * m2.M10 + m1.M02 * m2.M20 + m1.M03 * m2.M30;
            product.M01 = m1.M00 * m2.M01 + m1.M01 * m2.M11 + m1.M02 * m2.M21 + m1.M03 * m2.M31;
            product.M02 = m1.M00 * m2.M02 + m1.M01 * m2.M12 + m1.M02 * m2.M22 + m1.M03 * m2.M32;
            product.M03 = m1.M00 * m2.M03 + m1.M01 * m2.M13 + m1.M02 * m2.M23 + m1.M03 * m2.M33;

            product.M10 = m1.M10 * m2.M00 + m1.M11 * m2.M10 + m1.M12 * m2.M20 + m1.M13 * m2.M30;
            product.M11 = m1.M10 * m2.M01 + m1.M11 * m2.M11 + m1.M12 * m2.M21 + m1.M13 * m2.M31;
            product.M12 = m1.M10 * m2.M02 + m1.M11 * m2.M12 + m1.M12 * m2.M22 + m1.M13 * m2.M32;
            product.M13 = m1.M10 * m2.M03 + m1.M11 * m2.M13 + m1.M12 * m2.M23 + m1.M13 * m2.M33;

            product.M20 = m1.M20 * m2.M00 + m1.M21 * m2.M10 + m1.M22 * m2.M20 + m1.M23 * m2.M30;
            product.M21 = m1.M20 * m2.M01 + m1.M21 * m2.M11 + m1.M22 * m2.M21 + m1.M23 * m2.M31;
            product.M22 = m1.M20 * m2.M02 + m1.M21 * m2.M12 + m1.M22 * m2.M22 + m1.M23 * m2.M32;
            product.M23 = m1.M20 * m2.M03 + m1.M21 * m2.M13 + m1.M22 * m2.M23 + m1.M23 * m2.M33;

            product.M30 = m1.M30 * m2.M00 + m1.M31 * m2.M10 + m1.M32 * m2.M20 + m1.M33 * m2.M30;
            product.M31 = m1.M30 * m2.M01 + m1.M31 * m2.M11 + m1.M32 * m2.M21 + m1.M33 * m2.M31;
            product.M32 = m1.M30 * m2.M02 + m1.M31 * m2.M12 + m1.M32 * m2.M22 + m1.M33 * m2.M32;
            product.M33 = m1.M30 * m2.M03 + m1.M31 * m2.M13 + m1.M32 * m2.M23 + m1.M33 * m2.M33;

            return product;
        }
        /// <summary>
        /// Divides a matrix by a scalar.
        /// </summary>
        /// <param name="matrix">The matrix.</param>
        /// <param name="scalar">The scalar.</param>
        /// <returns>The matrix with each element divided by scalar.</returns>
        public static Matrix44D operator /(Matrix44D matrix, double scalar)
        {
            double f = 1 / scalar;
            matrix.M00 *= f; matrix.M01 *= f; matrix.M02 *= f; matrix.M03 *= f;
            matrix.M10 *= f; matrix.M11 *= f; matrix.M12 *= f; matrix.M13 *= f;
            matrix.M20 *= f; matrix.M21 *= f; matrix.M22 *= f; matrix.M23 *= f;
            matrix.M30 *= f; matrix.M31 *= f; matrix.M32 *= f; matrix.M33 *= f;
            return matrix;
        }


        /// <summary>
        /// Divides a matrix by a scalar.
        /// </summary>
        /// <param name="matrix">The matrix.</param>
        /// <param name="scalar">The scalar.</param>
        /// <returns>The matrix with each element divided by scalar.</returns>
        public static Matrix44D Divide(Matrix44D matrix, double scalar)
        {
            double f = 1 / scalar;
            matrix.M00 *= f; matrix.M01 *= f; matrix.M02 *= f; matrix.M03 *= f;
            matrix.M10 *= f; matrix.M11 *= f; matrix.M12 *= f; matrix.M13 *= f;
            matrix.M20 *= f; matrix.M21 *= f; matrix.M22 *= f; matrix.M23 *= f;
            matrix.M30 *= f; matrix.M31 *= f; matrix.M32 *= f; matrix.M33 *= f;
            return matrix;
        }


        /// <summary>
        /// Tests if two matrices are equal.
        /// </summary>
        /// <param name="matrix1">The first matrix.</param>
        /// <param name="matrix2">The second matrix.</param>
        /// <returns>
        /// <see langword="true"/> if the matrices are equal; otherwise <see langword="false"/>.
        /// </returns>
        /// <remarks>
        /// For the test the corresponding elements of the matrices are compared.
        /// </remarks>
        public static bool operator ==(Matrix44D matrix1, Matrix44D matrix2)
        {
            return (matrix1.M00 == matrix2.M00) && (matrix1.M01 == matrix2.M01) && (matrix1.M02 == matrix2.M02) && (matrix1.M03 == matrix2.M03)
                && (matrix1.M10 == matrix2.M10) && (matrix1.M11 == matrix2.M11) && (matrix1.M12 == matrix2.M12) && (matrix1.M13 == matrix2.M13)
                && (matrix1.M20 == matrix2.M20) && (matrix1.M21 == matrix2.M21) && (matrix1.M22 == matrix2.M22) && (matrix1.M23 == matrix2.M23)
                && (matrix1.M30 == matrix2.M30) && (matrix1.M31 == matrix2.M31) && (matrix1.M32 == matrix2.M32) && (matrix1.M33 == matrix2.M33);
        }


        /// <summary>
        /// Tests if two matrices are not equal.
        /// </summary>
        /// <param name="matrix1">The first matrix.</param>
        /// <param name="matrix2">The second matrix.</param>
        /// <returns>
        /// <see langword="true"/> if the matrices are different; otherwise <see langword="false"/>.
        /// </returns>
        /// <remarks>
        /// For the test the corresponding elements of the matrices are compared.
        /// </remarks>
        public static bool operator !=(Matrix44D matrix1, Matrix44D matrix2)
        {
            return (matrix1.M00 != matrix2.M00) || (matrix1.M01 != matrix2.M01) || (matrix1.M02 != matrix2.M02) || (matrix1.M03 != matrix2.M03)
                || (matrix1.M10 != matrix2.M10) || (matrix1.M11 != matrix2.M11) || (matrix1.M12 != matrix2.M12) || (matrix1.M13 != matrix2.M13)
                || (matrix1.M20 != matrix2.M20) || (matrix1.M21 != matrix2.M21) || (matrix1.M22 != matrix2.M22) || (matrix1.M23 != matrix2.M23)
                || (matrix1.M30 != matrix2.M30) || (matrix1.M31 != matrix2.M31) || (matrix1.M32 != matrix2.M32) || (matrix1.M33 != matrix2.M33);
        }


        /// <overloads>
        /// <summary>
        /// Performs an explicit conversion from <see cref="Matrix22D"/> to another type.
        /// </summary>
        /// </overloads>
        /// 
        /// <summary>
        /// Performs an explicit conversion from <see cref="Matrix44D"/> to a 2-dimensional 
        /// <see langword="double"/> array.
        /// </summary>
        /// <param name="matrix">The matrix.</param>
        /// <returns>The result of the conversion.</returns>
        public static explicit operator double[,] (Matrix44D matrix)
        {
            double[,] result = new double[4, 4];

            result[0, 0] = matrix.M00; result[0, 1] = matrix.M01; result[0, 2] = matrix.M02; result[0, 3] = matrix.M03;
            result[1, 0] = matrix.M10; result[1, 1] = matrix.M11; result[1, 2] = matrix.M12; result[1, 3] = matrix.M13;
            result[2, 0] = matrix.M20; result[2, 1] = matrix.M21; result[2, 2] = matrix.M22; result[2, 3] = matrix.M23;
            result[3, 0] = matrix.M30; result[3, 1] = matrix.M31; result[3, 2] = matrix.M32; result[3, 3] = matrix.M33;

            return result;
        }


        /// <summary>
        /// Converts this <see cref="Matrix44D"/> to a 2-dimensional <see langword="double"/> array.
        /// </summary>
        /// <returns>The result of the conversion.</returns>
        public double[,] ToArray2D()
        {
            return (double[,])this;
        }


        /// <summary>
        /// Performs an explicit conversion from <see cref="Matrix44D"/> to a jagged 
        /// <see langword="double"/> array.
        /// </summary>
        /// <param name="matrix">The matrix.</param>
        /// <returns>The result of the conversion.</returns>
        public static explicit operator double[][] (Matrix44D matrix)
        {
            double[][] result = new double[4][];
            result[0] = new double[4]; result[1] = new double[4]; result[2] = new double[4]; result[3] = new double[4];

            result[0][0] = matrix.M00; result[0][1] = matrix.M01; result[0][2] = matrix.M02; result[0][3] = matrix.M03;
            result[1][0] = matrix.M10; result[1][1] = matrix.M11; result[1][2] = matrix.M12; result[1][3] = matrix.M13;
            result[2][0] = matrix.M20; result[2][1] = matrix.M21; result[2][2] = matrix.M22; result[2][3] = matrix.M23;
            result[3][0] = matrix.M30; result[3][1] = matrix.M31; result[3][2] = matrix.M32; result[3][3] = matrix.M33;

            return result;
        }


        /// <summary>
        /// Converts this <see cref="Matrix44D"/> to a jagged <see langword="double"/> array.
        /// </summary>
        /// <returns>The result of the conversion.</returns>
        public double[][] ToArrayJagged()
        {
            return (double[][])this;
        }


       

    /// <summary>
    /// Performs an conversion from <see cref="Matrix"/> (XNA Framework) to <see cref="Matrix44D"/>
    /// (DigitalRune Mathematics).
    /// </summary>
    /// <param name="matrix">The <see cref="Matrix"/> (XNA Framework).</param>
    /// <returns>The <see cref="Matrix44D"/> (DigitalRune Mathematics).</returns>
    /// <remarks>
    /// <para>
    /// DigitalRune Mathematics uses column vectors whereas the XNA Framework uses row vectors. By
    /// converting a <see cref="Matrix"/> (XNA Framework) to <see cref="Matrix44D"/> (DigitalRune
    /// Mathematics) the matrix is automatically transposed.
    /// </para>
    /// <para>
    /// This method is available only in the XNA-compatible build of the
    /// DigitalRune.Mathematics.dll.
    /// </para>
    /// </remarks>
    public static explicit operator Matrix44D(Matrix matrix)
    {
      return new Matrix44D(matrix.M11, matrix.M21, matrix.M31, matrix.M41,
                           matrix.M12, matrix.M22, matrix.M32, matrix.M42,
                           matrix.M13, matrix.M23, matrix.M33, matrix.M43,
                           matrix.M14, matrix.M24, matrix.M34, matrix.M44);
    }


    /// <summary>
    /// Converts a <see cref="Matrix"/> (XNA Framework) to a <see cref="Matrix44F"/> (DigitalRune 
    /// Mathematics).
    /// </summary>
    /// <param name="matrix">The <see cref="Matrix"/> (XNA Framework).</param>
    /// <returns>The <see cref="Matrix44D"/> (DigitalRune Mathematics).</returns>
    /// <remarks>
    /// <para>
    /// DigitalRune Mathematics uses column vectors whereas the XNA Framework uses row vectors. By
    /// converting a <see cref="Matrix"/> (XNA Framework) to <see cref="Matrix44D"/> (DigitalRune
    /// Mathematics) the matrix is automatically transposed.
    /// </para>
    /// <para>
    /// This method is available only in the XNA-compatible build of the 
    /// DigitalRune.Mathematics.dll.
    /// </para>
    /// </remarks>
    public static Matrix44D FromXna(Matrix matrix)
    {
      return new Matrix44D(matrix.M11, matrix.M21, matrix.M31, matrix.M41,
                           matrix.M12, matrix.M22, matrix.M32, matrix.M42,
                           matrix.M13, matrix.M23, matrix.M33, matrix.M43,
                           matrix.M14, matrix.M24, matrix.M34, matrix.M44);
    }


    /// <summary>
    /// Performs an conversion from <see cref="Matrix44D"/> (DigitalRune Mathematics) to 
    /// <see cref="Matrix"/> (XNA Framework).
    /// </summary>
    /// <param name="matrix">The <see cref="Matrix44D"/> (DigitalRune Mathematics).</param>
    /// <returns>The <see cref="Matrix"/> (XNA Framework).</returns>
    /// <remarks>
    /// <para>
    /// DigitalRune Mathematics uses column vectors whereas the XNA Framework uses row vectors. By
    /// converting a <see cref="Matrix44D"/> (DigitalRune Mathematics) to <see cref="Matrix"/> (XNA 
    /// Framework) the matrix is automatically transposed.
    /// </para>
    /// <para>
    /// This method is available only in the XNA-compatible build of the
    /// DigitalRune.Mathematics.dll.
    /// </para>
    /// </remarks>
    public static explicit operator Matrix(Matrix44D matrix)
    {
      return new Matrix((float)matrix.M00, (float)matrix.M10, (float)matrix.M20, (float)matrix.M30,
                        (float)matrix.M01, (float)matrix.M11, (float)matrix.M21, (float)matrix.M31,
                        (float)matrix.M02, (float)matrix.M12, (float)matrix.M22, (float)matrix.M32,
                        (float)matrix.M03, (float)matrix.M13, (float)matrix.M23, (float)matrix.M33);
    }


    /// <summary>
    /// Converts this <see cref="Matrix44D"/> (DigitalRune Mathematics) to <see cref="Matrix"/> 
    /// (XNA Framework).
    /// </summary>
    /// <returns>The <see cref="Matrix"/> (XNA Framework).</returns>
    /// <remarks>
    /// <para>
    /// DigitalRune Mathematics uses column vectors whereas the XNA Framework uses row vectors. By
    /// converting a <see cref="Matrix44D"/> (DigitalRune Mathematics) to <see cref="Matrix"/> (XNA 
    /// Framework) the matrix is automatically transposed.
    /// </para>
    /// <para>
    /// This method is available only in the XNA-compatible build of the
    /// DigitalRune.Mathematics.dll.
    /// </para>
    /// </remarks>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly")]
    public Matrix ToXna()
    {
      return new Matrix((float)M00, (float)M10, (float)M20, (float)M30,
                        (float)M01, (float)M11, (float)M21, (float)M31,
                        (float)M02, (float)M12, (float)M22, (float)M32,
                        (float)M03, (float)M13, (float)M23, (float)M33);
    }

        #endregion


        //--------------------------------------------------------------
        #region Methods
        //--------------------------------------------------------------

        /// <overloads>
        /// <summary>
        /// Sets each matrix element to its absolute value.
        /// </summary>
        /// </overloads>
        /// 
        /// <summary>
        /// Sets each matrix element to its absolute value.
        /// </summary>
        public void Absolute()
        {
            M00 = Math.Abs(M00); M01 = Math.Abs(M01); M02 = Math.Abs(M02); M03 = Math.Abs(M03);
            M10 = Math.Abs(M10); M11 = Math.Abs(M11); M12 = Math.Abs(M12); M13 = Math.Abs(M13);
            M20 = Math.Abs(M20); M21 = Math.Abs(M21); M22 = Math.Abs(M22); M23 = Math.Abs(M23);
            M30 = Math.Abs(M30); M31 = Math.Abs(M31); M32 = Math.Abs(M32); M33 = Math.Abs(M33);
        }


        /// <overloads>
        /// <summary>
        /// Clamps near-zero matrix elements to zero.
        /// </summary>
        /// </overloads>
        /// 
        /// <summary>
        /// Clamps near-zero matrix elements to zero.
        /// </summary>
        /// <remarks>
        /// Each matrix element is compared to zero. If the element is in the interval 
        /// [-<see cref="Numeric.EpsilonD"/>, +<see cref="Numeric.EpsilonD"/>] it is set to zero, 
        /// otherwise it remains unchanged.
        /// </remarks>
        public void ClampToZero()
        {
            M00 = DoubleClassExtension.ClampToZero(M00);
            M01 = DoubleClassExtension.ClampToZero(M01);
            M02 = DoubleClassExtension.ClampToZero(M02);
            M03 = DoubleClassExtension.ClampToZero(M03);

            M10 = DoubleClassExtension.ClampToZero(M10);
            M11 = DoubleClassExtension.ClampToZero(M11);
            M12 = DoubleClassExtension.ClampToZero(M12);
            M13 = DoubleClassExtension.ClampToZero(M13);

            M20 = DoubleClassExtension.ClampToZero(M20);
            M21 = DoubleClassExtension.ClampToZero(M21);
            M22 = DoubleClassExtension.ClampToZero(M22);
            M23 = DoubleClassExtension.ClampToZero(M23);

            M30 = DoubleClassExtension.ClampToZero(M30);
            M31 = DoubleClassExtension.ClampToZero(M31);
            M32 = DoubleClassExtension.ClampToZero(M32);
            M33 = DoubleClassExtension.ClampToZero(M33);
        }


        /// <summary>
        /// Clamps near-zero matrix elements to zero.
        /// </summary>
        /// <param name="epsilon">The tolerance value.</param>
        /// <remarks>
        /// Each matrix element is compared to zero. If the element is in the interval 
        /// [-<paramref name="epsilon"/>, +<paramref name="epsilon"/>] it is set to zero, otherwise it 
        /// remains unchanged.
        /// </remarks>
        public void ClampToZero(double epsilon)
        {
            M00 = DoubleClassExtension.ClampToZero(M00, epsilon);
            M01 = DoubleClassExtension.ClampToZero(M01, epsilon);
            M02 = DoubleClassExtension.ClampToZero(M02, epsilon);
            M03 = DoubleClassExtension.ClampToZero(M03, epsilon);

            M10 = DoubleClassExtension.ClampToZero(M10, epsilon);
            M11 = DoubleClassExtension.ClampToZero(M11, epsilon);
            M12 = DoubleClassExtension.ClampToZero(M12, epsilon);
            M13 = DoubleClassExtension.ClampToZero(M13, epsilon);

            M20 = DoubleClassExtension.ClampToZero(M20, epsilon);
            M21 = DoubleClassExtension.ClampToZero(M21, epsilon);
            M22 = DoubleClassExtension.ClampToZero(M22, epsilon);
            M23 = DoubleClassExtension.ClampToZero(M23, epsilon);

            M30 = DoubleClassExtension.ClampToZero(M30, epsilon);
            M31 = DoubleClassExtension.ClampToZero(M31, epsilon);
            M32 = DoubleClassExtension.ClampToZero(M32, epsilon);
            M33 = DoubleClassExtension.ClampToZero(M33, epsilon);
        }


        

        /// <summary>
        /// Decomposes the matrix into the scale, translation, and rotation components.
        /// </summary>
        /// <param name="scale">The scale component of the matrix.</param>
        /// <param name="rotation">The rotation component of the matrix.</param>
        /// <param name="translation">The translation component of the matrix.</param>
        /// <returns>
        /// <see langword="true"/> if the matrix was successfully decomposed; otherwise, 
        /// <see langword="false"/>.
        /// </returns>
        /// <remarks>
        /// <para>
        /// This method assumes that the matrix is a 3D scale/rotation/translation (SRT) matrix.
        /// <see cref="Decompose(out Vector3D,out Matrix33D,out Vector3D)"/> returns 
        /// <see langword="false"/> when the matrix is not a valid SRT matrix. This is the case when two
        /// or more of the scale values are 0 or the last row of the matrix is something other than 
        /// (0, 0, 0, 1).
        /// </para>
        /// <para>
        /// <see cref="DecomposeFast(out Vector3D,out Matrix33D,out Vector3D)"/> is a faster version of 
        /// this method that can be used when it is certain that the matrix is a valid SRT matrix.
        /// </para>
        /// </remarks>
        /// <example>
        /// The following example shows how to compose the matrix scale, rotation, and translation
        /// components.
        /// <code>
        /// Matrix33D sr = rotation * Matrix33D.CreateScale(scale);
        /// Matrix44D srt = new Matrix44D(sr, translation);
        /// </code>
        /// </example>
        public bool Decompose(out Vector3D scale, out Matrix33D rotation, out Vector3D translation)
        {
            // Extract translation
            translation.X = M03;
            translation.Y = M13;
            translation.Z = M23;

            // Extract minor matrix that contains scale and rotation.
            Vector3D column0 = new Vector3D(M00, M10, M20);
            Vector3D column1 = new Vector3D(M01, M11, M21);
            Vector3D column2 = new Vector3D(M02, M12, M22);

            // Extract scale
            scale.X = column0.Length;
            scale.Y = column1.Length;
            scale.Z = column2.Length;

            // Remove scale from minor matrix
            column0 /= scale.X;
            column1 /= scale.Y;
            column2 /= scale.Z;

            // Check whether a scale is 0.
            // If only one scale component is 0, we can still compute the rotation matrix.
            bool scaleXIsZero = DoubleClassExtension.IsZero(scale.X);
            bool scaleYIsZero = DoubleClassExtension.IsZero(scale.Y);
            bool scaleZIsZero = DoubleClassExtension.IsZero(scale.Z);
            if (!scaleXIsZero && !scaleYIsZero && !scaleZIsZero)
            {
                rotation = new Matrix33D(column0.X, column1.X, column2.X,
                                         column0.Y, column1.Y, column2.Y,
                                         column0.Z, column1.Z, column2.Z);

                if (!rotation.IsOrthogonal)
                {
                    rotation = Matrix33D.Identity;
                    return false;
                }

                if (!DoubleClassExtension.AreEqual(1, rotation.Determinant))
                {
                    // The rotation matrix contains a mirroring. We can correct this by inverting any
                    // any scale component.
                    scale.X *= -1;
                    rotation.M00 *= -1;
                    rotation.M10 *= -1;
                    rotation.M20 *= -1;
                }
            }
            else
            {
                if (scaleXIsZero)
                {
                    if (scaleYIsZero || scaleZIsZero || !DoubleClassExtension.IsZero(Vector3D.Dot(column1, column2)))
                    {
                        rotation = Matrix33D.Identity;
                        return false;
                    }

                    column0 = Vector3D.Cross(column1, column2);
                }
                else if (scaleYIsZero)
                {
                    if (scaleZIsZero || !DoubleClassExtension.IsZero(Vector3D.Dot(column2, column0)))
                    {
                        rotation = Matrix33D.Identity;
                        return false;
                    }

                    column1 = Vector3D.Cross(column2, column0);
                }
                else
                {
                    if (!DoubleClassExtension.IsZero(Vector3D.Dot(column0, column1)))
                    {
                        rotation = Matrix33D.Identity;
                        return false;
                    }

                    column2 = Vector3D.Cross(column0, column1);
                }

                rotation = new Matrix33D(column0.X, column1.X, column2.X,
                                         column0.Y, column1.Y, column2.Y,
                                         column0.Z, column1.Z, column2.Z);
            }

            return DoubleClassExtension.IsZero(M30) && DoubleClassExtension.IsZero(M31) && DoubleClassExtension.IsZero(M32) && DoubleClassExtension.AreEqual(M33, 1.0f);
        }


        
        /// <summary>
        /// Decomposes the matrix into the scale, translation, and rotation components. (This method is
        /// faster than <see cref="Decompose(out Vector3D,out Matrix33D,out Vector3D)"/>, but the matrix
        /// must be a valid 3D scale/rotation/translation (SRT) matrix.)
        /// </summary>
        /// <param name="scale">The scale component of the matrix.</param>
        /// <param name="rotation">The rotation component of the matrix.</param>
        /// <param name="translation">The translation component of the matrix.</param>
        /// <remarks>
        /// This method requires that the matrix is a 3D scale/rotation/translation (SRT) matrix. See
        /// also <see cref="Decompose(out Vector3D,out Matrix33D,out Vector3D)"/>.
        /// </remarks>
        public void DecomposeFast(out Vector3D scale, out Matrix33D rotation, out Vector3D translation)
        {
            // Extract translation
            translation.X = M03;
            translation.Y = M13;
            translation.Z = M23;

            // Extract minor matrix that contains scale and rotation.
            Vector3D column0 = new Vector3D(M00, M10, M20);
            Vector3D column1 = new Vector3D(M01, M11, M21);
            Vector3D column2 = new Vector3D(M02, M12, M22);

            // Extract scale
            scale.X = column0.Length;
            scale.Y = column1.Length;
            scale.Z = column2.Length;

            // Remove scale from minor matrix
            column0 /= scale.X;
            column1 /= scale.Y;
            column2 /= scale.Z;

            // Check whether a scale is 0.
            // If only one scale component is 0, we can still compute the rotation matrix.
            bool scaleXIsZero = DoubleClassExtension.IsZero(scale.X);
            bool scaleYIsZero = DoubleClassExtension.IsZero(scale.Y);
            bool scaleZIsZero = DoubleClassExtension.IsZero(scale.Z);
            if (!scaleXIsZero && !scaleYIsZero && !scaleZIsZero)
            {
                rotation = new Matrix33D(column0.X, column1.X, column2.X,
                                         column0.Y, column1.Y, column2.Y,
                                         column0.Z, column1.Z, column2.Z);

                if (rotation.Determinant < 0)
                {
                    scale.X *= -1;
                    rotation.M00 *= -1;
                    rotation.M10 *= -1;
                    rotation.M20 *= -1;
                }
            }
            else
            {
                if (scaleXIsZero)
                    column0 = Vector3D.Cross(column1, column2);
                else if (scaleYIsZero)
                    column1 = Vector3D.Cross(column2, column0);
                else
                    column2 = Vector3D.Cross(column0, column1);

                rotation = new Matrix33D(column0.X, column1.X, column2.X,
                                         column0.Y, column1.Y, column2.Y,
                                         column0.Z, column1.Z, column2.Z);
            }
        }



        /// <summary>
        /// Inverts the matrix.
        /// </summary>
        /// <exception cref="MathematicsException">
        /// The matrix is singular (i.e. it is not invertible).
        /// </exception>
        /// <seealso cref="Inverse"/>
        /// <seealso cref="TryInvert"/>
        public void Invert()
        {
            if (TryInvert() == false)
                throw new Exception("Matrix is singular (i.e. it is not invertible).");
        }


        /// <summary>
        /// Converts this matrix to an array of <see langword="double"/> values.
        /// </summary>
        /// <param name="order">The order of the matrix elements in the array.</param>
        /// <returns>The result of the conversion.</returns>
        public double[] ToArray1D(MatrixOrder order)
        {
            double[] array = new double[16];

            if (order == MatrixOrder.ColumnMajor)
            {
                array[0] = M00; array[1] = M10; array[2] = M20; array[3] = M30;
                array[4] = M01; array[5] = M11; array[6] = M21; array[7] = M31;
                array[8] = M02; array[9] = M12; array[10] = M22; array[11] = M32;
                array[12] = M03; array[13] = M13; array[14] = M23; array[15] = M33;
            }
            else
            {
                array[0] = M00; array[1] = M01; array[2] = M02; array[3] = M03;
                array[4] = M10; array[5] = M11; array[6] = M12; array[7] = M13;
                array[8] = M20; array[9] = M21; array[10] = M22; array[11] = M23;
                array[12] = M30; array[13] = M31; array[14] = M32; array[15] = M33;
            }

            return array;
        }


        /// <summary>
        /// Converts this matrix to a list of <see langword="double"/> values.
        /// </summary>
        /// <param name="order">The order of the matrix elements in the list.</param>
        /// <returns>The result of the conversion.</returns>
        public IList<double> ToList(MatrixOrder order)
        {
            List<double> result = new List<double>(16);

            if (order == MatrixOrder.ColumnMajor)
            {
                result.Add(M00); result.Add(M10); result.Add(M20); result.Add(M30);
                result.Add(M01); result.Add(M11); result.Add(M21); result.Add(M31);
                result.Add(M02); result.Add(M12); result.Add(M22); result.Add(M32);
                result.Add(M03); result.Add(M13); result.Add(M23); result.Add(M33);
            }
            else
            {
                result.Add(M00); result.Add(M01); result.Add(M02); result.Add(M03);
                result.Add(M10); result.Add(M11); result.Add(M12); result.Add(M13);
                result.Add(M20); result.Add(M21); result.Add(M22); result.Add(M23);
                result.Add(M30); result.Add(M31); result.Add(M32); result.Add(M33);
            }

            return result;
        }


        /// <summary>
        /// Inverts the matrix if it is invertible.
        /// </summary>
        /// <returns>
        /// <see langword="true"/> if the matrix is invertible; otherwise <see langword="false"/>.
        /// </returns>
        /// <remarks>
        /// <para>
        /// This method is the equivalent to <see cref="Invert"/>, except that no exceptions are thrown.
        /// The return value indicates whether the operation was successful.
        /// </para>
        /// <para>
        /// Due to numerical errors it can happen that some singular matrices are not recognized as 
        /// singular by this method. This method is optimized for fast matrix inversion and not for safe
        /// detection of singular matrices. If you need to detect if a matrix is singular, you can, for 
        /// example, compute its <see cref="Determinant"/> and see if it is near zero.
        /// </para>
        /// </remarks>
        public bool TryInvert()
        {
            double m22m33_m23m32 = M22 * M33 - M23 * M32;
            double m21m33_m23m31 = M21 * M33 - M23 * M31;
            double m21m32_m22m31 = M21 * M32 - M22 * M31;
            double m20m33_m23m30 = M20 * M33 - M23 * M30;
            double m20m32_m22m30 = M20 * M32 - M22 * M30;
            double m20m31_m21m30 = M20 * M31 - M21 * M30;

            double detSubMatrix00 = M11 * m22m33_m23m32 - M12 * m21m33_m23m31 + M13 * m21m32_m22m31;
            double detSubMatrix01 = M10 * m22m33_m23m32 - M12 * m20m33_m23m30 + M13 * m20m32_m22m30;
            double detSubMatrix02 = M10 * m21m33_m23m31 - M11 * m20m33_m23m30 + M13 * m20m31_m21m30;
            double detSubMatrix03 = M10 * m21m32_m22m31 - M11 * m20m32_m22m30 + M12 * m20m31_m21m30;

            // Develop determinant after first row:
            double determinant = M00 * detSubMatrix00 - M01 * detSubMatrix01 + M02 * detSubMatrix02 - M03 * detSubMatrix03;

            // We check if determinant is zero using a very small epsilon, since the determinant
            // is the result of many multiplications of potentially small numbers.
            if (DoubleClassExtension.IsZero(determinant, DoubleClassExtension.EpsilonDSquared * DoubleClassExtension.EpsilonDSquared))
                return false;

            double detSubMatrix10 = M01 * m22m33_m23m32 - M02 * m21m33_m23m31 + M03 * m21m32_m22m31;
            double detSubMatrix11 = M00 * m22m33_m23m32 - M02 * m20m33_m23m30 + M03 * m20m32_m22m30;
            double detSubMatrix12 = M00 * m21m33_m23m31 - M01 * m20m33_m23m30 + M03 * m20m31_m21m30;
            double detSubMatrix13 = M00 * m21m32_m22m31 - M01 * m20m32_m22m30 + M02 * m20m31_m21m30;

            double m02m13_m03m12 = M02 * M13 - M03 * M12;
            double m01m13_m03m11 = M01 * M13 - M03 * M11;
            double m01m12_m02m11 = M01 * M12 - M02 * M11;
            double m00m13_m03m10 = M00 * M13 - M03 * M10;
            double m00m12_m02m10 = M00 * M12 - M02 * M10;
            double m00m11_m01m10 = M00 * M11 - M01 * M10;
            double detSubMatrix20 = M31 * m02m13_m03m12 - M32 * m01m13_m03m11 + M33 * m01m12_m02m11;
            double detSubMatrix21 = M30 * m02m13_m03m12 - M32 * m00m13_m03m10 + M33 * m00m12_m02m10;
            double detSubMatrix22 = M30 * m01m13_m03m11 - M31 * m00m13_m03m10 + M33 * m00m11_m01m10;
            double detSubMatrix23 = M30 * m01m12_m02m11 - M31 * m00m12_m02m10 + M32 * m00m11_m01m10;

            double detSubMatrix30 = M21 * m02m13_m03m12 - M22 * m01m13_m03m11 + M23 * m01m12_m02m11;
            double detSubMatrix31 = M20 * m02m13_m03m12 - M22 * m00m13_m03m10 + M23 * m00m12_m02m10;
            double detSubMatrix32 = M20 * m01m13_m03m11 - M21 * m00m13_m03m10 + M23 * m00m11_m01m10;
            double detSubMatrix33 = M20 * m01m12_m02m11 - M21 * m00m12_m02m10 + M22 * m00m11_m01m10;

            double f = 1.0 / determinant;
            M00 = detSubMatrix00 * f;
            M01 = -detSubMatrix10 * f;
            M02 = detSubMatrix20 * f;
            M03 = -detSubMatrix30 * f;

            M10 = -detSubMatrix01 * f;
            M11 = detSubMatrix11 * f;
            M12 = -detSubMatrix21 * f;
            M13 = detSubMatrix31 * f;

            M20 = detSubMatrix02 * f;
            M21 = -detSubMatrix12 * f;
            M22 = detSubMatrix22 * f;
            M23 = -detSubMatrix32 * f;

            M30 = -detSubMatrix03 * f;
            M31 = detSubMatrix13 * f;
            M32 = -detSubMatrix23 * f;
            M33 = detSubMatrix33 * f;

            return true;

 
        }


        /// <summary>
        /// Transforms a position.
        /// </summary>
        /// <param name="position">The position.</param>
        /// <returns>The transformed position.</returns>
        /// <remarks>
        /// <para>
        /// By using homogeneous coordinates 4 x 4 matrices can be used to define affine transformations 
        /// or projective transformations in 3D space. When a 3D vector is given, the vector can have 
        /// multiple meanings.
        /// </para>
        /// <para>
        /// <strong>Position Vectors:</strong>
        /// A position vector identifies a point in 3D. Use <see cref="TransformPosition"/> to 
        /// transform position vectors. This method interprets the given <see cref="Vector3D"/> as a 
        /// vector (x, y, z, 1) in homogeneous coordinates. The position vector is transformed by
        /// multiplication with the 4 x 4 matrix.
        /// </para>
        /// <para>
        /// <strong>Direction Vectors:</strong>
        /// A direction vector (or displacement vector) defines a direction and length in 3D. Use 
        /// <see cref="TransformDirection"/> to transform direction vectors. This method interprets the 
        /// given <see cref="Vector3D"/> as a vector (x, y, z, 0) in homogeneous coordinates. The 
        /// direction vector is transformed by multiplication with the upper, left 3 x 3 corner of the
        /// transformation matrix.
        /// </para>
        /// <para>
        /// <strong>Tangent Vectors:</strong>
        /// A tangent vector (surface tangent) defines a tangential direction at a point on a surface. 
        /// They can be treated similar to direction vectors. Use <see cref="TransformDirection"/> to 
        /// transform tangent vectors and binormals vectors.
        /// </para>
        /// <para>
        /// <strong>Normal vectors:</strong>
        /// A normal vector (surface normal) is a vector that is perpendicular to the tangent plane of
        /// a given point on a surface. In differential geometry normal vectors are "tangent covectors" 
        /// or "cotangent vectors". They need to be treated differently than direction vectors or 
        /// tangent vectors. Use <see cref="TransformNormal"/> to transform normal vectors. A normal 
        /// vector is transformed by multiplication with transpose of the inverse of the upper, left 
        /// 3 x 3 corner of the transformation matrix.
        /// </para>
        /// <para>
        /// (Note: If the transformation matrix contains only rotations, translations and uniform 
        /// scalings then <see cref="TransformDirection"/> can be used to transform normal vectors,
        /// which is faster.)
        /// </para>
        /// </remarks>
        /// <see cref="TransformDirection"/>
        /// <see cref="TransformNormal"/>
        public Vector3D TransformPosition(Vector3D position)
        {
            double x = M00 * position.X + M01 * position.Y + M02 * position.Z + M03;
            double y = M10 * position.X + M11 * position.Y + M12 * position.Z + M13;
            double z = M20 * position.X + M21 * position.Y + M22 * position.Z + M23;
            double w = M30 * position.X + M31 * position.Y + M32 * position.Z + M33;

            // Perform homogeneous divide if necessary.
            if (!DoubleClassExtension.AreEqual(w, 1))
            {
                double oneOverW = 1 / w;
                x *= oneOverW;
                y *= oneOverW;
                z *= oneOverW;
            }

            position.X = x;
            position.Y = y;
            position.Z = z;
            return position;
        }


        /// <summary>
        /// Transforms a direction vector (or tangent vector).
        /// </summary>
        /// <param name="direction">The direction vector.</param>
        /// <returns>The transformed direction vector.</returns>
        /// <inheritdoc cref="TransformPosition"/>
        /// <see cref="TransformNormal"/>
        /// <see cref="TransformPosition"/>
        public Vector3D TransformDirection(Vector3D direction)
        {
            double x = M00 * direction.X + M01 * direction.Y + M02 * direction.Z;
            double y = M10 * direction.X + M11 * direction.Y + M12 * direction.Z;
            double z = M20 * direction.X + M21 * direction.Y + M22 * direction.Z;

            direction.X = x;
            direction.Y = y;
            direction.Z = z;

            return direction;
        }


        /// <summary>
        /// Transforms a normal vector.
        /// </summary>
        /// <param name="normal">The normal vector.</param>
        /// <returns>
        /// The transformed normal. (Note: The resulting vector might need to be normalized!)
        /// </returns>
        /// <inheritdoc cref="TransformPosition"/>
        /// <see cref="TransformDirection"/>
        /// <see cref="TransformPosition"/>
        public Vector3D TransformNormal(Vector3D normal)
        {
            // TODO: Optimization - Inline the matrix inversion.
            // When inverting a matrix using Cramer's rule we need to divide by the determinant.
            // We can leave out this division. The resulting normal vector will have a different length,
            // but the normal vector will need to be normalized anyways.

            // Multiply the transpose of the inverse with vector
            Matrix33D inverse = Minor.Inverse;
            double x = inverse.M00 * normal.X + inverse.M10 * normal.Y + inverse.M20 * normal.Z;
            double y = inverse.M01 * normal.X + inverse.M11 * normal.Y + inverse.M21 * normal.Z;
            double z = inverse.M02 * normal.X + inverse.M12 * normal.Y + inverse.M22 * normal.Z;

            normal.X = x;
            normal.Y = y;
            normal.Z = z;

            return normal;
        }


        /// <summary>
        /// Transposes this matrix.
        /// </summary>
        public void Transpose()
        {
            DoubleClassExtension.Swap(ref M01, ref M10);
            DoubleClassExtension.Swap(ref M02, ref M20);
            DoubleClassExtension.Swap(ref M03, ref M30);
            DoubleClassExtension.Swap(ref M12, ref M21);
            DoubleClassExtension.Swap(ref M13, ref M31);
            DoubleClassExtension.Swap(ref M23, ref M32);
        }
        #endregion


        //--------------------------------------------------------------
        #region Static Methods
        //--------------------------------------------------------------

        /// <summary>
        /// Returns a matrix with the absolute values of the elements of the given matrix.
        /// </summary>
        /// <param name="matrix">The matrix.</param>
        /// <returns>A matrix with the absolute values of the elements of the given matrix.</returns>
        public static Matrix44D Absolute(Matrix44D matrix)
        {
            return new Matrix44D(Math.Abs(matrix.M00), Math.Abs(matrix.M01), Math.Abs(matrix.M02), Math.Abs(matrix.M03),
                                 Math.Abs(matrix.M10), Math.Abs(matrix.M11), Math.Abs(matrix.M12), Math.Abs(matrix.M13),
                                 Math.Abs(matrix.M20), Math.Abs(matrix.M21), Math.Abs(matrix.M22), Math.Abs(matrix.M23),
                                 Math.Abs(matrix.M30), Math.Abs(matrix.M31), Math.Abs(matrix.M32), Math.Abs(matrix.M33));
        }


        /// <overloads>
        /// <summary>
        /// Determines whether two matrices are equal (regarding a given tolerance).
        /// </summary>
        /// </overloads>
        /// 
        /// <summary>
        /// Determines whether two matrices are equal (regarding the tolerance 
        /// <see cref="Numeric.EpsilonD"/>).
        /// </summary>
        /// <param name="matrix1">The first matrix.</param>
        /// <param name="matrix2">The second matrix.</param>
        /// <returns>
        /// <see langword="true"/> if the matrices are equal (within the tolerance 
        /// <see cref="Numeric.EpsilonD"/>); otherwise, <see langword="false"/>.
        /// </returns>
        /// <remarks>
        /// The two matrices are compared component-wise. If the differences of the components are less
        /// than <see cref="Numeric.EpsilonD"/> the matrices are considered as being equal.
        /// </remarks>
        public static bool AreNumericallyEqual(Matrix44D matrix1, Matrix44D matrix2)
        {
            return DoubleClassExtension.AreEqual(matrix1.M00, matrix2.M00)
                && DoubleClassExtension.AreEqual(matrix1.M01, matrix2.M01)
                && DoubleClassExtension.AreEqual(matrix1.M02, matrix2.M02)
                && DoubleClassExtension.AreEqual(matrix1.M03, matrix2.M03)
                && DoubleClassExtension.AreEqual(matrix1.M10, matrix2.M10)
                && DoubleClassExtension.AreEqual(matrix1.M11, matrix2.M11)
                && DoubleClassExtension.AreEqual(matrix1.M12, matrix2.M12)
                && DoubleClassExtension.AreEqual(matrix1.M13, matrix2.M13)
                && DoubleClassExtension.AreEqual(matrix1.M20, matrix2.M20)
                && DoubleClassExtension.AreEqual(matrix1.M21, matrix2.M21)
                && DoubleClassExtension.AreEqual(matrix1.M22, matrix2.M22)
                && DoubleClassExtension.AreEqual(matrix1.M23, matrix2.M23)
                && DoubleClassExtension.AreEqual(matrix1.M30, matrix2.M30)
                && DoubleClassExtension.AreEqual(matrix1.M31, matrix2.M31)
                && DoubleClassExtension.AreEqual(matrix1.M32, matrix2.M32)
                && DoubleClassExtension.AreEqual(matrix1.M33, matrix2.M33);
        }


        /// <summary>
        /// Determines whether two matrices are equal (regarding a specific tolerance).
        /// </summary>
        /// <param name="matrix1">The first matrix.</param>
        /// <param name="matrix2">The second matrix.</param>
        /// <param name="epsilon">The tolerance value.</param>
        /// <returns>
        /// <see langword="true"/> if the matrices are equal (within the tolerance
        /// <paramref name="epsilon"/>); otherwise, <see langword="false"/>.
        /// </returns>
        /// <remarks>
        /// The two matrices are compared component-wise. If the differences of the components are less
        /// than <paramref name="epsilon"/> the matrices are considered as being equal.
        /// </remarks>
        public static bool AreNumericallyEqual(Matrix44D matrix1, Matrix44D matrix2, double epsilon)
        {
            return DoubleClassExtension.AreEqual(matrix1.M00, matrix2.M00, epsilon)
                && DoubleClassExtension.AreEqual(matrix1.M01, matrix2.M01, epsilon)
                && DoubleClassExtension.AreEqual(matrix1.M02, matrix2.M02, epsilon)
                && DoubleClassExtension.AreEqual(matrix1.M03, matrix2.M03, epsilon)
                && DoubleClassExtension.AreEqual(matrix1.M10, matrix2.M10, epsilon)
                && DoubleClassExtension.AreEqual(matrix1.M11, matrix2.M11, epsilon)
                && DoubleClassExtension.AreEqual(matrix1.M12, matrix2.M12, epsilon)
                && DoubleClassExtension.AreEqual(matrix1.M13, matrix2.M13, epsilon)
                && DoubleClassExtension.AreEqual(matrix1.M20, matrix2.M20, epsilon)
                && DoubleClassExtension.AreEqual(matrix1.M21, matrix2.M21, epsilon)
                && DoubleClassExtension.AreEqual(matrix1.M22, matrix2.M22, epsilon)
                && DoubleClassExtension.AreEqual(matrix1.M23, matrix2.M23, epsilon)
                && DoubleClassExtension.AreEqual(matrix1.M30, matrix2.M30, epsilon)
                && DoubleClassExtension.AreEqual(matrix1.M31, matrix2.M31, epsilon)
                && DoubleClassExtension.AreEqual(matrix1.M32, matrix2.M32, epsilon)
                && DoubleClassExtension.AreEqual(matrix1.M33, matrix2.M33, epsilon);
        }


        /// <summary>
        /// Returns a matrix with the matrix elements clamped to the range [min, max].
        /// </summary>
        /// <param name="matrix">The matrix.</param>
        /// <returns>The matrix with small elements clamped to zero.</returns>
        /// <remarks>
        /// Each matrix element is compared to zero. If it is in the interval 
        /// [-<see cref="Numeric.EpsilonD"/>, +<see cref="Numeric.EpsilonD"/>] it is set to zero, 
        /// otherwise it remains unchanged.
        /// </remarks>
        public static Matrix44D ClampToZero(Matrix44D matrix)
        {
            matrix.M00 = DoubleClassExtension.ClampToZero(matrix.M00);
            matrix.M01 = DoubleClassExtension.ClampToZero(matrix.M01);
            matrix.M02 = DoubleClassExtension.ClampToZero(matrix.M02);
            matrix.M03 = DoubleClassExtension.ClampToZero(matrix.M03);

            matrix.M10 = DoubleClassExtension.ClampToZero(matrix.M10);
            matrix.M11 = DoubleClassExtension.ClampToZero(matrix.M11);
            matrix.M12 = DoubleClassExtension.ClampToZero(matrix.M12);
            matrix.M13 = DoubleClassExtension.ClampToZero(matrix.M13);

            matrix.M20 = DoubleClassExtension.ClampToZero(matrix.M20);
            matrix.M21 = DoubleClassExtension.ClampToZero(matrix.M21);
            matrix.M22 = DoubleClassExtension.ClampToZero(matrix.M22);
            matrix.M23 = DoubleClassExtension.ClampToZero(matrix.M23);

            matrix.M30 = DoubleClassExtension.ClampToZero(matrix.M30);
            matrix.M31 = DoubleClassExtension.ClampToZero(matrix.M31);
            matrix.M32 = DoubleClassExtension.ClampToZero(matrix.M32);
            matrix.M33 = DoubleClassExtension.ClampToZero(matrix.M33);

            return matrix;
        }


        /// <summary>
        /// Returns a matrix with the matrix elements clamped to the range [min, max].
        /// </summary>
        /// <param name="matrix">The matrix.</param>
        /// <param name="epsilon">The tolerance value.</param>
        /// <returns>The matrix with small elements clamped to zero.</returns>
        /// <remarks>
        /// Each matrix element is compared to zero. If it is in the interval 
        /// [-<paramref name="epsilon"/>, +<paramref name="epsilon"/>] it is set to zero, otherwise it 
        /// remains unchanged.
        /// </remarks>
        public static Matrix44D ClampToZero(Matrix44D matrix, double epsilon)
        {
            matrix.M00 = DoubleClassExtension.ClampToZero(matrix.M00, epsilon);
            matrix.M01 = DoubleClassExtension.ClampToZero(matrix.M01, epsilon);
            matrix.M02 = DoubleClassExtension.ClampToZero(matrix.M02, epsilon);
            matrix.M03 = DoubleClassExtension.ClampToZero(matrix.M03, epsilon);

            matrix.M10 = DoubleClassExtension.ClampToZero(matrix.M10, epsilon);
            matrix.M11 = DoubleClassExtension.ClampToZero(matrix.M11, epsilon);
            matrix.M12 = DoubleClassExtension.ClampToZero(matrix.M12, epsilon);
            matrix.M13 = DoubleClassExtension.ClampToZero(matrix.M13, epsilon);

            matrix.M20 = DoubleClassExtension.ClampToZero(matrix.M20, epsilon);
            matrix.M21 = DoubleClassExtension.ClampToZero(matrix.M21, epsilon);
            matrix.M22 = DoubleClassExtension.ClampToZero(matrix.M22, epsilon);
            matrix.M23 = DoubleClassExtension.ClampToZero(matrix.M23, epsilon);

            matrix.M30 = DoubleClassExtension.ClampToZero(matrix.M30, epsilon);
            matrix.M31 = DoubleClassExtension.ClampToZero(matrix.M31, epsilon);
            matrix.M32 = DoubleClassExtension.ClampToZero(matrix.M32, epsilon);
            matrix.M33 = DoubleClassExtension.ClampToZero(matrix.M33, epsilon);

            return matrix;
        }


        /// <overloads>
        /// <summary>
        /// Creates a scaling matrix.
        /// </summary>
        /// </overloads>
        /// 
        /// <summary>
        /// Creates a scaling matrix.
        /// </summary>
        /// <param name="scale">
        /// The uniform scale factor that is applied to the x-, y-, and z-axis.
        /// </param>
        /// <returns>The created scaling matrix.</returns>
        public static Matrix44D CreateScale(double scale)
        {
            Matrix44D result = new Matrix44D
            {
                M00 = scale,
                M11 = scale,
                M22 = scale,
                M33 = 1.0
            };
            return result;
        }


        /// <summary>
        /// Creates a scaling matrix.
        /// </summary>
        /// <param name="scaleX">The value to scale by on the x-axis.</param>
        /// <param name="scaleY">The value to scale by on the y-axis.</param>
        /// <param name="scaleZ">The value to scale by on the z-axis.</param>
        /// <returns>The created scaling matrix.</returns>
        public static Matrix44D CreateScale(double scaleX, double scaleY, double scaleZ)
        {
            Matrix44D result = new Matrix44D
            {
                M00 = scaleX,
                M11 = scaleY,
                M22 = scaleZ,
                M33 = 1.0
            };
            return result;
        }


        /// <summary>
        /// Creates a scaling matrix.
        /// </summary>
        /// <param name="scale">Amounts to scale by the x, y, and z-axis.</param>
        /// <returns>The created scaling matrix.</returns>
        public static Matrix44D CreateScale(Vector3D scale)
        {
            Matrix44D result = new Matrix44D
            {
                M00 = scale.X,
                M11 = scale.Y,
                M22 = scale.Z,
                M33 = 1.0
            };
            return result;
        }


        /// <overloads>
        /// <summary>
        /// Creates a rotation matrix.
        /// </summary>
        /// </overloads>
        /// 
        /// <summary>
        /// Creates a rotation matrix from axis and angle.
        /// </summary>
        /// <param name="axis">The rotation axis. (Does not need to be normalized.)</param>
        /// <param name="angle">The rotation angle in radians.</param>
        /// <returns>The created rotation matrix.</returns>
        /// <exception cref="ArgumentException">
        /// The <paramref name="axis"/> vector has 0 length.
        /// </exception>
        public static Matrix44D CreateRotation(Vector3D axis, double angle)
        {
            if (!axis.TryNormalize())
                throw new ArgumentException("The axis vector has length 0.");

            double x = axis.X;
            double y = axis.Y;
            double z = axis.Z;
            double x2 = x * x;
            double y2 = y * y;
            double z2 = z * z;
            double xy = x * y;
            double xz = x * z;
            double yz = y * z;
            double co = Math.Cos(angle);
            double si = Math.Sin(angle);
            double xsi = x * si;
            double ysi = y * si;
            double zsi = z * si;
            double oneMinusCo = 1.0 - co;

            Matrix44D result;
            result.M00 = x2 + co * (1.0 - x2);
            result.M01 = xy * oneMinusCo - zsi;
            result.M02 = xz * oneMinusCo + ysi;
            result.M03 = 0.0;
            result.M10 = xy * oneMinusCo + zsi;
            result.M11 = y2 + co * (1.0 - y2);
            result.M12 = yz * oneMinusCo - xsi;
            result.M13 = 0.0;
            result.M20 = xz * oneMinusCo - ysi;
            result.M21 = yz * oneMinusCo + xsi;
            result.M22 = z2 + co * (1.0 - z2);
            result.M23 = 0.0;
            result.M30 = 0.0;
            result.M31 = 0.0;
            result.M32 = 0.0;
            result.M33 = 1.0;
            return result;
        }


        

        /// <overloads>
        /// <summary>
        /// Creates a translation matrix.
        /// </summary>
        /// </overloads>
        /// 
        /// <summary>
        /// Creates a translation matrix from the given values.
        /// </summary>
        /// <param name="x">The translation along the x-axis.</param>
        /// <param name="y">The translation along the y-axis.</param>
        /// <param name="z">The translation along the z-axis.</param>
        /// <returns>A transformation matrix that translates vectors.</returns>
        public static Matrix44D CreateTranslation(double x, double y, double z)
        {
            Matrix44D result = Identity;
            result.M03 = x;
            result.M13 = y;
            result.M23 = z;
            result.M33 = 1;
            return result;
        }


        /// <summary>
        /// Creates a translation matrix from a vector.
        /// </summary>
        /// <param name="translation">The translation.</param>
        /// <returns>A transformation matrix that translates vectors.</returns>
        public static Matrix44D CreateTranslation(Vector3D translation)
        {
            Matrix44D result = Identity;
            result.M03 = translation.X;
            result.M13 = translation.Y;
            result.M23 = translation.Z;
            result.M33 = 1;
            return result;
        }


        /// <summary>
        /// Creates a matrix that specifies a rotation around the x-axis.
        /// </summary>
        /// <param name="angle">The rotation angle in radians.</param>
        /// <returns>The created rotation matrix.</returns>
        public static Matrix44D CreateRotationX(double angle)
        {
            double cos = Math.Cos(angle);
            double sin = Math.Sin(angle);
            return new Matrix44D(1, 0, 0, 0,
                                 0, cos, -sin, 0,
                                 0, sin, cos, 0,
                                 0, 0, 0, 1);
        }


        /// <summary>
        /// Creates a matrix that specifies a rotation around the y-axis.
        /// </summary>
        /// <param name="angle">The rotation angle in radians.</param>
        /// <returns>The created rotation matrix.</returns>
        public static Matrix44D CreateRotationY(double angle)
        {
            double cos = Math.Cos(angle);
            double sin = Math.Sin(angle);
            return new Matrix44D(cos, 0, sin, 0,
                                 0, 1, 0, 0,
                                 -sin, 0, cos, 0,
                                 0, 0, 0, 1);
        }


        /// <summary>
        /// Creates a matrix that specifies a rotation around the z-axis.
        /// </summary>
        /// <param name="angle">The rotation angle in radians.</param>
        /// <returns>The created rotation matrix.</returns>
        public static Matrix44D CreateRotationZ(double angle)
        {
            double cos = Math.Cos(angle);
            double sin = Math.Sin(angle);
            return new Matrix44D(cos, -sin, 0, 0,
                                 sin, cos, 0, 0,
                                 0, 0, 1, 0,
                                 0, 0, 0, 1);
        }


    /// <summary>
    /// Creates a right-handed look-at matrix (view matrix). (Only available in the XNA-compatible 
    /// build.)
    /// </summary>
    /// <param name="position">The position of the viewer.</param>
    /// <param name="target">The target at which the viewer is looking.</param>
    /// <param name="upVector">
    /// The direction that is "up" from the viewer's point of view. (Does not need to be 
    /// normalized.)
    /// </param>
    /// <returns>The right-handed look-at matrix (view matrix).</returns>
    /// <remarks>
    /// <para>
    /// This method is available only in the XNA-compatible build of the 
    /// DigitalRune.Mathematics.dll.
    /// </para>
    /// <para>
    /// A look-at matrix is also known as view matrix. It transforms a position from world space to
    /// the view space. In view space x-axis points to the right, the y-axis points up, and the 
    /// z-axis points towards the viewer.
    /// </para>
    /// </remarks>
    /// <exception cref="ArgumentException">
    /// <paramref name="position"/> is the same as <paramref name="target"/>.
    /// </exception>
    /// <exception cref="ArgumentException">
    /// <paramref name="upVector"/> is (0, 0, 0).
    /// </exception>
    /// <exception cref="DivideByZeroException">
    /// The camera direction (<paramref name="target"/> - <paramref name="position"/>) is probably
    /// pointing in the same or opposite direction as <paramref name="upVector"/>. (The two vectors
    /// must not be parallel.)
    /// </exception>
    public static Matrix44D CreateLookAt(Vector3D position, Vector3D target, Vector3D upVector)
    {
      // See DirectX, D3DXMatrixLookAtRH().
      // This method is XNA-specific because the result depends on the used world/view coordinate
      // system (in XNA: +x is to the right, +y is up, -z is forward).

      if (Vector3D.AreNumericallyEqual(position, target))
        throw new ArgumentException("The target position must be different than the camera position", "target");
      if (upVector.IsNumericallyZero)
        throw new ArgumentException("The up-vector of the camera must not be 0.", "upVector");

      Vector3D zAxis = (position - target).Normalized;
      Vector3D xAxis = Vector3D.Cross(upVector, zAxis).Normalized;
      Vector3D yAxis = Vector3D.Cross(zAxis, xAxis);

      return new Matrix44D(
        xAxis.X, xAxis.Y, xAxis.Z, -Vector3D.Dot(xAxis, position),
        yAxis.X, yAxis.Y, yAxis.Z, -Vector3D.Dot(yAxis, position),
        zAxis.X, zAxis.Y, zAxis.Z, -Vector3D.Dot(zAxis, position),
        0, 0, 0, 1);
    }


    /// <summary>
    /// Creates a right-handed, orthographic projection matrix. (Only available in the XNA 
    /// compatible build.)
    /// </summary>
    /// <param name="width">The width of the view volume.</param>
    /// <param name="height">The height of the view volume.</param>
    /// <param name="zNear">
    /// The minimum z-value of the view volume. (Distance of the near view plane.)
    /// </param>
    /// <param name="zFar">
    /// The maximum z-value of the view volume. (Distance of the far view plane.)
    /// </param>
    /// <returns>The right-handed orthographic projection matrix.</returns>
    /// <remarks>
    /// <para>
    /// This method is available only in the XNA-compatible build of the 
    /// DigitalRune.Mathematics.dll.
    /// </para>
    /// <para>
    /// In contrast to all preceding coordinate spaces (model space, world space, view space) the 
    /// projection space is left-handed! This is necessary because DirectX uses a left-handed 
    /// clip space.
    /// </para>
    /// <para>
    /// In the projection space the x and y-coordinates range from −1 to 1, and the z-coordinates
    /// range from 0 (near) to 1 (far).
    /// </para>
    /// </remarks>
    /// <exception cref="ArgumentOutOfRangeException">
    /// <paramref name="width"/> or <paramref name="height"/> is 0.
    /// </exception>
    /// <exception cref="ArgumentException">
    /// <paramref name="zNear"/> is greater than or equal to <paramref name="zFar"/>.
    /// </exception>
    public static Matrix44D CreateOrthographic(double width, double height, double zNear, double zFar)
    {
      // See DirectX, D3DXMatrixOrthoRH().

      if (width == 0)
        throw new ArgumentOutOfRangeException("width", "The width must greater than 0.");
      if (height == 0)
        throw new ArgumentOutOfRangeException("height", "The height must greater than 0.");
      if (zNear >= zFar)
        throw new ArgumentException("The distance to the near view plane must be less than the distance to the far view plane (zNear < zFar).");

      return new Matrix44D(
        2 / width, 0, 0, 0,
        0, 2 / height, 0, 0,
        0, 0, 1 / (zNear - zFar), zNear / (zNear - zFar),
        0, 0, 0, 1);
    }


    /// <summary>
    /// Creates a customized (off-center), right-handed, orthographic projection matrix. (Only 
    /// available in the XNA-compatible build.)
    /// </summary>
    /// <param name="left">The minimum x-value of the view volume.</param>
    /// <param name="right">The maximum x-value of the view volume.</param>
    /// <param name="bottom">The minimum y-value of the view volume.</param>
    /// <param name="top">The maximum y-value of the view volume.</param>
    /// <param name="zNear">
    /// The minimum z-value of the view volume. (Distance of the near view plane.)
    /// </param>
    /// <param name="zFar">
    /// The maximum z-value of the view volume. (Distance of the far view plane.)
    /// </param>
    /// <returns>The customized (off-center), right-handed orthographic projection matrix.</returns>
    /// <remarks>
    /// <para>
    /// This method is available only in the XNA-compatible build of the 
    /// DigitalRune.Mathematics.dll.
    /// </para>
    /// <para>
    /// In contrast to all preceding coordinate spaces (model space, world space, view space) the 
    /// projection space is left-handed! This is necessary because DirectX uses a left-handed 
    /// clip space.
    /// </para>
    /// <para>
    /// In the projection space the x and y-coordinates range from −1 to 1, and the z-coordinates
    /// range from 0 (near) to 1 (far).
    /// </para>
    /// </remarks>
    /// <exception cref="ArgumentException">
    /// <paramref name="left"/> is equal to <paramref name="right"/>, 
    /// <paramref name="bottom"/> is equal to <paramref name="top"/>, or
    /// <paramref name="zNear"/> is greater than or equal to <paramref name="zFar"/>.
    /// </exception>
    public static Matrix44D CreateOrthographicOffCenter(double left, double right, double bottom, double top, double zNear, double zFar)
    {
      // See DirectX, D3DXMatrixOrthoOffCenterRH().

      if (left == right)
        throw new ArgumentException("The minimum x-value (left) must not be equal to the maximum x-value (right).");
      if (bottom == top)
        throw new ArgumentException("The minimum y-value (bottom) must not be equal to the maximum y-value (top).");
      if (zNear >= zFar)
        throw new ArgumentException("The distance to the near view plane must be less than the distance to the far view plane (zNear < zFar).");

      return new Matrix44D(
        2 / (right - left), 0, 0, (left + right) / (left - right),
        0, 2 / (top - bottom), 0, (top + bottom) / (bottom - top),
        0, 0, 1 / (zNear - zFar), zNear / (zNear - zFar),
        0, 0, 0, 1);
    }


    /// <summary>
    /// Creates a right-handed, perspective projection matrix. (Only available in the XNA-compatible 
    /// build.)
    /// </summary>
    /// <param name="width">The width of the view volume at the near view-plane.</param>
    /// <param name="height">The height of the view volume at the near view-plane.</param>
    /// <param name="zNear">
    /// The minimum z-value of the view volume. (Distance of the near view plane.)
    /// </param>
    /// <param name="zFar">
    /// The maximum z-value of the view volume. (Distance of the far view plane.)
    /// </param>
    /// <returns>The right-handed, perspective projection matrix.</returns>
    /// <remarks>
    /// <para>
    /// This method is available only in the XNA-compatible build of the 
    /// DigitalRune.Mathematics.dll.
    /// </para>
    /// <para>
    /// In contrast to all preceding coordinate spaces (model space, world space, view space) the 
    /// projection space is left-handed! This is necessary because DirectX uses a left-handed 
    /// clip space.
    /// </para>
    /// <para>
    /// In the projection space the x and y-coordinates range from −1 to 1, and the z-coordinates
    /// range from 0 (near) to 1 (far).
    /// </para>
    /// <para>
    /// <strong>Infinite Projections:</strong><br/>
    /// <paramref name="zFar"/> can be set to <see cref="double.PositiveInfinity"/> to create an
    /// <i>infinite projection</i> where the far clip plane is at infinity.
    /// </para>
    /// </remarks>
    /// <exception cref="ArgumentOutOfRangeException">
    /// A parameter is negative or 0.
    /// </exception>
    /// <exception cref="ArgumentException">
    /// <paramref name="zNear"/> is greater than or equal to <paramref name="zFar"/>.
    /// </exception>
    public static Matrix44D CreatePerspective(double width, double height, double zNear, double zFar)
    {
      // See DirectX, D3DXMatrixPerspectiveRH()
      //
      // For optimal precision zFar and zNear should be chosen such that
      //
      //             zFar                     zNear * zFar
      //   M33 = ------------    and    M34 = ------------
      //         zNear - zFar                 zNear - zFar
      //
      // have an exact floating-point representation.
      //
      // The infinite projection matrix Pinf is the same as the projection matrix,
      // except that the far clip plane is at ∞.
      //
      //   Pinf =   limit P
      //          zFar --> ∞
      //
      // The infinite projection compresses z values only slightly more than a finite
      // projection. However, [2] shows that the floating-point precision of an infinite
      // projection is actually better(!) than the precision of a finite projection matrix.
      //
      // References:
      // [1] Cass Everitt, Mark J. Kilgard: Practical and Robust Stenciled Shadow Volumes for Hardware-Accelerated Rendering
      // [2] Paul Upchurch and Mathieu Desbrun: Tightening the Precision of Perspective Rendering

      if (width == 0)
        throw new ArgumentOutOfRangeException("width", "The width must not be 0.");
      if (height == 0)
        throw new ArgumentOutOfRangeException("height", "The height must not be 0.");
      if (zNear <= 0)
        throw new ArgumentOutOfRangeException("zNear", "The distance to the near view plane must not be negative or 0.");
      if (zFar <= 0)
        throw new ArgumentOutOfRangeException("zFar", "The distance to the far view plane must not be negative or 0.");
      if (zNear >= zFar)
        throw new ArgumentException("The distance to the near view plane must be less than the distance to the far view plane (zNear < zFar).");

      if (double.IsPositiveInfinity(zFar))
      {
        // Infinite projection.
        return new Matrix44D(
          2 * zNear / width, 0, 0, 0,
          0, 2 * zNear / height, 0, 0,
          0, 0, -1, -zNear,
          0, 0, -1, 0);
      }

      return new Matrix44D(
        2 * zNear / width, 0, 0, 0,
        0, 2 * zNear / height, 0, 0,
        0, 0, zFar / (zNear - zFar), zNear * zFar / (zNear - zFar),
        0, 0, -1, 0);
    }


    /// <summary>
    /// Creates a right-handed, perspective projection matrix based on a field of view. (Only 
    /// available in the XNA-compatible build.)
    /// </summary>
    /// <param name="fieldOfViewY">The vertical field of view.</param>
    /// <param name="aspectRatio">The aspect ratio (width / height).</param>
    /// <param name="zNear">
    /// The minimum z-value of the view volume. (Distance of the near view plane.)
    /// </param>
    /// <param name="zFar">
    /// The maximum z-value of the view volume. (Distance of the far view plane.)
    /// </param>
    /// <returns>
    /// The right-handed, perspective projection matrix.
    /// </returns>
    /// <remarks>
    /// <para>
    /// This method is available only in the XNA-compatible build of the 
    /// DigitalRune.Mathematics.dll.
    /// </para>
    /// <para>
    /// In contrast to all preceding coordinate spaces (model space, world space, view space) the
    /// projection space is left-handed! This is necessary because DirectX uses a left-handed
    /// clip space.
    /// </para>
    /// <para>
    /// In the projection space the x and y-coordinates range from −1 to 1, and the z-coordinates
    /// range from 0 (near) to 1 (far).
    /// </para>
    /// <para>
    /// <strong>Infinite Projections:</strong><br/>
    /// <paramref name="zFar"/> can be set to <see cref="double.PositiveInfinity"/> to create an
    /// <i>infinite projection</i> where the far clip plane is at infinity.
    /// </para>
    /// </remarks>
    /// <exception cref="ArgumentOutOfRangeException">
    /// <paramref name="fieldOfViewY"/> is not between 0 and π radians (0° and 180°),
    /// <paramref name="aspectRatio"/> is negative or 0, <paramref name="zNear"/> is negative or 0,
    /// or <paramref name="zFar"/> is negative or 0.
    /// </exception>
    /// <exception cref="ArgumentException">
    /// <paramref name="zNear"/> is greater than or equal to <paramref name="zFar"/>.
    /// </exception>
    public static Matrix44D CreatePerspectiveFieldOfView(double fieldOfViewY, double aspectRatio, double zNear, double zFar)
    {
      // See DirectX, D3DXMatrixPerspectiveFovRH().

      if (fieldOfViewY <= 0 || fieldOfViewY >= DoubleClassExtension.Pi)
        throw new ArgumentOutOfRangeException("fieldOfViewY", "The field of view must be between 0 radians and π radians.");
      if (aspectRatio <= 0)
        throw new ArgumentOutOfRangeException("aspectRatio", "The aspect ratio must not be negative or 0.");
      if (zNear <= 0)
        throw new ArgumentOutOfRangeException("zNear", "The distance to the near view plane must not be negative or 0.");
      if (zFar <= 0)
        throw new ArgumentOutOfRangeException("zFar", "The distance to the far view plane must not be negative or 0.");
      if (zNear >= zFar)
        throw new ArgumentException("The distance to the near view plane must be less than the distance to the far view plane (zNear < zFar).");

      double yScale = 1.0 / Math.Tan(0.5 * fieldOfViewY);
      double xScale = yScale / aspectRatio;

      if (double.IsPositiveInfinity(zFar))
      {
        // Infinite projection.
        return new Matrix44D(
          xScale, 0, 0, 0,
          0, yScale, 0, 0,
          0, 0, -1, -zNear,
          0, 0, -1, 0);
      }

      return new Matrix44D(
        xScale, 0, 0, 0,
        0, yScale, 0, 0,
        0, 0, zFar / (zNear - zFar), zNear * zFar / (zNear - zFar),
        0, 0, -1, 0);
    }


    /// <summary>
    /// Creates a customized, right-handed, perspective projection matrix. (Only available in the 
    /// XNA-compatible build.)
    /// </summary>
    /// <param name="left">The minimum x-value of the view volume at the near view plane.</param>
    /// <param name="right">The maximum x-value of the view volume at the near view plane.</param>
    /// <param name="bottom">The minimum y-value of the view volume at the near view plane.</param>
    /// <param name="top">The maximum y-value of the view volume at the near view plane.</param>
    /// <param name="zNear">
    /// The minimum z-value of the view volume. (Distance of the near view plane.)
    /// </param>
    /// <param name="zFar">
    /// The maximum z-value of the view volume. (Distance of the far view plane.)
    /// </param>
    /// <returns>
    /// The customized, right-handed, perspective projection matrix.
    /// </returns>
    /// <remarks>
    /// <para>
    /// This method is available only in the XNA-compatible build of the 
    /// DigitalRune.Mathematics.dll.
    /// </para>
    /// <para>
    /// In contrast to all preceding coordinate spaces (model space, world space, view space) the
    /// projection space is left-handed! This is necessary because DirectX uses a left-handed
    /// clip space.
    /// </para>
    /// <para>
    /// In the projection space the x and y-coordinates range from −1 to 1, and the z-coordinates
    /// range from 0 (near) to 1 (far).
    /// </para>
    /// <para>
    /// <strong>Infinite Projections:</strong><br/>
    /// <paramref name="zFar"/> can be set to <see cref="double.PositiveInfinity"/> to create an
    /// <i>infinite projection</i> where the far clip plane is at infinity.
    /// </para>
    /// </remarks>
    /// <exception cref="ArgumentOutOfRangeException">
    /// <paramref name="zNear"/> or <paramref name="zFar"/> is negative or 0.
    /// </exception>
    /// <exception cref="ArgumentException">
    /// <paramref name="left"/> is equal to <paramref name="right"/>,
    /// <paramref name="bottom"/> is equal to <paramref name="top"/>, or 
    /// <paramref name="zNear"/> is greater than or equal to <paramref name="zFar"/>.
    /// </exception>
    public static Matrix44D CreatePerspectiveOffCenter(double left, double right, double bottom, double top, double zNear, double zFar)
    {
      // See DirectX, D3DXMatrixPerspectiveOffCenterRH().

      if (left == right)
        throw new ArgumentException("The minimum x-value (left) must not be equal to the maximum x-value (right).");
      if (bottom == top)
        throw new ArgumentException("The minimum y-value (bottom) must not be equal to the maximum y-value (top).");
      if (zNear <= 0)
        throw new ArgumentOutOfRangeException("zNear", "The distance to the near view plane must not be negative or 0.");
      if (zFar <= 0)
        throw new ArgumentOutOfRangeException("zFar", "The distance to the far view plane must not be negative or 0.");
      if (zNear >= zFar)
        throw new ArgumentException("The distance to the near view plane must be less than the distance to the far view plane (zNear < zFar).");

      if (double.IsPositiveInfinity(zFar))
      {
        // Infinite projection.
        return new Matrix44D(
          2 * zNear / (right - left), 0, (left + right) / (right - left), 0,
          0, 2 * zNear / (top - bottom), (top + bottom) / (top - bottom), 0,
          0, 0, -1, -zNear,
          0, 0, -1, 0);
      }

      return new Matrix44D(
        2 * zNear / (right - left), 0, (left + right) / (right - left), 0,
        0, 2 * zNear / (top - bottom), (top + bottom) / (top - bottom), 0,
        0, 0, zFar / (zNear - zFar), zNear * zFar / (zNear - zFar),
        0, 0, -1, 0);
    }

        #endregion
    }
}
