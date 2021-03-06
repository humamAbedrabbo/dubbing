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

        public class orderItem
        {
            public string oiMonth { get; set; }
            public int totalReceived { get; set; }
            public int totalDubbed { get; set; }
            public int totalUploaded { get; set; }
            public int totalShipped { get; set; }
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
            foreach (var item in model1.Select(b => new { b.logYear, b.logMonth }).Distinct())
            {
                activeWorksList.Add(item.logMonth + "/" + item.logYear + ";" + x1.Where(b => b.logYear == item.logYear && b.logMonth == item.logMonth).Select(b => b.workIntno).Count());
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

        public JsonResult ordersChartData(int? filterYear, long? clientIntno, long? workIntno)
        {
            int currYear = filterYear.HasValue ? filterYear.Value : DateTime.Today.Year;
            int currMonth = DateTime.Today.Month;
            List<string> monthString = new List<string>() { "Jan", "Feb", "Mar", "Apr", "May", "Jun", "Jul", "Aug", "Sep", "Oct", "Nov", "Dec"};
            List<orderItem> chartData = new List<orderItem>();
            int tr = 0; int td = 0; int tu = 0; int ts = 0;

            var x1 = db.logOrders.Where(b => b.logYear == currYear - 1 && b.logMonth >= currMonth && b.logMonth <= 12
                                        && (!clientIntno.HasValue || b.clientIntno == clientIntno)
                                        && (!workIntno.HasValue || b.workIntno == workIntno)).ToList();
            for (int i = currMonth; i <= 12; i++)
            {
                orderItem oi = new orderItem();
                oi.oiMonth = monthString[i - 1];
                var y = x1.Where(b => b.logMonth == i);
                if (y.Count() != 0)
                {
                    tr += y.Select(b => b.totalEpisodesReceived).Sum();
                    td += y.Select(b => b.totalEpisodesDubbed).Sum();
                    tu += y.Select(b => b.totalEpisodesUploaded).Sum();
                    ts += y.Select(b => b.totalEpisodesShipped).Sum();
                    oi.totalReceived = tr;
                    oi.totalDubbed = td;
                    oi.totalUploaded = tu;
                    oi.totalShipped = ts;
                }
                else
                {
                    oi.totalReceived = tr;
                    oi.totalDubbed = td;
                    oi.totalUploaded = tu;
                    oi.totalShipped = ts;
                }
                chartData.Add(oi);
            }

            var x2 = db.logOrders.Where(b => b.logYear == currYear && b.logMonth >= 1 && b.logMonth <= currMonth
                                        && (!clientIntno.HasValue || b.clientIntno == clientIntno)
                                        && (!workIntno.HasValue || b.workIntno == workIntno)).ToList();
            for (int i = 1; i <= currMonth; i++)
            {
                orderItem oi = new orderItem();
                oi.oiMonth = monthString[i - 1];
                var y = x2.Where(b => b.logMonth == i);
                if (y.Count() != 0)
                {
                    tr += y.Select(b => b.totalEpisodesReceived).Sum();
                    td += y.Select(b => b.totalEpisodesDubbed).Sum();
                    tu += y.Select(b => b.totalEpisodesUploaded).Sum();
                    ts += y.Select(b => b.totalEpisodesShipped).Sum();
                    oi.totalReceived = tr;
                    oi.totalDubbed = td;
                    oi.totalUploaded = tu;
                    oi.totalShipped = ts;
                }
                else
                {
                    oi.totalReceived = tr;
                    oi.totalDubbed = td;
                    oi.totalUploaded = tu;
                    oi.totalShipped = ts;
                }
                chartData.Add(oi);
            }
            return Json(chartData);
        }
    }
}