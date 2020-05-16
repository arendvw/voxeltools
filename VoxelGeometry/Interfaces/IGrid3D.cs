using Rhino.Geometry;

namespace StudioAvw.Voxels.Geometry.Interfaces
{
    
    public interface IGrid3D
    {
        bool IsValid { get; }

        Box BBox { get; }
        Plane Plane { get; }
        Point3i SizeUVW { get; set; }

        Point3d VoxelSize { get; }
        Point3d EvaluatePoint(Point3d ptUVW);
        Point3d EvaluatePoint(Point3i ptUvw);
        Point3d EvaluatePoint(int iUvw);
    }
}
