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
    public class schedulerController : Controller
    {
        private DUBBDBEntities db = new DUBBDBEntities();
        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }

        // GET: scheduler
        public ActionResult Index()
        {
            return View();
        }

        //schedules
        public ActionResult schedulesList()
        {
            var model = db.dubbingTrnHdrs.Where(b => b.status == true).OrderBy(b => b.fromDate);
            return PartialView("_schedulesList", model.ToList());
        }

        public ActionResult scheduleDetails(long schedule)
        {
            var hdr = db.dubbingTrnHdrs.Find(schedule);
            ViewBag.scheduleIntno = schedule;
            ViewBag.schedule = hdr.fromDate.ToString("dd/MM") + " - " + hdr.thruDate.ToString("dd/MM");

            List<string> detailsList = new List<string>();
            var x = (from A in db.orderTrnDtls
                     join B in db.studioEpisodes on A.orderTrnDtlIntno equals B.orderTrnDtlIntno
                     join C in db.studios on B.studioIntno equals C.studioIntno
                     join D in db.dubbingTrnHdrs on C.dubbTrnHdrIntno equals D.dubbTrnHdrIntno
                     join E in db.orderTrnHdrs on A.orderTrnHdrIntno equals E.orderTrnHdrIntno
                     join F in db.agreementWorks on E.workIntno equals F.workIntno
                     where D.dubbTrnHdrIntno == schedule
                     select new { F.workIntno, F.workName, E.episodeNo}).Distinct();

            foreach (var x1 in x.Select(b => new { b.workIntno, b.workName }).Distinct())
            {
                int cnt = x.Where(b => b.workIntno == x1.workIntno).Count();
                detailsList.Add(x1.workName + "|" + cnt);
            }
            return PartialView("_scheduleDetails", detailsList);
        }

        public ActionResult schedulesAddNew()
        {
            return PartialView("_schedulesAddNew");
        }

        [ValidateAntiForgeryToken]
        [HttpPost, ValidateInput(false)]
        public ActionResult schedulesAddNew(dubbingTrnHdr item)
        {
            var model = db.dubbingTrnHdrs;
            if (item.fromDate.DayOfWeek.ToString() != LookupModels.decodeDictionaryItem("settings", "fdw"))
                return new HttpStatusCodeResult(500, "Error message");
            else if (model.FirstOrDefault(b => b.fromDate == item.fromDate && b.status == true) != null)
                return new HttpStatusCodeResult(500, "Error message");
            else
            {
                //create the schedule
                item.thruDate = item.fromDate.AddDays(6);
                item.isPaid = false;
                item.status = true;
                model.Add(item);
                db.SaveChanges();

                //populate studios
                var studioModel = db.studios;
                var studioEpisodesModel = db.studioEpisodes;
                var x = db.orderTrnDtls.Include(b => b.orderTrnHdr)
                                        .Where(b => b.activityType == "04" && b.status == true
                                        && b.forDueDate >= item.fromDate && b.forDueDate <= item.thruDate).ToList();
                var y = x.Select(b => new { b.empIntno, b.orderTrnHdr.workIntno }).Distinct();
                foreach(var work in y)
                {
                    studio std = new studio();
                    std.dubbTrnHdrIntno = item.dubbTrnHdrIntno;
                    std.workIntno = work.workIntno;
                    std.sound = work.empIntno; //store supervisor here
                    studioModel.Add(std);

                    foreach(var dtl in x.Where(b => b.empIntno == work.empIntno && b.orderTrnHdr.workIntno == work.workIntno))
                    {
                        studioEpisode ep = new studioEpisode();
                        ep.orderTrnDtlIntno = dtl.orderTrnDtlIntno;
                        studioEpisodesModel.Add(ep);
                    }
                    db.SaveChanges();
                }
                
            }
            return RedirectToAction("schedulesList");
        }

        public ActionResult schedulesReload(long schedule)
        {
            var sch = db.dubbingTrnHdrs.Find(schedule);
            var x = db.orderTrnDtls.Include(b => b.orderTrnHdr)
                                    .Where(b => b.activityType == "04" && b.status == true
                                    && b.forDueDate >= sch.fromDate && b.forDueDate <= sch.thruDate).ToList();
            var y = x.Select(b => new { b.empIntno, b.orderTrnHdr.workIntno }).Distinct();
            foreach (var work in y)
            {
                var s = db.studios.FirstOrDefault(b => b.dubbTrnHdrIntno == schedule && b.workIntno == work.workIntno && b.sound == work.empIntno);
                if(s == null)
                {
                    var std = db.studios.Create();
                    std.dubbTrnHdrIntno = schedule;
                    std.workIntno = work.workIntno;
                    std.sound = work.empIntno;
                    db.studios.Add(std);

                    foreach (var dtl in x.Where(b => b.empIntno == work.empIntno && b.orderTrnHdr.workIntno == work.workIntno))
                    {
                        var ep = db.studioEpisodes.Create();
                        ep.orderTrnDtlIntno = dtl.orderTrnDtlIntno;
                        db.studioEpisodes.Add(ep);
                    }
                    db.SaveChanges();
                }
            }
            
            long schedule1 = schedule;
            return RedirectToAction("assignmentsList", new { schedule = schedule1 });
        }

        public ActionResult schedulesDelete(long schedule)
        {
            var model = db.dubbingTrnHdrs;
            var modelItem = model.Find(schedule);
            model.Remove(modelItem);
            db.SaveChanges();
            return RedirectToAction("schedulesList");
        }

        public ActionResult schedulesEndorse(long schedule)
        {
            var model = db.dubbingTrnHdrs;
            var logModel = db.logOrders;
            var modelItem = model.Find(schedule);
            
            var x = (from A in db.orderTrnDtls
                     join B in db.studioEpisodes on A.orderTrnDtlIntno equals B.orderTrnDtlIntno
                     join C in db.studios on B.studioIntno equals C.studioIntno
                     join D in db.dubbingTrnHdrs on C.dubbTrnHdrIntno equals D.dubbTrnHdrIntno
                     join E in db.orderTrnHdrs on A.orderTrnHdrIntno equals E.orderTrnHdrIntno
                     join F in db.agreementWorks on E.workIntno equals F.workIntno
                     where D.dubbTrnHdrIntno == schedule
                     select new { F.workIntno, F.workName, E.episodeNo, E.orderTrnHdrIntno }).Distinct().ToList();

            
            foreach(var x1 in x.Select(b => b.orderTrnHdrIntno).Distinct())
            {
                //endorse episode dubbing
                db.orderTrnHdrs.Find(x1).endDubbing = DateTime.Today.Date;

                //endorse all related assignments
                var dtls = db.orderTrnDtls.Where(b => b.orderTrnHdrIntno == x1 && b.status == true).ToList();
                foreach(var d in dtls)
                {
                    d.status = false;
                }
                db.SaveChanges();
            }

            // insert dubbed episodes log
            int currYear = modelItem.thruDate.Year;
            int currMonth = modelItem.thruDate.Month;

            foreach (var work in x.Select(b => b.workIntno).Distinct())
            {
                long workIntno = work;
                var lg = logModel.FirstOrDefault(b => b.workIntno == workIntno && b.logYear == currYear && b.logMonth == currMonth);
                if (lg == null)
                {
                    var z = db.agreementWorks.Include(b => b.agreement.client).FirstOrDefault(b => b.workIntno == workIntno);
                    logOrder lo = new logOrder();
                    lo.logYear = currYear;
                    lo.logMonth = currMonth;
                    lo.clientIntno = z.agreement.clientIntno;
                    lo.clientName = string.IsNullOrEmpty(z.agreement.client.clientShortName) ? z.agreement.client.clientName : z.agreement.client.clientShortName;
                    lo.workIntno = workIntno;
                    lo.workName = z.workName;
                    lo.workType = LookupModels.decodeDictionaryItem("workType", z.workType);
                    lo.totalEpisodesDubbed = x.Where(b => b.workIntno == workIntno).Count();
                    lo.lastEpisodeDubbed = x.Where(b => b.workIntno == workIntno).Max(b => b.episodeNo);
                    lo.lastUpdate = DateTime.Today;
                    logModel.Add(lo);
                }
                else
                {
                    lg.totalEpisodesDubbed += x.Where(b => b.workIntno == workIntno).Count();
                    lg.lastEpisodeDubbed = x.Where(b => b.workIntno == workIntno).Max(b => b.episodeNo);
                }
            }

            modelItem.status = false;
            db.SaveChanges();
            return RedirectToAction("schedulesList");
        }

        //supervisors
        public ActionResult supervisorsList(long? schedule)
        {
            var x = db.orderTrnDtls.Include(b => b.orderTrnHdr.agreementWork).Where(b => b.orderTrnHdr.agreementWork.status == "01" && b.activityType == "04" && b.status == true);
            var model = x;
            if (schedule.HasValue)
            {
                model = (from A in x
                         join B in db.studioEpisodes on A.orderTrnDtlIntno equals B.orderTrnDtlIntno
                         join C in db.studios on B.studioIntno equals C.studioIntno
                         join D in db.dubbingTrnHdrs on C.dubbTrnHdrIntno equals D.dubbTrnHdrIntno
                         where D.dubbTrnHdrIntno == schedule
                         select A).Include(b => b.employee);
            }
            else //unscheduled
            {
                model = x.Include(b => b.employee)
                    .Where(b => !b.studioEpisodes.Select(s => s.orderTrnDtlIntno).Contains(b.orderTrnDtlIntno));
            }
            return PartialView("_supervisorsList", model.Select(b => b.employee).Distinct().ToList());
        }

        //assignments
        public ActionResult assignmentsList(long? schedule, long? empIntno, string studioNo)
        {
            List<ViewModels.assignmentViewModel> model = new List<ViewModels.assignmentViewModel>();

            var x = db.orderTrnDtls.Include(b => b.orderTrnHdr.agreementWork).Include(b => b.employee).Where(b => b.orderTrnHdr.agreementWork.status == "01" && b.activityType == "04" && b.status == true
                                                && (!empIntno.HasValue || b.empIntno == empIntno));
            var y = x;
            if (schedule.HasValue)
            {
                y = (from A in x
                     join B in db.studioEpisodes on A.orderTrnDtlIntno equals B.orderTrnDtlIntno
                     join C in db.studios on B.studioIntno equals C.studioIntno
                     join D in db.dubbingTrnHdrs on C.dubbTrnHdrIntno equals D.dubbTrnHdrIntno
                     where D.dubbTrnHdrIntno == schedule && (studioNo == null || C.studioNo == studioNo)
                     select A);
            }
            else //unscheduled
            {
                y = x.Where(b => !b.studioEpisodes.Select(s => s.orderTrnDtlIntno).Contains(b.orderTrnDtlIntno));
            }
                
            foreach(var dtl in y)
            {
                ViewModels.assignmentViewModel item = new ViewModels.assignmentViewModel();
                item.orderTrnDtlIntno = dtl.orderTrnDtlIntno;
                item.empIntno = dtl.empIntno;
                item.empName = dtl.employee.fullName;
                item.workIntno = dtl.orderTrnHdr.workIntno;
                item.workName = dtl.orderTrnHdr.agreementWork.workName;
                item.episodeNo = dtl.orderTrnHdr.episodeNo;
                item.dueDate = dtl.forDueDate.HasValue ? dtl.forDueDate.Value.ToString("dd/MM") : DateTime.Today.Date.ToString("dd/MM");
                item.uploadDate = dtl.orderTrnHdr.plannedUpload.HasValue ? dtl.orderTrnHdr.plannedUpload.Value.ToString("dd/MM") : "-";
                item.status = "alert-default";

                if (schedule.HasValue)
                {
                    var std = db.studioEpisodes.Include(b => b.studio.dubbingTrnHdr).FirstOrDefault(b => b.orderTrnDtlIntno == dtl.orderTrnDtlIntno);
                    if (std != null)
                    {
                        item.studioIntno = std.studioIntno;
                        item.studioNo = std.studio.studioNo;
                        item.dubbTrnHdrIntno = std.studio.dubbTrnHdrIntno;
                        DateTime fromDate = std.studio.dubbingTrnHdr.fromDate;
                        DateTime thruDate = std.studio.dubbingTrnHdr.thruDate;
                        item.schedule = fromDate.ToString("dd/MM") + " - " + thruDate.ToString("dd/MM");
                        if (dtl.forDueDate.Value > thruDate)
                            item.status = "alert-danger";
                        if (dtl.forDueDate.Value < fromDate)
                            item.status = "alert-info";
                    }
                }
                model.Add(item);
            }
            ViewBag.schedule = schedule;
            return PartialView("_assignmentsList", model.OrderBy(b => b.dueDate));
        }

        public ActionResult assignmentReschedule(long assignment, long? oldSchedule)
        {
            List<SelectListItem> schList = new List<SelectListItem>();
            var x = db.dubbingTrnHdrs.Where(b => b.status == true);
            foreach (var x1 in x)
            {
                SelectListItem cp = new SelectListItem();
                cp.Value = x1.dubbTrnHdrIntno.ToString();
                cp.Text = x1.fromDate.ToString("dd/MM") + " - " + x1.thruDate.ToString("dd/MM");
                schList.Add(cp);
            }
            SelectList sList = new SelectList(schList, "Value", "Text");
            ViewBag.assignment = assignment;
            ViewBag.schList = sList;
            ViewBag.oldSchedule = oldSchedule;

            List<string> dtlsList = new List<string>();
            var y = db.orderTrnDtls.Include(b => b.orderTrnHdr.agreementWork).Include(b => b.employee)
                    .SingleOrDefault(b => b.orderTrnDtlIntno == assignment);
            dtlsList.Add("Work|" + y.orderTrnHdr.agreementWork.workName);
            dtlsList.Add("Episode|" + y.orderTrnHdr.episodeNo);
            dtlsList.Add("Supervisor|" + y.employee.fullName);
            dtlsList.Add("Due Date|" + y.forDueDate.Value.ToString("dd/MM"));
            var z = db.studioEpisodes.Include(b => b.studio.dubbingTrnHdr)
                    .FirstOrDefault(b => b.orderTrnDtlIntno == assignment && b.studio.workIntno == y.orderTrnHdr.workIntno);
            if (z != null)
                dtlsList.Add("Scheduled|" + z.studio.dubbingTrnHdr.fromDate.ToString("dd/MM") + " - " + z.studio.dubbingTrnHdr.thruDate.ToString("dd/MM"));
            ViewBag.assignmentDetailsList = dtlsList;
            return PartialView("_assignmentReschedule");
        }

        [ValidateAntiForgeryToken]
        [HttpPost, ValidateInput(false)]
        public ActionResult assignmentReschedule(string newSchedule, long assignment, long? oldSchedule, string submit)
        {
            long workIntno = db.orderTrnDtls.Include(b => b.orderTrnHdr).SingleOrDefault(b => b.orderTrnDtlIntno == assignment).orderTrnHdr.workIntno;
            var studioModel = db.studios;
            var episodeModel = db.studioEpisodes;
            var x = episodeModel.FirstOrDefault(b => b.orderTrnDtlIntno == assignment);

            if (submit == "1") //reschedule
            {
                if (!string.IsNullOrEmpty(newSchedule))
                {
                    long schedule = long.Parse(newSchedule);
                    if (x != null)
                    {
                        long x1 = studioModel.FirstOrDefault(b => b.workIntno == workIntno && b.dubbTrnHdrIntno == schedule).studioIntno;
                        x.studioIntno = x1;
                    }
                    else
                    {
                        var y = db.studios.FirstOrDefault(b => b.workIntno == workIntno && b.dubbTrnHdrIntno == schedule);
                        if (y != null)
                        {
                            studioEpisode se = new studioEpisode();
                            se.orderTrnDtlIntno = assignment;
                            se.studioIntno = y.studioIntno;
                            episodeModel.Add(se);
                        }
                        else
                        {
                            studio std = new studio();
                            std.dubbTrnHdrIntno = schedule;
                            std.workIntno = workIntno;
                            studioModel.Add(std);
                            studioEpisode se = new studioEpisode();
                            se.orderTrnDtlIntno = assignment;
                            episodeModel.Add(se);
                        }
                    }
                }
            }
            else // submit="02" remove from schedule
            {
                if(x != null)
                {
                    long y1 = x.studioIntno;
                    int y2 = episodeModel.Where(b => b.studioIntno == y1).Count();
                    if (y2 == 1) //last allocation for studio
                    {
                        studioModel.Remove(studioModel.Find(y1));
                    }
                    episodeModel.Remove(x);
                }
            }
            db.SaveChanges();
            
            return RedirectToAction("assignmentsList", new { schedule = oldSchedule });
        }

        //studios
        public ActionResult studiosList(long? sch)
        {
            var model = db.dubbDomains.Where(b => b.domainName == "studio" && b.langCode == "en" && b.status == true);
            if (sch.HasValue)
                ViewBag.allocatedList = db.studios.Where(b => b.dubbTrnHdrIntno == sch).Select(b => b.studioNo).Distinct().ToList();
            else
                ViewBag.allocatedList = null;
            return PartialView("_studiosList", model.ToList());
        }

        public ActionResult studioAllocation(long studioIntno, long schedule)
        {
            var model = db.studios.Include(b => b.agreementWork).SingleOrDefault(b => b.studioIntno == studioIntno);
            
            SelectList stdList = new SelectList(LookupModels.getDictionary("studio"), "key", "value");
            ViewBag.stdList = stdList;
            ViewBag.schedule = schedule;
            return PartialView("_studioAllocation", model);
        }

        [ValidateAntiForgeryToken]
        [HttpPost, ValidateInput(false)]
        public ActionResult studioAllocation(studio item, long schedule)
        {
            var model = db.studios;
            if (ModelState.IsValid)
            {
                var modelItem = model.Find(item.studioIntno);
                modelItem.studioNo = item.studioNo;
                modelItem.dubbTrnHdrIntno = schedule;
                db.SaveChanges();
            }
            return RedirectToAction("assignmentsList", new { schedule = item.dubbTrnHdrIntno });
        }

        //calendar
        public ActionResult generateCalendar(long schedule)
        {
            var model = db.dubbingAppointments;
            DateTime fromDate = db.dubbingTrnHdrs.Find(schedule).fromDate;
            int totalScenes = 0;
            int totalMinutes = 0;

            //find scheduled episodes
            var episodes = (from A in db.studios
                            join B in db.studioEpisodes on A.studioIntno equals B.studioIntno
                            join C in db.orderTrnDtls on B.orderTrnDtlIntno equals C.orderTrnDtlIntno
                            where A.dubbTrnHdrIntno == schedule
                            select C.orderTrnHdrIntno).Distinct().ToList();
            
            //find dubbing sheets for scheduled episodes
            var x = db.dubbingSheetHdrs.Include(b => b.orderTrnHdr).Where(b => episodes.Contains(b.orderTrnHdrIntno) && !b.actorName.Contains("ANONYM")).ToList();
            
            foreach (var x1 in x.Select(b => new { b.orderTrnHdr.workIntno, b.voiceActorIntno, b.actorName }).Distinct().ToList())
            {
                var hdrs = x.Where(b => b.orderTrnHdr.workIntno == x1.workIntno && b.voiceActorIntno == x1.voiceActorIntno && b.actorName == x1.actorName).Select(b => b.dubbSheetHdrIntno).Distinct().ToList();
                totalScenes = db.dubbingSheetDtls.Where(b => hdrs.Contains(b.dubbSheetHdrIntno)).Count();
                totalMinutes = 0;
                if (x1.voiceActorIntno != 0)
                {
                    var sph = db.workActors.FirstOrDefault(b => b.voiceActorIntno == x1.voiceActorIntno && b.workIntno == x1.workIntno && b.status == true);
                    if (sph != null && sph.scenesPerHour != 0)
                        totalMinutes = totalScenes * 60 / sph.scenesPerHour;
                }

                var std = db.studios.FirstOrDefault(b => b.dubbTrnHdrIntno == schedule && b.workIntno == x1.workIntno);
                if (std != null)
                {
                    var y = db.dubbingAppointments.FirstOrDefault(b => b.voiceActorIntno == x1.voiceActorIntno && b.actorName == x1.actorName && b.studioIntno == std.studioIntno);
                    if (y == null)
                    {
                        dubbingAppointment apt = new dubbingAppointment();
                        apt.voiceActorIntno = x1.voiceActorIntno;
                        apt.actorName = x1.actorName;
                        apt.studioIntno = std.studioIntno;
                        apt.appointmentDate = fromDate;
                        apt.workIntno = x1.workIntno;
                        apt.totalScenes = totalScenes;
                        apt.totalMinutes = totalMinutes;
                        model.Add(apt);
                    }
                    else
                    {
                        y.appointmentDate = fromDate;
                        y.totalScenes = totalScenes;
                        y.totalMinutes = totalMinutes;
                    }
                    db.SaveChanges();
                }
            }
            
            //close discharge and casting
            foreach(long ep in episodes)
            {
                db.orderTrnHdrs.Find(ep).endDischarge = DateTime.Now;
            }
            db.SaveChanges();

            return Content("Calendar Generated / Updated Successfully.", "text/html");
        }

    }
}