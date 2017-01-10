using System.Linq;
using System.Web;
using ActionMailer.Net.Mvc;
using Docller.Core.Common;
using Docller.Core.Models;
using Docller.Models;
using Docller.UI.Models;

namespace Docller.Controllers
{
    public class MailController : MailerBase
    {
        //
        // GET: /Mail/

        public EmailResult WelcomeEmail(CustomerAdminUserViewModel customer)
        {
            To.Add(customer.Email);
            From = Config.GetValue<string>(ConfigKeys.NoReplyEmailAddress);
            Subject = "Docller New Subscripton";
            return Email("WelcomeEmail",customer);
        }

        public EmailResult NewCustomerEmail(CustomerAdminUserViewModel customer)
        {
            To.Add(customer.Email);
            From = Config.GetValue<string>(ConfigKeys.NoReplyEmailAddress);
            Subject = "Docller New Subscripton";
            return Email("NewCustomerEmail", customer);
        }

        public EmailResult ForgotPasswordEmail(UserDetailsViewModel userdetails)
        {
            To.Add(userdetails.UserName);
            From = Config.GetValue<string>(ConfigKeys.NoReplyEmailAddress);
            Subject = "Docller forgot password recovery email";
            return Email("ForgotPasswordEmail", userdetails);
        }

        public EmailResult InviteUserEmail(InviteUserEmailViewModel invitedUser)
        {
            To.Add(invitedUser.To);
            From = Config.GetValue<string>(ConfigKeys.NoReplyEmailAddress);
            Subject = string.Format("{0} -- Invitation to Collaborate on Docller", invitedUser.ProjectName);
            return Email("InviteUserEmail", invitedUser);
            
        }

        public EmailResult ShareFolderEmail(ShareFolderEmailViewModel viewModel)
        {
            To.Add(viewModel.To);
            From = Config.GetValue<string>(ConfigKeys.NoReplyEmailAddress);
            Subject = string.Format("{0} -- Invitation to Collaborate on Docller", viewModel.ProjectName);
            return Email("ShareFolderEmail", viewModel);
        }


        public EmailResult ProjectInviteEmail(InviteUserEmailViewModel viewModel)
        {
            To.Add(viewModel.To);
            From = Config.GetValue<string>(ConfigKeys.NoReplyEmailAddress);
            Subject = string.Format("{0} -- Invitation to Collaborate on Docller", viewModel.ProjectName);
            return Email("ProjectInviteEmail", viewModel);
        }

        public EmailResult SharedFilesEmail(ShareFilesEmailViewModel shareFilesEmailViewModel)
        {
            To.Add(shareFilesEmailViewModel.To);
            From = Config.GetValue<string>(ConfigKeys.NoReplyEmailAddress);
            Subject = shareFilesEmailViewModel.Files.Count() > 1
                ? string.Format("{0} has sent you files on Docller", shareFilesEmailViewModel.SharedBy)
                : string.Format("{0} has send you a file on Docller", shareFilesEmailViewModel.SharedBy);

            return Email("ShareFilesEmail", shareFilesEmailViewModel);


        }
    }
}
