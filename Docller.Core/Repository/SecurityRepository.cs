using System;
using System.Collections.Generic;
using System.Linq;
using Docller.Core.DB;
using Docller.Core.Models;
using Docller.Core.Repository.Accessors;
using Docller.Core.Repository.Collections;
using Docller.Core.Repository.Mappers;
using Docller.Core.Repository.Mappers.StoredProcMappers;
using Microsoft.Practices.EnterpriseLibrary.Data;

namespace Docller.Core.Repository
{
    public class SecurityRepository : BaseRepository, ISecurityRepository
    {
        public SecurityRepository(FederationType federation, long federationKey) : base(federation, federationKey)
        {
        }

        public int GetProjectPermissions(string userName, long projectId)
        {
            object results = this.GetDb().ExecuteScalar(StoredProcs.GetProjectPermissions, userName, projectId);
            return Convert.ToInt32(results);
        }

        public int GetFolderPermissions(string userName, long projectId, long parentFolder)
        {
            object results = this.GetDb().ExecuteScalar(StoredProcs.GetFolderPermissions, userName, parentFolder);
            return Convert.ToInt32(results);
        }

        public IEnumerable<File> TryGetFileInfo(string userName, long[] fileIds)
        {
            Database db = this.GetDb();
            GenericParameterMapper mapper = new GenericParameterMapper(db);
            FilesForDownloadMapper resultMapper = new FilesForDownloadMapper(DefaultMappers.ForFileDownload);
            StoredProcAccessor<File> accessor = db.CreateStoredProcAccessor<File>(StoredProcs.GetFilesInfoForDownload, mapper, resultMapper);
            FileCollection fileCollection = new FileCollection();
            fileCollection.AddRange(fileIds.Select(fileId => new File() {FileId = fileId}));
           return accessor.Execute(userName, fileCollection);
        }

        public FileVersion TryGetFileVersionInfo(string userName, long fileId, int versionNumber)
        {
            Database db = this.GetDb();
            GenericParameterMapper mapper = new GenericParameterMapper(db);
            FileVersionDownloadMapper resultMapper = new FileVersionDownloadMapper();
            StoredProcAccessor<FileVersion> accessor = db.CreateStoredProcAccessor<FileVersion>(StoredProcs.GetFileVersionInfo, mapper, resultMapper);
            return accessor.ExecuteSingle(fileId, versionNumber, userName);

        }

        public FileAttachment TryGetFileAttachmentInfo(string userName, long fileId, int revisionNumber)
        {
           IStorageRepository repository = new StorageRepository(this.Federation,this.FederationKey);
            FileAttachment attachment = repository.GetFileAttachment(userName, fileId);
            FileAttachment a = revisionNumber == 0
                                                    ? attachment
                                                    : attachment.Versions.Single(x => x.RevisionNumber == revisionNumber);
            return a;
        }

        public int GetFolderPermission(string companyName, long folderId)
        {
            Database db = this.GetDb();
            object val = db.ExecuteScalar(StoredProcs.GetCompanyFolderPermisssion, companyName, folderId);
            if (val != null)
            {
                return Convert.ToInt32(val);
            }
            return 0;
        }

        public IEnumerable<PermissionInfo> GetProjectPermissions(long projectId)
        {
            Database db = this.GetDb();
            GenericParameterMapper parameterMapper = new GenericParameterMapper(db);
            StoredProcAccessor<PermissionInfo> accessor =
                db.CreateStoredProcAccessor(StoredProcs.GetPermissionsForProject, parameterMapper,
                    DefaultMappers.ForProjectPermissions);
            return accessor.Execute(projectId);
        }

        public void UpdateProjectPermissions(long projectId, IEnumerable<PermissionInfo> changedPermissions)
        {
            Database db = this.GetDb();
            GenericParameterMapper parameterMapper = new GenericParameterMapper(db);
            IEnumerable<User> users = changedPermissions.Select(userPermissionInfo => new User()
            {
                UserId = (int) userPermissionInfo.EntityId,
                CustomerPermissions = userPermissionInfo.Permissions
            });
            UserCollection usersCollection = new UserCollection(users);
            StoredProcAccessor<PermissionInfo> accessor = db.CreateStoredProcAccessor<PermissionInfo>(StoredProcs.UpdateProjectPermissions, parameterMapper);
            accessor.ExecuteNonQuery(projectId, usersCollection);

        }

        public IEnumerable<PermissionInfo> GetCompaniesFolderPermissions(string userName, long projectId, long folderId)
        {
            Database db = this.GetDb();
            GenericParameterMapper parameterMapper = new GenericParameterMapper(db);
            StoredProcAccessor<PermissionInfo> accessor =
                db.CreateStoredProcAccessor(StoredProcs.GetPermissionsForFolder, parameterMapper,
                    DefaultMappers.ForFolderPermissions);
            return accessor.Execute(userName, projectId, folderId);
        }

        public void UpdateFolderPermissions(long projectId, long folderId, IEnumerable<PermissionInfo> changedPermissions)
        {
            Database db = this.GetDb();
            GenericParameterMapper parameterMapper = new GenericParameterMapper(db);
            IEnumerable<Company> companies = changedPermissions.Select(userPermissionInfo => new Company()
            {
                CompanyId = userPermissionInfo.EntityId,
                Permission = userPermissionInfo.Permissions
            });
            CompanyCollection companyCollection = new CompanyCollection(companies);
            StoredProcAccessor<PermissionInfo> accessor = db.CreateStoredProcAccessor<PermissionInfo>(StoredProcs.UpdateFolderPermissions, parameterMapper);
            accessor.ExecuteNonQuery(projectId,folderId, companyCollection);
        }
    }
}