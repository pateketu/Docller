using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Docller.Core.Services;
using StructureMap;

namespace Docller.Core.Common
{
    public static class DocllerContext
    {
        /// <summary>
        /// Gets the current.
        /// </summary>
        public static IDocllerContext Current
        {
            get
            {
                IDocllerContext context = ObjectFactory.TryGetInstance<IDocllerContext>();
                if (context == null)
                {
                    throw new NoDocllerContextException();
                }
                return context;
            }
        }
    }
}
