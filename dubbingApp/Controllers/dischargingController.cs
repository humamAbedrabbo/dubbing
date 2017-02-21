using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.Entity;
using System.Web;
using System.Web.Mvc;
using dubbingModel;
using dubbingApp.Models;
using System.Web.UI.WebControls;
using System.IO;
using System.Web.UI;

namespace dubbingApp.Controllers
{
    [Authorize(Roles = "ADMIN, GENERAL_MANAGER, PRODUCTION_MANAGER")]
    public class dischargingController : Controller
    {
        private DUBBDBEntities db = new DUBBDBEntities();
        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }

        // GET: discharging
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult episodesList()
        {
            var x = db.orderTrnHdrs.Include(b => b.agreementWork).Where(b => b.endAdaptation.HasValue && !b.endDubbing.HasValue).OrderBy(b => new { b.workIntno, b.episodeNo });
            List<ViewModels.dischargeEpisodeViewModel> model = new List<ViewModels.dischargeEpisodeViewModel>();

            foreach(var x1 in x)
            {
                ViewModels.dischargeEpisodeViewModel item = new ViewModels.dischargeEpisodeViewModel();
                item.orderTrnHdrIntno = x1.orderTrnHdrIntno;
                item.workName = x1.agreementWork.workName;
                item.episodeNo = x1.episodeNo;

                long orderItem = x1.orderTrnHdrIntno;
                var sch = (from A in db.orderTrnDtls
                           join B in db.studioEpisodes on A.orderTrnDtlIntno equals B.orderTrnDtlIntno
                           join C in db.studios on B.studioIntno equals C.studioIntno
                           join D in db.dubbingTrnHdrs on C.dubbTrnHdrIntno equals D.dubbTrnHdrIntno
                           where A.orderTrnHdrIntno == orderItem && A.status == true && D.status == true
                           select new { D.fromDate, D.thruDate }).Distinct().ToList();
                if (sch.Count() != 0)
                {
                    item.isScheduled = true;
                    var sch1 = sch.FirstOrDefault();
                    if (DateTime.Today.Date >= sch1.fromDate && DateTime.Today.Date <= sch1.thruDate)
                        item.isThisWeek = true;
                    else
                        item.isThisWeek = false;
                }
                else
                {
                    item.isScheduled = false;
                    item.isThisWeek = false;
                }
                model.Add(item);
            }
            
            return PartialView("_episodesList", model);
        }

        public ActionResult castingList(long orderItem)
        {
            var sheetHdr = db.dubbingSheetHdrs.Include(b => b.workCharacter).Where(b => b.orderTrnHdrIntno == orderItem)
                            .Select(b => new { b, scenesCount = b.dubbingSheetDtls.Count(), takenCount = b.dubbingSheetDtls.Where(c => c.isTaken == true).Count() })
                            .OrderBy(b => b.b.workCharacter.characterType);
            List<ViewModels.castingListViewModel> model = new List<ViewModels.castingListViewModel>();
            foreach (var hdr in sheetHdr)
            {
                ViewModels.castingListViewModel item = new ViewModels.castingListViewModel();
                item.dubbSheetHdrIntno = hdr.b.dubbSheetHdrIntno;
                item.orderTrnHdrIntno = hdr.b.orderTrnHdrIntno;
                item.workCharacterIntno = hdr.b.workCharacterIntno;
                item.characterName = hdr.b.characterName;
                item.voiceActorIntno = hdr.b.voiceActorIntno;
                item.actorName = hdr.b.actorName;
                item.totalScenes = hdr.scenesCount;

                if (hdr.scenesCount != 0 && hdr.takenCount == hdr.scenesCount)
                    item.isEndorsed = true;
                else
                    item.isEndorsed = false;

                model.Add(item);
            }

            var orderHdr = db.orderTrnHdrs.Find(orderItem);
            long workId = orderHdr.workIntno;

            int sch = (from A in db.orderTrnDtls
                       join B in db.studioEpisodes on A.orderTrnDtlIntno equals B.orderTrnDtlIntno
                       join C in db.studios on B.studioIntno equals C.studioIntno
                       join D in db.dubbingTrnHdrs on C.dubbTrnHdrIntno equals D.dubbTrnHdrIntno
                       where A.orderTrnHdrIntno == orderItem && A.status == true && D.status == true
                       select A).Distinct().Count();
            if (sch != 0)
                ViewBag.isScheduled = true;
            else
                ViewBag.isScheduled = false;

            ViewBag.orderItem = orderItem;
            ViewBag.workEpisode = db.agreementWorks.Find(workId).workName + " / Episode: " + orderHdr.episodeNo;

            return PartialView("_castingList", model);
        }

