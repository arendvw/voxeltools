using System;
using System.Collections.Generic;
using System.Drawing;
using Grasshopper.Kernel;
using Rhino.Geometry;
using StudioAvw.Voxels.Helper;
using StudioAvw.Voxels.Types;

namespace StudioAvw.Voxels.Param
{
    /// <summary> 
    /// Scalargrid Param
    /// </summary>
    // ReSharper disable once InconsistentNaming
    public class Param_PixelGrid : GH_PersistentParam<GH_PixelGrid>
    {
        /// <summary>
        /// Provides a gh_param for a scalar grid
        /// </summary>
        public Param_PixelGrid()
            : base(new GH_InstanceDescription("PixelGrid", "PG", "Represents a collection of Pixel Grids", "Voxels", "Parameter"))
        {
            ClearData();
        }


        /// <summary>
        /// Unique id
        /// </summary>
        public override Guid ComponentGuid => new Guid("{713F3FB0-6DF8-4412-BD12-C81614132217}");

        public void DrawViewportMeshes(IGH_PreviewArgs args)
        {
            try
            {
                EnsureMeshCache();

                foreach (var m in _meshCache)
                {
                    args.Display.DrawMeshFalseColors(m);
                }
            }
            catch { }

        }

        public bool Hidden { get; set; } = false;

        public bool IsPreviewCapable => true;

        private List<Mesh> _meshCache;
        private BoundingBox _bboxCache;
        private bool _hasMeshCache = false;
        private void EnsureMeshCache()
        {

            if (_hasMeshCache == true) { return; };
            foreach (var ghGoo in m_data.AllData(true))
            {
                var pg = (GH_PixelGrid) ghGoo;
                var m = pg.Value.GenerateMesh(Color.Black, Color.White);
                _bboxCache.Union(pg.Value.BBox.BoundingBox);
                _meshCache.Add(m);
            }
            _hasMeshCache = true;
        }

        /// <summary>
        /// 
        /// </summary>
        public override void ClearData()
        {
            base.ClearData();
            if (_meshCache != null)
            {
                foreach (var m in _meshCache)
                {
                    m.Dispose();
                }
            }
            _meshCache = new List<Mesh>();
            _bboxCache = BoundingBox.Empty;
            _hasMeshCache = false;
        }

        public BoundingBox ClippingBox => _bboxCache;

        /// <summary>
        /// What to do when right click: set one scalargrid
        /// Current: nothing
        /// </summary>
        /// <param name="values"></param>
        /// <returns></returns>
        protected override GH_GetterResult Prompt_Plural(ref List<GH_PixelGrid> values)
        {
            return GH_GetterResult.cancel;
        }

        /// <summary>
        /// What to do when right click: set one scalargrid
        /// Current: nothing
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        protected override GH_GetterResult Prompt_Singular(ref GH_PixelGrid value)
        {
            return GH_GetterResult.cancel;
        }

        /// <summary>
        /// Icon of a pointcloud
        /// </summary>
        protected override Bitmap Icon => Images.VT_PointCloud;

        /// <summary>
        /// Show in main bar
        /// </summary>
        public override GH_Exposure Exposure => GH_Exposure.hidden;
    }
}
