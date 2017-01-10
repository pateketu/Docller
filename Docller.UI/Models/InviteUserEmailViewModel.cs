using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Docller.Core.Common;

namespace Docller.UI.Models
{
    public class InviteUserEmailViewModel
    {
        public string ProjectName { get; set; }
        public string InvitationFrom { get; set; }
        public string From { get; set; }
        public string To { get; set; }
        public string Password { get; set; }
        public bool IsNew { get; set; }
        public PermissionFlag Permissions { get; set; }
    }
}