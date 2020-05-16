// <copyright file="GH_VoxelComponent.cs" company="StudioAvw">
//     Copyright (c) 2013 StudioAvw. All rights reserved
// <author>Arend van Waart</author>
// </copyright>


using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using Grasshopper.Kernel;
using Rhino;
using Rhino.Geometry;
using StudioAvw.Voxels.Geometry;
using StudioAvw.Voxels.Geometry.Interfaces;
using StudioAvw.Voxels.Tools;

namespace StudioAvw.Voxels.Components.VoxelGrid
{
    /// <summary>
    /// Abstract voxelcomponent, implements voxelgrid rendering.
    /// </summary>
    public abstract class GhVoxelComponent : GH_Component
    {
        /// <summary>
        /// Voxel Component constructor, refer to the parent
        /// </summary>
        /// <param name="name"></param>
        /// <param name="nickname"></param>
        /// <param name="description"></param>
        /// <param name="category"></param>
        /// <param name="subCategory"></param>
        public GhVoxelComponent(string name, string nickname, string description, string category, string subCategory) : base(name, nickname, description, category, subCategory)
        {
        }

        /// <summary>
        /// toggle item?
        /// </summary>
        ToolStripMenuItem _toggleItem;

        /// <summary>
        /// Attempt to toggle the preview mode (failed)
        /// </summary>
        /// <param name="menu"></param>
        public override void AppendAdditionalMenuItems(ToolStripDropDown menu)
        {
            _toggleItem = Menu_AppendItem(menu, "Show preview mesh", TogglePreviewMode, Images.VT_CreateGrid, true, true);
        }

        /// <summary>
        /// Toggle to display the mesh
        /// </summary>
        protected bool DisplayMesh = true;

        /// <summary>
        /// Attempt to toggle the preview mode
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TogglePreviewMode(object sender, EventArgs e)
        {
            _toggleItem.Image = Images.VT_PointCloud;
            _toggleItem.Checked = false;
        }


        /// <summary>
        /// List of voxel grids to be rendered
        /// </summary>
        protected List<VoxelGrid3D> RenderGrid = new List<VoxelGrid3D> ();

        /// <summary>
        /// List of lines to be rendered
        /// </summary>
        protected List<Line> RenderLines = new List<Line>();

        /// <summary>
        /// List of dotted lines to be rendered
        /// </summary>
        protected List<Line> RenderDottedLines = new List<Line>();

        /// <summary>
        /// Bounding box of all meshes to be rendered
        /// </summary>
        protected BoundingBox RenderBb;
        /// <summary>
        /// Collection of voxelgrids to be rendered
        /// </summary>
        protected List<Mesh> RenderMesh = new List<Mesh>();

        /// <summary>
        /// Add a grid to the render queue
        /// </summary>
        /// <param name="grid">The grid.</param>
        public void AddRenderGrid(VoxelGrid3D grid)
        {
            RenderGrid.Add(grid);
            RenderBb.Union(grid.BBox.BoundingBox);
            GenerateBoxGrid(grid);
        }

        /// <summary>
        /// Clear data?
        /// </summary>
        public override void ClearData()
        {
            base.ClearData();
            ClearRenderGrid();
        }

        /// <summary>
        /// clear the current render cache
        /// </summary>
        private void ClearRenderGrid()
        {
            RenderGrid.Clear();
            RenderLines.Clear();
            RenderDottedLines.Clear();
            RenderBb = new BoundingBox();
            RenderMesh.Clear();
        }

        /// <summary>
        /// Render the grids
        /// </summary>
        private void GenerateBoxGrids()
        {
            if (RenderLines.Count != 0)
            {
                return;
            }

            foreach (var vg in RenderGrid)
            {
                GenerateBoxGrid(vg);
                // if generate point cloud
            }
        }

