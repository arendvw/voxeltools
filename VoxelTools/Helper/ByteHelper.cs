using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using Rhino.Geometry;
using StudioAvw.Voxels.Geometry;

namespace StudioAvw.Voxels.Helper
{
    /// <summary>
    /// ByteHelper helps serialize object to a reduced bytearray and back
    /// </summary>
    public static class ByteHelper
    {
        /// <summary>
        /// Convert a byte Array to a double Array
        /// </summary>
        /// <param name="bytes"></param>
        /// <returns></returns>
        public static double[] ToDoubleArray(byte[] bytes)
        {
            if (bytes.Length % 8 != 0)
            {
                throw new ArgumentException(
                    $"ToDoubleArray expects multiple 8byte/64bit segments {bytes.Length} is not a multiple of 8");
            }
            var length = (int)Convert.ToInt32(bytes.Length / 8);
            var output = new double[length];
            for (var i = 0; i < length; i++)
            {
                output[i] = BitConverter.ToDouble(bytes, i * 8);
            }
            return output;
        }


        /// <summary>
        /// Convert a double array to a byte array (8n indices)
        /// </summary>
        /// <param name="arr"></param>
        /// <returns></returns>
        public static byte[] ToByte(double[] arr)
        {
            var output = new byte[arr.Length*8];
            for (var i = 0; i < arr.Length; i++)
            {
                BitConverter.GetBytes(arr[i]).CopyTo(output, i * 8);
            }
            return output;
        }

        const int IntLength = 4;

        /// <summary>
        /// Convert a byte array to an int array
        /// </summary>
        /// <param name="bytes"></param>
        /// <returns></returns>
        public static int[] ToIntArray(byte[] bytes)
        {
            if (bytes.Length % IntLength != 0)
            {
                throw new ArgumentException(
                    $"ToIntArray expects multiple 4byte/32bit segments {bytes.Length} is not a multiple of 8");
            }
            var length = (int)Convert.ToInt32(bytes.Length / IntLength);
            var output = new int[length];
            for (var i = 0; i < length; i++)
            {
                output[i] = BitConverter.ToInt32(bytes, (int)i * IntLength);
            }
            return output;
        }

        /// <summary>
        /// Convert an int array to a byte array
        /// </summary>
        /// <param name="arr"></param>
        /// <returns></returns>
        public static byte[] ToByte(int[] arr)
        {
            var output = new byte[arr.Length * IntLength];
            for (var i = 0; i < arr.Length; i++)
            {
                BitConverter.GetBytes(arr[i]).CopyTo(output, i * IntLength);
            }
            return output;
        }


        /// <summary>
        /// Size of a Point3d is 24 byte (3x8 double)
        /// </summary>
        const int Point3dLength = 24;

        /// <summary>
        /// Convert a byte array (n=24) to a Point3d
        /// </summary>
        /// <param name="ba"></param>
        /// <returns></returns>
        public static Point3d ToPoint3d(byte[] ba)
        {
            if (ba.Length != Vector3dLength)
            {
                throw new ArgumentOutOfRangeException(
                    $"ToVector3d expects exactly {Point3dLength} bytes, received {ba.Length} bytes");
            }
            var pts = ToDoubleArray(ba);
            return new Point3d(pts[0], pts[1], pts[2]);
        }

        /// <summary>
        /// Convert a Point3d to a byte array (n=24)
        /// </summary>
        /// <param name="pt"></param>
        /// <returns></returns>
        public static byte[] ToByte(Point3d pt)
        {
            var output = ToByte( new double[3] { pt.X,pt.Y,pt.Z } );
            return output;
        }

        /// <summary>
        /// Size of a Vector3d is 24 byte (3x8 double)
        /// </summary>
        const int Vector3dLength = 24;

        /// <summary>
        /// Convert a byte array of 24 bytes to a Vector3d
        /// </summary>
        /// <param name="ba"></param>
        /// <returns></returns>
        public static Vector3d ToVector3d(byte[] ba)
        {
            if (ba.Length != Vector3dLength)
            {
                throw new ArgumentOutOfRangeException(
                    $"ToVector3d expects exactly {Vector3dLength} bytes, received {ba.Length} bytes");
            }

            var pts = ToDoubleArray(ba);
            return new Vector3d(pts[0], pts[1], pts[2]);
        }

