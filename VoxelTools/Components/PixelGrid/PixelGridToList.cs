using System;
using System.Collections.Generic;
using System.Drawing;
using Grasshopper.Kernel;
using Rhino.Geometry;
using StudioAvw.Voxels.Components.VoxelGrid;
using StudioAvw.Voxels.Geometry;
using StudioAvw.Voxels.Param;
using StudioAvw.Voxels.Helper;

namespace StudioAvw.Voxels.Components.PixelGrid
{
    /// <summary>
    /// Enumerate the voxel grid
    /// WARNING: This is an experiment without any useful application
    /// </summary>
    public class PixelGridToList : BaseVoxelComponent
    {
        /// <summary>
        /// Initializes a new instance of the VoxelGridIntersect class.
        /// </summary>
        public PixelGridToList()
            : base("PixelGridToList", "PGList",
                "Decompose a Voxel Grid to a Boolean List",
                "Pixels", "Analysis")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddParameter(new Param_PixelGrid(), "Grids", "G", "The grid to decompose", GH_ParamAccess.item);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddBooleanParameter("Bool", "B", "True/false value for all cells", GH_ParamAccess.list);
            pManager.AddPointParameter("Pts", "P", "All points that belong to the grid", GH_ParamAccess.list);
            pManager.AddRectangleParameter("BBox", "BB", "The boundingbox of the grid", GH_ParamAccess.item);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="da">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess da)
        {
            var pg = da.Fetch<PixelGrid2D>(0);


            var values = new List<bool>();
            var location = new List<Point3d>();
            if (pg == null || !pg.IsValid)
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "The (input) voxelgrid was invalid");
                return;
            }

            for (var i = 0; i < pg.SizeUV.SelfProduct(); i++)
            {
                values.Add(pg.ValueAt(i));
                location.Add(pg.PointAt(i));
            }
            da.SetDataList(0, values);
            da.SetDataList(1, location);
            da.SetData(2, pg.BBox);
        }

        /// <summary>
        /// Provides an Icon for the component.
        /// </summary>
        protected override Bitmap Icon =>
            //You can add image files to your project resources and access them like this:
            // return Resources.IconForThisComponent;
            Images.VT_GridToList;

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid => new Guid("{C04D2F2F-71B9-4A68-9617-5DA396BEDE2A}");

        public override GH_Exposure Exposure => GH_Exposure.hidden;
    }
}