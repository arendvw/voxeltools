using System.Collections;
using Rhino.Geometry;

namespace StudioAvw.Voxels.Geometry
{
    /// Refactor the scalar tools to be an extension
    /// <summary>
    /// Grid with scalar (floating point) values.
    /// </summary>
    public class ScalarGrid3D : Grid3D<float, float[]>
    {
        /// <summary>
        /// Create a grid of a encapsulating box, a width/height (pz) and a pHeight, and a printDel (a print delegate which can log the output of the class);
        /// </summary>
        /// <param name="boundingBox">The boundingbox</param>
        /// <param name="pixelDimensions">A point3d with the length of a pixel in x, y and z dimensions</param>
        public ScalarGrid3D(Box boundingBox, Point3d pixelDimensions)
            : base(boundingBox, pixelDimensions)
        {
            Grid = new float[SizeUVW.SelfProduct()];
        }

        /// <summary>
        /// Empty constructor
        /// </summary>
        // ReSharper disable once UnusedMember.Global
        public ScalarGrid3D() { }

        /// <summary>
        /// Return the amount of true voxels that have a value higher than 0
        /// </summary>
        /// <returns>Amount of non-zero voxels</returns>
        public int VoxelCount()
        {
            if (!IsValid)
            {
                return -1;
            }
            var count = 0;
            for (var i = 0; i < Count; i++)
            {
                if (this[i] > 0)
                {
                    count++;
                }
            }
            return count;
        }

        /// <summary>
        /// Get the value of of a voxel relative to voxel with number i
        /// </summary>
        /// <param name="voxelIndex">The voxel number to get a relative position for</param>
        /// <param name="relativeVoxel">Relative coordinates to the voxel, e.g. (1,0,0) for a voxel to the right</param>
        /// <returns>The value of the relative voxel</returns>
        public float GetRelativePointValue(int voxelIndex, Point3i relativeVoxel)
        {
            var pt = Point3i.IndexToPointUvw(SizeUVW, voxelIndex) + relativeVoxel;
            if (new Point3i(0, 0, 0) > pt || pt >= SizeUVW)
            {
                return float.NaN;
            }
            return GetValue(pt);
        }

        // calculate planes: foreach plane in the grid find the nodes and connect them.
        // and generate a mesh.
        // calculate edges: foreach edge in the grid get the edges

        /// <summary>
        /// Clone the object
        /// </summary>
        /// <returns></returns>
        public override object Clone()
        {
            var vg = new ScalarGrid3D(BBox, VoxelSize);
            for (var i = 0; i < Count; i++)
            {
                vg[i] = this[i];
            }
            return vg;
        }

        /// <summary>
        /// Set a relative point 
        /// </summary>
        /// <param name="voxelIndex"></param>
        /// <param name="fVxValue"></param>
        public override void SetValue(int voxelIndex, float fVxValue)
        {
            Grid[voxelIndex] = fVxValue;
        }


        /// <summary>
        /// Get point 
        /// </summary>
        /// <param name="voxelIndex"></param>
        /// <returns></returns>
        public override float GetValue(int voxelIndex)
        {
            return Grid[voxelIndex];
        }

        /// <summary>
        /// Get the enumerator for the values in the grid
        /// </summary>
        /// <returns></returns>
        public override IEnumerator GetEnumerator()
        {
            return Grid.GetEnumerator();
        }

    }
}