        /// <summary>
        /// Convert a Vector3d to a bytearray of 24 bytes.
        /// </summary>
        /// <param name="v">Vector3d</param>
        /// <returns></returns>
        public static byte[] ToByte(Vector3d v)
        {
            return ToByte(new double[3] { v.X, v.Y, v.Z });
        }


        /// <summary>
        /// A plane consists of 72 byte: 24 byte origin point, 2x24 byte vectors
        /// </summary>
        const int PlaneLength = 72;

        /// <summary>
        /// Convert a byte array to a Plane
        /// </summary>
        /// <param name="ba"></param>
        /// <returns></returns>
        public static Plane ToPlane(byte[] ba)
        {
            if (ba.Length != PlaneLength)
            {
                throw new ArgumentOutOfRangeException(
                    $"ToPlane expects exactly {PlaneLength} bytes, received {ba.Length} bytes");
            }

            var n = ToDoubleArray(ba);
            var origin = new Point3d(n[0], n[1], n[2]);
            var x = new Vector3d(n[3], n[4], n[5]);
            var y = new Vector3d(n[6], n[7], n[8]);
            return new Plane(origin, x, y);
        }

        /// <summary>
        /// Convert a Plane to Byte array
        /// </summary>
        /// <param name="ba"></param>
        /// <returns></returns>
        public static byte[] ToByte(Plane ba)
        {
            var output = new byte[72];
            ToByte(ba.Origin).CopyTo(output, 0);
            ToByte(ba.XAxis).CopyTo(output, 24);
            ToByte(ba.YAxis).CopyTo(output, 48);
            return output;
        }

        /// <summary>
        /// Size of an interval is 16 byte (2x8 byte)
        /// </summary>
        const int IntervalLength = 16;

        /// <summary>
        /// Convert an interval to a byte array of 16 byte
        /// </summary>
        /// <param name="i"></param>
        /// <returns></returns>
        public static byte[] ToByte(Interval i)
        {
            return ToByte(new double[] {i.Min, i.Max});
        }

        /// <summary>
        /// Convert a byte array of 16 bits to an Interval
        /// </summary>
        /// <param name="b"></param>
        /// <returns></returns>
        public static Interval ToInterval(byte[] b)
        {
            if (b.Length != IntervalLength)
            {
                throw new ArgumentOutOfRangeException(
                    $"Interval expects exactly {IntervalLength} bytes, received {b.Length} bytes");
            }

            var parts = ToDoubleArray(b);
            return new Interval(parts[0], parts[1]);
        }

        /// <summary>
        /// The length of a box is 120 (72 plane, 3x 16 intervals);
        /// </summary>
        const int BoxLength = 72+16+16+16;

        /// <summary>
        /// Convert a byte array of 120 bytes to a box.
        /// </summary>
        /// <param name="ba">Byte Array</param>
        /// <returns></returns>
        public static Box ToBox(byte[] ba)
        {
            if (ba.Length != BoxLength)
            {
                throw new ArgumentOutOfRangeException(
                    $"ToBox expects exactly {BoxLength} bytes, received {ba.Length} bytes");
            }

            var p = ToPlane(ba.Take(72).ToArray());
            var x = ToInterval(ba.Skip(72).Take(16).ToArray());
            var y = ToInterval(ba.Skip(88).Take(16).ToArray());
            var z = ToInterval(ba.Skip(104).Take(16).ToArray());
            return new Box(p, x, y, z);
        }

        /// <summary>
        /// Convert a box to a byte[120]
        /// </summary>
        /// <param name="bo"></param>
        /// <returns></returns>
        public static byte[] ToByte(Box bo)
        {
            var output = new byte[120];
            ToByte(bo.Plane).CopyTo(output, 0);
            ToByte(bo.X).CopyTo(output, 72);
            ToByte(bo.Y).CopyTo(output, 88);
            ToByte(bo.Z).CopyTo(output, 104);
            return output;
        }


        
        /// <summary>
        /// Byte Array to Hex
        /// </summary>
        /// <param name="bytes"></param>
        /// <returns>Hex String</returns>
        public static string ByteToHex(byte[] bytes)
        {
            var c = new char[bytes.Length * 2];
            int b;
            for (var i = 0; i < bytes.Length; i++)
            {
                b = bytes[i] >> 4;
                c[i * 2] = (char)(55 + b + (((b - 10) >> 31) & -7));
                b = bytes[i] & 0xF;
                c[i * 2 + 1] = (char)(55 + b + (((b - 10) >> 31) & -7));
            }
            return new string(c);
        }


