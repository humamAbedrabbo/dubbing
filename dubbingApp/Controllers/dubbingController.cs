using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.Entity;
using System.Web;
using System.Web.Mvc;
using dubbingModel;
using dubbingApp.Models;
using Microsoft.AspNet.Identity;

namespace dubbingApp.Controllers
{
    [Authorize(Roles = "ADMIN, STUDIO_SUPERVISOR, STUDIO_ASSISTANT")]
    public class dubbingController : Controller
    {
        private DUBBDBEntities db = new DUBBDBEntities();
        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }

        public class dubbingEntryPoint
        {
            public long empIntno { get; set; }
            public long workIntno { get; set; }
            public string workName { get; set; }
            public DateTime dueDate { get; set; }
            public DateTime fromDate { get; set; }
            public DateTime thruDate { get; set; }
            public long? schedule { get; set; }
            public long? studioIntno { get; set; }
        }

        public class studioUpdateResponse
        {
            public short? episodeNo { get; set; }
            public short? sceneNo { get; set; }
            public string startTimeCode { get; set; }
            public long? sceneIntno { get; set; }
        }

        public dubbingEntryPoint sessionEntry
        {
            get
            {
                return (dubbingEntryPoint)Session["sessionEntry"];
            }
            set
            {
                Session["sessionEntry"] = value;
            }
        }

        // GET: dubbing
        public ActionResult Index()
        {
            DateTime dToday = DateTime.Today.Date;
            string loginUserName = User.Identity.GetUserName();
            List<ViewModels.assignmentViewModel> model = new List<ViewModels.assignmentViewModel>();

            //find dubbing week attributes
            string fdw = LookupModels.decodeDictionaryItem("settings", "fdw");
            sessionEntry = new dubbingEntryPoint();
            DateTime tmp = dToday;
            while (tmp.DayOfWeek.ToString() != fdw)
                tmp = tmp.AddDays(-1);
            sessionEntry.fromDate = tmp;
            sessionEntry.thruDate = tmp.AddDays(6);
            sessionEntry.empIntno = db.employees.FirstOrDefault(b => b.email == loginUserName).empIntno;
            
            //get all assignments for the login supervisor
            var assignments = db.orderTrnDtls.Include(b => b.orderTrnHdr).Include(b => b.employee).Include(b => b.orderTrnHdr.agreementWork)
                                    .Where(b => b.employee.empIntno == sessionEntry.empIntno
                                    && b.orderTrnHdr.endAdaptation.HasValue
                                    && !b.orderTrnHdr.endDubbing.HasValue
                                    && b.orderTrnHdr.agreementWork.status == "01"
                                    && b.status == true)
                                    .OrderBy(b => new { b.forDueDate, b.orderTrnHdr.episodeNo }).ToList();
            foreach (var x in assignments)
            {
                ViewModels.assignmentViewModel item = new ViewModels.assignmentViewModel();
                item.orderTrnDtlIntno = x.orderTrnDtlIntno;
                item.empIntno = x.empIntno;
                item.empName = x.employee.fullName;
                item.workIntno = x.orderTrnHdr.workIntno;
                item.workName = x.orderTrnHdr.agreementWork.workName;
                item.episodeNo = x.orderTrnHdr.episodeNo;
                item.dueDate = x.forDueDate.Value.ToString("dd/MM");

                var std = db.studioEpisodes.FirstOrDefault(b => b.orderTrnDtlIntno == x.orderTrnDtlIntno);
                if (std != null)
                {
                    item.dubbTrnHdrIntno = std.studio.dubbTrnHdrIntno;
                    item.schedule = std.studio.dubbingTrnHdr.fromDate.ToString("dd/MM") + " - " + std.studio.dubbingTrnHdr.thruDate.ToString("dd/MM");
                    item.studioIntno = std.studioIntno;
                    item.studioEpisodeIntno = std.studioEpisodeIntno;
                    item.studioNo = std.studio.studioNo;
                    item.status = "SCHEDULED";
                }
                else
                {
                    if (x.forDueDate <= dToday)
                        item.status = "DUE";
                    else
                        item.status = "ASSIGNED";
                }
                model.Add(item);
            }
            return View(model);
        }

