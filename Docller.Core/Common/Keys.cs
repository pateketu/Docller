namespace Docller.Core.Common
{
    public static class RequestKeys
    {
        public static string ProjectId = "projectId";
        public static string FolderId = "folderId";
        public static string FileInternalName = "fileInternalName";
        public static string ParentFile = "parentFile";
        public static string IsExisting = "isExisting";
        public static string UploadAsNewVersion = "uploadAsNewVersion";
        public static string Chunk = "chunk";
        public static string Chunks = "chunks";
        public static string FileName = "fileName";
        public static string Name = "name";
        public static string FullPath = "fullPath";
        public static string BaseFileName = "baseFileName";
        public static string DocNumber = "docNumber";
        public static string FileSize = "fileSize";
        public static string FileId = "fileId";
        public static string ShowAsPicker = "showAsPicker";
        public static string ShowFileArea = "showFileArea";

    }

    public static class SessionAndCahceKeys
    {
        public static string ProjectNameFormat = "Project_{0}_{1}";
        public static string ProjectStatusesFormat = "Project_{0}_{1}_Status";
        public static string AllFolders = "AllFolders_{0}_{1}";
        public static string AllSubscribers = "Project_{0}_{1}_Subs";
        public static string AllCustomerSubscribers = "Customer_{0}_Subs";

        public static string CustomerInfoFormat = "Customer_{0}";
    }
}