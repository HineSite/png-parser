
namespace PngDecoder.Types
{
   public class IEND : PngType
   {
      private bool _dataFound; // IEND should never have data, this indicates an error.

      public override int Length
      {
         get
         {
            return 0; // Always 0
         }
      }

      public override void Fill(byte[] bytes)
      {
         if (bytes.Length > 0)
            this._dataFound = true;
      }

      public override PngType Create()
      {
         return new IEND();
      }

      public override byte[] getData()
      {
         return new byte[this.Length];
      }
   }
}
