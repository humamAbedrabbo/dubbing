using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using dubbingModel;
using dubbingApp.Models;

namespace dubbingApp.Controllers
{
    //[Authorize(Roles = "ADMIN, GENERAL_MANAGER")]
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

        //personnel
        public ActionResult personnelList(bool isActive)
        {
            var model = db.employees.Where(b => b.status == isActive);
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
    }
}