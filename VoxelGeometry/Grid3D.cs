using System;
using System.Collections;
using System.Collections.Generic;
using Rhino.Geometry;
using StudioAvw.Voxels.Geometry.Interfaces;

namespace StudioAvw.Voxels.Geometry
{
    /// <summary>
    /// Base class for a 3 dimensional grid with dimensions x,y,z
    /// </summary>
    /// <typeparam name="TValue"></typeparam>
    /// <typeparam name="TCollection"></typeparam>
    [Serializable]
    public abstract class Grid3D<TValue, TCollection> : IEnumerable, ICloneable, IGrid3D
    {
        #region Fields (4)

        // the boundingbox
        /// <summary>
        /// Bounding box of the voxel grid
        /// </summary>
        public Box BBox { get; protected set; }
        /// <summary>
        /// Grid containing the values
        /// </summary>
        public TCollection Grid { get; protected set; }
       
        public Point3d VoxelSize { get; protected set; }

        #endregion Fields

        #region Constructors (1)
        /// <summary>
        /// Empty constructor
        /// </summary>
        public Grid3D() { }

        /// <summary>
        /// Create an empty grid
        /// </summary>
        public Grid3D(Box bbox, Point3d voxelDimensions)
        {
            Initialize(bbox, voxelDimensions);
        }

        #endregion Constructors

        #region Properties (8)

        /// <summary>
        /// Count the amount of values in the grid
        /// </summary>
        public int Count => SizeUVW.SelfProduct();

        /// <summary>
        /// Return if this is a valid grid
        /// </summary>
        public bool IsValid => SizeUVW.SelfProduct() > 0 && SizeUVW.IsPositiveNonZero();

        /// <summary>
        /// Current plane of the VoxelGrid
        /// </summary>
        public Plane Plane => BBox.Plane;


        private Point3i _sizeUvw = new Point3i(0, 0, 0);
        /// <summary>
        /// Get or set the size (amount of cells) of the grid.
        /// </summary>
        public Point3i SizeUVW
        {
            get => _sizeUvw;
            set
            {
                if (value.SelfProduct() < 1 || !value.IsPositiveNonZero())
                {
                    throw new ArgumentException("The size of a grid should at least be 1x1x1 and non-negative");
                }
                _sizeUvw = value;
            }
        }

        /// <summary>
        /// Get or set the value of the voxel at voxel numer iVx
        /// </summary>
        /// <param name="iVx"></param>
        /// <returns></returns>
        public virtual TValue this[int iVx]
        {
            get => GetValue(iVx);
            set => SetValue(iVx, value);
        }
        #endregion Properties

        #region Methods (18)

        // Public Methods (16) 

        /// <summary>
        /// Clone the grid
        /// </summary>
        /// <returns></returns>
        public abstract object Clone();

        /// <summary>
        /// Clone the object
        /// </summary>
        /// <returns></returns>
        public TGridType Clone<TGridType>() where TGridType : Grid3D<TValue, TCollection>, new()
        {
            var vg = new TGridType();
            vg.Initialize(BBox, VoxelSize);
            for (var i = 0; i < Count; i++)
            {
                vg.SetValue(i, this[i]);
            }
            return vg;
        }

        /// <summary>
        /// Get the voxel center location for at point in 3d space.
        /// </summary>
        /// <param name="worldPt">Point in World Coordinates</param>
        /// <returns>UVW Coordinates</returns>
        public Point3i ClosestPoint(Point3d worldPt)
        {
            Plane.RemapToPlaneSpace(worldPt, out var p);
            var ix = (p.X - BBox.X.Min - VoxelSize.X / 2) / VoxelSize.X;
            var iy = (p.Y - BBox.Y.Min - VoxelSize.Y / 2) / VoxelSize.Y;
            var iz = (p.Z - BBox.Z.Min - VoxelSize.Z / 2) / VoxelSize.Z;

            return new Point3i(new Point3d(ix, iy, iz));
        }

