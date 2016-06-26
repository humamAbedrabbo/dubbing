using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace dubbingApp.Controllers
{
    [Authorize(Roles = "ADMIN, GENERAL_MANAGER, PRODUCTION_MANAGER")]
    public class dubbingMonitorController : Controller
    {
        // GET: dubbingMonitor
        public ActionResult Index()
        {
            return View();
        }
    }
}