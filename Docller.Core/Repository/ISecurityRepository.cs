using System.Collections.Generic;
using System.Linq;
using System.Text;
using Docller.Core.Models;

namespace Docller.Core.Repository
{
    public interface ISecurityRepository: IRepository
    {
        int GetProjectPermissions(string userName, long projectId);
        int GetFolderPermissions(string userName, long projectId, long parentFolder);
        IEnumerable<File> TryGetFileInfo(string userName, long[] fileIds);
        FileVersion TryGetFileVersionInfo(string userName, long fileId, int versionNumber);

        FileAttachment TryGetFileAttachmentInfo(string userName, long fileId, int revisionNumber);
        int GetFolderPermission(string companyName, long folderId);
        IEnumerable<PermissionInfo> GetProjectPermissions(long projectId);
        void UpdateProjectPermissions(long projectId, IEnumerable<PermissionInfo> changedPermissions);
        IEnumerable<PermissionInfo> GetCompaniesFolderPermissions(string userName, long projectId, long folderId);
        void UpdateFolderPermissions(long projectId, long folderId, IEnumerable<PermissionInfo> changedPermissions);
    }
}
