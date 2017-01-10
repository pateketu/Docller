using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Docller.Core.Models;

namespace Docller.Common
{
    public class TransmittalViewModelBinder:DefaultModelBinder
    {
        protected override void BindProperty(ControllerContext controllerContext, ModelBindingContext bindingContext,
                                             PropertyDescriptor propertyDescriptor)
        {
            if (propertyDescriptor.Name.Equals("To")
                || propertyDescriptor.Name.Equals("Cc"))
            {
                string values = controllerContext.HttpContext.Request.Form[propertyDescriptor.Name];
                SubscriberItemCollection itemCollection = new SubscriberItemCollection(values);
                propertyDescriptor.SetValue(bindingContext.Model, itemCollection);
                
            }else if (propertyDescriptor.Name.Equals("TransmittedFiles"))
            {
                propertyDescriptor.SetValue(bindingContext.Model, GetFilesValue(controllerContext));
            }
            else if(propertyDescriptor.Name.Equals("Action"))
            {
                string action = controllerContext.HttpContext.Request[TransmittalButtons.SaveTransmittal] != null
                                    ? TransmittalButtons.SaveTransmittal
                                    : TransmittalButtons.Transmit;
                
                propertyDescriptor.SetValue(bindingContext.Model, action);
            }
            else
            {
                base.BindProperty(controllerContext, bindingContext, propertyDescriptor);
            }
        }

        private List<TransmittedFile> GetFilesValue(ControllerContext controllerContext)
        {
            string idValues = controllerContext.HttpContext.Request.Form["FileId"];
            List<TransmittedFile> transmittedFiles = new List<TransmittedFile>();
            if (!string.IsNullOrEmpty(idValues))
            {
                string[] ids = idValues.Split(",".ToCharArray());

                string[] fileNames = controllerContext.HttpContext.Request.Form["FileName"].Split(",".ToCharArray());
                string[] titles = controllerContext.HttpContext.Request.Form["Title"].Split(",".ToCharArray());
                string[] revs = controllerContext.HttpContext.Request.Form["Revision"].Split(",".ToCharArray());
                //string[] statuses = controllerContext.HttpContext.Request.Form["Status"].Split(",".ToCharArray());
                

                for (int index = 0; index < ids.Length; index++)
                {
                    TransmittedFile tFile = new TransmittedFile
                        {
                            FileId = int.Parse(ids[index]),
                            FileName = fileNames[index],
                            Title = titles[index],
                            Revision = revs[index],
                            //Status = statuses[index]
                        };
                    transmittedFiles.Add(tFile);
                }
            }
            return transmittedFiles;
        }


    }
}