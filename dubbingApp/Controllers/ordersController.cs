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
    //[Authorize(Roles = "ADMIN, GENERAL_MANAGER, PRODUCTION_MANAGER, MONTAGE")]
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
            switch(stage)
            {
                case "01": //new
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
            return PartialView("_orderItemSelectList");
        }
    }
}