using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Docller.Core.Models
{
    public class UserInvitationError
    {
        //public int UserId { get; set; }
        public string UserName { get; set; }
        public string CompanyName { get; set; }
        //public long CompanyId { get; set; }
        public string ExistingCompany { get; set; }
    }
}
