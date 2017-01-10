using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Caching;

namespace Docller.Core.Common
{
    public interface ICache
    {
        object this[string key] { get; }
        void AddSlidingExpiration(string key, object value,CacheDurationHours duration);
        void Remove(string key);
    }
}
