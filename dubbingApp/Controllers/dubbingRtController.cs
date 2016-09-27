using System.Web.Mvc;

namespace dubbingApp.Controllers
{
    [Authorize(Roles = "ADMIN, GENERAL_MANAGER, PRODUCTION_MANAGER, STUDIO_SUPERVISOR")]
    public class dubbingRtController : Controller
    {
        // GET: dubbingRt
        public ActionResult Index()
        {
            if (User.IsInRole("STUDIO_SUPERVISOR") || User.IsInRole("STUDIO_ASSISTANT"))
                return RedirectToRoute("dubbingRoute");
            else if (User.IsInRole("GENERAL_MANAGER") || User.IsInRole("PRODUCTION_MANAGER") || User.IsInRole("ADMIN"))
                return RedirectToRoute("dubbingMonitorRoute");
            else
                return View();
        }
    }
}