using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace dubbingApp.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        public ActionResult activitiesSummary()
        {
            return PartialView("_activitiesSummary");
        }

        [Authorize(Roles = "ADMIN, GENERAL_MANAGER")]
        public ActionResult executives()
        {
            //if (User.IsInRole("ADMIN") || User.IsInRole("GENERAL_MANAGER"))
            //{
            //    return RedirectToRoutePermanent("executives");
            //}
            //else
            //    return new HttpStatusCodeResult(500, "Access Denied! User Has NO Privileges. Please Contact Administration.");
            return RedirectToRoutePermanent("executives");
        }
    }
}