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
    public struct Point2i
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
                    default:
                        throw new IndexOutOfRangeException("Point2i has a maximum size of 2");
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
                    default:
                        throw new IndexOutOfRangeException("Point2i has a maximum size of 2");
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
        public static Point2i operator -(Point2i p1, Point2i p2)
        {
            return new Point2i(p1.X - p2.X, p1.Y - p2.Y);
        }

        /// <summary>
        /// Add two points
        /// </summary>
        /// <param name="p1"></param>
        /// <param name="p2"></param>
        /// <returns></returns>
        public static Point2i operator +(Point2i p1, Point2i p2)
        {
            return new Point2i(p1.X + p2.X, p1.Y + p2.Y);
        }

        /// <summary>
        /// Is the point2i equal to point p1. Zero tolerance for differences.
        /// </summary>
        /// <param name="p">Point2i to compare with</param>
        /// <returns></returns>
        public bool Equals(Point2i p)
        {
            return X == p.X && Y == p.Y;
        }

        /// <summary>
        /// Are x,y all bigger or equal to 0?
        /// </summary>
        /// <returns></returns>
        internal bool IsPositive()
        {
            return X >= 0 && Y >= 0;
        }

        /// <summary>
        /// Are all dimensions larger than 0?
        /// </summary>
        /// <returns></returns>
        internal bool IsPositiveNonZero()
        {
            return X > 0 && Y > 0;
        }

        /// <summary>
        /// Create a Point2i from Point2d by rounding to integer values
        /// </summary>1
        /// <param name="p"></param>
        public Point2i(Point2d p)
        {
            X = (short)Math.Round(p.X);
            Y = (short)Math.Round(p.Y);
        }

        /// <summary>
        /// Construct a point2i from a integer vector
        /// </summary>
        /// <param name="vector"></param>
        public Point2i(int[] vector)
        {
            if (vector.Length != 2)
            {
                throw new Exception("Vector size should be 2 elements");
            }
            X = (short)vector[0];
            Y = (short)vector[1];
        }

        /// <summary>
        /// Create a new point2i
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public Point2i(short x, short y)
        {
            this.X = x; this.Y = y;
        }

        /// <summary>
        /// Create a new point2i from any x y values
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public Point2i(object x, object y)
        {
            this.X = Convert.ToInt16(x);
            this.Y = Convert.ToInt16(y);
        }

        /// <summary>
        /// Calculate X*Y*Z
        /// </summary>
        /// <returns></returns>
        public int SelfProduct()
        {
            return X * Y;
        }

        /// <summary>
        /// Convert to an array int[3]
        /// </summary>
        /// <returns></returns>
        public int[] ToArray()
        {
            return new int[2] { X, Y };
        }

        /// <summary>
        /// Convert to point3d
        /// </summary>
        /// <returns></returns>
        public int ToInt()
        {
            return X * Y;
        }

        /// <summary>
        /// Convert to point3d
        /// </summary>
        /// <returns></returns>
        public Point3d ToPoint3d()
        {
            return new Point3d(X, Y, 0);
        }

        /// <summary>
        /// Convert to Point3f
        /// </summary>
        /// <returns></returns>
        public Point3f ToPoint3f()
        {
            return new Point3f(X, Y, 0);
        }

        /// <summary>
        /// Convert to string
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {

            return $"{{{X},{Y}}}";
        }

        /// <summary>
        /// Convert point to a vector
        /// </summary>
        /// <returns></returns>
        public Vector3d ToVector3d()
        {
            return new Vector3d(X, Y, 0);
        }

		#endregion Methods 

        #region Operators
        /// <summary>
        /// Multiply two points
        /// </summary>
        /// <param name="p1"></param>
        /// <param name="f"></param>
        /// <returns></returns>
        public static Point2d operator *(Point2i p1, double f)
        {
            return new Point2d(p1.X *f, p1.Y * f);
        }

        /// <summary>
        /// Multiply points with factor i
        /// </summary>
        /// <param name="p1"></param>
        /// <param name="i"></param>
        /// <returns></returns>
        public static Point2i operator *(Point2i p1, int i)
        {
            return new Point2i(p1.X * i, p1.Y * i);
        }


        /// <summary>
        /// Calculate the 
        /// Assume Point2i is a vector describing the size of a 3-dimensional matrix with size x,y,z
        /// and Pt is a vector describing a x,y,z location in this 3 dimensional nmatrix.  
        /// All points in this matrix can be enumerated to one list of integer numbers (0... size.x*size.y*.size.z)
        /// This method calculates at which position in this list point Pt is.
        /// 
        /// See also Tools.VectorMatrix method for an n-dimensional implementation
        /// </summary>
        /// <param name="size">Size of matrix in x,y,z</param>
        /// <param name="pt">Point in this matrix</param>
        /// <returns></returns>
        public static int operator ^(Point2i size, Point2i pt)
        {
            
            return (int)(pt.Y * size.X + size.Y);
        }

        /// <summary>
        /// Opposite of ^ operator. Assumes pt to be a non-zero vector describing the size of a 3-dimensional matrix
        /// And integer item to be a number between 0 and pt.x*pt.y*pt.z. Calculate a x,y,z integer location for this number
        /// </summary>
        /// <param name="pt"></param>
        /// <param name="item"></param>
        /// <returns></returns>
        public static Point2i operator % (Point2i pt, int item)
        {
            return new Point2i(MatrixHelper.MatrixVector(item, pt.ToArray()));
        }

        /// <summary>
        /// Is any of the values of p1 larger than any of the values of p2?
        /// </summary>
        /// <param name="p1"></param>
        /// <param name="p2"></param>
        /// <returns></returns>
        public static bool operator >(Point2i p1, Point2i p2)
        {
            return p1.X > p2.X || p1.Y > p2.Y;
        }

        /// <summary>
        /// Are any of the values of p1 larger or equal to any of the values of p2?
        /// </summary>
        /// <param name="p1"></param>
        /// <param name="p2"></param>
        /// <returns></returns>
        public static bool operator >=(Point2i p1, Point2i p2)
        {
            return p1.X >= p2.X || p1.Y >= p2.Y;
        }

        /// <summary>
        /// Are any of the values of p1 smaller than any of the values of p2?
        /// </summary>
        /// <param name="p1"></param>
        /// <param name="p2"></param>
        /// <returns></returns>
        public static bool operator <(Point2i p1, Point2i p2)
        {
            return p1.X < p2.X || p1.Y < p2.Y;
        }

        /// <summary>
        /// Are any of the values of p1 smaller or equal to any of the values of p2?
        /// </summary>
        /// <param name="p1"></param>
        /// <param name="p2"></param>
        /// <returns></returns>
        public static bool operator <=(Point2i p1, Point2i p2)
        {
            return p1.X <= p2.X || p1.Y <= p2.Y;
        }
        #endregion operators
    }
}
