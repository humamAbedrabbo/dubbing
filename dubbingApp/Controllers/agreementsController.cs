using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using dubbingModel;
using dubbingApp.Models;

namespace dubbingApp.Controllers
{
    [Authorize(Roles = "ADMIN, GENERAL_MANAGER")]
    public class agreementsController : Controller
    {
        private DUBBDBEntities db = new DUBBDBEntities();

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }

        // GET: agreements
        public ActionResult Index(long id)
        {
            ViewBag.agreementIntno = id;
            ViewBag.returnUrl = Request.UrlReferrer.ToString();
            return View();
        }

        // specs
        public ActionResult specsList(long id)
        {
            var model = db.agreementSpecs.Where(b => b.agreementIntno == id);
            ViewBag.agreementIntno = id;
            return PartialView("_specsList", model.ToList());
        }

        public ActionResult specsAddNew(long id, string sType)
        {
            ViewBag.agreementIntno = id;
            ViewBag.specsType = sType;
            if (sType == "01")
                ViewBag.specsSubtypesList = LookupModels.getDictionary("videoSpecsSubtype");
            else if (sType == "02")
                ViewBag.specsSubtypesList = LookupModels.getDictionary("audioSpecsSubtype");
            else
                ViewBag.specsSubtypesList = LookupModels.getDictionary("otherSpecsSubtype");
            return PartialView("_specsAddNew");
        }

        [HttpPost, ValidateInput(false)]
        [ValidateAntiForgeryToken]
        public ActionResult specsAddNew(agreementSpec item, long id, string sType)
        {
            var model = db.agreementSpecs;
            if (ModelState.IsValid)
            {
                try
                {
                    item.agreementIntno = id;
                    item.specsType = sType;
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

        public ActionResult specsDelete(long id)
        {
            var model = db.agreementSpecs;
            long agr = 0;
            string sType;
            try
            {
                var modelItem = model.FirstOrDefault(b => b.specsIntno == id);
                agr = modelItem.agreementIntno;
                sType = modelItem.specsType;
                model.Remove(modelItem);
                db.SaveChanges();
            }
            catch (Exception e)
            {
                return Content("Failed to Delete! " + e.Message, "text/html");
            }
            var model1 = model.Where(b => b.agreementIntno == agr && b.specsType == sType);
            return PartialView("_specsList", model.ToList());
        }

        // terms
        public ActionResult termsList(long id)
        {
            var model = db.agreementTerms.Where(b => b.agreementIntno == id).OrderBy(b => b.sortOrder);
            ViewBag.agreementIntno = id;
            return PartialView("_termsList", model.ToList());
        }

        public ActionResult termsAddNew(long id)
        {
            ViewBag.agreementIntno = id;
            return PartialView("_termsAddNew");
        }

        [HttpPost, ValidateInput(false)]
        [ValidateAntiForgeryToken]
        public ActionResult termsAddNew(agreementTerm item, long id)
        {
            var model = db.agreementTerms;
            if (ModelState.IsValid)
            {
                try
                {
                    item.agreementIntno = id;
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

        public ActionResult termsDelete(long id)
        {
            var model = db.agreementTerms;
            long agr = 0;
            try
            {
                var modelItem = model.FirstOrDefault(b => b.termIntno == id);
                agr = modelItem.agreementIntno;
                model.Remove(modelItem);
                db.SaveChanges();
            }
            catch (Exception e)
            {
                return Content("Failed to Delete! " + e.Message, "text/html");
            }
            var model1 = model.Where(b => b.agreementIntno == agr);
            return PartialView("_termsList", model.ToList());
        }
    }
}