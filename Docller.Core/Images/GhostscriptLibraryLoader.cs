using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Docller.Core.Common;

namespace Docller.Core.Images
{
    public static class NativeMethods
    {
        [DllImport("kernel32.dll")]
        public static extern IntPtr LoadLibrary(string dllToLoad);

        [DllImport("kernel32.dll")]
        public static extern bool FreeLibrary(IntPtr hModule);
    }

    public interface IGhostscriptLibraryLoader
    {
        void LoadLibrary();
        void UnLoadLibrary();

    }
    public  class GhostscriptLibraryLoader:IGhostscriptLibraryLoader
    {
        private bool _isLoaded;
        public GhostscriptLibraryLoader()
        {
            Load();   
        }
        public void LoadLibrary()
        {
            //do nothing dummy mnethod as constructor will load it
        }

        public void UnLoadLibrary()
        {
            //for future
        }

        private void Load()
        {
            if (!_isLoaded)
            {
                _isLoaded = true;
                IPathMapper pathMapper = Factory.GetInstance<IPathMapper>();
                string gsDll = IntPtr.Size == 4 ? "gsdll32" : "gsdll64";
                string path = pathMapper.MapPath(string.Format("~/bin/Ghostscript/{0}",gsDll));
                NativeMethods.LoadLibrary(path);
            }
        }
    }
}
