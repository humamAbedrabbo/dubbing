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
    [Authorize(Roles = "ADMIN, GENERAL_MANAGER, PRODUCTION_MANAGER")]
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
            var x = db.dubbingTrnHdrs.Where(b => b.status == true).OrderBy(b => b.fromDate).ToList();
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
            var model = (from A in db.studios
                         join B in db.dubbingAppointments on A.studioIntno equals B.studioIntno
                         join C in db.dubbingSheetHdrs on B.voiceActorIntno equals C.voiceActorIntno
                         join D in db.voiceActors on C.voiceActorIntno equals D.voiceActorIntno
                         where A.dubbTrnHdrIntno == schedule
                         select new { D.voiceActorIntno, D.fullName, B.totalScenes }).Distinct();
            ViewBag.actorsList = new SelectList(model.OrderByDescending(b => b.totalScenes).ToList(), "voiceActorIntno", "fullName");
            ViewBag.schedule = schedule;
            return PartialView("_actorsList");
        }

        public ActionResult getCalendar(long schedule)
        {
            var model = db.dubbingAppointments.Include(b => b.voiceActor).Include(b => b.studio)
                            .Where(b => b.studio.dubbTrnHdrIntno == schedule).OrderBy(b => b.appointmentDate).ThenBy(b => b.fromTime).ToList();
            string fromTime;
            string thruTime;
            foreach(var item in model)
            {
                fromTime = item.fromTime.HasValue ? item.fromTime.Value.ToString("HH:mm") : null;
                thruTime = item.thruTime.HasValue ? item.thruTime.Value.ToString("HH:mm") : null;
                item.actorName = item.actorName + "|" + "(" + fromTime + " - " + thruTime + ")";
            }
            ViewBag.scheduleStartDate = db.dubbingTrnHdrs.Find(schedule).fromDate;
            ViewBag.studiosList = db.dubbDomains.Where(b => b.domainName == "studio" && b.status == true).OrderBy(b => b.sortOrder).ToList();
            
            return PartialView("_scheduleCalendar", model);
        }

        public ActionResult supervisorsList(long schedule)
        {
            var x = (from A in db.orderTrnDtls
                     join B in db.studioEpisodes on A.orderTrnDtlIntno equals B.orderTrnDtlIntno
                     join C in db.studios on B.studioIntno equals C.studioIntno
                     join D in db.employees on A.empIntno equals D.empIntno
                     where C.dubbTrnHdrIntno == schedule && A.status == true
                     select new { C.studioNo, D.fullName }).Distinct().OrderBy(b => b.studioNo).ToList();
            ViewBag.supervisorsList = new SelectList(x, "studioNo", "fullName");
            return PartialView("_supervisorsList");
        }

        public ActionResult appointmentsList(long actor, long schedule)
        {
            var model = db.dubbingAppointments.Include(b => b.studio).Include(b => b.agreementWork)
                        .Where(b => b.voiceActorIntno == actor && b.studio.dubbTrnHdrIntno == schedule)
                        .OrderBy(b => new { b.appointmentDate, b.fromTime });
            return PartialView("_appointmentsList", model.ToList());
        }

        public ActionResult appointmentAddNew(long id)
        {
            var x = db.dubbingAppointments.Include(b => b.studio).FirstOrDefault(b => b.dubbAppointIntno == id);
            var model = db.dubbingAppointments.Create();
            model.voiceActorIntno = x.voiceActorIntno;
            model.actorName = x.actorName;
            model.workIntno = x.workIntno;
            model.studioIntno = x.studioIntno;
            model.appointmentDate = x.appointmentDate;
            model.totalScenes = x.totalScenes;
            model.totalMinutes = x.totalMinutes;
            db.dubbingAppointments.Add(model);
            db.SaveChanges();

            return RedirectToAction("appointmentsList", new { actor = x.voiceActorIntno, schedule = x.studio.dubbTrnHdrIntno });
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