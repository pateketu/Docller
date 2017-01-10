using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace Docller
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                "Default", // Route name
                "{controller}/{action}/{projectid}", // URL with parameters
                new { controller = "Home", action = "Index", projectid = UrlParameter.Optional } // Parameter defaults

            );

            routes.MapRoute(
                "Folder", // Route name
                "{controller}/{action}/{projectid}/{folderid}", // URL with parameters
                new { controller = "Home", action = "Index", projectid = UrlParameter.Optional, folderid = UrlParameter.Optional } // Parameter defaults

            );
        }
    }
}