        public ActionResult castingUpdate(long id)
        {
            var hdr = db.dubbingSheetHdrs.Find(id);
            ViewModels.castingListViewModel item = new ViewModels.castingListViewModel();
            item.dubbSheetHdrIntno = hdr.dubbSheetHdrIntno;
            item.orderTrnHdrIntno = hdr.orderTrnHdrIntno;
            item.workCharacterIntno = hdr.workCharacterIntno;
            item.characterName = hdr.characterName;
            item.voiceActorIntno = hdr.voiceActorIntno;
            item.actorName = hdr.actorName;
            item.totalScenes = hdr.dubbingSheetDtls.Count();

            long orderItem = hdr.orderTrnHdrIntno;
            var orderHdr = db.orderTrnHdrs.Find(orderItem);
            long workId = orderHdr.workIntno;
            var x = (from A in db.voiceActors
                     join B in db.workActors on A.voiceActorIntno equals B.voiceActorIntno
                     where B.workIntno == workId && A.status == true
                     select new { A.voiceActorIntno, A.fullName })
                     .Union(from C in db.voiceActors
                            where C.voiceActorIntno == 0
                            select new { C.voiceActorIntno, C.fullName });
            ViewBag.actorsList = new SelectList(x, "voiceActorIntno", "fullName");
            ViewBag.orderItem = orderItem;
            return PartialView("_castingUpdate", item);
        }

        [ValidateAntiForgeryToken]
        [HttpPost, ValidateInput(false)]
        public ActionResult castingUpdate(ViewModels.castingListViewModel item)
        {
            var model = db.dubbingSheetHdrs;
            if (ModelState.IsValid)
            {
                var modelItem = model.FirstOrDefault(b => b.dubbSheetHdrIntno == item.dubbSheetHdrIntno);
                modelItem.voiceActorIntno = item.voiceActorIntno;
                if (item.voiceActorIntno != 0)
                    modelItem.actorName = db.voiceActors.Find(item.voiceActorIntno).fullName;
                else
                    modelItem.actorName = item.actorName;
                db.SaveChanges();
                return Content("Successfully Updated.", "text/html");
            }
            else
                return Content("Failed to Update! Please Correct All Errors.", "text/html");
        }

        public ActionResult refreshCast(long orderItem)
        {
            var model = db.dubbingSheetHdrs;
            var x = (from A in db.dubbingSheetHdrs
                     join B in db.workActors on A.workCharacterIntno equals B.workCharacterIntno
                     where A.orderTrnHdrIntno == orderItem && A.voiceActorIntno == 0
                     select new { A.dubbSheetHdrIntno, B.voiceActorIntno });
            foreach (var item in x)
            {
                var modelItem = model.Find(item.dubbSheetHdrIntno);
                modelItem.voiceActorIntno = item.voiceActorIntno;
            }
            db.SaveChanges();

            long oi = orderItem;
            return RedirectToAction("castingList", new { orderItem = oi });
        }

        public ActionResult endorseDubbing(long sheetHdr)
        {
            var model = db.dubbingSheetDtls;
            var x = db.subtitles.Include(b => b.dialog).Where(b => b.dubbSheetHdrIntno == sheetHdr).Select(b => b.dialog.sceneIntno).Distinct().ToList();

            long orderItem = db.dubbingSheetHdrs.Find(sheetHdr).orderTrnHdrIntno;
            var orderHdr = db.orderTrnHdrs.FirstOrDefault(b => b.orderTrnHdrIntno == orderItem);

            // find if it is the first in episode dubbing
            var dtls = db.dubbingSheetDtls.Where(b => b.orderTrnHdrIntno == orderItem && b.isTaken == true).ToList();
            if (dtls.Count() == 0)
            {
                orderHdr.startDubbing = DateTime.Today.Date;
                db.SaveChanges();
            }

            foreach (var item in x)
            {
                var y = db.scenes.Find(item);
                short sceneNo = y.sceneNo;
                var z = db.dubbingSheetDtls.FirstOrDefault(b => b.dubbSheetHdrIntno == sheetHdr && b.sceneNo == sceneNo);
                if (z == null)
                {
                    dubbingSheetDtl dtl = new dubbingSheetDtl();
                    dtl.dubbSheetHdrIntno = sheetHdr;
                    dtl.orderTrnHdrIntno = orderItem;
                    dtl.sceneNo = sceneNo;
                    dtl.isTaken = true;
                    dtl.dubbingDate = DateTime.Today.Date;
                    dtl.takenTimeStamp = DateTime.Now;
                    model.Add(dtl);
                }
                else
                {
                    z.isTaken = true;
                }
            }
            db.SaveChanges();

            // find if it is the last in episode dubbing
            dtls = db.dubbingSheetDtls.Where(b => b.orderTrnHdrIntno == orderItem && b.isTaken == false).ToList();
            if (dtls.Count() == 0)
            {
                orderHdr.endDubbing = DateTime.Today.Date;
                db.SaveChanges();
            }

            return Content("Dubbing Endorsed Successfully.", "text/html");
        }

        public ActionResult exportDischargingTable(long orderItem)
        {
            var model = db.subtitles.Include(b => b.dialog.scene.orderTrnHdr).Include(b => b.dubbingSheetHdr)
                        .Where(b => b.dialog.scene.orderTrnHdrIntno == orderItem);
                        
            var x = db.orderTrnHdrs.Include(b => b.agreementWork).FirstOrDefault(b => b.orderTrnHdrIntno == orderItem);
            ViewBag.episode = x.agreementWork.workName + " / Episode " + x.episodeNo;
            return PartialView("_exportDischargingTable", model.ToList());
        }

