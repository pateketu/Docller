using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Docller.Core.DB;
using Docller.Core.Models;
using Docller.Core.Repository;
using Docller.Core.Services;

namespace Docller.UnitTests.Mocks
{
    public class MockCustomerSubscriptionService : CustomerSubscriptionService
    {
        public MockCustomerSubscriptionService() : base(new CustomerSubscriptionRepository(FederationType.None, 0))
        {
        }

        public IEnumerable<SubscriberItem> GetAllSubscribes(long projectId)
        {
            return this.GetSubscribes(projectId);
        }

        public override SubscriberItemCollection GetSubscribers(long projectId, string[] ids)
        {
            return new SubscriberItemCollection();
        }
    }
}
