using System;
using System.Globalization;
using Docller.Core.Resources;

namespace Docller.Core.Services
{
    public class NoDocllerContextException   : Exception
    {
        public NoDocllerContextException()
        {
            
        }
        public NoDocllerContextException(Type type):base(string.Format(CultureInfo.InvariantCulture,StringResources.NoDcollerContext, type.FullName))
        {
            
        }
    }
}