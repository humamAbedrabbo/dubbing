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
    //[Authorize(Roles = "ADMIN, GENERAL_MANAGER, PRODUCTION_MANAGER")]
    public class calendarController : Controller
    {
        private DUBBDBEntities db = new DUBBDBEntities();
        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }

        // GET: calendar
        public ActionResult Index()
        {
            var x = db.dubbingTrnHdrs.Where(b => b.status == true).ToList();
            var schedulesList = new List<KeyValuePair<long, string>>();
            foreach (dubbingTrnHdr hdr in x)
            {
                string wk = hdr.fromDate.ToString("dd/MM") + " - " + hdr.thruDate.ToString("dd/MM");
                schedulesList.Add(new KeyValuePair<long, string>(hdr.dubbTrnHdrIntno, wk));
            }
            ViewBag.schedulesList = schedulesList;
            return View();
        }

        public ActionResult actorsList(long schedule)
        {
            var model = (from A in db.dubbingTrnDtls
                         join B in db.orderTrnHdrs on A.orderTrnHdrIntno equals B.orderTrnHdrIntno
                         join C in db.dubbingSheetHdrs on B.orderTrnHdrIntno equals C.orderTrnHdrIntno
                         join D in db.voiceActors on C.voiceActorIntno equals D.voiceActorIntno
                         where A.dubbTrnHdrIntno == schedule
                         select new { D.voiceActorIntno, D.fullName }).Distinct();
            ViewBag.actorsList = model.ToList();
            ViewBag.schedule = schedule;
            return PartialView("_actorsList");
        }

        public ActionResult appointmentsList(long actor, long schedule)
        {
            var model = db.dubbingAppointments.Include(b => b.studio).Include(b => b.agreementWork)
                        .Where(b => b.voiceActorIntno == actor && b.studio.dubbTrnHdrIntno == schedule);
            return PartialView("_appointmentsList", model.ToList());
        }

        public ActionResult appointmentUpdate(long apt)
        {
            var model = db.dubbingAppointments.Find(apt);
            long sch = db.studios.Find(model.studioIntno).dubbTrnHdrIntno;
            var x = db.studioDtls.Include(b => b.studio)
                                .Where(b => b.studio.dubbTrnHdrIntno == sch && b.workIntno == model.workIntno)
                                .Select(b => new { b.studioIntno, studioName = "Studio " + b.studio.studioNo }).Distinct().ToList();
            ViewBag.studiosList = new SelectList(x, "studioIntno", "studioName");
            return PartialView("_appointmentUpdate", model);
        }

        [ValidateAntiForgeryToken]
        [HttpPost, ValidateInput(false)]
        public ActionResult appointmentUpdate(dubbingAppointment item)
        {
            var model = db.dubbingAppointments;
            var modelItem = model.Find(item.dubbAppointIntno);
            long actor1 = modelItem.voiceActorIntno;
            long std = modelItem.studioIntno;

            this.UpdateModel(modelItem);
            db.SaveChanges();

            long schedule1 = db.studios.Find(std).dubbTrnHdrIntno;
            return RedirectToAction("appointmentsList", new { actor = actor1, schedule = schedule1 });
        }

        public ActionResult appointmentDelete(long apt)
        {
            var model = db.dubbingAppointments;
            var modelItem = model.Find(apt);
            long actor1 = modelItem.voiceActorIntno;
            long std = modelItem.studioIntno;

            model.Remove(modelItem);
            db.SaveChanges();

            long schedule1 = db.studios.Find(std).dubbTrnHdrIntno;
            return RedirectToAction("appointmentsList", new { actor = actor1, schedule = schedule1 });
        }
    }
}