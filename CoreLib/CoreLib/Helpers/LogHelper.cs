using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Reflection;

namespace CoreLib.Helpers
{
    
    public class LogHelper
    {
       
        private static object sync = new object();

        public static void Write(string obj)
        {
            try
            {
                // Путь
                string pathToLog = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Log");
                if (!Directory.Exists(pathToLog))
                    Directory.CreateDirectory(pathToLog); // Создаем директорию
                string filename = Path.Combine(pathToLog, $"{AppDomain.CurrentDomain.FriendlyName}_{DateTime.Now:dd.MM.yyy}.log");
                string fullText = obj+Environment.NewLine;
                lock (sync)
                {
                    File.AppendAllText(filename, fullText, Encoding.UTF8);
                }
            }
            catch
            {
               
            }
        }
    }
}
