using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using Docller.Core.Infrastructure;
using Docller.Core.Models;

namespace Docller.Core.Repository
{

    public interface ISubscriptionRepository : IRepository
    {
        
        /// <summary>
        /// Subscribes the specified customer.
        /// </summary>
        /// <param name="customer">The customer.</param>
        /// <returns></returns>
        CustomerSubInfo Subscribe(Customer customer);

        //int Subscribe(Company company);

        /// <summary>
        /// Determines whether [is domain URL in use] [the specified domain URL].
        /// </summary>
        /// <param name="domainUrl">The domain URL.</param>
        /// <returns>
        ///   <c>true</c> if [is domain URL in use] [the specified domain URL]; otherwise, <c>false</c>.
        /// </returns>
        bool IsDomainUrlInUse(string domainUrl);

        /// <summary>
        /// Gets the customer id.
        /// </summary>
        /// <param name="domainUrl">The domain URL.</param>
        /// <returns></returns>
        Customer GetCustomer(string domainUrl);


        Customer GetCustomer(long customerId);
        void UpdateCustomer(Customer customer);
    }
}
