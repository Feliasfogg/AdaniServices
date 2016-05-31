using System;
using System.Data;
using System.Data.Entity.Core.EntityClient;
using System.Data.SqlClient;
using System.Net;
using System.Text;
using System.Xml;
using CoreLib.Commands.Common;
using CoreLib.Commands.Settings;
using CoreLib.Encryption;
using CoreLib.Entity;
using CoreLib.Helpers;
using CoreLib.Listeners;
using CoreLib.Senders;
using CoreLib.Serialization;
using System.Configuration;


namespace DeviceSettingsServer.Listeners {
   public class SettingsListener : CommandListener {
      public SettingsListener(int listenPort, IPEndPoint localTcpEp) : base(listenPort, localTcpEp) {
      }

      protected override void Parse(byte[] data) {
         string strData = Encoding.ASCII.GetString(data);
         if(strData == "GET SETTINGS") {
            SendTcpSettings();
         }
         else {
            //дешифровка
            string decryptXml = Encoding.ASCII.GetString(Encrypter.DecryptData(data));
            //парсинг результирующего xml
            var xml = new XmlDocument();
            xml.LoadXml(decryptXml);
            XmlNodeList nodeList = xml.GetElementsByTagName("Command");
            var xmlNode = nodeList.Item(0);
            //выбор команды для выполнения
            switch(xmlNode.InnerText) {
            case "ExportDataBase":
               ExportDataBase(decryptXml);
               break;
            case "GetDevice":
               GetDevice(decryptXml);
               break;
            case "AddDevice":
               AddDevice(decryptXml);
               break;
            case "EditDevice":
               EditDevice(decryptXml);
               break;
            case "RemoveDevice":
               RemoveDevice(decryptXml);
               break;
            default:
               break;
            }
         }
      }

      private User GetUserInfo(string sessionKey) {
         try {
            var sender = new CommandSender(BroadcastHelper.BroadCastIp, 4444);
            sender.GetTcpSettings();

            var authInfoCommand = new ServiceCommand() {
               Command = CommandActions.GetUser,
               SessionKey = sessionKey
            };

            string strAuthInfoCommand = XmlSerializer<ServiceCommand>.SerializeToXmlString(authInfoCommand);

            sender.SendTcpCommand(strAuthInfoCommand);
            byte[] btarrResponse = sender.ReceiveData();
            string strAuthInfoResult = Encoding.ASCII.GetString(btarrResponse);
            //десериализация xml в объект пользователя
            var xmlUserInfo = XmlSerializer<User>.Deserialize(strAuthInfoResult);
            return xmlUserInfo;
         }
         catch {
            return null;
         }
      }

      private void GetDevice(string xmlCommand) {
         try {
            var command = XmlSerializer<SettingsCommand>.Deserialize(xmlCommand);

            User user = GetUserInfo(command.SessionKey);
            if(user == null) {
               throw new Exception("Cant get user info");
            }
            string xmlDeviceInfo;
            using(var provider = new EntityProvider()) {
               Device device = provider.GetDeviceInfo(command.DeviceId);
               if(device == null) {
                  throw new Exception("Cant get device information");
               }
               xmlDeviceInfo = XmlSerializer<Device>.SerializeToXmlString(device);
            }
            SendResponse(xmlDeviceInfo);
         }
         catch(Exception ex) {
            SendResponse($"{ex.Message} in {nameof(GetDevice)}");
         }
      }

      private void AddDevice(string xmlCommand) {
         try {
            var command = XmlSerializer<SettingsCommand>.Deserialize(xmlCommand);

            User user = GetUserInfo(command.SessionKey);
            if(user == null) {
               throw new Exception("Cant get user info in");
            }
            byte[] accessLevel = BitConverter.GetBytes(user.AccessLevel);

            if(accessLevel[7] >= 200) {
               using(var provider = new EntityProvider()) {
                  provider.AddDevice(command.Device);
               }
            }
            else {
               throw new Exception($"User with ID={user.Id} cant edit device settings");
            }
            SendResponse("ok");
         }
         catch(Exception ex) {
            SendResponse($"{ex.Message} in {nameof(AddDevice)}");
         }
      }

