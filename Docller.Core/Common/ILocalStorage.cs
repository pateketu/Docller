using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Docller.Core.Common
{
    public interface ILocalStorage
    {
        string CreateTempFolder();
        string EnsureCacheFolder(string folderName);
        
    }
}
