using System;
using System.Web.Hosting;

namespace Docller.Core.Common
{
    public class ServerPathMapper : IPathMapper
    {
        #region Implementation of IPathMapper

        public string MapPath(string relativePath)
        {
            return HostingEnvironment.MapPath(relativePath);
        }

        #endregion
    }
}