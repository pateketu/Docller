using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Docller.UI.Common
{
    public class CachedFileResult : FilePathResult
    {
        public CachedFileResult(string fileName, string contentType) : base(fileName, contentType)
        {
        }
        protected override void WriteFile(HttpResponseBase response)
        {
            base.WriteFile(response);
            FileInfo info = new FileInfo(this.FileName);
            response.Cache.SetLastModified(info.LastWriteTime);
            response.Cache.SetExpires(DateTime.Now.AddDays(7));
        }
    }
}