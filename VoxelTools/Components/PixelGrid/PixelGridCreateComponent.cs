using System;
using System.Drawing;
using Grasshopper.Kernel;
using Rhino.Geometry;
using StudioAvw.Voxels.Components.VoxelGrid;
using StudioAvw.Voxels.Geometry;
using StudioAvw.Voxels.Param;
using StudioAvw.Voxels.Tools;
using StudioAvw.Voxels.Types;

namespace StudioAvw.Voxels.Components.PixelGrid
{
    /// <summary>
    /// Create a voxelgrid from any type of geometry
    /// </summary>
    public class PixelGridCreateComponent : GhVoxelComponent
    {
        /// <summary>
        /// Initializes a new instance of the PixelGridCreate class.
        /// </summary>
        public PixelGridCreateComponent()
            : base("Create PixelGrid", "PixelCreate",
                "Create an empty pixelgrid",
                "Pixels", "Create")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddRectangleParameter("BoundingBox", "BB", "The bounding box in which the grid will be created", GH_ParamAccess.item);
            pManager.AddNumberParameter("SizeX", "X", "Size of a pixel in x direction", GH_ParamAccess.item);
            pManager.AddNumberParameter("SizeY", "Y", "Size of a pixel in Y direction", GH_ParamAccess.item);
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
            var bb = DA.Fetch<Rectangle3d>("BoundingBox");
            var x = DA.Fetch<double>("SizeX");
            var y = DA.Fetch<double>("SizeY");

            var vg = new PixelGrid2D(bb, new Point2d(x, y));

            if (!vg.IsValid)
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Result was an invalid grid");
            }
            
            DA.SetData(0, new GH_PixelGrid((PixelGrid2D)vg.Clone()));
        }

        /// <summary>
        /// Provides an Icon for the component.
        /// </summary>
        protected override Bitmap Icon =>
            //You can add image files to your project resources and access them like this:
            // return Resources.IconForThisComponent;
            Images.VT_CreateGrid;

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid => new Guid("{AEA8BCDD-CE5B-4FEC-8902-500C8F0218FB}");
    }
}