using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Docller.Core.Common;
using Docller.Core.Models;

namespace Docller.UnitTests.Mocks
{
    public class MockSecurityContext:ISecurityContext
    {
        public IEnumerable<Project> AvailableProjects { get; set; }
        public bool IsCustomerAdmin { get; set; }
        public bool CanCreateProject { get; set; }
        public bool CanAccessCurrentProject { get; private set; }
        public bool CanAccessCurrentFolder { get; private set; }
        public bool CanAdminProject { get { return true; } }
        public bool CanAdminFolder { get { return true; } }
        public bool CanCreateFolder { get { return true; } }
        public bool CanCreateTransmittal { get { return true; } }
        public bool HasReadWriteAccess { get { return true; } }
        public bool HasReadAccess { get; private set; }
        public bool CanViewAllFiles { get { return true; } }
        public void Refresh(long customerId, string userlogin, long currentProjectId, User user)
        {
            //throw new NotImplementedException();
        }

        public void Refresh(long customerId, string userlogin)
        {
            //throw new NotImplementedException();
        }

        public void Refresh(long customerId, string userlogin, long currentProjectId, IEnumerable<Project> availableProjectsForUser)
        {
            //throw new NotImplementedException();
        }

        public void RefreshProjects()
        {
            //throw new NotImplementedException();
        }

        public void Refresh(Folder folder)
        {
            //throw new NotImplementedException();
        }
    }
}
