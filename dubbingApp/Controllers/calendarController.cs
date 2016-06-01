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
                         select new { D.voiceActorIntno, D.fullName }).Distinct().OrderBy(b => b.fullName);
            ViewBag.actorsList = model.ToList();
            ViewBag.schedule = schedule;
            return PartialView("_actorsList");
        }

        public ActionResult appointmentsList(long actor, long schedule)
        {
            var model = db.dubbingAppointments.Include(b => b.studio).Include(b => b.agreementWork)
                        .Where(b => b.voiceActorIntno == actor && b.studio.dubbTrnHdrIntno == schedule)
                        .OrderBy(b => new { b.appointmentDate, b.fromTime });
            return PartialView("_appointmentsList", model.ToList());
        }

        public ActionResult appointmentUpdate(long apt)
        {
            var model = db.dubbingAppointments.Find(apt);
            long sch = db.studios.Find(model.studioIntno).dubbTrnHdrIntno;
            var x = db.studios
                    .Where(b => b.dubbTrnHdrIntno == sch && b.workIntno == model.workIntno)
                    .Select(b => new { b.studioIntno, studioName = "Studio " + b.studioNo }).Distinct().ToList();
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

            // validate appointment date within the schedule range, and validate thru time
            var x = db.studios.Include(b => b.dubbingTrnHdr).FirstOrDefault(b => b.studioIntno == std);
            if (item.appointmentDate < x.dubbingTrnHdr.fromDate || item.appointmentDate > x.dubbingTrnHdr.thruDate)
                return new HttpStatusCodeResult(500, "Appointment Date Should Fall Within the Schedule Week. Please Pick Another Date.");
            else if ((item.fromTime.HasValue && !item.thruTime.HasValue) || (item.actualFromTime.HasValue && !item.actualThruTime.HasValue))
                return new HttpStatusCodeResult(500, "End Time Must be Provided.");
            else
            {
                this.UpdateModel(modelItem);
                db.SaveChanges();
            }
            
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

        public ActionResult studioCalendarPopup(long std)
        {
            var model = db.dubbingAppointments.Include(b => b.voiceActor).Where(b => b.studioIntno == std)
                        .OrderBy(b => b.appointmentDate);
            return PartialView("_studioCalendarPopup", model.ToList());
        }
    }
}