using System;
using System.Collections.Generic;
using System.Drawing;
using Grasshopper.Kernel;
using Rhino.Geometry;
using StudioAvw.Voxels.Geometry;
using StudioAvw.Voxels.Param;

namespace StudioAvw.Voxels.Components.VoxelGrid
{
    /// <summary>
    /// Enumerate the voxel grid
    /// </summary>
    public class VoxelGridToList : GhVoxelComponent
    {
        /// <summary>
        /// Initializes a new instance of the VoxelGridIntersect class.
        /// </summary>
        public VoxelGridToList()
            : base("VoxelGridToList", "VGList",
                "Decompose a Voxel Grid to a Boolean List",
                "Voxels", "Analysis")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddParameter(new Param_VoxelGrid(), "Grids", "G", "The grid to decompose", GH_ParamAccess.item);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddBooleanParameter("Bool", "B", "True/false value for all cells", GH_ParamAccess.list);
            pManager.AddPointParameter("Pts", "P", "All points that belong to the grid", GH_ParamAccess.list);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            var vg = default(VoxelGrid3D);
            DA.GetData(0, ref vg);

            var values = new List<bool>();
            var location = new List<Point3d>();
            if (vg == null || !vg.IsValid)
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "The (input) voxelgrid was invalid");
                return;
            }

            for (var i = 0; i < vg.SizeUVW.SelfProduct(); i++)
            {
                values.Add(vg.GetValue(i));
                location.Add(vg.EvaluatePoint(i));
            }
            DA.SetDataList(0, values);
            DA.SetDataList(1, location);
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
        public override Guid ComponentGuid => new Guid("{1FD64427-290E-4CB7-9512-69ACC62E5256}");
    }
}