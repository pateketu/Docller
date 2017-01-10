using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using System.Web.Routing;
using Docller.Controllers;
using Docller.Core.Common;
using Docller.Core.Common.DataStructures;
using Docller.Core.Models;
using Docller.Serialization;
using Newtonsoft.Json;
using StructureMap;

namespace Docller.Common
{
    public static class ViewContextHelper
    {
        public static long GetCurrentProjectId(this ViewContext viewContext)
        {
            return Factory.GetInstance<IDocllerContext>().ProjectId;
        }

        public static long GetCurrentFolderId(this ViewContext viewContext)
        {
            IDocllerContext docllerContext = Factory.GetInstance<IDocllerContext>();
            if(docllerContext.FolderContext != null)
            {
                return docllerContext.FolderContext.CurrentFolderId;
            }
            return 0;
        }

        public static string GetCurrentProjectName(this ViewContext viewContext)
        {
            DocllerControllerBase controllerBase = viewContext.Controller as DocllerControllerBase;
            if(controllerBase != null)
            {
                return Utils.GetCurrentProjectName(controllerBase.DocllerContext);
            }
            return string.Empty;
        }
        public static string GetCustomerImageUrl(this ViewContext viewContext)
        {
            Customer c = GetCustomer(viewContext);
            return c != null && !string.IsNullOrEmpty(c.ImageUrl) ? string.Format("/Customer/Logo/") : string.Empty;
        }

        public static Customer GetCustomer(this ViewContext viewContext)
        {
            DocllerControllerBase controllerBase = viewContext.Controller as DocllerControllerBase;
            if (controllerBase != null)
            {
                return CustomerCache.Get(controllerBase.DocllerContext.CustomerId);
            }
            return null;
        }

        public static CookieData GetCurrentCookieData(this ViewContext viewContext)
        {
            DocllerControllerBase controllerBase = viewContext.Controller as DocllerControllerBase;
            if(controllerBase != null)
            {
                return controllerBase.CurrentCookieData;
            }
            throw new InvalidCastException("Controllers must inherit from Docller.Controllers.ControllerBase");
        }

        public static string GetFolderJson(this ViewContext viewContext)
        {
            IFolderContext folderContext = Factory.GetInstance<IFolderContext>();

            JsonNetResult jsonResult = new JsonNetResult(new JsonSerializerSettings()
                {
                    ContractResolver = new TreeContractResolver(),
                    NullValueHandling = NullValueHandling.Ignore


                }, "docller.treeData = ")
                {
                    Data = folderContext.AllFolders,
                    JsonRequestBehavior = JsonRequestBehavior.AllowGet
                };
            return jsonResult.GetRawJson();
        }

        public static IHtmlString TopNavBarProjectLink(this HtmlHelper helper, string linkText, bool isDefaultActive, string action, string controller, object routeValues, object htmlAttributes)
        {
            if (helper.ViewContext.GetCurrentProjectId() > 0)
            {
                return helper.TopNavBarLink(linkText, isDefaultActive, action, controller, routeValues, htmlAttributes);
            }
            return new HtmlString(string.Empty);
        }
        public static IHtmlString TopNavBarLink(this HtmlHelper helper, string linkText, bool isDefaultActive, string action, string controller, object routeValues, object htmlAttributes)
        {
            StringBuilder builder = new StringBuilder("<li");
            if ((helper.ViewBag.TopNavArea == null && isDefaultActive)
                    || (helper.ViewBag.TopNavArea != null 
                            &&
                            helper.ViewBag.TopNavArea.ToString().Equals(action, StringComparison.InvariantCultureIgnoreCase)))
            {
                builder.Append(" class=\"active\" ");
            }
            builder.Append(">");
            
            builder.Append(helper.ActionLink(linkText, action, controller, routeValues, htmlAttributes));
            builder.Append("</li>");
            return new HtmlString(builder.ToString());
        }

        public static IHtmlString AttachmentIcon(this HtmlHelper helper, FileVersion fileVersion)
        {
            if (fileVersion.Attachments != null && fileVersion.Attachments.Count == 1)
            {
                return helper.FileTypeIcon(fileVersion.Attachments.First());
            }
            return new HtmlString(string.Empty);
        }

