using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Docller.Core.Common;
using Docller.UI.Models;

namespace Docller.UI.Common
{
    public class AttachmentViewModelBinder:DefaultModelBinder
    {
        public override object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
        {
            FileAttachmentViewModel viewModel = new FileAttachmentViewModel();
            if (controllerContext.HttpContext.Request.Files != null &&
                controllerContext.HttpContext.Request.Files.Count == 1)
            {
                HttpPostedFileBase httpPostedFileBase = controllerContext.HttpContext.Request.Files[0];
                if (httpPostedFileBase != null)
                    viewModel.FileStream = httpPostedFileBase.InputStream;
            }

            long fileId;
            if (long.TryParse(controllerContext.HttpContext.Request[RequestKeys.ParentFile], out fileId))
            {
                viewModel.FileId = fileId;
            }
            int chunks;
            if (int.TryParse(controllerContext.HttpContext.Request[RequestKeys.Chunks], out chunks))
            {
                viewModel.TotalChunks = chunks;
            }
            int chunk;
            if (int.TryParse(controllerContext.HttpContext.Request[RequestKeys.Chunk], out chunk))
            {
                viewModel.CurrentChunk = chunk;
            }
            decimal fileSize;
            if (decimal.TryParse(controllerContext.HttpContext.Request[RequestKeys.FileSize], out fileSize))
            {
                viewModel.FileSize = fileSize;
            }
            viewModel.ProjectId = long.Parse(controllerContext.RouteData.Values[RequestKeys.ProjectId].ToString());
            viewModel.FolderId = long.Parse(controllerContext.RouteData.Values[RequestKeys.FolderId].ToString());

            viewModel.FileName = controllerContext.HttpContext.Request[RequestKeys.Name];

            return viewModel;
        }
    }
}