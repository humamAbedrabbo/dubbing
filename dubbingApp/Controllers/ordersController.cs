using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;
using System.Web.Mvc;
using dubbingModel;
using dubbingApp.Models;

namespace dubbingApp.Controllers
{
    [Authorize(Roles = "ADMIN, GENERAL_MANAGER, PRODUCTION_MANAGER, MONTAGE")]
    public class ordersController : Controller
    {
        private DUBBDBEntities db = new DUBBDBEntities();
        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }

        // GET: orders
        public ActionResult Index()
        {
            var worksList = db.agreementWorks.Where(b => b.status == "01").Select(b => new { b.workIntno, b.workName });
            ViewBag.worksList = new SelectList(worksList, "workIntno", "workName");
            return View();
        }

        public ActionResult tips()
        {
            return PartialView("_tips");
        }

        public ActionResult orderItemsList(long? work, string stage, int? fromEpisode, int? thruEpisode)
        {
            int fEpisode = fromEpisode.HasValue ? fromEpisode.Value : 1;
            int tEpisode = thruEpisode.HasValue ? thruEpisode.Value : 100000;
            var x = db.orderTrnHdrs.Include(b => b.workOrder).Include(b => b.agreementWork)
                                    .Where(b => (!work.HasValue || b.workIntno == work) 
                                            && b.episodeNo >= fEpisode
                                            && b.episodeNo <= tEpisode 
                                            );
            var model = x;
            if (!work.HasValue)// means new orders from all works
                stage = "01";

            switch (stage)
            {
                case "01": //New
                    model = x.Where(b => !b.startAdaptation.HasValue && b.status != "06");
                    break;
                case "02": //completed adaptation & ready for dubbing
                    model = x.Where(b => b.endAdaptation.HasValue && !b.startDubbing.HasValue && b.status != "06");
                    break;
                case "03": //completed dubbing & ready for upload
                    model = x.Where(b => b.endDubbing.HasValue && !b.shipmentLowRes.HasValue && b.status != "06");
                    break;
                case "04": //completed upload & ready for shipment
                    model = x.Where(b => b.shipmentLowRes.HasValue && !b.shipmentFinal.HasValue && b.status != "06");
                    break;
                case "05": //Archive: endorsed orders
                    model = x.Where(b => b.status == "06");
                    break;
                default:
                    model = x.Where(b => b.status != "06");
                    break;
            }

            return PartialView("_orderItemsList", model.OrderBy(b => new { b.workIntno, b.episodeNo }).ToList());
        }

        // new order
        public ActionResult orderAddNew(long? client)
        {
            ViewBag.clientsList = new SelectList(db.clients.Where(b => (!client.HasValue || b.clientIntno == client) && b.status == "01"), "clientIntno", "clientName");
            ViewBag.worksList = new SelectList(db.agreementWorks.Where(b => (!client.HasValue || b.agreement.clientIntno == client) && b.status == "01").Select(b => new { b.workIntno, b.workName }), "workIntno", "workName");
            return PartialView("_orderAddNew");
        }

        public JsonResult clientComboChanged(long? client)
        {
            var x = db.agreementWorks.Where(b => (!client.HasValue || b.agreement.clientIntno == client) && b.status == "01").Select(b => new { b.workIntno, b.workName });
            return Json(new SelectList(x, "workIntno", "workName"));
        }

