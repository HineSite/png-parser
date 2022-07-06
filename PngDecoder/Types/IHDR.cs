using System;

namespace PngDecoder.Types
{
   public class IHDR : PngType
   {
      public uint Width { get; private set; }
      public uint Height { get; private set; }
      public byte BitDepth { get; private set; }
      public byte ColorType { get; private set; }
      public byte CompressionMethod { get; private set; }
      public byte FilterMethod { get; private set; }
      public byte InterlaceMethod { get; private set; }

      public override int Length
      {
         get
         {
            return 13;
         }
      }

      public override void Fill(byte[] bytes)
      {
         if (bytes.Length != this.Length)
            throw new ArgumentException("Invalid byte length for PngType." + this.GetType().Name);

         Width = bytes.ToUInt32(0);
         Height = bytes.ToUInt32(4);
         BitDepth = bytes[8];
         ColorType = bytes[9];
         CompressionMethod = bytes[10];
         FilterMethod = bytes[11];
         InterlaceMethod = bytes[12];
      }

      public override PngType Create()
      {
         return new IHDR();
      }

      public override byte[] getData()
      {
         byte[] data = new byte[this.Length];

         data.Insert(this.Width, 0);
         data.Insert(this.Height, 4);

         data[8] = BitDepth;
         data[9] = ColorType;
         data[10] = CompressionMethod;
         data[11] = FilterMethod;
         data[12] = InterlaceMethod;

         return data;
      }
   }
}
