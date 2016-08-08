using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using dubbingModel;
using dubbingApp.Models;

namespace dubbingApp.Controllers
{
    [Authorize(Roles = "ADMIN")]
    public class dubbDomainsController : Controller
    {
        private DUBBDBEntities db = new DUBBDBEntities();

        // GET: dubbDomains
        public ActionResult Index(string domainName)
        {
            var model = staticCache.getDomainValuesList();
            ViewBag.domainsList = model.Select(b => b.domainName).Distinct();
            return View();
        }

        public ActionResult domainsList(string domainName)
        {
            var model = staticCache.getDomainValuesList().Where(b => (string.IsNullOrEmpty(domainName) || b.domainName == domainName));
            return PartialView("domainsList", model);
        }

        // GET: dubbDomains/Details/5
        public ActionResult Details(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            dubbDomain dubbDomain = db.dubbDomains.Find(id);
            if (dubbDomain == null)
            {
                return HttpNotFound();
            }
            return View(dubbDomain);
        }

        // GET: dubbDomains/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: dubbDomains/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "domainIntno,domainName,domainCode,domainValue,userMessage,valueUsage,langCode,sortOrder,minAccessLevel,status")] dubbDomain dubbDomain)
        {
            if (ModelState.IsValid)
            {
                db.dubbDomains.Add(dubbDomain);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(dubbDomain);
        }

        // GET: dubbDomains/Edit/5
        public ActionResult Edit(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            dubbDomain dubbDomain = db.dubbDomains.Find(id);
            if (dubbDomain == null)
            {
                return HttpNotFound();
            }
            return View(dubbDomain);
        }

        // POST: dubbDomains/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "domainIntno,domainName,domainCode,domainValue,userMessage,valueUsage,langCode,sortOrder,minAccessLevel,status")] dubbDomain dubbDomain)
        {
            if (ModelState.IsValid)
            {
                db.Entry(dubbDomain).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(dubbDomain);
        }

        // GET: dubbDomains/Delete/5
        public ActionResult Delete(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            dubbDomain dubbDomain = db.dubbDomains.Find(id);
            if (dubbDomain == null)
            {
                return HttpNotFound();
            }
            return View(dubbDomain);
        }

        // POST: dubbDomains/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(long id)
        {
            dubbDomain dubbDomain = db.dubbDomains.Find(id);
            db.dubbDomains.Remove(dubbDomain);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
