#region

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Reflection;
using Grasshopper;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using Rhino;
using Rhino.Geometry;
using Rhino.Geometry.Intersect;
using StudioAvw.Voxels.Geometry;
using StudioAvw.Voxels.Param;
using StudioAvw.Voxels.Helper;

#endregion

namespace StudioAvw.Voxels.Components.VoxelGrid
{
    /// <summary>
    ///   Add brep to the voxelgrid
    /// </summary>
    public class VoxelGridVoxelateGeometry : BaseVoxelComponent
    {
        /// <summary>
        ///   Distance used CP methods don to this component, default 0
        /// </summary>
        protected double Distance = 0;

        /// <summary>
        ///   Initializes a new instance of the MyComponent1 class.
        /// </summary>
        public VoxelGridVoxelateGeometry()
          : base("VoxelateGeometry", "VoxGeo",
            "Adds any geometry to a voxelgrid",
            "Voxels", "Create")
        {
        }

        /// <summary>
        ///   Provides an Icon for the component.
        /// </summary>
        protected override Bitmap Icon =>
            //You can add image files to your project resources and access them like this:
            // return Resources.IconForThisComponent;
            Images.VT_VoxelateBrep;

        /// <summary>
        ///   Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid => new Guid("{81c39005-5aac-4d1b-8082-62cd25e0f596}");

        /// <summary>
        ///   Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddGeometryParameter("Geometry", "G", "Geometry to voxelate", GH_ParamAccess.list);
            pManager.AddParameter(new Param_VoxelGrid());
            pManager.AddNumberParameter("Distance", "D", "Maximum distance to geometry", GH_ParamAccess.item, 0);
        }

