using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Web;
using log4net;
using log4net.Config;

namespace Docller.Core.Common
{
    public class Logger
    {

        /*
         * <configSections>
    <section name="log4net" type="log4net.Config.Log4NetConfigurationSectionHandler, log4net" />
  </configSections>
         * 
         *  <log4net>
    <appender name="BufferingForwardingAppender" type="log4net.Appender.BufferingForwardingAppender">
      <!--<bufferSize value="99" />-->
      <appender-ref ref="AzureTableAppender" />
    </appender>
    <appender type="Docller.Core.Common.AzureTableAppender, Docller.Core" name="AzureTableAppender"></appender>
    <root>
      <level value="ERROR" />
      <!--<appender-ref ref="AzureTableAppender" />-->
    </root>
    <logger name="DefaultLogger">
      <level value="WARN" />
      <appender-ref ref="AzureTableAppender" />
    </logger>
  </log4net>
         */
        private const string LoggerName = "DefaultLogger";

        static Logger()
        {
            XmlConfigurator.Configure();
        }
        public static void Log(HttpContextBase httpContext,  Exception ex)
        {
            ILog logger = LogManager.GetLogger(LoggerName);
            logger.Fatal(BuildFromHttpContext(httpContext), ex);
        }

        public static void Warn(string messageFormat, params object[] args)
        {
            ILog logger = LogManager.GetLogger(LoggerName);
            logger.WarnFormat(messageFormat,args);
        }
        public static void Warn(string message)
        {
            ILog logger = LogManager.GetLogger(LoggerName);
            logger.Warn(message);
        }
        private static string BuildFromHttpContext(HttpContextBase httpContext)
        {
            StringBuilder builder = new StringBuilder("Errors:");
            if (httpContext != null)
            {
                builder.AppendLine();
                builder.AppendFormat("Request Url: {0}", httpContext.Request.Url);
                builder.AppendLine();
                builder.AppendFormat("Request Referrer: {0}", httpContext.Request.UrlReferrer);
                builder.AppendLine();
                builder.AppendFormat("User: {0}",
                    httpContext.User != null && !string.IsNullOrEmpty(httpContext.User.Identity.Name)
                        ? httpContext.User.Identity.Name
                        : "---No User---");
                builder.AppendLine();
                builder.AppendFormat("Browser: {0}", httpContext.Request.UserAgent);
                builder.AppendLine();
                builder.AppendFormat("IP Address: {0}", httpContext.Request.UserHostAddress);
                builder.AppendLine();
                builder.AppendFormat("UserHost Name: {0}", httpContext.Request.UserHostName);
                
            }
            else
            {
                builder.AppendLine("No HttpContext!");
            }
            return builder.ToString();

        }
    }
}
