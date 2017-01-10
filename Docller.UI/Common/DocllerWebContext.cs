using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using Docller.Controllers;
using Docller.Core.Common;
using Docller.Core.Models;

namespace Docller.Common
{
    public class DocllerWebContext : IDocllerContext
    {
        private string _userName;
        private long _customerId;
        private long _projectId;
        private IDocllerSession _session;
        private ICache _cache;
        private IFolderContext _folderContext;
        private UrlHelper _urlHelper;
        public void InjectFolder(IFolderContext folderContext)
        {
            _folderContext = folderContext;
        }

        public UrlHelper UrlContext
        {
            get { return _urlHelper; }
        }

        public void Inject(Controller controller)
        {
            Inject(controller,null);
        }

        public void Inject(Controller controller, User user)
        {
            DocllerControllerBase docllerController = (DocllerControllerBase)controller;
            _userName = docllerController.HttpContext.User.Identity.Name;
            _customerId = docllerController.CurrentCookieData.CustomerId;

            long.TryParse(docllerController.RouteData.Values[RequestKeys.ProjectId] as string, out _projectId);
            _session = new DocllerWebSession(docllerController.Session);
            _cache = DocllerEnvironment.UseHttpCache
                         ? (ICache)new DocllerWebCache(docllerController.HttpContext.Cache)
                         : new AzureCache();
            _urlHelper = new UrlHelper(controller.Request.RequestContext);
            this.RouteData = controller.RouteData;
            this.Request = controller.Request;
            if (!string.IsNullOrEmpty(_userName))
            {
                this.Security = Factory.GetInstance<ISecurityContext>();
                this.Security.Refresh(_customerId, _userName, _projectId, user);
            }
        }

        public IDocllerSession Session
        {
            get { return _session; }
        }

        public string UserName
        {
            get { return _userName; }
        }

        public long CustomerId
        {
            get { return _customerId; }
        }

        public long ProjectId
        {
            get { return _projectId; }
        }

        public ICache Cache
        {
            get { return this._cache; }
        }

        public IFolderContext FolderContext
        {
            get { return _folderContext; }
        }

        public RouteData RouteData { get; private set; }
        public HttpRequestBase Request { get; private set; }
        public ISecurityContext Security { get; private set; }
    }
}
