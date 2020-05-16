using System;
using System.Drawing;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using Rhino.Geometry;
using StudioAvw.Voxels.Geometry;
using StudioAvw.Voxels.Param;
using StudioAvw.Voxels.Tools;

namespace StudioAvw.Voxels.Components.PixelGrid
{
    /// <summary>
    /// Create a voxelgrid mesh hull describing the outer hull of the voxel grid
    /// </summary>
    public class PixelGridMesh : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the MyComponent1 class.
        /// </summary>
        public PixelGridMesh()
            : base("PixelGrid To Mesh Hull", "PixMeshHull", "Generate a mesh from a pixelgrid",
                "Pixels", "Analysis")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddParameter(new Param_PixelGrid());

            pManager.AddColourParameter("TrueColour", "T", "Color for true cells", GH_ParamAccess.item, Color.Black);
            pManager.AddColourParameter("FalseColour", "F", "Color for false cells", GH_ParamAccess.item, Color.White);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddMeshParameter("Mesh", "M", "A Mesh of the hull of the VoxelGrid", GH_ParamAccess.item);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            /*
             * todo: select mesh box or simple box
             */
            var pg = DA.Fetch<PixelGrid2D> (0);
            var trueColor = DA.Fetch<Color>(1);
            var falseColor = DA.Fetch<Color>(2);

            if (pg == null || !pg.IsValid)
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "The (input) voxelgrid was invalid");
                return;
            }
            var m = pg.GenerateMesh(trueColor, falseColor);
            DA.SetData(0, new GH_Mesh(m));

        }

        /// <summary>
        /// Provides an Icon for the component.
        /// </summary>
        protected override Bitmap Icon =>
            //You can add image files to your project resources and access them like this:
            // return Resources.IconForThisComponent;
            Images.VT_Decompose;

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid => new Guid("{4DAA6753-C19D-4CB0-AAE6-08E09D8F6918}");
    }
}