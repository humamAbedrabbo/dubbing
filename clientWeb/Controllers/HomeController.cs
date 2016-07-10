using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.Entity;
using System.Web;
using System.Web.Mvc;
using dubbingModel;

namespace clientWeb.Controllers
{
    [Authorize(Roles = "ADMIN, GENERAL_MANAGER, CLIENT")]
    public class HomeController : Controller
    {
        private DUBBDBEntities db = new DUBBDBEntities();
        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }

        public class orderItemsViewModel
        {
            public long orderItemId { get; set; }
            public long workIntno { get; set; }
            public string workName { get; set; }
            public string episodeNo { get; set; }
            public string status { get; set; }
            public string since { get; set; }
            public string totalIssues { get; set; }
            public string totalClaims { get; set; }
        }
        private long getClientId()
        {
            string email = User.Identity.Name;
            var x = db.contacts.FirstOrDefault(b => b.contactEmailAddr == email && b.contactParty == "01");
            if (x != null)
                return x.partyIntno;
            else
                return 0;
        }

        // GET: clientCare
        public ActionResult Index()
        {
            long clientId = getClientId();
            if (clientId != 0)
                return View();
            else
                return View("accessDenied");
        }

        public ActionResult worksList()
        {
            long clientIntno = getClientId();
            var model = db.agreementWorks.Where(b => b.agreement.clientIntno == clientIntno && b.status == "01");
            return PartialView("_worksList", model.ToList());
        }

        public ActionResult ordersList(long? work)
        {
            long clientIntno = getClientId();
            var model = db.orderTrnHdrs.Include(b => b.agreementWork)
                        .Where(b => (!work.HasValue || b.workIntno == work) && b.agreementWork.agreement.clientIntno == clientIntno)
                        .OrderBy(b => new { b.workIntno, b.episodeNo });
            List<orderItemsViewModel> orderItemsList = new List<orderItemsViewModel>();
            foreach (var x in model)
            {
                orderItemsViewModel oi = new orderItemsViewModel();
                long id = x.orderTrnHdrIntno;
                oi.orderItemId = id;
                oi.workIntno = x.workIntno;
                oi.workName = x.agreementWork.workName;
                oi.episodeNo = x.episodeNo.ToString();
                var x1 = db.orderChecks.Where(b => b.orderTrnHdrIntno == id);
                var x2 = db.clientClaims.Where(b => b.orderTrnHdrIntno == id && b.status == true);
                if (x1.Count() != 0)
                    oi.totalIssues = x1.Count().ToString();
                else
                    oi.totalIssues = null;
                if (x2.Count() != 0)
                    oi.totalClaims = x2.Count().ToString();
                else
                    oi.totalClaims = null;

                // order item status
                if (!x.startAdaptation.HasValue && !x.startDubbing.HasValue && x.status != "03")
                {
                    oi.status = "Accepted";
                    oi.since = x.orderReceivedDate.Value.ToString("dd-MM");
                }
                else if (x.status == "03")
                {
                    oi.status = "Rejected";
                    oi.since = x.orderReceivedDate.Value.ToString("dd-MM");
                }
                else if (x.startAdaptation.HasValue && !x.endAdaptation.HasValue)
                {
                    oi.status = "Adaptation";
                    oi.since = x.startAdaptation.Value.ToString("dd-MM");
                }
                else if (x.startDubbing.HasValue && !x.endDubbing.HasValue)
                {
                    oi.status = "Dubbing";
                    oi.since = x.startDubbing.Value.ToString("dd-MM");
                }
                else if (x.shipmentLowRes.HasValue && !x.shipmentFinal.HasValue)
                {
                    oi.status = "Uploaded";
                    oi.since = x.shipmentLowRes.Value.ToString("dd-MM");
                }
                else if (x.shipmentFinal.HasValue)
                {
                    oi.status = "Shipped";
                    oi.since = x.shipmentFinal.Value.ToString("dd-MM");
                }
                else
                {
                    oi.status = "Received";
                    oi.since = x.orderReceivedDate.Value.ToString("dd-MM");
                }

                orderItemsList.Add(oi);
            }
            return PartialView("_ordersList", orderItemsList);
        }

