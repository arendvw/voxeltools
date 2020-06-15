using System;
using System.Collections.Generic;
using System.Drawing;
using Grasshopper.Kernel;
using StudioAvw.Voxels.Types;

namespace StudioAvw.Voxels.Param
{
    /// <summary> 
    /// Scalargrid Param
    /// </summary>
    public class Param_ScalarGrid : GH_PersistentParam<GH_ScalarGrid>
    {
        /// <summary>
        /// Provides a gh_param for a scalar grid
        /// </summary>
        public Param_ScalarGrid()
            : base(new GH_InstanceDescription("ScalarGrid", "SG", "Represents a collection of Scalar Grids", "Voxels", "Param"))
        {
        }


        /// <summary>
        /// Unique id
        /// </summary>
        public override Guid ComponentGuid => new Guid("{A09F682D-FF3B-49ED-923C-676B8F961FF7}");


        /// <summary>
        /// What to do when right click: set one scalargrid
        /// Current: nothing
        /// </summary>
        /// <param name="values"></param>
        /// <returns></returns>
        protected override GH_GetterResult Prompt_Plural(ref List<GH_ScalarGrid> values)
        {
            return GH_GetterResult.cancel;
        }

        /// <summary>
        /// What to do when right click: set one scalargrid
        /// Current: nothing
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        protected override GH_GetterResult Prompt_Singular(ref GH_ScalarGrid value)
        {
            return GH_GetterResult.cancel;
        }

        /// <summary>
        /// Icon of a pointcloud
        /// </summary>
        protected override Bitmap Icon => Images.ST_PointCloud;

        /// <summary>
        /// Show in main bar
        /// </summary>
        public override GH_Exposure Exposure => GH_Exposure.primary;
    }
}
