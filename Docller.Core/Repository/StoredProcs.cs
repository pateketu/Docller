using System.Data.Common;

namespace Docller.Core.Repository
{
    internal static class StoredProcs
    {
        public const string GetCommentMetaDataInfo = "usp_GetCommentMetaDataInfo";
        public const string UpdateUserCache = "usp_UpdateUserCache";
        public const string GetSubscribers = "usp_GetSubscribers";
        public const string GetFolders = "usp_GetFolders";
        public const string SubscribeCompanies = "usp_SubscribeCompanies";
        public const string EnsureUsers = "usp_EnsureUsers";
        public const string AddAttachment = "usp_AddAttachment";
        public const string AddFile = "usp_AddFile";
        public const string AddFolders = "usp_AddFolders";
        public const string IsDomainUrlExists = "usp_IsDomainUrlExists";
        public const string GetCutomer = "usp_GetCustomer";
        public const string GetUserLogonInfo = "usp_GetUserLogonInfo";
        public const string GetProjectPermissions = "usp_GetProjectPermissions";
        public const string GetFolderPermissions = "usp_GetFolderPermissions";
        public const string UpdateCompanyAndUserForNewCustomer = "usp_UpdateCompanyAndUserForNewCustomer";
        public const string RenameFolder = "usp_RenameFolder";
        public const string UpdateUserFailedLoginAttempt = "usp_UpdateFailedLoginAttempt";
        public const string GetFilePreUploadInfo = "usp_GetFilePreUploadInfo";
        public const string GetFilePrefs = "usp_GetFilePrefs";
        public const string GetProjectDetails = "usp_GetProjectDetails";
        public const string GetFilesForEdit = "usp_GetFilesForEdit";
        public const string UpdateFiles = "usp_UpdateFiles";
        public const string AddProject = "usp_AddProject";
        public const string GetProjectStatuses = "usp_GetProjectStatuses";
        public const string CreateTransmittal = "usp_CreateTransmittals";
        public const string GetTransmittal = "usp_GetTransmittal";
        public const string ShareFiles = "usp_ShareFiles";
        public const string GetFiles = "usp_GetFiles";
        public const string GetFilesInfoForDownload = "usp_GetFilesInfoForDownload";
        public const string GetFileVersionInfo = "usp_GetFileVersionInfo";
        public const string GetUserSubscriptionInfo = "usp_GetUserSubscriptionInfo";
        public const string GetFileHistory = "usp_GetFileHistory";
        public const string GetFileAttachment = "usp_GetAttachment";
        public const string DeleteAttachment = "usp_DeleteAttachment";
        public const string DeleteFile = "usp_DeleteFile";
        public const string GetIssueSheet = "usp_GetIssueSheet";
        public const string SetFilePreviews = "usp_SetFilePreviews";

        public const string GetCompanyFolderPermisssion = "usp_GetCompanyFolderPersmission";
        public const string DownloadSharedFiles = "usp_DownloadSharedFiles";
        public const string GetPermissionsForProject = "usp_GetPermissionsForProject";
        public const string UpdateProjectPermissions = "usp_UpdateProjectPermissions";
        public const string GetPermissionsForFolder = "usp_GetPermissionsForFolder";
        public const string UpdateFolderPermissions = "usp_UpdateFolderPermissions";
        public const string GetMyTransmittals = "usp_GetMyTransmittlas";
        public const string UpdateProjectDetails = "usp_UpdateProjectDetails";
        public const string UpdateCustomerDetails = "usp_UpdateCustomerDetails";
        public const string AddAttachment2 = "usp_AddAttachment2";
    }
}