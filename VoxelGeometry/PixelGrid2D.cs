using System;
using System.Collections;
using Rhino.Geometry;

namespace StudioAvw.Voxels.Geometry
{
    /// <summary>
    /// A grid containing voxel information
    /// </summary>
    public class PixelGrid2D : Grid2D<BitArray, bool>
    {
		#region Constructors (3) 

        /// <summary>
        /// Create a voxelgrid from a boundingbox, voxelsize and bitarray
        /// BitArray is not copied but injected directly in the grid.
        /// </summary>
        /// <param name="boundingBox"></param>
        /// <param name="voxelDimensions">Size of the pixel</param>
        /// <param name="grid"></param>
        public PixelGrid2D(Rectangle3d boundingBox, Point2d voxelDimensions, BitArray grid)
            : base()
        {
            Initialize(boundingBox, voxelDimensions);
            Grid = grid;
        }

        private void Initialize(Rectangle3d boundingBox, Point2d pixelDimensions)
        {
            _BBox = boundingBox;
            _PixelSize = pixelDimensions;

            int iX = Convert.ToInt16(Math.Floor(boundingBox.X.Length / pixelDimensions.X));
            int iY = Convert.ToInt16(Math.Floor(boundingBox.Y.Length / pixelDimensions.Y));

            var size = new Point2i(iX, iY);
            SizeUV = size;
        }

        /// <summary>
        /// Create a grid of a encapsulating box, a width/height (pz) and a pHeight, and a printDel (a print delegate which can log the output of the class);
        /// </summary>
        /// <param name="boundingBox">The boundingbox</param>
        /// <param name="voxelDimensions">A point3d with the length of a pixel in x, y and z dimensions</param>
        public PixelGrid2D(Rectangle3d boundingBox, Point2d voxelDimensions)
            : base()
        {
            Initialize(boundingBox, voxelDimensions);
            var bta = new BitArray(SizeUV.SelfProduct(), false);
            bta.SetAll(false);
            Grid = bta;
        }

        /// <summary>
        /// VoxelGrid constructor: Copies (clone) a VoxelGrid from another voxelGrid
        /// Complexity O(n) for voxelsize
        /// </summary>
        /// <param name="vg"></param>
        public PixelGrid2D(PixelGrid2D vg)
        {
            // struct: should be copy
            _BBox = vg._BBox;
            // struct: should be copy
            _PixelSize = vg.PixelSize;
            // struct: should be copy
            SizeUV = vg.SizeUV;
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
            var vg = new PixelGrid2D(_BBox, _PixelSize);
            for (var i = 0; i < Count; i++)
            {
                vg[i] = this[i];
            }
            return vg;
        }

        /// <summary>
        /// Copy the voxelgrid to an empty grid.
        /// </summary>
        /// <returns></returns>
        public PixelGrid2D CloneEmpty()
        {
            return new PixelGrid2D(BBox, PixelSize);
        }

		#endregion Methods 
   
        /// <summary>
        /// Get the value of the voxel ant
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public override bool ValueAt(int iUV)
        {
            return Grid[iUV];
        }

        public override void SetValue(int iUV, bool value)
        {
            Grid[iUV] = value;
        }

        public override IEnumerator GetEnumerator()
        {
            return _Grid.GetEnumerator();
        }

        public override int Count => _Grid.Count;


        public delegate void EnumeratePixels(int x);

    }
}