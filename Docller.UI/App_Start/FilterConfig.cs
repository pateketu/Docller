using System.Web;
using System.Web.Mvc;
using Docller.UI.Common;

namespace Docller
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new AuthorizeAttribute());
            filters.Add(new ExceptionHandlingAttribute());
        
        }
    }
}