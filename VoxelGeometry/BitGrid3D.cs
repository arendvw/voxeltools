using System;
using System.Collections;

namespace StudioAvw.Voxels.Geometry
{
    /// <summary>
    /// A bit grid is a simple x,y,z grid containing boolean values
    /// </summary>
    public class BitGrid3D : Grid3D<bool, BitArray>
    {
		#region Constructors (1) 

        /// <summary>
        /// Create empty bit grid
        /// </summary>
        public BitGrid3D() { }

		#endregion Constructors 

		#region Properties (1) 

        /// <summary>
        /// Count the amount of true values in the grid
        /// </summary>
        public int CountNonZero
        {
            get
            {
                var count = 0;
                if (Grid == null)
                {
                    return 0;
                }
                for (var i = 0; i < Grid.Count; i++)
                {
                    if (GetValue(i))
                    {
                        count++;
                    }
                }
                return count;
            }
        }

		#endregion Properties 

		#region Methods (12) 

		// Public Methods (12) 

        /// <summary>
        /// Set the values to the result of an AND operation with grid bt
        /// </summary>
        /// <param name="bt"></param>
        public void And(BitGrid3D bt)
        {
            if (bt.Count != Count)
            {
                throw new Exception("Grids are of different sizes");
            }

            for (var i = 0; i < Count; i++)
            {
                SetValue(i, GetValue(i) && bt.GetValue(i));
            }
        }

        /// <summary>
        /// Clone the object
        /// </summary>
        /// <returns></returns>
        public override object Clone()
        {
            return MemberwiseClone();
        }

        /// <summary>
        /// Get the enumerator of the grid
        /// </summary>
        /// <returns></returns>
        public override IEnumerator GetEnumerator()
        {
            return Grid.GetEnumerator();
        }

        /// <summary>
        /// Get the value of a relativePosition voxel, defined by a Point3i size.
        /// </summary>
        /// <param name="voxelIndex"></param>
        /// <param name="relativePosition"></param>
        /// <returns>
        /// returns 0 if empty
        /// returns -1 if non existant
        /// returns 1 if true
        /// </returns>
        /// 
        public sbyte GetRelativePointValue(int voxelIndex, Point3i relativePosition)
        {
            var pt = (Point3i.IndexToPointUvw(SizeUVW, voxelIndex)) + relativePosition;
            if (new Point3i(0, 0, 0) > pt || pt >= SizeUVW)
            {
                return -1;
            }
            return GetValue(pt) ? (sbyte)1 : (sbyte)0;
        }

        /// <summary>
        /// Get value of point voxelIndex
        /// </summary>
        /// <param name="voxelIndex"></param>
        /// <returns></returns>
        public override bool GetValue(int voxelIndex)
        {
            return Grid.Get(voxelIndex);
        }

        /// <summary>
        /// Inverse the grid
        /// </summary>
        public void Not()
        {
            for (var i = 0; i < Count; i++)
            {
                SetValue(i, (!GetValue(i)));
            }
        }

        /// <summary>
        /// Set values to the result of an OR operation with grid bt
        /// </summary>
        /// <param name="bt"></param>
        public void Or(BitGrid3D bt)
        {
            if (bt.Count != Count)
            {
                throw new Exception("Grids are of different sizes");
            }

            for (var idx = 0; idx < Count; idx++)
            {
                SetValue(idx, GetValue(idx) || bt.GetValue(idx));
            }
        }

        /// <summary>
        /// Set a relativePosition point value to value X
        /// </summary>
        /// <param name="voxelIndex"></param>
        /// <param name="relativePosition"></param>
        /// <param name="value"></param>
        public void SetRelativePointValue(int voxelIndex, Point3i relativePosition, bool value)
        {
            var pt = Point3i.IndexToPointUvw(SizeUVW, voxelIndex) + relativePosition;
            if (new Point3i(0, 0, 0) > pt || pt >= SizeUVW)
            {
                return;
            }

            this[pt] = value;
        }

        /// <summary>
        /// Set point to value bool
        /// </summary>
        /// <param name="voxelIndex">voxel index</param>
        /// <param name="value"></param>
        public override void SetValue(int voxelIndex, bool value)
        {
            if (voxelIndex >= Grid.Count || voxelIndex < 0)
            {
                return;
            }
            Grid.Set(voxelIndex, value);
        }

        /// <summary>
        /// Set the result to be the result of a subtraction with grid bt
        /// </summary>
        /// <param name="bt"></param>
        public void Subtract(BitGrid3D bt)
        {
            if (bt.Count != Count)
            {
                throw new Exception("Grids are of different sizes");
            }

            for (var i = 0; i < Count; i++)
            {
                if (this[i] && bt[i])
                {
                    this[i] = false;
                }
            }
        }

        /// <summary>
        /// Set the values of this grid to the result of a XOR operation with grid bt. 
        /// </summary>
        /// <param name="bt"></param>
        public void Xor(BitGrid3D bt)
        {
            if (bt.Count != Count)
            {
                throw new Exception("Grids are of different sizes");
            }

            for (var i = 0; i < Count; i++)
            {
                SetValue(i, GetValue(i) ^ bt.GetValue(i));
            }
        }

		#endregion Methods 
    }
}
