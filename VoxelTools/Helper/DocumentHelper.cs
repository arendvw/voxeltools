using Rhino;

namespace StudioAvw.Voxels.Helper
{
    public static class DocumentHelper
    {
        /// <summary>
        /// Safely get model tolerance -- use the active RhinoDoc where present, otherwise return a default.
        /// </summary>
        /// <returns></returns>
        public static double GetModelTolerance()
        {
            return RhinoDoc.ActiveDoc?.ModelAbsoluteTolerance ?? 0.01;
        }
    }
}
