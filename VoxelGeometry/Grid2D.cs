// -----------------------------------------------------------------------
// <copyright file="Class1.cs" company="">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections;
using Rhino.Geometry;
using StudioAvw.Voxels.Geometry.Interfaces;

namespace StudioAvw.Voxels.Geometry
{
    /// <summary>
    /// A 2d optimized alternative to the voxelgrid.
    /// </summary>
    public abstract class Grid2D<T, Y> : IEnumerable, ICloneable, IGrid2D
    {
        protected Rectangle3d _BBox;
        protected Point2d _PixelSize;
        protected Point2i _SizeUV;
        protected T _Grid;


         public Grid2D() : base() { }

        /// <summary>
        /// Construct a pixelgrid
        /// </summary>
        /// <param name="boundingBox"></param>
        /// <param name="pxielDimensions"></param>
        /// <param name="grid"></param>
        public Grid2D(Rectangle3d boundingBox, Point2d pxielDimensions, T grid)
            : base()
        {
            _BBox = boundingBox;
            _PixelSize = pxielDimensions;

            int iX = Convert.ToInt16(Math.Floor(boundingBox.X.Length / pxielDimensions.X));
            int iY = Convert.ToInt16(Math.Floor(boundingBox.Y.Length / pxielDimensions.Y));

            var gridsize = new Point2i(iX, iY);
            SizeUV = gridsize;
            Grid = grid;
        }



        /// <summary>
        /// Set or get the bounding region of the box
        /// </summary>
        public Rectangle3d BBox => _BBox;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="iVx"></param>
        /// <param name="ptRelVx"></param>
        /// <returns></returns>
        public Point2i GetRelativeUV(int iVx, Point2i ptRelVx)
        {
            var pt = IndexUV(iVx) + ptRelVx;
            if (new Point2i(0, 0) > pt || pt >= SizeUV)
            {
                throw new Exception("Index does not exist");
            }

            return pt;
        }

        /// <summary>
        /// Return the value of the voxel at index
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public abstract Y ValueAt(int index);
        
        /// <summary>
        /// Return a value at index
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public Y ValueAt(Point2i index)
        {
            return ValueAt(UVIndex(index));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public Point2i IndexUV(int index)
        {
 	        return (_SizeUV % index);
        }

        /// <summary>
        /// Convert a point2i location to the index of the point in the bitarray
        /// </summary>
        /// <param name="uv"></param>
        /// <returns></returns>
        public int UVIndex(Point2i uv)
        {
            return uv^_SizeUV;
        }

        /// <summary>
        /// Dimensions of a voxel
        /// </summary>
        public Point2d PixelSize { get => _PixelSize;
            set => _PixelSize = value;
        }

        /// <summary>
        /// Amount of cells in the grid in x and y directions
        /// </summary>
        public Point2i SizeUV { get => _SizeUV;
            set => _SizeUV = value;
        }

        /// <summary>
        /// Values of the grid
        /// </summary>
        public T Grid { get => _Grid;
            set => _Grid = value;
        }

        /// <summary>
        /// 
        /// </summary>
        public Plane Plane => _BBox.Plane;

        /// <summary>
        /// Clone the object
        /// </summary>
        /// <returns></returns>
        public abstract object Clone();

        // Indexer declaration: 
        public Y this[int index]
        {

            get => ValueAt(index);
            set => SetValue(index, value);
        }

        // Indexer declaration: 
        public Y this[Point2i index]
        {

            get => ValueAt(index);
            set => SetValue(index, value);
        }

        /// <summary>
        /// Set value of voxel with index
        /// </summary>
        /// <param name="index">index of voxel</param>
        /// <param name="value">value of voxel</param>
        public abstract void SetValue(int index, Y value);

        /// <summary>
        /// Set value of Voxel
        /// </summary>
        /// <param name="index"></param>
        /// <param name="value"></param>
        public void SetValue(Point2i index, Y value)
        {
            SetValue(UVIndex(index), value);
        }

        /// <summary>
        /// Return the enumerator
        /// </summary>
        /// <returns></returns>
        public abstract IEnumerator GetEnumerator();

        public bool IsValid => SizeUV.SelfProduct() > 0 && SizeUV.IsPositiveNonZero();

        /// <summary>
        /// Gets World coordinates for UVW coordinate
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        public Point3d PointAt(Point3d p)
        {
            return PointAt(p, Plane);
        }

        public Point3d PointAt(Point2d p)
        {
            return PointAt(new Point3d(p.X,p.Y,0), Plane);
        }

        /// <summary>
        /// Gets World coordinates for UVW coordinate
        /// </summary>
        /// <param name="ptUVW">Voxel location</param>
        /// <returns>Voxel in world coordinates</returns>
        public Point3d PointAt(Point2i ptUV)
        {
            return PointAt(new Point3d(ptUV.X,ptUV.Y, 0), Plane);
        }

        /// <summary>
        /// Gets World coordinates for UVW coordinate
        /// </summary>
        /// <param name="iUVW">Voxel number</param>
        /// <returns></returns>
        public Point3d PointAt(int iUVW)
        {
            return PointAt(SizeUV % iUVW);
        }

        /// <summary>
        /// Gets World coordinates for UVW coordinate
        /// </summary>
        /// <param name="pUVW">Point in voxel space</param>
        /// <param name="pln">Orientation for voxel grid</param>
        /// <returns>Point in 3d (World) space</returns>
        public Point3d PointAt(Point3d pUV, Plane pln)
        {
            var centerX = pUV.X * _PixelSize.X + _BBox.X.Min + _PixelSize.X / 2;
            var centerY = pUV.Y * _PixelSize.Y + _BBox.Y.Min + _PixelSize.Y / 2;
            return pln.PointAt(centerX, centerY);
        }

        public Point2i ClosestPoint (Point3d pt)
        {
            Plane.RemapToPlaneSpace(pt, out var p);
            var ix = (p.X - BBox.X.Min - PixelSize.X / 2) / PixelSize.X;
            var iy = (p.Y - BBox.Y.Min - PixelSize.Y / 2) / PixelSize.Y;
            return new Point2i(ix,iy);
        }

        public abstract int Count { get;  }

    }
}
