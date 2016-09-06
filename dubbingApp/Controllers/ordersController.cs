using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
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
            switch (epFilter)
            {
                case "01": //New
                    model = x.Where(b => !b.startAdaptation.HasValue && b.status != "06");
                    break;
                case "02": //translation
                    model = x.Where(b => b.startTranslation.HasValue && !b.endTranslation.HasValue && b.status != "06");
                    break;
                case "03": //adaptation
                    model = x.Where(b => b.startAdaptation.HasValue && !b.endAdaptation.HasValue && b.status != "06");
                    break;
                case "04": //dubbing
                    model = x.Where(b => b.startDubbing.HasValue && !b.endDubbing.HasValue && b.status != "06");
                    break;
                case "05": //mixage & montage
                    model = x.Where(b => (b.startMixage.HasValue && !b.endMixage.HasValue) || (b.startMontage.HasValue && !b.endMontage.HasValue) && b.status != "06");
                    break;
                case "06": //upload
                    model = x.Where(b => b.shipmentLowRes.HasValue && !b.shipmentFinal.HasValue && b.status != "06");
                    break;
                case "07": //endorsed - archive
                    model = x.Where(b => b.shipmentFinal.HasValue || b.status == "06");
                    break;
                case "08": //rejected
                    model = x.Where(b => b.status == "03");
                    break;
                case "09": //unscheduled
                    model = x.Where(b => !b.plannedUpload.HasValue && (!b.shipmentLowRes.HasValue || !b.shipmentFinal.HasValue) && b.status != "06");
                    break;
                case "11": // all episodes in work(s)
                    model = x;
                    break;
                default:
                    model = x.Where(b => b.status == "03" || b.status == "04");
                    break;
            }

            return PartialView("_orderItemsList", model.OrderBy(b => new { b.workIntno, b.episodeNo }).ToList());
        }

        // filter settings
        public ActionResult filterSettings(long? work)
        {
            string settingsList;
            int cnt;
            var x = db.orderTrnHdrs.Include(b => b.agreementWork)
                                        .Where(b => b.agreementWork.status == "01"
                                        && (!work.HasValue || b.workIntno == work)
                                        && b.status != "06").ToList();
            
            // received quality issues
            cnt = (from A in x
                   join B in db.orderChecks on A.orderTrnHdrIntno equals B.orderTrnHdrIntno
                   where B.isAccepted == false
                   select A.orderTrnHdrIntno).Distinct().Count();
            settingsList = cnt.ToString() + ";";

            // client claims
            cnt = (from A in x
                   join B in db.clientClaims on A.orderTrnHdrIntno equals B.orderTrnHdrIntno
                   where B.status == true
                   select A.orderTrnHdrIntno).Distinct().Count();
            settingsList = settingsList + cnt.ToString();

            ViewBag.settingsList = settingsList;
            return PartialView("_filters");
        }

        public ActionResult pipelineFilterSettings(long? work)
        {
            string settingsList = null;
            int cnt = 0;
            var model = db.orderTrnHdrs.Include(b => b.agreementWork)
                                        .Where(b => b.agreementWork.status == "01"
                                        && (!work.HasValue || b.workIntno == work));
            var x = model.Where(b => b.status != "06").ToList();
            var y = model.ToList();

            // new
            cnt = x.Where(b => !b.startAdaptation.HasValue).Count();
            settingsList = cnt.ToString() + ";";
            
            // translation
            cnt = x.Where(b => b.startTranslation.HasValue && !b.endTranslation.HasValue).Count();
            settingsList = settingsList + cnt.ToString() + ";";

            // adaptation
            cnt = x.Where(b => b.startAdaptation.HasValue && !b.endAdaptation.HasValue).Count();
            settingsList = settingsList + cnt.ToString() + ";";

            // dubbing
            cnt = x.Where(b => b.startDubbing.HasValue && !b.endDubbing.HasValue).Count();
            settingsList = settingsList + cnt.ToString() + ";";

            // mixage & montage
            cnt = x.Where(b => (b.startMixage.HasValue && !b.endMixage.HasValue) || (b.startMontage.HasValue && !b.endMontage.HasValue)).Count();
            settingsList = settingsList + cnt.ToString() + ";";

            // upload
            cnt = x.Where(b => b.shipmentLowRes.HasValue && !b.shipmentFinal.HasValue).Count();
            settingsList = settingsList + cnt.ToString() + ";";

            // endorsed - archive
            cnt = y.Where(b => b.shipmentFinal.HasValue || b.status == "06").Count();
            settingsList = settingsList + cnt.ToString() + ";";

            // all episodes in work(s)
            cnt = y.Count();
            settingsList = settingsList + cnt.ToString() + ";";

            // rejected
            cnt = y.Where(b => b.status == "03").Count();
            settingsList = settingsList + cnt.ToString() + ";";

            // unscheduled
            cnt = x.Where(b => !b.plannedUpload.HasValue && (!b.shipmentLowRes.HasValue || !b.shipmentFinal.HasValue)).Count();
            settingsList = settingsList + cnt.ToString();

            ViewBag.settingsList = settingsList;
            return PartialView("_pipelineFilter");
        }

        // orders
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
                    item.status = "03";

                    var x = db.agreementWorks.Include(b => b.agreement.client).SingleOrDefault(b => b.workIntno == item.workIntno);

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

        public ActionResult endorseOrder(long order)
        {
            var model = db.workOrders.Find(order);
            model.status = "05";
            var orderItemModel = db.orderTrnHdrs.Where(b => b.orderIntno == order && b.status == "04").ToList();
            foreach (var x in orderItemModel)
            {
                x.status = "06";
            }
            db.SaveChanges();
            return RedirectToAction("ordersHistory", new { work = model.workIntno });
        }

        public ActionResult ordersHistory(long? work)
        {
            if (!work.HasValue)
            {
                var model = (from A in db.workOrders
                             join B in db.orderTrnHdrs on A.orderIntno equals B.orderIntno
                             join C in db.agreementWorks on A.workIntno equals C.workIntno
                             where !B.startAdaptation.HasValue && A.status == "03" && C.status == "01"
                             select A).Distinct().Include(b => b.agreementWork);
                return PartialView("_ordersHistory", model.OrderByDescending(b => b.receivedDate).ToList());
            }
            else
            {
                List<string> historyList = new List<string>();
                var x = db.orderTrnHdrs.Where(b => b.workIntno == work.Value);
                if (x.Count() != 0)
                {
                    historyList.Add("Total Received|" + x.Count());
                    historyList.Add("Last Received|" + x.Max(b => b.orderReceivedDate.Value).ToShortDateString());
                    historyList.Add("Total Rejected|" + x.Where(b => b.status == "03").Count());
                    int u = x.Where(b => b.shipmentLowRes.HasValue && !b.shipmentFinal.HasValue).Count();
                    historyList.Add("Total Uploaded|" + u);
                    if (u != 0)
                        historyList.Add("Last Uploaded|" + x.Where(b => b.shipmentLowRes.HasValue && !b.shipmentFinal.HasValue).Max(b => b.shipmentLowRes.Value).ToShortDateString());
                    historyList.Add("Total Shipped|" + x.Where(b => b.shipmentFinal.HasValue).Count());
                }
                else
                {
                    historyList.Add("Total Received|0");
                    historyList.Add("Last Received|-");
                    historyList.Add("Total Rejected|0");
                    historyList.Add("Total Uploaded|0");
                    historyList.Add("Last Uploaded|-");
                    historyList.Add("Total Shipped|0");
                }

                ViewBag.historyList = historyList;
                ViewBag.workIntno = work.Value;
                var model1 = db.workOrders.Include(b => b.agreementWork)
                            .Where(b => b.workIntno == work.Value).OrderByDescending(b => b.receivedDate);
                return PartialView("_ContractHistory", model1.ToList());
            }
        }

        // plan upload, dubbing and shipment
        public ActionResult scheduleAddNew(long orderItem, int episodeNo)
        {
            ViewBag.workName = db.orderTrnHdrs.Include(b => b.agreementWork).FirstOrDefault(b => b.orderTrnHdrIntno == orderItem).agreementWork.workName;
            ViewBag.orderItem = orderItem;
            ViewBag.episodeNo = episodeNo;
            return PartialView("_scheduleAddNew");
        }

        [ValidateAntiForgeryToken]
        [HttpPost, ValidateInput(false)]
        public ActionResult scheduleAddNew(long orderItem, int fromEpisode, int thruEpisode, DateTime? dubbingDate, DateTime? uploadDate, DateTime? shipmentDate)
        {
            if (dubbingDate.HasValue || uploadDate.HasValue || shipmentDate.HasValue)
            {
                var model = db.orderTrnHdrs;
                long work = model.Find(orderItem).workIntno;
                var oiList = model.Where(b => b.workIntno == work && b.episodeNo >= fromEpisode && b.episodeNo <= thruEpisode)
                            .Select(b => b.orderTrnHdrIntno).ToList();
                foreach (long item in oiList)
                {
                    var modelItem = model.Find(item);

                    if (dubbingDate.HasValue)
                        modelItem.plannedDubbing = dubbingDate;
                    if (uploadDate.HasValue)
                        modelItem.plannedUpload = uploadDate;
                    if (shipmentDate.HasValue)
                        modelItem.plannedShipment = shipmentDate;
                }
                db.SaveChanges();
                return Content("Successfully Scheduled. ", "text/html");
            }
            else
                return Content("Failed! At Least one scheduling date must be given. ", "text/html");
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