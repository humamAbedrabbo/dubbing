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
    public class workLoadController : Controller
    {
        private DUBBDBEntities db = new DUBBDBEntities();
        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }

        public class assignment
        {
            public string taskType { get; set; }
            public long workIntno { get; set; }
            public short fromEpisode { get; set; }
            public short thruEpisode { get; set; }
            public DateTime? fromTimeCode { get; set; }
            public DateTime? thruTimeCode { get; set; }
            public long empIntno { get; set; }
            public DateTime? forDueDate { get; set; }
        }

        // GET: workLoad
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult assignmentsList()
        {
            var model = db.orderTrnDtls.Include(b => b.orderTrnHdr.agreementWork).Include(b => b.employee)
                        .Where(b => (b.activityType != "03" && b.activityType != "05") && b.status == false);
            return PartialView("_assignmentsList", model.ToList());
        }

        public ActionResult assignmentAddNew()
        {
            SelectList x = new SelectList(LookupModels.getDictionary("activityType"), "key", "value");
            ViewBag.activityTypesList = new SelectList(x.Where(b => b.Value != "03" && b.Value != "05"), "Value", "Text"); //remove Discharging and ME
            ViewBag.worksList = new SelectList(db.agreementWorks.Where(b => b.status == "01").Select(b => new { b.workIntno, b.workName }), "workIntno", "workName");
            ViewBag.resourcesList = new SelectList(db.employees.Where(b => b.empIntno == -1).Select(b => new { b.empIntno, b.fullName }), "empIntno", "fullName");
            return PartialView("_assignmentAddNew");
        }

        public JsonResult activityTypeChanged(string activityType)
        {
            var x = db.employees.Where(b => b.status == true);
            var model = x;
            if (activityType == "01") //translation
                model = x.Where(b => b.empType == "03");
            else if (activityType == "02") //adaptation
                model = x.Where(b => b.empType == "04" || b.empType == "02");
            else //studio supervisor
                model = x.Where(b => b.empType == "01");

            return Json(new SelectList(model.Select(b => new { b.empIntno, b.fullName }), "empIntno", "fullName"));
        }

        [ValidateAntiForgeryToken]
        [HttpPost, ValidateInput(false)]
        public ActionResult assignmentAddNew(assignment item)
        {
            var dtlModel = db.orderTrnDtls;
            var hdrModel = db.orderTrnHdrs;

            DateTime today = DateTime.Today.Date;
            DateTime? forDueDate;
            string fromTimeCode;
            string thruTimeCode;

            // validate input
            if (item.forDueDate.HasValue && item.forDueDate < today)
                forDueDate = today;
            else
                forDueDate = item.forDueDate;

            if (item.fromEpisode == item.thruEpisode)
            {
                if (item.fromTimeCode.HasValue && item.thruTimeCode.HasValue)
                {
                    fromTimeCode = item.fromTimeCode.Value.ToString("HH:mm:ss");
                    thruTimeCode = item.thruTimeCode.Value.ToString("HH:mm:ss");
                }
                else if (!item.fromTimeCode.HasValue && item.thruTimeCode.HasValue) // time control returns null when given zeros
                {
                    fromTimeCode = "00:00:00";
                    thruTimeCode = item.thruTimeCode.Value.ToString("HH:mm:ss");
                }
                else if (item.fromTimeCode.HasValue && !item.thruTimeCode.HasValue)
                {
                    fromTimeCode = item.fromTimeCode.Value.ToString("HH:mm:ss");
                    thruTimeCode = null;
                }
                else
                {
                    fromTimeCode = null;
                    thruTimeCode = null;
                }
            }
            else //remove wrong entry
            {
                fromTimeCode = null;
                thruTimeCode = null;
            }

            var orderItemsList = hdrModel.Where(b => b.workIntno == item.workIntno && b.episodeNo >= item.fromEpisode && b.episodeNo <= item.thruEpisode && b.status == "04").ToList();
            foreach (var hdr in orderItemsList)
            {
                orderTrnDtl dtl = new orderTrnDtl();
                dtl.activityType = item.taskType;
                dtl.orderTrnHdrIntno = hdr.orderTrnHdrIntno;
                dtl.empIntno = item.empIntno;
                dtl.fromTimeCode = fromTimeCode;
                dtl.thruTimeCode = thruTimeCode;
                dtl.assignedDate = today;
                dtl.forDueDate = forDueDate;
                dtl.status = false;
                dtlModel.Add(dtl);

                switch (item.taskType)
                {
                    case "01": //translation
                        hdr.startTranslation = today;
                        hdr.endTranslation = forDueDate;
                        break;
                    case "02": //adaptation
                        hdr.startAdaptation = today;
                        break;
                    case "04": //dubbing
                        hdr.startDubbing = today;
                        break;
                }
            }
            db.SaveChanges();

            return Content("Task Successfully Assigned.", "text/html");
        }

        public ActionResult assignmentUpdate(long id)
        {
            var model = db.orderTrnDtls.Find(id);
            string activityType = model.activityType;

            var oi = db.orderTrnHdrs.Include(b => b.agreementWork).FirstOrDefault(b => b.orderTrnHdrIntno == model.orderTrnHdrIntno);
            var x = db.employees.Where(b => b.status == true);
            var y = x;
            if (activityType == "01") //translation
                y = x.Where(b => b.empType == "03");
            else if (activityType == "02") //adaptation
                y = x.Where(b => b.empType == "04" || b.empType == "02");
            else //studio supervisor
                y = x.Where(b => b.empType == "01");

            ViewBag.workName = oi.agreementWork.workName;
            ViewBag.episodeNo = oi.episodeNo;
            ViewBag.resourcesList = new SelectList(y, "empIntno", "fullName");
            return PartialView("_assignmentUpdate", model);
        }

        [ValidateAntiForgeryToken]
        [HttpPost, ValidateInput(false)]
        public ActionResult assignmentUpdate(orderTrnDtl item)
        {
            var model = db.orderTrnDtls.Find(item.orderTrnDtlIntno);
            UpdateModel(model);
            db.SaveChanges();
            return Content("Task Successfully Assigned.", "text/html");
        }

        public ActionResult assignmentDelete(long id)
        {
            var model = db.orderTrnDtls;
            var modelItem = model.Find(id);
            model.Remove(modelItem);
            db.SaveChanges();
            return RedirectToAction("assignmentsList");
        }
    }
}