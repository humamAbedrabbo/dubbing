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

        public class studioUpdateResponse
        {
            public string workName { get; set; }
            public short? episodeNo { get; set; }
            public short? sceneNo { get; set; }
            public string startTimeCode { get; set; }
            public long? sheetDtl { get; set; }
        }

        // GET: dubbing
        public ActionResult Index()
        {
            DateTime dToday = DateTime.Today.Date;
            var sch = db.dubbingTrnHdrs.FirstOrDefault(b => b.fromDate <= dToday && b.thruDate >= dToday && b.status == true);
            long? schedule = null;
            if (sch != null)
            {
                schedule = sch.dubbTrnHdrIntno;
                ViewBag.scheduleIntno = schedule;
            }
            string loginUserName = User.Identity.GetUserName();
            var x = db.studios.Include(b => b.employee).FirstOrDefault(b => (!schedule.HasValue || b.dubbTrnHdrIntno == schedule.Value) 
                    && (b.employee1.email == loginUserName || b.employee.email == loginUserName));
            if (x == null)
            {
                return View("studioNotAllocated");
            }
            else
            {
                ViewBag.studioIntno = x.studioIntno;
                ViewBag.team = "Welcome! " + x.employee1.fullName + " & " + x.employee.fullName;
                return View();
            }
            
        }

        public ActionResult sceneHeader()
        {
            return PartialView("_sceneHeader");
        }

        public ActionResult actorsList(long std)
        {
            var model = db.dubbingAppointments.Where(b => b.studioIntno == std).OrderBy(b => new { b.appointmentDate, b.fromTime });
            return PartialView("_actorsList", model.ToList());
        }

        public ActionResult scheduledWorksList(long studio)
        {
            var model = db.studioEpisodes.Include(b => b.dubbingTrnDtl.agreementWork)
                        .Where(b => b.studioIntno == studio && b.status == true);
            return PartialView("_scheduledWorksList", model.ToList());
        }

        public ActionResult scenesList(long actor, string actorName, long studio)
        {
            List<ViewModels.dubbingSceneViewModel> scnList = new List<ViewModels.dubbingSceneViewModel>();
            var model = (from A in db.studioEpisodes
                         join B in db.dubbingTrnDtls on A.dubbTrnDtlIntno equals B.dubbTrnDtlIntno
                         join C in db.orderTrnHdrs on B.orderTrnHdrIntno equals C.orderTrnHdrIntno
                         join D in db.dubbingSheetHdrs on C.orderTrnHdrIntno equals D.orderTrnHdrIntno
                         join E in db.dubbingSheetDtls on D.dubbSheetHdrIntno equals E.dubbSheetHdrIntno
                         where A.studioIntno == studio && D.voiceActorIntno == actor && D.actorName == actorName
                         select new { E.dubbSheetDtlIntno, B.workIntno, C.orderTrnHdrIntno, C.episodeNo, D.dubbSheetHdrIntno, E.sceneNo, E.startTimeCode, E.isTaken })
                         .OrderBy(b => new { b.workIntno, b.episodeNo, b.sceneNo });
            foreach(var item in model)
            {
                ViewModels.dubbingSceneViewModel scn = new ViewModels.dubbingSceneViewModel();
                scn.dubbSheetDtlIntno = item.dubbSheetDtlIntno;
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
                scn.isTaken = item.isTaken;
                scnList.Add(scn);
            }
            
            return PartialView("_scenesList", scnList);
        }

        public ActionResult dialoguesList(long sheetHdr)
        {
            short scn;
            var model = db.adaptationDialogs.Include(b => b.dubbingSheetHdr)
                        .Where(b => b.dubbSheetHdrIntno == sheetHdr && b.isTaken == false)
                        .OrderBy(b => new { b.sceneNo, b.dialogNo });
            var x = db.adaptationDialogs.Include(b => b.dubbingSheetHdr)
                    .FirstOrDefault(b => b.dubbSheetHdrIntno == sheetHdr && b.isTaken == false);
            if (x != null)
            {
                scn = x.sceneNo;
                return PartialView("_dialoguesList", model.Where(b => b.sceneNo == scn).ToList());
            }
            else
                return PartialView("_dialoguesList", model.ToList());
        }

        public ActionResult subtitlesList(long dialogue)
        {
            var model = db.adaptationSubtitles.Where(b => b.dialogIntno == dialogue).OrderBy(b => b.subtitleNo);
            return PartialView("_subtitlesList", model.ToList());
        }

        public ActionResult selectEpisodeFirstScene(long sheetHdr)
        {
            var x = db.dubbingSheetDtls.Where(b => b.dubbSheetHdrIntno == sheetHdr && !b.isTaken).OrderBy(b => b.sceneNo);
            studioUpdateResponse result = new studioUpdateResponse();
            if (x.Count() != 0)
            {
                var z = x.First();
                result.sceneNo = z.sceneNo;
                result.startTimeCode = z.startTimeCode;
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public ActionResult dialogueTaken(long id, long studioIntno)
        {
            var dlg = db.adaptationDialogs.Find(id);
            short sceneNo = dlg.sceneNo;
            long sheetHdr = dlg.dubbSheetHdrIntno;
            var dlgList = db.adaptationDialogs.Where(b => b.dubbSheetHdrIntno == sheetHdr && b.sceneNo == sceneNo && b.isTaken == false);

            long orderItem = db.dubbingSheetHdrs.Find(sheetHdr).orderTrnHdrIntno;
            var orderHdr = db.orderTrnHdrs.Include(b => b.agreementWork).FirstOrDefault(b => b.orderTrnHdrIntno == orderItem);

            studioUpdateResponse result = new studioUpdateResponse();
            result.workName = orderHdr.agreementWork.workName;
            result.episodeNo = orderHdr.episodeNo;

            var x = db.dubbingSheetDtls.FirstOrDefault(b => b.dubbSheetHdrIntno == sheetHdr && b.sceneNo == sceneNo);
            long? dtl = null;
            if (dlgList.Count() == 1)
            {
                //update current scene status (isTaken = true) and get next scene
                var std = db.studios.Find(studioIntno);
                dtl = x.dubbSheetDtlIntno;
                x.isTaken = true;
                x.studioNo = std.studioNo;
                x.supervisor = std.supervisor;
                x.soundTechnician = std.sound;
                x.takenTimeStamp = DateTime.Now;

                //get next scene if any
                var y = db.dubbingSheetDtls.FirstOrDefault(b => b.dubbSheetHdrIntno == sheetHdr && b.sceneNo > sceneNo);
                if (y != null)
                {
                    result.sceneNo = y.sceneNo;
                    result.startTimeCode = y.startTimeCode;
                    result.sheetDtl = dtl;
                }
                else
                {
                    result.sceneNo = null;
                    result.startTimeCode = null;
                    result.sheetDtl = dtl;
                }
            }
            else
            {
                result.sceneNo = x.sceneNo;
                result.startTimeCode = x.startTimeCode;
                result.sheetDtl = null;
            }
            dlg.isTaken = true;
            db.SaveChanges();
            
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public ActionResult progressBarUpdate(long studio, long? schedule)
        {
            int progress = 0;
            if (schedule.HasValue)
            {
                var x = (from A in db.studios
                         join B in db.studioEpisodes on A.studioIntno equals B.studioIntno
                         join C in db.dubbingTrnDtls on B.dubbTrnDtlIntno equals C.dubbTrnDtlIntno
                         join D in db.orderTrnHdrs on C.orderTrnHdrIntno equals D.orderTrnHdrIntno
                         join E in db.dubbingSheetDtls on D.orderTrnHdrIntno equals E.orderTrnHdrIntno
                         where A.studioIntno == studio && A.dubbTrnHdrIntno == schedule && B.status == true
                         select new { E.isTaken });
                progress = x.Where(b => b.isTaken == true).Count() * 100 / x.Count();
            }
            
            return Content(progress.ToString(), "text/html");
        }
    }
}