using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Docller.Core.Common;
using Docller.Core.Repository;
using StructureMap;

namespace Docller.Core.Services
{
    public static class ServiceFactory
    {
        public static  IUserService GetUserService()
        {
            return Factory.GetInstance<IUserService>();
        }

        public static IStorageService GetStorageService(long customerId)
        {
            IStorageRepository storageRepository = Factory.GetRepository<IStorageRepository>(customerId);
            return ObjectFactory.With(storageRepository).GetInstance<IStorageService>();
        }

        public static IUserSubscriptionService GetUserSubscriptionService()
        {
            //IUserSubscriptionRepository subscriptionRepository = Factory.GetRepository<IUserSubscriptionRepository>(customerId);
            return ObjectFactory.GetInstance<IUserSubscriptionService>();
        }
       
        public static ISubscriptionService GetSubscriptionService()
        {
            return Factory.GetInstance<ISubscriptionService>();
        }

        
        public static IProjectService GetProjectService(long customerId)
        {
            IProjectRepository repo = Factory.GetRepository<IProjectRepository>(customerId);
            return ObjectFactory.With(repo).GetInstance<IProjectService>();
        }

        public static ISecurityService GetSecurityService(long customerId)
        {
            ISecurityRepository repo = Factory.GetRepository<ISecurityRepository>(customerId);
            return ObjectFactory.With(repo).GetInstance<ISecurityService>();
        }

        public static ICustomerSubscriptionService GetCustomerSubscriptionService(long customerId)
        {
            ICustomerSubscriptionRepository repo = Factory.GetRepository<ICustomerSubscriptionRepository>(customerId);
            return ObjectFactory.With(repo).GetInstance<ICustomerSubscriptionService>();
        }

        public static ITransmittalService GetTransmittalService(long customerId)
        {
            ITransmittalRepository repo = Factory.GetRepository<ITransmittalRepository>(customerId);
            return ObjectFactory.With(repo).GetInstance<ITransmittalService>();

        }

        
    }
}
