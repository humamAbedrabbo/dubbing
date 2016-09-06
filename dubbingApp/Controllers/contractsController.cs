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
    [Authorize(Roles = "ADMIN, GENERAL_MANAGER")]
    public class contractsController : Controller
    {
        private DUBBDBEntities db = new DUBBDBEntities();

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }

        //
        // GET: /clients/

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult filters()
        {
            var x = db.clients.Where(b => b.status == "01")
                    .Select(b => new { b.clientIntno, b.clientName });
            ViewBag.clientsList = new SelectList(x, "clientIntno", "clientName");

            var y = db.agreements.Where(b => b.status == "01")
                    .Select(b => new { b.agreementIntno, b.agreementName });
            ViewBag.agreementsList = new SelectList(y, "agreementIntno", "agreementName");

            SelectList workStatusList = new SelectList(new List<SelectListItem> {
                new SelectListItem { Selected = true, Text = "Active Works", Value = "01" },
                new SelectListItem { Selected = false, Text = "Suspended Works", Value = "02" },
                new SelectListItem { Selected = false, Text = "Archived Works", Value = "03" },
                new SelectListItem { Selected = false, Text = "Canceled Works", Value = "04" }
            }, "Value", "Text");
            ViewBag.workStatusList = workStatusList;

            SelectList workTypesList = new SelectList(LookupModels.getDictionary("workType"), "key", "value");
            ViewBag.workTypesList = workTypesList;
            return PartialView("_filters");
        }

        public ActionResult clientContactsList(long? client)
        {
            if (client.HasValue)
            {
                var model = db.contacts.Where(b => b.partyIntno == client && b.contactParty == "01" && b.status == true);
                return PartialView("_clientContactsList", model.ToList());
            }
            else
                return null;
        }

        // clients
        public ActionResult clientAddNew()
        {
            return PartialView("_clientAddNew");
        }

        [HttpPost, ValidateInput(false)]
        [ValidateAntiForgeryToken]
        public ActionResult clientAddNew(client item)
        {
            var model = db.clients;
            item.status = "01";
            model.Add(item);
            db.SaveChanges();

            var model1 = db.clients.Where(b => b.status == "01")
                    .Select(b => new { b.clientIntno, b.clientName });
            SelectList cList = new SelectList(model1, "clientIntno", "clientName");
            return Json(cList);
        }

        public ActionResult clientUpdate(long id)
        {
            var model = db.clients.FirstOrDefault(b => b.clientIntno == id);
            ViewBag.statusList = LookupModels.getDictionary("clientStatus");
            return PartialView("_clientUpdate", model);
        }

        [HttpPost, ValidateInput(false)]
        [ValidateAntiForgeryToken]
        public ActionResult clientUpdate(client item)
        {
            var model = db.clients;
            if (ModelState.IsValid)
            {
                try
                {
                    var modelItem = model.FirstOrDefault(b => b.clientIntno == item.clientIntno);
                    this.UpdateModel(modelItem);
                    db.SaveChanges();
                }
                catch (Exception e)
                {
                    return Content("Failed! Please Correct All Data. " + e.Message, "text/html");
                }
            }
            else
                return Content("Failed! Please Enter All Data.", "text/html");
            return Content("Successfully Updated.", "text/html");
        }

        // agreements
        public ActionResult agreementAddNew()
        {
            ViewBag.clientsList = new SelectList(db.clients.Select(b => new { b.clientIntno, b.clientName }), "clientIntno", "clientName");
            ViewBag.agreementTypesList = LookupModels.getDictionary("agreementType");
            ViewBag.durationUomList = LookupModels.getDictionary("durationUom");
            return PartialView("_agreementAddNew");
        }

        [HttpPost, ValidateInput(false)]
        [ValidateAntiForgeryToken]
        public ActionResult agreementAddNew(agreement item)
        {
            var model = db.agreements;
            item.status = "01";
            model.Add(item);
            db.SaveChanges();

            var model1 = db.agreements.Where(b => b.status == "01")
                    .Select(b => new { b.agreementIntno, b.agreementName });
            SelectList aList = new SelectList(model1, "agreementIntno", "agreementName");
            return Json(aList);
        }

        public ActionResult agreementUpdate(long id)
        {
            var model = db.agreements.FirstOrDefault(b => b.agreementIntno == id);
            ViewBag.agreementTypesList = LookupModels.getDictionary("agreementType");
            ViewBag.durationUomList = LookupModels.getDictionary("durationUom");
            ViewBag.statusList = LookupModels.getDictionary("agreementStatus");
            return PartialView("_agreementUpdate", model);
        }

        [HttpPost, ValidateInput(false)]
        [ValidateAntiForgeryToken]
        public ActionResult agreementUpdate(agreement item)
        {
            var model = db.agreements;
            if (ModelState.IsValid)
            {
                try
                {
                    var modelItem = model.FirstOrDefault(b => b.agreementIntno == item.agreementIntno);
                    this.UpdateModel(modelItem);
                    db.SaveChanges();
                }
                catch (Exception e)
                {
                    return Content("Failed! Please Correct All Data. " + e.Message, "text/html");
                }
            }
            else
                return Content("Failed! Please Enter All Data.", "text/html");
            return Content("Successfully Updated.", "text/html");
        }

        public JsonResult populateAgreementsListCombo(long? id)
        {
            var model = db.agreements.Where(b => !id.HasValue || b.clientIntno == id).Select(b => new { b.agreementIntno, b.agreementName });
            SelectList aList = new SelectList(model, "agreementIntno", "agreementName");
            return Json(aList);
        }
        
        public JsonResult agreementChanged(long? id)
        {
            if (id.HasValue)
                return Json(db.agreements.Find(id).clientIntno);
            else
                return Json(null);
        }

        // contracts
        public ActionResult contractsList(long? client, long? agreement, string status, string workType)
        {
            var model = db.agreementWorks.Include(b => b.agreement)
                        .Where(b => (!client.HasValue || b.agreement.clientIntno == client) 
                        && (!agreement.HasValue || b.agreementIntno == agreement) 
                        && (string.IsNullOrEmpty(workType) || b.workType == workType)
                        && b.status == status);
            return PartialView("_contractsList", model.ToList());
        }

        public ActionResult contractAddNew(long client)
        {
            var x = db.agreements.Where(b => b.clientIntno == client && b.status == "01").Select(b => new { b.agreementIntno, b.agreementName });
            ViewBag.agreementsList = new SelectList(x, "agreementIntno", "agreementName");
            ViewBag.workTypesList = LookupModels.getDictionary("workType");
            ViewBag.workNationalitiesList = LookupModels.getDictionary("workNationality");
            ViewBag.originalLanguagesList = LookupModels.getDictionary("originalLanguage");
            var y = db.contacts.Where(b => b.partyIntno == client && b.contactParty == "01" && b.status == true).Select(b => new { b.contactIntno, b.contactName });
            ViewBag.contactsList = new SelectList(y, "contactIntno", "contactName");
            return PartialView("_contractAddNew");
        }

        [HttpPost, ValidateInput(false)]
        [ValidateAntiForgeryToken]
        public ActionResult contractAddNew(agreementWork item)
        {
            var model = db.agreementWorks;
            var logModel = db.logWorks;
            if (ModelState.IsValid)
            {
                try
                {
                    item.status = "01";
                    model.Add(item);
                    db.SaveChanges();

                    //insert new work in works log
                    logWork lw = new logWork();
                    var x = db.agreements.Include(b => b.client).FirstOrDefault(b => b.agreementIntno == item.agreementIntno);
                    lw.clientIntno = x.clientIntno;
                    lw.clientName = string.IsNullOrEmpty(x.client.clientShortName) ? x.client.clientName : x.client.clientShortName;
                    lw.workIntno = item.workIntno;
                    lw.workName = item.workName;
                    lw.workType = LookupModels.decodeDictionaryItem("workType", item.workType);
                    lw.workNationality = LookupModels.decodeDictionaryItem("workNationality", item.workNationality);
                    lw.contractedDate = DateTime.Today.Date;
                    lw.contractedYear = DateTime.Today.Year;
                    lw.contractedMonth = DateTime.Today.Month;
                    lw.lastUpdate = DateTime.Today;
                    lw.updatedBy = User.Identity.Name;
                    logModel.Add(lw);
                    db.SaveChanges();
                }
                catch(Exception e)
                {
                    return Content("Failed! Please Correct All Data. " + e.Message, "text/html");
                }
            }
            else
                return Content("Failed! Please Enter All Data.", "text/html");
            return Content("Successfully Added.", "text/html");
        }

        public ActionResult contractUpdate(long id)
        {
            var model = db.agreementWorks.FirstOrDefault(b => b.workIntno == id);
            long client = model.agreement.clientIntno;
            var x = db.agreements.Where(b => b.clientIntno == client && b.status == "01").Select(b => new { b.agreementIntno, b.agreementName });
            ViewBag.agreementsList = new SelectList(x, "agreementIntno", "agreementName");
            ViewBag.workTypesList = LookupModels.getDictionary("workType");
            ViewBag.workNationalitiesList = LookupModels.getDictionary("workNationality");
            ViewBag.originalLanguagesList = LookupModels.getDictionary("originalLanguage");
            var y = db.contacts.Where(b => b.partyIntno == client && b.contactParty == "01" && b.status == true).Select(b => new { b.contactIntno, b.contactName });
            ViewBag.contactsList = new SelectList(y, "contactIntno", "contactName");
            return PartialView("_contractUpdate", model);
        }

        [HttpPost, ValidateInput(false)]
        [ValidateAntiForgeryToken]
        public ActionResult contractUpdate(agreementWork item)
        {
            var model = db.agreementWorks;
            if (ModelState.IsValid)
            {
                try
                {
                    var modelItem = model.FirstOrDefault(b => b.workIntno == item.workIntno);
                    this.UpdateModel(modelItem);
                    db.SaveChanges();
                }
                catch (Exception e)
                {
                    return Content("Failed! Please Correct All Data. " + e.Message, "text/html");
                }
            }
            else
                return Content("Failed! Please Enter All Data.", "text/html");
            return Content("Successfully Updated.", "text/html");
        }
        
        public ActionResult contractStatusChange(long? client, long? agreement, long work, string newStatus)
        {
            if (newStatus == "03") //endorse contract(work)
            {
                //call stored procedure to clean unecessary details
                string resp = null;
                db.archiveEndorsedWork(work, User.Identity.Name);
                if (!string.IsNullOrEmpty(resp))
                    return Content("Failed! Unable to Endorse Work(Contract). Please try later.", "text/html");
            }
            else
            {
                var model = db.agreementWorks;
                try
                {
                    var modelItem = model.FirstOrDefault(b => b.workIntno == work);
                    modelItem.status = newStatus;
                    db.SaveChanges();
                }
                catch (Exception e)
                {
                    return Content("Failed! Please Correct All Data. " + e.Message, "text/html");
                }
            }

            long? client1 = client;
            long? agreement1 = agreement;
            return RedirectToAction("contractsList", new { client = client1, agreement = agreement1, status = "01" });
        }

        // work contact
        public ActionResult workContact(long work)
        {
            var x = db.agreementWorks.Find(work);
            var model = db.contacts.Find(x.contactIntno);
            model.salutation = LookupModels.decodeDictionaryItem("salutation", model.salutation);
            return PartialView("_workContact", model);
        }


        // personnel
        public ActionResult teamList(long id)
        {
            var model = db.workPersonnels.Where(b => b.workIntno == id && b.status == true).OrderBy(b => b.titleType);
            ViewBag.workIntno = id;
            return PartialView("_teamList", model.ToList());
        }

        public ActionResult titlesComboChanged(string title)
        {
            string empType;
            // mapping titleType to empType
            if (title == "01" || title == "02" || title == "03")
                empType = "01";
            else if (title == "04")
                empType = "02";
            else if (title == "05")
                empType = "03";
            else if (title == "06")
                empType = "04";
            else
                empType = null;

            var x = db.employees.Where(b => (string.IsNullOrEmpty(title) || b.empType == empType) && b.status == true).Select(b => new { b.empIntno, b.fullName });
            return Json(new SelectList(x, "empIntno", "fullName"));
        }

        public ActionResult teamAddNew(long id)
        {
            ViewBag.workIntno = id;
            ViewBag.titleTypesList = LookupModels.getDictionary("titleType");
            ViewBag.personnelList = new SelectList(db.employees.Where(b => b.empType == "02" && b.status == true), "empIntno", "fullName");
            return PartialView("_teamAddNew");
        }

        [HttpPost, ValidateInput(false)]
        [ValidateAntiForgeryToken]
        public ActionResult teamAddNew(workPersonnel item, long id)
        {
            var model = db.workPersonnels;
            if (ModelState.IsValid)
            {
                try
                {
                    item.workIntno = id;
                    item.status = true;
                    model.Add(item);
                    db.SaveChanges();
                }
                catch (Exception e)
                {
                    return Content("Failed! Please Correct All Data. " + e.Message, "text/html");
                }
            }
            else
                return Content("Failed! Please Enter All Data.", "text/html");
            return Content("Successfully Added.", "text/html");
        }

        public ActionResult teamDelete(long id)
        {
            var model = db.workPersonnels;
            long work = 0;
            try
            {
                var modelItem = model.Find(id);
                work = modelItem.workIntno;
                modelItem.thruDate = DateTime.Today.Date;
                modelItem.status = false;
                this.UpdateModel(modelItem);
                db.SaveChanges();
            }
            catch (Exception e)
            {
                return Content("Failed to Remove!. " + e.Message, "text/html");
            }
            var model1 = db.workPersonnels.Where(b => b.workIntno == work && b.status == true);
            return PartialView("_teamList", model1.ToList());
        }

        // orders
        public ActionResult ordersHistory(long? client, long? work, string workStatus)
        {
            var z = db.workOrders.Include(b => b.agreementWork).Where(b => (!client.HasValue || b.clientIntno == client)
                                        && (string.IsNullOrEmpty(workStatus) || b.agreementWork.status == workStatus));
            var model = z;
            if (!work.HasValue)
            {
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
                ViewBag.contractStatus = LookupModels.decodeDictionaryItem("workStatus", db.agreementWorks.Find(work.Value).status);
                model = z.Where(b => b.workIntno == work.Value).OrderByDescending(b => b.receivedDate);
                return PartialView("_ContractHistory", model.ToList());
            }
            
        }

    }
}