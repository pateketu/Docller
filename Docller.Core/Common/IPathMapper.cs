using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Docller.Core.Common
{
    public interface IPathMapper
    {
        string MapPath(string relativePath);
    }
}
