
namespace PngDecoder.Types
{
   public class Unknown : PngType
   {
      private int _dataLength;
      private byte[] _data;

      public override int Length
      {
         get
         {
            return _dataLength;
         }
      }

      public override void Fill(byte[] bytes)
      {
         this._dataLength = bytes.Length;
         this._data = bytes;
      }

      public override PngType Create()
      {
         return new Unknown();
      }

      public override byte[] getData()
      {
         return this._data;
      }
   }
}
