using System.Web.Mvc;

namespace dubbingApp.Controllers
{
    [Authorize(Roles = "ADMIN, GENERAL_MANAGER, PRODUCTION_MANAGER, STUDIO_SUPERVISOR")]
    public class dubbingRouterController : Controller
    {
        // GET: dubbingRouter
        public ActionResult Index()
        {
            if (User.IsInRole("STUDIO_SUPERVISOR") || User.IsInRole("STUDIO_ASSISTANT") || User.IsInRole("ADMIN"))
                return RedirectToRoutePermanent("dubbing");
            else if (User.IsInRole("GENERAL_MANAGER") || User.IsInRole("PRODUCTION_MANAGER") || User.IsInRole("ADMIN"))
                return RedirectToRoutePermanent("dubbingMonitor");
            else
                return View();
        }
    }
}