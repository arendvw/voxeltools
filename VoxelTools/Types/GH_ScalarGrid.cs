using System;
using Grasshopper.Kernel.Types;
using StudioAvw.Voxels.Geometry;

namespace StudioAvw.Voxels.Types
{
    /// <summary>
    /// The GH Wrapper for ScalarGrid, that provides metadata, and casting support for scalar grids.
    /// </summary>
    public class GH_ScalarGrid : GH_Goo<ScalarGrid3D>
    {

        /// <summary>
        /// Constructor: create a new Scalar Grid form a previous scalar grid
        /// </summary>
        /// <param name="sc"></param>
        public GH_ScalarGrid(ScalarGrid3D sc)
            : base()
        {
            Value = sc;
        }

        /// <summary>
        /// Create an empty / invalid scalar grid
        /// </summary>
        public GH_ScalarGrid()
            : base()
        {
        }

        /// <summary>
        /// Duplicate a scalar grid
        /// </summary>
        /// <returns></returns>
        public override IGH_Goo Duplicate()
        {
            var duplicate = new GH_ScalarGrid();
            duplicate.Value = (ScalarGrid3D) Value.Clone();
            return duplicate;

        }

        /// <summary>
        /// Returns true if ScalarGrid is Valid
        /// </summary>
        public override bool IsValid => Value.IsValid;

        /// <summary>
        /// Show a string with information on the VoxelGrid
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            if (Value == null)
            {
                return "Null/void Voxelgrid";
            }
            if (!Value.IsValid)
            {
                return $"Invalid ScalarGrid [{Value.SizeUVW.X},{Value.SizeUVW.Y},{Value.SizeUVW.Z}]";
            }
            return $"ScalarGrid [{Value.SizeUVW.X},{Value.SizeUVW.Y},{Value.SizeUVW.Z}={Value.SizeUVW.SelfProduct()}]";
            
            /*
            return String.Format("VoxelGrid [{0},{1},{2}={3}] with Cell Size [{4.0},{5.0},{6.0}] with {7} Voxels",
                Value.Size.x, Value.Size.y, Value.Size.z, Value.Size.selfProduct(),
                Value.VoxelSize.X, Value.VoxelSize.Y, Value.VoxelSize.Z,
                Value.VoxelCount()
                );*/
        }

        /// <summary>
        /// Human readable type description
        /// </summary>
        public override string TypeDescription => "ScalarGrid";

        /// <summary>
        /// Human Readable TypeName
        /// </summary>
        public override string TypeName => "ScalarGrid";

        /// <summary>
        /// Underlying ScalarGrid
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
            if (typeof(Q).IsAssignableFrom(typeof(ScalarGrid3D)))
            {
                object ptr = Value;
                target = (Q)ptr;
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
            if (source.GetType().IsAssignableFrom(typeof(ScalarGrid3D)))
            {
                Value = (ScalarGrid3D)source;
                return true;
            }

            //We've exhausted the possible conversions, it seems that source
            //cannot be converted into a TriStateType after all.
            return false;
        }
    }
}
