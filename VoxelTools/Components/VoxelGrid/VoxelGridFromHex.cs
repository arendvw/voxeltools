using System;
using System.Drawing;
using Grasshopper.Kernel;
using StudioAvw.Voxels.Geometry;
using StudioAvw.Voxels.Param;
using StudioAvw.Voxels.Helper;

namespace StudioAvw.Voxels.Components.VoxelGrid
{
    /// <summary>
    /// Create list of hexadecimal numbers
    /// </summary>
    public class VoxelGridFromHex : BaseVoxelComponent
    {
        /// <summary>
        /// Initializes a new instance of the VoxelGridIntersect class.
        /// </summary>
        public VoxelGridFromHex()
            : base("VoxelGrid From Hex String", "VGFromHex",
                "Convert a voxelgrid from a hexadecimal string",
                "Voxels", "Input/Output")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddTextParameter("Hex", "H", "Hexadecimal string describing the grid", GH_ParamAccess.item);
            
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
            var hex = "";
            DA.GetData(0, ref hex);
            var output = ByteHelper.ToVoxelGrid(ByteHelper.Decompress(ByteHelper.StringToByteArray(hex)));
            DA.SetData(0, output);
        }

        /// <summary>
        /// Provides an Icon for the component.
        /// </summary>
        protected override Bitmap Icon =>
            //You can add image files to your project resources and access them like this:
            // return Resources.IconForThisComponent;
            Images.VT_GridFromHex;

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid => new Guid("{08A46861-EF75-4579-A427-379C5576B73A}");
    }
}