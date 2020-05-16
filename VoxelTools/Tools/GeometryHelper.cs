using System.Collections.Generic;
using Grasshopper.Kernel.Types;

namespace StudioAvw.Voxels.Tools
{
    class GeometryHelper
    {
        /// <summary>
        /// Find geometry in list of geometric goo that can be cast to a certain object.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="geometry"></param>
        /// <returns></returns>
        public static Dictionary<int,T> TryCastGeometry<T>(Dictionary<int,IGH_GeometricGoo> geometry, bool removeWhenFound)
        {
            var collected = new Dictionary<int,T>();
            foreach (var igg in geometry)
            {
                if (igg.Value.CastTo(out T outputObject))
                {
                    collected.Add(igg.Key,outputObject);
                    if (removeWhenFound)
                    {
                        geometry.Remove(igg.Key);
                    }
                }
            }
            return collected;
        }

        public static Dictionary<int, T> TryCastGeometry<T>(Dictionary<int, IGH_GeometricGoo> geometryIndex)
        {
            return TryCastGeometry<T>(geometryIndex, false);
        }
    }
}
