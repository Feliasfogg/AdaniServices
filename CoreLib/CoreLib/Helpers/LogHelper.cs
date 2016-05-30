using System;
using System.IO;
using System.Text;

namespace CoreLib.Helpers {
   public class LogHelper {
      private static object sync = new object();

      public static void Write(string obj) {
         try {
            // Путь
            string pathToLog = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "ServerLogs");
            if(!Directory.Exists(pathToLog)) {
               Directory.CreateDirectory(pathToLog); // Создаем директорию
            }
            string filename = Path.Combine(pathToLog, $"ServerLog_{DateTime.Now:dd.MM.yyy}.log");
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