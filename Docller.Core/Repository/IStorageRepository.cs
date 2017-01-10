using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Docller.Core.Common;
using Docller.Core.Infrastructure;
using Docller.Core.Models;
using Docller.Core.Repository.Collections;

namespace Docller.Core.Repository
{
    public delegate bool DeleteBlob();

    public interface IStorageRepository : IRepository
    {
        int CreateFolders(string userName, long customerId, long projectId, long parentFolder, PermissionFlag flag, string Seperator, List<Folder> folders, out List<Folder> duplicateFolders);
        int RenameFolder(Folder folder);
        Folders GetFolders(string userName, long projectId, long parentFolderId, int maxLevel);
        IEnumerable<File> GetUploadVersionInfo(long projectId, long folderId, List<File> files);
        Company GetFilePreferences(string userName);
        RepositoryStatus AddFile(File file, bool addAsNewVersion, string versionPath, string userName);
        RepositoryStatus AddAttachment(FileAttachment attachment, bool addAsNewVersion, string versionPath, string userName);
        IEnumerable<File> GetFilesForEdit(List<File> filesToEdit);
        FilesInfo UpdateFiles(List<File> files);

        Files GetFiles(long projectId, long folderId, string userName, bool restrictToTransmitted,
                                FileSortBy sortBy, SortDirection sortDirection, int pageNumber, int pageSize);

        FileHistory GetFileHistory(long fileId);

        FileAttachment GetFileAttachment(string userName, long fileId);
        FileAttachment DeleteAttachment(string userName, long fileId, int revisionNumber, DeleteBlob deleteBlob);
        bool DeleteFile(string userName, long fileId, int revisionNumber, DeleteBlob deleteBlob);
        FileHistory DeleteFileVersion(string userName, long fileId, int revisionNumber, DeleteBlob deleteBlob);
        void SetPreviewsTimestamp(long fileId, DateTime timestamp);

        MarkUpFile GetMarkUpMetadataInfo(string userName, string markupFileName, long fileId,
                                         long folderId, long projectId);

        RepositoryStatus AddAttachment2(FileAttachment2 attachment, bool addAsNewVersion, string versionPath, string userName);
    }

}
