using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Docller.Core.Services
{
    public enum SubscriptionServiceStatus
    {
        Unknown=-1,
        Success=0,
        ExistingCustomer=101,
        DomainUrlInUse=102,
        ExistingUserNewCustomer=103,
        

    }
}