        /// <summary>
        ///   Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddParameter(new Param_VoxelGrid());
        }

        /// <summary>
        ///   This is the method that actually does the work.
        /// </summary>
        /// <param name="da">The da object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess da)
        {
            //1 divide the brep by the Z components of the grid (get Z values on plane of the grid
            //2 intersect the grid for reach Z value -> planar surface the results
            //3 check for each pixel if it is closer than the 
            var geometryList = new List<IGH_GeometricGoo>();
            var vg = default(VoxelGrid3D);
            da.GetDataList(0, geometryList);
            da.GetData(1, ref vg);
            da.GetData(2, ref Distance);

            // add brep
            if (vg == null || !vg.IsValid)
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "The (input) voxelgrid was invalid");
                return;
            }

            vg = (VoxelGrid3D)vg.Clone();

            foreach (var b in geometryList)
            {
                if (b.CastTo(out Box oBox))
                {
                    AddBox(oBox, ref vg);
                    continue;
                }

                if (b.CastTo(out Brep oBrep))
                {
                    AddBrep(oBrep, ref vg);
                    continue;
                }

                if (b.CastTo(out Mesh oMesh))
                {
                    AddMeshNew(oMesh, ref vg);
                    continue;
                }

                if (b.CastTo(out Surface oSurf))
                {
                    AddBrep(oSurf.ToBrep(), ref vg);
                    continue;
                }

                if (b.CastTo(out Curve oCurve))
                {
                    AddCrv(oCurve, ref vg);
                    continue;
                }

                if (b.CastTo(out Point3d oPt))
                {
                    AddPt(oPt, ref vg);
                    continue;
                }
            }

            AddRenderGrid(vg);
            da.SetData(0, vg);

        }

        /// <summary>
        /// Adds the mesh.
        /// </summary>
        /// <param name="m">The m.</param>
        /// <param name="vg">The vg.</param>
        private void AddMesh(Mesh m, ref VoxelGrid3D vg)
        {
            if (Distance < RhinoMath.ZeroTolerance)
            {
                // add closed mesh
                var bb = m.GetBoundingBox(true);
                var length = bb.Diagonal.Length * 1.1;
                for (var i = 0; i < vg.Count; i++)
                {
                    var pt = vg.EvaluatePoint(i);
                    var isInside = false;
                    if (bb.Contains(pt))
                    {
                        Intersection.MeshLine(m, new Line(pt, Vector3d.XAxis, length), out var faces);
                        if (faces != null)
                        {
                            isInside = faces.Length % 2 == 1;
                        }
                    }
                    if (isInside)
                    {
                        vg.SetValue(i, true);
                    }
                }

            }
            else
            {
                // add open mesh
                var bb = m.GetBoundingBox(true);
                bb.Inflate(Distance * 0.6);
                for (var i = 0; i < vg.Count; i++)
                {
                    var isInside = false;
                    var pt = vg.EvaluatePoint(i);
                    if (bb.Contains(pt))
                    {
                        var foundPoint = m.ClosestPoint(pt);

                        if (foundPoint.DistanceTo(pt) <= Distance)
                        {
                            isInside = true;
                        }
                    }
                    if (isInside)
                    {
                        vg[i] = true;
                    }
                }
            }
        }


        /// <summary>
        /// Adds the mesh.
        /// </summary>
        /// <param name="m">The m.</param>
        /// <param name="vg">The vg.</param>
        private void AddMeshNew(Mesh m, ref VoxelGrid3D vg)
        {
            
            if (Distance < RhinoMath.ZeroTolerance)
            {
                // test in 3 directions.
                var vgX = vg.CloneEmpty();
                var vgY = vg.CloneEmpty();
                var vgZ = vg.CloneEmpty();

                VoxelateMeshInDirection(m, 0, ref vgX);
                VoxelateMeshInDirection(m, 1, ref vgY);
                VoxelateMeshInDirection(m, 2, ref vgZ);
                vgX.And(vgY);
                vgX.And(vgZ);
                vg.Or(vgX);
            }
            else
            {
                // add open mesh
                var bb = m.GetBoundingBox(true);
                bb.Inflate(Distance * 0.6);
                for (var i = 0; i < vg.Count; i++)
                {
                    var isInside = false;
                    var pt = vg.EvaluatePoint(i);
                    if (bb.Contains(pt))
                    {
                        var foundPoint = m.ClosestPoint(pt);

                        if (foundPoint.DistanceTo(pt) <= Distance)
                        {
                            isInside = true;
                        }
                    }
                    if (isInside)
                    {
                        vg[i] = true;
                    }
                }
            }
        }
        private void VoxelateMeshInDirection(Mesh m, int dir, ref VoxelGrid3D vg)
        {
            var box = new Box(vg.Plane, vg.BBox.ToBrep());
            box.Inflate(box.X.Length*0.1, box.Y.Length * 0.1, box.Z.Length * 0.1);
            Plane min;
            Plane max;
            int dirA;
            int dirB;

            switch (dir)
            {
                case 0:
                {
                    dirA = 1;
                    dirB = 2;
                    min = new Plane(box.PointAt(0, 0, 0), vg.Plane.XAxis);
                    max = new Plane(box.PointAt(1, 0, 0), vg.Plane.XAxis);
                    break;
                }
                case 1:
                {
                    dirA = 0;
                    dirB = 2;
                    min = new Plane(box.PointAt(0, 0, 0), vg.Plane.YAxis);
                    max = new Plane(box.PointAt(0, 1, 0), vg.Plane.YAxis);
                        break;
                }
                case 2:
                {
                    dirA = 0;
                    dirB = 1;
                    min = new Plane(box.PointAt(0, 0, 0), vg.Plane.ZAxis);
                    max = new Plane(box.PointAt(0, 0, 1), vg.Plane.ZAxis);
                        break;
                }
                default:
                    throw new Exception("Invalid direction");
            }
            
            for (var a = 0; a < vg.SizeUVW[dirA]; a++)
            {
                for (var b = 0; b < vg.SizeUVW[dirB]; b++)
                {
                    var idx = new Point3i {[dirA] = a, [dirB] = b};
                    var pt = vg.EvaluatePoint(idx);
                    var ptMin = min.ClosestPoint(pt);
                    var ptMax = max.ClosestPoint(pt);
                    var line = new Line(ptMin, ptMax);
                    var pts = Intersection.MeshLine(m, new Line(ptMin, ptMax), out _);
                    // sort by line direction, the points should be in the same order as the line.
                    pts = pts.OrderBy(intPt => line.ClosestParameter(intPt)).ToArray();
                    for (var i = 0; i < pts.Length - 1; i += 2)
                    {
                        AddLine(new Line(pts[i], pts[i + 1]), ref vg);
                    }
                }
            }
        }

        /// <summary>
        /// Add a line in world coordinates to a voxelgrid
        /// </summary>
        /// <param name="line"></param>
        /// <param name="vg"></param>
        private void AddLine(Line line, ref VoxelGrid3D vg)
        {
            var start = vg.ClosestPoint(line.From);
            var end = vg.ClosestPoint(line.To);
            foreach (var pt in InterateLine(start, end))
            {
                if (pt < Point3i.Origin || pt >= vg.SizeUVW) continue;
                vg[pt] = true;
            }
        }
        
        /// <summary>
        /// Based on: http://www.cs.yorku.ca/~amana/research/grid.pdf
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <returns></returns>
        static IEnumerable<Point3i> InterateLine(Point3i start, Point3i end)
        {
            int x1 = end.X, y1 = end.Y, z1 = end.Z, currentU = start.X, currentV = start.Y, currentW = start.Z;
            var dx = Math.Abs(x1 - currentU);
            var dy = Math.Abs(y1 - currentV);
            var dz = Math.Abs(z1 - currentW);
            var stepX = currentU < x1 ? 1 : -1;
            var stepY = currentV < y1 ? 1 : -1;
            var stepZ = currentW < z1 ? 1 : -1;
            var hypotenuse = Math.Sqrt(Math.Pow(dx, 2) + Math.Pow(dy, 2) + Math.Pow(dz, 2));
            var tMaxX = hypotenuse * 0.5 / dx;
            var tMaxY = hypotenuse * 0.5 / dy;
            var tMaxZ = hypotenuse * 0.5 / dz;
            var tDeltaX = hypotenuse / dx;
            var tDeltaY = hypotenuse / dy;
            var tDeltaZ = hypotenuse / dz;
            while (currentU != x1 || currentV != y1 || currentW != z1)
            {
                if (tMaxX < tMaxY)
                {
                    if (tMaxX < tMaxZ)
                    {
                        currentU += stepX;
                        tMaxX += tDeltaX;
                    }
                    else if (tMaxX > tMaxZ)
                    {
                        currentW += stepZ;
                        tMaxZ += tDeltaZ;
                    }
                    else
                    {
                        currentU += stepX;
                        tMaxX += tDeltaX;
                        currentW += stepZ;
                        tMaxZ += tDeltaZ;
                    }
                }
                else if (tMaxX > tMaxY)
                {
                    if (tMaxY < tMaxZ)
                    {
                        currentV += stepY;
                        tMaxY += tDeltaY;
                    }
                    else if (tMaxY > tMaxZ)
                    {
                        currentW += stepZ;
                        tMaxZ += tDeltaZ;
                    }
                    else
                    {
                        currentV += stepY;
                        tMaxY += tDeltaY;
                        currentW += stepZ;
                        tMaxZ += tDeltaZ;
                    }
                }
                else
                {
                    if (tMaxY < tMaxZ)
                    {
                        currentV += stepY;
                        tMaxY += tDeltaY;
                        currentU += stepX;
                        tMaxX += tDeltaX;
                    }
                    else if (tMaxY > tMaxZ)
                    {
                        currentW += stepZ;
                        tMaxZ += tDeltaZ;
                    }
                    else
                    {
                        currentU += stepX;
                        tMaxX += tDeltaX;
                        currentV += stepY;
                        tMaxY += tDeltaY;
                        currentW += stepZ;
                        tMaxZ += tDeltaZ;

                    }
                }
                yield return new Point3i(currentU, currentV, currentW);
            }
        }

        private void AddBox(Box oBox, ref VoxelGrid3D vg)
        {
            var newBox = oBox.BoundingBox;
            for (var i = 0; i < vg.Count; i++)
            {
                var pt = vg.EvaluatePoint(i);
                if (newBox.Contains(pt, false))
                {
                    vg[i] = true;
                }
            }
        }

        /// <summary>
        ///   Add curve that intersects with voxels to the grid
        ///   TODO:
        ///   Improve by dividing the curve?
        ///   Getting more intervals?
        ///   Getting custom distance on the curve?
        /// </summary>
        /// <param name="curve"></param>
        /// <param name="vg"></param>
        public void AddCrv(Curve curve, ref VoxelGrid3D vg)
        {

            // todo: explode curves, and only search in radius of box?
            if (Distance > RhinoMath.ZeroTolerance)
            {
                var bb = curve.GetBoundingBox(true);
                bb.Inflate((vg.VoxelSize.X+Distance) * 2, (vg.VoxelSize.Y + Distance) * 2, (vg.VoxelSize.Z + Distance) * 2);
                var maxDistance =
                    Math.Sqrt(Math.Pow(vg.VoxelSize.X, 2) + Math.Pow(vg.VoxelSize.Y, 2) + Math.Pow(vg.VoxelSize.Z, 2));
                for (var i = 0; i < vg.Count; i++)
                {
                    var pt = vg.EvaluatePoint(i);
                    if (bb.Contains(pt))
                    {
                        curve.ClosestPoint(pt, out var t);
                        var cp = curve.PointAt(t);
                        var dist = cp.DistanceTo(pt);
                        if (Math.Abs(Distance) < RhinoMath.ZeroTolerance && dist <= maxDistance)
                        {
                            AddPt(cp, ref vg);
                        }
                        else if (dist < Distance)
                        {
                            vg[i] = true;
                        }
                    }
                }
            }
            else
            {
                var plc = curve.ToPolyline(0, 0, 0.1, 0, 0.5, RhinoDoc.ActiveDoc.ModelAbsoluteTolerance, 0,
                    vg.VoxelSize.MinimumCoordinate, true);
                plc.TryGetPolyline(out var pl);
                foreach (var item in pl.GetSegments())
                {
                    AddLine(item, ref vg);
                }
            }
        }

        /// <summary>
        ///   Add point to the grid.
        /// </summary>
        /// <param name="pt">Point in world space</param>
        /// <param name="vg">Voxelgrid</param>
        public void AddPt(Point3d pt, ref VoxelGrid3D vg)
        {
            if (Distance < RhinoMath.ZeroTolerance)
            {
                var pti = vg.ClosestPoint(pt);
                if (pti >= Point3i.Origin && pti <= vg.SizeUVW)
                {
                    vg.SetValue(pti, true);
                }
            }
            else
            {
                var bb = new BoundingBox(new []{ pt });
                bb.Inflate(Distance*1.1);
                foreach (var subPt in vg.PointsInBox(bb))
                {
                    if (vg.PointInGrid(subPt) 
                        && !vg[subPt]
                        && vg.EvaluatePoint(subPt).DistanceTo(pt) < Distance)
                    {
                        vg[subPt] = true;
                    }
                }
            }
        }

        /// <summary>
        ///   Add a brep to the voxelgrid
        /// </summary>
        /// <param name="b"></param>
        /// <param name="vg"></param>
        public void AddBrep(Brep b, ref VoxelGrid3D vg)
        {
            if (b == null || b.Equals(default(Brep)) || !b.IsValid)
            {
                return;
            }

            if (!b.IsSolid || Distance > RhinoMath.ZeroTolerance)
            {
                AddOpenBrep(b, ref vg);
            }
            else
            {
                AddSolidBrep(b, ref vg);
            }
        }

        /// <summary>
        ///   Add a non closed brep by using a distance to the grid
        /// </summary>
        /// <param name="b"></param>
        /// <param name="vg"></param>
        public void AddOpenBrep(Brep b, ref VoxelGrid3D vg)
        {
            var bb = b.GetBoundingBox(false);
            bb.Inflate(Distance * 0.6);
            for (var i = 0; i < vg.Grid.Count; i++)
            {
                var pt = vg.EvaluatePoint(i);
                if (!bb.Contains(pt)) continue;

                var cp = b.ClosestPoint(pt);
                AddPt(cp, ref vg);
                if (pt.DistanceTo(cp) < Distance)
                {
                    AddPt(pt, ref vg);
                }
            }
        }
        
        // iterate through the grid in the x directions
        /// <summary>
        ///   Iterate through the grid and add voxels that are inside the Brep
        /// </summary>
        /// <param name="brepVolume"></param>
        /// <param name="vg"></param>
        public void AddSolidBrep(Brep brepVolume, ref VoxelGrid3D vg)
        {
            // get the axis direction for each point
            var pt1 = vg.EvaluatePoint(new Point3i(0, 0, 0));
            var pt2 = vg.EvaluatePoint(new Point3i(1, 0, 0));
            var pln = new Plane(pt1, (pt2 - pt1));

            for (var x = 0; x < vg.SizeUVW.X; x++)
            {
                pln.Origin = vg.EvaluatePoint(new Point3i(x, 0, 0));
                Intersection.BrepPlane(brepVolume, pln, DocumentHelper.GetModelTolerance(), out var sections, out var pts);
                var surfaces = Brep.CreatePlanarBreps(sections);

                // perhaps check first if the points are inside the bounding box of the surface.
                if (surfaces == null) continue;

                for (var y = 0; y < vg.SizeUVW.Y; y++)
                {
                    for (var z = 0; z < vg.SizeUVW.Z; z++)
                    {
                        var pt = vg.EvaluatePoint(new Point3i(x, y, z));
                        var hasPixel = surfaces.Any(t => t.ClosestPoint(pt).DistanceTo(pt) < DocumentHelper.GetModelTolerance());
                        if (hasPixel) {
                            vg.SetValue(new Point3i(x, y, z), true);
                        }
                    }
                }
            }
        }
    }
}