using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Docller.Core.Common
{

    public class FileTypeIconsFactory
    {
        private static volatile FileTypeIconsFactory _instance;
        private static readonly object SyncRoot = new Object();
        private readonly SmallFileTypeIcons _smallFileTypeIcons;
        private readonly MeidumFileTypeIcons _meidumFileTypeIcons;
        
        private FileTypeIconsFactory()
        {
            _smallFileTypeIcons = new SmallFileTypeIcons();
            _meidumFileTypeIcons = new MeidumFileTypeIcons();
        }

        public static FileTypeIconsFactory Current
        {
            get
            {
                if (_instance == null)
                {
                    lock (SyncRoot)
                    {
                        if (_instance == null)
                            _instance = new FileTypeIconsFactory();
                    }
                }

                return _instance;
            }
        }

        public FileTypeIcons Small
        {
            get { return _smallFileTypeIcons; }
        }

        public FileTypeIcons Medium
        {
            get { return _meidumFileTypeIcons; }
        }

        public void Refresh(IPathMapper pathMapper)
        {
            _smallFileTypeIcons.Refresh(pathMapper);
            _meidumFileTypeIcons.Refresh(pathMapper);
        }
    }

    public abstract class FileTypeIcons
    {
        private readonly Dictionary<string, string> _iconCache;

        protected FileTypeIcons()
        {
            _iconCache = new Dictionary<string, string>(StringComparer.InvariantCultureIgnoreCase);   
        }

        protected Dictionary<string, string> IconCache
        {
            get { return _iconCache; }
        }
        public string this[string extension]
        {
            get
            {
                extension = extension.StartsWith(".") ? extension.Remove(0, 1) : extension;
                if (_iconCache.ContainsKey(extension))
                {
                    return _iconCache[extension];
                }
                return Constants.DefaultFileTypeIcon;
            }
        }

        public void Refresh(IPathMapper pathMapper)
        {
            _iconCache.Clear();
            string folderPath = GetFolderPath(pathMapper);

            string[] files = Directory.GetFiles(folderPath);

            foreach (string file in files)
            {
                FileInfo fileInfo = new FileInfo(file);
                if (!_iconCache.ContainsKey(fileInfo.Extension))
                {
                    int i = fileInfo.Name.LastIndexOf('.');
                    _iconCache.Add(fileInfo.Name.Remove(i),fileInfo.Name);
                }
            }
            InsertDuplicateIcons();
        }

        protected abstract string GetFolderPath(IPathMapper pathMapper);
        protected virtual void InsertDuplicateIcons()
        {
            _iconCache.Add("docx", _iconCache["doc"]);
            _iconCache.Add("pptx", _iconCache["ppt"]);
        }

    }

    public class MeidumFileTypeIcons : FileTypeIcons
    {
        protected override string GetFolderPath(IPathMapper pathMapper)
        {
            return pathMapper.MapPath(Constants.MeidumFileTypeIconsFolder);
        }
    }

    public class SmallFileTypeIcons : FileTypeIcons
    {
        protected override string GetFolderPath(IPathMapper pathMapper)
        {
            return pathMapper.MapPath(Constants.SmallFileTypeIconsFolder);
        }

    }
}
