using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Docller.UI.Common
{
    public class MarkupFileUploadModelBinder : DefaultModelBinder
    {
        protected override void BindProperty(ControllerContext controllerContext, ModelBindingContext bindingContext, System.ComponentModel.PropertyDescriptor propertyDescriptor)
        {
            if (propertyDescriptor.Name.Equals("FileStream"))
            {
                HttpPostedFileBase httpPostedFileBase = controllerContext.HttpContext.Request.Files[0];
                if (httpPostedFileBase != null)
                    propertyDescriptor.SetValue(bindingContext.Model, httpPostedFileBase.InputStream);
            }
            else
            {
                base.BindProperty(controllerContext, bindingContext, propertyDescriptor);    
            }
            
        }
    }
}