        public ActionResult studio(long id, long? std)
        {
            if (std.HasValue)
            {
                var x = db.studioEpisodes.Include(b => b.studio.dubbingTrnHdr)
                            .FirstOrDefault(b => b.studioEpisodeIntno == std);
                sessionEntry.schedule = x.studio.dubbTrnHdrIntno;
                sessionEntry.studioIntno = x.studioIntno;
                ViewBag.studioNo = "STUDIO " + x.studio.studioNo;
                ViewBag.schedule = x.studio.dubbingTrnHdr.fromDate.ToString("dd/MM") + " - " + x.studio.dubbingTrnHdr.thruDate.ToString("dd/MM");
            }
            else
            {
                ViewBag.studioNo = "STUDIO ??";
                ViewBag.schedule = sessionEntry.fromDate.ToString("dd/MM") + " - " + sessionEntry.thruDate.ToString("dd/MM");
            }

            var oiDtl = db.orderTrnDtls.Include(b => b.orderTrnHdr.agreementWork).Include(b => b.employee)
                        .FirstOrDefault(b => b.orderTrnDtlIntno == id);
            sessionEntry.workIntno = oiDtl.orderTrnHdr.workIntno;
            sessionEntry.workName = oiDtl.orderTrnHdr.agreementWork.workName;
            sessionEntry.dueDate = oiDtl.forDueDate.Value;
            
            ViewBag.team = oiDtl.employee.fullName;
            ViewBag.work = oiDtl.orderTrnHdr.workIntno;
            ViewBag.workName = oiDtl.orderTrnHdr.agreementWork.workName;
            ViewBag.dueDate = oiDtl.forDueDate;
            return View();
        }

        public ActionResult sceneHeader()
        {
            ViewBag.workName = sessionEntry.workName;
            return PartialView("_sceneHeader");
        }

        public ActionResult actorsList()
        {
            var model = (from A in db.orderTrnDtls
                         join B in db.orderTrnHdrs on A.orderTrnHdrIntno equals B.orderTrnHdrIntno
                         join C in db.dubbingSheetHdrs on B.orderTrnHdrIntno equals C.orderTrnHdrIntno
                         where A.empIntno == sessionEntry.empIntno && B.workIntno == sessionEntry.workIntno && A.forDueDate == sessionEntry.dueDate
                         select C).Distinct();
            return PartialView("_actorsList", model.ToList());
        }

        public ActionResult actorCalendar(long actorIntno, string actorName)
        {
            ViewBag.actorName = actorName;
            if (sessionEntry.studioIntno.HasValue)
            {
                var model = db.dubbingAppointments.Where(b => b.voiceActorIntno == actorIntno && b.actorName == actorName
                                                    && b.studioIntno == sessionEntry.studioIntno.Value)
                                                    .OrderBy(b => new { b.appointmentDate, b.fromTime });
                return PartialView("_actorCalendar", model.ToList());
            }
            else
                return PartialView("_actorCalendar");
        }

        public ActionResult studioCalendar ()
        {
            List<string> model = new List<string>();
            if (sessionEntry.studioIntno.HasValue)
            {
                var x = db.dubbingAppointments.Where(b => !sessionEntry.studioIntno.HasValue || b.studioIntno == sessionEntry.studioIntno.Value)
                                            .Select(b => new { b.actorName, b.appointmentDate }).Distinct()
                                            .OrderBy(b => b.appointmentDate).ToList();
                foreach (var x1 in x)
                    model.Add(x1.actorName + "|" + x1.appointmentDate.ToString("dd/MM"));
            }
            return PartialView("_studioCalendar", model);
        }