        public ActionResult scheduleDubbing(long id)
        {
            var orderItem = db.orderTrnHdrs.Find(id);
            DateTime? startDate;
            if (orderItem.startDubbing.HasValue)
                startDate = orderItem.startDubbing.Value;
            else if (orderItem.plannedUpload.HasValue)
                startDate = orderItem.plannedUpload.Value.AddDays(-7);
            else if (orderItem.plannedShipment.HasValue)
                startDate = orderItem.plannedShipment.Value.AddDays(-14);
            else
                startDate = null;

            if (startDate == null)
                return Content("Unable to Schedule the given Episode! None of Dubbing/Upload/Shipment Scheduling date is given", "text/html");
            else
            {
                //update order item with dubbing start date
                orderItem.startDubbing = startDate.Value;
                orderItem.endDischarge = startDate;
                db.SaveChanges();

                //identify supervisor(s) and insert assignments if not provided
                var orderDtlsModel = db.orderTrnDtls;
                var emp = db.workPersonnels.FirstOrDefault(b => b.workIntno == orderItem.workIntno && b.status == true);
                var assignments = orderDtlsModel.Where(b => b.orderTrnHdrIntno == id && b.activityType == "04" && b.status == true);
                var empAssign = assignments;
                if (emp != null && assignments.Count() != 0)
                    empAssign.FirstOrDefault(b => b.empIntno == emp.empIntno);
                else
                    empAssign.FirstOrDefault(b => b.empIntno == -10);

                if ((assignments.Count() == 0 && emp != null) || (empAssign == null && emp != null))
                {
                    orderTrnDtl dtl = new orderTrnDtl();
                    dtl.orderTrnHdrIntno = id;
                    dtl.activityType = "04";
                    dtl.empIntno = emp.empIntno;
                    dtl.status = true;
                    orderDtlsModel.Add(dtl);
                    db.SaveChanges();
                }
                else
                    return Content("Unable to Schedule the given Episode! No Supervisor Assigned to Perform the Given Discharge Table.", "text/html");

                assignments = orderDtlsModel.Where(b => b.orderTrnHdrIntno == id && b.activityType == "04" && b.status == true);

                //identify or create new schedule
                string fdw = LookupModels.decodeDictionaryItem("settings", "fdw");
                while (startDate.Value.DayOfWeek.ToString() != fdw)
                {
                    startDate.Value.AddDays(-1);
                }

                var sch = db.dubbingTrnHdrs.FirstOrDefault(b => b.fromDate == startDate.Value && b.status == true);
                if(sch == null)
                {
                    var schHdrModel = db.dubbingTrnHdrs;
                    dubbingTrnHdr schHdr = new dubbingTrnHdr();
                    schHdr.fromDate = startDate.Value;
                    schHdr.thruDate = startDate.Value.AddDays(7);
                    schHdr.isPaid = false;
                    schHdr.status = true;
                    schHdrModel.Add(schHdr);

                    var stdModel = db.studios;
                    studio std = new studio();
                    std.workIntno = orderItem.workIntno;
                    std.studioNo = "01";
                    stdModel.Add(std);

                    var stdDtlsModel = db.studioEpisodes;
                    foreach(var x in assignments)
                    {
                        studioEpisode stdDtl = new studioEpisode();
                        stdDtl.orderTrnDtlIntno = x.orderTrnDtlIntno;
                        stdDtlsModel.Add(stdDtl);
                    }
                    db.SaveChanges();
                }
                else
                {
                    var stdHdr = db.studios.FirstOrDefault(b => b.dubbTrnHdrIntno == sch.dubbTrnHdrIntno && b.workIntno == orderItem.workIntno);
                    if (stdHdr == null)
                    {
                        var stdModel = db.studios;
                        studio std = new studio();
                        std.dubbTrnHdrIntno = sch.dubbTrnHdrIntno;
                        std.workIntno = orderItem.workIntno;
                        std.studioNo = "01";
                        stdModel.Add(std);

                        var stdDtlsModel = db.studioEpisodes;
                        foreach (var x in assignments)
                        {
                            studioEpisode stdDtl = new studioEpisode();
                            stdDtl.orderTrnDtlIntno = x.orderTrnDtlIntno;
                            stdDtlsModel.Add(stdDtl);
                        }
                        db.SaveChanges();
                    }
                    else
                    {
                        var stdDtlsModel = db.studioEpisodes;
                        foreach (var x in assignments)
                        {
                            var stdDtl = db.studioEpisodes.FirstOrDefault(b => b.studioIntno == stdHdr.studioIntno && b.orderTrnDtlIntno == x.orderTrnDtlIntno);
                            studioEpisode stdDtlItem = new studioEpisode();
                            stdDtlItem.studioIntno = stdHdr.studioIntno;
                            stdDtlItem.orderTrnDtlIntno = x.orderTrnDtlIntno;
                            stdDtlsModel.Add(stdDtlItem);
                        }
                        db.SaveChanges();
                    }
                }
                return Content ("Schedule Successfully Generated for the given Discharge Table.", "text/html");
            }
        }
    }
}