using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using dubbingApp.Models;
using dubbingModel;

namespace dubbingApp.Controllers
{
    public class clientContactsController : Controller
    {
        private DUBBDBEntities db = new DUBBDBEntities();

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }

        // GET: clientContacts
        public ActionResult Index(long client)
        {
            ViewBag.clientIntno = client;
            ViewBag.returnUrl = Request.UrlReferrer.ToString();
            return View();
        }

        public ActionResult clientContactsList(long id)
        {
            var model = db.contacts.Where(b => b.partyIntno == id && b.contactParty == "01" && b.status == true);
            ViewBag.clientIntno = id;
            return PartialView("_clientContactsList", model.ToList());
        }

        public ActionResult clientContactAddNew(long id)
        {
            ViewBag.clientIntno = id;
            ViewBag.salutationsList = LookupModels.getDictionary("salutation");
            return PartialView("_clientContactAddNew");
        }

        [HttpPost, ValidateInput(false)]
        [ValidateAntiForgeryToken]
        public ActionResult clientContactAddNew(contact item, long id)
        {
            var model = db.contacts;
            if (ModelState.IsValid)
            {
                try
                {
                    item.contactParty = "01";
                    item.partyIntno = id;
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

        public ActionResult clientContactUpdate(long id)
        {
            var model = db.contacts.FirstOrDefault(b => b.contactIntno == id);
            ViewBag.clientIntno = model.partyIntno;
            ViewBag.salutationsList = LookupModels.getDictionary("salutation");
            return PartialView("_clientContactUpdate", model);
        }

        [HttpPost, ValidateInput(false)]
        [ValidateAntiForgeryToken]
        public ActionResult clientContactUpdate(contact item)
        {
            var model = db.contacts;
            if (ModelState.IsValid)
            {
                try
                {
                    var modelItem = model.FirstOrDefault(b => b.contactIntno == item.contactIntno);
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
    }
}