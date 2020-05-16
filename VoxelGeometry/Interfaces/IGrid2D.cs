using Rhino.Geometry;

namespace StudioAvw.Voxels.Geometry.Interfaces
{
    
    public interface IGrid2D
    {
        bool IsValid { get; }
        Rectangle3d BBox { get; }
        Plane Plane { get; }
        Point2i SizeUV { get; set; }
        Point2d PixelSize { get; }
        Point3d PointAt(Point2d ptUV);
        Point3d PointAt(Point2i ptUV);
        Point3d PointAt(int iUV);
    }
}
