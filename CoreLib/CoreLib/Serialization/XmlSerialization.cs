using System;
using System.CodeDom;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace CoreLib.Serialization {
   public class XmlSerialization<TClass> {
      private XmlSerializer _Serializer = new XmlSerializer(typeof(TClass));

      public Stream Serialize(TClass instance) {
         var stream = new MemoryStream();
         _Serializer.Serialize(stream, instance);
         return stream;
      }

      public TClass Deserialize(Stream stream) {
         return (TClass)_Serializer.Deserialize(stream);
      }
   }
}