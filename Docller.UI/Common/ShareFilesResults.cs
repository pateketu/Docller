using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Docller.Common;
using Docller.Controllers;
using Docller.Core.Common;
using Docller.Core.Models;
using Docller.UI.Models;

namespace Docller.UI.Common
{
    public class ShareFilesResults:JsonNetResult
    {
        private readonly SharedFilesInfo _sharedFilesInfo;
        private readonly string[] _to;
        private readonly string _message;
        private readonly IDocllerContext _docllerContext;
        private readonly CookieData _cookieData;

        public ShareFilesResults(SharedFilesInfo sharedFilesInfo, string[] to, string message, IDocllerContext docllerContext, CookieData cookieData):base()
        {
            _sharedFilesInfo = sharedFilesInfo;
            _to = to;
            _message = message;
            _docllerContext = docllerContext;
            _cookieData = cookieData;
        }

        public override void ExecuteResult(ControllerContext context)
        {
            MailController mailController = new MailController();

            foreach (string s in _to)
            {
                ShareFilesEmailViewModel viewModel = new ShareFilesEmailViewModel()
                {
                    Files = _sharedFilesInfo.Files,
                    Message = _message,
                    SharedBy = _cookieData.DisplayName,
                    To = s,
                    TransmittalId = _sharedFilesInfo.TransmittalId,
                    ProjectId = this._docllerContext.ProjectId,
                    DownloadUrl =
                        string.Format("{0}/Download/DownloadShared/{1}?Id={2}&e={3}",
                            Utils.GetRootUrl(context.HttpContext), _docllerContext.ProjectId,
                            _sharedFilesInfo.TransmittalId, s)

                };
                mailController.SharedFilesEmail(viewModel).Deliver();
            }
            base.ExecuteResult(context);
        }
    }
}