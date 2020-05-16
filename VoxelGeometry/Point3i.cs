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
        public short x;
        /// <summary>
        /// Y value of point
        /// </summary>
        public short y;
        /// <summary>
        /// Z value of point
        /// </summary>
        public short z;
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
                        return x;
                    case 1:
                        return y;
                    case 2:
                        return z;
                    default:
                        throw new IndexOutOfRangeException("Point3i has a maximum size of 3");
                }
            }
            set
            {
                switch (iDim)
                {
                    case 0:
                        x = (short) value;
                        break;
                    case 1:
                        y = (short) value;
                        break;
                    case 2:
                        z = (short) value;
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
            return new Point3i(p1.x - p2.x, p1.y - p2.y, p1.z - p2.z);
        }

        /// <summary>
        /// Add two points
        /// </summary>
        /// <param name="p1"></param>
        /// <param name="p2"></param>
        /// <returns></returns>
        public static Point3i operator +(Point3i p1, Point3i p2)
        {
            return new Point3i(p1.x + p2.x, p1.y + p2.y, p1.z + p2.z);
        }

        /// <summary>
        /// Is the point3i equal to point p1. Zero tolerance for differences.
        /// </summary>
        /// <param name="p">Point3i to compare with</param>
        /// <returns></returns>
        public bool Equals(Point3i p)
        {
            return x == p.x && y == p.y && z == p.z;
        }

        /// <summary>
        /// Are x,y,z all bigger or equal to 0?
        /// </summary>
        /// <returns></returns>
        internal bool IsPositive()
        {
            return x >= 0 && y >= 0 && z >= 0;
        }

        /// <summary>
        /// Are all dimensions larger than 0?
        /// </summary>
        /// <returns></returns>
        internal bool IsPositiveNonZero()
        {
            return x > 0 && y > 0 && z > 0;
        }

        /// <summary>
        /// Create a Point3i from Point3d by rounding to integer values
        /// </summary>1
        /// <param name="p"></param>
        public Point3i(Point3d p)
        {
            x = (short)Math.Round(p.X);
            y = (short)Math.Round(p.Y);
            z = (short)Math.Round(p.Z);
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
            x = (short)vector[0];
            y = (short)vector[1];
            z = (short)vector[2];
        }

        /// <summary>
        /// Create a new point3i
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="z"></param>
        public Point3i(short x, short y, short z)
        {
            this.x = x; this.y = y; this.z = z;
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
            this.x = Convert.ToInt16(x);
            this.y = Convert.ToInt16(y);
            this.z = Convert.ToInt16(z);
        }

        /// <summary>
        /// Calculate X*Y*Z
        /// </summary>
        /// <returns></returns>
        public int SelfProduct()
        {
            return x * y * z;
        }

        /// <summary>
        /// Convert to an array int[3]
        /// </summary>
        /// <returns></returns>
        public int[] ToArray()
        {
            return new int[3] { x, y, z };
        }

        /// <summary>
        /// Convert to point3d
        /// </summary>
        /// <returns></returns>
        [Obsolete]
        public int ToInt()
        {
            return x * y * z;
        }

        /// <summary>
        /// Convert to point3d
        /// </summary>
        /// <returns></returns>
        public Point3d ToPoint3D()
        {
            return new Point3d(x, y, z);
        }

        /// <summary>
        /// Convert to Point3f
        /// </summary>
        /// <returns></returns>
        public Point3f ToPoint3f()
        {
            return new Point3f(x, y, z);
        }

        /// <summary>
        /// Convert to string
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {

            return string.Format("{{0},{1},{2}}", x, y, z);
        }

        /// <summary>
        /// Convert point to a vector
        /// </summary>
        /// <returns></returns>
        public Vector3d ToVector3D()
        {
            return new Vector3d(x, y, z);
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
            return new Point3d(p1.x *f, p1.y * f, p1.z * f);
        }

        /// <summary>
        /// Multiply points with factor i
        /// </summary>
        /// <param name="p1"></param>
        /// <param name="i"></param>
        /// <returns></returns>
        public static Point3i operator *(Point3i p1, int i)
        {
            return new Point3i(p1.x * i, p1.y * i, p1.z * i);
        }


        /// <summary>
        /// Calculate the 
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
        public static int operator ^(Point3i size, Point3i pt)
        {
            
            return size.z * size.y * pt.x + size.z * pt.y + pt.z;
        }

        /// <summary>
        /// Opposite of ^ operator. Assumes pt to be a non-zero vector describing the size of a 3-dimensional matrix
        /// And integer item to be a number between 0 and pt.x*pt.y*pt.z. Calculate a x,y,z integer location for this number
        /// </summary>
        /// <param name="pt"></param>
        /// <param name="item"></param>
        /// <returns></returns>
        public static Point3i operator % (Point3i pt, int item)
        {
            return new Point3i(MatrixHelper.MatrixVector(item, pt.ToArray()));
        }

        /// <summary>
        /// Is any of the values of p1 larger than any of the values of p2?
        /// </summary>
        /// <param name="p1"></param>
        /// <param name="p2"></param>
        /// <returns></returns>
        public static bool operator >(Point3i p1, Point3i p2)
        {
            return p1.x > p2.x || p1.y > p2.y || p1.z > p2.z;
        }

        /// <summary>
        /// Are any of the values of p1 larger or equal to any of the values of p2?
        /// </summary>
        /// <param name="p1"></param>
        /// <param name="p2"></param>
        /// <returns></returns>
        public static bool operator >=(Point3i p1, Point3i p2)
        {
            return p1.x >= p2.x || p1.y >= p2.y || p1.z >= p2.z;
        }

        /// <summary>
        /// Are any of the values of p1 smaller than any of the values of p2?
        /// </summary>
        /// <param name="p1"></param>
        /// <param name="p2"></param>
        /// <returns></returns>
        public static bool operator <(Point3i p1, Point3i p2)
        {
            return p1.x < p2.x || p1.y < p2.y || p1.z < p2.z;
        }

        /// <summary>
        /// Are any of the values of p1 smaller or equal to any of the values of p2?
        /// </summary>
        /// <param name="p1"></param>
        /// <param name="p2"></param>
        /// <returns></returns>
        public static bool operator <=(Point3i p1, Point3i p2)
        {
            return p1.x <= p2.x || p1.y <= p2.y || p1.z <= p2.z;
        }
        #endregion operators
    }
}
