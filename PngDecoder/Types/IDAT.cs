using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PngDecoder.Types
{
   public class IDAT : PngType
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
         return new IDAT();
      }

      public override byte[] getData()
      {
         // Data could be large; not sure if we should copy this.
         // Other PngTypes are either copies or new arrays which makes this confusing.

         return this._data;
      }
   }
}
