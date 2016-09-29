using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using dubbingModel;

namespace dubbingApp.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private DUBBDBEntities db = new DUBBDBEntities();
        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }

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
            DateTime dToday = DateTime.Now.Date;
            List<string> model = new List<string>();
            // new works
            var x = db.agreementWorks.Where(b => b.status == "01" && !b.orderTrnHdrs.Select(o => o.workIntno).Contains(b.workIntno)).ToList();
            // active works
            var y = db.agreementWorks.Where(b => b.status == "01" && b.orderTrnHdrs.Select(o => o.workIntno).Contains(b.workIntno)).ToList();
            foreach (var x1 in x)
                model.Add(x1.workName + "|NEW|");
            foreach (var y1 in y)
            {
                string y2 = db.orderTrnHdrs.Where(b => b.workIntno == y1.workIntno && b.status == "04").Count().ToString();
                string y3 = db.clientClaims.Include(b => b.orderTrnHdr).Where(b => b.orderTrnHdr.workIntno == y1.workIntno && b.status == true).Count().ToString();
                model.Add(y1.workName + "|" + y2 + "|" + y3);
            }

            ViewBag.delayedUploads = db.orderTrnHdrs.Where(b => b.status == "04" && b.plannedUpload.HasValue && b.plannedUpload < dToday).Count();
            var ship = db.shipments.Include(b => b.client).FirstOrDefault(b => b.shipmentDate >= dToday);
            if (ship != null)
                ViewBag.nextShipment = ship.shipmentDate.ToString("dd/MM") + " (" + ship.client.clientShortName + ")";
            else
                ViewBag.nextShipment = null;
            return PartialView("_activitiesSummary", model);
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