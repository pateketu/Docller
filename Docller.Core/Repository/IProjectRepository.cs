using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Docller.Core.Common;
using Docller.Core.Models;
using Docller.Core.Repository.Collections;

namespace Docller.Core.Repository
{
    public interface IProjectRepository:IRepository
    {
        int Create(string userName, PermissionFlag permissionFlag, Project project, List<Status> defaultStatus);
        Project GetProjectDetails(string userName, long projectId);
        IEnumerable<Status> GetProjectStatuses(long projectId);
        void UpdateProject(Project project);
    }
}
