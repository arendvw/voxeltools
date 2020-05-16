using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using Rhino;
using Rhino.Geometry;
using Rhino.Geometry.Intersect;
using StudioAvw.Voxels.Geometry;
using StudioAvw.Voxels.Param;

namespace StudioAvw.Voxels.Components.ScalarGrid
{
    /// <summary>
    /// Voxelize points to a scalar grid
    /// </summary>
    public class ScalarGridVoxilizeComponent : GH_ScalarComponent
    {
        /// <summary>
        /// Initializes a new instance of the MyComponent1 class.
        /// </summary>
        public ScalarGridVoxilizeComponent()
            : base("Voxelate Geometry into Scalar Grid", "VoxGeoScalar",
                "Adds any geometry to a Scalar Grid",
                "Voxels", "Scalar")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddGeometryParameter("Geometry","G", "Geometry to voxelate", GH_ParamAccess.list);
            pManager.AddParameter(new Param_ScalarGrid());
            pManager.AddNumberParameter("Exponent", "E", "1 = linear, 2=quadratic, 3=cubic", GH_ParamAccess.list, 1);
            pManager.AddNumberParameter("Mass", "M", "Mass of the voxelated object", GH_ParamAccess.list);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddParameter(new Param_ScalarGrid());
        }


        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            try
            {
                //1 divide the brep by the Z components of the grid (get Z values on plane of the grid
                //2 intersect the grid for reach Z value -> planar surface the results
                //3 check for each pixel if it is closer than the 
                var geometryList = new List<IGH_GeometricGoo>();
                var massList = new List<double>();
                var exponentList = new List<double>();


                var vg = default(ScalarGrid3D);
                DA.GetDataList(0, geometryList);
                DA.GetData(1, ref vg);
                DA.GetDataList(2, exponentList);
                DA.GetDataList(3, massList);

                if (exponentList.Count == 0)
                {
                    exponentList.Add(1);
                }

                if (massList.Count == 0)
                {
                    massList.Add(1);
                }

                // add brep
                if (vg == null || !vg.IsValid)
                {
                    AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "The (input) ScalarGrid was invalid");
                    return;
                }


                vg = (ScalarGrid3D)vg.Clone();

                for (var i = 0; i < geometryList.Count; i++)
                {
                    double mass;
                    if (i >= massList.Count)
                    {
                        mass = massList.Last();
                    }
                    else
                    {
                        mass = massList[i];
                    }

                    double exponent;
                    if (i >= exponentList.Count)
                    {
                        exponent = exponentList.Last();
                    }
                    else
                    {
                        exponent = exponentList[i];
                    }

                    var b = geometryList[i];

                    if (b.CastTo(out Box oBox))
                    {
                        AddBox(oBox, exponent, mass, ref vg);
                        continue;
                    }

                    if (b.CastTo(out Brep oBrep))
                    {
                        AddBrep(oBrep, exponent, mass, ref vg);
                        continue;
                    }

                    if (b.CastTo(out Mesh oMesh))
                    {
                        AddMesh(oMesh, exponent, mass, ref vg);
                        continue;
                    }

                    if (b.CastTo(out Surface oSurf))
                    {
                        AddBrep(oSurf.ToBrep(), exponent, mass, ref vg);
                        continue;
                    }

                    if (b.CastTo(out Curve oCurve))
                    {
                        AddCrv(oCurve, exponent, mass, ref vg);
                        continue;
                    }

                    if (b.CastTo(out Point3d oPt))
                    {
                        AddPt(oPt, exponent, mass, ref vg);
                        continue;
                    }
                }