        public static IHtmlString AttachmentIcon(this HtmlHelper helper, File file)
        {
            if (file.Attachments != null && file.Attachments.Count == 1)
            {
                return helper.FileTypeIcon(file.Attachments.First());
            }
            return new HtmlString(string.Empty);
        }
        public static IHtmlString FileAttachmentIcon(this HtmlHelper helper, FileAttachmentVersion fileAttachment)
        {
            return helper.FileAttachmentIcon((FileAttachment)fileAttachment);
        }
        public static IHtmlString FileAttachmentIcon(this HtmlHelper helper, FileAttachment fileAttachment)
        {
            string cssClass = string.Empty;
            string title = string.Empty;
            string extraInfo = string.Empty;
            StringBuilder icon = new StringBuilder("<img src=\"/Images/filetype-icons/");
            FileTypeIcons fileTypeIcons = null;
            title = fileAttachment.FileName;
            cssClass = "filetype-icon-large";
            icon.Append("32X32/");
            fileTypeIcons = FileTypeIconsFactory.Current.Medium;
            icon.AppendFormat("{0}\" class=\"{1}\" title=\"{2}\" {3}></img>", fileTypeIcons[fileAttachment.FileExtension],
                                  cssClass, title, extraInfo);

            return new HtmlString(icon.ToString());
        }
        public static IHtmlString FileTypeIcon(this HtmlHelper helper, BlobBase blobBase)
        {
            string cssClass = string.Empty;
            string title = string.Empty;
            string extraInfo = string.Empty;
            StringBuilder icon = new StringBuilder("<img src=\"/Images/filetype-icons/");
            FileTypeIcons fileTypeIcons = null;
            if (blobBase is FileAttachment)
            {
                title = string.Format("{0} Attachment", blobBase.FileExtension.Remove(0, 1));
                cssClass = "filetype-icon-small";
                icon.Append("16X16/");
                fileTypeIcons = FileTypeIconsFactory.Current.Small;
                extraInfo = string.Format("data-toggle=\"popover\" data-trigger=\"hover\" data-content=\"{0}\"",
                                           blobBase.FileName);
            }
            else if(blobBase is File)
            {
                title = string.Format("Download {0}", blobBase.FileName);
                cssClass = "filetype-icon-large";
                icon.Append("32X32/");
                fileTypeIcons = FileTypeIconsFactory.Current.Medium;
                
            }
            
            if (fileTypeIcons != null)
            {
                icon.AppendFormat("{0}\" class=\"{1}\" title=\"{2}\" {3}></img>", fileTypeIcons[blobBase.FileExtension],
                                  cssClass, title, extraInfo);

                return new HtmlString(icon.ToString());
            }
            return new HtmlString(string.Empty);
        }

        public static IHtmlString VersionBadge(this HtmlHelper helper, File file)
        {
            if (file.VersionCount > 1)
            {
                return
                    new HtmlString(
                        string.Format(
                            "<span class=\"badge badge-version badge-Inverse\" data-toggle=\"tooltip\" title=\"Version {0}\">V{0}</span>",
                            file.VersionCount));
            }
            return new HtmlString(string.Empty);
        }

        public static RouteValueDictionary GetCombinedRouteValues(this ViewContext viewContext, object newRouteValues = null)
        {
            RouteValueDictionary combinedRouteValues = new RouteValueDictionary(viewContext.RouteData.Values);

            NameValueCollection queryString = viewContext.RequestContext.HttpContext.Request.QueryString;
            foreach (string key in queryString.AllKeys.Where(key => key != null))
                combinedRouteValues[key] = queryString[key];

            if (newRouteValues != null)
            {
                return combinedRouteValues.AddAdditionalRouteValues(newRouteValues);
            }

            return combinedRouteValues;
        }

        public static RouteValueDictionary AddAdditionalRouteValues(this RouteValueDictionary routeValueDictionary, object additionalValues)
        {
            if (additionalValues != null)
            {
                foreach (PropertyDescriptor descriptor in TypeDescriptor.GetProperties(additionalValues))
                    routeValueDictionary[descriptor.Name] = descriptor.GetValue(additionalValues);
            }
            return routeValueDictionary;
        }

       
    }
}