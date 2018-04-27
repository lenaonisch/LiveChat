using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Hosting;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
namespace Log
{

   static class Logger
    {
        //public const string dir = @"~";
        //public const string fileName = "log.txt";

        public static string Path { get; set; }

        private static StreamWriter sw;
        private static FileStream f;
        private static object lockObj = new object();

        static Logger()
        {
            
        }

        public static void Start()
        {
            f = new FileStream(Path, FileMode.Create);
            sw = new StreamWriter(f);
        }

        public static void Clear()
        {
            lock (lockObj)
            {
                f.Close();
                f = new FileStream(Path, FileMode.Create);
                sw = new StreamWriter(f);
            }
        }

        public static string GetHTMLAndClear()
        {
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
            lock(lockObj)
            {
                //instance.sw.WriteLineAsync(s).Wait();
                sw.WriteLine(s);
                Flush();
            }
        }

        public static void Flush()
        {
            sw.Flush();
        }

    }
}
