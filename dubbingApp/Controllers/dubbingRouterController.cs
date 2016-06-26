using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.Entity;
using System.Web;
using System.Web.Mvc;
using dubbingModel;
using dubbingApp.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Web.Routing;

namespace dubbingApp.Controllers
{
    [Authorize(Roles = "ADMIN, GENERAL_MANAGER, PRODUCTION_MANAGER, STUDIO_SUPERVISOR")]
    public class dubbingRouterController : Controller
    {
        private UserManager<ApplicationUser> UserManager { get; set; }
        private ApplicationDbContext context = new ApplicationDbContext();

        // GET: dubbingRouter
        public ActionResult Index()
        {
            UserManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(context));
            
            if (User.IsInRole("STUDIO_SUPERVISOR") || User.IsInRole("STUDIO_ASSISTANT") || User.IsInRole("ADMIN"))
                return RedirectToRoutePermanent("dubbing");
            else if (User.IsInRole("GENERAL_MANAGER") || User.IsInRole("PRODUCTION_MANAGER") || User.IsInRole("ADMIN"))
                return RedirectToRoutePermanent("dubbingMonitor");
            else
                return View();
        }
    }
}