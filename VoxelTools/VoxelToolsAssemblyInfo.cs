using System;
using System.Drawing;
using Grasshopper.Kernel;

namespace StudioAvw.Voxels
{
    public class VoxelToolsAssemblyInfo : GH_AssemblyInfo
    {
        public override string Description => "VoxelTools offers lightweight voxel-geometry for grasshopper";

        public override Bitmap Icon => Images.VT_VoxelateBrep;

        public override string Name => "VoxelTools";

        public override string Version => "1.0.13";

        public override Guid Id => new Guid("{ca4510da-6cec-401c-8964-18aa571f5e9c}");

        public override string AuthorName => "Arend van Waart";

        public override string AuthorContact => "https://github.com/arendvw";
    }

}
