using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Docller.Core.DB;
using Docller.Core.Repository;
using Docller.Core.Services;
using StructureMap;

namespace Docller.Core.Common
{
    public static class Factory
    {

        public static T GetInstance<T>()
        {
            return ObjectFactory.GetInstance<T>();
        }

        internal static T GetRepository<T>(long customerId) where T : IRepository
        {
            return !DocllerEnvironment.IsFederationEnabled
                       ? ObjectFactory.GetInstance<T>()
                       : ObjectFactory.With(FederationType.Member).With(customerId).GetInstance<T>();
        }

        public static ILocalStorage GetLocalStorageProvider()
        {
            return ObjectFactory.GetInstance<ILocalStorage>();
        }

        
    }
}
