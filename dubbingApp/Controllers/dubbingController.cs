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

        public class studioUpdateResponse
        {
            public string workName { get; set; }
            public short? episodeNo { get; set; }
            public short? sceneNo { get; set; }
            public string startTimeCode { get; set; }
            public long? sceneIntno { get; set; }
        }

        // GET: dubbing
        public ActionResult Index()
        {
            DateTime dToday = DateTime.Today.Date;
            var sch = db.dubbingTrnHdrs.FirstOrDefault(b => b.fromDate <= dToday && b.thruDate >= dToday && b.status == true);
            long schedule;
            if (sch != null)
            {
                schedule = sch.dubbTrnHdrIntno;
                ViewBag.scheduleIntno = schedule;
                string loginUserName = User.Identity.GetUserName();
                var x = db.studios.FirstOrDefault(b => b.dubbTrnHdrIntno == schedule
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
            else
                return View("studioNotAllocated");
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
                         join E in db.scenes on C.orderTrnHdrIntno equals E.orderTrnHdrIntno
                         join F in db.dialogs on E.sceneIntno equals F.sceneIntno
                         join G in db.subtitles on F.dialogIntno equals G.dialogIntno
                         join H in db.subtitles on D.dubbSheetHdrIntno equals H.dubbSheetHdrIntno
                         where A.studioIntno == studio && D.voiceActorIntno == actor && D.actorName == actorName
                         select new { E.sceneIntno, B.workIntno, C.orderTrnHdrIntno, C.episodeNo, D.dubbSheetHdrIntno, E.sceneNo, E.startTimeCode }).Distinct()
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
                var z = db.dubbingSheetDtls.FirstOrDefault(b => b.dubbSheetHdrIntno == scn.dubbSheetHdrIntno && b.sceneNo == scn.sceneNo);
                if (z == null)
                    scn.isTaken = false;
                else
                    scn.isTaken = true;
                scnList.Add(scn);
            }
            
            return PartialView("_scenesList", scnList);
        }

        public ActionResult dialoguesList(long sceneId, long sheetHdr)
        {
            var model = db.dialogs.Where(b => b.sceneIntno == sceneId).OrderBy(b => b.dialogNo);
            ViewBag.sheetHdr = sheetHdr;
            return PartialView("_dialoguesList", model.ToList());
        }

        public ActionResult subtitlesList(long dialogId, long sheetHdr)
        {
            var model = db.subtitles.Include(b => b.dubbingSheetHdr).Where(b => b.dialogIntno == dialogId).OrderBy(b => b.subtitleNo);
            ViewBag.sheetHdr = sheetHdr;
            return PartialView("_subtitlesList", model.ToList());
        }

        public ActionResult selectEpisodeFirstScene(long sheetHdr)
        {
            var x = (from A in db.subtitles
                    join B in db.dialogs on A.dialogIntno equals B.dialogIntno
                    join C in db.scenes on B.sceneIntno equals C.sceneIntno
                    where A.dubbSheetHdrIntno == sheetHdr && !B.isTaken
                    select new { C.sceneIntno, C.sceneNo, C.startTimeCode }).Distinct();
            studioUpdateResponse result = new studioUpdateResponse();
            if (x != null)
            {
                var z = x.OrderBy(b => b.sceneNo).First();
                result.sceneNo = z.sceneNo;
                result.sceneIntno = z.sceneIntno;
                result.startTimeCode = z.startTimeCode;
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public ActionResult dialogueTaken(long id, long studioIntno)
        {
            var dlg = db.dialogs.FirstOrDefault(b => b.dialogIntno == id);
            short sceneNo = dlg.scene.sceneNo;
            long sceneId = dlg.sceneIntno;
            long sheetHdr = db.subtitles.FirstOrDefault(b => b.dialogIntno == id).dubbSheetHdrIntno;
            var dlgList = db.dialogs.Where(b => b.sceneIntno == sceneId && b.isTaken == false);

            long orderItem = dlg.scene.orderTrnHdrIntno;
            var orderHdr = db.orderTrnHdrs.Include(b => b.agreementWork).FirstOrDefault(b => b.orderTrnHdrIntno == orderItem);

            studioUpdateResponse result = new studioUpdateResponse();
            result.workName = orderHdr.agreementWork.workName;
            result.episodeNo = orderHdr.episodeNo;

            var model = db.dubbingSheetDtls;
            if (dlgList.Count() == 1)
            {
                //insert taken scene and get next scene
                dubbingSheetDtl dtl = new dubbingSheetDtl();
                var std = db.studios.Find(studioIntno);
                dtl.orderTrnHdrIntno = orderItem;
                dtl.dubbSheetHdrIntno = sheetHdr;
                dtl.sceneNo = sceneNo;
                dtl.isTaken = true;
                dtl.studioNo = std.studioNo;
                dtl.supervisor = std.supervisor;
                dtl.soundTechnician = std.sound;
                dtl.takenTimeStamp = DateTime.Now;
                model.Add(dtl);
                db.SaveChanges();

                //get next scene if any
                var y = db.scenes.FirstOrDefault(b => b.orderTrnHdrIntno == orderItem && b.sceneNo > sceneNo);
                if (y != null)
                {
                    result.sceneNo = y.sceneNo;
                    result.startTimeCode = y.startTimeCode;
                    result.sceneIntno = y.sceneIntno;
                }
                else
                {
                    result.sceneNo = null;
                    result.startTimeCode = null;
                    result.sceneIntno = sceneId;
                }
            }
            else
            {
                result.sceneNo = sceneNo;
                result.startTimeCode = dlg.scene.startTimeCode;
                result.sceneIntno = sceneId;
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
                int x = (from A in db.studios
                         join B in db.studioEpisodes on A.studioIntno equals B.studioIntno
                         join C in db.dubbingTrnDtls on B.dubbTrnDtlIntno equals C.dubbTrnDtlIntno
                         join D in db.orderTrnHdrs on C.orderTrnHdrIntno equals D.orderTrnHdrIntno
                         join E in db.dubbingSheetDtls on D.orderTrnHdrIntno equals E.orderTrnHdrIntno
                         where A.studioIntno == studio && A.dubbTrnHdrIntno == schedule && B.status == true
                         select E).Count();
                int y = (from A in db.studios
                         join B in db.studioEpisodes on A.studioIntno equals B.studioIntno
                         join C in db.dubbingTrnDtls on B.dubbTrnDtlIntno equals C.dubbTrnDtlIntno
                         join D in db.orderTrnHdrs on C.orderTrnHdrIntno equals D.orderTrnHdrIntno
                         join E in db.scenes on D.orderTrnHdrIntno equals E.orderTrnHdrIntno
                         where A.studioIntno == studio && A.dubbTrnHdrIntno == schedule && B.status == true
                         select E).Count();
                progress = x * 100 / y;
            }
            
            return Content(progress.ToString(), "text/html");
        }
    }
}