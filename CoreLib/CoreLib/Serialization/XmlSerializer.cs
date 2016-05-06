using System;
using System.CodeDom;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace CoreLib.Serialization {
   public class XmlSerializer<TClass> {
      private XmlSerializer _Serializer = new XmlSerializer(typeof(TClass));

      public Stream Serialize(TClass instance) {
         var stream = new MemoryStream();
         _Serializer.Serialize(stream, instance);
         stream.Position = 0;
         return stream;
      }

      public string SerializeToXmlString(TClass instance) {
         var stream = new MemoryStream();
         _Serializer.Serialize(stream, instance);
         stream.Position = 0;
         byte[] btarr = new byte[stream.Length];
         stream.Read(btarr, 0, btarr.Length);
         var resultString = Encoding.ASCII.GetString(btarr);
         return resultString;
      }

      public byte[] SerializeToBytes(TClass instance) {
         var stream = new MemoryStream();
         _Serializer.Serialize(stream, instance);
         stream.Position = 0;
         byte[] btarr = new byte[stream.Length];
         stream.Read(btarr, 0, btarr.Length);
         return btarr;
      }

      public TClass Deserialize(Stream stream) {
         return (TClass)_Serializer.Deserialize(stream);
      }

      public TClass Deserialize(byte[] data) {
         var stream = new MemoryStream(data);
         return (TClass)_Serializer.Deserialize(stream);
      }

      public TClass Deserialize(string xmlString) {
         byte[] btarr = Encoding.ASCII.GetBytes(xmlString);
         Stream stream = new MemoryStream(btarr);
         return (TClass)_Serializer.Deserialize(stream);
      }
   }
}