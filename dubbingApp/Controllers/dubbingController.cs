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
    //[Authorize(Roles = "ADMIN, GENERAL_MANAGER, PRODUCTION_MANAGER, STUDIO_SUPERVISOR")]
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
            var x = db.studios.Include(b => b.employee).FirstOrDefault(b => b.studioNo == "01" && b.supervisor.HasValue);
            ViewBag.studio = x.studioIntno;
            ViewBag.team = "Welcome! " + (x.supervisor.HasValue ? x.employee.fullName : "Supervisor")
                                        + (x.sound.HasValue ? (" & " + x.employee1.fullName) : " & Sound Technician");
            return View();
        }

        public ActionResult sceneHeader()
        {
            return PartialView("_sceneHeader");
        }

        public ActionResult actorsList(long std)
        {
            //list should be ordered according to appointments
            DateTime dToday = DateTime.Today;
            var model = (from A in db.dubbingSheetHdrs
                         join C in db.dubbingTrnDtls on A.orderTrnHdrIntno equals C.orderTrnHdrIntno
                         join D in db.dubbingTrnHdrs on C.dubbTrnHdrIntno equals D.dubbTrnHdrIntno
                         join E in db.studios on D.dubbTrnHdrIntno equals E.dubbTrnHdrIntno
                         where E.studioIntno == std && dToday >= D.fromDate && dToday <= D.thruDate
                         select A).Include(b => b.voiceActor).Include(b => b.workCharacter);
            //ViewBag.actorsList = model.Select(b => new { b.voiceActorIntno, b.voiceActor.fullName }).Distinct();
            ViewBag.actorsList = db.voiceActors.Select(b => new { b.voiceActorIntno, b.fullName }).ToList(); //to be removed later and to use the above viewbag
            return PartialView("_actorsList");
        }

        public ActionResult scenesList(string actor)
        {
            long actorIntno = long.Parse(actor);
            return PartialView("_scenesList");
        }

        public ActionResult dialoguesList(string actor)
        {
            //return ordered dialogues per work, episode, actor, scene ordered by start time
            //dialogue should have an id
            //scene has sceneNo and isTaken flag
            long actorIntno = long.Parse(actor);
            return PartialView("_dialoguesList");
        }

        public ActionResult dialogueTaken(string id)
        {
            return null;
        }
    }
}