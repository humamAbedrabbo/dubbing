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
    //[Authorize(Roles = "ADMIN, GENERAL_MANAGER, FINANCE_MANAGER")]
    public class adminController : Controller
    {
        private DUBBDBEntities db = new DUBBDBEntities();
        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }

        // GET: admin
        public ActionResult Index()
        {
            return View();
        }

        // actors
        public ActionResult actorsList(bool isActive, string alpha)
        {
            var model = db.voiceActors.Where(b => (string.IsNullOrEmpty(alpha) || b.fullName.StartsWith(alpha)) && b.status == isActive).ToList();
            List<string> alphaList = new List<string>();
            string fl = model[0].fullName.Substring(0, 1).ToUpper();
            alphaList.Add(fl);
            for (int i = 1; i < model.Count(); i++)
            {
                fl = model[i].fullName.Substring(0, 1).ToUpper();
                if (!alphaList.Contains(fl))
                    alphaList.Add(fl);
            }
            ViewBag.alphaList = alphaList.OrderBy(x => x);
            return PartialView("_actorsList", model);
        }

        public ActionResult actorsAddNew()
        {
            return PartialView("_actorsAddNew");
        }

        [ValidateAntiForgeryToken]
        [HttpPost, ValidateInput(false)]
        public ActionResult actorsAddNew(voiceActor item)
        {
            var model = db.voiceActors;
            if (ModelState.IsValid)
            {
                try
                {
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
            return Content("Successfully Created.", "text/html");
        }

        public ActionResult actorsUpdate(long id)
        {
            var model = db.voiceActors.Find(id);
            return PartialView("_actorsUpdate", model);
        }

        [ValidateAntiForgeryToken]
        [HttpPost, ValidateInput(false)]
        public ActionResult actorsUpdate(voiceActor item)
        {
            var model = db.voiceActors;
            if (ModelState.IsValid)
            {
                try
                {
                    var modelItem = model.Find(item.voiceActorIntno);
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

        public ActionResult actorsDelete(long id)
        {
            var model = db.voiceActors.Find(id);
            model.status = false;
            db.SaveChanges();
            return RedirectToAction("actorsList", new { isActive = true });
        }

        //personnel
        public ActionResult personnelList(bool isActive)
        {
            var model = db.employees.Where(b => b.status == isActive);
            ViewBag.empTypesList = LookupModels.getDictionary("empType");
            return PartialView("_personnelList", model.ToList());
        }

        public ActionResult personnelAddNew(string empType)
        {
            ViewBag.empType = empType;
            return PartialView("_personnelAddNew");
        }

        [ValidateAntiForgeryToken]
        [HttpPost, ValidateInput(false)]
        public ActionResult personnelAddNew(employee item, string empType)
        {
            var model = db.employees;
            if (ModelState.IsValid)
            {
                try
                {
                    item.empType = empType;
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
            return Content("Successfully Created.", "text/html");
        }

        public ActionResult personnelUpdate(long id)
        {
            var model = db.employees.Find(id);
            ViewBag.empTypesList = LookupModels.getDictionary("empType");
            return PartialView("_personnelUpdate", model);
        }

        [ValidateAntiForgeryToken]
        [HttpPost, ValidateInput(false)]
        public ActionResult personnelUpdate(employee item)
        {
            var model = db.employees;
            if (ModelState.IsValid)
            {
                try
                {
                    var modelItem = model.Find(item.empIntno);
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

        public ActionResult personnelDelete(long id)
        {
            var model = db.employees.Find(id);
            model.status = false;
            db.SaveChanges();
            return RedirectToAction("personnelList", new { isActive = true });
        }

        // charges for both personnel and actors
        public ActionResult workChargesList(long id, string partyType)
        {
            var model = db.workCharges.Where(b => b.workPartyType == partyType && b.workPartyIntno == id);
            if (partyType == "01") // actors
            {
                var x = db.workActors.Include(b => b.agreementWork)
                    .Where(b => b.voiceActorIntno == id && b.agreementWork.status == "01").OrderBy(b => new { b.workIntno, b.fromDate })
                    .Select(b => new { b.workIntno, b.agreementWork.workName });
                ViewBag.worksList = new SelectList(x, "workIntno", "workName");
            }
            else if (partyType == "02") // technical team
            {
                var x = db.workPersonnels.Include(b => b.agreementWork)
                    .Where(b => b.empIntno == id && b.agreementWork.status == "01").OrderBy(b => new { b.workIntno, b.fromDate })
                    .Select(b => new { b.workIntno, b.agreementWork.workName });
                ViewBag.worksList = new SelectList(x, "workIntno", "workName");
            }
            ViewBag.workPartyType = partyType;
            ViewBag.workPartyIntno = id;
            return PartialView("_workChargesList", model.ToList());
        }

        public ActionResult workChargesAddNew(long id, long work, string partyType)
        {
            ViewBag.workPartyIntno = id;
            ViewBag.workIntno = work;
            ViewBag.workPartyType = partyType;
            ViewBag.chargeUomList = LookupModels.getDictionary("chargeUom");
            ViewBag.currenciesList = LookupModels.getDictionary("currencyCode");
            return PartialView("_workChargesAddNew");
        }

        [ValidateAntiForgeryToken]
        [HttpPost, ValidateInput(false)]
        public ActionResult workChargesAddNew(workCharge item, long id, long work, string partyType)
        {
            var model = db.workCharges;
            if (ModelState.IsValid)
            {
                try
                {
                    // inactivate existing charge
                    var old = model.FirstOrDefault(b => b.workIntno == work && b.workPartyType == partyType && b.workPartyIntno == id && b.status == true);
                    if (old != null)
                        old.status = false;
                    // insert new charge
                    item.workIntno = work;
                    item.workPartyType = partyType;
                    item.workPartyIntno = id;
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
            long id1 = id;
            string partyType1 = partyType;
            return RedirectToAction("workChargesList", new { id = id1, partyType = partyType1 });
        }
    }
}