        [ValidateAntiForgeryToken]
        [HttpPost, ValidateInput(false)]
        public ActionResult orderAddNew(workOrder item)
        {
            var model = db.workOrders;
            var orderItemsModel = db.orderTrnHdrs;
            if (ModelState.IsValid)
            {
                try
                {
                    item.allowFirstDubbing = false;
                    item.priority = "02";
                    item.lastUpdated = DateTime.Now;
                    item.updatedBy = LookupModels.getUser();
                    item.status = "01";

                    var x = db.agreementWorks.Where(b => b.workIntno == item.workIntno).SingleOrDefault();

                    if (x.firstEpisodeShowDate.HasValue)
                    {
                        // calculate expected delivery date
                        int orderFirstEpisode = item.fromEpisode;
                        int nbrWeeklyEpisodes = x.totalWeekNbrEpisodes;
                        double nbrDays = (orderFirstEpisode / nbrWeeklyEpisodes) * 7;
                        item.expectedDeliveryDate = x.firstEpisodeShowDate.Value.AddDays(nbrDays).Date;
                    }
                    model.Add(item);

                    // because it is about new received episodes, then insert order items
                    for (int i = item.fromEpisode; i <= item.thruEpisode; i++)
                    {
                        orderTrnHdr oi = new orderTrnHdr();
                        oi.workIntno = item.workIntno;
                        oi.episodeNo = (short)i;
                        oi.allowFirstDubbing = item.allowFirstDubbing;
                        oi.priority = item.priority;
                        oi.orderReceivedDate = item.receivedDate;
                        if (item.expectedDeliveryDate.HasValue)
                            oi.expectedDeliveryDate = item.expectedDeliveryDate.Value.AddDays(i);
                        oi.status = "01";
                        orderItemsModel.Add(oi);
                    }
                    db.SaveChanges();
                }
                catch (Exception e)
                {
                    return Content("Failed to Create! Please Correct All Errors. " + e.Message, "text/html");
                }
            }
            else
                return Content("Failed to Create! Please Correct All Errors. ", "text/html");
            return Content("Successfully Created. ", "text/html");
        }

        // order quality check
        public ActionResult orderItemIssuesList(long id)
        {
            var model = db.orderChecks.Where(b => b.orderTrnHdrIntno == id);
            ViewBag.orderItem = id;
            return PartialView("_orderItemIssuesList", model.ToList());
        }

        public ActionResult orderItemIssueAddNew(long id)
        {
            ViewBag.orderItem = id;
            ViewBag.checkTypesList = LookupModels.getDictionary("checkType");
            return PartialView("_orderItemIssueAddNew");
        }

        [ValidateAntiForgeryToken]
        [HttpPost, ValidateInput(false)]
        public ActionResult orderItemIssueAddNew(orderCheck item, long id)
        {
            var model = db.orderChecks;
            if (ModelState.IsValid)
            {
                try
                {
                    item.orderTrnHdrIntno = id;
                    if (!item.isAccepted) // if the item is of rejected quality, change the status of the order item to rejected
                    {
                        var orderItem = db.orderTrnHdrs.Find(id);
                        orderItem.status = "03";
                    }
                    model.Add(item);
                    db.SaveChanges();
                }
                catch (Exception e)
                {
                    return Content("Failed to Add! Please Correct All Errors. " + e.Message, "text/html");
                }
            }
            else
                return Content("Failed to Add! Please Correct All Errors. ", "text/html");

            return Content("Successfully Added. ", "text/html");
        }

        public ActionResult orderItemIssueDelete(long id)
        {
            var model = db.orderChecks;
            var modelItem = model.Find(id);
            long orderTrnHdrIntno = modelItem.orderTrnHdrIntno;
            model.Remove(modelItem);
            db.SaveChanges();

            // check if the deleted issue was in rejected status, then update the order item status to active if no more reject issues
            var x = model.Where(b => b.orderTrnHdrIntno == orderTrnHdrIntno && b.isAccepted == false).ToList();
            var y = db.orderTrnHdrs.Find(orderTrnHdrIntno);
            if (y.status == "03" && x.Count == 0)
            {
                y.status = "04";
                db.SaveChanges();
            }
            var model1 = db.orderChecks.Where(b => b.orderTrnHdrIntno == orderTrnHdrIntno);
            return PartialView("_orderItemIssuesList", model1.ToList());
        }

        // planning dubbing, upload and shipment
        public ActionResult orderItemSelectList(long id)
        {
            long work = db.orderTrnHdrs.Find(id).workIntno;
            ViewBag.adaptorsList = new SelectList(db.workPersonnels.Include(b => b.employee)
                                .Where(b => b.workIntno == work && (b.titleType == "04" || b.titleType == "06") && b.status == true)
                                .Select(b => new { b.employee.empIntno, b.employee.fullName }), "empIntno", "fullName");
            ViewBag.translatorsList = new SelectList(db.workPersonnels.Include(b => b.employee)
                                .Where(b => b.workIntno == work && (b.titleType == "04" || b.titleType == "05") && b.status == true)
                                .Select(b => new { b.employee.empIntno, b.employee.fullName }), "empIntno", "fullName");
            return PartialView("_orderItemSelectList");
        }

