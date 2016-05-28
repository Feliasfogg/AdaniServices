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
                // Путь .\\Log
                string pathToLog = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Log");
                if (!Directory.Exists(pathToLog))
                    Directory.CreateDirectory(pathToLog); // Создаем директорию, если нужно
                string filename = Path.Combine(pathToLog, string.Format("{0}_{1:dd.MM.yyy}.log",
                    AppDomain.CurrentDomain.FriendlyName, DateTime.Now));
                string fullText = obj+Environment.NewLine;
                lock (sync)
                {
                    File.AppendAllText(filename, fullText, Encoding.GetEncoding("utf-8"));
                }
            }
            catch
            {
                // Перехватываем все и ничего не делаем
            }
        }
    }
}
