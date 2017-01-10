using System;
using System.Globalization;
using System.Web;
using Docller.Core.Models;

namespace Docller.Core.Common
{
    public static class Utils
    {
        public static Project GetCurrentProject(IDocllerContext context)
        {
            if (context != null && context.ProjectId > 0)
            {
                string key = GetProjectKey(context);
                Project project = context.Cache[key] as Project;
                return project;
            }
            return null;

        }

        public static string GetDomain(Uri uri)
        {
            return string.Format(CultureInfo.InvariantCulture, "{0}{1}{2}",
                                     uri.Scheme, System.Uri.SchemeDelimiter, uri.Host);
        }

        public static string GetRootUrl(HttpContextBase context)
        {
            return string.Format("{0}{1}", Utils.GetDomain(context.Request.UrlReferrer),
                context.Request.UrlReferrer.Port != 80 ||
                context.Request.UrlReferrer.Port != 443
                    ? string.Concat(":", context.Request.UrlReferrer.Port)
                    : string.Empty);
        }
        public static string GetCurrentProjectName(IDocllerContext context)
        {
            Project project = GetCurrentProject(context);
            return project != null ? project.ProjectName : string.Empty;
        }
        public static string GetCurrentProjectImage(IDocllerContext context)
        {
            Project project = GetCurrentProject(context);
            return project != null ? project.ProjectImage : string.Empty;
        }

        public static string GetKeyForProject(string keyFormat, long cusomerId, long projectId)
        {
            return string.Format(CultureInfo.InvariantCulture, keyFormat,
                                 cusomerId,projectId);
        }

        public static string GetKeyForCustomer(long cusomerId)
        {
            return string.Format(CultureInfo.InvariantCulture, SessionAndCahceKeys.CustomerInfoFormat,
                                 cusomerId);
        }

        public static string GetProjectKey(IDocllerContext context)
        {
            return GetKeyForProject(SessionAndCahceKeys.ProjectNameFormat,
                                                    context.CustomerId, context.ProjectId);
        }

        public static string GetIssueSheetFileName(long transmittalId)
        {
            return string.Format("IssueSheet_{0}.pdf", transmittalId);
        }

        public static BlobBase DeserializeBlobDetails(HttpRequestBase request)
        {
            BlobBase blob = null;
            bool isExisting = !string.IsNullOrEmpty(request.Form[RequestKeys.IsExisting])
                                          ? true
                                          : false;
            if (!string.IsNullOrEmpty(request.Form[RequestKeys.ParentFile]))
            {
                Guid parentFile;
                if(Guid.TryParse(request.Form[RequestKeys.ParentFile], out parentFile))
               {
                   blob = new FileAttachment();
                   ((FileAttachment) blob).ParentFile = parentFile;
                   ((FileAttachment) blob).IsExistingFile = isExisting;
                   ((FileAttachment)blob).BaseFileName = request.Form[RequestKeys.BaseFileName];
               }

            }
            if(blob == null)
            {
                blob = new File();
                ((File)blob).IsExistingFile = isExisting;
                ((File) blob).BaseFileName = request.Form[RequestKeys.BaseFileName];
                ((File)blob).DocNumber = request.Form[RequestKeys.DocNumber];
            }

            int fileSize;
            if(Int32.TryParse(request.Form[RequestKeys.FileSize],out fileSize))
            {
                //decimal kbSize = fileSize/1000;
                blob.FileSize = fileSize;

            }

            long fileId;
            if(long.TryParse(request.Form[RequestKeys.FileId], out fileId))
            {
                blob.FileId = fileId;
            }
            blob.FileName = request.Form[RequestKeys.FileName];
            blob.FileInternalName = !string.IsNullOrEmpty(request.Form[RequestKeys.FileInternalName])
                                        ? new Guid(request.Form[RequestKeys.FileInternalName])
                                        : Guid.Empty;

            blob.Project = GetCurrentProject(DocllerContext.Current);
            blob.Folder = new Folder
                              {
                                  FolderId = DocllerContext.Current.FolderContext.CurrentFolderId,
                                  FullPath = request.Form[RequestKeys.FullPath]
                              };

            return blob;
        }

        
    }
}