        // create a box from x y z coordinates
        /// <summary>
        /// Creater a box for voxel numer voxelIndex
        /// </summary>
        /// <param name="iVx">Box with dimensions of VoxelDimensions</param>
        /// <returns></returns>
        public Box CreateBox(int iVx)
        {
            var center = EvaluatePoint(iVx);
            var bp = BBox.Plane;
            bp.Origin = center;
            var b = new Box(
              bp,
              new Interval(-VoxelSize.X / 2, VoxelSize.X / 2),
              new Interval(-VoxelSize.Y / 2, VoxelSize.Y / 2),
              new Interval(-VoxelSize.Z / 2, VoxelSize.Z / 2)
              );
            return b;
        }

        /// <summary>
        /// Get enumerator
        /// </summary>
        /// <returns></returns>
        public abstract IEnumerator GetEnumerator();

        /// <summary>
        /// Get point value at location voxelIndex
        /// </summary>
        /// <param name="voxelIndex"></param>
        /// <returns></returns>
        public abstract TValue GetValue(int voxelIndex);

        /// <summary>
        /// Get value of point at location pointUvw
        /// </summary>
        /// <param name="pointUvw"></param>
        /// <returns></returns>
        public TValue GetValue(Point3i pointUvw)
        {
            return GetValue(Point3i.PointUvwToIndex(SizeUVW, pointUvw));
        }

        /// <summary>
        /// Get a list of points inside a bounding box
        /// </summary>
        /// <param name="bb">The bb.</param>
        /// <returns></returns>
        public IEnumerable<Point3i> PointsInBox(BoundingBox bb)
        {
            var pts = new List<Point3i>();
            bb.Inflate(VoxelSize.X * 2, VoxelSize.Y * 2, VoxelSize.Z * 2);
            var min = ClosestPoint(bb.Min);
            var max = ClosestPoint(bb.Max);
            for (int iX = Math.Max(min.X, (short)0); iX <= Math.Min(max.X, SizeUVW.X); iX++)
            {
                for (int iY = Math.Max(min.Y, (short)0); iY <= Math.Min(max.Y, SizeUVW.Y); iY++)
                {
                    for (int iZ = Math.Max(min.Z, (short)0); iZ <= Math.Min(max.Z, SizeUVW.Z); iZ++)
                    {
                        yield return new Point3i(iX, iY, iZ);
                    }
                }
            }
        }

        /// <summary>
        /// Gets World coordinates for UVW coordinate
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        public Point3d EvaluatePoint(Point3d p)
        {
            return EvaluatePoint(p, Plane);
        }

        /// <summary>
        /// Gets World coordinates for UVW coordinate
        /// </summary>
        /// <param name="ptUvw">Voxel location</param>
        /// <returns>Voxel in world coordinates</returns>
        public Point3d EvaluatePoint(Point3i ptUvw)
        {
            return EvaluatePoint(ptUvw, BBox.Plane);
        }

        /// <summary>
        /// Gets World coordinates for UVW coordinate
        /// </summary>
        /// <param name="voxelIndex">Voxel number</param>
        /// <returns></returns>
        public Point3d EvaluatePoint(int voxelIndex)
        {
            return EvaluatePoint(Point3i.IndexToPointUvw(SizeUVW, voxelIndex));
        }

        /// <summary>
        /// Gets World coordinates for UVW coordinate
        /// </summary>
        /// <param name="pUvw">Point in voxel space</param>
        /// <param name="pln">Orientation for voxel grid</param>
        /// <returns>Point in 3d (World) space</returns>
        public Point3d EvaluatePoint(Point3d pUvw, Plane pln)
        {
            var centerX = pUvw.X * VoxelSize.X + BBox.X.Min + VoxelSize.X / 2;
            var centerY = pUvw.Y * VoxelSize.Y + BBox.Y.Min + VoxelSize.Y / 2;
            var centerZ = pUvw.Z * VoxelSize.Z + BBox.Z.Min + VoxelSize.Z / 2;
            return pln.PointAt(centerX, centerY, centerZ);
        }