        [ValidateAntiForgeryToken]
        [HttpPost, ValidateInput(false)]
        public ActionResult scheduleAddNew(string oiListHF, DateTime? scheduleDate, string scheduleTypeHF, long? adaptor, long? translator)
        {
            if (scheduleDate.HasValue)
            {
                var model = db.orderTrnHdrs;
                var model1 = db.orderTrnDtls;
                string[] oiList = oiListHF.Split(';');

                // input data validation
                var z = LookupModels.decodeDictionaryItem("settings", "fdw").ToUpper();
                DateTime dt = scheduleDate.Value;
                if (scheduleTypeHF == "01" && dt.DayOfWeek.ToString().ToUpper() != z)
                    return Content("The Date for Scheduling MUST Start on " + z + ". ", "text/html");
                if (scheduleTypeHF == "04" || scheduleTypeHF == "05") //adaptation & translation
                {
                    if ((scheduleTypeHF == "04" && !adaptor.HasValue) || (scheduleTypeHF == "05" && !translator.HasValue))
                        return Content("Please Provide the Assigned Personnel. ", "text/html");
                    long work = db.orderTrnHdrs.Find(long.Parse(oiList[0])).workIntno;
                    for (int i = 1; i < oiList.Count(); i++)
                    {
                        long tmp = db.orderTrnHdrs.Find(long.Parse(oiList[i])).workIntno;
                        if (tmp != work)
                            return Content("Failed! Selected Episodes Belong to Different Works. Please Correct Selection. ", "text/html");
                    }
                }
                
                foreach(string item in oiList)
                {
                    long id = long.Parse(item);
                    switch (scheduleTypeHF)
                    {
                        case "01":
                            model.Find(id).plannedDubbing = scheduleDate.Value;
                            break;
                        case "02":
                            model.Find(id).plannedUpload = scheduleDate.Value;
                            break;
                        case "03":
                            model.Find(id).plannedShipment = scheduleDate.Value;
                            break;
                        case "04":
                            model.Find(id).startAdaptation = scheduleDate.Value;
                            orderTrnDtl dtl = new orderTrnDtl();
                            dtl.orderTrnHdrIntno = id;
                            dtl.activityType = "02";
                            dtl.empIntno = adaptor.Value;
                            model1.Add(dtl);
                            break;
                        case "05":
                            model.Find(id).endTranslation = scheduleDate.Value;
                            orderTrnDtl dtl2 = new orderTrnDtl();
                            dtl2.orderTrnHdrIntno = id;
                            dtl2.activityType = "01";
                            dtl2.empIntno = translator.Value;
                            model1.Add(dtl2);
                            break;
                    }
                }
                db.SaveChanges();
            }
            else
                return Content("Please Select a Date for Scheduling. ", "text/html");
            return Content("Successfully Recorded / Scheduled. ", "text/html");
        }

