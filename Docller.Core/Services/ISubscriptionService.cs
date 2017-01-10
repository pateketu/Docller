using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Docller.Core.Models;

namespace Docller.Core.Services
{
    public interface ISubscriptionService
    {
        SubscriptionServiceStatus Subscribe(Customer customer);
        SubscriptionServiceStatus Subscribe(IEnumerable<User> users);
        bool IsDomainUrlExists(string domainUrl);
        Customer GetCustomer(string domainUrl);
        Customer GetCustomer(long customerId);
        void SaveLogo(long customerId, Stream logoStream, string fileName);
        string LogoFile(long customerId);
    }
}
