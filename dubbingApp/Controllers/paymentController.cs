using System;
using System.Linq;
using System.Data.Entity;
using System.Web.Mvc;
using dubbingModel;
using dubbingApp.Models;
using System.Data;
using System.Reflection;
using ClosedXML.Excel;
using System.IO;
using System.Web;

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
            //find discharge tables with entries having all scenes taken - no scene left
            //List<long> IDs = new List<long>();
            //var hdrs = db.dubbingSheetHdrs.Where(b => b.isPaid == false).Select(b => b.dubbSheetHdrIntno).ToList();
            //foreach(long hdr1 in hdrs)
            //{
            //    int x = db.subtitles.Include(b => b.dialog).Where(b => b.dubbSheetHdrIntno == hdr1)
            //            .Select(b => b.dialog.sceneIntno).Distinct().Count();
            //    int y = db.dubbingSheetDtls.Where(b => b.dubbSheetHdrIntno == hdr1 && b.isTaken == true).Count();

            //    if (x == y)
            //    {
            //        IDs.Add(hdr1);
            //    }
            //}

            //var model = db.dubbingSheetDtls.Include(b => b.dubbingSheetHdr).Include(b => b.orderTrnHdr.agreementWork)
            //            .Where(b => IDs.Contains(b.dubbSheetHdrIntno));

            var model = (from A in db.dubbingSheetDtls
                         join B in db.dubbingSheetHdrs on A.dubbSheetHdrIntno equals B.dubbSheetHdrIntno
                         where B.isPaid == false && A.isTaken == true
                         select A).Include(b => b.dubbingSheetHdr).Include(b => b.orderTrnHdr.agreementWork);
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
            int totalUnits = (from A in db.dubbingSheetDtls
                             join B in db.dubbingSheetHdrs on A.dubbSheetHdrIntno equals B.dubbSheetHdrIntno
                             join C in db.orderTrnHdrs on B.orderTrnHdrIntno equals C.orderTrnHdrIntno
                             where B.isPaid == false && A.isTaken == true && C.workIntno == workIntno && B.voiceActorIntno == rscId
                             select A).Count();

            int unitRate = 0;
            string currencyCode = "01";
            var charges = db.workCharges.FirstOrDefault(b => b.workIntno == workIntno && b.workPartyType == "01" && b.workPartyIntno == rscId && b.status == true);
            if (charges != null)
            {
                unitRate = (int)charges.chargeAmount;
                if(!string.IsNullOrEmpty(charges.currencyCode))
                    currencyCode = charges.currencyCode;
            }
            var rsc = db.voiceActors.Find(rscId);
            
            model.paymentDate = DateTime.Today.Date;
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
                item.paymentDate = DateTime.Today.Date;
                item.status = true;
                item.isExported = false;
                item.isPaid = true;
                model.Add(item);
                db.SaveChanges();

                //insert payment details
                var x = (from A in db.dubbingSheetDtls
                         join B in db.dubbingSheetHdrs on A.dubbSheetHdrIntno equals B.dubbSheetHdrIntno
                         join C in db.orderTrnHdrs on B.orderTrnHdrIntno equals C.orderTrnHdrIntno
                         where B.isPaid == false && A.isTaken == true && C.workIntno == item.workIntno && B.voiceActorIntno == item.voiceActorIntno && B.actorName == item.fullName
                         select A).ToList();
                var d = x.Select(b => b.dubbingDate).Distinct().ToList();
                foreach (DateTime x1 in d)
                {
                    paymentDetail dtl = new paymentDetail();
                    dtl.paymentIntno = item.paymentIntno;
                    dtl.dubbingDate = x1;
                    dtl.totalUnits = x.Where(b => b.dubbingDate == x1).Count();
                    dtlModel.Add(dtl);
                }

                db.SaveChanges();

                var ds = x.Select(b => b.dubbSheetHdrIntno).Distinct().ToList();
                foreach(long ds1 in ds)
                {
                    var hdr = db.dubbingSheetHdrs.FirstOrDefault(b => b.dubbSheetHdrIntno == ds1);
                    hdr.isPaid = true;
                }

                db.SaveChanges();
            }

            return RedirectToAction("paymentsDueList");
        }

        public void exportToExcel()
        {
            var model = db.payments.Include(b => b.agreementWork).Where(b => b.status == true && b.isPaid == true && b.isExported == false).ToList();
            DataTable dt = new DataTable();
            dt.Columns.AddRange(new DataColumn[6] { new DataColumn("Actor", typeof(string)),
                                                    new DataColumn("AccountNo", typeof(string)),
                                                    new DataColumn("Cost Center",typeof(string)),
                                                    new DataColumn("Total Scenes",typeof(int)),
                                                    new DataColumn("Total Amount",typeof(int)),
                                                    new DataColumn("Payment Date",typeof(DateTime)) });
            foreach(var payment in model)
            {
                dt.Rows.Add(payment.fullName, payment.accountNo, payment.agreementWork.workName, payment.totalScenes, payment.totalAmount, payment.paymentDate);
                payment.isExported = true;
            }

            var wb = new XLWorkbook();
            wb.Worksheets.Add(dt, "Payments");

            string file1 = "Payments" + "_" + DateTime.Now.ToShortDateString() + ".xlsx";
            Response.Clear();
            Response.Buffer = true;
            Response.AddHeader("content-disposition", "attachment; filename=" + file1);
            Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            MemoryStream stream = GetStream(wb);// The method is defined below
            Response.BinaryWrite(stream.ToArray());
            Response.End();

            db.SaveChanges();
        }

        public ActionResult printVouchersList(long actorId, string actorName)
        {
            var model = db.paymentDetails.Include(b => b.payment.agreementWork)
                        .Where(b => b.payment.voiceActorIntno == actorId && b.payment.fullName == actorName && b.payment.status == true && b.payment.isPaid == true && b.payment.isExported == false);
            ViewBag.actorName = actorName;
            return PartialView("_printVouchersList", model.ToList());
        }

        public MemoryStream GetStream(XLWorkbook excelWorkbook)
        {
            MemoryStream fs = new MemoryStream();
            excelWorkbook.SaveAs(fs);
            fs.Position = 0;
            return fs;
        }
    }
}