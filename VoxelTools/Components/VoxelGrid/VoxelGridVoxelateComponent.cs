#region

using System;
using System.Collections.Generic;
using System.Drawing;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using Rhino;
using Rhino.Geometry;
using Rhino.Geometry.Intersect;
using StudioAvw.Voxels.Geometry;
using StudioAvw.Voxels.Param;

#endregion

namespace StudioAvw.Voxels.Components.VoxelGrid
{
  /// <summary>
  ///   Add brep to the voxelgrid
  /// </summary>
  public class VoxelGridVoxelateComponent : GhVoxelComponent
  {
    /// <summary>
    ///   Distance used CP methods don to this component, default 0
    /// </summary>
    protected double Distance = 0;

    /// <summary>
    ///   Initializes a new instance of the MyComponent1 class.
    /// </summary>
    public VoxelGridVoxelateComponent()
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

      vg = (VoxelGrid3D) vg.Clone();

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
          AddSolidMesh(oMesh, ref vg);
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
      if (m.IsClosed)
      {
        // add closed mesh
        var bb = m.GetBoundingBox(true);
        var length = bb.Diagonal.Length*1.1;
        for (var i = 0; i < vg.Count; i++)
        {
          var pt = vg.EvaluatePoint(i);
          var isInside = false;
          if (bb.Contains(pt))
          {
              Intersection.MeshLine(m, new Line(pt, Vector3d.XAxis, length), out var faces);
            if (faces != null)
            {
              isInside = faces.Length%2 == 1;
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
        bb.Inflate(Distance*0.6);
        for (var i = 0; i < vg.Count; i++)
        {
          var faces = new int[0];
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
      var bb = curve.GetBoundingBox(true);
      bb.Inflate(vg.VoxelSize.X*2, vg.VoxelSize.Y*2, vg.VoxelSize.Z*2);
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
          if (Distance == 0 && dist <= maxDistance)
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

    /// <summary>
    ///   Add point to the grid.
    /// </summary>
    /// <param name="pt">Point in world space</param>
    /// <param name="vg">Voxelgrid</param>
    public void AddPt(Point3d pt, ref VoxelGrid3D vg)
    {
      var pti = vg.ClosestPoint(pt);
      if (new Point3i(0, 0, 0) > pti || pti >= vg.SizeUVW)
      {
        return;
      }

      vg.SetValue(pti, true);
    }

    /// <summary>
    ///   Add a brep to the voxelgrid
    /// </summary>
    /// <param name="b"></param>
    /// <param name="vg"></param>
    public void AddBrep(Brep b, ref VoxelGrid3D vg)
    {
      if (b == null || (b).Equals(default(Brep)) || !b.IsValid)
      {
        return;
      }

      if (!b.IsSolid)
      {
        AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Brep is not closed, brep will be treated as a surface");
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
      bb.Inflate(Distance*0.6);
      for (var i = 0; i < vg.Grid.Count; i++)
      {
        var pt = vg.EvaluatePoint(i);
        if (bb.Contains(pt))
        {
          var cp = b.ClosestPoint(pt);
          AddPt(cp, ref vg);
        }
      }
    }

    // iterate through the grid in the x directions
    /// <summary>
    ///   Iterate through the grid and add voxels that are inside the Brep
    /// </summary>
    /// <param name="mesh"></param>
    /// <param name="vg"></param>
    public void AddSolidMesh(Mesh mesh, ref VoxelGrid3D vg) {
      // get the axis direction for each point
      var pt1 = vg.EvaluatePoint(new Point3i(0, 0, 0));
      var pt2 = vg.EvaluatePoint(new Point3i(1, 0, 0));
      var pln = new Plane(pt1, (pt2 - pt1));

      // currently we have chosen the multitasking utra parralel option
      // this should speed up some of the more demanding grids.
      //System.Threading.Tasks.Parallel.For(0, vg.Size.x, x =>
      //{
      for (var x = 0; x < vg.SizeUVW.x; x++) {
        pln.Origin = vg.EvaluatePoint(new Point3i(x, 0, 0));
        var polylines = Intersection.MeshPlane(mesh, pln);
        if (polylines == null) {  continue; }
        foreach (var pl in polylines)
        {
          if (pl.IsClosedWithinTolerance(RhinoDoc.ActiveDoc.ModelAbsoluteTolerance))
          {
            var plc = new PolylineCurve(pl);
            foreach (var ptUvw in vg.PointsInBox(pl.BoundingBox))
            {
              var containment = plc.Contains(vg.EvaluatePoint(ptUvw));
              if (containment == PointContainment.Inside || containment == PointContainment.Coincident)
              {
                vg[ptUvw] = true;
              }
            }
          }
          else
          {
            AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "Mesh intersection was not closed, applying CP method");
          }
        }
        // perhaps check first if the points are inside the bounding box of the surface.
        
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

      // currently we have chosen the multitasking utra parralel option
      // this should speed up some of the more demanding grids.
      //System.Threading.Tasks.Parallel.For(0, vg.Size.x, x =>
      //{
      for (var x = 0; x < vg.SizeUVW.x; x++)
      {
        pln.Origin = vg.EvaluatePoint(new Point3i(x, 0, 0));
        Intersection.BrepPlane(brepVolume, pln, RhinoDoc.ActiveDoc.ModelAbsoluteTolerance, out var sections, out var pts);
        var surfaces = Brep.CreatePlanarBreps(sections);

        // perhaps check first if the points are inside the bounding box of the surface.
        if (surfaces != null)
        {
          for (var y = 0; y < vg.SizeUVW.y; y++)
          {
            for (var z = 0; z < vg.SizeUVW.z; z++)
            {
              var hasPixel = false;
              var pt = vg.EvaluatePoint(new Point3i(x, y, z));
              for (var i = 0; i < surfaces.Length; i++)
              {
                //BoundingBox bb = surfaces[i].GetBoundingBox(false);

                //bb.Inflate(vg.VoxelSize.X * 2, vg.VoxelSize.Y * 2, vg.VoxelSize.Z * 2);
                if (surfaces[i].ClosestPoint(pt).DistanceTo(pt) < RhinoDoc.ActiveDoc.ModelAbsoluteTolerance)
                {
                  hasPixel = true;
                  break;
                }
              }

              if (hasPixel == true)
              {
                //lock (vg.locker)
                //{
                vg.SetValue(new Point3i(x, y, z), true);
                //}
              }
            }
          }
        }
      }
    }
  }
}