using System.Runtime.Serialization;

namespace Tests {
   [DataContract]
   public class TestClass {
      [DataMember]
      public string Name { get; set; }

      [DataMember]
      public int Value { get; set; }
   }
}