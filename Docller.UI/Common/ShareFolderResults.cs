using System.Collections.Generic;
using System.Web.Mvc;
using Docller.Controllers;
using Docller.Core.Common;
using Docller.Core.Models;
using Docller.UI.Models;

namespace Docller.Common
{
    public class SubscribeUsersResults : JsonNetResult
    {
        private readonly IEnumerable<User> _newUsers;
        private readonly IDocllerContext _docllerContext;
        private readonly CookieData _cookieData;

        public SubscribeUsersResults(IEnumerable<User> newUsers, IDocllerContext docllerContext, CookieData cookieData):base()
        {
            _newUsers = newUsers;
            _docllerContext = docllerContext;
            _cookieData = cookieData;
        }

        public override void ExecuteResult(ControllerContext context)
        {
            string projectName = Utils.GetCurrentProjectName(this._docllerContext);
            string fromName = this._cookieData.DisplayName;
            MailController mailController = new MailController();
            foreach (User newUser in _newUsers)
            {
                mailController.InviteUserEmail(new InviteUserEmailViewModel()
                    {
                        ProjectName = projectName,
                        InvitationFrom = fromName,
                        To = newUser.Email,
                        Password = newUser.Password,
                    }).Deliver();
            }
            
            base.ExecuteResult(context);
        }
    }
}