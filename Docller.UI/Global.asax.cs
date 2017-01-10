using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing; 
using Docller.Common;
using Docller.Core.Common;
using StructureMap;

namespace Docller.UI
{
    // Note: For instructions on enabling IIS6 or IIS7 classic mode, 
    // visit http://go.microsoft.com/?LinkId=9394801
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            Registry.RegisterMappings();
            AreaRegistration.RegisterAllAreas();

            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            ModelBinders.RegisterModelBinders();

            ServerPathMapper mapPathMapper = new ServerPathMapper();
            MIMETypes.Current.Refresh(mapPathMapper);
            FileTypeIconsFactory.Current.Refresh(mapPathMapper);
            CloudDeployment.Initialize();
            
        }

        protected void Application_EndRequest()
        {
            ObjectFactory.ReleaseAndDisposeAllHttpScopedObjects();
        } 

    }
}