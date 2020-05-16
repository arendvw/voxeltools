using System;
using System.Collections.Generic;
using System.Drawing;
using Rhino.Geometry;
using StudioAvw.Voxels.Geometry;

namespace StudioAvw.Voxels.Tools
{
    /// <summary>
    /// Provides mesh helper classes, to convert voxelgrids to meshes.
    /// </summary>
    public static class BrepHelper
    {
        
        
        /// <summary>
        /// Converts a voxelgrid to a mesh hull.
        /// TODO: Add hashmap vertex id  > pixel/pixelface; to compute the pixel to which the mesh belongs.
        /// </summary>
        /// <param name="vg"></param>
        /// <returns></returns>
        public static Mesh VoxelGridToMesh (VoxelGrid3D vg)
        {
            Vector3d[] FaceDirections;
            Vector3d[] AxisDirections;
            Point3i[] RelativePosition;
            int[,] FaceAxis;
            double[] FaceSizes;

            FaceDirections = new Vector3d[6] { vg.BBox.Plane.XAxis, -vg.BBox.Plane.XAxis, vg.BBox.Plane.YAxis, -vg.BBox.Plane.YAxis, vg.BBox.Plane.ZAxis, -vg.BBox.Plane.ZAxis };
            AxisDirections = new Vector3d[3] { vg.BBox.Plane.XAxis, vg.BBox.Plane.YAxis, vg.BBox.Plane.ZAxis };
            // the different faces
            RelativePosition = new Point3i[6] { new Point3i(1, 0, 0), new Point3i(-1, 0, 0), new Point3i(0, 1, 0), new Point3i(0, -1, 0), new Point3i(0, 0, 1), new Point3i(0, 0, -1) };

            FaceAxis = new int[3, 2] {
              {1,2},
              {2,0},
              {0,1}
              };

            // the sizes of the faces
            FaceSizes = new double[3] { vg.SizeUVW.x, vg.SizeUVW.y, vg.SizeUVW.z };

            var m = new Mesh();
            for (var i = 0; i < vg.Count; i++)
            {
                if (vg[i] == false)
                {
                    continue;
                }

                for (var faceId = 0; faceId < 6; faceId++)
                {
                    if (vg.GetRelativePointValue(i, RelativePosition[faceId]) != 1)
                    {
                        AddFacadeToMesh(vg, i, faceId, ref m, FaceDirections, AxisDirections, FaceAxis);
                    }
                }
            }
            //m.Vertices.CombineIdentical(false, true);
            m.Normals.ComputeNormals();
            m.Compact();
            return m;
        }

        /// <summary>
        /// Gets the voxelgrid mesh by plane
        /// Return a seperate list for each face direction
        /// </summary>
        /// <param name="vg"></param>
        /// <returns></returns>
        public static List<Mesh> VoxelGridToMeshByPlanes(VoxelGrid3D vg)
        {
            Vector3d[] FaceDirections;
            Vector3d[] AxisDirections;
            Point3i[] RelativePosition;
            int[,] FaceAxis;
            double[] FaceSizes;
            var Meshes = new List<Mesh>();
            for (var i = 0; i < 6; i++)
            {
                Meshes.Add(new Mesh());
            }


            FaceDirections = new Vector3d[6] { vg.BBox.Plane.XAxis, -vg.BBox.Plane.XAxis, vg.BBox.Plane.YAxis, -vg.BBox.Plane.YAxis, vg.BBox.Plane.ZAxis, -vg.BBox.Plane.ZAxis };
            AxisDirections = new Vector3d[3] { vg.BBox.Plane.XAxis, vg.BBox.Plane.YAxis, vg.BBox.Plane.ZAxis };
            // the different faces
            RelativePosition = new Point3i[6] { new Point3i(1, 0, 0), new Point3i(-1, 0, 0), new Point3i(0, 1, 0), new Point3i(0, -1, 0), new Point3i(0, 0, 1), new Point3i(0, 0, -1) };

            FaceAxis = new int[3, 2] {
              {1,2},
              {2,0},
              {0,1}
              };

            // the sizes of the faces
            FaceSizes = new double[3] { vg.SizeUVW.x, vg.SizeUVW.y, vg.SizeUVW.z };

            for (var i = 0; i < vg.Count; i++)
            {
                if (vg[i] == false)
                {
                    continue;
                }
                    for (var faceId = 0; faceId < 6; faceId++)
                    {
                        var m = Meshes[faceId];
                        if (vg.GetRelativePointValue(i, RelativePosition[faceId]) != 1)
                        {
                            try
                            {
                                AddFacadeToMesh(vg, i, faceId, ref m, FaceDirections, AxisDirections, FaceAxis);
                            }
                            catch (Exception e)
                            {
                                throw new Exception(
                                    $"Adding Facade {faceId} of Voxel {i} to Mesh failed: {e.ToString()}");
                            }
                        }
                    }
            }
            //m.Vertices.CombineIdentical(false, true);
            for (var i = 0; i < 6; i++)
            {
                Meshes[i].Normals.ComputeNormals();
                Meshes[i].Compact();
            }
            return Meshes;
        }

