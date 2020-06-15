using System;
using System.Drawing;
using Grasshopper.Kernel.Types;
using Rhino.Geometry;
using StudioAvw.Voxels.Geometry;
using StudioAvw.Voxels.Helper;

namespace StudioAvw.Voxels.Types
{
    /// <summary>
    /// The GH_VoxelGrid data type. Takes care of casting from and to voxelgrids.
    /// </summary>
    public class GH_VoxelGrid : GH_Goo<VoxelGrid3D>
    {

        public GH_VoxelGrid()  { }
        public GH_VoxelGrid(VoxelGrid3D grid)
        {
            Value = (VoxelGrid3D) grid.Clone();
        }
        /// <summary>
        /// Duplicate the Voxel Grid
        /// </summary>
        /// <returns></returns>
        public override IGH_Goo Duplicate()
        {

            var duplicate = new GH_VoxelGrid();
            duplicate.Value = (VoxelGrid3D) Value.Clone();
            return duplicate;

        }

        /// <summary>
        /// Returns true if grid is valid
        /// </summary>
        public override bool IsValid => Value.IsValid;

        /// <summary>
        /// Creates a string of the voxelgrid
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
                return $"Invalid VoxelGrid [{Value.SizeUVW.X},{Value.SizeUVW.Y},{Value.SizeUVW.Z}]";
            }
            return
                $"VoxelGrid [{Value.SizeUVW.X},{Value.SizeUVW.Y},{Value.SizeUVW.Z}={Value.SizeUVW.SelfProduct()}] True = {Value.CountNonZero}";
            
        }
        

        /// <summary>
        /// Returns a human readable explanation of the tpye
        /// </summary>
        public override string TypeDescription => "VoxelGrid";

        /// <summary>
        /// Type of component
        /// </summary>
        public override string TypeName => "VoxelGrid";

        /// <summary>
        /// Returns the underlying voxelgrid
        /// </summary>
        /// <returns>VoxelGrid</returns>
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
            if (typeof(Q).IsAssignableFrom(typeof(VoxelGrid3D)))
            {
                object ptr = Value;
                target = (Q)ptr;
                return true;
            }

            if (typeof(Q).IsAssignableFrom(typeof(GH_Mesh)))
            {
                var m = VoxelGridMeshHelper.VoxelGridToMesh(Value);
                // don't add fake meshes..
                // VoxelGridMeshHelper.addFakeShadow(ref m, new Vector3d(-0.495633, 0.142501, 0.856762), 1.0, Color.White, Color.Black);
                var ghm = new GH_Mesh(m);
                target = (Q)(object)ghm;
                return true;
            }

            //First, see if Q is similar to the Integer primitive.
            if (typeof(Q).IsAssignableFrom(typeof(string)))
            {
                target = (Q)(object)ToString();
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
            if (source.GetType().IsAssignableFrom(typeof(VoxelGrid3D)))
            {
                Value = (VoxelGrid3D) ((VoxelGrid3D)source).Clone();
                return true;
            }
            //We've exhausted the possible conversions, it seems that source
            //cannot be converted into a TriStateType after all.
            return false;
        }

        /*
        public override bool Write(GH_IO.Serialization.GH_IWriter writer)
        {
            bool rc = base.Write(writer);
            writer.RemoveChunk("PersistentData");
            // save structure
            writer.SetString("PersistentDataHack", myZippedString);

        }

        public override bool Read(GH_IO.Serialization.GH_IReader reader)
        {
            reader.GetString(
        }*/
    }
}
