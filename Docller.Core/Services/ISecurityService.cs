using System.Collections.Generic;
using System.Linq;
using System.Text;
using Docller.Core.Common;
using Docller.Core.Models;

namespace Docller.Core.Services
{

    public interface ISecurityService
    {
        bool CanCreateProject(ContextInfo context);
        bool CanCreateFolder(ContextInfo context, long parentFolderId);
        bool IsProjectAdmin(ContextInfo context);
        IEnumerable<File> TryGetFileInfo(string userName, long[] fileIds, PermissionFlag permissions);
        FileVersion TryGetFileVersionInfo(string userName, long fileId, int versionNumber, PermissionFlag permissionFlag);

        FileAttachment TryGetFileAttachmentInfo(string userName, long fileId, int versionNumber,
                                                PermissionFlag permissions);

        Transmittal TryGetTransmittalInfo(string userName, long customerId, long projectId, long transmittalId);
        PermissionFlag GetFolderPermission(string companyName, long folderId);
        IEnumerable<PermissionInfo> GetProjectPermissions(long projectId);
        void UpdateProjectPermissions(long projectId, IEnumerable<PermissionInfo> changedPermissions);
        IEnumerable<PermissionInfo> GetFolderPermissions(long projectId, long folderId);
        void UpdateFolderPermissions(long projectId, long folderId, IEnumerable<PermissionInfo> changedPermissions);
    }
}
