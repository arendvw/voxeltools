using System;
using System.Collections.Generic;
using System.Drawing;
using Grasshopper.Kernel;
using StudioAvw.Voxels.Geometry;
using StudioAvw.Voxels.Param;

namespace StudioAvw.Voxels.Components.VoxelGrid
{
    /// <summary>
    /// Create grid from list of booleans
    /// </summary>
    public class VoxelGridFromList : GhVoxelComponent
    {
        /// <summary>
        /// Initializes a new instance of the VoxelGridIntersect class.
        /// </summary>
        public VoxelGridFromList()
            : base("VoxelGrid From List", "VGList",
                "Set the voxel values from a list of booleans",
                "Voxels", "Analysis")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddParameter(new Param_VoxelGrid(), "Grid", "G", "The grid to modify", GH_ParamAccess.item);
            pManager.AddBooleanParameter("Bool", "B", "True/false value for all cells", GH_ParamAccess.list);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddParameter(new Param_VoxelGrid(), "Grid", "G", "The modified grid", GH_ParamAccess.list);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            var grid = default(VoxelGrid3D);
            var vals = new List<bool>();
            DA.GetData(0, ref grid);
            grid = (VoxelGrid3D)grid.Clone();

            DA.GetDataList(1, vals);
            var max = Math.Min(grid.Grid.Count, vals.Count);

            for (var i = 0; i < max; i++)
            {
                grid.SetValue(i, vals[i]);
            }

            DA.SetData(0, grid);
            AddRenderGrid(grid);
        }

        /// <summary>
        /// Provides an Icon for the component.
        /// </summary>
        protected override Bitmap Icon =>
            //You can add image files to your project resources and access them like this:
            // return Resources.IconForThisComponent;
            Images.VT_ConstructFromList;

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid => new Guid("{E487FE7B-0416-49F2-BDD3-FE6CB0938D25}");
    }
}