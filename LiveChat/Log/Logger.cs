using System;
using System.Collections.Generic;
using System.IO;
namespace Log
{

   static class Logger
    {
        private const bool isOn = false;

        public static string Path { get; set; }

        private static StreamWriter sw;
        private static FileStream f;
        private static object lockObj = new object();

        static Logger()
        {
            
        }

        public static void Start()
        {
            if(!isOn) return;
            f = new FileStream(Path, FileMode.Create);
            sw = new StreamWriter(f);
        }

        public static void Clear()
        {
            if (!isOn) return;
            lock (lockObj)
            {
                f.Close();
                f = new FileStream(Path, FileMode.Create);
                sw = new StreamWriter(f);
            }
        }

        public static string GetHTMLAndClear()
        {
            if (!isOn) return "";
            string res = "";
            lock (lockObj)
            {
                f.Close();
                using (var f2 = new FileStream(Path, FileMode.Open))
                {
                    using (StreamReader sr = new StreamReader(f2))
                    {
                        while (!sr.EndOfStream)
                            res += sr.ReadLine() + "<br/>";
                    }
                }
                f = new FileStream(Path, FileMode.Create);
                sw = new StreamWriter(f);
            }
            return res;
        }

        public static void LogMessage(string s)
        {
            if (!isOn) return;
            if (f == null) Start();
            lock(lockObj)
            {
                //instance.sw.WriteLineAsync(s).Wait();
                sw.WriteLine(s);
                Flush();
            }
        }

        public static void Flush()
        {
            if (!isOn) return;
            sw.Flush();
        }

    }
}