                DA.SetData(0, vg);
            }
            catch (Exception e)
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Error, e.ToString() + e.StackTrace.ToString());
            }
        }

        /// <summary>
        /// Calculate the mass for point at distance d, source mass, and falloff exponent exp
        /// Set a negative value if inside.
        /// </summary>
        /// <param name="dist"></param>
        /// <param name="mass"></param>
        /// <param name="exp"></param>
        /// <param name="isInside"></param>
        /// <returns></returns>
        float CalculateMass(double dist, double mass, double exp, bool isInside)
        {
            if (isInside)
            {
                dist *= -1;
            }
            return CalculateMass(dist, mass, exp);
        }

        /// <summary>
        /// Calculate mass at distance d, with mass m and falloff exponent exp
        /// Solve equation: F = (p1-p2)*mass/(r^exp)
        /// </summary>
        /// <param name="dist"></param>
        /// <param name="mass"></param>
        /// <param name="exp"></param>
        /// <returns></returns>
        float CalculateMass(double dist, double mass, double exp)
        {
            return Convert.ToSingle(mass/Math.Pow(Math.Abs(dist),exp));
        }

        /// <summary>
        /// Add a mesh to the scalar grid
        /// </summary>
        /// <param name="m">Volumetric mesh</param>
        /// <param name="exp">Falloff Exponent</param>
        /// <param name="mass">Mass m</param>
        /// <param name="sg">Scalar grid</param>
        private void AddMesh(Mesh m, double exp, double mass, ref ScalarGrid3D sg)
        {
            if (m.IsClosed)
            {
                // add closed mesh
                var bb = m.GetBoundingBox(true);
                var length = bb.Diagonal.Length * 1.1;
                for (var i = 0; i < sg.Count; i++)
                {
                    var pt = sg.EvaluatePoint(i);
                    var isInside = false;
                    var faces = new int[0];
                    Intersection.MeshLine(m, new Line(pt, Vector3d.XAxis, length), out faces);
                    if (faces != null)
                    {
                        isInside = faces.Length % 2 == 1;
                    }
                    var cp = m.ClosestPoint(pt);
                    sg[i] += CalculateMass(cp.DistanceTo(pt), mass, exp, isInside);
                }
            }
            else
            {
                // add open mesh
                for (var i = 0; i < sg.Count; i++)
                {
                    var faces = new int[0];
                    var isInside = false;
                    var pt = sg.EvaluatePoint(i);
                    var foundPoint = m.ClosestPoint(pt);
                    var dist = foundPoint.DistanceTo(pt);
                    sg[i] += CalculateMass(dist, mass, exp, isInside);
                }
            }
        }

        /// <summary>
        /// Add a box to the scalar grid
        /// </summary>
        /// <param name="oBox"></param>
        /// <param name="exp"></param>
        /// <param name="mass"></param>
        /// <param name="sg"></param>
        private void AddBox(Box oBox, double exp, double mass, ref ScalarGrid3D sg)
        {
            for (var i = 0; i < sg.Count; i++)
            {
                var pt = sg.EvaluatePoint(i);
                var contains = oBox.Contains(pt, false);
                var cp = oBox.ClosestPoint(pt);
                    var distance = pt.DistanceTo(cp);

                    if (contains)
                    {
                        distance *= -1;
                    }
                    sg[i] =+ CalculateMass(Convert.ToSingle(distance), mass, exp);
            }
        }


        /// <summary>
        /// Add a curve to the scalar grid
        /// </summary>
        /// <param name="curve"></param>
        /// <param name="exp"></param>
        /// <param name="mass"></param>
        /// <param name="sg"></param>
        public void AddCrv(Curve curve, double exp, double mass, ref ScalarGrid3D sg)
        {
            var bb = curve.GetBoundingBox(true);
            bb.Inflate(sg.VoxelSize.X * 2, sg.VoxelSize.Y * 2, sg.VoxelSize.Z * 2);
            var maxDistance = Math.Sqrt(Math.Pow(sg.VoxelSize.X, 2) + Math.Pow(sg.VoxelSize.Y, 2) + Math.Pow(sg.VoxelSize.Z, 2));
            for (var i = 0; i < sg.Count; i++)
            {
                var pt = sg.EvaluatePoint(i);
                curve.ClosestPoint(pt, out var t);
                var cp = curve.PointAt(t);
                var dist = cp.DistanceTo(pt);
                sg[i] += CalculateMass(dist, mass, exp);

            }
            
        }

        /// <summary>
        /// Add a point to the scalar grid. Currently an expensive operation
        /// </summary>
        /// <param name="pt"></param>
        /// <param name="exp"></param>
        /// <param name="mass"></param>
        /// <param name="vg"></param>
        public void AddPt(Point3d pt, double exp, double mass, ref ScalarGrid3D vg)
        {
            for (var i = 0; i < vg.Count; i++)
            {
                var dist = vg.EvaluatePoint(i).DistanceTo(pt);
                vg[i] += CalculateMass(dist, mass, exp);
            }
            //
        }

        /// <summary>
        /// Provides an Icon for the component.
        /// </summary>
        protected override Bitmap Icon =>
            //You can add image files to your project resources and access them like this:
            // return Resources.IconForThisComponent;
            Images.ST_VoxelateBrep;

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid => new Guid("{B9F6A6F9-6662-4CE8-BC05-0938F0F123E9}");

        /// <summary>
        /// Add a brep to the scalar grid
        /// </summary>
        /// <param name="b"></param>
        /// <param name="exp"></param>
        /// <param name="mass"></param>
        /// <param name="vg"></param>
        public void AddBrep(Brep b, double exp, double mass, ref ScalarGrid3D vg)
        {
            if (b == null || (b).Equals(default(Brep)) || !b.IsValid)
            {
                return;
            }

            if (!b.IsSolid)
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Brep is not closed, brep will be treated as a surface");
                AddOpenBrep(b, exp, mass, ref vg);
            }
            else
            {
                AddSolidBrep(b, exp, mass, ref vg);
            }
        }

        /// <summary>
        /// Add an open brep (non inside detection)
        /// </summary>
        /// <param name="b"></param>
        /// <param name="exp"></param>
        /// <param name="mass"></param>
        /// <param name="vg"></param>
        public void AddOpenBrep(Brep b, double exp, double mass, ref ScalarGrid3D vg)
        {
            for (var i = 0; i < vg.Count; i++)
            {
                var pt = vg.EvaluatePoint(i);
                var cp = b.ClosestPoint(pt);
                var dist = pt.DistanceTo(cp);
                vg[i] += CalculateMass(dist, mass, exp);
            }
        }


        /// <summary>
        /// Add a solid brep
        /// </summary>
        /// <param name="b"></param>
        /// <param name="exp"></param>
        /// <param name="mass"></param>
        /// <param name="vg"></param>
        public void AddSolidBrep(Brep b, double exp, double mass, ref ScalarGrid3D vg)
        {
            // get the axis direction for each point
            var pt1 = vg.EvaluatePoint(new Point3i(0, 0, 0));
            var pt2 = vg.EvaluatePoint(new Point3i(1, 0, 0));
            var pln = new Plane(pt1, (pt2 - pt1));
            
            // currently we NOT have chosen the multitasking ultra parralel option
            // this should speed up some of the more demanding grids.
            //System.Threading.Tasks.Parallel.For(0, vg.Size.x, x =>
            //{
            for (var x = 0; x < vg.SizeUVW.x; x++)
            {
                pln.Origin = vg.EvaluatePoint(new Point3i(x, 0, 0));
                Intersection.BrepPlane(b, pln, RhinoDoc.ActiveDoc.ModelAbsoluteTolerance, out var sections, out var pts);
                var surfaces = Brep.CreatePlanarBreps(sections);

                
                // perhaps check first if the points are inside the bounding box of the surface.
                if (surfaces != null)
                {
                    for (var y = 0; y < vg.SizeUVW.y; y++)
                    {
                        for (var z = 0; z < vg.SizeUVW.z; z++)
                        {
                            var isInside = false;
                            var pti = new Point3i(x, y, z);
                            var pt = vg.EvaluatePoint(pti);
                            var distance = b.ClosestPoint(pt).DistanceTo(pt);
                            for (var i = 0; i < surfaces.Length; i++)
                            {
                                //BoundingBox bb = surfaces[i].GetBoundingBox(false);
                                if(surfaces[i].ClosestPoint(pt).DistanceTo(pt) < RhinoDoc.ActiveDoc.ModelAbsoluteTolerance)
                                {
                                    isInside = true;
                                    break;
                                }
                            }

                            vg[pti] =+ CalculateMass(distance, mass, exp, isInside);
                        }
                    }
                }
            }
        }
    }
}