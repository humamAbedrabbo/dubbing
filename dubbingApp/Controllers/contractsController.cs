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
    //[Authorize(Roles = "ADMIN, GENERAL_MANAGER")]
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
            var x = db.clients.Where(b => b.status == "01")
                    .Select(b => new { b.clientIntno, b.clientName });
            ViewBag.clientsList = new SelectList(x, "clientIntno", "clientName");

            var y = db.agreements.Where(b => b.status == "01")
                    .Select(b => new { b.agreementIntno, b.agreementName });
            ViewBag.agreementsList = new SelectList(y, "agreementIntno", "agreementName");

            return View();
        }

        public ActionResult clientContactsList(long? client)
        {
            if (client.HasValue)
            {
                var model = db.contacts.Where(b => b.partyIntno == client && b.contactParty == "01" && b.status == true);
                return PartialView("_clientContactsList", model.ToList());
            }
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
            if (ModelState.IsValid)
            {
                try
                {
                    item.status = "01";
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
            return Content("Successfully Created.", "text/html");
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
            if (ModelState.IsValid)
            {
                try
                {
                    item.status = "01";
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
            return Content("Successfully Created.", "text/html");
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
        public ActionResult contractsList(long? client, long? agreement)
        {
            var model = db.agreementWorks.Include(b => b.agreement)
                        .Where(b => (!client.HasValue || b.agreement.clientIntno == client) && (!agreement.HasValue || b.agreementIntno == agreement) && (b.status == "01" || b.status == "02"));
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
            if (ModelState.IsValid)
            {
                try
                {
                    item.status = "01";
                    model.Add(item);
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

        public ActionResult contractHistory(long id)
        {
            List<string> historyList = new List<string>();
            var x = db.orderTrnHdrs.Where(b => b.workIntno == id);
            historyList.Add("Total Received|" + x.Count());
            historyList.Add("Last Received|" + x.Max(b => b.orderReceivedDate.Value).ToShortDateString());
            historyList.Add("Total Rejected|" + x.Where(b => b.status == "03").Count());
            historyList.Add("Total Uploaded|" + x.Where(b => b.shipmentLowRes.HasValue && !b.shipmentFinal.HasValue).Count());
            historyList.Add("Last Uploaded|" + x.Where(b => b.shipmentLowRes.HasValue && !b.shipmentFinal.HasValue).Max(b => b.shipmentLowRes.Value).ToShortDateString());
            historyList.Add("Total Shipped|" + x.Where(b => b.shipmentFinal.HasValue).Count());

            ViewBag.workIntno = id;
            return PartialView("_ContractHistory", historyList);
        }

        public ActionResult contractStatusChange(long? client, long? agreement, long work, string newStatus)
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

            var model1 = db.agreementWorks.Include(b => b.agreement).Where(b => (!client.HasValue || b.agreement.clientIntno == client) && (!agreement.HasValue || b.agreementIntno == agreement) && (b.status == "01" || b.status == "02"));
            return PartialView("_contractsList", model1.ToList());
        }

        // personnel
        public ActionResult teamList(long id)
        {
            var model = db.workPersonnels.Where(b => b.workIntno == id && b.status == true);
            ViewBag.workIntno = id;
            return PartialView("_teamList", model.ToList());
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
    }
}