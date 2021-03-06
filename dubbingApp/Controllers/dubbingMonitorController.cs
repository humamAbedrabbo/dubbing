using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.Entity;
using System.Web;
using System.Web.Mvc;
using dubbingModel;

namespace dubbingApp.Controllers
{
    [Authorize(Roles = "ADMIN, GENERAL_MANAGER, PRODUCTION_MANAGER")]
    public class dubbingMonitorController : Controller
    {
        private DUBBDBEntities db = new DUBBDBEntities();
        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }

        public class refreshModel
        {
            public string studioNo { get; set; }
            public string startedTime { get; set; }
            public string lastTime { get; set; }
            public string work { get; set; }
            public string episodeNo { get; set; }
            public string sceneNo { get; set; }
            public string actor { get; set; }
            public string completed { get; set; }
            public int progress { get; set; }
        }

        // GET: dubbingMonitor
        public ActionResult Index()
        {
            DateTime dToday = DateTime.Today.Date;
            var sch = db.dubbingTrnHdrs.FirstOrDefault(b => b.fromDate <= dToday && b.thruDate >= dToday && b.status == true);
            if (sch != null)
                ViewBag.schedule = sch.dubbTrnHdrIntno;
            else
                ViewBag.schedule = 0;
            return View();
        }

        public ActionResult studiosList(long schedule)
        {
            var model = db.studioEpisodes.Include(b => b.studio.employee).Include(b => b.orderTrnDtl.employee).Include(b => b.orderTrnDtl.orderTrnHdr.agreementWork)
                                        .Where(b => b.studio.dubbTrnHdrIntno == schedule).OrderBy(b => b.studio.studioNo);
            return PartialView("_studiosList", model.ToList());
        }

        public ActionResult actorsList(long studioIntno)
        {
            DateTime dToday = DateTime.Today.Date;
            var model = db.dubbingAppointments.Where(b => b.studioIntno == studioIntno && b.appointmentDate == dToday).OrderBy(b => b.fromTime);
            return PartialView("_actorsList", model.ToList());
        }

        public ActionResult currentActivity(long studioIntno)
        {
            ViewBag.studioNo = db.studios.Find(studioIntno).studioNo;
            return PartialView("_currentActivity");
        }

        public ActionResult progressList(long schedule)
        {
            var model = db.studios.Include(b => b.employee).Where(b => b.dubbTrnHdrIntno == schedule).OrderBy(b => b.studioIntno);
            return PartialView("_progressList", model.ToList());
        }

        public ActionResult refreshAll(long? schedule)
        {
            List<refreshModel> model = new List<refreshModel>();
            DateTime dt1 = DateTime.Today.Date;
            DateTime dt2 = dt1.AddDays(1);
            var x = (from A in db.dubbingSheetDtls
                     join B in db.dubbingSheetHdrs on A.dubbSheetHdrIntno equals B.dubbSheetHdrIntno
                     where A.takenTimeStamp >= dt1 && A.takenTimeStamp <= dt2
                     select new { A.studioNo, A.sceneNo, A.takenTimeStamp, B.actorName, A.orderTrnHdrIntno }).ToList();
            var stdList = x.Select(b => b.studioNo).Distinct().ToList();
            foreach(var std in stdList)
            {
                var x1 = x.Where(b => b.studioNo == std).OrderBy(b => b.takenTimeStamp).First();
                var x2 = x.Where(b => b.studioNo == std).OrderBy(b => b.takenTimeStamp).Last();
                var orderItem = db.orderTrnHdrs.Find(x2.orderTrnHdrIntno);
                refreshModel rm = new refreshModel();
                rm.studioNo = std;
                rm.startedTime = x1.takenTimeStamp.Value.ToString("HH:mm");
                rm.lastTime = x2.takenTimeStamp.Value.ToString("HH:mm");
                rm.actor = x2.actorName;
                rm.work = orderItem.agreementWork.workName;
                rm.episodeNo = orderItem.episodeNo.ToString();
                rm.sceneNo = x2.sceneNo.ToString();

                //calculate total schedule completed scenes
                var scn = (from A in db.studios
                           join B in db.studioEpisodes on A.studioIntno equals B.studioIntno
                           join C in db.orderTrnDtls on B.orderTrnDtlIntno equals C.orderTrnDtlIntno
                           join D in db.orderTrnHdrs on C.orderTrnHdrIntno equals D.orderTrnHdrIntno
                           join E in db.dubbingSheetDtls on D.orderTrnHdrIntno equals E.orderTrnHdrIntno
                           where (!schedule.HasValue || A.dubbTrnHdrIntno == schedule.Value) && A.studioNo == std && E.isTaken == true
                           select new { A });
                rm.completed = scn.Count().ToString();

                //calculate progress
                var y = (from A in db.studios
                         join B in db.studioEpisodes on A.studioIntno equals B.studioIntno
                         join C in db.orderTrnDtls on B.orderTrnDtlIntno equals C.orderTrnDtlIntno
                         join D in db.orderTrnHdrs on C.orderTrnHdrIntno equals D.orderTrnHdrIntno
                         join E in db.dubbingSheetDtls on D.orderTrnHdrIntno equals E.orderTrnHdrIntno
                         where A.studioNo == std && A.dubbTrnHdrIntno == schedule
                         select new { E.isTaken });
                rm.progress = y.Where(b => b.isTaken == true).Count() * 100 / y.Count();
                model.Add(rm);
            }
            return Json(model, JsonRequestBehavior.AllowGet);
        }
    }
}