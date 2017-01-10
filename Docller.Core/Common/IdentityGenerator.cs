using System;
using System.Globalization;
using Docller.Core.Resources;
using Docller.Core.Services;
using SnowMaker;
using StructureMap;

namespace Docller.Core.Common
{
    public static class IdentityGenerator
    {
        //private const string CustomerFormat = "Customer_{0}_";
        private const string ScopeFormat = "{0}_Ids";

        /// <summary>
        /// Creates A Long Id for the scope.
        /// </summary>
        /// <param name="identityScope">The identity scope.</param>
        /// <returns></returns>
        public static long Create(IdentityScope identityScope)
        {
            return Create(identityScope, Context.CustomerId);
        }

        /// <summary>
        /// Creates the specified identity scope.
        /// </summary>
        /// <param name="identityScope">The identity scope.</param>
        /// <param name="customerId">The customer id.</param>
        /// <returns></returns>
        public static long Create(IdentityScope identityScope, long customerId)
        {
            if (customerId <= 0) throw new ArgumentException(StringResources.ArgumnetException_InvalidCustomerId);
            //string scopePrefix = string.Format(CultureInfo.InvariantCulture, CustomerFormat, customerId);
            //string scope = string.Concat(scopePrefix, Enum.GetName(typeof(IdentityScope), identityScope));
            string scope = string.Format(ScopeFormat, Enum.GetName(typeof (IdentityScope), identityScope));
            IUniqueIdGenerator idGenerator = Factory.GetInstance<IUniqueIdGenerator>();
            return idGenerator.NextId(scope);
        }

        private static IDocllerContext Context
        {
            get
            {
                IDocllerContext context = ObjectFactory.TryGetInstance<IDocllerContext>();
                if (context == null)
                {
                    throw new NoDocllerContextException(typeof(IdentityGenerator));
                }
                return context;
            }
        }
    }
}
