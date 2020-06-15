using System.Collections.Generic;
using Grasshopper.Kernel;

namespace StudioAvw.Voxels.Helper
{
    class ComponentHelper
    {
        ///
        public static T FetchData<T>(int position, IGH_DataAccess DA)
        {
            var temp = default(T);
            DA.GetData(position, ref temp);
            return temp;
        }

        public static T FetchData<T>(string position, IGH_DataAccess DA)
        {
            var temp = default(T);
            DA.GetData(position, ref temp);
            return temp;
        }

        public static List<T> FetchDataList<T>(int position, IGH_DataAccess DA)
        {
            var temp = new List<T>();
            DA.GetDataList(position, temp);
            return temp;
        }

        public static List<T> FetchDataList<T>(string position, IGH_DataAccess DA)
        {
            var temp = new List<T> ();
            DA.GetDataList(position, temp);
            return temp;
        }
    }
}
