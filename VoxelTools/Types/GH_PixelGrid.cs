using System;
using System.Drawing;
using Grasshopper.Kernel.Types;
using Rhino.Geometry;
using StudioAvw.Voxels.Geometry;
using StudioAvw.Voxels.Helper;

namespace StudioAvw.Voxels.Types
{
    /// <summary>
    /// The GH Wrapper for PixelGrid, that provides metadata, and casting support for PixelGrids.
    /// </summary>
    public class GH_PixelGrid : GH_Goo<PixelGrid2D>
    {

        /// <summary>
        /// Constructor: create a new PixelGrid form a previous PixelGrid
        /// </summary>
        /// <param name="sc"></param>
        public GH_PixelGrid(PixelGrid2D sc)
            : base()
        {
            Value = sc;
        }

        /// <summary>
        /// Create an empty / invalid PixelGrid
        /// </summary>
        public GH_PixelGrid()
            : base()
        {
        }

        /// <summary>
        /// Duplicate a PixelGrid
        /// </summary>
        /// <returns></returns>
        public override IGH_Goo Duplicate()
        {
            var duplicate = new GH_PixelGrid();
            duplicate.Value = (PixelGrid2D)Value.Clone();
            return duplicate;

        }

        /// <summary>
        /// Returns true if PixelGrid is Valid
        /// </summary>
        public override bool IsValid => Value.IsValid;

        /// <summary>
        /// Show a string with information on the PixelGrid
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {

            if (Value == null)
            {
                return "Null/void PixelGrid";
            }
            if (!Value.IsValid)
            {
                return $"Invalid PixelGrid [{Value.SizeUV.X},{Value.SizeUV.Y}]";
            }

            var truecount = 0;
            foreach (bool b in Value.Grid)
            {
                if (b)
                {
                    truecount++;
                }
            }

            return $"PixelGrid [{Value.SizeUV.X},{Value.SizeUV.Y}={Value.SizeUV.SelfProduct()} true={truecount}]";
        }

        /// <summary>
        /// Human readable type description
        /// </summary>
        public override string TypeDescription => "PixelGrid";

        /// <summary>
        /// Human Readable TypeName
        /// </summary>
        public override string TypeName => "PixelGrid";

        /// <summary>
        /// Underlying PixelGrid
        /// </summary>
        /// <returns></returns>
        public override object ScriptVariable()
        {
            return Value;
        }

        /// <summary>
        /// This function is called when Grasshopper needs to convert this 
        /// instance of TriStateType into some other type Q.
        /// </summary>
        /// <typeparam name="Q"></typeparam>
        /// <param name="target"></param>
        /// <returns></returns>
        public override bool CastTo<Q>(ref Q target)
        {
            //First, see if Q is similar to the Integer primitive.
            if (typeof(Q).IsAssignableFrom(typeof(string)))
            {
                target = (Q)(object)ToString();
                return true;
            }

            //First, see if Q is similar to the Integer primitive.
            if (typeof(Q).IsAssignableFrom(typeof(PixelGrid2D)))
            {
                object ptr = Value;
                target = (Q)ptr;
                return true;
            }

            if (typeof(Q).IsAssignableFrom(typeof(GH_Mesh)))
            {
                var m = Value.GenerateMesh(Color.Black, Color.White);
                var ghm = new GH_Mesh(m);
                target = (Q)(object)ghm;
                return true;
            }

            //We could choose to also handle casts to Boolean, GH_Boolean, 
            //Double and GH_Number, but this is left as an exercise for the reader.
            return false;
        }

        /// <summary>
        /// This function is called when Grasshopper needs to convert other data 
        /// into TriStateType.
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public override bool CastFrom(object source)
        {
            //Abort immediately on bogus data.
            if (source == null) { return false; }

            //Use the Grasshopper Integer converter. By specifying GH_Conversion.Both 
            //we will get both exact and fuzzy results. You should always try to use the
            //methods available through GH_Convert as they are extensive and consistent.
            if (source.GetType().IsAssignableFrom(typeof(PixelGrid2D)))
            {
                Value = (PixelGrid2D)source;
                return true;
            }


            //We've exhausted the possible conversions, it seems that source
            //cannot be converted into a TriStateType after all.
            return false;
        }
    }
}
