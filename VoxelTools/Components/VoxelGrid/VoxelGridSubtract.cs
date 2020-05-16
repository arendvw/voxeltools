using System;
using System.Collections.Generic;
using System.Drawing;
using Grasshopper.Kernel;
using StudioAvw.Voxels.Geometry;
using StudioAvw.Voxels.Param;

namespace StudioAvw.Voxels.Components.VoxelGrid
{
    /// <summary>
    /// GH Component to subtract grid A from grid B
    /// </summary>
    public class VoxelGridSubtract : GhVoxelComponent
    {
        /// <summary>
        /// Initializes a new instance of the VoxelGridIntersect class.
        /// </summary>
        public VoxelGridSubtract()
            : base("VoxelGrid Boolean Subtract", "VGSubtract",
                "Subtract grid 1..n from the first grid",
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
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            var grids = new List<VoxelGrid3D> ();
            DA.GetDataList(0, grids);
            if (grids.Count == 0)
            {
                return;
            }

            var outGrid = (VoxelGrid3D)grids[0].Clone();
            for (var i = 1; i < grids.Count; i++)
            {
                outGrid.Subtract(grids[i]);
            }
            DA.SetData(0, outGrid);
            AddRenderGrid(outGrid);
        }

        /// <summary>
        /// Provides an Icon for the component.
        /// </summary>
        protected override Bitmap Icon =>
            //You can add image files to your project resources and access them like this:
            // return Resources.IconForThisComponent;
            Images.VT_GridDifference;

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid => new Guid("{6AC05026-BBA2-4FBD-ABEB-AC868E99D96E}");
    }
}