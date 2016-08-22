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
                }
            }
            var model1 = db.dubbingTrnHdrs.Where(b => b.status == true).OrderBy(b => b.fromDate);
            return PartialView("_schedulesList", model1.ToList());
        }

        public ActionResult schedulesDelete(long schedule)
        {
            var model = db.dubbingTrnHdrs;
            var modelItem = model.Find(schedule);
            modelItem.status = false;
            db.SaveChanges();
            return RedirectToAction("schedulesList");
        }

        public ActionResult schedulesEndorse(long schedule)
        {
            var model = db.dubbingTrnHdrs;
            var logModel = db.logOrders;
            var modelItem = model.Find(schedule);
            modelItem.status = false;

            // insert dubbed episodes log
            var x = db.dubbingTrnDtls.Where(b => b.dubbTrnHdrIntno == schedule).ToList();
            var y = x.Select(b => b.workIntno).Distinct().ToList();
            
            int currYear = modelItem.thruDate.Year;
            int currMonth = modelItem.thruDate.Month;

            foreach(var work in y)
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
            db.SaveChanges();
            return RedirectToAction("schedulesList");
        }

        public ActionResult scheduleDetails(long sch)
        {
            var hdr = db.dubbingTrnHdrs.Find(sch);
            ViewBag.scheduleIntno = sch;
            ViewBag.schedule = hdr.fromDate.ToString("dd/MM") + " - " + hdr.thruDate.ToString("dd/MM");

            List<string> detailsList = new List<string>();
            var x = db.studioEpisodes.Include(b => b.studio).Include(b => b.dubbingTrnDtl)
                    .Where(b => b.studio.dubbTrnHdrIntno == sch && b.status == true)
                    .Select(b => new { b.studio.workIntno, b.studio.studioNo, b.studioIntno, b.dubbTrnDtlIntno }).ToList();
            foreach (var x1 in x.Select(b => new { b.workIntno, b.studioNo, b.studioIntno }).Distinct().OrderBy(b => b.studioNo))
            {
                long x2 = x1.workIntno;
                long x3 = x1.studioIntno;
                string work = db.agreementWorks.Find(x2).workName;
                int cnt = x.Where(b => b.workIntno == x2 && b.studioIntno == x3).Count();
                detailsList.Add(x1.studioNo + "|" + work + "|" + cnt);
            }
            return PartialView("_scheduleDetails", detailsList);
        }

        //works and episodes
        public ActionResult episodesList(long sch)
        {
            List<ViewModels.scheduleViewModel> model = new List<ViewModels.scheduleViewModel>();

            var schedule = db.dubbingTrnHdrs.Find(sch);
            var worksList = db.orderTrnHdrs.Include(b => b.agreementWork)
                            .Where(b => !b.startDubbing.HasValue && b.status == "04")
                            .Select(b => new { b.workIntno, b.agreementWork.workName, b.agreementWork.totalWeekNbrEpisodes }).Distinct().ToList();

            for (int i = 0; i < worksList.Count(); i++)
            {
                ViewModels.scheduleViewModel modelItem = new ViewModels.scheduleViewModel();
                modelItem.workIntno = worksList[i].workIntno;
                modelItem.workName = worksList[i].workName;
                modelItem.episodesPerWeek = worksList[i].totalWeekNbrEpisodes;
                modelItem.dubbTrnHdrIntno = sch;
                modelItem.fromDate = schedule.fromDate;
                modelItem.thruDate = schedule.thruDate;

                var s = db.dubbingTrnDtls.Where(b => b.dubbTrnHdrIntno == sch && b.workIntno == modelItem.workIntno);
                if (s.Count() == 0)
                {
                    modelItem.isScheduled = false;
                }
                else
                {
                    modelItem.isScheduled = true;
                    var orderItemsList = db.dubbingTrnDtls.Include(b => b.orderTrnHdr)
                                    .Where(b => b.dubbTrnHdrIntno == sch && b.workIntno == modelItem.workIntno).OrderBy(b => b.episodeNo);
                    List<ViewModels.episodeItem> epList = new List<ViewModels.episodeItem>();
                    foreach (var oi in orderItemsList)
                    {
                        ViewModels.episodeItem ep = new ViewModels.episodeItem();
                        ep.episode = oi;
                        ep.status = oi.orderTrnHdr.endAdaptation.HasValue ? "02" : "01"; // "01" planned, "02" ready
                        epList.Add(ep);
                    }
                    modelItem.episodesList = epList;

                    var x = db.studioEpisodes.Include(b => b.studio)
                            .Where(b => b.studio.dubbTrnHdrIntno == sch && b.studio.workIntno == modelItem.workIntno)
                            .OrderBy(b => new { b.studioIntno, b.dubbingTrnDtl.episodeNo, b.status });
                    List<ViewModels.studioEpisodeItem> studiosList = new List<ViewModels.studioEpisodeItem>();
                    foreach (var stdEp in x)
                    {
                        ViewModels.studioEpisodeItem std = new ViewModels.studioEpisodeItem();
                        std.studioEpisodeIntno = stdEp.studioEpisodeIntno;
                        std.episodeNo = stdEp.dubbingTrnDtl.episodeNo;
                        std.studioNo = stdEp.studio.studioNo;
                        std.status = stdEp.status;
                        studiosList.Add(std);
                    }
                    modelItem.studioEpisodesList = studiosList;
                }
                model.Add(modelItem);
            }
            
            return PartialView("_episodesList", model.OrderByDescending(b => b.isScheduled));
        }

        public ActionResult episodesAddNew(long work, long schedule)
        {
            var hdr = db.dubbingTrnHdrs.Find(schedule);
            List<string> episodesList = new List<string>();
            // find planned episodes suitable for the dubbing schedule
            var x1 = db.dubbingTrnDtls.Where(b => b.dubbTrnHdrIntno == schedule).Select(b => b.orderTrnHdrIntno).ToList();
            var x = db.orderTrnHdrs.Where(b => b.workIntno == work && !b.startDubbing.HasValue 
                && b.plannedUpload.HasValue && !x1.Contains(b.orderTrnHdrIntno) && b.status == "04")
                .OrderBy(b => b.episodeNo);
            string episode;
            foreach (orderTrnHdr item in x)
            {
                episode = item.orderTrnHdrIntno + "|" + item.episodeNo + "|" + LookupModels.decodeDictionaryItem("priority", item.priority) + "|" 
                        + item.plannedUpload.Value.ToString("dd/MM") + "|" + item.endAdaptation.HasValue;
                episodesList.Add(episode);
            }
            
            ViewBag.work = work;
            ViewBag.workName = db.agreementWorks.Find(work).workName;
            ViewBag.schedule = schedule;
            return PartialView("_episodesAddNew", episodesList);
        }

        public ActionResult loadEpisodesToSchedule(long work, long schedule, string episodes)
        {
            var model = db.dubbingTrnDtls;
            var studioEpisodesModel = db.studioEpisodes;
            string[] episodesList = episodes.Split(';');
            for (int i = 0; i < episodesList.Count(); i++)
            {
                dubbingTrnDtl dtl = new dubbingTrnDtl();
                long orderItem = long.Parse(episodesList[i]);
                dtl.orderTrnHdrIntno = orderItem;
                dtl.dubbTrnHdrIntno = schedule;
                dtl.workIntno = work;
                dtl.episodeNo = db.orderTrnHdrs.Find(orderItem).episodeNo;
                model.Add(dtl);

                // create set of studios allocation for the episode if already the studios is allocated for the parent work
                var x = db.studios.Where(b => b.dubbTrnHdrIntno == schedule && b.workIntno == work && b.status == true).ToList();
                for (int j = 0; j < x.Count(); j++)
                {
                    studioEpisode std = new studioEpisode();
                    std.studioIntno = x[j].studioIntno;
                    std.status = true;
                    studioEpisodesModel.Add(std);
                }
                db.SaveChanges();
            }

            long work1 = work;
            long schedule1 = schedule;
            return RedirectToAction("episodesAddNew", new { work = work1, schedule = schedule1 });
        }

        public ActionResult episodesDelete(long id)
        {
            var model = db.dubbingTrnDtls;
            var modelItem = model.Find(id);
            long sch1 = modelItem.dubbTrnHdrIntno;
            long work = modelItem.workIntno;
            long dtl = modelItem.dubbTrnDtlIntno;

            var studioModel = db.studios;
            var studioEpisodesModel = db.studioEpisodes;

            try
            {
                var x = db.studioEpisodes.Where(b => b.dubbTrnDtlIntno == dtl).ToList();
                if (x.Count() != 0)
                {
                    foreach (var x1 in x)
                    {
                        long std = x1.studioIntno;
                        studioEpisodesModel.Remove(x1);
                        if (db.studioEpisodes.Where(b => b.studioIntno == std).Count() == 0)
                            studioModel.Remove(db.studios.Find(std));
                    }
                }
                model.Remove(modelItem);
                db.SaveChanges();
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
            var model = db.dubbDomains.Where(b => b.domainName == "studio" && b.langCode == "en" && b.status == true);
            ViewBag.allocatedList = db.studios.Where(b => b.dubbTrnHdrIntno == sch && b.status == true)
                                    .Select(b => b.studioNo).Distinct().ToList();
            ViewBag.schedule = sch;
            return PartialView("_studiosList", model.ToList());
        }

        //studio allocations
        public ActionResult studioAllocationList(long schedule, string studio)
        {
            var model = db.studios.Include(b => b.agreementWork).Include(b => b.employee)
                        .Where(b => b.dubbTrnHdrIntno == schedule && b.studioNo == studio && b.status == true)
                        .OrderBy(b => b.workIntno);
            ViewBag.studio = studio;
            ViewBag.schedule = schedule;
            return PartialView("_studioAllocationList", model.ToList());
        }

        public ActionResult studioAllocationAddNew(long schedule, string studio)
        {
            var x = db.employees.Where(b => b.empType == "01" && b.status == true).Select(b => new { b.empIntno, b.fullName });
            ViewBag.studioTeamList = new SelectList(x.ToList(), "empIntno", "fullName");

            var y = db.dubbingTrnDtls.Include(b => b.agreementWork).Where(b => b.dubbTrnHdrIntno == schedule)
                    .Select(b => new { b.workIntno, b.agreementWork.workName }).Distinct();
            ViewBag.worksList = new SelectList(y.ToList(), "workIntno", "workName");
            ViewBag.studio = studio;
            ViewBag.schedule = schedule;
            return PartialView("_studioAllocationAddNew");
        }

        [ValidateAntiForgeryToken]
        [HttpPost, ValidateInput(false)]
        public ActionResult studioAllocationAddNew(studio item, long schedule, string studioNo)
        {
            var model = db.studios;
            var x = db.studios.Where(b => b.dubbTrnHdrIntno == schedule && b.workIntno == item.workIntno && b.studioNo == studioNo && b.supervisor == item.supervisor && b.status == true).ToList();
            if (x.Count() == 0)
            {
                item.dubbTrnHdrIntno = schedule;
                item.status = true;
                model.Add(item);
                
                var studioEpisodesModel = db.studioEpisodes;
                var y = db.dubbingTrnDtls.Where(b => b.dubbTrnHdrIntno == schedule && b.workIntno == item.workIntno).Select(b => b.dubbTrnDtlIntno);
                foreach(long ep in y)
                {
                    studioEpisode ep1 = new studioEpisode();
                    ep1.dubbTrnDtlIntno = ep;
                    ep1.status = true;
                    studioEpisodesModel.Add(ep1);
                }
                db.SaveChanges();
            }
            
            long schedule1 = schedule;
            return RedirectToAction("studioAllocationList", new { schedule = schedule1, studio = studioNo });
        }

        public ActionResult studioAllocationDelete(long id)
        {
            var model = db.studios;
            var modelItem = model.Find(id);
            string studio1 = modelItem.studioNo;
            long schedule1 = modelItem.dubbTrnHdrIntno;

            model.Remove(modelItem);

            var epModel = db.studioEpisodes;
            var x = epModel.Where(b => b.studioIntno == id);
            epModel.RemoveRange(x);

            db.SaveChanges();

            return RedirectToAction("studioAllocationList", new { schedule = schedule1, studio = studio1 });
        }

        public ActionResult studioEpisodeAllocationToggle(long id)
        {
            var model = db.studioEpisodes;
            var modelItem = model.Find(id);
            long schedule = modelItem.studio.dubbTrnHdrIntno;
            long std = modelItem.studioIntno;

            int cnt;
            var studioModel = db.studios;
            var studioModelItem = studioModel.Find(std);

            if (modelItem.status)
            {
                cnt = model.Where(b => b.studioIntno == std && b.status == true).Count();
                modelItem.status = false;
                if (cnt == 1) //if this is the last allocation to remove then disable the parent studio as well
                    studioModelItem.status = false;
            }
            else
            {
                cnt = model.Where(b => b.studioIntno == std && b.status == true).Count();
                modelItem.status = true;
                if (cnt == 0) //if this is the first allocation to enable then enable the parent studio as well
                    studioModelItem.status = true;
            }

            db.SaveChanges();
            return RedirectToAction("episodesList", new { sch = schedule });
        }

        public ActionResult getDefaultStudioTeam(long work, long schedule, string role)
        {
            var x = db.workPersonnels.Include(b => b.employee).Where(b => b.workIntno == work && b.titleType == role).ToList();
            string team = null;
            for (int i = 0; i < x.Count(); i++)
            {
                long emp = x[i].empIntno;
                var y = db.studios.FirstOrDefault(b => b.dubbTrnHdrIntno == schedule && b.workIntno == work
                        && (role == "02" && b.supervisor == emp) || (role == "03" && b.sound == emp));
                if (i == 0)
                    team = x[i].employee.fullName;
                else
                    team = team + "; " + x[i].employee.fullName;
                if (y != null)
                    team = team + " (used)";
            }
            return Content(team, "text/html");
        }
    }
}