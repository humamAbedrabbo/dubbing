using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.Entity;
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

        public ActionResult filters()
        {
            ViewBag.clientsList = new SelectList(db.clients, "clientIntno", "clientShortName");
            ViewBag.worksList = new SelectList(db.agreementWorks.Where(b => b.agreementIntno == -1), "workIntno", "workName");
            ViewBag.currYear = DateTime.Today.Year.ToString();
            return PartialView("_filters");
        }

        public ActionResult populateWorksCombo(long? clientIntno)
        {
            var x = db.agreementWorks.Include(b => b.agreement.client).Where(b => !clientIntno.HasValue || b.agreement.clientIntno == clientIntno.Value);
            SelectList aList = new SelectList(x, "workIntno", "workName");
            return Json(aList);
        }
        
        public ActionResult worksLog(int? filterYear, int? filterMonth, long? clientIntno, long? workIntno)
        {
            var x = db.logWorks.Where(b => (!clientIntno.HasValue || b.clientIntno == clientIntno.Value)
                                && (!workIntno.HasValue || b.workIntno == workIntno.Value));
            var x1 = db.logOrders.Where(b => (!clientIntno.HasValue || b.clientIntno == clientIntno)
                                    && (!workIntno.HasValue || b.workIntno == workIntno));
            var model = x;
            var model1 = x1;

            int currYear;
            int currMonth;
            if (filterYear.HasValue)
            {
                currYear = filterYear.Value;
                if (filterMonth.HasValue)
                {
                    currMonth = filterMonth.Value;
                    model = x.Where(b => b.contractedYear == filterYear.Value && b.contractedMonth == filterMonth.Value);
                }
                else
                {
                    currMonth = 1;
                    model = x.Where(b => b.contractedYear == filterYear.Value);
                }
                model1 = x1.Where(b => b.logYear == currYear && b.logMonth == currMonth)
                                    .OrderByDescending(b => b.logIntno);
            }
            else
            {
                DateTime thruDate = DateTime.Today.Date;
                DateTime fromDate = thruDate.AddYears(-1);
                model = x.Where(b => (b.contractedDate <= thruDate && b.contractedDate >= fromDate)
                                        || (b.endorsedDate <= thruDate && b.endorsedDate >= fromDate));

                currYear = DateTime.Today.Year;
                currMonth = DateTime.Today.Month;
                model1 = x1.Where(b => (b.logYear == currYear && b.logMonth <= currMonth)
                                    || (b.logYear == currYear - 1 && b.logMonth >= currMonth))
                                    .OrderByDescending(b => b.logIntno);
            }

            // active works to be extracted from ordersLog
            List<string> activeWorksList = new List<string>();
            foreach(var item in model1.Select(b => new { b.logYear, b.logMonth }).Distinct())
            {
                //string works = null;
                //foreach(var wk in model1.Where(b => b.logYear == item.logYear && b.logMonth == item.logMonth))
                //{
                //    if (string.IsNullOrEmpty(works))
                //        works = wk.workName;
                //    else
                //        works = works + "*" + wk.workName;
                //}
                activeWorksList.Add(item.logMonth + "/" + item.logYear + ";" + model1.Where(b => b.logYear == item.logYear && b.logMonth == item.logMonth).Count());
            }
            ViewBag.activeWorksList = activeWorksList;
            return PartialView("_worksLog", model.ToList());
        }

        public ActionResult ordersLog(int? filterYear, int? filterMonth, long? clientIntno, long? workIntno)
        {
            var x = db.logOrders.Where(b => (!clientIntno.HasValue || b.clientIntno == clientIntno)
                                    && (!workIntno.HasValue || b.workIntno == workIntno));
            var model = x;
            int currYear;
            int currMonth;
            if (filterYear.HasValue)
            {
                currYear = filterYear.Value;
                if (filterMonth.HasValue)
                    currMonth = filterMonth.Value;
                else
                    currMonth = 1;
                model = x.Where(b => b.logYear == currYear && b.logMonth == currMonth)
                                    .OrderByDescending(b => b.logIntno);
            }
            else
            {
                currYear = DateTime.Today.Year;
                currMonth = DateTime.Today.Month;
                model = x.Where(b => (b.logYear == currYear && b.logMonth <= currMonth)
                                    || (b.logYear == currYear - 1 && b.logMonth >= currMonth))
                                    .OrderByDescending(b => b.logIntno);
            }
            
            return PartialView("_ordersLog", model.ToList());
        }
    }
}