        public ActionResult orderItemDetails(long orderItem)
        {
            var model = db.orderTrnHdrs.Find(orderItem);
            List<string> dtlList = new List<string>();

            dtlList.Add("Received|" + model.orderReceivedDate.Value.ToShortDateString());
            if (model.expectedDeliveryDate.HasValue)
                dtlList.Add("Airing Date|" + model.expectedDeliveryDate.Value.ToShortDateString());
            else
                dtlList.Add("Airing Date|Not Provided By Client.");

            if (model.plannedDubbing.HasValue)
                dtlList.Add("Dubbing|" + model.plannedDubbing.Value.ToShortDateString());
            else
                dtlList.Add("Dubbing|Not Scheduled Yet.");

            if (model.plannedUpload.HasValue)
                dtlList.Add("Upload|" + model.plannedUpload.Value.ToShortDateString());
            else
                dtlList.Add("Upload|Not Scheduled Yet.");

            if (model.plannedShipment.HasValue)
                dtlList.Add("Shipment|" + model.plannedShipment.Value.ToShortDateString());
            else
                dtlList.Add("Shipment|Not Scheduled Yet.");

            ViewBag.workName = model.agreementWork.workName;
            ViewBag.episodeNo = model.episodeNo;
            return PartialView("_orderItemDetails", dtlList);
        }

        public ActionResult orderItemIssues(long orderItem)
        {
            var model = db.orderChecks.Where(b => b.orderTrnHdrIntno == orderItem);
            var checkTypes = db.dubbDomains.Where(b => b.domainName == "checkType" && b.langCode == "en" && b.status == true).ToList();
            foreach (var x in model)
            {
                string domainCode = x.checkType;
                x.checkType = checkTypes.FirstOrDefault(b => b.domainCode == domainCode).domainValue;
            }
            return PartialView("_orderItemIssues", model.ToList());
        }

        public ActionResult orderItemClaims(long orderItem)
        {
            var model = db.clientClaims.Where(b => b.orderTrnHdrIntno == orderItem);
            return PartialView("_orderItemClaims", model.ToList());
        }

        public ActionResult clientClaimsAddNew()
        {
            long clientIntno = getClientId();
            var orderItemsList = db.orderTrnHdrs.Where(b => b.agreementWork.agreement.clientIntno == clientIntno
                                    && ((b.shipmentLowRes.HasValue && !b.shipmentFinal.HasValue)
                                    || (b.shipmentFinal.HasValue && b.status == "04")))
                                    .Select(b => new { orderItem = b.orderTrnHdrIntno, workName = b.agreementWork.workName + " / " + b.episodeNo });
            ViewBag.orderItemsList = new SelectList(orderItemsList, "orderItem", "workName");
            return PartialView("_clientClaimsAddNew");
        }

        [ValidateAntiForgeryToken]
        [HttpPost, ValidateInput(false)]
        public ActionResult clientClaimsAddNew(clientClaim item)
        {
            var model = db.clientClaims;
            if (ModelState.IsValid && !string.IsNullOrEmpty(item.orderTrnHdrIntno.ToString()) && !string.IsNullOrEmpty(item.claimDesc) && !string.IsNullOrEmpty(item.requiredAction))
            {
                try
                {
                    var orderItem = db.orderTrnHdrs.Find(item.orderTrnHdrIntno);
                    if (orderItem.shipmentLowRes.HasValue && !orderItem.shipmentFinal.HasValue)
                        item.claimType = "01";
                    else
                        item.claimType = "02";
                    item.clientIntno = orderItem.agreementWork.agreement.clientIntno;
                    item.receivedDate = DateTime.Today.Date;
                    item.status = true;
                    model.Add(item);
                    db.SaveChanges();
                }
                catch (Exception e)
                {
                    return Content("Failed to Submit! Please Correct All Errors. " + e.Message, "text/html");
                }
            }
            else
                return Content("Failed to Submit! Please Complete Required Data.", "text/html");
            return Content("Claim Successfully Submitted.", "text/html");
        }

        public ActionResult approveDelivered(long orderItem)
        {
            var model = db.orderTrnHdrs.Find(orderItem);
            if (model.shipmentLowRes.HasValue && !model.shipmentFinal.HasValue)
                model.status = "01";
            else
                model.status = "02";
            db.SaveChanges();
            return null;
        }

        public ActionResult Contact()
        {
            return View();
        }
    }
}