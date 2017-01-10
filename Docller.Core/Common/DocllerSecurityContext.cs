using System.Collections.Generic;
using System.Linq;
using Docller.Core.Models;
using Docller.Core.Services;

namespace Docller.Core.Common
{
    public class DocllerSecurityContext : ISecurityContext
    {

        private Project CurrentProject { get; set; }
        private Folder CurrentFolder { get; set; }
        private  PermissionFlag CurrentCustomerPermissions { get; set; }
        public IEnumerable<Project> AvailableProjects { get; set; }
        
        
        public bool IsCustomerAdmin { get; set; }
        public bool CanCreateProject
        {

            get
            {
                PermissionFlag effectivePermissions = GetEffectiveCustomerPermissions();
                return effectivePermissions.HasPermissions(PermissionFlag.CreateProject) ||
                       effectivePermissions.HasPermissions(PermissionFlag.Admin);
            }
        }

        public bool CanAccessCurrentProject
        {
            get
            {
                return GetEffectiveProjectPermissions() != PermissionFlag.None;
            }
        }

        public bool CanAccessCurrentFolder
        {
            get { return GetEffectiveFolderPersmissions() != PermissionFlag.None; }
        }

        public bool CanAdminProject
        {
            get { return (GetEffectiveProjectPermissions().HasPermissions(PermissionFlag.Admin)); }
        }

        public bool CanAdminFolder
        {
            get { return GetEffectiveFolderPersmissions().HasPermissions(PermissionFlag.Admin); }
        }

        public bool CanCreateFolder {
            get { return HasReadWriteAccess; }
        }

        public bool CanCreateTransmittal
        {
            get { return HasReadWriteAccess; }
        }

        public bool HasReadWriteAccess
        {
            get
            {
                PermissionFlag effectivePermissions = GetEffectiveFolderPersmissions();
                return effectivePermissions.HasPermissions(PermissionFlag.ReadWrite) || effectivePermissions.HasPermissions(PermissionFlag.Admin);
            }
        }

        public bool HasReadAccess
        {
            get
            {
                PermissionFlag effectivePermissions = GetEffectiveFolderPersmissions();
                return effectivePermissions.HasPermissions(PermissionFlag.Read) ||
                       effectivePermissions.HasPermissions(PermissionFlag.DefaultFlag);
            }
        }

        public bool CanViewAllFiles
        {
            get
            {
                PermissionFlag effectivePermissions = GetEffectiveFolderPersmissions();
                return effectivePermissions.HasPermissions(PermissionFlag.Read) ||
                       effectivePermissions.HasPermissions(PermissionFlag.ReadWrite) ||
                       effectivePermissions.HasPermissions(PermissionFlag.Admin);
            }
        }

        public void Refresh(long customerId, string userlogin, long currentProjectId, User user)
        {
            if (user != null)
            {
                Set(user);
            }
            else if(AvailableProjects == null) //if is loaded from Session AvailableProjects will be populated
            {
                LoadUserInfo(userlogin,customerId);
            }
            
            if (CurrentProjectRefreshRequired(currentProjectId))
            {
                RefreshCurrentProject(currentProjectId);
            }
        }

        public void Refresh(long customerId, string userlogin)
        {
            LoadUserInfo(userlogin,customerId);
        }

        private void Set(User user)
        {
            this.AvailableProjects = user.Projects;
            this.IsCustomerAdmin = user.IsCustomerAdmin;
            this.CurrentCustomerPermissions = user.CustomerPermissions;
        }
        private void LoadUserInfo(string userName, long customerId)
        {
            IUserService userService = ServiceFactory.GetUserService();
            User user = userService.GetUserInfo(userName, customerId,false);
            if (user != null)
            {
               Set(user);
            }
            else
            {
                this.AvailableProjects = new List<Project>();
            }

        }



        public void Refresh(Folder folder)
        {
            this.CurrentFolder = folder;
        }

        private void RefreshCurrentProject(long projectId)
        {
            CurrentProject = this.AvailableProjects.FirstOrDefault(p => p.ProjectId == projectId);
        }

        private bool CurrentProjectRefreshRequired(long projectId)
        {
            return this.CurrentProject == null || this.CurrentProject.ProjectId != projectId;
        }

        private PermissionFlag GetEffectiveCustomerPermissions()
        {
            if (IsCustomerAdmin)
            {
                return PermissionFlag.AllFlags;
            }
            return this.CurrentCustomerPermissions;
        }

        private PermissionFlag GetEffectiveProjectPermissions()
        {
            PermissionFlag permissions;
            if (this.IsCustomerAdmin)
            {
                permissions = PermissionFlag.AllFlags;
                
            }else if (CurrentProject != null)
            {
                permissions = CurrentProject.CurrentUserPermissions == PermissionFlag.Admin
                                  ? PermissionFlag.AllFlags
                                  : CurrentProject.CurrentUserPermissions;
            }
            else
            {
                permissions = PermissionFlag.None;
            }
            return permissions;
        }

        private PermissionFlag GetEffectiveFolderPersmissions()
        {
            PermissionFlag projectPermissions = this.GetEffectiveProjectPermissions();
            PermissionFlag folderPersmissions;
            if (projectPermissions.HasPermissions(PermissionFlag.Admin))
            {
                folderPersmissions = PermissionFlag.Admin;

            }else if (projectPermissions.HasPermissions(PermissionFlag.ReadWrite))
            {
                folderPersmissions = PermissionFlag.DefaultFlag |
                                     PermissionFlag.ReadWrite | PermissionFlag.Read;
            }
            else if (CurrentFolder != null)
            {
                folderPersmissions = CurrentFolder.CurrentUserPermissions;
            }
            else
            {
                folderPersmissions = PermissionFlag.None;
            }
            return folderPersmissions;
        }
        
    }
}