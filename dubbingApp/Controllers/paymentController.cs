using ClosedXML.Excel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Data.Entity;
using System.Web;
using System.Web.Mvc;
using System.Web.UI;
using System.Web.UI.WebControls;
using dubbingModel;
using dubbingApp.Models;

namespace dubbingApp.Controllers
{
    [Authorize(Roles = "ADMIN, GENERAL_MANAGER, FINANCE_MANAGER")]
    public class paymentController : Controller
    {
        private DUBBDBEntities db = new DUBBDBEntities();
        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }

        // GET: payment
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult paymentsDueList()
        {
            var model = db.paymentsDueVWs;
            return PartialView("_paymentsDueList", model.ToList());
        }
        
        public ActionResult paidVouchersList()
        {
            var model = db.payments.Where(b => b.status == true && b.isPaid == true && b.isExported == false);
            return PartialView("_paidVouchersList", model.ToList());
        }

        public ActionResult paymentsAddNew(long workIntno, string workName, long rscId, string rscName)
        {
            payment model = new payment();
            int totalUnits = db.paymentsDueVWs.Where(b => b.workIntno == workIntno && b.voiceActorIntno == rscId && b.actorName == rscName)
                                                .Sum(b => b.totalUnits).Value;

            int unitRate = 0;
            string currencyCode = null;
            var charges = db.workCharges.FirstOrDefault(b => b.workIntno == workIntno && b.workPartyType == "01" && b.workPartyIntno == rscId && b.status == true);
            if (charges != null)
            {
                unitRate = (int)charges.chargeAmount;
                currencyCode = charges.currencyCode;
            }
            var rsc = db.voiceActors.Find(rscId);

            model.paymentDate = DateTime.Now.Date;
            model.workIntno = workIntno;
            model.voiceActorIntno = rscId;
            model.fullName = rscName;
            model.costCenterType = "01";
            model.totalScenes = totalUnits;
            model.unitRate = unitRate;
            model.currencyCode = currencyCode;
            model.deduction = 0;
            model.totalAmount = totalUnits * unitRate;
            model.accountNo = rsc == null ? "---" : rsc.accountNo;

            ViewBag.costCenterType = LookupModels.decodeDictionaryItem("costCenterType", model.costCenterType);
            ViewBag.currenciesList = new SelectList(LookupModels.getDictionary("currencyCode"), "key", "value");
            if (!string.IsNullOrEmpty(currencyCode))
                ViewBag.currency = LookupModels.decodeDictionaryItem("currencyCode", currencyCode);
            else
                ViewBag.currency = "";
            ViewBag.workName = workName;
            return PartialView("_paymentsAddNew", model);
        }

        [ValidateAntiForgeryToken]
        [HttpPost, ValidateInput(false)]
        public ActionResult paymentsAddNew(payment item)
        {
            var model = db.payments;
            var dtlModel = db.paymentDetails;
            if (ModelState.IsValid)
            {
                item.status = true;
                item.isExported = false;
                item.isPaid = true;
                model.Add(item);
                
                //insert payment details
                var x = db.paymentsDueVWs.Where(b => b.workIntno == item.workIntno && b.voiceActorIntno == item.voiceActorIntno && b.actorName == item.fullName).ToList();
                foreach(var x1 in x)
                {
                    paymentDetail dtl = new paymentDetail();
                    dtl.dubbingDate = x1.dubbingDate.Value;
                    dtl.totalUnits = x1.totalUnits.Value;
                    dtlModel.Add(dtl);
                }
                db.SaveChanges();
            }

            return RedirectToAction("paymentsDueList");
        }

        public ActionResult exportToExcel()
        {
            var model = db.payments.Where(b => b.status == true && b.isPaid == true && b.isExported == false);
            var grid = new GridView();
            var exportModel = model.Select(b => new { Actor = b.fullName, AccountNo = b.accountNo, CostCenter = b.agreementWork.workName, TotalScenes = b.totalScenes, TotalAmount = b.totalAmount, PaymentDate = b.paymentDate });

            grid.DataSource = exportModel.ToList();
            grid.DataBind();

            Response.ClearContent();
            Response.Buffer = true;
            Response.AddHeader("content-disposition", "attachment; filename=Payments-" + DateTime.Now.ToString() + ".xls");
            //Response.AppendHeader("ContentType", "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
            Response.AppendHeader("ContentType", "application/vnd.ms-excel");

            Response.Charset = "UTF-8";
            StringWriter objStringWriter = new StringWriter();
            HtmlTextWriter objHtmlTextWriter = new HtmlTextWriter(objStringWriter);

            grid.RenderControl(objHtmlTextWriter);

            Response.Output.Write(objStringWriter.ToString());
            Response.Flush();
            Response.End();

            foreach (var x in model)
            {
                x.isExported = true;
            }
            db.SaveChanges();
            return null;
        }

        public ActionResult printVouchersList(long actorId, string actorName)
        {
            var model = db.paymentDetails.Include(b => b.payment.agreementWork)
                        .Where(b => b.payment.voiceActorIntno == actorId && b.payment.fullName == actorName && b.payment.status == true && b.payment.isPaid == true && b.payment.isExported == false);
            ViewBag.actorName = actorName;
            return PartialView("_printVouchersList", model.ToList());
        }
    }
}