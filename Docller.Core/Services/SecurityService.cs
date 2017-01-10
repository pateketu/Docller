using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using Docller.Core.Common;
using Docller.Core.Models;
using Docller.Core.Repository;

namespace Docller.Core.Services
{
    public class SecurityService: ServiceBase<ISecurityRepository>, ISecurityService
    {
        public SecurityService(ISecurityRepository repository) : base(repository)
        {
        }

        public bool CanCreateProject(ContextInfo context)
        {
            //IUserService userService = ServiceFactory.GetUserService(context.CurrentCustomerId);
            //User user = userService.GetUserInfo(context.CurrentUserName, context.CurrentCustomerId);
            //return user.IsCustomerAdmin;
            return true;
        }

        public bool CanCreateFolder(ContextInfo context, long parentFolderId)
        {
            //int permissionMask = this.Repository.GetFolderPermissions(context.CurrentUserName, context.CurrentProjectId,
            //                                                          parentFolderId);
            //bool canCreate = (permissionMask & (int)PermissionFlag.ReadWrite) == (int)PermissionFlag.ReadWrite;
            //if(!canCreate)
            //{
            //    //check if user is ProjectAdmin
            //    canCreate = this.IsProjectAdmin(context);
            //}
            //return canCreate;
            return true;
        }

        public bool IsProjectAdmin(ContextInfo context)
        {
            //int permissionMask = this.Repository.GetProjectPermissions(context.CurrentUserName, context.CurrentProjectId);
            //return (permissionMask & (int)PermissionFlag.ProjectAdmin) == (int)PermissionFlag.ProjectAdmin;
            return true;
        }

        public IEnumerable<File> TryGetFileInfo(string userName, long[] fileIds, PermissionFlag permissions)
        {
            //Check against the persmissions
            return this.Repository.TryGetFileInfo(userName, fileIds);
        }

        public FileVersion TryGetFileVersionInfo(string userName, long fileId, int versionNumber, PermissionFlag permissionFlag)
        {
            return this.Repository.TryGetFileVersionInfo(userName, fileId, versionNumber);
        }

        public FileAttachment TryGetFileAttachmentInfo(string userName, long fileId, int versionNumber, PermissionFlag permissions)
        {
            //ensure here the user has appropairate access to perform the operaion
            return this.Repository.TryGetFileAttachmentInfo(userName, fileId, versionNumber);
           
        }

        public Transmittal TryGetTransmittalInfo(string userName, long customerId, long projectId, long transmittalId)
        {
            ITransmittalRepository transmittalRepository = Factory.GetRepository<ITransmittalRepository>(customerId);
            Transmittal transmittal = transmittalRepository.GetTransmittal(projectId, transmittalId);
            //need to do some security validation here
            return transmittal;
        }

        public PermissionFlag GetFolderPermission(string companyName, long folderId)
        {
            int permissionFlag =  this.Repository.GetFolderPermission(companyName, folderId);
            return (PermissionFlag) permissionFlag;
        }

        public IEnumerable<PermissionInfo> GetProjectPermissions(long projectId)
        {
            List<PermissionInfo> allPermissionInfos =
                new List<PermissionInfo>(this.Repository.GetProjectPermissions(projectId));
            allPermissionInfos.Remove(
               allPermissionInfos.First(
                   x => x.Email.Equals(this.Context.UserName, StringComparison.CurrentCultureIgnoreCase)));
            return allPermissionInfos;
        }

        public void UpdateProjectPermissions(long projectId, IEnumerable<PermissionInfo> changedPermissions)
        {
            this.Repository.UpdateProjectPermissions(projectId, changedPermissions);
        }

        public IEnumerable<PermissionInfo> GetFolderPermissions(long projectId, long folderId)
        {
            return this.Repository.GetCompaniesFolderPermissions(this.Context.UserName, projectId, folderId);
            
        }

        public void UpdateFolderPermissions(long projectId, long folderId, IEnumerable<PermissionInfo> changedPermissions)
        {
                this.Repository.UpdateFolderPermissions(projectId,folderId,changedPermissions);
        }
    }
}