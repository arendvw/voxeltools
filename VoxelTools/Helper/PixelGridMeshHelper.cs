using System;
using System.Collections.Generic;
using System.Drawing;
using Rhino.Geometry;
using StudioAvw.Voxels.Geometry;

namespace StudioAvw.Voxels.Helper
{
    public static class PixelGridMeshHelper
    {
        public static Mesh GenerateMesh(this PixelGrid2D pixelGrid, Color trueColor, Color falseColor)
        {
            var m = new Mesh();
            var sizeU = pixelGrid.PixelSize[0]/2;
            var sizeV = pixelGrid.PixelSize[1]/2;

            var pts = new Point3d[] {
                  new Point3d(-sizeU, -sizeV, 0),
                  new Point3d(-sizeU, sizeV, 0),
                  new Point3d(sizeU, sizeV, 0),
                  new Point3d(sizeU, -sizeV, 0)
                  };

            var pln = pixelGrid.Plane;
            
            for (var i = 0; i < pixelGrid.Count; i++)
            {
                var pt = pixelGrid.PointAt(i);
                pln.Origin = pt;

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
                if (Vector3d.VectorAngle(facenormal, pln.Normal) > Math.PI / 2)
                {
                    m.Faces.AddFace(cCount + 3, cCount + 2, cCount + 1, cCount);
                }
                else
                {
                    m.Faces.AddFace(cCount, cCount + 1, cCount + 2, cCount + 3);
                }

                if (pixelGrid[i] == true)
                {
                    m.VertexColors.Add(trueColor);  m.VertexColors.Add(trueColor);  m.VertexColors.Add(trueColor);  m.VertexColors.Add(trueColor);
                }
                else
                {
                    m.VertexColors.Add(falseColor);  m.VertexColors.Add(falseColor);  m.VertexColors.Add(falseColor); m.VertexColors.Add(falseColor);
                }
                m.FaceNormals.SetFaceNormal(iFaceIndex, pln.Normal);
                m.Normals.ComputeNormals();
                m.Compact();
            }
            return m;
        }
    }
}
