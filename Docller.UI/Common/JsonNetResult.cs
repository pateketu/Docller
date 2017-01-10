using System;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Docller.Core.Resources;
using Docller.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Docller.Common
{
    public class JsonNetResult:JsonResult
    {
        private readonly JsonSerializerSettings _settings;
        private readonly string _jsPrefix;

        public JsonNetResult():this(new JsonSerializerSettings()
                                                             {
                                                                 ContractResolver = new CamelCasePropertyNamesContractResolver(),
                                                                 NullValueHandling = NullValueHandling.Include
                                                             })
        {
            
        }
        public JsonNetResult(JsonSerializerSettings settings):this(settings,string.Empty)
        {
            
        }

        public JsonNetResult(JsonSerializerSettings settings, string jsPrefix)
        {
            _settings = settings;
            _jsPrefix = jsPrefix;
        }

        public string GetRawJson()
        {
            if (Data != null)
            {
                string json = JsonConvert.SerializeObject(this.Data, Formatting.None,
                                                          this._settings);
                return string.Format(CultureInfo.InvariantCulture, "{0}{1}", this._jsPrefix, json);
            }
            return string.Empty;

        }

        public override void ExecuteResult(ControllerContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException("context");
            }
            if (JsonRequestBehavior == JsonRequestBehavior.DenyGet &&
                String.Equals(context.HttpContext.Request.HttpMethod, "GET", StringComparison.OrdinalIgnoreCase))
            {
                throw new InvalidOperationException(StringResources.JsonRequest_GetNotAllowed);
            }

            HttpResponseBase response = context.HttpContext.Response;

            response.ContentType = !String.IsNullOrEmpty(ContentType) ? ContentType : "application/json";
            if (ContentEncoding != null)
            {
                response.ContentEncoding = ContentEncoding;
            }
            string json = this.GetRawJson();
            response.Write(json);
        }
    }
}