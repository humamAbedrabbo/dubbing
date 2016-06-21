using dubbingModel;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace dubbingApp.Controllers
{
    public class adaptationController : Controller
    {
        DUBBDBEntities ctx = new DUBBDBEntities();

        // GET: adaptations
        public ActionResult Index()
        {
            // Get list of episodes where adaptation is in progress
            var model = ctx.orderTrnHdrs.Include(x => x.agreementWork).Where(x => x.startAdaptation.HasValue && !x.endAdaptation.HasValue).ToList();
            return View(model);
        }

        public ActionResult Edit(int? id)
        {
            var model = ctx.orderTrnHdrs.Include(x => x.agreementWork).First(x => x.orderIntno == id);
            var scenes = model.dubbingSheetDtls.Select(x => x.sceneNo).Distinct().OrderBy(x => x);
            /*
             * 1- select scenes of the selected episode ordered by seqNo
             * 2- 
             */ 

            return View(model);
        }

        
        public ActionResult sceneList(long orderTrnHdrIntno)
        {
            var ordHdr = ctx.orderTrnHdrs.Include(x => x.agreementWork).First(x => x.orderTrnHdrIntno == orderTrnHdrIntno);
            var model = new List<short>();
            if (ordHdr.dubbingSheetHdrs.Count() > 0)
            {
                model = ordHdr.dubbingSheetDtls.Select(x => x.sceneNo).Distinct().OrderBy(x => x).ToList();
            }


            ViewBag.orderTrnHdrIntno = orderTrnHdrIntno;
            return PartialView("_sceneList", model);
            
        }

        public ActionResult  addScene(long orderTrnHdrIntno)
        {
            var ordTrnHdr = ctx.orderTrnHdrs.Include(x => x.agreementWork).First(x => x.orderTrnHdrIntno == orderTrnHdrIntno);
            short sceneNo = 1;
            if (ordTrnHdr.dubbingSheetDtls.Count() > 0)
            {
                sceneNo = ordTrnHdr.dubbingSheetDtls.Select(x => x.sceneNo).Max() ;
                sceneNo++;
            }

            var sheetHdr = ordTrnHdr.dubbingSheetHdrs.Where(x => x.orderTrnHdrIntno == orderTrnHdrIntno && x.characterName == "UNKNOWN").FirstOrDefault();
            if (sheetHdr == null)
            {
                sheetHdr = ctx.dubbingSheetHdrs.Create();
                sheetHdr.orderTrnHdrIntno = orderTrnHdrIntno;
                sheetHdr.voiceActorIntno = 0;
                sheetHdr.actorName = "ANONYMOUS";
                sheetHdr.characterName = "UNKNOWN";
                ctx.dubbingSheetHdrs.Add(sheetHdr);
                ctx.SaveChanges();
            }

            var sheetDtl = ctx.dubbingSheetDtls.Create();
            sheetDtl.dubbSheetHdrIntno = sheetHdr.dubbSheetHdrIntno;
            sheetDtl.orderTrnHdrIntno = orderTrnHdrIntno;
            sheetDtl.sceneNo = sceneNo;
            sheetDtl.isTaken = false;
            sheetDtl.startTimeCode = "00:00:00";
            ctx.dubbingSheetDtls.Add(sheetDtl);
            ctx.SaveChanges();

            var lastDialogNo = ctx.adaptationDialogs.Where(x => x.dubbSheetHdrIntno == sheetHdr.dubbSheetHdrIntno && x.sceneNo == sceneNo).Select(x => x.dialogNo).Max();
            if (lastDialogNo == null || lastDialogNo == 0)
                lastDialogNo = 1;
            else
                lastDialogNo++;

            var dialog = ctx.adaptationDialogs.Create();
            dialog.dialogNo = lastDialogNo;
            dialog.dubbSheetHdrIntno = sheetHdr.dubbSheetHdrIntno;
            dialog.sceneNo = sceneNo;
            ctx.adaptationDialogs.Add(dialog);
            ctx.SaveChanges();

            var subtitle = ctx.adaptationSubtitles.Create();
            subtitle.dialogIntno = dialog.dialogIntno;
            subtitle.subtitleNo = 1;
            subtitle.scentence = "جملة جديدة";
            subtitle.startTime = "00:00:00";
            subtitle.endTime = "00:00:00";
            ctx.adaptationSubtitles.Add(subtitle);
            ctx.SaveChanges();



            return sceneList(orderTrnHdrIntno);
        }

        public ActionResult deleteScene(long orderTrnHdrIntno, short sceneNo)
        {
            var ordTrnHdr = ctx.orderTrnHdrs.Include(x => x.agreementWork).First(x => x.orderTrnHdrIntno == orderTrnHdrIntno);
            
            var sheetDtls = ordTrnHdr.dubbingSheetDtls.Where(x => x.sceneNo == sceneNo);
            var parentSheetHdrs = sheetDtls.Select(x => x.dubbSheetHdrIntno).Distinct().ToList();

            ctx.dubbingSheetDtls.RemoveRange(sheetDtls);
            ctx.SaveChanges();

            //delete dialogs
            var dialogs = ctx.adaptationDialogs.Where(x => parentSheetHdrs.Contains(x.dubbSheetHdrIntno) && x.sceneNo == sceneNo);
            ctx.adaptationDialogs.RemoveRange(dialogs);
            ctx.SaveChanges();

            // renumber dialogs
            foreach (var h in parentSheetHdrs)
            {
                RenumberDialogs(h, sceneNo);
            }

            // decrease sceneNo for next scenes
            sheetDtls = ordTrnHdr.dubbingSheetDtls.Where(x => x.sceneNo > sceneNo);
            foreach(var item in sheetDtls)
            {
                var childDialogs = ctx.adaptationDialogs.Where(x => x.dubbSheetHdrIntno == item.dubbSheetHdrIntno && x.sceneNo > item.sceneNo);
                item.sceneNo--;
                foreach(var d in childDialogs)
                {
                    d.sceneNo--;
                }
            }
            ctx.SaveChanges();



            // delete empty sheet headers
            var sheetHdrs = ordTrnHdr.dubbingSheetHdrs.Where(x => x.dubbingSheetDtls.Count() == 0);
            //var hdrsIdList = sheetHdrs.Select(x => x.dubbSheetHdrIntno).ToList();
            
            //ctx.adaptationDialogs.RemoveRange(dialogs);
            ctx.dubbingSheetHdrs.RemoveRange(sheetHdrs);
            ctx.SaveChanges();


            return sceneList(orderTrnHdrIntno);
        }

        public void RenumberDialogs(long sheetHdrIntno, short sceneNo)
        {
            var dialogs = ctx.adaptationDialogs.Where(x => x.dubbSheetHdrIntno == sheetHdrIntno && x.sceneNo == sceneNo).OrderBy(x => x.dialogNo);
            short dialogNo = 1;
            foreach(var d in dialogs)
            {
                d.dialogNo = dialogNo;
                dialogNo++;
            }
            ctx.SaveChanges();
        }

        public ActionResult dialogList(long orderTrnHdrIntno ,short sceneNo = 0)
        {
            var ordTrnHdr = ctx.orderTrnHdrs.Include(x => x.agreementWork).First(x => x.orderTrnHdrIntno == orderTrnHdrIntno);
            var sheetHdrs = ordTrnHdr.dubbingSheetHdrs.Select(x => x.dubbSheetHdrIntno).ToList();
            var dialogs = ctx.adaptationDialogs.Include(x => x.dubbingSheetHdr).Where(x => sheetHdrs.Contains(x.dubbSheetHdrIntno) && x.sceneNo == sceneNo).OrderBy(x => x.dialogNo).ToList();
            ViewBag.SceneNo = sceneNo;
            return PartialView("_dialogList", dialogs);
        }

        public ActionResult addDialog(long orderTrnHdrIntno, short sceneNo)
        {
            var ordTrnHdr = ctx.orderTrnHdrs.Include(x => x.agreementWork).First(x => x.orderTrnHdrIntno == orderTrnHdrIntno);
            var sheetHdr = ordTrnHdr.dubbingSheetHdrs.Where(x => x.orderTrnHdrIntno == orderTrnHdrIntno && x.characterName == "UNKNOWN").FirstOrDefault();
            if (sheetHdr == null)
            {
                sheetHdr = ctx.dubbingSheetHdrs.Create();
                sheetHdr.orderTrnHdrIntno = orderTrnHdrIntno;
                sheetHdr.voiceActorIntno = 0;
                sheetHdr.actorName = "ANONYMOUS";
                sheetHdr.characterName = "UNKNOWN";
                ctx.dubbingSheetHdrs.Add(sheetHdr);
                ctx.SaveChanges();
            }

            var scene = sheetHdr.dubbingSheetDtls.Where(x => x.dubbSheetHdrIntno == sheetHdr.dubbSheetHdrIntno && x.sceneNo == sceneNo).FirstOrDefault();
            if (scene == null)
            {
                scene = ctx.dubbingSheetDtls.Create();
                scene.orderTrnHdrIntno = orderTrnHdrIntno;
                scene.sceneNo = sceneNo;
                scene.dubbSheetHdrIntno = sheetHdr.dubbSheetHdrIntno;
                scene.isTaken = false;
                scene.startTimeCode = "00:00:00";

                ctx.dubbingSheetDtls.Add(scene);
                ctx.SaveChanges();
            }

            var lastDialogNo = ctx.adaptationDialogs.Where(x => x.dubbSheetHdrIntno == sheetHdr.dubbSheetHdrIntno && x.sceneNo == sceneNo).Select(x => x.dialogNo).Max();
            if (lastDialogNo == null || lastDialogNo == 0)
                lastDialogNo = 1;
            else
                lastDialogNo++;

            var dialog = ctx.adaptationDialogs.Create();
            dialog.dialogNo = lastDialogNo;
            dialog.dubbSheetHdrIntno = sheetHdr.dubbSheetHdrIntno;
            dialog.sceneNo = sceneNo;
            ctx.adaptationDialogs.Add(dialog);
            ctx.SaveChanges();

            var subtitle = ctx.adaptationSubtitles.Create();
            subtitle.dialogIntno = dialog.dialogIntno;
            subtitle.subtitleNo = 1;
            subtitle.scentence = "جملة جديدة";
            subtitle.startTime = "00:00:00";
            subtitle.endTime = "00:00:00";
            ctx.adaptationSubtitles.Add(subtitle);
            ctx.SaveChanges();

            return dialogList(orderTrnHdrIntno, sceneNo);
        }

        public ActionResult deleteDialog(long dialogIntno)
        {
            var dialog = ctx.adaptationDialogs.Find(dialogIntno);
            var sheetHdrIntno = dialog.dubbSheetHdrIntno;
            var sceneNo = dialog.sceneNo;
            var dubSheet = ctx.dubbingSheetHdrs.Find(sheetHdrIntno);
            var orderTrnHdrIntno = dubSheet.orderTrnHdrIntno;
            ctx.adaptationDialogs.Remove(dialog);
            ctx.SaveChanges();
            RenumberDialogs(sheetHdrIntno, sceneNo);

            return dialogList(orderTrnHdrIntno, sceneNo);
        }

        public ActionResult subtitleList(long dialogIntno)
        {
            var subtitles = ctx.adaptationSubtitles.Where(x => x.dialogIntno == dialogIntno).OrderBy(x => x.subtitleNo).ToList();
            return PartialView("_subtitlesList", subtitles);
        }

        public ActionResult addSubtitle(long dialogIntno)
        {
            var dialog = ctx.adaptationDialogs.Find(dialogIntno);
            short lastNo = 0 ;
            if (dialog.adaptationSubtitles.Count() > 0)
            {
                lastNo = dialog.adaptationSubtitles.Select(x => x.subtitleNo.Value).Max();
                lastNo++;
            }
            else
                lastNo = 1;
            var subtitle = ctx.adaptationSubtitles.Create();
            subtitle.dialogIntno = dialogIntno;
            subtitle.scentence = "جملة جديدة";
            subtitle.subtitleNo = lastNo;
            subtitle.startTime = "00:00:00";
            subtitle.endTime = "00:00:00";
            dialog.adaptationSubtitles.Add(subtitle);
            ctx.SaveChanges();

            return subtitleList(dialogIntno);
        }

        public ActionResult deleteSubtitle(long subtitleIntno)
        {
            var subtitle = ctx.adaptationSubtitles.Find(subtitleIntno);
            var dialogIntno = subtitle.dialogIntno;
            var subtitleNo = subtitle.subtitleNo;

            ctx.adaptationSubtitles.Remove(subtitle);
            var subtitles = ctx.adaptationSubtitles.Where(x => x.dialogIntno == dialogIntno && x.subtitleNo > subtitleNo).OrderBy(x => x.subtitleNo);
            foreach(var s in subtitles)
            {
                s.subtitleNo--;
            }
            ctx.SaveChanges();
            return subtitleList(dialogIntno);
        }
    }
}