        public ActionResult selectEpisodeFirstScene(long sheetHdr)
        {
            var y = db.dubbingSheetDtls.Where(b => b.dubbSheetHdrIntno == sheetHdr && !b.isTaken).OrderBy(b => b.sceneNo).FirstOrDefault();
            var x = (from A in db.subtitles
                     join B in db.dialogs on A.dialogIntno equals B.dialogIntno
                     join C in db.scenes on B.sceneIntno equals C.sceneIntno
                     where A.dubbSheetHdrIntno == sheetHdr && C.sceneNo == y.sceneNo
                     select new { C.sceneIntno, C.sceneNo, C.startTimeCode }).OrderBy(b => b.sceneNo).FirstOrDefault();
            studioUpdateResponse result = new studioUpdateResponse();
            if (x != null)
            {
                result.sceneNo = x.sceneNo;
                result.sceneIntno = x.sceneIntno;
                result.startTimeCode = x.startTimeCode;
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public ActionResult scenesList(long actor, string actorName)
        {
            List<ViewModels.dubbingSceneViewModel> scnList = new List<ViewModels.dubbingSceneViewModel>();
            var model = (from C in db.orderTrnHdrs
                         join D in db.dubbingSheetHdrs on C.orderTrnHdrIntno equals D.orderTrnHdrIntno
                         join F in db.dubbingSheetDtls on D.dubbSheetHdrIntno equals F.dubbSheetHdrIntno
                         join G in db.orderTrnDtls on C.orderTrnHdrIntno equals G.orderTrnHdrIntno
                         join H in db.studioEpisodes on G.orderTrnDtlIntno equals H.orderTrnDtlIntno
                         join J in db.studios on H.studioIntno equals J.studioIntno
                         where C.workIntno == sessionEntry.workIntno && (!sessionEntry.schedule.HasValue || J.dubbTrnHdrIntno == sessionEntry.schedule) && G.activityType == "04" && D.voiceActorIntno == actor && D.actorName == actorName
                         select new { C.workIntno, C.orderTrnHdrIntno, C.episodeNo, D.dubbSheetHdrIntno, F.sceneNo, F.isTaken }).Distinct()
                         .OrderBy(b => new { b.workIntno, b.episodeNo, b.sceneNo }).ToList();
            
            foreach (var item in model)
            {
                ViewModels.dubbingSceneViewModel scn = new ViewModels.dubbingSceneViewModel();
                var x = db.scenes.FirstOrDefault(b => b.orderTrnHdrIntno == item.orderTrnHdrIntno && b.sceneNo == item.sceneNo);
                scn.sceneIntno = x.sceneIntno;
                scn.orderTrnHdrIntno = item.orderTrnHdrIntno;
                scn.dubbSheetHdrIntno = item.dubbSheetHdrIntno;
                scn.actor = actor;
                scn.actorName = actorName;
                long work = item.workIntno;
                scn.workIntno = work;
                scn.workName = db.agreementWorks.Find(work).workName;
                scn.episodeNo = item.episodeNo;
                scn.sceneNo = item.sceneNo;
                scn.startTimeCode = x.startTimeCode;
                scn.isTaken = item.isTaken;
                scnList.Add(scn);
            }

            return PartialView("_scenesList", scnList);
        }

        public ActionResult subtitlesList(long sceneId, long sheetHdr)
        {
            var model = db.subtitles.Include(b => b.dialog).Include(b => b.dubbingSheetHdr).Where(b => b.dialog.sceneIntno == sceneId).OrderBy(b => b.subtitleNo);
            ViewBag.sheetHdr = sheetHdr;
            ViewBag.sceneId = sceneId;
            ViewBag.sceneNo = db.scenes.Find(sceneId).sceneNo;
            return PartialView("_subtitlesList", model.ToList());
        }

        public JsonResult sceneTaken(long id, long sheetHdr)
        {
            var scn = db.scenes.Find(id);
            short sceneNo = scn.sceneNo;
            var oi = db.orderTrnHdrs.Find(scn.orderTrnHdrIntno);
            if (!oi.startDubbing.HasValue)
                oi.startDubbing = DateTime.Now.Date;

            string loginUserName = User.Identity.GetUserName();
            long supervisor = db.employees.FirstOrDefault(b => b.email == loginUserName).empIntno;
            var model = db.dubbingSheetDtls.SingleOrDefault(b => b.dubbSheetHdrIntno == sheetHdr && b.sceneNo == sceneNo);
            if(model != null)
            {
                model.isTaken = true;
                model.supervisor = supervisor;
                model.takenTimeStamp = DateTime.Now;
                model.dubbingDate = DateTime.Now.Date;
            }
            else
            {
                var item = db.dubbingSheetDtls.Create();
                item.dubbSheetHdrIntno = sheetHdr;
                item.orderTrnHdrIntno = scn.orderTrnHdrIntno;
                item.sceneNo = sceneNo;
                item.isTaken = true;
                item.supervisor = supervisor;
                item.takenTimeStamp = DateTime.Now;
                item.dubbingDate = DateTime.Now.Date;
                db.dubbingSheetDtls.Add(item);
            }
            db.SaveChanges();
            
            studioUpdateResponse result = new studioUpdateResponse();
            result.episodeNo = oi.episodeNo;
            result.sceneIntno = id;
            result.sceneNo = sceneNo;

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public ActionResult progressBarUpdate()
        {
            int progress = 0;
            if (sessionEntry.studioIntno.HasValue && sessionEntry.schedule.HasValue)
            {
                int x = (from A in db.studios
                         join B in db.studioEpisodes on A.studioIntno equals B.studioIntno
                         join C in db.orderTrnDtls on B.orderTrnDtlIntno equals C.orderTrnDtlIntno
                         join D in db.orderTrnHdrs on C.orderTrnHdrIntno equals D.orderTrnHdrIntno
                         join E in db.dubbingSheetDtls on D.orderTrnHdrIntno equals E.orderTrnHdrIntno
                         where A.studioIntno == sessionEntry.studioIntno && A.dubbTrnHdrIntno == sessionEntry.schedule
                         select E).Count();
                int y = (from A in db.studios
                         join B in db.studioEpisodes on A.studioIntno equals B.studioIntno
                         join C in db.orderTrnDtls on B.orderTrnDtlIntno equals C.orderTrnDtlIntno
                         join D in db.orderTrnHdrs on C.orderTrnHdrIntno equals D.orderTrnHdrIntno
                         join E in db.scenes on D.orderTrnHdrIntno equals E.orderTrnHdrIntno
                         where A.studioIntno == sessionEntry.studioIntno && A.dubbTrnHdrIntno == sessionEntry.schedule
                         select E).Count();
                progress = x * 100 / y;
            }

            return Content(progress.ToString(), "text/html");
        }
    }
}