using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Routing;
using Docller.Common;
using Docller.Core.Common;
using Docller.Core.Models;
using Newtonsoft.Json.Serialization;
using StructureMap;

namespace Docller.Serialization
{
    public class TreeContractResolver : CamelCasePropertyNamesContractResolver
    {
        protected override JsonObjectContract CreateObjectContract(Type objectType)
        {
           JsonObjectContract contract = base.CreateObjectContract(objectType);
            contract.Properties.Add(new JsonProperty()
                                        {
                                            Readable = true,
                                            ShouldSerialize = value => true,
                                            PropertyName = "addClass",
                                            PropertyType = typeof(string),
                                            Converter = ResolveContractConverter(typeof(string)),
                                            ValueProvider = new StaticValueProvider("dc-folder")
                                        });

            contract.Properties.Add(new JsonProperty()
                                        {
                                            Readable = true,
                                            ShouldSerialize = value => true,
                                            PropertyName = "href",
                                            PropertyType = typeof (string),
                                            Converter = ResolveContractConverter(typeof (string)),
                                            ValueProvider = new FolderHrefValueProvider()
                                        });

            contract.Properties.Add(new JsonProperty()
                                        {
                                            Readable = true,
                                            ShouldSerialize = value => true,
                                            PropertyName = "isFolder",
                                            PropertyType = typeof (bool),
                                            Converter = ResolveContractConverter(typeof (bool)),
                                            ValueProvider = new StaticValueProvider(true)
                                        });

            contract.Properties.Add(new JsonProperty()
                                        {
                                            Readable = true,
                                            ShouldSerialize = value => true,
                                            PropertyName = "id",
                                            PropertyType = typeof (string),
                                            Converter = ResolveContractConverter(typeof (bool)),
                                            ValueProvider = new FolderIdProvider()
                                        });

            contract.Properties.Add(new JsonProperty()
                                        {
                                            Readable = true,
                                            ShouldSerialize = value => true,
                                            PropertyName = "expand",
                                            PropertyType = typeof(bool),
                                            Converter = ResolveContractConverter(typeof(bool)),
                                            ValueProvider = new ExpandValueProvider()
                                        });
            contract.Properties.Add(new JsonProperty()
                                        {
                                            Readable = true,
                                            ShouldSerialize = value => true,
                                            PropertyName = "focus",
                                            PropertyType = typeof(bool),
                                            Converter = ResolveContractConverter(typeof(bool)),
                                            ValueProvider = new FocusValueProvider()
                                        });
           return contract;
        }
    }

    #region Value Providers

    public abstract class ContextValueProvider : IValueProvider
    {
        protected IFolderContext FolderContext
        {
            get
            {
                return Factory.GetInstance<IDocllerContext>().FolderContext;
            }
        }

        protected  IDocllerContext DocllerContext
        {
            get
            {
                return Factory.GetInstance<IDocllerContext>();
            }
        }

        #region Implementation of IValueProvider

        public abstract void SetValue(object target, object value);
        public abstract object GetValue(object target);

        #endregion

    }
    public class FocusValueProvider : ContextValueProvider
    {
        
        #region Implementation of IValueProvider

        public override void SetValue(object target, object value)
        {
            throw new NotImplementedException();
        }

        public override object GetValue(object target)
        {
            return ((Folder)target).FolderId == this.FolderContext.CurrentFolderId;
        }

        #endregion
    }

    public class ExpandValueProvider : ContextValueProvider
    {
        #region Implementation of IValueProvider

        public override void SetValue(object target, object value)
        {
            throw new NotImplementedException();
        }

        public override object GetValue(object target)
        {
            Folder folder = (Folder)target;
            return this.FolderContext.CurrentFolderId > 0 &&
                   (this.FolderContext.CurrentFolderId == folder.FolderId
                    || this.FolderContext.AllFolders.IsAncestor(this.FolderContext.CurrentFolderId, folder.FolderId));
        }

        #endregion
    }
    public class FolderHrefValueProvider : ContextValueProvider
    {
        #region Implementation of IValueProvider

        public override void SetValue(object target, object value)
        {
            
        }

        public override object GetValue(object target)
        {
            Folder folder = (Folder) target;
            RouteValueDictionary dictionary = new RouteValueDictionary() { { RequestKeys.FolderId, folder.FolderId } };

            if (this.DocllerContext.Request.QueryString[RequestKeys.ShowAsPicker] != null)
            {
                dictionary.Add(RequestKeys.ShowAsPicker, true);
                dictionary.Add(RequestKeys.ShowFileArea, true);
            }

            string url =
                this.DocllerContext.UrlContext.RouteUrl(dictionary);
            return string.Format("{0}",url);
        }

        #endregion
    }

    public class FolderIdProvider:IValueProvider
    {
        #region Implementation of IValueProvider

        public void SetValue(object target, object value)
        {
            
        }

        public object GetValue(object target)
        {
            return ((Folder) target).FolderId.ToString();
        }

        #endregion
    }

    public class StaticValueProvider:IValueProvider 
    {
        private readonly object _value;

        public StaticValueProvider(object value)
        {
            _value = value;
        }

        #region Implementation of IValueProvider

        public void SetValue(object target, object value)
        {
            
        }

        public object GetValue(object target)
        {
            return this._value;
        }

        #endregion
    }

#endregion
}