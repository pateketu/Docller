using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using Docller.Core.Common;
using Docller.Core.Models;
using Docller.Tests.Mocks;

namespace Docller.UnitTests.Mocks
{
    public class MockContext:IDocllerContext
    {
        private readonly string _userName;
        private readonly long _customerId;
        private readonly  Dictionary<string, object > _dictionary;

        public MockContext(string userName, long customerId)
        {
            _userName = userName;
            _customerId = customerId;
            _dictionary = new Dictionary<string, object>();
        }


        public void Inject(Controller controller)
        {
            throw new NotImplementedException();
        }

        public void Inject(Controller controller, User user)
        {
            throw new NotImplementedException();
        }

        public void Inject(Controller controller, IEnumerable<Project> availableProjectsForUser)
        {
            throw new NotImplementedException();
        }

        public void InjectFolder(IFolderContext folderContext)
        {
            throw new NotImplementedException();
        }

        

        public UrlHelper UrlContext
        {
            get { throw new NotImplementedException(); }
        }

        public IDocllerSession Session
        {
            get { throw new NotImplementedException(); }
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
            get { return 0; }
        }

        public ICache Cache
        {
            get { return new MockCache(); }
        }

        public IFolderContext FolderContext
        {
            get { return null; }
        }

        public RouteData RouteData { get; private set; }
        public HttpRequestBase Request { get; private set; }
        public ISecurityContext Security { get{return new MockSecurityContext();} }

        public object this[string key]
        {
            get { return this._dictionary.ContainsKey(key) ? this._dictionary[key] : null; }
            set
            {
                if (this._dictionary.ContainsKey(key))
                {
                    this._dictionary[key] = value;
                }
                else
                {
                    this._dictionary.Add(key,value);
                }
            }
        }
    }
}
