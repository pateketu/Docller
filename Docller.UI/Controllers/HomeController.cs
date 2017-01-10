using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Docller.Controllers
{
    public class HomeController : DocllerControllerBase
    {
        public ActionResult Index()
        {
            return RedirectToAction("LogOn", "Account");
        }

        
    }
}
