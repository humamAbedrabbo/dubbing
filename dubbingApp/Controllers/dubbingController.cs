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
    [Authorize(Roles = "ADMIN, GENERAL_MANAGER, PRODUCTION_MANAGER, STUDIO_SUPERVISOR")]
    public class dubbingController : Controller
    {
        private DUBBDBEntities db = new DUBBDBEntities();
        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
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
            var x = db.studios.Include(b => b.employee).FirstOrDefault(b => (!schedule.HasValue || b.dubbTrnHdrIntno == schedule.Value) && b.studioNo == "01");
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

        public ActionResult scheduledWorksList(long studio, long? schedule)
        {
            if (schedule.HasValue)
            {
                var model = db.studioEpisodes.Include(b => b.studio).Include(b => b.dubbingTrnDtl).Include(b => b.dubbingTrnDtl.agreementWork)
                        .Where(b => b.studioIntno == studio && b.studio.dubbTrnHdrIntno == schedule.Value && b.status == true);
                return PartialView("_scheduledWorksList", model.ToList());
            }
            else
                return PartialView("_scheduledWorksList");
        }

        public ActionResult actorsList(long std)
        {
            //list should be ordered according to appointments
            DateTime dToday = DateTime.Today;
            string nonCastedCharacter = db.voiceActors.Find(0).fullName;
            var model = (from A in db.dubbingSheetHdrs
                         join C in db.dubbingTrnDtls on A.orderTrnHdrIntno equals C.orderTrnHdrIntno
                         join D in db.dubbingTrnHdrs on C.dubbTrnHdrIntno equals D.dubbTrnHdrIntno
                         join E in db.studios on D.dubbTrnHdrIntno equals E.dubbTrnHdrIntno
                         //where E.studioIntno == std && dToday >= D.fromDate && dToday <= D.thruDate && A.actorName != nonCastedCharacter
                         select A).Include(b => b.workCharacter);
            ViewBag.actorsList = model.Select(b => new { b.voiceActorIntno, b.actorName }).Distinct().ToList();
            return PartialView("_actorsList");
        }

        public ActionResult scenesList(long actorIntno, string actorName, long studio)
        {
            List<ViewModels.dubbingSceneViewModel> scnList = new List<ViewModels.dubbingSceneViewModel>();
            var model = (from A in db.studioEpisodes
                         join B in db.dubbingTrnDtls on A.dubbTrnDtlIntno equals B.dubbTrnDtlIntno
                         join C in db.orderTrnHdrs on B.orderTrnHdrIntno equals C.orderTrnHdrIntno
                         join D in db.dubbingSheetHdrs on C.orderTrnHdrIntno equals D.orderTrnHdrIntno
                         join E in db.dubbingSheetDtls on D.dubbSheetHdrIntno equals E.dubbSheetHdrIntno
                         where A.studioIntno == studio && D.voiceActorIntno == actorIntno && D.actorName == actorName
                         select new { E.dubbSheetDtlIntno, B.workIntno, C.orderTrnHdrIntno, C.episodeNo, D.dubbSheetHdrIntno, E.sceneNo, E.startTimeCode, E.isTaken })
                         .OrderBy(b => new { b.workIntno, b.episodeNo, b.sceneNo });
            foreach(var item in model)
            {
                ViewModels.dubbingSceneViewModel scn = new ViewModels.dubbingSceneViewModel();
                scn.dubbSheetDtlIntno = item.dubbSheetDtlIntno;
                scn.orderTrnHdrIntno = item.orderTrnHdrIntno;
                scn.dubbSheetHdrIntno = item.dubbSheetHdrIntno;
                long work = item.workIntno;
                scn.workIntno = work;
                scn.workName = db.agreementWorks.Find(work).workName;
                scn.episodeNo = item.episodeNo;
                scn.actorIntno = actorIntno;
                scn.actorName = actorName;
                scn.sceneNo = item.sceneNo;
                scn.startTimeCode = item.startTimeCode;
                scn.isTaken = item.isTaken;
                scnList.Add(scn);
            }
            
            return PartialView("_scenesList", scnList);
        }

        public ActionResult dialoguesList(long actorIntno, string actorName)
        {
            //return ordered dialogues per work, episode, actor, scene ordered by start time
            //dialogue should have an id
            //scene has sceneNo and isTaken flag
            return PartialView("_dialoguesList");
        }
        
        public ActionResult sceneTaken(long sheetHdr)
        {
            string scene;
            var x = db.dubbingSheetDtls.Where(b => b.dubbSheetHdrIntno == sheetHdr && !b.isTaken).OrderBy(b => b.sceneNo);
            if (x.Count() != 0)
            {
                scene = x.First().sceneNo + "|" + x.First().startTimeCode;
                return Content(scene, "text/html");
            }
            else
                return null;    
        }

        public ActionResult dialogueTaken(long id, long scene, int episode, long work)
        {
            // save dialogue to db - it might be edited by supervisor
            int inSceneRemainDialog = 2; //get the count of remaining dialogues in the scene
            if (inSceneRemainDialog == 0)
            {
                //update current scene status (isTaken = true) and get next scene
                var x = db.dubbingSheetDtls.Find(scene);
                x.isTaken = true;
                long sheet = x.dubbSheetHdrIntno;
                int sceneNo = x.sceneNo;
                ViewBag.nextScene = db.dubbingSheetDtls.FirstOrDefault(b => b.dubbSheetHdrIntno == sheet && b.sceneNo > sceneNo && b.isTaken == false).dubbSheetDtlIntno;
                db.SaveChanges();
            }
            return null;
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