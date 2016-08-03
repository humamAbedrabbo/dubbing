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
    [Authorize(Roles = "ADMIN, GENERAL_MANAGER, FINANCE_MANAGER")]
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
            List<char> alphaList = new List<char>();
            //build alphabetical list
            for (char c = 'A'; c <= 'Z'; ++c)
            {
                alphaList.Add(c);
            }
            ViewBag.alphaList = alphaList;
            return View();
        }

        public ActionResult actorsList(string status, string alpha)
        {
            var x = db.voiceActors.Where(b => (string.IsNullOrEmpty(alpha) || b.fullName.TrimStart().StartsWith(alpha))
                                    && b.voiceActorIntno != 0);
            var model = x;
            switch (status)
            {
                case "01":
                    model = x.Where(b => b.status == true);
                    break;
                case "02":
                    model = x.Where(b => b.status == false);
                    break;
                case "03":
                    model = x.Where(b => string.IsNullOrEmpty(b.accountNo));
                    break;
                case "04":
                    model = (from A in x
                             join B in db.workActors on A.voiceActorIntno equals B.voiceActorIntno
                             join C in db.agreementWorks on B.workIntno equals C.workIntno
                             join D in db.workCharges on C.workIntno equals D.workIntno
                             join E in db.workCharges on A.voiceActorIntno equals E.workPartyIntno
                             where C.status == "01" && E.workPartyType == "01" && E.status == true
                             select A).Distinct();
                    break;
                default:
                    break;
            }
            return PartialView("_actorsList", model.ToList());
        }

        public ActionResult filterSettings()
        {
            string filtersList = null;
            int cnt = 0;
            var x = db.voiceActors.Where(b => b.voiceActorIntno != 0);

            //active
            cnt = x.Where(b => b.status == true).Count();
            filtersList = cnt.ToString();

            //inactive
            cnt = x.Where(b => b.status == false).Count();
            filtersList = filtersList + ";" + cnt.ToString();

            //incomplete definition
            cnt = x.Where(b => string.IsNullOrEmpty(b.accountNo)).Count();
            filtersList = filtersList + ";" + cnt.ToString();

            //uncharged
            cnt = (from A in x
                   join B in db.workActors on A.voiceActorIntno equals B.voiceActorIntno
                   join C in db.agreementWorks on B.workIntno equals C.workIntno
                   join D in db.workCharges on C.workIntno equals D.workIntno
                   join E in db.workCharges on A.voiceActorIntno equals E.workPartyIntno
                   where C.status == "01" && E.workPartyType == "01" && E.status == true
                   select A).Distinct().Count();
            filtersList = filtersList + ";" + cnt.ToString();

            ViewBag.filtersList = filtersList;
            return PartialView("_filters");
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
            return RedirectToAction("actorsList", new { status = "01" });
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
            var x = db.workActors.Include(b => b.agreementWork)
                    .Where(b => b.voiceActorIntno == id && b.agreementWork.status == "01").OrderBy(b => new { b.workIntno, b.fromDate })
                    .Select(b => new { b.workIntno, b.agreementWork.workName });
            ViewBag.worksList = new SelectList(x, "workIntno", "workName");
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