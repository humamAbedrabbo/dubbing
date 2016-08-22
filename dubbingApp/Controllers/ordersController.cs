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

        public ActionResult orderItemsList(long? work, string epFilter, int? fromEpisode, int? thruEpisode)
        {
            var x = db.orderTrnHdrs.Include(b => b.agreementWork)
                                    .Where(b => b.agreementWork.status == "01"
                                            && (!work.HasValue || b.workIntno == work)
                                            && (!fromEpisode.HasValue || b.episodeNo >= fromEpisode)
                                            && (!thruEpisode.HasValue || b.episodeNo <= thruEpisode)
                                            );
            var model = x;
            DateTime todayDate = DateTime.Today.Date;
            switch (epFilter)
            {
                case "01": //New
                    model = x.Where(b => !b.startAdaptation.HasValue && b.status != "06");
                    break;
                case "02": //delayed adaptation
                    model = x.Where(b => b.startAdaptation.HasValue && !b.endAdaptation.HasValue && !b.startDubbing.HasValue
                            && b.startDischarge.HasValue && b.startDischarge.Value < todayDate && b.status != "06");
                    break;
                case "03": //delayed dubbing
                    model = x.Where(b => b.startDubbing.HasValue && (!b.endDubbing.HasValue || !b.endMixage.HasValue || !b.endMontage.HasValue)
                            && b.plannedUpload.HasValue && b.plannedUpload.Value < todayDate && b.status != "06");
                    break;
                case "04": //delayed upload
                    model = x.Where(b => b.plannedUpload.HasValue && b.shipmentLowRes.HasValue && !b.shipmentFinal.HasValue
                            && b.plannedUpload.Value < todayDate && b.status != "06");
                    break;
                case "05": //delayed shipment
                    model = x.Where(b => b.plannedShipment.HasValue && b.shipmentLowRes.HasValue && !b.shipmentFinal.HasValue
                            && b.plannedShipment.Value < todayDate && b.status != "06");
                    break;
                case "06": //Archive: endorsed orders
                    model = x.Where(b => b.status == "06");
                    break;
                case "07": //episodes with received quality issues
                    model = (from A in x
                             join B in db.orderChecks on A.orderTrnHdrIntno equals B.orderTrnHdrIntno
                             select A);
                    break;
                case "08": //episodes having client claims open
                    model = (from A in x
                             join B in db.clientClaims on A.orderTrnHdrIntno equals B.orderTrnHdrIntno
                             where B.status == true
                             select A);
                    break;
                default:
                    model = x.Where(b => b.status != "06");
                    break;
            }

            return PartialView("_orderItemsList", model.OrderBy(b => new { b.workIntno, b.episodeNo }).ToList());
        }

        // filter settings
        public ActionResult filterSettings(long? work, string epFilter)
        {
            string settingsList = null;
            var x = db.orderTrnHdrs.Include(b => b.agreementWork)
                                    .Where(b => b.agreementWork.status == "01"
                                            && (!work.HasValue || b.workIntno == work)
                                            && b.status != "06").ToList();
            int cnt = 0;
            DateTime todayDate = DateTime.Today.Date;

            if (epFilter == "01") // for new episodes only where all of the counts are zero except received quality
            {
                cnt = (from A in x
                       join B in db.orderChecks on A.orderTrnHdrIntno equals B.orderTrnHdrIntno
                       where !A.startAdaptation.HasValue
                       select A).Count();
                settingsList = "0;0;0;0;" + cnt.ToString() + ";0";
            }
            else
            {
                // delayed adaptation
                cnt = x.Where(b => b.startAdaptation.HasValue && !b.endAdaptation.HasValue && !b.startDubbing.HasValue
                                && b.startDischarge.HasValue && b.startDischarge.Value < todayDate).Count();
                settingsList = cnt.ToString() + ";";

                // delayed dubbing
                cnt = x.Where(b => b.startDubbing.HasValue && (!b.endDubbing.HasValue || !b.endMixage.HasValue || !b.endMontage.HasValue)
                                && b.plannedUpload.HasValue && b.plannedUpload.Value < todayDate).Count();
                settingsList = settingsList + cnt.ToString() + ";";

                // delayed upload
                cnt = x.Where(b => b.plannedUpload.HasValue && b.shipmentLowRes.HasValue && !b.shipmentFinal.HasValue
                                && b.plannedUpload.Value < todayDate).Count();
                settingsList = settingsList + cnt.ToString() + ";";

                // delayed shipment
                cnt = x.Where(b => b.plannedShipment.HasValue && b.shipmentLowRes.HasValue && !b.shipmentFinal.HasValue
                                && b.plannedShipment.Value < todayDate).Count();
                settingsList = settingsList + cnt.ToString() + ";";

                // received quality issues
                cnt = (from A in x
                       join B in db.orderChecks on A.orderTrnHdrIntno equals B.orderTrnHdrIntno
                       select A).Count();
                settingsList = settingsList + cnt.ToString() + ";";

                // client claims
                cnt = (from A in x
                       join B in db.clientClaims on A.orderTrnHdrIntno equals B.orderTrnHdrIntno
                       where B.status == true
                       select A).Count();
                settingsList = settingsList + cnt.ToString();
            }

            ViewBag.settingsList = settingsList;
            return PartialView("_filters");
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
            var logModel = db.logOrders;
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

                    var x = db.agreementWorks.Include(b => b.agreement.client).Where(b => b.workIntno == item.workIntno).SingleOrDefault();

                    if (x.firstEpisodeShowDate.HasValue)
                    {
                        // calculate expected delivery date
                        int orderFirstEpisode = item.fromEpisode;
                        int nbrWeeklyEpisodes = x.totalWeekNbrEpisodes;
                        double nbrDays = (orderFirstEpisode / nbrWeeklyEpisodes) * 7;
                        item.expectedDeliveryDate = x.firstEpisodeShowDate.Value.AddDays(nbrDays).Date;
                    }
                    model.Add(item);

                    // log the newly received order
                    var lg = db.logOrders.FirstOrDefault(b => b.logYear == item.receivedDate.Year && b.logMonth == item.receivedDate.Month && b.workIntno == item.workIntno);
                    if (lg == null)
                    {
                        logOrder lo = new logOrder();
                        lo.logYear = item.receivedDate.Year;
                        lo.logMonth = item.receivedDate.Month;
                        lo.clientIntno = x.agreement.clientIntno;
                        lo.clientName = string.IsNullOrEmpty(x.agreement.client.clientShortName) ? x.agreement.client.clientName : x.agreement.client.clientShortName; 
                        lo.workIntno = item.workIntno;
                        lo.workName = x.workName;
                        lo.workType = LookupModels.decodeDictionaryItem("workType", x.workType);
                        lo.totalEpisodesReceived = item.thruEpisode - item.fromEpisode + 1;
                        lo.lastEpisodeReceived = item.thruEpisode;
                        lo.lastUpdate = DateTime.Today;
                        logModel.Add(lo);
                    }
                    else
                    {
                        lg.totalEpisodesReceived += item.thruEpisode - item.fromEpisode + 1;
                        lg.lastEpisodeReceived = item.thruEpisode;
                        lg.lastUpdate = DateTime.Today;
                    }

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
                        oi.status = "04";
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
            ViewBag.orderItem = orderTrnHdrIntno;
            return PartialView("_orderItemIssuesList", model1.ToList());
        }

        // planning dubbing, upload and shipment
        public ActionResult orderItemSelectList()
        {
            return PartialView("_orderItemSelectList");
        }

        public ActionResult scheduleAddNew(string oiList)
        {
            ViewBag.fdw = LookupModels.decodeDictionaryItem("settings", "fdw");
            ViewBag.oiList = oiList;
            return PartialView("_scheduleAddNew");
        }
        
        [ValidateAntiForgeryToken]
        [HttpPost, ValidateInput(false)]
        public ActionResult scheduleAddNew(string oiList, DateTime? dubbingDate, DateTime? uploadDate, DateTime? shipmentDate)
        {
            var model = db.orderTrnHdrs;
            string[] oiList1 = oiList.Split(';');
            bool changes = false;
            foreach(string item in oiList1)
            {
                var modelItem = model.Find(long.Parse(item));
                if (dubbingDate.HasValue || uploadDate.HasValue || shipmentDate.HasValue)
                    changes = true;
                
                if (dubbingDate.HasValue)
                    modelItem.plannedDubbing = dubbingDate;
                if (uploadDate.HasValue)
                    modelItem.plannedUpload = uploadDate;
                if (shipmentDate.HasValue)
                    modelItem.plannedShipment = shipmentDate;
            }
            if (changes)
                db.SaveChanges();
            return Content("Successfully Scheduled. ", "text/html");
        }

        // adaptation assignment
        public ActionResult adaptationAddNew(long? work, string oiList)
        {
            ViewBag.adaptorsList = new SelectList(db.workPersonnels.Include(b => b.employee)
                                .Where(b => (!work.HasValue || b.workIntno == work) && (b.titleType == "04" || b.titleType == "06") && b.status == true)
                                .Select(b => new { b.employee.empIntno, b.employee.fullName }), "empIntno", "fullName");
            ViewBag.translatorsList = new SelectList(db.workPersonnels.Include(b => b.employee)
                                .Where(b => (!work.HasValue || b.workIntno == work) && (b.titleType == "04" || b.titleType == "05") && b.status == true)
                                .Select(b => new { b.employee.empIntno, b.employee.fullName }), "empIntno", "fullName");
            ViewBag.oiList = oiList;
            return PartialView("_adaptationAddNew");
        }

        [ValidateAntiForgeryToken]
        [HttpPost, ValidateInput(false)]
        public ActionResult adaptationAddNew(string oiList, long? editor, long? translator, DateTime? startTranslation, DateTime? endTranslation, DateTime? startAdaptation, DateTime? endAdaptation)
        {
            var model = db.orderTrnHdrs;
            var dtlModel = db.orderTrnDtls;
            string[] oiList1 = oiList.Split(';');
            foreach (string item in oiList1)
            {
                long id = long.Parse(item);
                var modelItem = model.Find(id);
                if (startTranslation.HasValue)
                    modelItem.startTranslation = startTranslation;
                if (endTranslation.HasValue)
                    modelItem.endTranslation = endTranslation;
                if (startAdaptation.HasValue)
                    modelItem.startAdaptation = startAdaptation;
                if (endAdaptation.HasValue)
                    modelItem.startDischarge = endAdaptation;

                if(editor.HasValue)
                {
                    var x1 = dtlModel.FirstOrDefault(b => b.orderTrnHdrIntno == id && b.activityType == "02");
                    if (x1 == null)
                    {
                        orderTrnDtl dtl = new orderTrnDtl();
                        dtl.orderTrnHdrIntno = id;
                        dtl.empIntno = editor.Value;
                        dtl.activityType = "02";
                        dtlModel.Add(dtl);
                    }
                    else
                    {
                        x1.empIntno = editor.Value;
                    }
                }

                if (translator.HasValue)
                {
                    var x2 = dtlModel.FirstOrDefault(b => b.orderTrnHdrIntno == id && b.activityType == "01");
                    if (x2 == null)
                    {
                        orderTrnDtl dtl = new orderTrnDtl();
                        dtl.orderTrnHdrIntno = id;
                        dtl.empIntno = translator.Value;
                        dtl.activityType = "01";
                        dtlModel.Add(dtl);
                    }
                    else
                    {
                        x2.empIntno = translator.Value;
                    }
                }
            }

            db.SaveChanges();
            return Content("Successfully Assigned. ", "text/html");
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
            long client = db.orderTrnHdrs.Find(id).agreementWork.agreement.clientIntno;
            if (ModelState.IsValid)
            {
                try
                {
                    item.clientIntno = client;
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