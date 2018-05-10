using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
namespace Log
{

   static class Logger
    {
        private const bool isOn = true;

        private static List<string> messages = new List<string>();
        private static object lockObj = new object();

        static Logger()
        {
            
        }

        public static void Clear()
        {
            if (!isOn) return;
            lock (lockObj)
            {
                messages.Clear();
            }
        }

        public static string GetHTMLAndClear()
        {
            if (!isOn) return "";
            string res = "";
            lock (lockObj)
            {
                foreach(var s in messages)
                {
                    res += s + "<br/>";
                }
                Clear();
            }
            return res;
        }

        public static void LogMessage(string s)
        {
            if (!isOn) return;
            lock(lockObj)
            {
                messages.Add(s);
            }
        }

    }
}
