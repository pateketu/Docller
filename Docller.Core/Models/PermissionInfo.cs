using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Docller.Core.Common;
using Org.BouncyCastle.Bcpg;

namespace Docller.Core.Models
{
    public class PermissionInfo
    {
        public long EntityId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }

        public string DisplayName
        {
            get
            {
                if (!string.IsNullOrEmpty(this.LastName) || !string.IsNullOrEmpty(LastName))
                {
                    return string.Format(CultureInfo.InvariantCulture, "{0} {1}", this.FirstName, this.LastName).Trim();
                }
                return string.Empty;
            }
        }

        public string Email { get; set; }
        public string CompanyName { get; set; }
        public PermissionFlag  Permissions { get; set; }
        

    }
}
