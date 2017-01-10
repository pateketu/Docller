using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Docller.Core.Common
{
    public class ContextInfo
    {
        public string CurrentUserName { get; set; }
        public long CurrentProjectId { get; set; }
        public long CurrentCustomerId { get; set; }
    }
}
