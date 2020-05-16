using System;
using System.Collections.Generic;
using System.Drawing;
using Grasshopper.Kernel;
using StudioAvw.Voxels.Geometry;
using StudioAvw.Voxels.Param;

namespace StudioAvw.Voxels.Components.VoxelGrid
{
    /// <summary>
    /// GH Component to intersect two voxelgrids
    /// </summary>
    public class VoxelGridIntersect : GhVoxelComponent
    {
        /// <summary>
        /// Initializes a new instance of the VoxelGridIntersect class.
        /// </summary>
        public VoxelGridIntersect()
            : base("VoxelGrid Boolean Intersect", "VGIntersect",
                "Intersection result of two grids",
                "Voxels", "Boolean")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddParameter(new Param_VoxelGrid(), "Grids", "G", "The grids for the intersection operations", GH_ParamAccess.list);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddParameter(new Param_VoxelGrid(), "Grids", "G", "The grids for the intersection operations", GH_ParamAccess.item);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="da">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess da)
        {
            var grids = new List<VoxelGrid3D> ();
            da.GetDataList(0, grids);
            if (grids.Count == 0)
            {
                return;
            }

            var outGrid = (VoxelGrid3D)grids[0].Clone();
            for (var i = 1; i < grids.Count; i++)
            {
                outGrid.And(grids[i]);
            }
            da.SetData(0, outGrid);
            AddRenderGrid(outGrid);
        }

        /// <summary>
        /// Provides an Icon for the component.
        /// </summary>
        protected override Bitmap Icon =>
            //You can add image files to your project resources and access them like this:
            // return Resources.IconForThisComponent;
            Images.VT_GridIntersect;

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid => new Guid("{c30ffc42-2b0f-4f88-a1f2-84e4a510a006}");
    }
}