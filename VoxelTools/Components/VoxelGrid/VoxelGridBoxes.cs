﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Parameters;
using Grasshopper.Kernel.Types;
using Rhino.Geometry;
using StudioAvw.Voxels.Geometry;
using StudioAvw.Voxels.Helper;
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
                "Get a list of boxes from a voxelgrid. Warning: Can severely slow down your computer for large grids.",
                "Voxels", "Analysis")
        {
        }

        public enum SelectionType
        {
            True = 1,
            False = 0,
            All = -1,
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddParameter(new Param_VoxelGrid());
            var param = new Param_Integer();
            DataAccessHelper.AddEnumOptionsToParam<SelectionType>(param);
            param.PersistentData.Append(new GH_Integer(1));
            pManager.AddParameter(new Param_Integer(), "Selection", "S", "0 = all false voxels, 1 = all true voxels (default), -1 = all voxels", GH_ParamAccess.item);
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
        /// <param name="da">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess da)
        {
            /*
             * todo: select mesh box or simple box
             */
            var vg = default(VoxelGrid3D);
            da.GetData(0, ref vg);
            if (vg == null || !vg.IsValid)
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "The (input) voxelgrid was invalid");
                return;
            }

            vg = (VoxelGrid3D)vg.Clone();

            int selectionInt = 0;
            if (!da.GetData(1, ref selectionInt) || selectionInt < -1 || selectionInt > 1)
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Invalid selection given, only -1, 0, or 1 are valid values");
            }

            var type = (SelectionType) selectionInt;

            var boxes = new List<Box>();
            var values = new List<Point3d>();

            switch (type)
            {
                case SelectionType.True:
                    for (var i = 0; i < vg.SizeUVW.SelfProduct(); i++)
                    {
                        if (!vg.GetValue(i)) continue;

                        boxes.Add(vg.CreateBox(i));
                        values.Add(vg.EvaluatePoint(i));
                    }
                    break;
                case SelectionType.False:
                    for (var i = 0; i < vg.SizeUVW.SelfProduct(); i++)
                    {
                        if (vg.GetValue(i)) continue;

                        boxes.Add(vg.CreateBox(i));
                        values.Add(vg.EvaluatePoint(i));
                    }
                    break;
                case SelectionType.All:
                    for (var i = 0; i < vg.SizeUVW.SelfProduct(); i++)
                    {
                        boxes.Add(vg.CreateBox(i));
                        values.Add(vg.EvaluatePoint(i));
                    }
                    break;
                default:
                    // this should never happen, validated above.
                    throw new ArgumentOutOfRangeException();
            }

            da.SetDataList(0, boxes);
            da.SetDataList(1, values);
        }

        /// <summary>
        /// Provides an Icon for the component.
        /// </summary>
        protected override Bitmap Icon =>
            //You can add image files to your project resources and access them like this:
            // return Resources.IconForThisComponent;
            Images.VT_Boxes;

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid => new Guid("{01E3D92A-DD7D-4A20-B754-20186CC5AC8D}");
    }
}
