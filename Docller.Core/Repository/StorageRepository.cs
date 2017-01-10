using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using Docller.Core.Common;
using Docller.Core.DB;
using Docller.Core.Infrastructure;
using Docller.Core.Models;
using Docller.Core.Repository.Accessors;
using Docller.Core.Repository.Collections;
using Docller.Core.Repository.Collections.Mappers.StoredProcMappers;
using Docller.Core.Repository.Mappers;
using Docller.Core.Repository.Mappers.StoredProcMappers;
using Microsoft.Practices.EnterpriseLibrary.Data;

namespace Docller.Core.Repository
{
    public class StorageRepository : BaseRepository, IStorageRepository
    {
        public StorageRepository(FederationType federation, long federationKey) : base(federation, federationKey)
        {
        }

        public int CreateFolders(string userName, long customerId, long projectId, long parentFolder, PermissionFlag flag, string Seperator, List<Folder> folders, out List<Folder> duplicateFolders)
        {
            Database db = this.GetDb();
            GenericParameterMapper parameterMapper = new GenericParameterMapper(db);

            StoredProcAccessor<Folder> accessor = db.CreateStoredProcAccessor(StoredProcs.AddFolders,
                                                                                      parameterMapper,
                                                                                      DefaultMappers.ForDuplicateFolders);
            duplicateFolders = accessor.Execute(userName,
                                            customerId,
                                            projectId,
                                            parentFolder,
                                            flag,
                                            Seperator,
                                            new FolderCollection(folders)).ToList();

            return parameterMapper.ReturnValue != null ? parameterMapper.ReturnValue.Value : 0;

        }

        public int RenameFolder(Folder folder)
        {
            Database db = this.GetDb();
            ModelParameterMapper<Folder> parameterMapper = new ModelParameterMapper<Folder>(db, folder);
            int returnVal = SqlDataRepositoryHelper.ExecuteNonQuery(db, StoredProcs.RenameFolder, folder, parameterMapper);
            return returnVal;
        }

        public Folders GetFolders(string userName, long projectId, long parentFolderId, int maxLevel)
        {
            Database db = this.GetDb();
            FoldersMapper foldersMapper = new FoldersMapper();
            GenericParameterMapper parameterMapper = new GenericParameterMapper(db);
            StoredProcAccessor<Folder> storedProcAccessor = db.CreateStoredProcAccessor(StoredProcs.GetFolders,
                                                                                        parameterMapper,
                                                                                        foldersMapper);
            List<object> parameters = new List<object> {userName, projectId};
            
            if (maxLevel > 0) parameters.Add(maxLevel); else parameters.Add(null);
            if (parentFolderId > 0) parameters.Add(parentFolderId); else parameters.Add(null);

            Folders folders = (Folders)storedProcAccessor.Execute(parameters.ToArray());
            
            return folders;
        }


        public IEnumerable<File> GetUploadVersionInfo(long projectId, long folderId, List<File> files)
        {
            FileCollection fileCollection = new FileCollection();
            FileAttachmentCollection attachmentCollection = new FileAttachmentCollection();

            foreach (File file in files)
            {
                fileCollection.Add(file);
                if(file.Attachments != null)
                {
                    attachmentCollection.AddRange(file.Attachments);
                }
            }
            Database db = this.GetDb();
            GenericParameterMapper parameterMapper = new GenericParameterMapper(db);
            StoredProcAccessor<File> accessor = db.CreateStoredProcAccessor(StoredProcs.GetFilePreUploadInfo, parameterMapper, new FilePreUploadInfoMapper());
            IEnumerable<File> results = accessor.Execute(projectId, folderId, fileCollection, attachmentCollection);
            return results;

        }

        public Company GetFilePreferences(string userName)
        {
            Database db = this.GetDb();
            IRowMapper<Company> rowMapper = DefaultMappers.ForFilePreferences;
            GenericParameterMapper parameterMapper = new GenericParameterMapper(db);
            StoredProcAccessor<Company> accessor = db.CreateStoredProcAccessor(StoredProcs.GetFilePrefs, parameterMapper,
                                                                               rowMapper);
            return accessor.ExecuteSingle(userName);


        }

        public RepositoryStatus AddFile(File file, bool addAsNewVersion, string versionPath, string userName)
        {
            Database db = this.GetDb();
            ModelParameterMapper<File> modelParameterMapper = new ModelParameterMapper<File>(db, file);
            modelParameterMapper.Map(versionPath).ToParameter("@VersionPath").Map(
                addAsNewVersion).ToParameter("@AddAsNewVersion").Map(userName).ToParameter("@UserName").Map(
                    file.Folder.FolderId).ToParameter("@FolderId").Map(file.Project.ProjectId).ToParameter("@ProjectId");

            StoredProcAccessor<File> storedProcAccessor = db.CreateStoredProcAccessor<File>(StoredProcs.AddFile,
                                                                                      modelParameterMapper);
            storedProcAccessor.ExecuteNonQuery(file);

            return modelParameterMapper.ReturnValue != null
                       ? (RepositoryStatus) modelParameterMapper.ReturnValue.Value
                       : RepositoryStatus.Unknown;

        }

