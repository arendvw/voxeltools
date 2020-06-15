#region

using System;
using System.Drawing;
using Grasshopper.Kernel;
using StudioAvw.Voxels.Geometry;
using StudioAvw.Voxels.Param;
using StudioAvw.Voxels.Helper;

#endregion

namespace StudioAvw.Voxels.Components.VoxelGrid
{
  /// <summary>
  /// Convert voxelgrid to hexadecimal number
  /// </summary>
  public class VoxelGridHex : BaseVoxelComponent
  {
    /// <summary>
    /// Initializes a new instance of the VoxelGridIntersect class.
    /// </summary>
    /// <exclude />
    public VoxelGridHex()
      : base("VoxelGrid To Hex String", "VGToHex",
        "Convert a voxelgrid to a hexadecimal string",
        "Voxels", "Input/Output")
    {
    }

    /// <summary>
    /// Provides an Icon for the component.
    /// </summary>
    protected override Bitmap Icon =>
        //You can add image files to your project resources and access them like this:
        // return Resources.IconForThisComponent;
        Images.VT_GridToHex;

    /// <summary>
    ///   Gets the unique ID for this component. Do not change this ID after release.
    /// </summary>
    public override Guid ComponentGuid => new Guid("{1338ADC7-CF4C-4A59-8F25-73031DAEF4A2}");

    /// <summary>
    /// Registers all the input parameters for this component.
    /// </summary>
    /// <param name="pManager">Use the pManager to register new parameters. pManager is never null.</param>
    protected override void RegisterInputParams(GH_InputParamManager pManager)
    {
      pManager.AddParameter(new Param_VoxelGrid(), "Grids", "G", "The grids for the intersection operations",
        GH_ParamAccess.item);
    }

    /// <summary>
    /// Registers all the output parameters for this component.
    /// </summary>
    /// <param name="pManager">Use the pManager to register new parameters. pManager is never null.</param>
    protected override void RegisterOutputParams(GH_OutputParamManager pManager)
    {
      pManager.AddTextParameter("Hex", "H", "Hexadecimal string describing the grid", GH_ParamAccess.item);
    }

    /// <summary>
    /// This is the method that actually does the work.
    /// </summary>
    /// <param name="da">The da object is used to retrieve from inputs and store in outputs.</param>
    protected override void SolveInstance(IGH_DataAccess da)
    {
      var vg = default(VoxelGrid3D);
      da.GetData(0, ref vg);
      var output = ByteHelper.ByteToHex(ByteHelper.Compress(ByteHelper.ToByte(vg)));
      da.SetData(0, output);
    }
  }
}