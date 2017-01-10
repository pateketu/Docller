using System;
using System.IO;
using Docller.Core.Common;

namespace Docller.UI.PdfViews
{
    public interface IPDfStreamProvider: IDisposable
    {
        Stream CreateStream(ILocalStorage localStorage);
        Stream OpenStream();
    }
}