        public RepositoryStatus AddAttachment(FileAttachment attachment, bool addAsNewVersion, string versionPath, string userName)
        {
            Database db = this.GetDb();
            ModelParameterMapper<FileAttachment> modelParameterMapper = new ModelParameterMapper<FileAttachment>(db, attachment);
            modelParameterMapper.Map(versionPath).ToParameter("@VersionPath").Map(
                addAsNewVersion).ToParameter("@AddAsNewVersion").Map(userName).ToParameter("@UserName").Map(
                    attachment.Folder.FolderId).ToParameter("@FolderId").Map(attachment.Project.ProjectId).ToParameter("@ProjectId"); 

            StoredProcAccessor<File> storedProcAccessor = db.CreateStoredProcAccessor<File>(StoredProcs.AddAttachment,
                                                                                      modelParameterMapper);
            storedProcAccessor.ExecuteNonQuery(attachment);

            return modelParameterMapper.ReturnValue != null
                       ? (RepositoryStatus) modelParameterMapper.ReturnValue.Value
                       : RepositoryStatus.Unknown;
        }

        public IEnumerable<File> GetFilesForEdit(List<File> filesToEdit)
        {
            Database db = this.GetDb();
            GenericParameterMapper parameterMapper = new GenericParameterMapper(db);
            StoredProcAccessor<File> accessor = db.CreateStoredProcAccessor(StoredProcs.GetFilesForEdit, parameterMapper,
                                                                            DefaultMappers.ForFilesToEdit);
            return accessor.Execute(new FileCollection(filesToEdit));
        }

        public FilesInfo UpdateFiles(List<File> files)
        {
            Database db = this.GetDb();
            GenericParameterMapper parameterMapper = new GenericParameterMapper(db);
            StoredProcAccessor<File> accessor = db.CreateStoredProcAccessor(StoredProcs.UpdateFiles, parameterMapper,
                                                                            DefaultMappers.ForUpdateFiles);
            FilesInfo info = new FilesInfo
                {
                    DuplicateFiles = accessor.Execute(new FileCollection(files)),
                    Status = parameterMapper.ReturnValue != null
                                 ? (RepositoryStatus) parameterMapper.ReturnValue.Value
                                 : RepositoryStatus.Unknown
                };
            return info;
        }

        public Files GetFiles(long projectId, long folderId, string userName, bool restrictToTransmitted, FileSortBy sortBy,
                                       SortDirection sortDirection, int pageNumber, int pageSize)
        {
            Database db = this.GetDb();
            GenericParameterMapper parameterMapper = new GenericParameterMapper(db);
            FilesMapper filesMapper = new FilesMapper();
            StoredProcAccessor<File> accessor = db.CreateStoredProcAccessor(StoredProcs.GetFiles, parameterMapper,
                                                                            filesMapper);
            var f = accessor.Execute(projectId, folderId, userName, restrictToTransmitted,
                                                       Enum.GetName(typeof (FileSortBy), sortBy), sortDirection,
                                                       pageNumber, pageSize);
            Files files = (Files) f;
            files.PageNumber = pageNumber;
            files.SortBy = sortBy;
            files.Direction = sortDirection;
            files.PageSize = pageSize;
            return files;
        }

        public FileHistory GetFileHistory(long fileId)
        {
            Database db = this.GetDb();
            GenericParameterMapper parameterMapper = new GenericParameterMapper(db);
            FileHistoryMapper fileHistoryMapper = new FileHistoryMapper();
            StoredProcAccessor<FileHistory> accessor = db.CreateStoredProcAccessor(StoredProcs.GetFileHistory, parameterMapper,
                                                                            fileHistoryMapper);
            return accessor.Execute(fileId).FirstOrDefault();
        }

        public FileAttachment GetFileAttachment(string userName, long fileId)
        {
            Database db = this.GetDb();
            GenericParameterMapper parameterMapper = new GenericParameterMapper(db);
            AttachmentMapper attachmentMapper = new AttachmentMapper();
            StoredProcAccessor<FileAttachment> accessor = db.CreateStoredProcAccessor(StoredProcs.GetFileAttachment, parameterMapper, attachmentMapper);
            return accessor.ExecuteSingle(userName, fileId);
        }

