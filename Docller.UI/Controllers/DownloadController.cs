using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Docller.Common;
using Docller.Core.Common;
using Docller.Core.Models;
using Docller.Core.Services;

namespace Docller.Controllers
{
    public class DownloadController : DocllerControllerBase 
    {
        //
        // GET: /Download/Fina


       public async Task<ActionResult> DownloadFiles(long[] fileIds)
        {
            DownloadService downloadService = new DownloadService();
            DownloadState state = await downloadService.DownloadAsync(this.DocllerContext.UserName, this.DocllerContext.CustomerId,
                                              new ClientConnection(this.HttpContext.Response), fileIds);
            if (state.Exception != null)
            {
                Logger.Log(null, state.Exception);
                return new FailedDownloadResult(HttpStatusCode.InternalServerError);
            }
            return new DownloadResult(state.DownloadProvider);
        }

        public async Task<ActionResult> DownloadVersion(long fileId, int version)
        {
            DownloadService downloadService = new DownloadService();
            DownloadState state = await downloadService.DownloadAsync(this.DocllerContext.UserName, this.DocllerContext.CustomerId,
                                              new ClientConnection(this.HttpContext.Response), fileId,version);
            if (state.Exception != null)
            {
                Logger.Log(null, state.Exception);
                return new FailedDownloadResult(HttpStatusCode.InternalServerError);
            }
            return new DownloadResult(state.DownloadProvider); ;
        }
        public async Task<ActionResult> DownloadTransmittal(long projectId, long transmittalId)
        {
            DownloadService downloadService = new DownloadService();
            DownloadState state = await downloadService.DownloadTransmittalAsync(this.DocllerContext.UserName, this.DocllerContext.CustomerId,
                                              new ClientConnection(this.HttpContext.Response), projectId, transmittalId);
            if (state.Exception != null)
            {
                throw state.Exception;
                //return new FailedDownloadResult(HttpStatusCode.InternalServerError);
            }
            return new DownloadResult(state.DownloadProvider);
        }

        [AllowAnonymous]
        public async Task<ActionResult> DownloadShared(long projectId, long id, string e)
        {
            EnsureAnonymousContext(); //This is valid action hence we need to insert the Context for Anonymous user for our backend to regonize the request as valid
            DownloadService downloadService = new DownloadService();
            DownloadState state = await downloadService.DownloadSharedFilesAsync(this.DocllerContext.CustomerId,
                                              new ClientConnection(this.HttpContext.Response), projectId, id, e);
            if (state.Exception != null)
            {
                throw state.Exception;
                //return new FailedDownloadResult(HttpStatusCode.InternalServerError);
            }
            return new DownloadResult(state.DownloadProvider);
        }
       
    }
}