        // order item details
        public ActionResult orderItemDetails(long id)
        {
            List<string> oiDetailsList = new List<string>();
            var x = db.orderTrnHdrs.Find(id);
            long y = 0;

            oiDetailsList.Add("Work|" + x.agreementWork.workName);
            oiDetailsList.Add("Episode|" + x.episodeNo);
            oiDetailsList.Add("Received On|" + x.orderReceivedDate.Value.ToShortDateString());
            oiDetailsList.Add("Order No|" + x.workOrder.orderIntno);
            oiDetailsList.Add("Priority|" + LookupModels.decodeDictionaryItem("priority", x.priority));
            oiDetailsList.Add("Status|" + LookupModels.decodeDictionaryItem("orderItemStatus", x.status));
            oiDetailsList.Add(null);
            oiDetailsList.Add("Translation Started|" + (x.startTranslation.HasValue ? x.startTranslation.Value.ToShortDateString() : null));
            oiDetailsList.Add("Translation Ended|" + (x.endTranslation.HasValue ? x.endTranslation.Value.ToShortDateString() : null));
            if (x.endTranslation.HasValue && x.orderTrnDtls.FirstOrDefault(b => b.activityType == "01") != null)
            {
                y = x.orderTrnDtls.FirstOrDefault(b => b.activityType == "01").empIntno;
                oiDetailsList.Add("Translation By|" + db.employees.FirstOrDefault(e => e.empIntno == y).fullName);
            }
            else
                oiDetailsList.Add("Translation By|");
            oiDetailsList.Add(null);
            oiDetailsList.Add("Adaptation Started|" + (x.startAdaptation.HasValue ? x.startAdaptation.Value.ToShortDateString() : null));
            oiDetailsList.Add("Adaptation Ended|" + (x.endAdaptation.HasValue ? x.endAdaptation.Value.ToShortDateString() : null));
            if (x.startAdaptation.HasValue && x.orderTrnDtls.FirstOrDefault(b => b.activityType == "02") != null)
            {
                y = x.orderTrnDtls.FirstOrDefault(b => b.activityType == "02").empIntno;
                oiDetailsList.Add("Adaptation By|" + db.employees.FirstOrDefault(e => e.empIntno == y).fullName);
            }
            else
                oiDetailsList.Add("Adaptation By|");
            oiDetailsList.Add(null);
            oiDetailsList.Add("Dubbing Started|" + (x.startDubbing.HasValue ? x.startDubbing.Value.ToShortDateString() : null));
            oiDetailsList.Add("Dubbing Ended|" + (x.endDubbing.HasValue ? x.endDubbing.Value.ToShortDateString() : null));
            oiDetailsList.Add("Dubbing By|");
            oiDetailsList.Add(null);
            oiDetailsList.Add("Mixage Started|" + (x.startMixage.HasValue ? x.startMixage.Value.ToShortDateString() : null));
            oiDetailsList.Add("Mixage Ended|" + (x.endMixage.HasValue ? x.endMixage.Value.ToShortDateString() : null));
            oiDetailsList.Add("Mixage By|");
            oiDetailsList.Add(null);
            oiDetailsList.Add("Montage Started|" + (x.startMontage.HasValue ? x.startMontage.Value.ToShortDateString() : null));
            oiDetailsList.Add("Montage Ended|" + (x.endMontage.HasValue ? x.endMontage.Value.ToShortDateString() : null));
            oiDetailsList.Add("Montage By|");
            oiDetailsList.Add(null);
            oiDetailsList.Add("Uploaded On|" + (x.shipmentLowRes.HasValue ? x.shipmentLowRes.Value.ToShortDateString() : null));
            oiDetailsList.Add("Shipped On|" + (x.shipmentFinal.HasValue ? x.shipmentFinal.Value.ToShortDateString() : null));

            ViewBag.orderTrnHdrIntno = id;
            return PartialView("_orderItemDetails", oiDetailsList);
        }

        public ActionResult orderItemDetailsUpdate(long id, string op)
        {
            var model = db.orderTrnHdrs.Find(id);
            switch(op)
            {
                case "A": // active status
                    model.status = "04"; 
                    break;
                case "R": // cancel status
                    model.status = "05"; 
                    break;
                case "P": // increase priority
                    if (model.priority != "03")
                        model.priority = "0" + (int.Parse(model.priority) + 1);
                    break;
                case "D": // decrease priority
                    if (model.priority != "01")
                        model.priority = "0" + (int.Parse(model.priority) - 1);
                    break;
            }
            db.SaveChanges();
            return RedirectToAction("orderItemDetails", new { id = model.orderTrnHdrIntno });
        }

        // client complains
        public ActionResult orderItemComplainsList(long id)
        {
            var model = db.clientClaims.Where(b => b.orderTrnHdrIntno == id);
            ViewBag.orderItem = id;
            return PartialView("_orderItemComplainsList", model.ToList());
        }

        public ActionResult orderItemComplainAddNew(long id)
        {
            ViewBag.orderItem = id;
            ViewBag.claimTypesList = LookupModels.getDictionary("claimType");
            return PartialView("_orderItemComplainAddNew");
        }

        [ValidateAntiForgeryToken]
        [HttpPost, ValidateInput(false)]
        public ActionResult orderItemComplainAddNew(clientClaim item, long id)
        {
            var model = db.clientClaims;
            if (ModelState.IsValid)
            {
                try
                {
                    item.orderTrnHdrIntno = id;
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
            return Content("Successfully Created.", "text/html");
        }

        public ActionResult orderItemComplainEndorse(long id)
        {
            var model = db.clientClaims;
            var modelItem = model.Find(id);
            ViewBag.orderItem = modelItem.orderTrnHdrIntno;
            modelItem.status = false;
            db.SaveChanges();
            return PartialView("_orderItemComplainsList", model.Where(b => b.orderTrnHdrIntno == modelItem.orderTrnHdrIntno).ToList());
        }
    }
}