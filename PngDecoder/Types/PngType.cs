using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace PngDecoder.Types
{
   public abstract class PngType
   {
      private static Dictionary<String, PngType> _pngTypes = new Dictionary<String, PngType>();
      
      protected byte[] _type;
      public byte[] Type
      {
         get
         {
            return this._type.Copy(); // Read only... Close enough.
         }
      }

      public string Name { get; private set; }

      public abstract int Length
      {
         get;
      }

      static PngType()
      {
         Type pngType = typeof(PngType);
         object obj;
         PngType pngObj;

         foreach (Type type in Assembly.GetAssembly(pngType).GetTypes().Where(type => type.IsClass && !type.IsAbstract && type.IsSubclassOf(pngType)))
         {
            obj = Activator.CreateInstance(type);
            if (obj == null)
               continue;

            pngObj = obj as PngType;
            if (pngObj == null)
               continue;
            
            if (_pngTypes.ContainsKey(type.Name)) // Not supposed to happen
               continue;

            _pngTypes.Add(type.Name, pngObj);
         }
      }

      public static PngType From(byte[] bytes)
      {
         if (bytes.Length != 4)
            return null;

         foreach (byte bite in bytes)
         {
            if (!(bite >= 65 && bite <= 90) && !(bite >= 97 && bite <= 122))
               return null;
         }
         
         String typeName = Encoding.ASCII.GetString(bytes);

         if (!_pngTypes.ContainsKey(typeName))
         {
            Unknown unknown = new Unknown();
            unknown._type = bytes.Copy();
            unknown.Name = typeName;

            return unknown;
         }

         PngType pngType = _pngTypes[typeName].Create();
         pngType._type = bytes.Copy();
         pngType.Name = typeName;

         return pngType;
      }
      
      public abstract byte[] getData();

      public abstract void Fill(byte[] bytes);

      public abstract PngType Create();
   }
}
