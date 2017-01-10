using System;
using System.IO;

namespace Docller.Core.Common
{
    public class LocalStorage : ILocalStorage
    {
        public string CreateTempFolder()
        {
            Guid guid = Guid.NewGuid();
            DirectoryInfo info = EnsureTempFolder();
            DirectoryInfo subDir = info.CreateSubdirectory(guid.ToString("D"));
            return subDir.FullName;
        }

        public string EnsureCacheFolder(string folderName)
        {
            DirectoryInfo info = new DirectoryInfo(string.Format("{0}\\{1}",LocalStorageFolderPath, folderName));
            if (!info.Exists)
            {
                info.Create();
            }
            return info.FullName;
        }

        protected DirectoryInfo EnsureTempFolder()
        {
            DirectoryInfo info = new DirectoryInfo(string.Format("{0}\\{1}", LocalStorageFolderPath, TempFolder));
            if (!info.Exists)
            {
                info.Create();
            }
            return info;
        }

        protected virtual  string LocalStorageFolderPath
        {
            get { return Config.GetValue<string>(ConfigKeys.LocalStoragePath); }
        }

        protected virtual string TempFolder
        {
            get { return "Temp"; }
        }
    }
}