        /// <summary>
        /// Convert a string to a byte array
        /// </summary>
        /// <param name="hex"></param>
        /// <returns></returns>
        public static byte[] StringToByteArray(string hex)
        {
            var NumberChars = hex.Length / 2;
            var bytes = new byte[NumberChars];
            using (var sr = new StringReader(hex))
            {
                for (var i = 0; i < NumberChars; i++)
                    bytes[i] =
                      Convert.ToByte(new string(new char[2] { (char)sr.Read(), (char)sr.Read() }), 16);
            }
            return bytes;
        }

        /// <summary>
        /// Convert a BitArray to an byte array with a 32bit/8byte prefix for the array length
        /// A bitarray can have an arbitrary amount of bits - not all can be 
        /// </summary>
        /// <param name="bits"></param>
        /// <returns></returns>
        public static byte[] ToByte(BitArray bits)
        {
            var numBits = bits.Count;
            var numBytes = Convert.ToInt32(Math.Ceiling((double) bits.Count / 8));
            if (bits.Count % 8 != 0) numBytes++;

            
            var bytes = new byte[numBytes];
            bits.CopyTo(bytes, 0);


            // add the prefix for the amount of bytes
            var output = new byte[numBytes + 4];
            BitConverter.GetBytes(numBits).CopyTo(output,0);
            bytes.CopyTo(output,4);
            return output;
        }

        /// <summary>
        /// Convert a byte array to a bitarray. The first 32 bits should denote how long the bit array is
        /// </summary>
        /// <param name="bytes"></param>
        /// <returns></returns>
        public static BitArray ToBitArray(byte[] bytes)
        {
            var bits = new byte[bytes.Length - 4];
            var size = BitConverter.ToInt32(bytes,0);
            // can probably be faster
            bytes.Skip(4).Take((bytes.Length - 4)).ToArray().CopyTo(bits, 0);

            var output = new BitArray(bits);
            output.Length = size;
            return output;
        }


        /// <summary>
        /// Convert a voxelgrid to a bytearray
        /// </summary>
        /// <param name="vg"></param>
        /// <returns></returns>
        public static byte[] ToByte(VoxelGrid3D vg)
        {
          // check for integer overflows
          checked {
            var byteGrid = ToByte(vg.Grid);
            var byteLength = 120 + 24 + 4 + byteGrid.Length;

            var output = new byte[byteLength];
            // voxelsize can be float = 9 bytes
            ToByte(vg.VoxelSize).CopyTo(output, 0);
            // bbox can be ptA, ptB + plane equation. 3x3x2 = 18
            // plane equation = normalize plane. Take a plane and take the origin out of the equation
            // plane equation - 
            ToByte(vg.BBox).CopyTo(output, 24);
            ToByte(vg.Grid).CopyTo(output, 144);
            return output;
          }
        }

        /// <summary>
        /// Convert a byte array to a voxelgrid
        /// </summary>
        /// <param name="b"></param>
        /// <returns></returns>
        public static VoxelGrid3D ToVoxelGrid(byte[] b)
        {
            var voxSize = ToPoint3d(b.Take(24).ToArray());
            var BBox = ToBox(b.Skip(24).Take(120).ToArray());
            var Grid = ToBitArray(b.Skip(144).ToArray());
            return new VoxelGrid3D(BBox, voxSize, Grid);
        }
        /// <summary>
        /// Compress the byte[] array
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static byte[] Compress(byte[] input)
        {
            byte[] output;
            using (var ms = new MemoryStream())
            {
                using (var gs = new GZipStream(ms, CompressionMode.Compress))
                {
                    gs.Write(input, 0, input.Length);
                    gs.Close();
                    output = ms.ToArray();
                }
                ms.Close();
            }
            return output;
        }

        /// <summary>
        /// Decompress the byte[] array
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static byte[] Decompress(byte[] input)
        {
            var output = new List<byte>();
            using (var ms = new MemoryStream(input))
            {
                using (var gs = new GZipStream(ms, CompressionMode.Decompress))
                {
                    var readByte = gs.ReadByte();
                    while (readByte != -1)
                    {
                        output.Add((byte)readByte);
                        readByte = gs.ReadByte();
                    }
                    gs.Close();
                }
                ms.Close();
            }
            return output.ToArray();
        } 


    }
}