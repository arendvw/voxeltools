﻿using System;
using System.Collections.Generic;
using System.Drawing;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Parameters;
using Rhino.Geometry;
using StudioAvw.Voxels.Geometry;
using StudioAvw.Voxels.Param;
using StudioAvw.Voxels.Helper;

namespace StudioAvw.Voxels.Components.VoxelGrid
{
    /// <summary>
    /// Create a voxelgrid mesh hull describing the outer hull of the voxel grid
    /// </summary>
    public class VoxelGridMeshHull : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the MyComponent1 class.
        /// </summary>
        public VoxelGridMeshHull()
            : base("VoxelGrid To Mesh Hull", "VoxMeshHull",
                "Generate a mesh of a Voxelgrid",
                "Voxels", "Analysis")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddParameter(new Param_VoxelGrid());
            pManager.AddBooleanParameter("Fake Shadow", "FS", "Add fake shadow to the mesh, it can help to visualize the grid.",
                GH_ParamAccess.item, false);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            //FaceDirections = new Vector3d[6] { vg.BBox.Plane.XAxis, -vg.BBox.Plane.XAxis, vg.BBox.Plane.YAxis, -vg.BBox.Plane.YAxis, vg.BBox.Plane.ZAxis, -vg.BBox.Plane.ZAxis };
            pManager.AddMeshParameter("Mesh", "M", "A Mesh of the hull of the VoxelGrid", GH_ParamAccess.item);
            pManager.AddMeshParameter("+X", "+X", "Meshes with the normal in the X direction", GH_ParamAccess.item);
            pManager.AddMeshParameter("-X", "-X", "Meshes with the normal in the -X direction", GH_ParamAccess.item);
            pManager.AddMeshParameter("+Y", "+Y", "Meshes with the normal in the Y direction", GH_ParamAccess.item);
            pManager.AddMeshParameter("-Y", "-Y", "Meshes with the normal in the -Y direction", GH_ParamAccess.item);
            pManager.AddMeshParameter("+Z", "+Z", "Meshes with the normal in the Z direction", GH_ParamAccess.item);
            pManager.AddMeshParameter("-Z", "-Z", "Meshes with the normal in the -Z direction", GH_ParamAccess.item);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="da">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess da)
        {
            /*
             * todo: select mesh box or simple box
             */
            bool fakeShadow = false;
            var vg = default(VoxelGrid3D);
            da.GetData(0, ref vg);
            da.GetData(1, ref fakeShadow);

            var meshes = new List<Mesh>();
            if (vg == null || !vg.IsValid)
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "The (input) voxelgrid was invalid");
                return;
            }

            var m = VoxelGridMeshHelper.VoxelGridToMesh(vg);
            try
            {
                meshes = VoxelGridMeshHelper.VoxelGridToMeshByPlanes(vg);
            }
            catch (Exception e)
            {
                throw new Exception($"Creating multiple meshes failed: {e.ToString()}");
            }

            if (fakeShadow)
            {
                VoxelGridMeshHelper.addFakeShadow(ref m, new Vector3d(-0.495633, 0.142501, 0.856762), 1.0, Color.White, Color.Black);
            }
            
            da.SetData(0, m);

            if (meshes.Count == 6)
            {
                for (var i = 1; i <= meshes.Count; i++)
                {
                    var mesh = meshes[i - 1];
                    if (fakeShadow)
                    {
                        VoxelGridMeshHelper.addFakeShadow(ref mesh, new Vector3d(-0.495633, 0.142501, 0.856762), 1.0,
                            Color.White, Color.Black);
                    }
                    da.SetData(i, mesh);
                }
            }
            else
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Error,"Getting world planes failed");
            }
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
        public override Guid ComponentGuid => new Guid("{2C2E9FB7-2018-4F5E-BDA9-C56A171709F0}");
    }
}