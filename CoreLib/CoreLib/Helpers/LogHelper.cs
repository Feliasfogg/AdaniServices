using System;
using System.IO;
using System.Text;

namespace CoreLib.Helpers {
   public class LogHelper {
      private static object sync = new object();

      public static void Write(string obj, string folderPath) {
         try {
            // Путь
            if(!Directory.Exists(folderPath)) {
               Directory.CreateDirectory(folderPath); // Создаем директорию
            }
            string filename = Path.Combine(folderPath, $"ServerLog_{DateTime.Now:dd.MM.yyy}.log");
            string fullText = obj + Environment.NewLine;
            lock(sync) {
               File.AppendAllText(filename, fullText, Encoding.UTF8);
            }
         }
         catch {
         }
      }
   }
}