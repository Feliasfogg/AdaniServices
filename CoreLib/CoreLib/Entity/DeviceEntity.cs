using System.Runtime.Serialization;

namespace CoreLib.Entity {
   [DataContract]
   public class DeviceEntity {
      [DataMember]
      public int Id { get; set; }

      [DataMember]
      public int DeviceGroupId { get; set; }

      [DataMember]
      public string ConnectionType { get; set; }

      [DataMember]
      public int GeneratorType { get; set; }

      [DataMember]
      public int NormalVoltage { get; set; }

      [DataMember]
      public int HighVoltage { get; set; }

      [DataMember]
      public int NormalCurrent { get; set; }

      [DataMember]
      public int HighCurrent { get; set; }

      [DataMember]
      public int HighMode { get; set; }

      [DataMember]
      public double ReseasonDate { get; set; }

      [DataMember]
      public double WorkTime { get; set; }

      [DataMember]
      public double XRayTime { get; set; }

      [DataMember]
      public double LastWorkedDate { get; set; }

      [DataMember]
      public string Name { get; set; }
   }
}