using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Docller.Core.Models;
using Docller.UI.Models;
using Newtonsoft.Json;

namespace Docller.UI.Common
{
    public class UserPermissionInfosModelBinder : DefaultModelBinder
    {
        public override object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
        {
            string jsonString = controllerContext.HttpContext.Request["changedPermissions"];
            IEnumerable<PermissionInfo> changedPermissions = JsonConvert.DeserializeObject<IEnumerable<PermissionInfo>>(jsonString);
            return changedPermissions;
        }

    }
}