using System;
using System.Collections.Generic;
using System.Drawing;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using Rhino.Geometry;
using StudioAvw.Voxels.Geometry;
using StudioAvw.Voxels.Param;
using StudioAvw.Voxels.Helper;
using StudioAvw.Voxels.Types;

namespace StudioAvw.Voxels.Components.PixelGrid
{
    /// <summary>
    /// Add brep to the voxelgrid
    /// WARNING: This is an experiment without any useful application
    /// </summary>
    public class PixelGridPixelateComponent : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the MyComponent1 class.
        /// </summary>
        public PixelGridPixelateComponent()
            : base("PixelateGeometry", "PixGeo",
                "Adds any geometry to a PixelGrid",
                "Pixels", "Create")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddGeometryParameter("Geometry","G", "Geometry to voxelate", GH_ParamAccess.list);
            pManager.AddParameter(new Param_PixelGrid());
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddParameter(new Param_PixelGrid());
        }


        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            //1 divide the brep by the Z components of the grid (get Z values on plane of the grid
            //2 intersect the grid for reach Z value -> planar surface the results
            //3 check for each pixel if it is closer than the 
            var geometry = DA.FetchList<IGH_GeometricGoo>(0);
            var pg = DA.Fetch<PixelGrid2D>(1);
            pg = (PixelGrid2D)pg.Clone();
         
            if (pg == null || !pg.IsValid)
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "The (input) voxelgrid was invalid");
                return;
            }

                     
            var geometryIndex = new Dictionary<int, IGH_GeometricGoo>();
            for (int i = 0, count = geometry.Count; i < count; i++)
            {
                geometryIndex.Add(i, geometry[i]);
            }

            var ptList = GeometryHelper.TryCastGeometry<Point3d>(geometryIndex, false);
            var curveList = GeometryHelper.TryCastGeometry<Curve>(geometryIndex, false);

            foreach (var pt in ptList.Values)
            {
                AddPoint3d(pg, pt);
            }

            foreach (var c in curveList.Values)
            {
                AddCurve(pg, c);
            }

            DA.SetData(0, new GH_PixelGrid((PixelGrid2D) pg.Clone()));

        }

        private void AddCurve(PixelGrid2D pg, Curve c)
        {
            var projection = Transform.PlanarProjection(pg.Plane);
            c.Transform(projection);

            if (c.IsClosed)
            {
                var bb = c.GetBoundingBox(pg.Plane);
                PixelGrid2D.EnumeratePixels testDelB 
                    = delegate(int i) {

                    };

                for (var i = 0; i < pg.Count; i++)
                {
                    var pt = pg.PointAt(i);
                    var pc = c.Contains(pt, pg.Plane);
                    if (pc.HasFlag(PointContainment.Inside) || pc.HasFlag(PointContainment.Coincident))
                    {
                        pg.SetValue(i, true);
                    }
                }
            }
        }

        private void AddPoint3d(PixelGrid2D pg, Point3d pt)
        {
            pg.Plane.RemapToPlaneSpace(pt, out var p);
            var ix = (p.X - pg.BBox.X.Min - pg.PixelSize.X / 2) / pg.PixelSize.X;
            var iy = (p.Y - pg.BBox.Y.Min - pg.PixelSize.Y / 2) / pg.PixelSize.Y;
            

            var pti = pg.ClosestPoint(pt);

            pti = new Point2i(ix, iy);
            if (new Point2i(0, 0) > pti || pti >= pg.SizeUV)
            {
                return;
            }
            pg[pti] = true;
        }


        /// <summary>
        /// Provides an Icon for the component.
        /// </summary>
        protected override Bitmap Icon =>
            //You can add image files to your project resources and access them like this:
            // return Resources.IconForThisComponent;
            Images.VT_VoxelateBrep;

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid => new Guid("{B646EB8C-7FC4-4880-88B4-307BACD4B77C}");

        public override GH_Exposure Exposure => GH_Exposure.hidden;
    }
}