        /// <summary>
        /// Create a mesh quad from a voxelgrid
        /// </summary>
        /// <param name="vg">VoxelGrid</param>
        /// <param name="i">The voxel number in the grid</param>
        /// <param name="faceId">The id of the face (faces are defined in FaceDirections)</param>
        /// <param name="m">The mesh to add the face to</param>
        /// <param name="FaceDirections">(Predefined) List of directions for each face</param>
        /// <param name="AxisDirections">(Predefined) List of directions of Axis (x,y,z)</param>
        /// <param name="FaceAxis">(Predefined) Define which axis lies on which plane</param>
        public static void AddFacadeToMesh(VoxelGrid3D vg, int i, int faceId, ref Mesh m, Vector3d[] FaceDirections, Vector3d[] AxisDirections, int[,] FaceAxis)
        {
            // 0 for x, 1 for y, 2 for z
            int iDir = Convert.ToInt16(Math.Floor((double)faceId / 2));
            // get the position and the location
            var position = vg.EvaluatePoint(i);

            // to determine the correct position of the plane
            // we have to aim the x or y direction
            var distance = FaceDirections[faceId] * (vg.VoxelSize[iDir] * 0.5);
            position.Transform(Transform.Translation(distance));

            var pln = new Plane(position, AxisDirections[FaceAxis[iDir, 0]], AxisDirections[FaceAxis[iDir, 1]]);

            //planes.Add(pln);
            var sizeU = vg.VoxelSize[FaceAxis[iDir, 0]] / 2;
            var sizeV = vg.VoxelSize[FaceAxis[iDir, 1]] / 2;

            var pts = new Point3d[] {
              new Point3d(-sizeU, -sizeV, 0),
              new Point3d(-sizeU, sizeV, 0),
              new Point3d(sizeU, sizeV, 0),
              new Point3d(sizeU, -sizeV, 0)
              };

            var p3fs = new List<Point3f>();
            foreach (var ptd in pts)
            {
                var worldpt = pln.PointAt(ptd.X, ptd.Y, ptd.Z);
                p3fs.Add(new Point3f((float)worldpt.X, (float)worldpt.Y, (float)worldpt.Z));
            }


            // try to use unique vertices
            var cCount = m.Vertices.Count;
            m.Vertices.AddVertices(p3fs);
            var iFaceIndex = m.Faces.Count;
            var facenormal = Vector3d.CrossProduct(p3fs[1] - p3fs[0], p3fs[2] - p3fs[0]);
            if (Vector3d.VectorAngle(facenormal, FaceDirections[faceId]) > Math.PI / 2)
            {
                m.Faces.AddFace(cCount + 3, cCount + 2, cCount + 1, cCount);
            } else  {
                m.Faces.AddFace(cCount, cCount + 1, cCount + 2, cCount + 3);
            }
            m.FaceNormals.SetFaceNormal(iFaceIndex, FaceDirections[faceId]);
        }

        /// <summary>
        /// Set mesh Vertex colors, based on a (light) vector (fake shadow), based on the angle the face normal has with the sun vector
        /// Does not take in to accound shadows, mainly colors faces with different directions in a different way.
        /// </summary>
        /// <param name="msh"></param>
        /// <param name="sun"></param>
        /// <param name="maxShadow"></param>
        /// <param name="clr"></param>
        /// <param name="shadowClr"></param>
        public static void addFakeShadow(ref Mesh msh, Vector3d sun, double maxShadow, Color clr, Color shadowClr)
        {
              msh.Normals.ComputeNormals();
              msh.FaceNormals.ComputeFaceNormals();
              msh.VertexColors.CreateMonotoneMesh(clr);
              for (var i = 0; i < msh.Faces.Count; i++)
              {
                var shadowPct = (Vector3d.VectorAngle(msh.FaceNormals[i], -sun) / (Math.PI * 2)) * maxShadow;
                var draw = Blend(shadowClr, clr, shadowPct);
                msh.VertexColors[msh.Faces[i].A] = draw;
                msh.VertexColors[msh.Faces[i].B] = draw;
                msh.VertexColors[msh.Faces[i].C] = draw;
                msh.VertexColors[msh.Faces[i].D] = draw;
                //Print(Vector3d.VectorAngle(msh.FaceNormals[i], -sun).ToString());
              }
        }

        /// <summary>
        ///  Blend two colors a x b with (amount*a) and (1-amount)*b
        /// </summary>
        /// <param name="color"></param>
        /// <param name="backColor"></param>
        /// <param name="amount"></param>
        /// <returns></returns>
        public static Color Blend(Color color, Color backColor, double amount)
        {
            var r = (byte)((color.R * amount) + backColor.R * (1 - amount));
            var g = (byte)((color.G * amount) + backColor.G * (1 - amount));
            var b = (byte)((color.B * amount) + backColor.B * (1 - amount));
            return Color.FromArgb(r, g, b);
        }
    }
}
