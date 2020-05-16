// <copyright file="VoxelGridAccessibleRoof.cs" company="StudioAvw">
//     Copyright (c) 2013 StudioAvw. All rights reserved
// <author>Arend van Waart</author>
// </copyright>

using System;
using System.Drawing;
using Grasshopper.Kernel;
using StudioAvw.Voxels.Geometry;
using StudioAvw.Voxels.Param;

namespace StudioAvw.Voxels.Components.VoxelGrid
{
    /// <summary>
    /// Counts the number of accessible roofs and outputs them in two different lists
    /// </summary>
    public class VoxelGridRoofs : GhVoxelComponent
    {
		#region Constructors (1) 

        /// <summary>
        /// Initializes a new instance of the VoxelGridRoofs class.
        /// </summary>
        public VoxelGridRoofs()
            : base("VoxelGrid RoofAccess", "VoxGridRoof",
                "Check if voxels have accessible roof pixels",
                "Voxels", "Analysis")
        {
        }

		#endregion Constructors 

		#region Properties (2) 

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid => new Guid("{FB7ED4B6-421B-4302-AA5B-DADA34AA2CCA}");

        /// <summary>
        /// Provides an Icon for the component.
        /// </summary>
        protected override Bitmap Icon =>
            //You can add image files to your project resources and access them like this:
            // return Resources.IconForThisComponent;
            Images.VT_Statistics;

        #endregion Properties 

		#region Methods (3) 

		// Protected Methods (3) 

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
            pManager.AddParameter(new Param_VoxelGrid(), "HasAccessible", "HA", "Voxels with roof access", GH_ParamAccess.item);
            pManager.AddParameter(new Param_VoxelGrid(), "IsRoof", "IR", "Voxels with roofs that can be accessed", GH_ParamAccess.item);
            pManager.AddParameter(new Param_VoxelGrid(), "Access via stairs", "AS", "Voxels access via stairs", GH_ParamAccess.item);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            var vg = default(VoxelGrid3D);
            DA.GetData(0, ref vg);

            var isRoof = new VoxelGrid3D(vg.BBox, vg.VoxelSize);
            var hasAccessible = new VoxelGrid3D(vg.BBox, vg.VoxelSize);
            var accessViaStairs = new VoxelGrid3D(vg.BBox, vg.VoxelSize);

            var tr = new Point3i(0, 0, 1);
            var t1 = new Point3i(0, 1, 1);
            var t2 = new Point3i(1, 0, 1);
            var t3 = new Point3i(-1, 0, 1);
            var t4 = new Point3i(0, -1, 1);

            for (var i = 0; i < vg.Count; i++)
            {
                if (vg[i] == false)
                {
                    continue;
                }
                // there's nothing above us.. only sky
                if (vg.GetRelativePointValue(i, tr) == 0)
                {
                    var accessible = false;
                    if (vg.GetRelativePointValue(i, t1) == 1)
                    {
                        accessible = true;
                        hasAccessible.SetRelativePointValue(i, t1, true);
                    }
                    if (vg.GetRelativePointValue(i, t2) == 1)
                    {
                        accessible = true;
                        hasAccessible.SetRelativePointValue(i, t2, true);
                    }
                    if (vg.GetRelativePointValue(i, t3) == 1)
                    {
                        accessible = true;
                        hasAccessible.SetRelativePointValue(i, t3, true);
                    }
                    if (vg.GetRelativePointValue(i, t4) == 1)
                    {
                        accessible = true;
                        hasAccessible.SetRelativePointValue(i, t4, true);
                    }

                  if (accessible)
                  {
                    isRoof[i] = true;
                  }
                  else
                  {
                    accessViaStairs[i] = true;
                  }
                }
            }

            DA.SetData(0, hasAccessible);
            DA.SetData(1, isRoof);
            DA.SetData(2, accessViaStairs);
        }

		#endregion Methods 
    }
}