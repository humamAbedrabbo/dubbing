﻿using System;
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
    public class actorsController : Controller
    {
        private DUBBDBEntities db = new DUBBDBEntities();
        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }

        // GET: actors
        public ActionResult Index()
        {
            return View();
        }

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

        // work charges for actors
        public ActionResult workChargesList(long id)
        {
            var model = db.workCharges.Where(b => b.workPartyIntno == id && b.workPartyType == "01");
            var x = db.workActors.Include(b => b.agreementWork)
                    .Where(b => b.voiceActorIntno == id && b.agreementWork.status == "01").OrderBy(b => new { b.workIntno, b.fromDate })
                    .Select(b => new { b.workIntno, b.agreementWork.workName });
            ViewBag.worksList = new SelectList(x, "workIntno", "workName");
            ViewBag.workPartyIntno = id;
            return PartialView("_workChargesList", model.ToList());
        }

        public ActionResult workChargesAddNew(long id)
        {
            ViewBag.workPartyIntno = id;
            ViewBag.worksList = db.agreementWorks.Where(b => b.status == "01").ToList();
            ViewBag.chargeUomList = LookupModels.getDictionary("chargeUom");
            ViewBag.currenciesList = LookupModels.getDictionary("currencyCode");
            return PartialView("_workChargesAddNew");
        }

        [ValidateAntiForgeryToken]
        [HttpPost, ValidateInput(false)]
        public ActionResult workChargesAddNew(workCharge item, long id)
        {
            var model = db.workCharges;
            if (ModelState.IsValid)
            {
                try
                {
                    // inactivate existing charge
                    var old = model.FirstOrDefault(b => b.workIntno == item.workIntno && b.workPartyType == "01" && b.workPartyIntno == id && b.status == true);
                    if (old != null)
                        old.status = false;
                    // insert new charge
                    item.workPartyType = "01";
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
            return Content("Successfully Added.", "text/html");
        }
    }
}