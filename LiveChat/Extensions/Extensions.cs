using Log;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LiveChat.Extensions
{
    public static class Extensions
    {
        public static void Add<T, R>(this ConcurrentDictionary<T,R> cd, T key, R value)
        {
           while(!cd.TryAdd(key, value))
            {
                Logger.LogMessage("element" + value.ToString() + "with key" + key.ToString() + " was not added to the dictionary");
                if (cd.ContainsKey(key)) break;
            }
        }

        public static void Remove<T, R>(this ConcurrentDictionary<T, R> cd, T key)
        {
            R tempObject;
            while (!cd.TryRemove(key, out tempObject))
            {
                Logger.LogMessage("element with key" + key.ToString() + " was not removed from dictionary");
                if (!cd.ContainsKey(key)) break;
            }
        }

        public static void LAdd<T>(this HashSet<T> hs, T value)
        {
            lock (hs) hs.Add(value);
        }

        public static void LRemove<T>(this HashSet<T> hs, T value)
        {
            lock (hs) hs.Remove(value);
        }
    }
}