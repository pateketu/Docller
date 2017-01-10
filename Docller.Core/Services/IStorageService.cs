using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Docller.Core.Common;
using Docller.Core.Common.DataStructures;
using Docller.Core.Models;
using Docller.Core.Repository.Collections;
using Docller.Core.Storage;
using File = Docller.Core.Models.File;

namespace Docller.Core.Services
{
    public interface IStorageService
    {
        FolderCreationStatus CreateFolders(string userName, long projectId, List<Folder> folders);
        FolderCreationStatus CreateFolders(string userName, long projectId, long parentFolder, List<Folder> folders);
        Folders GetFolders(string userName, long projectId);
        StorageServiceStatus RenameFolder(Folder folder);
        IEnumerable<File> GetPreUploadInfo(long projectId, long folderId, string[] fileNames, bool attachCADFilesToPdfs, bool patternMatchForVersions);
        StorageServiceStatus AddFile(File file, bool addAsNewVersion, string versionPath);
        StorageServiceStatus AddAttachment(FileAttachment attachment, bool addAsNewVersion, string versionPath);
        StorageServiceStatus Upload<T>(T file,Stream data, int chunk, int totalChunks, bool addAsNewVersion) where T : BlobBase, new();
        IEnumerable<File> GetFilesForEdit(List<File> filesToEdit);
        IEnumerable<File> GetFiles(List<Guid> fileInternNames);
        StorageServiceStatus TryUpdateFiles(List<File> filesToUpdate, out IEnumerable<File> filesNotUpdated);
        Files GetFiles(long projectId, long folderId, string userName,
                                FileSortBy sortBy, SortDirection sortDirection, int pageNumber, int pageSize);
        StorageServiceStatus DownloadToStream(Stream target, BlobBase file, IClientConnection clientConnection);
        FileHistory GetFileHistory(long fileId);
        FileAttachment GetFileAttachment(long fileId);
        void DeleteAttachment(FileAttachment fileAttachment);
        FileAttachment DeleteAttachment(FileAttachmentVersion fileAttachmentVersion);
        bool DeleteFiles(long[] fileIds, long projectId);
        FileHistory DeleteFileVersion(long fileId, int revisionNumber);
        StorageServiceStatus UploadAttachment(FileAttachment fileAttachment, Stream data, int chunk, int totalChunks);
        StorageServiceStatus UploadVersion(long fileId, long folderId, long projectId, string fileName, decimal fileSize, Stream data, int chunk, int totalChunks);

        StorageServiceStatus UploadComment(long projectId, long folderId, long fileId, string fileName, decimal fileSize, Stream data, int chunk, int totalChunks, string comments, bool isFromApp, string appDetails);
        MarkUpFile GetMarkUpMetadataInfo(string markupFileName, long fileId,
                                         long folderId,
                                         long projectId);

    }
}
