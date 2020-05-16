using System;
using System.Drawing;
using Grasshopper.Kernel;
using StudioAvw.Voxels.Geometry;
using StudioAvw.Voxels.Param;

namespace StudioAvw.Voxels.Components.VoxelGrid
{

    /// <summary>
    /// Get numerical statistics about the voxel grid
    /// </summary>
    public class VoxelGridStatistics : GhVoxelComponent
    {
        /// <summary>
        /// Initializes a new instance of the MyComponent1 class.
        /// </summary>
        public VoxelGridStatistics()
            : base("VoxelGrid Statistics", "VoxGridStat",
                "Get numerical statistics of a voxelgrid",
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
            pManager.AddNumberParameter("Surfaces", "F", "The amount of surfaces in the grid", GH_ParamAccess.item);
            pManager.AddNumberParameter("Facade", "F", "The amount of facades in the grid", GH_ParamAccess.item);
            pManager.AddNumberParameter("Terraces", "T", "The amount of terraces in the grid", GH_ParamAccess.item);
            pManager.AddNumberParameter("Overhang", "O", "The amount of overhanging faces in the grid", GH_ParamAccess.item);
            pManager.AddNumberParameter("Footprint", "Fp", "The amount of surfaces touching the ground", GH_ParamAccess.item);
            pManager.AddNumberParameter("VoxelCount", "Vc", "The amount of cells in the grid", GH_ParamAccess.item);
            pManager.AddNumberParameter("VoidCount", "VoC", "The amount of void in the grid", GH_ParamAccess.item);
            pManager.AddNumberParameter("Porosity", "P", "The area per volume ratio (meters)", GH_ParamAccess.item);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            var vg = default(VoxelGrid3D);
            DA.GetData(0, ref vg);
            var facade = 0;
            double facadeArea = 0;

            var terraces = 0;
            double terraceArea = 0;

            var trusses = 0;
            double trussArea = 0;

            var floor = 0;
            double floorArea = 0;


            var f1 = new Point3i(1, 0, 0);
            var f2 = new Point3i(0, 1, 0);
            var f3 = new Point3i(-1, 0, 0);
            var f4 = new Point3i(0, -1, 0);
            var t1 = new Point3i(0, 0, 1);
            var tr = new Point3i(0, 0, -1);

            for (var i = 0; i < vg.Count; i++)
            {
                if (vg[i] == false)
                {
                    continue;
                }

                if (vg.GetRelativePointValue(i, f1) != 1)
                {
                    facade++;
                    facadeArea += vg.VoxelSize.Y*vg.VoxelSize.Z;
                }
                if (vg.GetRelativePointValue(i, f2) != 1)
                {
                    facade++;
                    facadeArea += vg.VoxelSize.X * vg.VoxelSize.Z;
                }
                if (vg.GetRelativePointValue(i, f3) != 1)
                {
                    facade++;
                    facadeArea += vg.VoxelSize.Y * vg.VoxelSize.Z;
                }
                if (vg.GetRelativePointValue(i, f4) != 1)
                {
                    facade++;
                    facadeArea += vg.VoxelSize.X * vg.VoxelSize.Z;
                }
                if (vg.GetRelativePointValue(i, t1) != 1)
                {
                    terraces++;
                    terraceArea += vg.VoxelSize.X * vg.VoxelSize.Z;
                }
                if (vg.GetRelativePointValue(i, tr) == 0)
                {
                    trusses++;
                    trussArea += vg.VoxelSize.X * vg.VoxelSize.Z;
                }
                if (vg.GetRelativePointValue(i, tr) == -1)
                {
                    floor++;
                    floorArea += vg.VoxelSize.X * vg.VoxelSize.Z;
                }
            }

            DA.SetData("Surfaces", facade + terraces +floor + trusses);
            DA.SetData("Facade", facade);
            DA.SetData("Terraces", terraces);
            DA.SetData("Overhang", trusses);
            DA.SetData("Footprint", floor);
            var gridCount = vg.CountNonZero;
            DA.SetData("VoxelCount", gridCount);
            DA.SetData("VoidCount", vg.Count - gridCount);
            DA.SetData("Porosity", (facadeArea + terraceArea + trussArea + floorArea) / (gridCount * vg.VoxelSize[0] * vg.VoxelSize[1] * vg.VoxelSize[2]));
        }

        /// <summary>
        /// Provides an Icon for the component.
        /// </summary>
        protected override Bitmap Icon =>
            //You can add image files to your project resources and access them like this:
            // return Resources.IconForThisComponent;
            Images.VT_Statistics;

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid => new Guid("{19412CC0-E6FC-4D89-BA7C-70D1DC23AD33}");
    }
}