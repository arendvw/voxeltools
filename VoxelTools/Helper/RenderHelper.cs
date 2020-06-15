// -----------------------------------------------------------------------
// <copyright file="Class1.cs" company="">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

using System.Collections.Generic;
using Rhino.Geometry;
using StudioAvw.Voxels.Geometry.Interfaces;

namespace StudioAvw.Voxels.Helper
{
    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public static class RenderHelper
    {
        public static void GenerateGridPreviewLines(IGrid3D renderGrid, out List<Line> renderDottedLines, out List<Line> renderLines)
        {
            renderDottedLines = new List<Line> ();
            renderLines = new List<Line> ();

            for (var k = 0; k <= renderGrid.SizeUVW.X; k++)
            {
                var pt1 = renderGrid.EvaluatePoint(new Point3d(k-0.5, -0.5, -0.5));
                var pt2 = renderGrid.EvaluatePoint(new Point3d(k-0.5, renderGrid.SizeUVW.Y - 0.5, -0.5));
                var l = new Line(pt1, pt2);
                renderLines.Add(l);

            }

            for (var m = 0; m <= renderGrid.SizeUVW.Y; m++)
            {
                var pt1 = renderGrid.EvaluatePoint(new Point3d(-0.5, m-0.5, -0.5));
                var pt2 = renderGrid.EvaluatePoint(new Point3d(renderGrid.SizeUVW.X-0.5, m-0.5, -0.5));
                var l = new Line(pt1, pt2);
                renderLines.Add(l);
            }

            // x = 0, y =0, z = 1
            {
                var pt1 = renderGrid.EvaluatePoint(new Point3d(-0.5, -0.5, -0.5));
                var pt2 = renderGrid.EvaluatePoint(new Point3d(-0.5, -0.5, renderGrid.SizeUVW.Z-0.5));
                var l = new Line(pt1, pt2);
                renderLines.Add(l);
            }

            // x = 1, y =0, z = 1
            {
                var pt1 = renderGrid.EvaluatePoint(new Point3d(renderGrid.SizeUVW.X-0.5, -0.5, -0.5));
                var pt2 = renderGrid.EvaluatePoint(new Point3d(renderGrid.SizeUVW.X-0.5, -0.5, renderGrid.SizeUVW.Z-0.5));
                var l = new Line(pt1, pt2);
                renderLines.Add(l);
            }

            // x = 0, y =0, z = 1
            {
                var pt1 = renderGrid.EvaluatePoint(new Point3d(renderGrid.SizeUVW.X-0.5, renderGrid.SizeUVW.Y-0.5, 0-0.5));
                var pt2 = renderGrid.EvaluatePoint(new Point3d(renderGrid.SizeUVW.X-0.5, renderGrid.SizeUVW.Y-0.5, renderGrid.SizeUVW.Z-0.5));
                var l = new Line(pt1, pt2);
                renderLines.Add(l);
            }


            // x = 0, y =1, z = 1
            {
                var pt1 = renderGrid.EvaluatePoint(new Point3d(-0.5, renderGrid.SizeUVW.Y - 0.5, -0.5));
                var pt2 = renderGrid.EvaluatePoint(new Point3d(- 0.5, renderGrid.SizeUVW.Y - 0.5, renderGrid.SizeUVW.Z - 0.5));
                var l = new Line(pt1, pt2);
                renderLines.Add(l);
            }

            // top curves
            // x = 0, y =1, z = 1
            {
                var pt1 = renderGrid.EvaluatePoint(new Point3d(-0.5, renderGrid.SizeUVW.Y - 0.5, renderGrid.SizeUVW.Z - 0.5));
                var pt2 = renderGrid.EvaluatePoint(new Point3d(renderGrid.SizeUVW.X - 0.5, renderGrid.SizeUVW.Y - 0.5, renderGrid.SizeUVW.Z - 0.5));
                var l = new Line(pt1, pt2);
                renderLines.Add(l);
            }


            // top curves
            // x = 0, y =1, z = 1
            {
                var pt1 = renderGrid.EvaluatePoint(new Point3d(renderGrid.SizeUVW.X - 0.5, -0.5, renderGrid.SizeUVW.Z - 0.5));
                var pt2 = renderGrid.EvaluatePoint(new Point3d(renderGrid.SizeUVW.X - 0.5, renderGrid.SizeUVW.Y - 0.5, renderGrid.SizeUVW.Z - 0.5));
                var l = new Line(pt1, pt2);
                renderLines.Add(l);
            }

            // top curves
            // x = 0, y =1, z = 1
            {
                var pt1 = renderGrid.EvaluatePoint(new Point3d(-0.5, -0.5, renderGrid.SizeUVW.Z - 0.5));
                var pt2 = renderGrid.EvaluatePoint(new Point3d(-0.5, renderGrid.SizeUVW.Y - 0.5, renderGrid.SizeUVW.Z - 0.5));
                var l = new Line(pt1, pt2); 
                renderLines.Add(l);
            }


            // top curves
            // x = 0, y =1, z = 1
            {
                var pt1 = renderGrid.EvaluatePoint(new Point3d(-0.5, -0.5, renderGrid.SizeUVW.Z - 0.5));
                var pt2 = renderGrid.EvaluatePoint(new Point3d(renderGrid.SizeUVW.X - 0.5, -0.5
                    , renderGrid.SizeUVW.Z - 0.5));
                var l = new Line(pt1, pt2);
                renderLines.Add(l);
            }
        }
    }
}
