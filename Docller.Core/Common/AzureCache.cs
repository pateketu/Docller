using System;
using Microsoft.ApplicationServer.Caching;

namespace Docller.Core.Common
{
    public class AzureCache : ICache
    {
        private readonly DataCache _dataCache;
        public AzureCache()
        {
            _dataCache = new DataCache();
        }
        #region Implementation of ICache

        public object this[string key]
        {
            get { return _dataCache[key]; }
        }

        public void AddSlidingExpiration(string key, object value, CacheDurationHours duration)
        {
            _dataCache.Add(key, value, new TimeSpan(0, (int)duration, 0, 0));
        }

        public void Remove(string key)
        {
            _dataCache.Remove(key);
        }

        #endregion
    }
}