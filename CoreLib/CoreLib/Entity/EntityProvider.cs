using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CoreLib.Encryption;

namespace CoreLib.Entity {
   public class EntityProvider : IDisposable {
      private EntityDataModelContainer _Context = new EntityDataModelContainer();

      public EntityProvider() {
         _Context.Configuration.ProxyCreationEnabled = false;
      }

      public DbSet<User> Users {
         get { return _Context.Users; }
      }

      public bool AddUser(User user) {
         //TODO незабыть потом снова включить проверку
         //var existUser = _Context.Users.FirstOrDefault(usr => usr.Login == user.Login);
         //if(existUser != null) {
         //   return false;
         //}
         _Context.Users.Add(user);
         return true;
      }

      public User GetUserByCredentials(string login, string password) {
         return _Context.Users.FirstOrDefault(user => user.Login == login && user.Password == password);
      }

      public User GetUserById(int id) {
         return _Context.Users.FirstOrDefault(user => user.Id == id);
      }

      public User GetUserByKey(string strSessionKey) {
         SessionKey sessionKey = _Context.SessionKeys.FirstOrDefault(key => key.Key == strSessionKey);
         if(sessionKey == null) {
            return null;
         }
         var user = _Context.Users.FirstOrDefault(usr => usr.SessionKey.Key == sessionKey.Key);
         //здесь я делаю объект user без поля SessionKey т.к там идут циклические ссылки User->SessionKey->User...
         //а xml сериализатор не поддерживает такую сериализацию. Потому оставляем только те поля, котоыре необходимы для работы
         var proxy = new User() {
            Id = user.Id,
            AccessLevel = user.AccessLevel,
            Login = user.Login,
            Password = user.Password,
            Name = user.Name
         };
         return proxy;
      }

      public bool RemoveUser(int id) {
         var user = _Context.Users.FirstOrDefault(usr => usr.Id == id);
         if(user == null) {
            return false;
         }

         var key = _Context.SessionKeys.FirstOrDefault(sessionKey => sessionKey.User.Id == user.Id);
         if(key != null) {
            _Context.SessionKeys.Remove(key);
         }
         _Context.Users.Remove(user);

         return true;
      }

      public string CreateSessionKey(User user) {
         string sessionKey = Encrypter.GeneratePassword(32);
         SessionKey key = GetSessionKey(user);
         if(key == null) {
            user.SessionKey = new SessionKey() {
               Key = sessionKey,
               ExpirationTime = DateTime.Now.AddHours(2)
            };
         }
         else {
            user.SessionKey.Key = sessionKey;
            user.SessionKey.ExpirationTime = DateTime.Now.AddHours(2);
         }
         return sessionKey;
      }

      public SessionKey GetSessionKey(User user) {
         return _Context.SessionKeys.FirstOrDefault(key => key.User.Id == user.Id);
      }

      public void AddDevice(Device device) {
         _Context.Devices.Add(device);
      }

      public Device GetDeviceInfo(int deviceId) {
         return _Context.Devices.FirstOrDefault(device => device.Id == deviceId);
      }

      public bool RemoveDevice(int id) {
         var device = _Context.Devices.FirstOrDefault(dvc => dvc.Id == id);
         if (device == null) {
            return false;
         }
         _Context.Devices.Remove(device);
         return true;
      }

      public void Dispose() {
         _Context.SaveChanges();
         _Context.Dispose();
      }
   }
}