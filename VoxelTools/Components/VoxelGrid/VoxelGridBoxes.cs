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
    /// Converts voxels to their containing boxes
    /// </summary>
    public class VoxelGridBoxes : GH_Component
    {
        /// TODO: Add this class as old/replaced
        /// Add new class that implements the points in a correct way
        /// This class has been used before to enumerate all the points in a grid
        /// <summary>
        /// Initializes a new instance of the MyComponent1 class.
        /// </summary>
        public VoxelGridBoxes()
            : base("VoxelGrid To Boxes", "VoxBox",
                "Get a list of boxes from a voxelgrid",
                "Voxels", "Analysis")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddParameter(new Param_VoxelGrid());
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddBoxParameter("Boxes", "BOX", "A list of boxes that are contained in the grid", GH_ParamAccess.list);
            pManager.AddPointParameter("Points", "PTS", "A list of all points contained", GH_ParamAccess.list);
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
            var vg = default(VoxelGrid3D);
            DA.GetData(0, ref vg);
            vg = (VoxelGrid3D) vg.Clone();
            
            var boxes = new List<Box> ();
            var values = new List<Point3d>();
            if (vg == null || !vg.IsValid)
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "The (input) voxelgrid was invalid");
                return;
            }

            for (var i = 0; i < vg.SizeUVW.SelfProduct(); i++)
            {
                values.Add(vg.EvaluatePoint(i));
                if (vg.GetValue(i) == true)
                {
                    boxes.Add(vg.CreateBox(i));
                }
            }
            DA.SetDataList(0, boxes);
            DA.SetDataList(1, values);


            // get top faces
            // get bottom faces
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
        public override Guid ComponentGuid => new Guid("{d8f14822-91e4-417a-931c-a42455a07361}");
    }
}