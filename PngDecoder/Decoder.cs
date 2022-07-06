using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DamienG.Security.Cryptography;
using PngDecoder.Types;

namespace PngDecoder
{
   public class Decoder
   {
      public bool StrictMode { get; set; }

      public Decoder()
      {

      }

      public List<Chunk> Decode(string filename)
      {
         byte[] input = File.ReadAllBytes(filename);

         if (input == null)
            throw new FileNotFoundException(filename);

         if (input.Length < 44) // At minimum we need 8B header, IHDR (12), IDAT (12), IEND (12)
            throw new InvalidDataException("File too Small, not PNG?");


         if (StrictMode && !isValidHeader(input))
            throw new InvalidDataException("Not a Valid PNG Header");

         List<Chunk> chunks = new List<Chunk>();
         Chunk chunk = new Chunk();
         byte[] data = null;
         byte[] crcData = null;
         byte[] data4 = new byte[4];
         byte lengthIndex = 0;
         byte typeIndex = 0;
         uint dataIndex = 0;
         byte crcIndex = 0;

         for (int i = 8; i < input.Length; i++)
         {
            if (lengthIndex < 4) // 4 byte length
            {
               data4[lengthIndex++] = input[i];

               if (lengthIndex == 4)
               {
                  chunk.Length = data4.ToUInt32(0);
                  data = new byte[chunk.Length];
                  crcData = new byte[chunk.Length + 4]; // data length plus type length
               }

               continue;
            }

            if (typeIndex < 4) // 4 byte type
            {
               data4[typeIndex] = input[i];
               crcData[typeIndex] = input[i];

               typeIndex++;
               if (typeIndex == 4)
               {
                  chunk.Type = PngType.From(data4);
               }

               continue;
            }

            if (dataIndex < data.Length) // Unknown data length
            {
               data[dataIndex] = input[i];
               crcData[dataIndex + 4] = input[i]; // Offset by 4 to accommodate type

               dataIndex++;
               if (dataIndex == (data.Length - 1))
                  chunk.Type.Fill(data);

               continue;
            }

            if (crcIndex < 4) // 4 byte CRC
            {
               data4[crcIndex++] = input[i];

               if (crcIndex == 4)
               {
                  chunk.Crc = data4.ToUInt32(0);

                  chunks.Add(chunk);

                  // Reset for next chunk
                  chunk = new Chunk();
                  lengthIndex = 0;
                  typeIndex = 0;
                  dataIndex = 0;
                  crcIndex = 0;
               }

               continue;
            }
         }

         List<string> warnings = new List<string>();
         List<string> errors = new List<string>();
         bool canContinue = Validate(chunks, out warnings, out errors);

         long totalData = 0;
         long numIdats = 0;
         foreach (Chunk chnk in chunks)
         {
            if (chnk.Type.Name == "IDAT")
            {
               totalData += chnk.Type.Length;
               numIdats++;
            }
         }

         Console.WriteLine("IDAT Length: " + totalData);
         Console.WriteLine("IDAT Num: " + numIdats);

         foreach (string warning in warnings)
            Console.WriteLine("Warning: " + warning);

         foreach (string error in errors)
            Console.WriteLine("Error: " + error);

         return chunks;
      }

      /// <summary>
      ///   Validates all processed chunks.
      /// </summary>
      /// <param name="chunks">A list of all chunks in the order they were pulled from the file.</param>
      /// <param name="warnings">Output: A list of possible "errors" not required by w3 specs. (e.g. Unknown chunk type)</param>
      /// <param name="errors">Output: A list of "errors" as defined by w3 specs.</param>
      /// <returns>Returns true if there MIGHT be enough information at this time to process this file. This is not a guarantee.</returns>
      private bool Validate(List<Chunk> chunks, out List<string> warnings, out List<string> errors)
      {
         warnings = new List<string>();
         errors = new List<string>();
         bool canContinue = true;
         bool hasIhdr = false;
         bool hasIdat = false;
         bool hasIend = false;
         string lastChunk = String.Empty;
         Type unknownType = typeof(Unknown);

         for (int i = 0; i < chunks.Count; i++)
         {
            if (i == 0)
            {
               if (chunks[i].Type == null || chunks[i].Type.Name != "IHDR")
               {
                  errors.Add("First chunk is not IHDR");
               }
               else
               {
                  hasIhdr = true;
               }
            }
            else if (i == (chunks.Count - 1))
            {
               if (chunks[i].Type == null || chunks[i].Type.Name != "IEND")
               {
                  errors.Add("Last chunk is not IEND");
               }
               else
               {
                  if (hasIend)
                     errors.Add("IEND specified more than once");

                  hasIend = true;
               }
            }
            else
            {
               if (chunks[i].Type == null) // Not possible?
               {
                  errors.Add("Null chunk type found");
               }
               else if (chunks[i].Type.GetType() == unknownType)
               {
                  warnings.Add("Unknown chunk type found: " + chunks[i].Type.Name + " (" + String.Join(", ", chunks[i].Type.Type) + ") Data Length: " + chunks[i].Type.Length);
               }
               else if (chunks[i].Type.Name == "IHDR")
               {
                  if (hasIhdr)
                  {
                     errors.Add("IHDR specified more than once");

                     // We could do a data validity check on these.
                     // Perhaps check if they are equal.
                     // If not equal, check if either is correct.
                  }
                  else
                  {
                     errors.Add("IHDR is not the first chunk");
                  }
               }
               else if (chunks[i].Type.Name == "IHDR")
               {
                  hasIend = true; // Error message will occur later.
               }
               else if (chunks[i].Type.Name == "IDAT")
               {
                  if (hasIdat && lastChunk != "IDAT")
                     errors.Add("IDAT chunks must be sequential");

                  hasIdat = true;
               }

               lastChunk = (chunks[i].Type == null) ? "" : chunks[i].Type.Name;
            }
         }

         return (canContinue && hasIhdr && hasIdat);
      }

      private bool isValidHeader(byte[] input)
      {
         // The first eight bytes of a PNG file always contain the following (decimal) values:
         byte[] header = new byte[] { 137, 80, 78, 71, 13, 10, 26, 10 };

         for (int i = 0; i < 8; i++)
         {
            if (input[i] != header[i])
               return false;
         }

         return true;
      }
   }
}
