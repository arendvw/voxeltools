namespace StudioAvw.Voxels.Geometry.Helper
{
    class MatrixHelper
    {
        /// <summary>
        /// each position in the vector has a maximum value comparable with binary or hexadecimal values and are multiples of values that are before that
        /// [3,4,6] => [72,24,6] and is a multiple of [24,6,1]
        /// thus the calculation is for item 55 in this grid:
        /// 55 % 6  = 1 / 1   = 1
        /// 54 % 24 = 6 / 6   = 1
        /// 48 % 72 = 48 / 24 = 2
        /// resulting vector => [2,1,1]
        /// </summary>
        /// <param name="item">The item in the range of numbers to be evaluated</param>
        /// <param name="sizeVector">The size of the grid</param>
        /// <returns></returns>
        public static int[] MatrixVector(int item, int[] sizeVector)
        {
            // the item number of the vector
            var i = item;
            // create an empty output array
            var output = new int[sizeVector.Length];
            var rest = i;
            var kCount = 1;
            // each position in the vector has a maximum value comparable with binary or hexadecimal values and are multiples of values that are before that
            // [3,4,6] => [72,24,6] and is a multiple of [24,6,1]
            // thus the calculation is for item 55 in this grid:
            // 55 % 6  = 1 / 1   = 1
            // 54 % 24 = 6 / 6   = 1
            // 48 % 72 = 48 / 24 = 2
            // resulting vector => [2,1,1]
            for (var k = sizeVector.Length - 1; k >= 0; k--)
            {
                output[k] = (rest % (kCount * sizeVector[k])) / kCount;
                kCount *= sizeVector[k];
                rest = rest - (rest % kCount);
            }
            return output;
        }


        /// <summary>
        /// A tool to enumerate any n-dimensional matrix space.
        /// Get the position of a vector v in matrix i;
        /// </summary>
        /// <param name="vector">A vector (position) in a discrete n-dimensional space</param>
        /// <param name="matrix">The size of the matrix in </param>
        /// <returns>Position for the vector in the enumeration of the matrix</returns>
        public static int VectorMatrix(int[] vector, int[] matrix)
        {
            var result = 0;
            for (var i = 0; i < vector.Length; i++)
            {
                var higherDimensionSize = 1;
                // for x we need the size of the x*y dimension
                // i=0
                // matrix[2]*matrix[1]
                // for y we need the z dimension
                // i=1 matrix[2]
                // i=0
                // for z we don't need a dimension size.
                for (var j = i + 1; j < vector.Length; j++)
                {
                    higherDimensionSize *= matrix[j];
                }
                result += higherDimensionSize * vector[i];
            }
            return result;
        }

        /// <summary>
        /// Delegate based on vector input
        /// </summary>
        /// <param name="count"></param>
        /// <param name="row"></param>
        public delegate void DelegateVector(int count, int[] row);

        /// <summary>
        /// Delegate based on x,y,z matrix size definitions
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="z"></param>

        public delegate void DelegateXYZ(int x, int y, int z);
        /// <summary>
        /// Delegated based on Point3i matrix size definition
        /// </summary>
        /// <param name="count"></param>
        /// <param name="point"></param>
        public delegate void DelegatePoint3i(int count, Point3i point);

        /// <summary>
        /// Delegated based on Point3i matrix size definition (Multithreaded, experimental)
        /// </summary>
        /// <param name="count"></param>
        /// <param name="point"></param>
        /// <param name="locker"></param>
        /// 
        public delegate void DelegatePoint3iMT(int count, Point3i point, object locker);
        /// <summary>
        /// Iterate over a matrix (square sized in n dimensions with the dimensions given in int[])
        /// A singlethreaded approach is used
        /// </summary>
        /// <param name="sizeVector">The size of the matrix to be evaluated (the search space)</param>
        /// <param name="callback">A delegate which will handle each vector</param>
        public static void MapMatrix(int[] sizeVector, DelegateVector callback)
        {
            var count = 1;
            for (var i = 0; i < sizeVector.Length; i++)
            {
                count *= sizeVector[i];
            }

            for (var j = 0; j < count; j++)
            {
                callback(j, MatrixVector(j, sizeVector));
            }
        }

        /// <summary>
        /// Iterate over a matrix (square sized in n dimensions with the dimensions given in int[])
        /// A singlethreaded approach is used
        /// </summary>
        /// <param name="sizeVector">The size of the matrix to be evaluated (the search space)</param>
        /// <param name="delegateVector">A delegate which will handle each vector</param>
        public static void MapMatrix(Point3i sizeVector, DelegateVector delegateVector)
        {
            MapMatrix(sizeVector.ToArray(), delegateVector);
        }

        /// <summary>
        /// Call delegate for each item in a n-dimensional matrix (grid)
        /// </summary>
        /// <param name="sizeVector">Size of the n-dimensional matrix</param>
        /// <param name="callback">Callback (delegate) to call for each item</param>
        public static void MapMatrix(Point3i sizeVector, DelegateXYZ callback)
        {
            MapMatrix(sizeVector.ToArray(), delegate(int i, int[] vector) { callback(vector[0], vector[1], vector[2]); });
        }

        /// <summary>
        /// Map matrix over a set of point3i's
        /// </summary>
        /// <param name="sizeVector"></param>
        /// <param name="callback"></param>
        public static void MapMatrixMT(Point3i sizeVector, DelegatePoint3i callback)
        {
            MapMatrixMT(
                sizeVector.ToArray(),
                delegate(int i, int[] vector)
                {
                    callback(i, new Point3i(vector));
                }
            );
        }

        /// <summary>
        /// Map matrix over a set of point3i's
        /// </summary>
        /// <param name="sizeVector"></param>
        /// <param name="callback"></param>
        public static void MapMatrix(Point3i sizeVector, DelegatePoint3i callback)
        {
            MapMatrix(
                sizeVector.ToArray(),
                delegate(int i, int[] vector)
                {
                    callback(i, new Point3i(vector));
                }
            );
        }

        /// <summary>
        /// Iterate over a matrix (square sized in n dimensions with the dimensions given in int[])
        /// A multithreaded approach is used
        /// </summary>
        /// <param name="sizeVector">The size of the matrix to be evaluated (the search space)</param>
        /// <param name="delegateVector">A delegate which will handle each vector</param>
        public static void MapMatrixMT(int[] sizeVector, DelegateVector delegateVector)
        {
            var count = 1;
            for (var i = 0; i < sizeVector.Length; i++)
            {
                count *= sizeVector[i];
            }

        }

        /// <summary>
        /// Map matrix multithreaded. Call the delegate callback for each item in the matrix.
        /// </summary>
        /// <param name="sizeVector">Size of the matrix</param>
        /// <param name="delegateVector">Delegate to execute for each vector</param>
        public static void MapMatrixMT(Point3i sizeVector, DelegateVector delegateVector)
        {
            MapMatrixMT(sizeVector.ToArray(), delegateVector);
        }

        /// <summary>
        /// Map matrix multithreaded. Call the delegate callback for each item in the matrix.
        /// Currently obsolete
        /// </summary>
        /// <param name="sizeVector"></param>
        /// <param name="callback"></param>
        public static void MapMatrixMT(Point3i sizeVector, DelegateXYZ callback)
        {
            MapMatrix(sizeVector.ToArray(), delegate(int i, int[] vector) { callback(vector[0], vector[1], vector[2]); });
        }
    }
}
