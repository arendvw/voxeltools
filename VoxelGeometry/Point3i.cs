using System;
using Rhino.Geometry;
using StudioAvw.Voxels.Geometry.Helper;

namespace StudioAvw.Voxels.Geometry
{
    /// <summary>
    /// Int16 lightweight 3d dimensional point struct with an allowable range between -32767 and 32767. 
    /// Takes in a total of 3*2 bytes = 12 bytes. Point3d takes 8*3 bytes = 24 byte.
    /// </summary>
    
    [Serializable]
    public struct Point3i
    {
		#region Data Members (4) 

        /// <summary>
        /// X Value of point
        /// </summary>
        public short X { get; set; }
        /// <summary>
        /// Y value of point
        /// </summary>
        public short Y { get; set; }
        /// <summary>
        /// Z value of point
        /// </summary>
        public short Z { get; set; }

        public static Point3i Origin { get; } = new Point3i(0, 0, 0);

        /// <summary>
        /// Value of dimension iDim
        /// </summary>
        /// <param name="iDim"></param>
        /// <returns></returns>
        public int this[int iDim]
        {
            get
            {
                switch (iDim)
                {
                    case 0:
                        return X;
                    case 1:
                        return Y;
                    case 2:
                        return Z;
                    default:
                        throw new IndexOutOfRangeException("Point3i has a maximum size of 3");
                }
            }
            set
            {
                switch (iDim)
                {
                    case 0:
                        X = (short) value;
                        break;
                    case 1:
                        Y = (short) value;
                        break;
                    case 2:
                        Z = (short) value;
                        break;
                    default:
                        throw new IndexOutOfRangeException("Point3i has a maximum size of 3");
                }
            }
        }

		#endregion Data Members 

		#region Methods (16) 

        /// <summary>
        /// Subtract operator
        /// </summary>
        /// <param name="p1">Point to subtract from</param>
        /// <param name="p2">Point to subtract with</param>
        /// <returns>Difference between pt1 and pt2 in integers</returns>
        public static Point3i operator -(Point3i p1, Point3i p2)
        {
            return new Point3i(p1.X - p2.X, p1.Y - p2.Y, p1.Z - p2.Z);
        }

        /// <summary>
        /// Add two points
        /// </summary>
        /// <param name="p1"></param>
        /// <param name="p2"></param>
        /// <returns></returns>
        public static Point3i operator +(Point3i p1, Point3i p2)
        {
            return new Point3i(p1.X + p2.X, p1.Y + p2.Y, p1.Z + p2.Z);
        }

        /// <summary>
        /// Is the point3i equal to point p1. Zero tolerance for differences.
        /// </summary>
        /// <param name="p">Point3i to compare with</param>
        /// <returns></returns>
        public bool Equals(Point3i p)
        {
            return X == p.X && Y == p.Y && Z == p.Z;
        }

        /// <summary>
        /// Are x,y,z all bigger or equal to 0?
        /// </summary>
        /// <returns></returns>
        internal bool IsPositive()
        {
            return X >= 0 && Y >= 0 && Z >= 0;
        }

        /// <summary>
        /// Are all dimensions larger than 0?
        /// </summary>
        /// <returns></returns>
        internal bool IsPositiveNonZero()
        {
            return X > 0 && Y > 0 && Z > 0;
        }

        /// <summary>
        /// Create a Point3i from Point3d by rounding to integer values
        /// </summary>1
        /// <param name="p"></param>
        public Point3i(Point3d p)
        {
            X = (short)Math.Round(p.X);
            Y = (short)Math.Round(p.Y);
            Z = (short)Math.Round(p.Z);
        }

        /// <summary>
        /// Construct a point3i from a integer vector
        /// </summary>
        /// <param name="vector"></param>
        public Point3i(int[] vector)
        {
            if (vector.Length != 3)
            {
                throw new Exception("Vector size should be 3 elements");
            }
            X = (short)vector[0];
            Y = (short)vector[1];
            Z = (short)vector[2];
        }

        /// <summary>
        /// Create a new point3i
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="z"></param>
        public Point3i(short x, short y, short z)
        {
            X = x; Y = y; Z = z;
        }

        /// <summary>
        /// Create a new point3i from any x y z values
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="z"></param>
        public Point3i(object x, object y, object z)
        {
            // TODO: Complete member initialization
            X = Convert.ToInt16(x);
            Y = Convert.ToInt16(y);
            Z = Convert.ToInt16(z);
        }

        /// <summary>
        /// Calculate X*Y*Z
        /// </summary>
        /// <returns></returns>
        public int SelfProduct()
        {
            return X * Y * Z;
        }

        /// <summary>
        /// Convert to an array int[3]
        /// </summary>
        /// <returns></returns>
        public int[] ToArray()
        {
            return new int[] { X, Y, Z };
        }

        /// <summary>
        /// Convert to point3d
        /// </summary>
        /// <returns></returns>
        [Obsolete]
        public int ToInt()
        {
            return X * Y * Z;
        }

        /// <summary>
        /// Convert to point3d
        /// </summary>
        /// <returns></returns>
        public Point3d ToPoint3D()
        {
            return new Point3d(X, Y, Z);
        }

