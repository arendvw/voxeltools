using Grasshopper.Kernel;

namespace StudioAvw.Voxels.Components.ScalarGrid
{
    /// <summary>
    /// Provides abstract functionality for all Scalar comonents
    /// Currently only extends GH_Component
    /// </summary>
    public abstract class GH_ScalarComponent : GH_Component
    {
        // just pass it along
        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="name"></param>
        /// <param name="nickname"></param>
        /// <param name="description"></param>
        /// <param name="category"></param>
        /// <param name="subCategory"></param>
        public GH_ScalarComponent(string name, string nickname, string description, string category, string subCategory) : base(name, nickname, description, category, subCategory)
        {
        }
    }
}
