using System;
using System.Web.Caching;

namespace Docller.Core.Common
{
    public class DocllerWebCache : ICache
    {
        private readonly Cache _cache;
        public DocllerWebCache(Cache cache)
        {
            this._cache = cache;
        }
        #region Implementation of ICache

        public object this[string key]
        {
            get { return this._cache[key]; }
        }

        public void Add(string key, object value, CacheDependency dependencies, DateTime absoluteExpiration, TimeSpan slidingExpiration, CacheItemPriority priority, CacheItemRemovedCallback onRemoveCallback)
        {
            this._cache.Add(key, value, dependencies, absoluteExpiration, slidingExpiration, priority, onRemoveCallback);
        }

        public void AddSlidingExpiration(string key, object value, CacheDurationHours duration)
        {
            this.Add(key, value, null, Cache.NoAbsoluteExpiration,
                     new TimeSpan(0, (int)duration, 0, 0), CacheItemPriority.Normal, null);
        }

        public void Remove(string key)
        {
            this._cache.Remove(key);
        }

        #endregion
    }
}