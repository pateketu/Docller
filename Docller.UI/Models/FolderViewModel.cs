using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Docller.Models
{
    public class FolderViewModel
    {
        public long ProjectId { get; set; }
        public long FolderId { get; set; }
        public long ParentFolderId { get; set; }
        [Required]
        public string FolderName { get; set; }

    }
}