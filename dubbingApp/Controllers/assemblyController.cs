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
    [Authorize(Roles = "ADMIN, GENERAL_MANAGER, PRODUCTION_MANAGER, MIXAGE, MONTAGE")]
    public class assemblyController : Controller
    {
        private DUBBDBEntities db = new DUBBDBEntities();
        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }

        // GET: assembly
        public ActionResult Index()
        {
            var x = db.orderTrnHdrs.Include(b => b.agreementWork)
                    .Where(b => b.status != "06").Select(b => new { b.workIntno, b.agreementWork.workName }).Distinct().ToList();
            ViewBag.worksList = new SelectList(x, "workIntno", "workName");
            return View();
        }

        public ActionResult episodesList(long? work, string status, long? ship)
        {
            if (ship.HasValue)
            {
                var model = (from A in db.orderTrnHdrs
                             join B in db.shipmentDetails on A.orderTrnHdrIntno equals B.orderTrnHdrIntno
                             where B.shipmentIntno == ship.Value
                             select A);
                return PartialView("_episodesList", model.Include(b => b.agreementWork).OrderBy(b => new { b.orderReceivedDate, b.workIntno, b.episodeNo }).ToList());
            }
            else
            {
                var x = db.orderTrnHdrs.Include(b => b.agreementWork)
                    .Where(b => (!work.HasValue || b.workIntno == work) && b.status != "06");
                var model = x;

                switch (status)
                {
                    case "01": // in dubbing stage
                        model = x.Where(b => b.startDubbing.HasValue && !b.endDubbing.HasValue);
                        break;
                    case "02": //dubbing endorsed
                        model = x.Where(b => b.endDubbing.HasValue && !b.endMixage.HasValue);
                        break;
                    case "03": // mixage endorsed
                        model = x.Where(b => b.endMixage.HasValue && !b.endMontage.HasValue);
                        break;
                    case "04": // uploaded
                        model = x.Where(b => b.shipmentLowRes.HasValue && !b.shipmentFinal.HasValue);
                        break;
                    case "05": // shipped
                        model = x.Where(b => b.shipmentFinal.HasValue);
                        break;
                    default:
                        model = x.Where(b => !b.startDubbing.HasValue && b.status != "03");
                        break;
                }
                return PartialView("_episodesList", model.OrderBy(b => new { b.orderReceivedDate, b.workIntno, b.episodeNo }).ToList());
            }
        }

        public ActionResult endorseSelectedEpisodes(long? work, string episodes, string activity)
        {
            var model = db.orderTrnHdrs;
            var logModel = db.logOrders;
            string[] episodesList = episodes.Split(';');
            List<orderTrnHdr> oiList = new List<orderTrnHdr>();

            foreach (string ep in episodesList)
            {
                long orderItem = long.Parse(ep);
                var modelItem = model.Find(orderItem);
                oiList.Add(modelItem);
                switch (activity)
                {
                    case "01": // endorse mixage
                        modelItem.endMixage = DateTime.Today.Date;
                        modelItem.startMontage = DateTime.Today.Date;
                        break;
                    case "02": // endorse montage
                        modelItem.endMontage = DateTime.Today.Date;
                        break;
                    case "03": // endorse upload
                        modelItem.shipmentLowRes = DateTime.Today.Date;
                        break;
                }
            }
            if (activity == "03") //insert upload endorsement log
            {
                int currYear = DateTime.Today.Year;
                int currMonth = DateTime.Today.Month;
                foreach (long wk in oiList.Select(b => b.workIntno).Distinct())
                {
                    var lg = logModel.FirstOrDefault(b => b.workIntno == wk && b.logYear == currYear && b.logMonth == currMonth);
                    if (lg == null)
                    {
                        var z = db.agreementWorks.Include(b => b.agreement.client).FirstOrDefault(b => b.workIntno == wk);
                        logOrder lo = new logOrder();
                        lo.logYear = currYear;
                        lo.logMonth = currMonth;
                        lo.clientIntno = z.agreement.clientIntno;
                        lo.clientName = string.IsNullOrEmpty(z.agreement.client.clientShortName) ? z.agreement.client.clientName : z.agreement.client.clientShortName;
                        lo.workIntno = wk;
                        lo.workName = z.workName;
                        lo.workType = LookupModels.decodeDictionaryItem("workType", z.workType);
                        lo.totalEpisodesUploaded = oiList.Where(b => b.workIntno == wk).Count();
                        lo.lastEpisodeDubbed = oiList.Where(b => b.workIntno == wk).Max(b => b.episodeNo);
                        lo.lastUpdate = DateTime.Today;
                        logModel.Add(lo);
                    }
                    else
                    {
                        lg.totalEpisodesUploaded += oiList.Where(b => b.workIntno == wk).Count();
                        lg.lastEpisodeUploaded = oiList.Where(b => b.workIntno == wk).Max(b => b.episodeNo);
                    }
                }
            }
            db.SaveChanges();
            long? work1 = work;
            return RedirectToAction("episodesList", new { work = work1 });
        }

        public ActionResult episodeDetails(long episode)
        {
            ViewBag.episodeNo = db.orderTrnHdrs.Find(episode).episodeNo;
            var x = db.shipmentDetails.Include(b => b.shipment)
                                .FirstOrDefault(b => b.orderTrnHdrIntno == episode && b.shipment.status == true);
            if (x != null)
            {
                ViewBag.shipment = x.shipment.shipmentDate.ToShortDateString();
                ViewBag.dtlIntno = x.shipmentDtlIntno;
            }
            else
            {
                ViewBag.shipment = null;
                ViewBag.dtlIntno = null;
            }
            ViewBag.claimsList = db.clientClaims.Where(b => b.orderTrnHdrIntno == episode).ToList();
            ViewBag.meList = db.orderTrnDtls.Where(b => b.orderTrnHdrIntno == episode && b.activityType == "05").ToList();
            return PartialView("_episodeDetails");
        }

        // shipments
        public ActionResult shipmentsList(bool isPopup)
        {
            var model = db.shipments.Include(b => b.client).Include(b => b.carrier).Where(b => b.status == true)
                        .OrderBy(b => b.shipmentDate);
            ViewBag.isPopup = isPopup;
            return PartialView("_shipmentsList", model.ToList());
        }

        public ActionResult shipmentsAddNew()
        {
            ViewBag.clientsList = new SelectList(db.clients.Where(b => b.status == "01"), "clientIntno", "clientName");
            ViewBag.carriersList = new SelectList(db.carriers.Where(b => b.status == true), "carrierIntno", "carrierName");
            return PartialView("_shipmentsAddNew");
        }

        [ValidateAntiForgeryToken]
        [HttpPost, ValidateInput(false)]
        public ActionResult shipmentsAddNew(shipment item)
        {
            var model = db.shipments;
            if(ModelState.IsValid)
            {
                try
                {
                    item.status = true;
                    model.Add(item);
                    db.SaveChanges();
                }
                catch(Exception e)
                {
                    return Content("Failed! Please Correct All Errors. " + e.Message, "text/html");
                }
            }
            else
                return Content("Failed! Please Enter All Data.", "text/html");

            return Content("Shipment Successfully Created", "text/html");
        }

        public ActionResult shipmentsUpdate(long ship)
        {
            var model = db.shipments.Find(ship);
            ViewBag.carriersList = new SelectList(db.carriers.Where(b => b.status == true), "carrierIntno", "carrierName");
            return PartialView("_shipmentsUpdate", model);
        }

        [ValidateAntiForgeryToken]
        [HttpPost, ValidateInput(false)]
        public ActionResult shipmentsUpdate(shipment item, int updateBtn)
        {
            var model = db.shipments;
            if(ModelState.IsValid)
            {
                try
                {
                    var modelItem = model.Find(item.shipmentIntno);
                    if (updateBtn == 1) //update
                    {
                        this.UpdateModel(modelItem);
                        var x = db.shipmentDetails.Include(b => b.orderTrnHdr).Where(b => b.shipmentIntno == item.shipmentIntno);
                        foreach (var orderItem in x)
                        {
                            orderItem.orderTrnHdr.plannedShipment = item.shipmentDate;
                        }
                    }
                    else if (updateBtn == 2) //endorse shipment
                    {
                        db.shipments.Find(item.shipmentIntno).status = false;
                        var x = db.shipmentDetails.Where(b => b.shipmentIntno == item.shipmentIntno).Select(b => b.orderTrnHdrIntno).Distinct();

                        var logModel = db.logOrders;
                        List<orderTrnHdr> oiList = new List<orderTrnHdr>();

                        foreach (long x1 in x)
                        {
                            var oi = db.orderTrnHdrs.Find(x1);
                            oi.shipmentFinal = DateTime.Today.Date;
                            oiList.Add(oi);
                        }

                        //insert shipment endorsement log
                        int currYear = DateTime.Today.Year;
                        int currMonth = DateTime.Today.Month;
                        foreach (long wk in oiList.Select(b => b.workIntno).Distinct())
                        {
                            var lg = logModel.FirstOrDefault(b => b.workIntno == wk && b.logYear == currYear && b.logMonth == currMonth);
                            if (lg == null)
                            {
                                var z = db.agreementWorks.Include(b => b.agreement.client).FirstOrDefault(b => b.workIntno == wk);
                                logOrder lo = new logOrder();
                                lo.logYear = currYear;
                                lo.logMonth = currMonth;
                                lo.clientIntno = z.agreement.clientIntno;
                                lo.clientName = string.IsNullOrEmpty(z.agreement.client.clientShortName) ? z.agreement.client.clientName : z.agreement.client.clientShortName;
                                lo.workIntno = wk;
                                lo.workName = z.workName;
                                lo.workType = LookupModels.decodeDictionaryItem("workType", z.workType);
                                lo.totalEpisodesShipped = oiList.Where(b => b.workIntno == wk).Count();
                                lo.lastEpisodeShipped = oiList.Where(b => b.workIntno == wk).Max(b => b.episodeNo);
                                lo.lastUpdate = DateTime.Today;
                                logModel.Add(lo);
                            }
                            else
                            {
                                lg.totalEpisodesShipped += oiList.Where(b => b.workIntno == wk).Count();
                                lg.lastEpisodeShipped = oiList.Where(b => b.workIntno == wk).Max(b => b.episodeNo);
                            }
                        }
                        db.SaveChanges();
                    }
                    else //delete shipment and shipment details
                    {
                        var dtlModel = db.shipmentDetails;
                        var x = dtlModel.Where(b => b.shipmentIntno == item.shipmentIntno);
                        db.shipmentDetails.RemoveRange(x);
                        model.Remove(modelItem);
                    }
                    db.SaveChanges();
                }
                catch (Exception e)
                {
                    return Content("Failed! Please Correct All Errors. " + e.Message, "text/html");
                }
            }
            else
                return Content("Failed! Please Enter All Data.", "text/html");

            return Content("Shipment Successfully Updated.", "text/html");
        }

        // shipment Details
        public ActionResult shipmentDetailsAddNew(long ship, string episodes)
        {
            var model = db.shipmentDetails;

            string[] episodesList = episodes.Split(';');
            foreach (string ep1 in episodesList)
            {
                try
                {
                    long ep = long.Parse(ep1);
                    var x = model.Where(b => b.shipmentIntno == ship && b.orderTrnHdrIntno == ep);
                    if (x.Count() == 0)
                    {
                        shipmentDetail dtl = new shipmentDetail();
                        dtl.shipmentIntno = ship;
                        dtl.orderTrnHdrIntno = ep;
                        model.Add(dtl);
                    }
                    db.orderTrnHdrs.Find(ep).plannedShipment = db.shipments.Find(ship).shipmentDate;
                    db.SaveChanges();
                }
                catch (Exception e)
                {
                    return Content("Failed! Please Correct All Errors. " + e.Message, "text/html");
                }
            }
            return Content("Successfully Added Selection to Shipment.", "text/html");
        }

        public ActionResult shipmentDetailsDelete(long id)
        {
            var model = db.shipmentDetails;
            long episode1;
            try
            {
                var modelItem = model.Find(id);
                episode1 = modelItem.orderTrnHdrIntno;
                model.Remove(modelItem);
                db.SaveChanges();
            }
            catch (Exception e)
            {
                return Content("Failed! Unable to Remove Episode from Shipment. " + e.Message, "text/html");
            }
            return RedirectToAction("episodeDetails", new { episode = episode1 });
        }

        // Music & Effects
        public ActionResult musicEffectsAddNew(string episode)
        {
            long ep = long.Parse(episode);
            ViewBag.contractorsList = new SelectList(db.employees.Where(b => b.empType == "05" && b.status == true), "empIntno", "fullName");
            ViewBag.ratingsList = new SelectList(LookupModels.getDictionary("rating"), "Key", "Value");
            ViewBag.episode = ep;
            return PartialView("_musicEffectsAddNew");
        }

        [ValidateAntiForgeryToken]
        [HttpPost,ValidateInput(false)]
        public ActionResult musicEffectsAddNew(orderTrnDtl item, long episode)
        {
            var model = db.orderTrnDtls;
            if(ModelState.IsValid)
            {
                try
                {
                    item.orderTrnHdrIntno = episode;
                    item.activityType = "05";
                    model.Add(item);
                    db.SaveChanges();
                }
                catch (Exception e)
                {
                    return Content("Failed! Please Correct All Errors. " + e.Message, "text/html");
                }
            }
            else
                return Content("Failed! Please Enter All Data.", "text/html");

            return Content("New ME Successfully Added.", "text/html");
        }

        public ActionResult musicEffectsDetails(long id)
        {
            var model = db.orderTrnDtls.Include(b => b.employee).FirstOrDefault(b => b.orderTrnDtlIntno == id);
            model.activityType = LookupModels.decodeDictionaryItem("activityType", model.activityType);
            if (!string.IsNullOrEmpty(model.timeRating))
                model.timeRating = LookupModels.decodeDictionaryItem("rating", model.timeRating);
            if (!string.IsNullOrEmpty(model.qualityRating))
                model.qualityRating = LookupModels.decodeDictionaryItem("rating", model.qualityRating);
            return PartialView("_musicEffectsDetails", model);
        }

        // client claims
        public ActionResult clientClaimsAddNew(string episode)
        {
            long ep = long.Parse(episode);
            ViewBag.claimTypesList = new SelectList(LookupModels.getDictionary("claimType"), "Key", "Value");
            ViewBag.episode = ep;
            return PartialView("_clientClaimsAddNew");
        }

        [ValidateAntiForgeryToken]
        [HttpPost, ValidateInput(false)]
        public ActionResult clientClaimsAddNew(clientClaim item, long episode)
        {
            var model = db.clientClaims;
            if (ModelState.IsValid)
            {
                try
                {
                    item.orderTrnHdrIntno = episode;
                    item.clientIntno = db.orderTrnHdrs.Include(b => b.workOrder).FirstOrDefault(b => b.orderTrnHdrIntno == episode).workOrder.clientIntno;
                    item.status = true;
                    model.Add(item);
                    db.SaveChanges();
                }
                catch (Exception e)
                {
                    return Content("Failed! Please Correct All Errors. " + e.Message, "text/html");
                }
            }
            else
                return Content("Failed! Please Enter All Data.", "text/html");

            return Content("New Claim Successfully Added.", "text/html");
        }

        public ActionResult clientClaimsDetails(long id)
        {
            var model = db.clientClaims.Find(id);
            model.claimType = LookupModels.decodeDictionaryItem("claimType", model.claimType);
            return PartialView("_clientClaimsDetails", model);
        }

        public ActionResult clientClaimsEndorse(long id)
        {
            try
            {
                var model = db.clientClaims.Find(id);
                model.status = false;
                db.SaveChanges();
            }
            catch (Exception e)
            {
                return Content("Failed! Unable to Endorse Claim. " + e.Message, "text/html");
            }
            return Content("Claim Successfully Endorsed.", "text/html");
        }
        
        public ActionResult carriersList()
        {
            var model = db.carriers.Where(b => b.status == true);
            return PartialView("_carriersList", model.ToList());
        }

        public ActionResult carriersAddNew()
        {
            ViewBag.carrierTypesList = new SelectList(LookupModels.getDictionary("carrierType"), "Key", "Value");
            return PartialView("_carriersAddNew");
        }

        [ValidateAntiForgeryToken]
        [HttpPost, ValidateInput(false)]
        public ActionResult carriersAddNew(carrier item)
        {
            var model = db.carriers;
            if(ModelState.IsValid)
            {
                item.status = true;
                model.Add(item);
                db.SaveChanges();
            }
            return Content("Carrier Successfully Added.", "text/html");
        }

        public ActionResult carriersUpdate(long id)
        {
            var model = db.carriers.Find(id);
            ViewBag.carrierTypesList = new SelectList(LookupModels.getDictionary("carrierType"), "Key", "Value");
            return PartialView("_carriersUpdate", model);
        }

        [ValidateAntiForgeryToken]
        [HttpPost, ValidateInput(false)]
        public ActionResult carriersUpdate(carrier item)
        {
            var model = db.carriers;
            if(ModelState.IsValid)
            {
                var modelItem = model.Find(item.carrierIntno);
                UpdateModel(modelItem);
                db.SaveChanges();
            }
            return Content("Carrier Successfully Updated.", "text/html");
        }

        //agreement
        public ActionResult specsAndTermsList(long work, string cat)
        {
            long agr = db.agreementWorks.Find(work).agreementIntno;
            List<string> stList = new List<string>();
            switch(cat)
            {
                case "01": //video specs
                    var x = db.agreementSpecs.Where(b => b.agreementIntno == agr && b.specsType == cat).ToList();
                    foreach(var x1 in x)
                        stList.Add(LookupModels.decodeDictionaryItem("videoSpecsSubtype", x1.specsSubtype) + "|" + x1.specsValue);
                    break;
                case "02":
                    var y = db.agreementSpecs.Where(b => b.agreementIntno == agr && b.specsType == cat).ToList();
                    foreach (var y1 in y)
                        stList.Add(LookupModels.decodeDictionaryItem("videoSpecsSubtype", y1.specsSubtype) + "|" + y1.specsValue);
                    break;
                case "03":
                    var z = db.agreementTerms.Where(b => b.agreementIntno == agr).OrderBy(b => b.sortOrder).ToList();
                    foreach (var z1 in z)
                        stList.Add(z1.termDesc);
                    break;
            }
            ViewBag.cat = cat;
            return PartialView("_specsAndTermsList", stList);
        }
    }
}