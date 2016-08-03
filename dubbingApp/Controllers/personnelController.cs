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
    public class personnelController : Controller
    {
        private DUBBDBEntities db = new DUBBDBEntities();
        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }

        // GET: personnel
        public ActionResult Index()
        {
            return View();
        }

        //personnel
        public ActionResult personnelList(string status)
        {
            var x = db.employees.Where(b => b.empIntno != 0);

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
                             join B in db.workPersonnels on A.empIntno equals B.empIntno
                             join C in db.agreementWorks on B.workIntno equals C.workIntno
                             join D in db.workCharges on C.workIntno equals D.workIntno
                             join E in db.workCharges on A.empIntno equals E.workPartyIntno
                             where C.status == "01" && E.workPartyType == "02" && E.status == true
                             select A).Distinct();
                    break;
                default:
                    break;
            }
            
            ViewBag.empTypesList = LookupModels.getDictionary("empType");
            return PartialView("_personnelList", model.Distinct().ToList());
        }

        public ActionResult filterSettings()
        {
            string filtersList = null;
            int cnt = 0;
            var x = db.employees.ToList();

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
                    join B in db.workPersonnels on A.empIntno equals B.empIntno
                    join C in db.agreementWorks on B.workIntno equals C.workIntno
                    join D in db.workCharges on C.workIntno equals D.workIntno
                    join E in db.workCharges on A.empIntno equals E.workPartyIntno
                    where C.status == "01" && E.workPartyType == "02" && E.status == true
                    select A).Distinct().Count();
            filtersList = filtersList + ";" + cnt.ToString();

            ViewBag.filtersList = filtersList;
            return PartialView("_filters");
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

        // charges for personnel
        public ActionResult workChargesList(long id)
        {
            var model = db.workCharges.Where(b => b.workPartyType == "02" && b.workPartyIntno == id);
            var x = db.workPersonnels.Include(b => b.agreementWork)
                    .Where(b => b.empIntno == id && b.agreementWork.status == "01").OrderBy(b => new { b.workIntno, b.fromDate })
                    .Select(b => new { b.workIntno, b.agreementWork.workName });
            ViewBag.worksList = new SelectList(x, "workIntno", "workName");
            ViewBag.workPartyIntno = id;
            return PartialView("_workChargesList", model.ToList());
        }

        public ActionResult workChargesAddNew(long id)
        {
            ViewBag.workPartyIntno = id;
            var x = db.workPersonnels.Include(b => b.agreementWork)
                    .Where(b => b.empIntno == id && b.agreementWork.status == "01").OrderBy(b => new { b.workIntno, b.fromDate })
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
                    var old = model.FirstOrDefault(b => b.workIntno == item.workIntno && b.workPartyType == "02" && b.workPartyIntno == id && b.status == true);
                    if (old != null)
                        old.status = false;
                    // insert new charge
                    item.workPartyType = "02";
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
            return RedirectToAction("workChargesList", new { id = id1 });
        }
    }
}