using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Docller.UI.Models
{
    public class ShareFilesViewModel
    {
        public long ProjectId { get; set; }
        public long FolderId { get; set; }
        [Required]
        public long[] FileIds { get; set; }
        [Required]
        public string[] Emails { get; set; }
        public string EmailMessage { get; set; }
    }
}