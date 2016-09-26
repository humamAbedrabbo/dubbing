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

            var s = db.dubbingTrnHdrs.FirstOrDefault(b => b.fromDate <= dToday && b.thruDate >= dToday);
            if (s != null)
                sessionEntry.schedule = s.dubbTrnHdrIntno;

            //get all assignments for the login supervisor
            var assignments = db.orderTrnDtls.Include(b => b.employee).Include(b => b.orderTrnHdr.agreementWork)
                                    .Where(b => b.employee.email == loginUserName && b.status == true)
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
                item.dueDate = x.forDueDate.Value.ToShortDateString();

                var std = db.studioEpisodes.Include(b => b.studio.dubbingTrnHdr)
                            .FirstOrDefault(b => b.orderTrnDtlIntno == x.orderTrnDtlIntno 
                            && b.studio.dubbingTrnHdr.fromDate <= dToday && b.studio.dubbingTrnHdr.thruDate >= dToday);
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
                            .FirstOrDefault(b => b.studioEpisodeIntno == id);
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
                         select C);
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
            var x = (from A in db.subtitles
                     join B in db.dialogs on A.dialogIntno equals B.dialogIntno
                     join C in db.scenes on B.sceneIntno equals C.sceneIntno
                     where A.dubbSheetHdrIntno == sheetHdr && !B.isTaken
                     select new { C.sceneIntno, C.sceneNo, C.startTimeCode }).Distinct();
            studioUpdateResponse result = new studioUpdateResponse();
            if (x.Count() != 0)
            {
                var z = x.OrderBy(b => b.sceneNo).First();
                result.sceneNo = z.sceneNo;
                result.sceneIntno = z.sceneIntno;
                result.startTimeCode = z.startTimeCode;
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public ActionResult scenesList(long actor, string actorName)
        {
            List<ViewModels.dubbingSceneViewModel> scnList = new List<ViewModels.dubbingSceneViewModel>();
            var model = (from B in db.orderTrnDtls
                         join C in db.orderTrnHdrs on B.orderTrnHdrIntno equals C.orderTrnHdrIntno
                         join D in db.dubbingSheetHdrs on C.orderTrnHdrIntno equals D.orderTrnHdrIntno
                         join E in db.scenes on C.orderTrnHdrIntno equals E.orderTrnHdrIntno
                         join F in db.dialogs on E.sceneIntno equals F.sceneIntno
                         join G in db.subtitles on F.dialogIntno equals G.dialogIntno
                         join H in db.subtitles on D.dubbSheetHdrIntno equals H.dubbSheetHdrIntno
                         where D.voiceActorIntno == actor && D.actorName == actorName && B.empIntno == sessionEntry.empIntno && B.forDueDate == sessionEntry.dueDate
                         select new { E.sceneIntno, C.workIntno, C.orderTrnHdrIntno, C.episodeNo, D.dubbSheetHdrIntno, E.sceneNo, E.startTimeCode }).Distinct()
                         .OrderBy(b => new { b.workIntno, b.episodeNo, b.sceneNo });
            foreach (var item in model)
            {
                ViewModels.dubbingSceneViewModel scn = new ViewModels.dubbingSceneViewModel();
                scn.sceneIntno = item.sceneIntno;
                scn.orderTrnHdrIntno = item.orderTrnHdrIntno;
                scn.dubbSheetHdrIntno = item.dubbSheetHdrIntno;
                scn.actor = actor;
                scn.actorName = actorName;
                long work = item.workIntno;
                scn.workIntno = work;
                scn.workName = db.agreementWorks.Find(work).workName;
                scn.episodeNo = item.episodeNo;
                scn.sceneNo = item.sceneNo;
                scn.startTimeCode = item.startTimeCode;
                var z = db.subtitles.Include(b => b.dialog).Where(b => b.dubbSheetHdrIntno == item.dubbSheetHdrIntno
                                    && b.dialog.sceneIntno == item.sceneIntno && b.dialog.isTaken == false);
                if (z.Count() == 0)
                    scn.isTaken = true;
                else
                    scn.isTaken = false;
                scnList.Add(scn);
            }

            return PartialView("_scenesList", scnList);
        }

        public ActionResult dialoguesList(long sceneId, long sheetHdr, bool isReadOnly)
        {
            var model = (from A in db.dialogs
                         join B in db.subtitles on A.dialogIntno equals B.dialogIntno
                         where B.dubbSheetHdrIntno == sheetHdr && A.sceneIntno == sceneId && (isReadOnly || A.isTaken == false)
                         select A).Distinct().OrderBy(b => b.dialogNo);
            ViewBag.sheetHdr = sheetHdr;
            return PartialView("_dialoguesList", model.ToList());
        }

        public ActionResult subtitlesList(long dialogId, long sheetHdr)
        {
            var model = db.subtitles.Include(b => b.dubbingSheetHdr).Where(b => b.dialogIntno == dialogId).OrderBy(b => b.subtitleNo);
            ViewBag.sheetHdr = sheetHdr;
            return PartialView("_subtitlesList", model.ToList());
        }

        public ActionResult dialogueTaken(long id, long sheetHdr)
        {
            var dialogItem = db.dialogs.Find(id);
            long scn = dialogItem.sceneIntno;
            var model = (from A in db.dialogs
                         join B in db.scenes on A.sceneIntno equals B.sceneIntno
                         join C in db.orderTrnHdrs on B.orderTrnHdrIntno equals C.orderTrnHdrIntno
                         join D in db.agreementWorks on C.workIntno equals D.workIntno
                         join E in db.subtitles on A.dialogIntno equals E.dialogIntno
                         where B.sceneIntno == scn && A.isTaken == false && E.dubbSheetHdrIntno == sheetHdr
                         select new { D.workName, C.orderTrnHdrIntno, C.episodeNo, B.sceneNo, A.dialogIntno }).Distinct();

            studioUpdateResponse result = new studioUpdateResponse();
            var x = model.FirstOrDefault();
            result.episodeNo = x.episodeNo;
            result.sceneIntno = scn;

            if (model.Count() == 1) //means the taken dialogue is the last one for the actor in the scene
            {
                //insert taken complete scene
                result.sceneNo = x.sceneNo;

                var y = model.First();
                var trnModel = db.dubbingSheetDtls;

                var hdr = db.orderTrnHdrs.FirstOrDefault(b => b.orderTrnHdrIntno == y.orderTrnHdrIntno);
                if (!hdr.startDubbing.HasValue)
                    hdr.startDubbing = DateTime.Now.Date;

                dubbingSheetDtl dtl = new dubbingSheetDtl();
                
                dtl.orderTrnHdrIntno = y.orderTrnHdrIntno;
                dtl.dubbSheetHdrIntno = sheetHdr;
                dtl.sceneNo = y.sceneNo;
                dtl.isTaken = true;
                string loginUserName = User.Identity.GetUserName();
                dtl.supervisor = db.employees.FirstOrDefault(b => b.email == loginUserName).empIntno;
                if (sessionEntry.studioIntno.HasValue)
                {
                    var std = db.studios.Find(sessionEntry.studioIntno);
                    dtl.studioNo = std.studioNo;
                    dtl.soundTechnician = std.sound;
                }
                dtl.takenTimeStamp = DateTime.Now;
                dtl.dubbingDate = DateTime.Now.Date;
                trnModel.Add(dtl);
                db.SaveChanges();
            }
            
            dialogItem.isTaken = true;
            db.SaveChanges();

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