        public FileAttachment DeleteAttachment(string userName, long fileId, int revisionNumber, DeleteBlob deleteBlob)
        {
            SqlAzureDatabase db = (SqlAzureDatabase)GetDb();
            
            FileAttachment attachment;
            using (DbConnection connection = db.OpenConnection())
            {
               
                using (DbTransaction transaction = connection.BeginTransaction())
                {
                    try
                    {
                        GenericParameterMapper parameterMapper = new GenericParameterMapper(db);
                        AttachmentMapper attachmentMapper = new AttachmentMapper();
                        StoredProcAccessor<FileAttachment> accessor = db.CreateStoredProcAccessor(StoredProcs.DeleteAttachment, parameterMapper,
                                                                                attachmentMapper);
                        attachment = accessor.ExecuteSingle(transaction, userName, fileId, revisionNumber);

                        if (deleteBlob())
                        {
                            transaction.Commit();
                        }
                        else
                        {
                            transaction.Rollback();
                        }
                    }
                    catch
                    {
                        transaction.Rollback();
                        throw;
                    }
                }
            }
            return attachment;
        }

        public bool DeleteFile(string userName, long fileId, int revisionNumber, DeleteBlob deleteBlob)
        {
            SqlAzureDatabase db = (SqlAzureDatabase)GetDb();
            bool deleted = false;
            using (DbConnection connection = db.OpenConnection())
            {
                using (DbTransaction transaction = connection.BeginTransaction())
                {
                    try
                    {
                        db.ExecuteNonQuery(transaction, StoredProcs.DeleteFile, userName, fileId, revisionNumber);
                        if (deleteBlob())
                        {
                            transaction.Commit();
                            deleted = true;
                        }
                        else
                        {
                            transaction.Rollback();
                        }
                    }
                    catch
                    {
                        transaction.Rollback();
                        throw;
                    }
                }
            }
            return deleted;
        }

        public FileHistory DeleteFileVersion(string userName, long fileId, int revisionNumber, DeleteBlob deleteBlob)
        {
            SqlAzureDatabase db = (SqlAzureDatabase)GetDb();
            FileHistory history;
            using (DbConnection connection = db.OpenConnection())
            {

                using (DbTransaction transaction = connection.BeginTransaction())
                {
                    try
                    {
                        GenericParameterMapper parameterMapper = new GenericParameterMapper(db);
                        FileHistoryMapper historyMapper = new FileHistoryMapper();
                        StoredProcAccessor<FileHistory> accessor = db.CreateStoredProcAccessor(StoredProcs.DeleteFile, parameterMapper,
                                                                                historyMapper);
                        history = accessor.ExecuteSingle(transaction, userName, fileId, revisionNumber);

                        if (deleteBlob())
                        {
                            transaction.Commit();
                            
                        }
                        else
                        {
                            transaction.Rollback();
                        }
                    }
                    catch
                    {
                        transaction.Rollback();
                        throw;
                    }
                }
            }
            return history;
        }

        public void SetPreviewsTimestamp(long fileId, DateTime timestamp)
        {
            Database db = this.GetDb();
            if (timestamp == DateTime.MinValue)
            {
                db.ExecuteNonQuery(StoredProcs.SetFilePreviews, fileId, null);
            }
            else
            {
                db.ExecuteNonQuery(StoredProcs.SetFilePreviews, fileId, timestamp);
            }

            
        }

        public MarkUpFile GetMarkUpMetadataInfo(string userName, string markupFileName, long fileId, long folderId,
                                                long projectId)
        {
            Database db = this.GetDb();
            GetMarkUpMetaDataInfoMapper mapper = new GetMarkUpMetaDataInfoMapper();
            StoredProcAccessor<MarkUpFile> accessor = db.CreateStoredProcAccessor(StoredProcs.GetCommentMetaDataInfo,
                                                                                    new GenericParameterMapper(db),
                                                                                    mapper);
            return accessor.ExecuteSingle(userName, markupFileName, fileId, folderId, projectId);
        }

        public RepositoryStatus AddAttachment2(FileAttachment2 attachment, bool addAsNewVersion, string versionPath, string userName)
        {
            Database db = this.GetDb();
            ModelParameterMapper<FileAttachment2> mapper = new ModelParameterMapper<FileAttachment2>(db, attachment);
            mapper.Map(userName)
                  .ToParameter("@UserName")
                  .Map(addAsNewVersion)
                  .ToParameter("@AddAsNewVersion")
                  .Map(attachment.Project.ProjectId)
                  .ToParameter("@ProjectId")
                  .Map(attachment.Folder.FolderId)
                  .ToParameter("@FolderId").Map(versionPath).ToParameter("@VersionPath");

            StoredProcAccessor<FileAttachment2> storedProcAccessor =
                db.CreateStoredProcAccessor<FileAttachment2>(StoredProcs.AddAttachment2,
                                                             mapper);
            storedProcAccessor.ExecuteNonQuery(attachment);

            return mapper.ReturnValue != null
                       ? (RepositoryStatus) mapper.ReturnValue.Value
                       : RepositoryStatus.Unknown;


        }
    }
}