using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Docller.Core.Models;

namespace Docller.Core.Services
{
    public class DistributionExtractor
    {
        private readonly long _customerId;
        private readonly long _projectId;
        private readonly IEnumerable<SubscriberItem> _to;
        private readonly IEnumerable<SubscriberItem> _cc;
        private readonly Dictionary<int, TransmittalUser> _transmittalUsers;
        private Dictionary<long, Company> _companies;
        public DistributionExtractor(long customerId, long projectId, IEnumerable<SubscriberItem> to,
                                                             IEnumerable<SubscriberItem> cc)
        {
            _customerId = customerId;
            _projectId = projectId;
            _to = to;
            _cc = cc;
            if (_to == null)
                throw new ArgumentNullException("to");

            if (_cc == null)
            {
                _cc = new List<SubscriberItem>();
            }
            _transmittalUsers = new Dictionary<int, TransmittalUser>();
        }

        public List<TransmittalUser> Extract()
        {
            AddTransmittalCompanies();
            AddTransmittalUser();
            return this._transmittalUsers.Values.ToList();
        }

        private void AddTransmittalCompanies()
        {
            //figure out first if there are companies 
            List<SubscriberCompany> toCompanies = _to.OfType<SubscriberCompany>().ToList();
            List<SubscriberCompany> ccCompanies = _cc.OfType<SubscriberCompany>().ToList();

            if (toCompanies.Any() || ccCompanies.Any())
            {
                //we need to get companies to get all users in it
                ICustomerSubscriptionService customerSubscriptionService =
                ServiceFactory.GetCustomerSubscriptionService(_customerId);

                IEnumerable<Company> c = customerSubscriptionService.GetSubscribedCompanies(_projectId);
                _companies = c.ToDictionary(x => x.CompanyId);
                AddTransmittalUser(toCompanies,false);
                AddTransmittalUser(ccCompanies, true);

            }
        }

        private void AddTransmittalUser()
        {
            List<SubscriberUser> toUsers = _to.OfType<SubscriberUser>().ToList();
            List<SubscriberUser> ccUsers= _cc.OfType<SubscriberUser>().ToList();
            AddTransmittalUser(toUsers.Select(x => x.UserId),false);
            AddTransmittalUser(ccUsers.Select(x => x.UserId), true);
        }


        private void AddTransmittalUser(IEnumerable<SubscriberItem> companies,
                                        bool isCCed)
        {
            foreach (SubscriberCompany subsciberCompany in companies)
            {
                if (_companies.ContainsKey(subsciberCompany.CompanyId))
                {
                    IEnumerable<User> users = _companies[subsciberCompany.CompanyId].Users;
                    AddTransmittalUser(users.Select(x=>x.UserId), isCCed);
                }
            }
        }

        private void AddTransmittalUser(IEnumerable<int> users, bool isCced)
        {
            if (users != null)
            {
                foreach (int userId in users)
                {
                    if (!_transmittalUsers.ContainsKey(userId))
                    {
                        _transmittalUsers.Add(userId, new TransmittalUser() { UserId = userId, IsCced = isCced });
                    }
                }
            }
        }

    }
}
