using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using CoreLib.Helpers;
using CoreLib.Serialization;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Tests {
   [TestClass]
   public class UnitTest1 {
      [TestMethod]
      public void SerializeDeserializeTest() {
         var testInstance = new TestClass() {
            Name = "Test",
            Value = 1
         };

         var serializer = new XmlSerialization<TestClass>();
         Stream stream = serializer.Serialize(testInstance);
         stream.Position = 0;
         TestClass deserializeInstance = serializer.Deserialize(stream);

         Assert.IsTrue(testInstance.Name == deserializeInstance.Name);
         Assert.IsTrue(testInstance.Value == deserializeInstance.Value);
      }

      [TestMethod]
      public async Task UdpTest() {
         var udpHelper = new UdpHelper(4444, new IPEndPoint(IPAddress.Parse("127.0.0.1"), 45887));
         await udpHelper.ListenAsync();
      }
   }
}