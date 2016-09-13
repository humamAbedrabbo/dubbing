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
                        .Where(b => (b.activityType != "03" && b.activityType != "05") && b.status == true);
            return PartialView("_assignmentsList", model.ToList());
        }

        public ActionResult resourceAssignmentsList(long empIntno, string empName, long workIntno, string workName)
        {
            var model = db.orderTrnDtls.Include(b => b.orderTrnHdr.agreementWork)
                        .Where(b => b.orderTrnHdr.workIntno == workIntno && b.empIntno == empIntno && b.status == true);
            ViewBag.workName = workName;
            ViewBag.empName = empName;
            return PartialView("_resourceAssignmentsList", model.ToList());
        }

        public ActionResult assignmentAddNew()
        {
            SelectList x = new SelectList(LookupModels.getDictionary("activityType"), "key", "value");
            ViewBag.activityTypesList = new SelectList(x.Where(b => b.Value != "03" && b.Value != "05"), "Value", "Text"); //remove Discharging and ME
            ViewBag.worksList = new SelectList(db.agreementWorks.Where(b => b.status == "01").Select(b => new { b.workIntno, b.workName }), "workIntno", "workName");
            ViewBag.resourcesList = new SelectList(db.employees.Where(b => b.empIntno == -1).Select(b => new { b.empIntno, b.fullName }), "empIntno", "fullName");
            return PartialView("_assignmentAddNew");
        }

        public JsonResult activityTypeChanged(string activityType, long? workIntno)
        {
            List<SelectListItem> rlist = new List<SelectListItem>();
            var x = db.employees.Where(b => b.status == true);
            var model = x;
            long empDefault = 0;
            var y = x.Where(b => b.workPersonnels.Where(d => d.workIntno == workIntno && d.status == true)
                                                        .Select(d => d.empIntno).Contains(b.empIntno));
            var y1 = y.FirstOrDefault();
            if (y.Count() != 0)
            {
                if (activityType == "01") //translation
                    y1 = y.FirstOrDefault(b => b.empType == "03");
                else if (activityType == "02") //adaptation
                    y1 = y.FirstOrDefault(b => b.empType == "04" || b.empType == "02");
                else //studio supervisor
                    y1 = y.FirstOrDefault(b => b.empType == "01");

                if (y1 != null)
                {
                    SelectListItem rscDefault = new SelectListItem();
                    rscDefault.Value = y1.empIntno.ToString();
                    rscDefault.Text = y1.fullName + " (Default)";
                    rscDefault.Selected = true;
                    empDefault = y1.empIntno;
                    rlist.Add(rscDefault);
                }
            }

            if (activityType == "01") //translation
                model = x.Where(b => b.empType == "03" && b.empIntno != empDefault);
            else if (activityType == "02") //adaptation
                model = x.Where(b => (b.empType == "04" || b.empType == "02") && b.empIntno != empDefault);
            else //studio supervisor
                model = x.Where(b => b.empType == "01" && b.empIntno != empDefault);
            foreach(var emp in model)
            {
                SelectListItem rsc = new SelectListItem();
                rsc.Value = emp.empIntno.ToString();
                rsc.Text = emp.fullName;
                rsc.Selected = false;
                rlist.Add(rsc);
            }
            return Json(rlist);
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
                dtl.status = true;
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
                        hdr.plannedDubbing = forDueDate.Value.AddDays(-6);
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
            ViewBag.workIntno = oi.workIntno;
            ViewBag.episodeNo = oi.episodeNo;
            ViewBag.resourcesList = new SelectList(y, "empIntno", "fullName");
            ViewBag.dtlIntno = id;
            return PartialView("_assignmentUpdate", model);
        }

        [ValidateAntiForgeryToken]
        [HttpPost, ValidateInput(false)]
        public ActionResult assignmentUpdate(orderTrnDtl item, long dtlIntno, long _workIntno, string _workName)
        {
            var model = db.orderTrnDtls.Include(b => b.employee).FirstOrDefault(b => b.orderTrnDtlIntno == dtlIntno);
            UpdateModel(model);
            db.SaveChanges();

            long empIntno1 = model.empIntno;
            string empName1 = model.employee.fullName;
            return RedirectToAction("resourceAssignmentsList", new { empIntno = empIntno1, empName = empName1, workIntno = _workIntno, workName = _workName });
        }

        public ActionResult assignmentDelete(long id)
        {
            var model = db.orderTrnDtls;
            var modelItem = model.Find(id);
            var model1 = db.studioEpisodes;

            var x = model1.Where(b => b.orderTrnDtlIntno == id).ToList();
            if (x.Count() != 0)
                model1.RemoveRange(x);
            model.Remove(modelItem);

            db.SaveChanges();
            return RedirectToAction("assignmentsList");
        }
        
        public ActionResult waitingList(string activityType)
        {
            var x = db.orderTrnHdrs.Include(b => b.agreementWork)
                                    .Where(b => b.status == "04" && b.agreementWork.status == "01"
                                    && !b.orderTrnDtls.Select(d => d.orderTrnHdrIntno).Contains(b.orderTrnHdrIntno));
            var model = x;

            List<string> wList = new List<string>();
            string epMin;
            string epMax;
            string activityType1;

            //translation
            if (activityType == "01" || string.IsNullOrEmpty(activityType))
            {
                activityType1 = string.IsNullOrEmpty(activityType) ? "01" : activityType;
                var y1 = db.orderTrnDtls.Where(d => d.activityType == activityType1 && d.status == false)
                        .Select(d => d.orderTrnHdrIntno).ToList();
                model = x.Where(b => !b.startTranslation.HasValue && !y1.Contains(b.orderTrnHdrIntno));
                var model1 = model.Select(b => new { b.workIntno, b.agreementWork.workName }).Distinct();
                if (model1 != null)
                {
                    wList.Add("Translation|xxx");
                    foreach (var w in model1)
                    {
                        epMin = model.Where(b => b.workIntno == w.workIntno).Select(b => b.episodeNo).Min().ToString();
                        epMax = model.Where(b => b.workIntno == w.workIntno).Select(b => b.episodeNo).Max().ToString();
                        wList.Add(w.workName + "|" + epMin + " - " + epMax);
                    }
                }
            }

            //adaptation
            if (activityType == "02" || string.IsNullOrEmpty(activityType))
            {
                activityType1 = string.IsNullOrEmpty(activityType) ? "02" : activityType;
                var y2 = db.orderTrnDtls.Where(d => d.activityType == activityType1 && d.status == false)
                        .Select(d => d.orderTrnHdrIntno).ToList();
                model = x.Where(b => !b.startAdaptation.HasValue && !y2.Contains(b.orderTrnHdrIntno));
                var model2 = model.Select(b => new { b.workIntno, b.agreementWork.workName }).Distinct();
                if (model2 != null)
                {
                    wList.Add("Adaptation|xxx");
                    foreach (var w in model2)
                    {
                        epMin = model.Where(b => b.workIntno == w.workIntno).Select(b => b.episodeNo).Min().ToString();
                        epMax = model.Where(b => b.workIntno == w.workIntno).Select(b => b.episodeNo).Max().ToString();
                        wList.Add(w.workName + "|" + epMin + " - " + epMax);
                    }
                }
            }

            //studio supervision
            if (activityType == "04" || string.IsNullOrEmpty(activityType))
            {
                activityType1 = string.IsNullOrEmpty(activityType) ? "04" : activityType;
                var y3 = db.orderTrnDtls.Where(d => d.activityType == activityType1 && d.status == false)
                        .Select(d => d.orderTrnHdrIntno).ToList();
                model = x.Where(b => !b.startDubbing.HasValue && !y3.Contains(b.orderTrnHdrIntno));
                var model3 = model.Select(b => new { b.workIntno, b.agreementWork.workName }).Distinct();
                if (model3 != null)
                {
                    wList.Add("Supervision|xxx");
                    foreach (var w in model3)
                    {
                        epMin = model.Where(b => b.workIntno == w.workIntno).Select(b => b.episodeNo).Min().ToString();
                        epMax = model.Where(b => b.workIntno == w.workIntno).Select(b => b.episodeNo).Max().ToString();
                        wList.Add(w.workName + "|" + epMin + " - " + epMax);
                    }
                }
            }
            
            return PartialView("_waitingList", wList);
        }
    }
}