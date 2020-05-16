using System.Collections.Generic;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Data;
using Grasshopper.Kernel.Types;

namespace StudioAvw.Voxels.Tools
{
    static class DataAccessHelper
    {
        /// <summary>
        /// Fetch data at index position
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="DA"></param>
        /// <param name="position"></param>
        /// <returns></returns>
        public static T Fetch<T>(this IGH_DataAccess DA, int position)
        {
            var temp = default(T);
            DA.GetData(position, ref temp);
            return temp;
        }
        /// <summary>
        /// Fetch data with name
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="DA"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public static T Fetch<T>(this IGH_DataAccess DA, string name)
        {
            var temp = default(T);
            DA.GetData(name, ref temp);
            return temp;
        }

        /// <summary>
        /// Fetch data list with position
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="DA"></param>
        /// <param name="position"></param>
        /// <returns></returns>
        public static List<T> FetchList<T>(this IGH_DataAccess DA, int position)
        {
            var temp = new List<T>();
            DA.GetDataList(position, temp);
            return temp;
        }

        /// <summary>
        /// Fetch data list with name
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="DA"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public static List<T> FetchList<T>(this IGH_DataAccess DA, string name)
        {
            var temp = new List<T> ();
            DA.GetDataList(name, temp);
            return temp;
        }
        /// <summary>
        /// Fetch structure with position
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="DA"></param>
        /// <param name="position"></param>
        /// <returns></returns>
        public static GH_Structure<T> FetchTree<T>(this IGH_DataAccess DA, int position) where T : IGH_Goo
        {
            DA.GetDataTree(position, out GH_Structure<T> temp);
            return temp;
        }

        /// <summary>
        /// Fetch structure with name
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="DA"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public static GH_Structure<T> FetchTree<T>(this IGH_DataAccess DA, string name) where T : IGH_Goo
        {
            DA.GetDataTree(name, out GH_Structure<T> temp);
            return temp;
        }
    }
}
