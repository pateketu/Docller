using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Docller.Common;
using Docller.Core.Models;
using Docller.Models;
using Docller.UI.Common;
using Docller.UI.Models;

namespace Docller
{
    public class ModelBinders
    {
        public static void RegisterModelBinders()
        {
            System.Web.Mvc.ModelBinders.Binders.Add(typeof(TransmittalViewModel), new TransmittalViewModelBinder());
            System.Web.Mvc.ModelBinders.Binders.Add(typeof(IEnumerable<Company>), new CompaniesModelBinder());
            System.Web.Mvc.ModelBinders.Binders.Add(typeof(FileAttachmentViewModel), new AttachmentViewModelBinder());
            System.Web.Mvc.ModelBinders.Binders.Add(typeof(ShareFilesViewModel), new ShareFilesViewModelBinder());
            System.Web.Mvc.ModelBinders.Binders.Add(typeof(IEnumerable<PermissionInfo>), new UserPermissionInfosModelBinder());
            System.Web.Mvc.ModelBinders.Binders.Add(typeof(MarkupFileUploadModel), new MarkupFileUploadModelBinder());

        }
    }
}