        /// <summary>
        /// render the meshes
        /// </summary>
        private void GenerateBoxMeshes()
        {
            if (RenderMesh.Count != 0)
            {
                return;
            }

            foreach (var vg in RenderGrid)
            {
                var m = VoxelGridMeshHelper.VoxelGridToMesh(vg);
                VoxelGridMeshHelper.addFakeShadow(ref m, new Vector3d(-0.495633, 0.142501, 0.856762), 1.0, Color.White, Color.Black);
                RenderMesh.Add(m);
            }
        }



        /// <summary>
        /// Render the box grid
        /// </summary>
        /// <param name="renderGrid">The render grid.</param>
        private void GenerateBoxGrid(IGrid3D renderGrid)
        {
            RenderHelper.GenerateGridPreviewLines(renderGrid, out var dottedLines, out var lines);
            RenderDottedLines.AddRange(dottedLines);
            RenderLines.AddRange(lines);
        }

        /// <summary>
        /// Return a BoundingBox that contains all the geometry you are about to draw.
        /// </summary>
        public override BoundingBox ClippingBox => RenderBb;

        /// <summary>
        /// Draw the preview meshes, draws fake shadow
        /// </summary>
        /// <param name="args">Display data used during preview drawing.</param>
        public override void DrawViewportMeshes(IGH_PreviewArgs args)
        {
          try
          {
            /*
                GH_PreviewWireArgs e;
                if (Attributes.GetTopLevel.Selected)
                {
                    e = new GH_PreviewWireArgs(args.Viewport, args.Display, args.WireColour_Selected, args.DefaultCurveThickness);
                }
                else
                {
                    e = new GH_PreviewWireArgs(args.Viewport, args.Display, args.WireColour, args.DefaultCurveThickness);
                }*/
            GenerateBoxMeshes();

            foreach (var m in RenderMesh)
            {
              args.Display.DrawMeshFalseColors(m);
            }
          }
          catch (Exception e)
          {
            RhinoApp.WriteLine("Error in voxeltools rendering {0}", e.Message);
          }
        }

        /// <summary>
        /// Draw the viewport wires
        /// </summary>
        /// <param name="args"></param>
        public override void DrawViewportWires(IGH_PreviewArgs args)
        {
            try {

                GH_PreviewWireArgs e;
                // ReSharper disable once ConvertIfStatementToConditionalTernaryExpression
                if (Attributes.GetTopLevel.Selected)
                {
                    e = new GH_PreviewWireArgs(args.Viewport, args.Display, args.WireColour_Selected, args.DefaultCurveThickness);
                }
                else
                {
                    e = new GH_PreviewWireArgs(args.Viewport, args.Display, args.WireColour, args.DefaultCurveThickness);
                }

                // only gets executed once
                GenerateBoxGrids();
                RenderVoxelGridWires(args, e);
            } catch {  }
        }


        /// <summary>
        /// Render voxel grid wires
        /// </summary>
        /// <param name="args">The arguments.</param>
        /// <param name="e">The e.</param>
        private void RenderVoxelGridWires(IGH_PreviewArgs args, GH_PreviewWireArgs e)
        {
            args.Display.DrawLines(RenderLines, e.Color);
            foreach (var l in RenderDottedLines)
            {
                args.Display.DrawDottedLine(l, e.Color);
            }
        }

        /// <summary>
        /// Render the voxel grid mesh
        /// </summary>
        /// <param name="m">The m.</param>
        /// <param name="args">The arguments.</param>
        /// <param name="e">The e.</param>
        private void RenderVoxelGridMesh(Mesh m, IGH_PreviewArgs args, GH_PreviewWireArgs e)
        {
            RenderVoxelGridWires(args, e);
            for (var i =0; i< RenderGrid.Count; i++)
            {
                args.Display.DrawMeshWires(RenderMesh[i], e.Color);
            }
        }

        /// <summary>
        /// Explain that we have a preview available
        /// </summary>
        public override bool IsPreviewCapable => true;
    }
}