      private void EditDevice(string xmlCommand) {
         try {
            var command = XmlSerializer<SettingsCommand>.Deserialize(xmlCommand);

            User user = GetUserInfo(command.SessionKey);
            if(user == null) {
               throw new Exception("Cant get user info in");
            }
            byte[] accessLevel = BitConverter.GetBytes(user.AccessLevel);

            if(accessLevel[7] >= 200) {
               //изменения может вносить только пользователь с доступом выше 200, параметры доступа определяются младшим байтом
               using(var provider = new EntityProvider()) {
                  var device = provider.GetDeviceInfo(command.Device.Id);
                  if(device == null) {
                     throw new Exception($"No exist device with id {command.Device.Id}");
                  }
                  device.ConnectionType = command.Device.ConnectionType;
                  device.DeviceGroupId = command.Device.DeviceGroupId;
                  device.GeneratorType = command.Device.GeneratorType;
                  device.HighCurrent = command.Device.HighCurrent;
                  device.NormalCurrent = command.Device.NormalCurrent;
                  device.HighMode = command.Device.HighMode;
                  device.HighVoltage = command.Device.HighVoltage;
                  device.NormalVoltage = command.Device.NormalVoltage;
                  device.LastWorkedDate = command.Device.LastWorkedDate;
                  device.WorkTime = command.Device.WorkTime;
                  device.Name = command.Device.Name;
                  device.ReseasonDate = command.Device.ReseasonDate;
                  device.XRayTime = command.Device.XRayTime;
               }
            }
            else {
               throw new Exception($"User with ID={user.Id} cant edit device settings");
            }
            SendResponse("ok");
         }
         catch(Exception ex) {
            SendResponse($"{ex.Message} in {nameof(AddDevice)}");
         }
      }

      private void RemoveDevice(string xmlCommand) {
         try {
            var command = XmlSerializer<SettingsCommand>.Deserialize(xmlCommand);

            User user = GetUserInfo(command.SessionKey);
            if(user == null) {
               throw new Exception("Cant get user info in");
            }
            byte[] accessLevel = BitConverter.GetBytes(user.AccessLevel);

            if(accessLevel[7] >= 200) {
               using(var provider = new EntityProvider()) {
                  bool result = provider.RemoveDevice(command.DeviceId);
                  if(!result) {
                     throw new Exception($"cant delete device with ID {command.DeviceId}");
                  }
               }
            }
            else {
               throw new Exception($"User with ID={user.Id} cant edit device settings");
            }
            SendResponse("ok");
         }
         catch(Exception ex) {
            SendResponse($"{ex.Message} in {nameof(AddDevice)}");
         }
      }

      private void ExportDataBase(string xmlCommand) {
         try {
            var command = XmlSerializer<SettingsCommand>.Deserialize(xmlCommand);

            User user = GetUserInfo(command.SessionKey);
            if(user == null) {
               throw new Exception("Cant get user info in");
            }
            byte[] accessLevel = BitConverter.GetBytes(user.AccessLevel);

            if(accessLevel[7] >= 200) {
               DataSet ds = new DataSet();
               var tables = new[] { "DeviceGroups", "Devices" };
               string data = String.Empty;

               string connectionString = new EntityConnectionStringBuilder(ConfigurationManager.ConnectionStrings["EntityDataModelContainer"].ConnectionString).ProviderConnectionString;

               foreach(var table in tables) {
                  var query = $"SELECT * FROM {table}";
                  SqlConnection conn = new SqlConnection(connectionString);
                  SqlCommand cmd = new SqlCommand(query, conn);
                  conn.Open();
                  SqlDataAdapter da = new SqlDataAdapter(cmd);
                  da.Fill(ds);
                  conn.Close();
                  conn.Dispose();
                  data += ds.GetXml();
               }
               SendResponse(data);
            }
            else {
               throw new Exception($"User with ID={user.Id} cant edit device settings");
            }
         }
         catch(Exception ex) {
            SendResponse($"{ex.Message} in {nameof(AddDevice)}");
         }
      }
   }
}