using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using dubbingModel;
using dubbingApp.Models;

namespace dubbingApp.Controllers
{
    [Authorize(Roles = "ADMIN, GENERAL_MANAGER")]
    public class executiveSummaryController : Controller
    {
        private DUBBDBEntities db = new DUBBDBEntities();
        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }

        // GET: executiveSummary
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult worksLog()
        {
            DateTime thruDate = DateTime.Today.Date;
            DateTime fromDate = thruDate.AddYears(-1);

            var model = db.logWorks.Where(b => (b.contractedDate <= thruDate && b.contractedDate >= fromDate)
                                    || (b.endorsedDate <= thruDate && b.endorsedDate >= fromDate));
            return PartialView("_worksLog", model.ToList());
        }

        public ActionResult ordersLog()
        {
            int currYear = DateTime.Today.Year;
            int currMonth = DateTime.Today.Month;

            var model = db.logOrders.Where(b => (b.logYear == currYear && b.logMonth <= currMonth)
                                    || (b.logYear == currYear - 1 && b.logMonth >= currMonth))
                                    .OrderByDescending(b => b.logIntno);
            return PartialView("_ordersLog", model.ToList());
        }
    }
}