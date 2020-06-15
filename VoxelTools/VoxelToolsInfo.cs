using System;
using System.Drawing;
using Grasshopper.Kernel;

namespace StudioAvw.Voxels
{

    public class VoxelToolsInfo : GH_AssemblyInfo
    {
        public override string Name => "VoxelTools";

        public override Bitmap Icon =>
            //Return a 24x24 pixel bitmap to represent this GHA library.
            Images.VT_PointCloud;

        public override string Description =>
            //Return a short string describing the purpose of this GHA library.
            "VoxelTools offers lightweight voxel-geometry for grasshopper";

        public override Guid Id => new Guid("43b6cb74-76cb-4047-96fc-e10aea638d3a");

        public override string Version => "1.0.1";

        public override GH_LibraryLicense AssemblyLicense => GH_LibraryLicense.opensource;

        public override string AuthorName =>
            //Return a string identifying you or your company.
            "Arend van Waart";

        public override string AuthorContact => "https://github.com/arendvw";
    }
}
