using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Docller.Core.Common;
using Docller.Core.Models;

namespace Docller.Core.Common
{
    public interface ISecurityContext
    {
        IEnumerable<Project> AvailableProjects { get; set; }
        bool IsCustomerAdmin { get; set; }
        bool CanCreateProject { get; }
        bool CanAccessCurrentProject { get; }
        bool CanAccessCurrentFolder { get; }
        bool CanAdminProject { get; }
        bool CanAdminFolder { get; }
        bool CanCreateFolder { get; }
        bool CanCreateTransmittal { get; }
        bool HasReadWriteAccess { get; }
        bool HasReadAccess { get; }
        bool CanViewAllFiles { get; }
        void Refresh(long customerId, string userlogin, long currentProjectId, User user);
        void Refresh(long customerId, string userlogin);
        void Refresh(Folder folder);

    }
}
