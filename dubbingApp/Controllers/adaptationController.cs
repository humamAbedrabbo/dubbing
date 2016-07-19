﻿using dubbingModel;
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

            return View(model);
        }

       public ActionResult sceneList(long orderTrnHdrIntno)
        {
            var order = ctx.orderTrnHdrs.Include(x => x.agreementWork).First(x => x.orderTrnHdrIntno == orderTrnHdrIntno);
            var model = order.scenes.OrderBy(x => x.sceneNo).ToList();
            ViewBag.orderTrnHdrIntno = orderTrnHdrIntno;

            return PartialView("_sceneList", model);            
        }

        public ActionResult sceneDetails(long sceneIntno)
        {
            var model = ctx.scenes.Find(sceneIntno);

            return PartialView("_sceneDetails", model);
        }

        public ActionResult addScene(long orderTrnHdrIntno)
        {
            var order = ctx.orderTrnHdrs.Include(x => x.agreementWork).First(x => x.orderTrnHdrIntno == orderTrnHdrIntno);
            short sceneNo = 1;

            if(order.scenes.Count() > 0)
            {
                sceneNo = order.scenes.Max(x => x.sceneNo);
                sceneNo++;
            }

            var newScene = ctx.scenes.Create();
            newScene.orderTrnHdrIntno = orderTrnHdrIntno;
            newScene.sceneNo = sceneNo;
            ctx.scenes.Add(newScene);
            ctx.SaveChanges();

            // add default dialog to the new scene
            var newDialog = ctx.dialogs.Create();
            newDialog.sceneIntno = newScene.sceneIntno;
            newDialog.dialogNo = 1;
            ctx.dialogs.Add(newDialog);
            ctx.SaveChanges();

            // add default subtitle to the new dialog
            var newSubtitle = ctx.subtitles.Create();
            newSubtitle.dialogIntno = newDialog.dialogIntno;
            newSubtitle.subtitleNo = 1;
            newSubtitle.scentence = "جملة جديدة";
            newSubtitle.startTimeCode = "00:00:00";
            newSubtitle.endTimeCode = "00:00:00";
            newSubtitle.dubbSheetHdrIntno = getDefaultCharacter(newDialog.scene.orderTrnHdrIntno);
            ctx.subtitles.Add(newSubtitle);
            ctx.SaveChanges();

            return sceneList(orderTrnHdrIntno);
        }

        public ActionResult deleteScene(long orderTrnHdrIntno, short sceneNo)
        {
            var scenes = ctx.scenes.Where(x => x.orderTrnHdrIntno == orderTrnHdrIntno && x.sceneNo == sceneNo);
            ctx.scenes.RemoveRange(scenes);
            ctx.SaveChanges();
            renumberScenes(orderTrnHdrIntno, sceneNo);
            return sceneList(orderTrnHdrIntno);
        }

        public ActionResult saveSceneTimeCode(long sceneIntno, string startTimeCode, string endTimeCode)
        {
            var scene = ctx.scenes.Find(sceneIntno);
            scene.startTimeCode = startTimeCode;
            scene.endTimeCode = endTimeCode;
            ctx.SaveChanges();
            return PartialView("_sceneDetails", scene);
        }

        private void renumberScenes(long orderTrnHdrIntno, short sceneNo)
        {
            var scenes = ctx.scenes.Where(x => x.orderTrnHdrIntno == orderTrnHdrIntno && x.sceneNo > sceneNo).OrderBy(x => x.sceneNo);
            foreach(var item in scenes)
            {
                item.sceneNo--;
            }
            ctx.SaveChanges();
        }

        public ActionResult dialogList(long sceneIntno)
        {
            var model = ctx.dialogs.Where(x => x.sceneIntno == sceneIntno).OrderBy(x => x.dialogNo).ToList();
            return PartialView("_dialogList", model);
        }

        public ActionResult dialogDetails(dialog dialog)
        {
            
            return PartialView("_dialogDetails", dialog);
        }

        public ActionResult addDialog(long sceneIntno)
        {
            var scene = ctx.scenes.Find(sceneIntno);
            short dialogNo = 1;
            if(scene.dialogs.Count() > 0)
            {
                dialogNo = scene.dialogs.Max(x => x.dialogNo);
                dialogNo++;
            }

            var newDialog = ctx.dialogs.Create();
            newDialog.sceneIntno = sceneIntno;
            newDialog.dialogNo = dialogNo;
            ctx.dialogs.Add(newDialog);
            ctx.SaveChanges();

            // add default subtitle to the new dialog
            var newSubtitle = ctx.subtitles.Create();
            newSubtitle.dialogIntno = newDialog.dialogIntno;
            newSubtitle.subtitleNo = 1;
            newSubtitle.scentence = "جملة جديدة";
            newSubtitle.startTimeCode = "00:00:00";
            newSubtitle.endTimeCode = "00:00:00";
            newSubtitle.dubbSheetHdrIntno = getDefaultCharacter(newDialog.scene.orderTrnHdrIntno);
            ctx.subtitles.Add(newSubtitle);
            ctx.SaveChanges();

            return dialogList(sceneIntno);
        }

        public ActionResult deleteDialog(long dialogIntno)
        {
            var dialog = ctx.dialogs.Find(dialogIntno);
            long sceneIntno = dialog.sceneIntno;
            short dialogNo = dialog.dialogNo;
            ctx.dialogs.Remove(dialog);
            ctx.SaveChanges();
            renumberDialogs(sceneIntno, dialogNo);

            return dialogList(sceneIntno);
        }

        private void renumberDialogs(long sceneIntno, short dialogNo)
        {
            var dialogs = ctx.dialogs.Where(x => x.sceneIntno == sceneIntno && x.dialogNo > dialogNo).OrderBy(x => x.dialogNo);
            foreach (var item in dialogs)
            {
                item.dialogNo--;
            }
            ctx.SaveChanges();
        }

        public ActionResult saveDialogTimeCode(long dialogIntno, string startTimeCode, string endTimeCode)
        {
            var dialog = ctx.dialogs.Find(dialogIntno);
            dialog.startTimeCode = startTimeCode;
            dialog.endTimeCode = endTimeCode;
            ctx.SaveChanges();
            return dialogDetails(dialog);
        }

        public ActionResult subtitlesList(long dialogIntno)
        {
            var model = ctx.subtitles.Where(x => x.dialogIntno == dialogIntno).OrderBy(x => x.subtitleNo).ToList();
            return PartialView("_subtitlesList", model);
        }

        public ActionResult subtitleDetails(subtitle subtitle)
        {

            return PartialView("_subtitleDetails", subtitle);
        }

        public ActionResult addSubtitle(long dialogIntno)
        {
            var dialog = ctx.dialogs.Find(dialogIntno);
            short subtitleNo = 1;
            if(dialog.subtitles.Count() > 0)
            {
                subtitleNo = dialog.subtitles.Max(x => x.subtitleNo);
                subtitleNo++;
            }

            var newSubtitle = ctx.subtitles.Create();
            newSubtitle.dialogIntno = dialogIntno;
            newSubtitle.subtitleNo = subtitleNo;
            newSubtitle.scentence = "جملة جديدة";
            newSubtitle.startTimeCode = "00:00:00";
            newSubtitle.endTimeCode = "00:00:00";
            newSubtitle.dubbSheetHdrIntno = getDefaultCharacter(dialog.scene.orderTrnHdrIntno);
            ctx.subtitles.Add(newSubtitle);
            ctx.SaveChanges();

            return subtitlesList(dialogIntno);
        }

        public ActionResult deleteSubtitle(long subtitleIntno)
        {
            var subtitle = ctx.subtitles.Find(subtitleIntno);
            var dialogIntno = subtitle.dialogIntno;
            var subtitleNo = subtitle.subtitleNo;

            ctx.subtitles.Remove(subtitle);
            ctx.SaveChanges();
            renumberSubtitles(dialogIntno, subtitleNo);

            return subtitlesList(dialogIntno);
        }

        private long getDefaultCharacter(long orderTrnHdrIntno)
        {
            var dubbSheetHdr = ctx.dubbingSheetHdrs.FirstOrDefault(x => x.orderTrnHdrIntno == orderTrnHdrIntno && x.characterName.ToUpper() == "UNKNOWN");
            if(dubbSheetHdr == null)
            {
                dubbSheetHdr = ctx.dubbingSheetHdrs.Create();
                dubbSheetHdr.orderTrnHdrIntno = orderTrnHdrIntno;
                dubbSheetHdr.characterName = "UNKNOWN";
                dubbSheetHdr.voiceActorIntno = 0;
                dubbSheetHdr.actorName = "ANONYMOUS";
                ctx.dubbingSheetHdrs.Add(dubbSheetHdr);
                ctx.SaveChanges();
            }

            return dubbSheetHdr.dubbSheetHdrIntno;
        }

        private void renumberSubtitles(long dialogIntno, short subtitleNo)
        {
            var subtitles = ctx.subtitles.Where(x => x.dialogIntno == dialogIntno && x.subtitleNo > subtitleNo).OrderBy(x => x.subtitleNo);
            foreach (var item in subtitles)
            {
                item.subtitleNo--;
            }
            ctx.SaveChanges();
        }

        public ActionResult editCharacter(long subtitleIntno)
        {
            var model = ctx.subtitles.Find(subtitleIntno);
            return PartialView("_editCharacter", model);
        }

        public ActionResult getCharactersList(long subtitleIntno)
        {
            var subtitle = ctx.subtitles.Find(subtitleIntno);

            var order = subtitle.dubbingSheetHdr.orderTrnHdr ;
            var list = order.agreementWork.workCharacters.Select(x => x.characterName).ToList();
            var list2 = ctx.dubbingSheetHdrs.Where(x => x.orderTrnHdrIntno == order.orderTrnHdrIntno).Select(x => x.characterName).ToList();
            list.AddRange(list2);
            HashSet<string> names = new HashSet<string>();
            foreach (var s in list)
                names.Add(s);
            return PartialView("_namesList", names.OrderBy(x=> x));
        }

        public ActionResult charactersList(long orderTrnHdrIntno)
        {
            var order = ctx.orderTrnHdrs.Find(orderTrnHdrIntno);
            var list = order.agreementWork.workCharacters.Select(x => x.characterName).ToList();
            var list2 = ctx.dubbingSheetHdrs.Where(x => x.orderTrnHdrIntno == order.orderTrnHdrIntno).Select(x => x.characterName).ToList();
            list.AddRange(list2);
            HashSet<string> names = new HashSet<string>();
            foreach (var s in list)
                names.Add(s);
            return PartialView("_charactersList", names.OrderBy(x => x));
        }

        public void saveCharacter(long subtitleIntno, string newName)
        {
            var subtitle = ctx.subtitles.Find(subtitleIntno);
            var order = subtitle.dubbingSheetHdr.orderTrnHdr;
            var sheetHdr = ctx.dubbingSheetHdrs.FirstOrDefault(x => x.orderTrnHdrIntno == order.orderTrnHdrIntno && x.characterName.ToUpper() == newName.ToUpper());
            if(sheetHdr == null)
            {
                sheetHdr = ctx.dubbingSheetHdrs.Create();
                sheetHdr.orderTrnHdrIntno = order.orderTrnHdrIntno;
                sheetHdr.characterName = newName;
                sheetHdr.voiceActorIntno = 0;
                sheetHdr.actorName = "ANONYMOUS";
                var workChar = order.agreementWork.workCharacters.FirstOrDefault(x => x.characterName.ToUpper() == newName.ToUpper());
                if(workChar != null)
                {
                    sheetHdr.workCharacterIntno = workChar.workCharacterIntno;
                    var workActor = workChar.workActors.FirstOrDefault(x => x.status == true);
                    if(workActor != null)
                    {
                        sheetHdr.voiceActorIntno = workActor.voiceActorIntno;
                        sheetHdr.actorName = workActor.voiceActor.fullName;
                    }
                }
                ctx.dubbingSheetHdrs.Add(sheetHdr);
                ctx.SaveChanges();
            }

            subtitle.dubbSheetHdrIntno = sheetHdr.dubbSheetHdrIntno;
            ctx.SaveChanges();
        }

        public ActionResult editSubtitle(long subtitleIntno)
        {
            var subtitle = ctx.subtitles.Find(subtitleIntno);

            return PartialView("_editSubtitle", subtitle);
        }

        public void saveSubtitle(long subtitleIntno, string newSubtitle)
        {
            var subtitle = ctx.subtitles.Find(subtitleIntno);
            subtitle.scentence = newSubtitle;
            ctx.SaveChanges();
        }

        public void saveSubtitleStartTimeCode(long subtitleIntno, string startTime)
        {
            var subtitle = ctx.subtitles.Find(subtitleIntno);
            subtitle.startTimeCode = startTime;
            ctx.SaveChanges();
        }

        public void saveSubtitleEndTimeCode(long subtitleIntno, string endTime)
        {
            var subtitle = ctx.subtitles.Find(subtitleIntno);
            subtitle.endTimeCode = endTime;
            ctx.SaveChanges();
        }
        
        
    }    
}