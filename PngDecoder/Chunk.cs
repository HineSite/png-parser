using PngDecoder.Types;

namespace PngDecoder
{
   public struct Chunk
   {
      public uint Length;
      public PngType Type;
      public uint Crc;
   }
}
