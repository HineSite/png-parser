using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PngDecoder
{
   public static class Extensions
   {
      // Bit converter is a pain in the donkey. Windows uses little endian, but png numbers are big endian.
      // Instead of reversing them, we just do it ourselves.

      /// <summary>
      ///   Inserts an integer into the array at the specified index. (Big Endian)
      /// </summary>
      public static void Insert(this byte[] array, uint integer, int index)
      {
         array[index++] = (byte)(integer >> 24);
         array[index++] = (byte)(integer >> 16);
         array[index++] = (byte)(integer >> 8);
         array[index] = (byte)(integer);
      }

      /// <summary>
      ///   Creates a UInt32 from the first four bytes starting at index. (Big Endian)
      /// </summary>
      public static UInt32 ToUInt32(this byte[] array, int index)
      {
         return (UInt32)((array[index++] << 24) | (array[index++] << 16) | (array[index++] << 8) | array[index]);
      }

      /// <summary>
      ///   Returns a copy of the original array.
      /// </summary>
      public static byte[] Copy(this byte[] array)
      {
         byte[] clone = new byte[array.Length];
         Array.Copy(array, 0, clone, 0, array.Length);

         return clone;
      }
   }
}
