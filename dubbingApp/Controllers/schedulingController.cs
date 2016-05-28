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
    public class schedulingController : Controller
    {
        private DUBBDBEntities db = new DUBBDBEntities();
        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }

        // GET: scheduling
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult schedulesList()
        {
            var model = db.dubbingTrnHdrs.Where(b => b.status == true).OrderBy(b => b.fromDate);
            return PartialView("_schedulesList", model.ToList());
        }

        public ActionResult scheduleDetails(long sch)
        {
            var hdr = db.dubbingTrnHdrs.Find(sch);
            ViewBag.scheduleIntno = sch;
            ViewBag.schedule = hdr.fromDate.ToString("dd/MM") + " - " + hdr.thruDate.ToString("dd/MM");

            List<string> detailsList = new List<string>();
            // find planned episodes suitable for the dubbing schedule
            var x = db.orderTrnHdrs.Include(b => b.agreementWork).Where(b => !b.startDubbing.HasValue && !b.endAdaptation.HasValue && b.plannedDubbing.HasValue && b.status == "04").ToList();
            detailsList.Add("Planned|" + x.Count() + " Episodes");
            int cnt = 0;
            int last = 0;
            foreach (orderTrnHdr item in x)
            {
                var x1 = db.dubbingTrnDtls.FirstOrDefault(b => b.orderTrnHdrIntno == item.orderTrnHdrIntno);
                if (x1 == null && item.plannedDubbing >= hdr.fromDate && item.plannedDubbing <= hdr.thruDate)
                {
                    cnt++;
                    detailsList.Add(item.agreementWork.workName + "|" + item.episodeNo);
                }
            }
            if (cnt == 0)
            {
                detailsList[0] = "Planned|None";
                last = 1;
            }
            else
            {
                last = cnt + 1;
                cnt = 0;
            }
            
            
            //find episodes with finalized adaptation
            var y = db.orderTrnHdrs.Include(b => b.agreementWork).Where(b => !b.startDubbing.HasValue && b.endAdaptation.HasValue && b.status == "04").ToList();
            detailsList.Add("xxx|");
            detailsList.Add("Finalized Adaptation|" + y.Count() + " Episodes");
            foreach (orderTrnHdr item in y)
            {
                var y1 = db.dubbingTrnDtls.FirstOrDefault(b => b.orderTrnHdrIntno == item.orderTrnHdrIntno);
                if (y1 == null)
                {
                    cnt++;
                    detailsList.Add(item.agreementWork.workName + "|" + item.episodeNo);
                }
            }
            if (cnt == 0)
                detailsList[last + 1] = "Finalized Adaptation|None";

            return PartialView("_scheduleDetails", detailsList);
        }

        public ActionResult scheduleChange(long sch, string op)
        {
            var model = db.dubbingTrnDtls;
            var hdr = db.dubbingTrnHdrs.Find(sch);
            switch(op)
            {
                case "01": //reload planned episodes for dubbing
                    var x = db.orderTrnHdrs.Where(b => !b.startDubbing.HasValue && !b.endAdaptation.HasValue && b.plannedDubbing.HasValue && b.status == "04").ToList();
                    foreach (orderTrnHdr item in x)
                    {
                        var x1 = db.dubbingTrnDtls.FirstOrDefault(b => b.orderTrnHdrIntno == item.orderTrnHdrIntno && b.dubbTrnHdrIntno == sch);
                        if (x1 == null && item.plannedDubbing >= hdr.fromDate && item.plannedDubbing <= hdr.thruDate)
                        {
                            dubbingTrnDtl dtl = new dubbingTrnDtl();
                            dtl.dubbTrnHdrIntno = sch;
                            dtl.workIntno = item.workIntno;
                            dtl.orderTrnHdrIntno = item.orderTrnHdrIntno;
                            model.Add(dtl);
                        }
                    }
                    db.SaveChanges();
                    break;
                case "02": //reload episodes with adaptation ready
                    var y = db.orderTrnHdrs.Where(b => !b.startDubbing.HasValue && b.endAdaptation.HasValue && b.status == "04").ToList();
                    foreach (orderTrnHdr item in y)
                    {
                        var y1 = db.dubbingTrnDtls.FirstOrDefault(b => b.orderTrnHdrIntno == item.orderTrnHdrIntno && b.dubbTrnHdrIntno == sch);
                        if (y1 == null)
                        {
                            dubbingTrnDtl dtl = new dubbingTrnDtl();
                            dtl.dubbTrnHdrIntno = sch;
                            dtl.workIntno = item.workIntno;
                            dtl.orderTrnHdrIntno = item.orderTrnHdrIntno;
                            model.Add(dtl);
                        }
                    }
                    db.SaveChanges();
                    break;
                case "03": //delete schedule
                    hdr.status = false;
                    db.SaveChanges();
                    break;
            }

            if (op != "03")
            {
                long sch1 = sch;
                return RedirectToAction("episodesList", new { sch = sch1 });
            }
            else
            {
                return RedirectToAction("schedulesList");
            }
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
            else
            {
                var x = db.dubbingTrnHdrs.Where(b => b.fromDate == item.fromDate && b.status == true).ToList();
                if (x.Count() != 0)
                    return new HttpStatusCodeResult(500, "Error message");
                else
                {
                    item.thruDate = item.fromDate.AddDays(6);
                    item.isPaid = false;
                    item.status = true;
                    model.Add(item);
                    db.SaveChanges();

                    var studioModel = db.studios;
                    var studioList = db.dubbDomains.Where(b => b.domainName == "studio").OrderBy(b => b.sortOrder)
                                    .Select(b => new { b.domainCode, b.sortOrder }).ToList();
                    for (int i = 0; i < studioList.Count(); i++)
                    {
                        studio std = new studio();
                        std.dubbTrnHdrIntno = item.dubbTrnHdrIntno;
                        std.studioNo = studioList[i].domainCode;
                        std.srl = studioList[i].sortOrder;
                        std.status = true;
                        studioModel.Add(std);
                    }
                    db.SaveChanges();
                }
            }
            var model1 = db.dubbingTrnHdrs.Where(b => b.status == true).OrderBy(b => b.fromDate);
            return PartialView("_schedulesList", model1.ToList());
        }
        
        //episodes
        public ActionResult episodesList(long sch)
        {
            var model = db.dubbingTrnDtls.Include(b => b.agreementWork).Include(b => b.dubbingTrnHdr).Include(b => b.orderTrnHdr)
                        .Where(b => b.dubbTrnHdrIntno == sch).OrderBy(b => new { b.workIntno, b.orderTrnHdr.episodeNo });
            var x = db.studioDtls.Include(b => b.studio).Where(b => b.studio.dubbTrnHdrIntno == sch && b.studio.status == true)
                    .Select(b => new { b.workIntno, b.studio.studioNo });
            ViewBag.studios = x;
            ViewBag.schedule = sch;
            return PartialView("_episodesList", model.ToList());
        }

        public ActionResult episodesDelete(long id)
        {
            var model = db.dubbingTrnDtls;
            var modelItem = model.Find(id);
            long sch1 = modelItem.dubbTrnHdrIntno;
            long work = modelItem.workIntno;
            try
            {
                model.Remove(modelItem);
                db.SaveChanges();

                //delete all studio allocations when no more episodes of the same work are included in the dubbingTrnDtl
                if (model.Where(b => b.workIntno == work && b.dubbTrnHdrIntno == sch1).Count() == 0)
                {
                    var x = db.studioDtls.Include(b => b.studio).Where(b => b.workIntno == work && b.studio.dubbTrnHdrIntno == sch1);
                    db.studioDtls.RemoveRange(x);
                    db.SaveChanges();
                }
            }
            catch
            {
                return new HttpStatusCodeResult(500, "Error message");
            }
            return RedirectToAction("episodesList", new { sch = sch1 });
        }

        //studios
        public ActionResult studiosList(long sch)
        {
            var model = db.studios.Where(b => b.dubbTrnHdrIntno == sch && b.status == true);
            return PartialView("_studiosList", model.ToList());
        }

        public ActionResult studiosUpdate(long id)
        {
            var model = db.studios.Find(id);
            ViewBag.teamList = new SelectList(db.employees.Where(b => b.empType == "01" && b.status == true), "empIntno", "fullName");
            ViewBag.studioWorksList = db.studioDtls.Include(b => b.agreementWork).Where(b => b.studioIntno == id).ToList();
            ViewBag.schedule = model.dubbTrnHdrIntno;
            return PartialView("_studiosUpdate", model);
        }

        [ValidateAntiForgeryToken]
        [HttpPost,ValidateInput(false)]
        public ActionResult studiosUpdate(studio item)
        {
            var model = db.studios.Find(item.studioIntno);
            model.supervisor = item.supervisor;
            model.sound = item.sound;
            db.SaveChanges();
            return RedirectToAction("studiosList", new { sch = item.dubbTrnHdrIntno });
        }

        [HttpGet]
        public ActionResult allocateWorkToStudio(long work, long sch)
        {
            ViewBag.work = work + "|" + db.agreementWorks.Find(work).workName;
            long? supervisorIntno = null;
            var x = db.workPersonnels.FirstOrDefault(b => b.workIntno == work && b.titleType == "02" && b.status == true); //work supervisors
            if (x != null)
            {
                supervisorIntno = x.empIntno;
                ViewBag.studio = db.employees.Find(supervisorIntno.Value).fullName + "|" + db.studios.FirstOrDefault(b => b.supervisor == supervisorIntno.Value && b.status == true).studioNo;

            }
            else
                ViewBag.studio = null;
            var z = db.studios.Include(b => b.employee)
                    .Where(b => b.dubbTrnHdrIntno == sch && (!supervisorIntno.HasValue || b.supervisor != supervisorIntno.Value) && b.status == true)
                    .Select(b => new { b.studioIntno, studioName = ("Studio " + b.studioNo + " / " + b.employee.fullName) });
            ViewBag.studiosList = new SelectList(z, "studioIntno", "studioName");
            ViewBag.schedule = sch;
            return PartialView("_allocateWorkToStudio");
        }

        [ValidateAntiForgeryToken]
        [HttpPost, ValidateInput(false)]
        public ActionResult allocateWorkToStudio(studioDtl item, string work, string sch)
        {
            var model = db.studioDtls;
            long workIntno = long.Parse(work);
            long schedule = long.Parse(sch);
            if (ModelState.IsValid)
            {
                var x = db.studioDtls.Where(b => b.studioIntno == item.studioIntno && b.workIntno == workIntno).ToList();
                if (x.Count() == 0)
                {
                    try
                    {
                        item.workIntno = workIntno;
                        model.Add(item);
                        db.SaveChanges();
                    }
                    catch
                    {
                        return new HttpStatusCodeResult(500, "Error message");
                    }
                }
                else
                    return new HttpStatusCodeResult(500, "Already Allocated to the Same Studio.");
            }
            else
            {
                var x = db.workPersonnels.FirstOrDefault(b => b.workIntno == workIntno && b.titleType == "02" && b.status == true);
                if (x != null)
                {
                    long x1 = db.studios.FirstOrDefault(b => b.dubbTrnHdrIntno == schedule && b.supervisor == x.empIntno).studioIntno;
                    var y = db.studioDtls.FirstOrDefault(b => b.studioIntno == x1 && b.workIntno == workIntno);
                    if (y == null)
                    {
                        try
                        {
                            studioDtl dtl = new studioDtl();
                            dtl.workIntno = workIntno;
                            dtl.studioIntno = x1;
                            dtl.isDefault = true;
                            
                            model.Add(dtl);
                            db.SaveChanges();
                        }
                        catch
                        {
                            return new HttpStatusCodeResult(500, "Error message");
                        }
                    }
                    else
                        return new HttpStatusCodeResult(500, "Already Allocated to the Same Studio.");
                }
                else
                    return new HttpStatusCodeResult(500, "Error message");
            }

            return RedirectToAction("episodesList", new { sch = schedule });
        }

        public ActionResult workAllocationDelete(long id)
        {
            var model = db.studioDtls;
            var modelItem = model.Find(id);
            long std = modelItem.studioIntno;
            model.Remove(modelItem);

            var aptModel = db.dubbingAppointments;
            var aptList = aptModel.Include(b => b.studio.studioDtls).Where(b => b.studio.studioDtls.FirstOrDefault(t => t.studioDtlsIntno == id).studioDtlsIntno == id).ToList();
            if (aptList.Count() != 0)
                return new HttpStatusCodeResult(500, "Failed! Unable to Cancel/Delete The Selected Schedule. Appointments are Already Given to Actors. Deleting This Schedule WILL Require to Manually Delete the Given Appointments and then Retry Again.");
            else
            {
                db.SaveChanges();
                return RedirectToAction("studiosUpdate", new { id = std });
            }
        }

        public ActionResult workCalendar(long work, long schedule)
        {
            var x = (from A in db.dubbingAppointments
                     join B in db.studios on A.studioIntno equals B.studioIntno
                     join C in db.studioDtls on B.studioIntno equals C.studioIntno
                     where C.workIntno == work && B.dubbTrnHdrIntno == schedule && B.status == true && B.status == true
                     select new { B.studioIntno, B.studioNo }).Distinct().ToList();
            ViewBag.studiosList = x;
            ViewBag.work = work;
            ViewBag.schedule = schedule;
            return PartialView("_workCalendar");
        }

        public ActionResult generateCalendar(long work, long schedule, long? studio)
        {
            var model = db.dubbingAppointments;
            DateTime fromDate = db.dubbingTrnHdrs.Find(schedule).fromDate;
            var x = (from A in db.dubbingTrnDtls
                     join B in db.orderTrnHdrs on A.orderTrnHdrIntno equals B.orderTrnHdrIntno
                     join C in db.dubbingSheetHdrs on B.orderTrnHdrIntno equals C.orderTrnHdrIntno
                     where A.dubbTrnHdrIntno == schedule && A.workIntno == work
                     select new { C.voiceActorIntno, totalScenes = C.dubbingSheetDtls.Count() }).Distinct().ToList();

            if (studio.HasValue) // this is a reload of an already generated calendar for the given studio
            {
                for (int i = 0; i < x.Count(); i++)
                {
                    long actor = x[i].voiceActorIntno;
                    var y = db.dubbingAppointments.Where(b => b.voiceActorIntno == actor && b.studioIntno == studio.Value).ToList();
                    if (y.Count() == 0)
                    {
                        dubbingAppointment apt = new dubbingAppointment();
                        apt.voiceActorIntno = actor;
                        apt.studioIntno = studio.Value;
                        apt.appointmentDate = fromDate;
                        apt.workIntno = work;
                        apt.totalScenes = x[i].totalScenes;
                        var sph = db.workActors.FirstOrDefault(b => b.voiceActorIntno == actor && b.workIntno == work && b.status == true);
                        if (sph != null && sph.scenesPerHour != 0)
                            apt.totalMinutes = x[i].totalScenes * 60 / sph.scenesPerHour;
                        else
                            apt.totalMinutes = 0;
                        model.Add(apt);
                    }
                }
                db.SaveChanges();
            }
            else // this is a generate of new calendars for the allocated studios
            {
                var z = (from A in db.studios
                         join B in db.studioDtls on A.studioIntno equals B.studioIntno
                         where A.dubbTrnHdrIntno == schedule && B.workIntno == work
                         select A.studioIntno).Distinct().ToList();
                foreach (long std in z)
                {
                    for (int i = 0; i < x.Count(); i++)
                    {
                        long actor = x[i].voiceActorIntno;
                        dubbingAppointment apt = new dubbingAppointment();
                        apt.voiceActorIntno = actor;
                        apt.studioIntno = std;
                        apt.appointmentDate = fromDate;
                        apt.workIntno = work;
                        apt.totalScenes = x[i].totalScenes;
                        var sph = db.workActors.FirstOrDefault(b => b.voiceActorIntno == actor && b.workIntno == work && b.status == true);
                        if (sph != null && sph.scenesPerHour != 0)
                            apt.totalMinutes = x[i].totalScenes * 60 / sph.scenesPerHour;
                        else
                            apt.totalMinutes = 0;
                        model.Add(apt);
                    }
                }
                db.SaveChanges();
            }
            long work1 = work;
            long schedule1 = schedule;
            return RedirectToAction("workCalendar", new { work = work1, schedule = schedule1 });
        }
    }
}