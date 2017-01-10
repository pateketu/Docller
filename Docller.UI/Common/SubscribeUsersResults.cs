using System.Collections.Generic;
using System.Web.Mvc;
using Docller.Controllers;
using Docller.Core.Common;
using Docller.Core.Models;
using Docller.UI.Models;

namespace Docller.Common
{
    public class ShareFolderResults : JsonNetResult
    {
        private readonly IEnumerable<User> _newUsers;
        private readonly IDocllerContext _docllerContext;
        private readonly CookieData _cookieData;
        private readonly PermissionFlag _permissionFlag;

        public ShareFolderResults(IEnumerable<User> newUsers, IDocllerContext docllerContext, CookieData cookieData, PermissionFlag permissionFlag)
            : base()
        {
            _newUsers = newUsers;
            _docllerContext = docllerContext;
            _cookieData = cookieData;
            _permissionFlag = permissionFlag;
        }

        public override void ExecuteResult(ControllerContext context)
        {
            string projectName = Utils.GetCurrentProjectName(this._docllerContext);
            string fromName = this._cookieData.DisplayName;
            MailController mailController = new MailController();
            string folderName =
                _docllerContext.FolderContext.AllFolders[_docllerContext.FolderContext.CurrentFolderId].FolderName;
            foreach (User newUser in _newUsers)
            {
                mailController.ShareFolderEmail(new ShareFolderEmailViewModel()
                    {
                        ProjectName = projectName,
                        InvitationFrom = fromName,
                        To = newUser.Email,
                        Password = newUser.Password,
                        Folder = folderName,
                        Permissions = _permissionFlag,
                        IsNew = newUser.IsNew
                    }).Deliver();
            }
            
            base.ExecuteResult(context);
        }
    }
}