        /// <summary>
        /// Convert to Point3f
        /// </summary>
        /// <returns></returns>
        // ReSharper disable once InconsistentNaming
        public Point3f ToPoint3f()
        {
            return new Point3f(X, Y, Z);
        }

        /// <summary>
        /// Convert to string
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return $"[{X},{Y},{Z}]";
        }

        /// <summary>
        /// Convert point to a vector
        /// </summary>
        /// <returns></returns>
        public Vector3d ToVector3D()
        {
            return new Vector3d(X, Y, Z);
        }

		#endregion Methods 

        #region Operators
        /// <summary>
        /// Multiply two points
        /// </summary>
        /// <param name="p1"></param>
        /// <param name="f"></param>
        /// <returns></returns>
        public static Point3d operator *(Point3i p1, double f)
        {
            return new Point3d(p1.X *f, p1.Y * f, p1.Z * f);
        }

        /// <summary>
        /// Multiply points with factor i
        /// </summary>
        /// <param name="p1"></param>
        /// <param name="i"></param>
        /// <returns></returns>
        public static Point3i operator *(Point3i p1, int i)
        {
            return new Point3i(p1.X * i, p1.Y * i, p1.Z * i);
        }


        /// <summary>
        /// Assume Point3i is a vector describing the size of a 3-dimensional matrix with size x,y,z
        /// and Pt is a vector describing a x,y,z location in this 3 dimensional nmatrix.  
        /// All points in this matrix can be enumerated to one list of integer numbers (0... size.x*size.y*.size.z)
        /// This method calculates at which position in this list point Pt is.
        /// 
        /// See also Tools.VectorMatrix method for an n-dimensional implementation
        /// </summary>
        /// <param name="size">Size of matrix in x,y,z</param>
        /// <param name="pt">Point in this matrix</param>
        /// <returns></returns>
        [Obsolete("This method has been renamed to Point3i.PointUvwToIndex")]
        public static int operator ^(Point3i size, Point3i pt)
        {
            return size.Z * size.Y * pt.X + size.Z * pt.Y + pt.Z;
        }

        /// <summary>
        /// Each cell in this grid has a number, the highest number is calculated by cellCount.x*cellCount.y*.cellCount.z)
        /// This method calculates at which number belongs to pointUvw
        /// 
        /// This is a 3 dimensions calculation. See also Tools.VectorMatrix method for an n-dimensional implementation
        /// </summary>
        /// <param name="cellCount">Describing the size of a 3-dimensional matrix with cell count x,y,z</param>
        /// <param name="pointUvw">vector describing a u, v, w location in this 3 dimensional grid.  </param>
        /// <returns></returns>
        public static int PointUvwToIndex(Point3i cellCount, Point3i pointUvw)
        {
            return cellCount.Z * cellCount.Y * pointUvw.X + cellCount.Z * pointUvw.Y + pointUvw.Z;
        }

        /// <summary>
        /// Opposite of ^ operator. Assumes pt to be a non-zero vector describing the size of a 3-dimensional matrix
        /// And integer item to be a number between 0 and pt.x*pt.y*pt.z. Calculate a x,y,z integer location for this number
        /// </summary>
        /// <param name="pt"></param>
        /// <param name="item"></param>
        /// <returns></returns>
        [Obsolete("This method has been renamed to Point3i.IndexToPointUvw")]
        public static Point3i operator % (Point3i pt, int item)
        {
            return IndexToPointUvw(pt, item);
        }

        public static Point3i IndexToPointUvw(Point3i gridDimensions, int index)
        {
            return new Point3i(MatrixHelper.MatrixVector(index, gridDimensions.ToArray()));
        }

        /// <summary>
        /// Is any of the values of p1 larger than any of the values of p2?
        /// </summary>
        /// <param name="p1"></param>
        /// <param name="p2"></param>
        /// <returns></returns>
        public static bool operator >(Point3i p1, Point3i p2)
        {
            return p1.X > p2.X || p1.Y > p2.Y || p1.Z > p2.Z;
        }

        /// <summary>
        /// Are any of the values of p1 larger or equal to any of the values of p2?
        /// </summary>
        /// <param name="p1"></param>
        /// <param name="p2"></param>
        /// <returns></returns>
        public static bool operator >=(Point3i p1, Point3i p2)
        {
            return p1.X >= p2.X || p1.Y >= p2.Y || p1.Z >= p2.Z;
        }

        /// <summary>
        /// Are any of the values of p1 smaller than any of the values of p2?
        /// </summary>
        /// <param name="p1"></param>
        /// <param name="p2"></param>
        /// <returns></returns>
        public static bool operator <(Point3i p1, Point3i p2)
        {
            return p1.X < p2.X || p1.Y < p2.Y || p1.Z < p2.Z;
        }

        /// <summary>
        /// Are any of the values of p1 smaller or equal to any of the values of p2?
        /// </summary>
        /// <param name="p1"></param>
        /// <param name="p2"></param>
        /// <returns></returns>
        public static bool operator <=(Point3i p1, Point3i p2)
        {
            return p1.X <= p2.X || p1.Y <= p2.Y || p1.Z <= p2.Z;
        }
        #endregion operators
    }
}
