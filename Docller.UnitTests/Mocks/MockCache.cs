using Docller.Core.Common;

namespace Docller.Tests.Mocks
{
    public class MockCache : ICache
    {
        public object this[string key]
        {
            get { return null; }
        }

        public void AddSlidingExpiration(string key, object value, CacheDurationHours duration)
        {
            //
        }

        public void Remove(string key)
        {
            //
        }
    }
}