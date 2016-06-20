﻿using ClosedXML.Excel;
using dubbingModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.UI;
using System.Web.UI.WebControls;

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

        public ActionResult actorsList()
        {
            var actorsList = new List<KeyValuePair<long, string>>();
            var model = db.payments.Where(b => b.voiceActorIntno.HasValue && b.isExported == false)
                        .Select(b => new { actorIntno = b.voiceActorIntno.Value , actorName = b.fullName })
                        .Union(db.paymentTemps.Where(b => !b.refPaymentIntno.HasValue).Select(b => new { actorIntno = b.personIntno, actorName = b.personName }));
            foreach(var x in model.Distinct())
            {
                long actorIntno = x.actorIntno;
                string actorName = x.actorName;
                var y = db.paymentTemps.Where(b => b.personIntno == actorIntno && b.personName == actorName && !b.refPaymentIntno.HasValue).ToList();
                int totalScenes = 0;
                if (y.Count() != 0)
                    totalScenes = y.Select(b => b.totalUnits).Sum();
                actorsList.Add(new KeyValuePair<long, string>(x.actorIntno, x.actorName + "|" + totalScenes.ToString()));
            }
            return PartialView("_actorsList", actorsList);
        }

        public ActionResult paymentsDueList(long id, string name)
        {
            var model = db.paymentTemps.Where(b => b.personIntno == id && b.personName == name);
            ViewBag.id = id;
            ViewBag.name = name;
            return PartialView("_paymentsDueList", model.ToList());
        }
        
        public ActionResult paidVouchersList()
        {
            var model = db.payments.Where(b => b.status == true && b.isPaid == true && b.isExported == false);
            return PartialView("_paidVouchersList", model.ToList());
        }

        public ActionResult paymentsList(long id, string name)
        {
            var model = db.payments.Where(b => b.voiceActorIntno == id  && b.fullName == name && b.status == true && b.isExported == false);
            return PartialView("_paymentsList", model.ToList());
        }

        public ActionResult paymentsAddNew(long id, string name)
        {
            var model = db.payments;
            var detailsModel = db.paymentDetails;

            var actor = db.voiceActors.Find(id);
            var tempModel = db.paymentTemps.Where(b => b.personIntno == id && b.personName == name);
            var worksList = tempModel.Select(b => b.workIntno).Distinct().ToList();
            foreach(long work in worksList)
            {
                var tempModel1 = db.paymentTemps.Where(b => b.workIntno == work && b.personIntno == id && b.personName == name);
                var charges = db.workCharges.FirstOrDefault(b => b.workIntno == work && b.workPartyType == "01" && b.workPartyIntno == id && b.status == true);
                var x = db.payments.FirstOrDefault(b => b.workIntno == work && b.voiceActorIntno == id && b.fullName == name);
                if (x != null) // do payment update
                {
                    x.totalScenes = tempModel1.Sum(b => b.totalUnits);
                    x.unitRate = int.Parse(charges.chargeAmount.ToString());
                    x.totalAmount = x.totalScenes * x.unitRate;
                    x.accountNo = db.employees.Find(id).accountNo == null ? "xxx" : db.employees.Find(id).accountNo;

                    long paymentIntno = x.paymentIntno;
                    foreach (var rec in tempModel1)
                    {
                        DateTime dubbDate = rec.trnDate;
                        var y = db.paymentDetails.FirstOrDefault(b => b.paymentIntno == paymentIntno && b.dubbingDate == dubbDate);
                        if (y != null) // do payment details update
                        {
                            y.totalUnits = rec.totalUnits;
                        }
                        else //insert new payment details
                        {
                            paymentDetail dtl = new paymentDetail();
                            dtl.paymentIntno = paymentIntno;
                            dtl.dubbingDate = rec.trnDate;
                            dtl.totalUnits = rec.totalUnits;
                            detailsModel.Add(dtl);
                        }
                        rec.refPaymentIntno = paymentIntno;
                    }
                    db.SaveChanges();
                }
                else //insert new payment and details
                {
                    payment pay = new payment();
                    pay.workIntno = work;
                    pay.voiceActorIntno = id;
                    pay.fullName = name;
                    pay.costCenterType = "01";
                    pay.totalScenes = tempModel1.Sum(b => b.totalUnits);
                    pay.unitRate = int.Parse(charges.chargeAmount.ToString());
                    pay.currencyCode = "01";
                    pay.deduction = 0;
                    pay.totalAmount = pay.totalScenes * pay.unitRate;
                    pay.accountNo = db.employees.Find(id).accountNo == null ? "xxx" : db.employees.Find(id).accountNo;
                    pay.status = true;
                    pay.isExported = false;
                    pay.isPaid = false;
                    model.Add(pay);
                    db.SaveChanges();

                    foreach (var rec in tempModel1)
                    {
                        paymentDetail dtl = new paymentDetail();
                        dtl.paymentIntno = pay.paymentIntno;
                        dtl.dubbingDate = rec.trnDate;
                        dtl.totalUnits = rec.totalUnits;
                        detailsModel.Add(dtl);
                        rec.refPaymentIntno = pay.paymentIntno;
                    }
                    db.SaveChanges();
                }
                
            }
            
            long id1 = id;
            string name1 = name;

            return RedirectToAction("paymentsList", new { id = id1, name = name1 });
        }

        public ActionResult paymentsUpdate(long id)
        {
            var model = db.payments.FirstOrDefault(b => b.paymentIntno == id);
            return PartialView("_paymentsUpdate", model);
        }

        [ValidateAntiForgeryToken]
        [HttpPost, ValidateInput(false)]
        public ActionResult paymentsUpdate(payment item, int updateBtn)
        {
            var model = db.payments;
            var dtlModel = db.paymentDetails;
            var tempModel = db.paymentTemps;
            if (ModelState.IsValid)
            {
                var modelItem = model.Find(item.paymentIntno);
                try
                {
                    switch (updateBtn)
                    {
                        case 1: //update payment
                            this.UpdateModel(modelItem);
                            break;
                        case 2: //endorse payment
                            modelItem.isPaid = true;
                            var temp = tempModel.Where(b => b.refPaymentIntno == item.paymentIntno);
                            tempModel.RemoveRange(temp);
                            break;
                        case 3: //delete payment
                            var x = dtlModel.Where(b => b.paymentIntno == item.paymentIntno);
                            dtlModel.RemoveRange(x);
                            model.Remove(modelItem);
                            var y = tempModel.Where(b => b.refPaymentIntno == item.paymentIntno);
                            foreach(var y1 in y)
                            {
                                y1.refPaymentIntno = null;
                            }
                            break;
                    }
                    db.SaveChanges();
                }
                catch(Exception e)
                {
                    return Content("Failed to Perform the Requested Operation. " + e.Message, "text/html");
                }
            }
            else
                return Content("Failed! Please Enter All Data. ", "text/html");
            return Content("Successfully Updated. ", "text/html");
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
    }
}