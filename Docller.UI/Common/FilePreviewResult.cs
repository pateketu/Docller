using System;
using System.Web;
using System.Web.Mvc;

namespace Docller.UI.Common
{
    public class FilePreviewResult:FilePathResult
    {
        private readonly DateTime _modifieDateTime;
        public FilePreviewResult(string fileName, string contentType, long modifiedTimestamp) : base(fileName, contentType)
        {
            _modifieDateTime = new DateTime(modifiedTimestamp);
        }

        protected override void WriteFile(HttpResponseBase response)
        {
            base.WriteFile(response);
            response.Cache.SetLastModified(_modifieDateTime);
            response.Cache.SetExpires(_modifieDateTime.AddDays(30));
        }
    }
}