using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Docller.Core.Common.DataStructures;
using Docller.Core.Models;

namespace Docller.Core.Services
{
    public interface IProjectService
    {
        ProjectServiceStatus Create(string userName, Project project);
        Project GetProjectDetails(string userName, long projectId);
        IEnumerable<Status> GetProjectStatuses(long projectId);
        void UpdateProject(Project project);
    }
}

