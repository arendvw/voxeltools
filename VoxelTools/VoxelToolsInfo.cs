using Grasshopper.Kernel;

namespace SharpLab.Voxels
{

    public class VoxelToolsInfo : GH_AssemblyInfo
    {
        public override string Name
        {
            get
            {
                return "VoxelTools";
            }
        }
        public override Bitmap Icon
        {
            get
            {
                //Return a 24x24 pixel bitmap to represent this GHA library.
                return Images.VT_PointCloud;
            }
        }
        public override string Description
        {
            get
            {
                //Return a short string describing the purpose of this GHA library.
                return "VoxelTools provide voxelized datastructures and a set of tools to visualize voxel grids.";
            }
        }
        public override Guid Id
        {
            get
            {
                return new Guid("43b6cb74-76cb-4047-96fc-e10aea638d3a");
            }
        }

        public override string AuthorName
        {
            get
            {
                //Return a string identifying you or your company.
                return "Arend van Waart";
            }
        }
        public override string AuthorContact
        {
            get
            {
                //Return a string representing your preferred contact details.
                return "arend@studioavw.nl";
            }
        }
    }
}