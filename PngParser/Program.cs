using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using PngDecoder;

namespace PngParser
{
   class Program
   {
      private Program()
      {
         Decoder decoder = new Decoder();
         List<Chunk> chunks = decoder.Decode(@"test1.png");

         //byte[] array1 = new byte[] { 104, 5, 1, 108 };
         //byte[] array2 = new byte[] { 42, 147, 213, 1 };
         //byte[] array3 = new byte[] {  150, 234, 1, 241 };

         //Console.WriteLine(array1.ToUInt32(0));
         //Console.WriteLine(array2.ToUInt32(0));
         //Console.WriteLine(array3.ToUInt32(0));

         using (StreamWriter writer = new StreamWriter(@"test1.png"))
         {
            writer.WriteLine(String.Join(", ", chunks[1].Type.getData()));
            writer.WriteLine("");
            writer.WriteLine("");
            writer.WriteLine("");
            writer.WriteLine("");
            writer.WriteLine("");
            //writer.WriteLine(String.Join(", ", chunks[2].Type.getData()));
         }

         //Console.WriteLine(String.Join(", ", chunks[1].Type.getData()));
         //Console.WriteLine("");
         //Console.WriteLine(String.Join(", ", chunks[2].Type.getData()));
      }

      public static void Main(string[] args)
      {
         new Program();

         Console.WriteLine("\nDone!");
         Console.Read();
      }
   }
}
