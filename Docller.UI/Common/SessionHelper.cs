using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Docller.Core.Common;

namespace Docller.Common
{
    public static class SessionHelper
    {
        public static T Get<T>(string key) where T : class
        {
            return GetOrSet<T>(key, null);
        }

        public static void Set<T>(string key, T value) where T : class
        {
            DocllerContext.Current.Session[key] = value;
        }
        public static T GetOrSet<T>(string key, Func<T> getter) where T : class
        {
            IDocllerContext context = DocllerContext.Current;
            object value = context.Session[key];
            if(value != null)
            {
                return value as T;
            }
            if (getter != null)
            {
                T t = getter();
                context.Session[key] = t;
                return t;
            }
            return null;
        }

        public static void Invalidate(string key)
        {
            DocllerContext.Current.Session.Remove(key);    
        }

    }

    
}