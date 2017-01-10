using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Docller.Controllers;
using Docller.Core.Models;
using Docller.UI.Common;

namespace Docller.Common
{
    public class ViewBagWrapper
    {
        private readonly dynamic _viewBag;
        public ViewBagWrapper(DocllerControllerBase controller)
        {
            _viewBag = controller.ViewBag;
        }

        public bool IsDlg
        {
            set { _viewBag.IsDlg = value; }
        }

        public string ReturnUrl
        {
            set { _viewBag.ReturnUrl = value; }
        }

        public string Message
        {
            set { _viewBag.Message = value; }
        }

        public string FormAction
        {
            set { _viewBag.FormAction = value; }
        }

        public string HeaderBrand
        {
            set { _viewBag.HeaderBrand = value; }
        }

        public string TopNavArea
        {
            set { _viewBag.TopNavArea = value; }
        }

        public bool TransmittalSaved
        {
            set { _viewBag.TransmittalSaved = value; }
            
        }

        public bool ShowAsPicker    
        {
            set { _viewBag.ShowAsPicker = value; }
        }

        public bool ThumbnailView
        {
            set { _viewBag.ThumbnailView = value; }
        }

        public string ErrorMessage
        {
            set { _viewBag.ErrorMessage = value; }
        }

        public MyTransmittalSearch? TransmittalSearchOption
        {
            set { _viewBag.TransmittalSearchOption = value; }
        }
    }
}