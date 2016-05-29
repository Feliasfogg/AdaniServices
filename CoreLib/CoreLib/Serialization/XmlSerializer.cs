using System;
using System.CodeDom;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace CoreLib.Serialization {
   public static class XmlSerializer<TClass> {
      public static Stream Serialize(TClass instance) {
         XmlSerializer _Serializer = new XmlSerializer(typeof(TClass));
         var stream = new MemoryStream();
         _Serializer.Serialize(stream, instance);
         stream.Position = 0;
         return stream;
      }

      public static string SerializeToXmlString(TClass instance) {
         XmlSerializer _Serializer = new XmlSerializer(typeof(TClass));
         var stream = new MemoryStream();
         _Serializer.Serialize(stream, instance);
         stream.Position = 0;
         byte[] btarr = new byte[stream.Length];
         stream.Read(btarr, 0, btarr.Length);
         var resultString = Encoding.ASCII.GetString(btarr);
         return resultString;
      }

      public static byte[] SerializeToBytes(TClass instance) {
         XmlSerializer _Serializer = new XmlSerializer(typeof(TClass));
         var stream = new MemoryStream();
         _Serializer.Serialize(stream, instance);
         stream.Position = 0;
         byte[] btarr = new byte[stream.Length];
         stream.Read(btarr, 0, btarr.Length);
         return btarr;
      }

      public static TClass Deserialize(Stream stream) {
         XmlSerializer _Serializer = new XmlSerializer(typeof(TClass));
         return (TClass)_Serializer.Deserialize(stream);
      }

      public static TClass Deserialize(byte[] data) {
         XmlSerializer _Serializer = new XmlSerializer(typeof(TClass));
         var stream = new MemoryStream(data);
         return (TClass)_Serializer.Deserialize(stream);
      }

      public static TClass Deserialize(string xmlString) {
         XmlSerializer _Serializer = new XmlSerializer(typeof(TClass));
         byte[] btarr = Encoding.UTF8.GetBytes(xmlString);
         Stream stream = new MemoryStream(btarr);
         return (TClass)_Serializer.Deserialize(stream);
      }
   }
}