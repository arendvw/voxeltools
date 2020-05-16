using System;
using System.Collections.Generic;
using System.Drawing;
using GH_IO.Serialization;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Data;
using Rhino.Geometry;
using StudioAvw.Voxels.Geometry;
using StudioAvw.Voxels.Tools;
using StudioAvw.Voxels.Types;

namespace StudioAvw.Voxels.Param
{
    /// <summary>
    /// The input / output options of a voxelgrid
    /// </summary>
    public class Param_VoxelGrid : GH_PersistentParam<GH_VoxelGrid>, IGH_PreviewObject//, IDisposable
    {
        /// <summary>
        /// VoxelGrid constructor
        /// Defines the name, description and where it is placed in the Voxelgrid
        /// </summary>
        public Param_VoxelGrid()
            : base(new GH_InstanceDescription("VoxelGrid Param", "VG", "Represents a collection of VoxelGrids", "Voxels", "Param"))
        {
        }

        /// <summary>
        /// Guid (unique id)
        /// </summary>
        public override Guid ComponentGuid => new Guid("B68C9F85-6382-42DC-B8FC-48645CDC3FB7");


        /// <summary>
        /// What to when right click: Set Multiple VoxelGrid
        /// Current: Do nothing (cancel)
        /// </summary>
        /// <param name="values"></param>
        /// <returns></returns>
        protected override GH_GetterResult Prompt_Plural(ref List<GH_VoxelGrid> values)
        {
            return GH_GetterResult.cancel;
        }

        /// <summary>
        /// What to do when right click > Set One VoxelGrid, Currently:
        /// Do nothing.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        protected override GH_GetterResult Prompt_Singular(ref GH_VoxelGrid value)
        {
            return GH_GetterResult.cancel;
        }

        /// <summary>
        /// Gets the 24x24 pixel icon
        /// </summary>
        protected override Bitmap Icon => Images.VT_PointCloud;

        /// <summary>
        /// Where to show the component
        /// </summary>
        public override GH_Exposure Exposure => GH_Exposure.primary;


        private List<Line> _lineCache = new List<Line> ();
        private List<Line> _dottedLineCache = new List<Line>();
        private List<Mesh> _meshCache = new List<Mesh>();
        private BoundingBox _bboxCache = BoundingBox.Empty;
        private bool _hidden = false;

        public BoundingBox ClippingBox => _bboxCache;

        public void DrawViewportMeshes(IGH_PreviewArgs args)
        {
            try
            {
                ensureMeshCache();

                foreach (var m in _meshCache)
                {
                    args.Display.DrawMeshFalseColors(m);
                }
            } catch { }

        }

        private bool _hasMeshCache = false;
        private void ensureMeshCache()
        {
            
            if (_hasMeshCache == true) { return; };
            foreach (GH_VoxelGrid vg in m_data.AllData(true))
            {
                var m = VoxelGridMeshHelper.VoxelGridToMesh(vg.Value);
                _bboxCache.Union(vg.Value.BBox.BoundingBox);
                VoxelGridMeshHelper.addFakeShadow(ref m, new Vector3d(-0.495633, 0.142501, 0.856762), 1.0, Color.White, Color.Black);
                _meshCache.Add(m);
            }
            _hasMeshCache = true;
        }

        public void DrawViewportWires(IGH_PreviewArgs args)
        {
            try
            {
                ensureLineCache();
                GH_PreviewWireArgs e;

                if (Attributes.GetTopLevel.Selected)
                {
                    e = new GH_PreviewWireArgs(args.Viewport, args.Display, args.WireColour_Selected, args.DefaultCurveThickness);
                }
                else
                {
                    e = new GH_PreviewWireArgs(args.Viewport, args.Display, args.WireColour, args.DefaultCurveThickness);
                }

                args.Display.DrawLines(_lineCache, e.Color);

                foreach (var l in _dottedLineCache)
                {
                    args.Display.DrawDottedLine(l, e.Color);
                }

                foreach (var m in _meshCache)
                {
                    args.Display.DrawMeshFalseColors(m);
                }

            }
            catch { }
        }

        public bool Hidden
        {
            get => _hidden;
            set => _hidden = value;
        }

        public bool IsPreviewCapable => true;

        /// <summary>
        /// 
        /// </summary>
        private bool _hasLineCache = false;

        /// <summary>
        /// 
        /// </summary>
        private void ensureLineCache ()
        {
            if (_hasLineCache == true) { return; };
            foreach (GH_VoxelGrid grid in m_data.AllData(true))
            {
                RenderHelper.GenerateGridPreviewLines(grid.Value, out var dottedLines, out var lines);
                _dottedLineCache.AddRange(dottedLines);
                _lineCache.AddRange(lines);
                _bboxCache.Union(grid.Value.BBox.BoundingBox);
            }
            _hasLineCache = true;
        }

        /// <summary>
        /// 
        /// </summary>
        public override void ClearData()
        {
            base.ClearData();
            foreach (var m in _meshCache)
            {
                m.Dispose();
            }
            _lineCache = new List<Line> ();
            _dottedLineCache = new List<Line>();
            _meshCache = new List<Mesh>();
            _hasLineCache = false;
            _hasMeshCache = false;
        }


        /*
        public void Dispose()
        {
            foreach (Mesh m in this._meshCache)
            {
                m.Dispose();
            }
        }*/


        public override bool Write(GH_IWriter writer)
        {
            base.Write(writer);

            try {
                writer.RemoveChunk("PersistentData");
                writer.SetInt32("GridCount", PersistentData.DataCount);

                var total = 0;
                for (var i = 0; i < PersistentData.Branches.Count; i++)
                {
                    var path = PersistentData.Paths[i];
                    foreach (var ghvg in PersistentData.Branches[i])
                    {
                        var currentGrid = writer.CreateChunk("Grid", total);
                        currentGrid.SetString("Data", ByteHelper.ByteToHex(ByteHelper.Compress(ByteHelper.ToByte(ghvg.Value))));
                        currentGrid.SetString("Path", path.ToString());
                        total++;
                    }
                }
                return true;
            } catch (Exception e) {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Error, e.ToString() + " " + e.StackTrace.ToString());
                return true;
            }
        }

        public override bool Read(GH_IReader reader)
        {
            
            try {
                base.Read(reader);
                var count = reader.GetInt32("GridCount");
                var data = new GH_Structure<GH_VoxelGrid>();
                for (var i = 0; i < count; i++)
                {
                    var currentGrid = reader.FindChunk("Grid", i);
                    var gridString = currentGrid.GetString("Data");
                    var pathString = currentGrid.GetString("Path");
                    var path = new GH_Path();
                    path.FromString(pathString);
                    var output = ByteHelper.ToVoxelGrid(ByteHelper.Decompress(ByteHelper.StringToByteArray(gridString)));
                    data.Append(new GH_VoxelGrid(output), path);
                }
                SetPersistentData(data);
                return true;
            } catch (Exception e) {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Error, e.ToString() + " " + e.StackTrace.ToString());
                return true;
            }
        }
    }
}
