using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Docller.Core.Common
{
    public sealed class MIMETypes
    {
        private static volatile MIMETypes _instance;
        private static readonly object SyncRoot = new Object();
        private readonly Dictionary<string, string> _mimeCache;
        
        private MIMETypes()
        {
            _mimeCache = new Dictionary<string, string>(StringComparer.InvariantCultureIgnoreCase);    
            
        }

        public static MIMETypes Current
        {
            get
            {
                if (_instance == null)
                {
                    lock (SyncRoot)
                    {
                        if (_instance == null)
                            _instance = new MIMETypes();
                    }
                }

                return _instance;
            }
        }

        public string this[string extension]
        {
            get
            {
                if(_mimeCache.ContainsKey(extension))
                {
                    return _mimeCache[extension];
                }
                return Constants.DefaultContentType;
            }
        }

        public void Refresh(IPathMapper pathMapper)
        {
            this._mimeCache.Clear();
            string filePath = pathMapper.MapPath(Constants.MIMETypesTable);
            FileInfo fileInfo = new FileInfo(filePath);
            if(fileInfo.Exists)
            {
               using(StreamReader reader = new StreamReader(filePath))
               {
                   while (reader.Peek() > -1)
                   {
                       string tuple = reader.ReadLine();
                       if (!string.IsNullOrEmpty(tuple))
                       {
                           string[] data = tuple.Split(" ".ToCharArray(),
                                                                    StringSplitOptions.RemoveEmptyEntries);

                           if(data.Length == 2 && !_mimeCache.ContainsKey(data[0]))
                           {
                               _mimeCache.Add(data[0],data[1]);
                           }
                       }

                   }    
               }
            }

        }
    }
}
