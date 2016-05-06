using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreLib.Entity {
   public class EntityProvider : IDisposable {
      private EntityDataModelContainer _Model = new EntityDataModelContainer();

      public User GetUserByCredentials(string login, string password) {
         return _Model.Users.FirstOrDefault(user => user.Login == login && user.Password == password);
      }

      public User GetUserById(int id) {
         return _Model.Users.FirstOrDefault(user => user.Id == id);
      }

      public string CreateSessionKey(User user) {
         string sessionKey = Guid.NewGuid().ToString();
         if(user.SessionKey == null) {
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

      public User GetUserByKey(string strSessionKey) {
         SessionKey sessionKey = _Model.SessionKeys.FirstOrDefault(key => key.Key == strSessionKey);
         if(sessionKey == null) {
            return null;
         }
         return _Model.Users.FirstOrDefault(user => user.SessionKey.Key == sessionKey.Key);
      }

      public bool DeleteUser(int userId) {
         var user = _Model.Users.FirstOrDefault(usr => usr.Id == userId);
         if(user == null) {
            return false;
         }

         var key = _Model.SessionKeys.FirstOrDefault(sessionKey => sessionKey.User.Id == user.Id);
         if (key != null) {
            _Model.SessionKeys.Remove(key);
         }
         _Model.Users.Remove(user);

         return true;
      }

      public void Dispose() {
         _Model.SaveChanges();
         _Model.Dispose();
      }
   }
}