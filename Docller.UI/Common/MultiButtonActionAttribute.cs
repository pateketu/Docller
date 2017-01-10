using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Mvc;

namespace Docller.Common
{
    public class MultiButtonActionAttribute : ActionNameSelectorAttribute
    {
        public string Button { get; set; }
        public override bool IsValidName(ControllerContext controllerContext, string actionName, MethodInfo methodInfo)
        {
            if (string.IsNullOrEmpty(Button))
            {
                return false;
            }
           return controllerContext.RequestContext.HttpContext.Request[Button] != null;
        }
    }

}