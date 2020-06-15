using System;
using System.Drawing;
using Grasshopper.Kernel;
using Rhino.Geometry;
using StudioAvw.Voxels.Geometry;
using StudioAvw.Voxels.Param;
using StudioAvw.Voxels.Types;

namespace StudioAvw.Voxels.Components.ScalarGrid
{
    /// <summary>
    /// Creates a scalar grid
    /// </summary>
    public class ScalarGridCreateComponent : BaseScalarComponent
    {
        /// <summary>
        /// Initializes a new instance of the VoxelGridCreate class.
        /// </summary>
        public ScalarGridCreateComponent()
            : base("Create Scalar Grid", "ScaVoxCreate",
                "Create an empty scalarlgrid",
                "Voxels", "Scalar")
        {
        }


        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            
            pManager.AddBoxParameter("BoundingBox", "BB", "The bounding box in which the grid will be created", GH_ParamAccess.item);
            pManager.AddNumberParameter("SizeX", "X", "Size of a pixel in x direction", GH_ParamAccess.item);
            pManager.AddNumberParameter("SizeY", "Y", "Size of a pixel in Y direction", GH_ParamAccess.item);
            pManager.AddNumberParameter("SizeZ", "Z", "Size of a pixel in Z direction", GH_ParamAccess.item);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddParameter(new Param_ScalarGrid());
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            var bb = new Box();
            double x, y, z;
            x = y = z = 0;
            DA.GetData(1, ref x);
            DA.GetData(2, ref y);
            DA.GetData(3, ref z);
            DA.GetData(0, ref bb);

            var vg = new ScalarGrid3D(bb, new Point3d(x,y,z));

            if (!vg.IsValid)
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "Result was an invalid grid");
            }

            DA.SetData(0, new GH_ScalarGrid(vg));
        }

        /// <summary>
        /// Provides an Icon for the component.
        /// </summary>
        protected override Bitmap Icon =>
            //You can add image files to your project resources and access them like this:
            // return Resources.IconForThisComponent;
            Images.ST_CreateGrid;

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid => new Guid("{880FC894-98AB-497E-82FC-63F8F9689CBC}");
    }
}