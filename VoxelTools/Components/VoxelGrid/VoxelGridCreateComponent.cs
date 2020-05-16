using System;
using System.Drawing;
using Grasshopper.Kernel;
using Rhino.Geometry;
using StudioAvw.Voxels.Geometry;
using StudioAvw.Voxels.Param;

namespace StudioAvw.Voxels.Components.VoxelGrid
{
    /// <summary>
    /// Create a voxelgrid from any type of geometry
    /// </summary>
    public class VoxelGridCreateComponent : GhVoxelComponent
    {
        /// <summary>
        /// Initializes a new instance of the VoxelGridCreate class.
        /// </summary>
        public VoxelGridCreateComponent()
            : base("Create VoxelGrid", "VoxCreate",
                "Create an empty voxelgrid",
                "Voxels", "Create")
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
            pManager.AddParameter(new Param_VoxelGrid());
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

            var vg = new VoxelGrid3D(bb, new Point3d(x,y,z));

            if (!vg.IsValid)
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "Result was an invalid grid");
            }

            DA.SetData(0, (VoxelGrid3D) vg.Clone());
            AddRenderGrid(vg);
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
        public override Guid ComponentGuid => new Guid("{623edb3b-0d3e-4ff6-a04a-20dcbd7e1ab5}");
    }
}