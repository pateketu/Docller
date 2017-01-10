

using System.Collections.Generic;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using Docller.Core.Models;

namespace Docller.Core.Common
{
    public interface IDocllerContext
    {
        void Inject(Controller controller);
        void Inject(Controller controller, User user);
        void InjectFolder(IFolderContext folderContext);
        UrlHelper UrlContext { get; }
        IDocllerSession Session { get; }
        string UserName { get; }
        long CustomerId { get; }
        long ProjectId { get; }
        ICache Cache { get; }
        IFolderContext FolderContext { get; }
        RouteData RouteData { get; }
        HttpRequestBase Request { get; }
        ISecurityContext Security { get; }
    }
}
