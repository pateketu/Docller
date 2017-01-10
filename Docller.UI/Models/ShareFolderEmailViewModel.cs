using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Docller.Core.Common;

namespace Docller.UI.Models
{
    public class ShareFolderEmailViewModel:InviteUserEmailViewModel
    {
        public string Folder { get; set; }
        
    }
}