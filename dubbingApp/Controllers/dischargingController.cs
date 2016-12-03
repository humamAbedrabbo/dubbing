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
            var model = db.orderTrnHdrs.Include(b => b.agreementWork).Where(b => b.endAdaptation.HasValue && !b.startDubbing.HasValue).OrderBy(b => new { b.workIntno, b.episodeNo });
            return PartialView("_episodesList", model.ToList());
        }

        public ActionResult castingList(long orderItem)
        {
            var sheetHdr = db.dubbingSheetHdrs.Include(b => b.workCharacter).Where(b => b.orderTrnHdrIntno == orderItem).OrderBy(b => b.workCharacter.characterType);
            List<ViewModels.castingListViewModel> model = new List<ViewModels.castingListViewModel>();
            foreach (dubbingSheetHdr hdr in sheetHdr)
            {
                ViewModels.castingListViewModel item = new ViewModels.castingListViewModel();
                item.dubbSheetHdrIntno = hdr.dubbSheetHdrIntno;
                item.orderTrnHdrIntno = hdr.orderTrnHdrIntno;
                item.workCharacterIntno = hdr.workCharacterIntno;
                item.characterName = hdr.characterName;
                item.voiceActorIntno = hdr.voiceActorIntno;
                item.actorName = hdr.actorName;
                item.totalScenes = db.subtitles.Include(b => b.dialog.scene).Where(b => b.dubbSheetHdrIntno == hdr.dubbSheetHdrIntno)
                                    .Select(b => b.dialog.scene.sceneNo).Distinct().Count();
                model.Add(item);
            }

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
            ViewBag.workEpisode = db.agreementWorks.Find(workId).workName + " / Episode: " + orderHdr.episodeNo;

            return PartialView("_castingList", model);
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
            var x = db.subtitles.Include(b => b.dialog).Where(b => b.dubbSheetHdrIntno == sheetHdr).Select(b => b.dialog.sceneIntno).Distinct();
            
            foreach(var item in x)
            {
                var y = db.scenes.Find(item);
                long orderItem = y.orderTrnHdrIntno;
                short sceneNo = y.sceneNo;
                var z = db.dubbingSheetDtls.FirstOrDefault(b => b.dubbSheetHdrIntno == sheetHdr && b.orderTrnHdrIntno == orderItem && b.sceneNo == sceneNo);
                if (z == null)
                {
                    dubbingSheetDtl dtl = new dubbingSheetDtl();
                    dtl.dubbSheetHdrIntno = sheetHdr;
                    dtl.orderTrnHdrIntno = orderItem;
                    dtl.sceneNo = sceneNo;
                    dtl.isTaken = true;
                    dtl.takenTimeStamp = DateTime.Now;
                    model.Add(dtl);
                }
                else
                {
                    z.isTaken = true;
                }
            }
            db.SaveChanges();
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
    }
}