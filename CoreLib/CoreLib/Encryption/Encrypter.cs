using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace CoreLib.Encryption {
   public static class Encrypter {
      // Эта константа используется для определения размера ключа алгоритма шифрования в битах. 
      // Мы разделим ее на 8, чтобы получить эквивалентное число байтов.
      private const int Keysize = 256;

      //  Эта константа определяет число итераций для функции генерации паролей байт.
      private const int DerivationIterations = 1000;

      public static string GeneratePassword(int length) {
         const string chars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890";
         StringBuilder res = new StringBuilder();
         var random = new Random();
         for(int i = 0; i < length; ++i) {
            res.Append(chars[random.Next(chars.Length)]);
         }
         return res.ToString();
      }

      public static byte[] EncryptData(string data) {
         /*
          * Суть шифрования что мы сначала генерируем случайный 8 символьынй ключ - публичный.
          * На основе приватного ключа мы генерируем его ХЭШ по md5 - приватный ключ
          * затем этим хэшем мы шифруем данные и после шифрования добавляем в конце 8 символов публичного ключа.
          * На сервере будет операция в обратном порядке. Основное условия что и сервер и клиент должны получать 
          * хэш одинаковой хэшфункцией.
          */

         string publicKey = Encrypter.GeneratePassword(8);
         string hash = Encrypter.GenerateHash(publicKey);
         string encryptCommand = Encrypter.Encrypt(data, hash);
         encryptCommand += publicKey;
         return Encoding.ASCII.GetBytes(encryptCommand);
      }

      public static byte[] EncryptData(byte[] data) {
         return EncryptData(Encoding.ASCII.GetString(data));
      }

      public static byte[] DecryptData(byte[] data) {
         /*
          * Данные зашифрованы публичным 8 значным ключом. Мы точно знаем что в нем 8 символов. Мы берем этот ключ 
          * и генерируем на его основе MD5 сумму - приватный ключ. Затем дешифруем сообщение. 
          */
         string strData = Encoding.ASCII.GetString(data);
         string publicKey = strData.Substring(data.Length - 8);
         string privateKey = Encrypter.GenerateHash(publicKey);
         strData = strData.Substring(0, data.Length - 8);
         string decryptString = Encrypter.Decrypt(strData, privateKey);
         return Encoding.ASCII.GetBytes(decryptString);
      }

      private static string Encrypt(string plainText, string passPhrase) {
         var saltStringBytes = Generate256BitsOfRandomEntropy();
         var ivStringBytes = Generate256BitsOfRandomEntropy();
         var plainTextBytes = Encoding.UTF8.GetBytes(plainText);
         using(var password = new Rfc2898DeriveBytes(passPhrase, saltStringBytes, DerivationIterations)) {
            var keyBytes = password.GetBytes(Keysize / 8);
            using(var symmetricKey = new RijndaelManaged()) {
               symmetricKey.BlockSize = 256;
               symmetricKey.Mode = CipherMode.CBC;
               symmetricKey.Padding = PaddingMode.PKCS7;
               using(var encryptor = symmetricKey.CreateEncryptor(keyBytes, ivStringBytes)) {
                  using(var memoryStream = new MemoryStream()) {
                     using(var cryptoStream = new CryptoStream(memoryStream, encryptor, CryptoStreamMode.Write)) {
                        cryptoStream.Write(plainTextBytes, 0, plainTextBytes.Length);
                        cryptoStream.FlushFinalBlock();
                        //Создание байтового массива в виде склеивания случайных байтов
                        var cipherTextBytes = saltStringBytes;
                        cipherTextBytes = cipherTextBytes.Concat(ivStringBytes).ToArray();
                        cipherTextBytes = cipherTextBytes.Concat(memoryStream.ToArray()).ToArray();
                        memoryStream.Close();
                        cryptoStream.Close();
                        return Convert.ToBase64String(cipherTextBytes);
                     }
                  }
               }
            }
         }
      }

      private static string Decrypt(string cipherText, string passPhrase) {
         // Get the complete stream of bytes that represent:
         // [32 bytes of Salt] + [32 bytes of IV] + [n bytes of CipherText]
         var cipherTextBytesWithSaltAndIv = Convert.FromBase64String(cipherText);
         // Получение байт путем извлечения первых 32 байт из байт шифротекста.
         var saltStringBytes = cipherTextBytesWithSaltAndIv.Take(Keysize / 8).ToArray();
         // Получаем байты через извлечение следующих 32 байт из байт шифротекста.
         var ivStringBytes = cipherTextBytesWithSaltAndIv.Skip(Keysize / 8).Take(Keysize / 8).ToArray();
         //Получить фактические заэнкрипченные байты, удалив первые 64 байта из строки шифротекста.
         var cipherTextBytes = cipherTextBytesWithSaltAndIv.Skip((Keysize / 8) * 2).Take(cipherTextBytesWithSaltAndIv.Length - ((Keysize / 8) * 2)).ToArray();

         using(var password = new Rfc2898DeriveBytes(passPhrase, saltStringBytes, DerivationIterations)) {
            var keyBytes = password.GetBytes(Keysize / 8);
            using(var symmetricKey = new RijndaelManaged()) {
               symmetricKey.BlockSize = 256;
               symmetricKey.Mode = CipherMode.CBC;
               symmetricKey.Padding = PaddingMode.PKCS7;
               using(var decryptor = symmetricKey.CreateDecryptor(keyBytes, ivStringBytes)) {
                  using(var memoryStream = new MemoryStream(cipherTextBytes)) {
                     using(var cryptoStream = new CryptoStream(memoryStream, decryptor, CryptoStreamMode.Read)) {
                        var plainTextBytes = new byte[cipherTextBytes.Length];
                        var decryptedByteCount = cryptoStream.Read(plainTextBytes, 0, plainTextBytes.Length);
                        memoryStream.Close();
                        cryptoStream.Close();
                        return Encoding.UTF8.GetString(plainTextBytes, 0, decryptedByteCount);
                     }
                  }
               }
            }
         }
      }

      public static string GenerateHash(string thisPassword) {
         MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider();
         byte[] tmpSource = ASCIIEncoding.ASCII.GetBytes(thisPassword);
         byte[] tmpHash = md5.ComputeHash(tmpSource);
         for(int i = 0; i < 3; ++i) {
            tmpHash = md5.ComputeHash(tmpHash);
         }
         StringBuilder output = new StringBuilder(tmpHash.Length);
         for(int i = 0; i < tmpHash.Length; i++) {
            output.Append(tmpHash[i].ToString("X2")); // X2 для 16 ричного формата
         }
         return output.ToString();
      }

      private static byte[] Generate256BitsOfRandomEntropy() {
         var randomBytes = new byte[32]; // 32 Bytes will give us 256 bits.
         using(var rngCsp = new RNGCryptoServiceProvider()) {
            rngCsp.GetBytes(randomBytes);
         }
         return randomBytes;
      }
   }
}