        /// <summary>
        /// Get the position of a voxel in 3d (world) space.
        /// </summary>
        /// <param name="pUvw">Voxel Position</param>
        /// <param name="pln">Plane (orientation) for the grid</param>
        /// <returns></returns>
        public Point3d EvaluatePoint(Point3i pUvw, Plane pln)
        {
            return EvaluatePoint(pUvw.ToPoint3D(), pln);
        }

        /// <summary>
        /// Get the voxel location for voxel numer voxelIndex, in world coordinates
        /// </summary>
        /// <param name="indexUvw">Voxel numer</param>
        /// <param name="pln">Orientation of the grid</param>
        /// <returns></returns>
        public Point3d EvaluatePoint(int indexUvw, Plane pln)
        {
            return EvaluatePoint(Point3i.IndexToPointUvw(SizeUVW, indexUvw), pln);
        }


        /// <summary>
        /// Set voxel at grid location pointUvw
        /// </summary>
        /// <param name="pointUvw"></param>
        /// <param name="value"></param>
        public void SetValue(Point3i pointUvw, TValue value)
        {
            SetValue(Point3i.PointUvwToIndex(SizeUVW, pointUvw), value);
        }

        /// <summary>
        /// Set value of voxel at location voxelIndex 
        /// </summary>
        /// <param name="voxelIndex"></param>
        /// <param name="value"></param>
        public abstract void SetValue(int voxelIndex, TValue value);

        /// <summary>
        /// Convert the grid to a string describing the numerics of the grid
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            if (!IsValid)
            {
                return $"Invalid VoxelGrid [{SizeUVW.X},{SizeUVW.Y},{SizeUVW.Z}={SizeUVW.SelfProduct()}]";
            }
            return $"VoxelGrid [{SizeUVW.X},{SizeUVW.Y},{SizeUVW.Z}={SizeUVW.SelfProduct()}]";
            /*
            return String.Format("VoxelGrid [{0},{1},{2}={3}] with Cell Size [{4.0},{5.0},{6.0}] with {7} Voxels",
                this.Size.x, this.Size.y, this.Size.z, this.Size.selfProduct(),
                this.VoxelSize.X, this.VoxelSize.Y, this.VoxelSize.Z,
                this.VoxelCount()
                );*/
        }

        /**
         * Convert a point3i into a worldvector on the box plane 
         */
        public Vector3d ToVector(Point3d p, Plane pln)
        {
            var centerX = p.X * VoxelSize.X + BBox.X.Min + VoxelSize.X / 2;
            var centerY = p.Y * VoxelSize.Y + BBox.Y.Min + VoxelSize.Y / 2;
            var centerZ = p.Z * VoxelSize.Z + BBox.Z.Min + VoxelSize.Z / 2;
            return pln.PointAt(centerX, centerY, centerZ) - pln.PointAt(0, 0, 0);
        }
        // Protected Methods (1) 

        /// <summary>
        /// Initialize a grid from a bounding box and voxel dimensions
        /// </summary>
        /// <param name="boundingBox"></param>
        /// <param name="voxelDimensions"></param>
        protected void Initialize(Box boundingBox, Point3d voxelDimensions)
        {
            BBox = boundingBox;
            VoxelSize = voxelDimensions;

            int iX = Convert.ToInt16(Math.Floor(boundingBox.X.Length / voxelDimensions.X));
            int iY = Convert.ToInt16(Math.Floor(boundingBox.Y.Length / voxelDimensions.Y));
            int iZ = Convert.ToInt16(Math.Floor(boundingBox.Z.Length / voxelDimensions.Z));

            var size = new Point3i(iX, iY, iZ);
            SizeUVW = size;

        }
        // Private Methods (1) 

        /// <summary>
        /// Return the enumerator for this grid
        /// </summary>
        /// <returns></returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        #endregion Methods

        /// <summary>
        /// Get value of voxel at location pointUvw
        /// </summary>
        /// <param name="ptVx">x,y,z location of voxel</param>
        /// <returns></returns>
        public TValue this[Point3i ptVx]    // Indexer declaration
        {
            get => GetValue(ptVx);
            set => SetValue(ptVx, value);
        }
    }
}
