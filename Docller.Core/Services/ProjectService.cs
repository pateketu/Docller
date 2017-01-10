using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.ServiceModel.Security;
using Docller.Core.Common;
using Docller.Core.Common.DataStructures;
using Docller.Core.Models;
using Docller.Core.Repository;
using Docller.Core.Storage;

namespace Docller.Core.Services
{
    public class ProjectService : ServiceBase<IProjectRepository>, IProjectService
    {
        private readonly IBlobStorageProvider _blobStorageProvider;
        private ISecurityService _securityService;

        public ProjectService(IProjectRepository repository, IBlobStorageProvider blobStorageProvider)
            : base(repository)
        {
            _blobStorageProvider = blobStorageProvider;
        }

        public ProjectServiceStatus Create(string userName, Project project)
        {
            
            project.ProjectId = IdentityGenerator.Create(IdentityScope.Project);
            project.BlobContainer = string.Format(CultureInfo.InvariantCulture, StorageHelper.BlobContainerFormat,
                                                  this.Context.CustomerId, project.ProjectId);
            int result = this.Repository.Create(userName, PermissionFlag.Admin, project,
                                                GetDefaultStatus(this.Context.CustomerId));

            ProjectServiceStatus status = (ProjectServiceStatus)result;
            if (status == ProjectServiceStatus.Success)
            {
                //BlobStorageProvider uses the same BlobContainerFormat to create the Blob
                project.CustomerId = this.Context.CustomerId;
                this._blobStorageProvider.CreateContainer(project);
                IStorageService storageService = ServiceFactory.GetStorageService(this.Context.CustomerId);
                storageService.CreateFolders(userName, project.ProjectId, GetCommonFolders(project));
                this.Context.Security.Refresh(this.Context.CustomerId, userName);
            }

            return status;
        }

        public Project GetProjectDetails(string userName, long projectId)
        {
            return this.Repository.GetProjectDetails(userName, projectId);
        }

        public IEnumerable<Status> GetProjectStatuses(long projectId)
        {
            return this.Repository.GetProjectStatuses(projectId);
        }

        public void UpdateProject(Project project)
        {
            this.Repository.UpdateProject(project);
        }

        #region Private Methods


        private ISecurityService GetSecurityService(long customerId)
        {
            return this._securityService ?? (this._securityService = ServiceFactory.GetSecurityService(customerId));
        }

        private bool IsProjectAdmin(ContextInfo context)
        {
            return this.GetSecurityService(context.CurrentCustomerId).IsProjectAdmin(context);
        }

       
        private static List<Status> GetDefaultStatus(long customerId)
        {
            string[] allStatus = Config.GetValue<string>(ConfigKeys.DefaultStatus).Split(',');
            return
                allStatus.Select(
                    status =>
                    new Status()
                        {StatusText = status, StatusId = IdentityGenerator.Create(IdentityScope.Status, customerId)}).
                    ToList();
        }
        private static List<Folder> GetCommonFolders(Project project)
        {
            List<Folder> folders = new List<Folder>();
            string[] defaultfolders = Config.GetValue<string>(ConfigKeys.DefaultFolders).Split(',');
            foreach (string foldername in defaultfolders)
            {
                folders.Add(new Folder()
                {
                    FolderName = foldername,
                    CreatedDate = DateTime.Now,
                    CustomerId = project.CustomerId,
                    ParentFolderId = 0
                });
            }
            return folders;
        }

        #endregion



    }
}

