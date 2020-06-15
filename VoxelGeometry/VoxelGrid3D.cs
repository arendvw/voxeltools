using System.Collections;
using Rhino.Geometry;

namespace StudioAvw.Voxels.Geometry
{
    /// <summary>
    /// A grid containing voxel information
    /// </summary>
    public class VoxelGrid3D : BitGrid3D
    {
        #region Constructors (3)

        /// <summary>
        /// Create a voxelgrid from a boundingbox, voxelsize and bitarray
        /// BitArray is not copied but injected directly in the grid.
        /// </summary>
        /// <param name="boundingBox"></param>
        /// <param name="voxelDimensions">Size of the pixel</param>
        /// <param name="grid"></param>
        public VoxelGrid3D(Box boundingBox, Point3d voxelDimensions, BitArray grid)
        {
            Initialize(boundingBox, voxelDimensions);
            Grid = grid;
        }

        /// <summary>
        /// Create a grid of a encapsulating box, a width/height (pz) and a pHeight, and a printDel (a print delegate which can log the output of the class);
        /// </summary>
        /// <param name="boundingBox">The boundingbox</param>
        /// <param name="voxelDimensions">A point3d with the length of a pixel in x, y and z dimensions</param>
        public VoxelGrid3D(Box boundingBox, Point3d voxelDimensions)
        {
            Initialize(boundingBox, voxelDimensions);
            var bta = new BitArray(SizeUVW.SelfProduct(), false);
            bta.SetAll(false);
            Grid = bta;
        }

        /// <summary>
        /// VoxelGrid constructor: Copies (clone) a VoxelGrid from another voxelGrid
        /// Complexity O(n) for voxelsize
        /// </summary>
        /// <param name="vg"></param>
        public VoxelGrid3D(VoxelGrid3D vg)
        {
            // struct: should be copy
            this.BBox = vg.BBox;
            // struct: should be copy
            VoxelSize = vg.VoxelSize;
            // struct: should be copy
            SizeUVW = vg.SizeUVW;
            // class: needs deep copy
            Grid = new BitArray(vg.Grid.Count);
            for (var i = 0; i < vg.Grid.Count; i++)
            {
                Grid[i] = vg.Grid[i];
            }
        }

        #endregion Constructors

        #region Methods (5)

        // Public Methods (4) 

        /// <summary>
        /// Clone the object
        /// </summary>
        /// <returns></returns>
        public override object Clone()
        {
            var vg = new VoxelGrid3D(this.BBox, VoxelSize);
            for (var i = 0; i < Count; i++)
            {
                if (this[i])
                {
                    vg[i] = true;
                }
            }
            return vg;
        }

        /// <summary>
        /// Copy the voxelgrid to an empty grid.
        /// </summary>
        /// <returns></returns>
        public VoxelGrid3D CloneEmpty()
        {
            return new VoxelGrid3D(BBox, VoxelSize);
        }

        #endregion Methods

        public bool PointInGrid(Point3i subPt)
        {
            return !(subPt < Point3i.Origin || subPt >= SizeUVW);
        }
    }
}