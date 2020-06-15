using System;
using System.Collections.Generic;
using System.Drawing;
using Grasshopper.Kernel;
using Rhino.Display;
using Rhino.Geometry;
using StudioAvw.Voxels.Geometry;
using StudioAvw.Voxels.Param;
using StudioAvw.Voxels.Helper;

namespace StudioAvw.Voxels.Components.ScalarGrid
{
    /// <summary>
    /// Get the hull of a scalar grid using marching cubes (YAY BLOBS)
    /// </summary>
    public class ScalarGridVisualizeComponent : BaseScalarComponent
    {
        /// <summary>
        /// Initializes a new instance of the VoxelGridIntersect class.
        /// </summary>
        public ScalarGridVisualizeComponent()
            : base("Visualize Scalar Grid", "ScalarGridVis",
                "Show the values of a scalar grid in 3d",
                "Voxels", "Scalar")
        {
        }


        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddParameter(new Param_ScalarGrid(), "Grids", "G", "The grid render each node's value", GH_ParamAccess.item);
            //pManager.AddNumberParameter("Size", "S", "Text size for labels", GH_ParamAccess.item);
            pManager.AddTextParameter("Format", "F", "Format number notation", GH_ParamAccess.item, "{0:0.00}");
        }



        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            //pManager.AddMeshParameter("Mesh", "M", "Marching cube output", GH_ParamAccess.item);
            //pManager.AddIntegerParameter("cubeindex", "ci", "bla", GH_ParamAccess.list);
        }

        private readonly List<ScalarGrid3D> _renderBuffer = new List<ScalarGrid3D>();
        private string _renderFormat;

        public override bool IsPreviewCapable => true;

        protected override void BeforeSolveInstance()
        {

            _renderFormat = null;
            base.BeforeSolveInstance();
            _renderBuffer.Clear();
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {

            var sc = ComponentHelper.FetchData<ScalarGrid3D>(0, DA);
            var text = ComponentHelper.FetchData<string>(1, DA);

            _renderBuffer.Add(sc);
            _renderFormat = text;
        }

        BoundingBox _clippingBox = BoundingBox.Unset;

        public override BoundingBox ClippingBox
        {
            get
            {
                if (_clippingBox.Equals(BoundingBox.Unset))
                {
                    var bb = BoundingBox.Empty;
                    foreach (var sc in _renderBuffer)
                    {
                        bb.Union(sc.BBox.BoundingBox);
                    }
                    _clippingBox = bb;
                }
                return _clippingBox;
            }
        }

        public override void DrawViewportWires(IGH_PreviewArgs args)
        {
            foreach (var sc in _renderBuffer)
            {
                for (var i = 0; i < sc.SizeUVW.SelfProduct(); i++)
                {
                    var pt = sc.EvaluatePoint(i);
                    args.Display.DrawPoint(pt, PointStyle.X, 10, args.WireColour);
                    args.Display.Draw2dText(string.Format(_renderFormat, sc.GetValue(i)), args.WireColour, pt, false);
                }
            }

        }


        /// <summary>
        /// Provides an Icon for the component.
        /// </summary>
        protected override Bitmap Icon =>
            Images.ST_ToList;

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid => new Guid("{0033E73A-AF86-42D6-94BC-87FC1F5B7F19}");
    }
}