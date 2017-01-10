using System;

namespace Docller.Core.Common
{
    public static class CacheHelper
    {
        public static T GetOrSet<T>(string key, Func<T> getter) where T : class
        {
            return GetOrSet<T>(key,CacheDurationHours.Default,getter);
        }
        public static T GetOrSet<T>(string key,CacheDurationHours cacheDuration, Func<T> getter) where T : class
        {
            IDocllerContext context = DocllerContext.Current;
            object value = context.Cache[key];
            if (value != null)
            {
                return value as T;
            }
            if (getter != null)
            {
                T t = getter();
                context.Cache.AddSlidingExpiration(key, t, cacheDuration);
                return t;
            }
            return null;
        }
    }
}