using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Docller.Core.Models;
using Docller.Core.Services;

namespace Docller.Core.Common
{
    public class CustomerCache
    {
        public static void Set(Customer customer)
        {
            string cacheKey = Utils.GetKeyForCustomer(customer.CustomerId);
            ICache cache = DocllerContext.Current.Cache;
            if (cache[cacheKey] == null)
            {
                cache.AddSlidingExpiration(cacheKey,customer,CacheDurationHours.Long);
            }
        }

        public static Customer Get(long customerId)
        {
            string cacheKey = Utils.GetKeyForCustomer(customerId);
            return CacheHelper.GetOrSet(cacheKey, CacheDurationHours.Long, () => GetCustomer(customerId));
        }

        public static void Invalidate(long customerId)
        {
            string cacheKey = Utils.GetKeyForCustomer(customerId);
            ICache cache = DocllerContext.Current.Cache;
            if (cache[cacheKey] != null)
            {
                cache.Remove(cacheKey);
            }
        }

        private static Customer GetCustomer(long customerId)
        {
            ISubscriptionService subscriptionService = ServiceFactory.GetSubscriptionService();
            return subscriptionService.GetCustomer(customerId);
        }
        
    }
}
