using dubbingApp.Models;
using dubbingApp.Utils;
using dubbingModel;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace dubbingApp.Controllers
{
    [Authorize(Roles = "ADMIN, GENERAL_MANAGER, EDITOR, STUDIO_SUPERVISOR")]
    public class adaptationController : Controller
    {
        DUBBDBEntities ctx = new DUBBDBEntities();

         // GET: adaptations
        public ActionResult Index()
        {
            // Get list of episodes where adaptation is in progress
            var model = ctx.orderTrnDtls.Include(x => x.employee).Include(x => x.orderTrnHdr).Include(x => x.orderTrnHdr.agreementWork).Where(x => !x.status).ToList();
            

            if (User.IsInRole("EDITOR"))
            {
                var employee = ctx.employees.FirstOrDefault(x => x.email.ToUpper() == User.Identity.Name);
                if(employee == null)
                {
                    model.Clear();
                }
                else
                {
                    var userWorks = ctx.workPersonnels.Where(x => x.empIntno == employee.empIntno && x.status == true && (x.titleType == "04" || x.titleType == "05" || x.titleType == "06")).Select(x => x.workIntno).ToList() ;
                    model = model.Where(x => x.empIntno == employee.empIntno && userWorks.Contains(x.orderTrnHdr.workIntno) && (x.activityType == "01" || x.activityType == "02")).ToList() ;
                }
            }

            return View(model);
            
        }

        public ActionResult CompleteAdaptation(long orderTrnDtlIntno)
        {
            var order = ctx.orderTrnDtls.Find(orderTrnDtlIntno);
            order.status = true;
            ctx.SaveChanges();
            var assignments = order.orderTrnHdr.orderTrnDtls.Where(x =>x.activityType == order.activityType && x.status == false);

            if (assignments.Count() == 0 && !order.orderTrnHdr.endAdaptation.HasValue)
            {
                order.orderTrnHdr.endAdaptation = DateTime.Now;
                ctx.SaveChanges();
            }
            return RedirectToAction("Index");
        }

        public ActionResult Edit(long? id)
        {
            var model = ctx.orderTrnHdrs.Include(x => x.agreementWork).First(x => x.orderTrnHdrIntno == id);

            return View(model);
        }

        public ActionResult Edit2(long? id, string fromTime = "00:00:00", string toTime = "00:00:00")
        {
            var hdr = ctx.orderTrnHdrs.Include(x => x.agreementWork).First(x => x.orderTrnHdrIntno == id);
            var model = new AdaptationViewModel();

            // setup model
            model.OrderTrnHdrIntno = hdr.orderTrnHdrIntno;
            model.Title = string.Format("{0}/{1}", hdr.agreementWork.workName, hdr.episodeNo);
            model.SceneMin = TimeConverter.StringToSeconds(fromTime);
            model.SceneMax = TimeConverter.StringToSeconds(toTime);
            model.SceneMinNo = 1;
            model.SceneMaxNo = 1000;

            RefreshSubtitles(hdr, model);
            RefreshCharacters(hdr, model);

            return View("Edit2", model);
        }

        private void RefreshSubtitles(orderTrnHdr order, AdaptationViewModel model)
        {
            var subtitles = ctx.subtitles.Include(x => x.dialog)
                .Include(x => x.dialog.scene)
                .Include(x => x.dubbingSheetHdr)
                .Include(x => x.dubbingSheetHdr.workCharacter)
                .Where(x => x.dialog.scene.orderTrnHdrIntno == order.orderTrnHdrIntno).ToList()
                .OrderBy(x => TimeConverter.StringToSeconds(x.startTimeCode));

            model.Subtitles.Clear();

            foreach (var s in subtitles)
            {
                model.Subtitles.Add(new ASubtitle()
                {
                    Id = s.subtitleIntno,
                    SceneId = s.dialog.scene.sceneIntno,
                    SceneNo = s.dialog.scene.sceneNo,
                    DlgId = s.dialog.dialogIntno,
                    DlgNo = s.dialog.dialogNo,
                    No = s.subtitleNo,
                    StartTime = s.startTimeCode,
                    Start = s.startSecond,
                    EndTime = s.endTimeCode,
                    End = s.endSecond,
                    Text = s.scentence,
                    CharacterId = s.dubbingSheetHdr.workCharacterIntno,
                    CharacterName = s.dubbingSheetHdr.characterName
                });
            }
        }

        private void RefreshCharacters(orderTrnHdr order, AdaptationViewModel model)
        {
            
            var list = order.agreementWork.workCharacters.Select(x => x.characterName).ToList();
            var list2 = ctx.dubbingSheetHdrs.Where(x => x.orderTrnHdrIntno == order.orderTrnHdrIntno).Select(x => x.characterName).ToList();
            list.AddRange(list2);
            HashSet<string> names = new HashSet<string>();
            foreach (var s in list)
                names.Add(s);

            model.Characters.Clear();

            foreach (var w in names)
            {
                model.Characters.Add(new ACharacter()
                {
                    Name = w
                });
            }
        }

        public ActionResult SubtitlesList2(long orderTrnHdrIntno, int? from, int? to)
        {
            var hdr = ctx.orderTrnHdrs.Include(x => x.agreementWork).First(x => x.orderTrnHdrIntno == orderTrnHdrIntno);
            var model = new AdaptationViewModel();

            // setup model
            model.OrderTrnHdrIntno = hdr.orderTrnHdrIntno;
           

            RefreshSubtitles(hdr, model);
            return PartialView("_subtitlesList2", model);
        }

        
        public void SubmitSubtitle(long orderTrnHdrIntno, string name, string from, string to, string text, bool newScene = true, bool newDlg = true)
        {
            var hdr = ctx.orderTrnHdrs.Find(orderTrnHdrIntno);
            // Find or add character
            var character = hdr.agreementWork.workCharacters.FirstOrDefault(x => x.characterName.ToUpper() == name.ToUpper());

            // Find or add dubbSheetHdr related to character
            var sheetHdr = hdr.dubbingSheetHdrs.FirstOrDefault(x => x.characterName.ToUpper() == name.ToUpper());
            if(sheetHdr == null)
            {
                sheetHdr = ctx.dubbingSheetHdrs.Create();
                sheetHdr.orderTrnHdrIntno = orderTrnHdrIntno;
                sheetHdr.characterName = name;
                sheetHdr.voiceActorIntno = 0;
                sheetHdr.actorName = "ANONYMOUS";
                if (character != null)
                {
                    sheetHdr.workCharacterIntno = character.workCharacterIntno;
                    var workActor = character.workActors.FirstOrDefault(x => x.status == true);
                    if (workActor != null)
                    {
                        sheetHdr.voiceActorIntno = workActor.voiceActorIntno;
                        sheetHdr.actorName = workActor.voiceActor.fullName;
                    }
                }

                ctx.dubbingSheetHdrs.Add(sheetHdr);
                ctx.SaveChanges();
            }

            // Handle scene and dialog IDs
            long sceneId = 0;
            long dlgId = 0;
            if(newScene)
            {
                var scene = ctx.scenes.Create();
                scene.orderTrnHdrIntno = hdr.orderTrnHdrIntno;
                scene.sceneNo = 1;
                scene.startTimeCode = from;
                scene.endTimeCode = to;
                scene.startSecond = TimeConverter.StringToSeconds(from);
                scene.endSecond = TimeConverter.StringToSeconds(to);
                scene.isTaken = false;
                
                ctx.scenes.Add(scene);
                ctx.SaveChanges();
                sceneId = scene.sceneIntno;

                var dlg = ctx.dialogs.Create();
                dlg.sceneIntno = sceneId;
                dlg.dialogNo = 1;
                dlg.isTaken = false;
                dlg.startTimeCode = from;
                dlg.endTimeCode = to;
                dlg.startSecond = TimeConverter.StringToSeconds(from);
                dlg.endSecond = TimeConverter.StringToSeconds(to);
                ctx.dialogs.Add(dlg);
                ctx.SaveChanges();
                dlgId = dlg.dialogIntno;
            }
            else
            {
                var scene = hdr.scenes.Where(x => x.startSecond <= TimeConverter.StringToSeconds(from)).OrderBy(x => x.startSecond).LastOrDefault();
                if(scene == null)
                {
                    // add new scene because the wanted is not exist
                    scene = ctx.scenes.Create();
                    scene.orderTrnHdrIntno = hdr.orderTrnHdrIntno;
                    scene.sceneNo = 1;
                    scene.startTimeCode = from;
                    scene.endTimeCode = to;
                    scene.startSecond = TimeConverter.StringToSeconds(from);
                    scene.endSecond = TimeConverter.StringToSeconds(to);
                    scene.isTaken = false;

                    ctx.scenes.Add(scene);
                    ctx.SaveChanges();                    
                }
                else
                {
                    // Update scene end time
                    if(scene.endSecond < TimeConverter.StringToSeconds(to))
                    {
                        scene.endSecond = TimeConverter.StringToSeconds(to);
                        scene.endTimeCode = to;
                        ctx.SaveChanges();    
                    }
                }
                sceneId = scene.sceneIntno;
                dialog dlg = null;
                if(!newDlg)
                {
                    dlg = scene.dialogs.Where(x => x.startSecond <= TimeConverter.StringToSeconds(from)).OrderBy(x => x.startSecond).LastOrDefault();                    
                    if(dlg != null)
                    {
                        if(dlg.endSecond < TimeConverter.StringToSeconds(to))
                        {
                            dlg.endSecond = TimeConverter.StringToSeconds(to);
                            dlg.endTimeCode = to;
                            ctx.SaveChanges();
                        }
                    }
                }
                if(dlg == null)
                {
                    dlg = ctx.dialogs.Create();
                    dlg.sceneIntno = sceneId;
                    dlg.dialogNo = 1;
                    dlg.isTaken = false;
                    dlg.startTimeCode = from;
                    dlg.endTimeCode = to;
                    dlg.startSecond = TimeConverter.StringToSeconds(from);
                    dlg.endSecond = TimeConverter.StringToSeconds(to);
                    ctx.dialogs.Add(dlg);
                    ctx.SaveChanges();
                }
                dlgId = dlg.dialogIntno;
            }

            // Add subtitle
            var subtitle = ctx.subtitles.Create();
            subtitle.dialogIntno = dlgId;
            subtitle.dubbSheetHdrIntno = sheetHdr.dubbSheetHdrIntno;
            subtitle.startTimeCode = from;
            subtitle.endTimeCode = to;
            subtitle.subtitleNo = 0;
            subtitle.startSecond = TimeConverter.StringToSeconds(from);
            subtitle.endSecond = TimeConverter.StringToSeconds(to);
            subtitle.scentence = text;

            ctx.subtitles.Add(subtitle);
            ctx.SaveChanges();
        }


        /***********************************************************************************************************
        *
        * OLD CODE FOR Edit1
        *             
       /************************************************************************************************************/

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

        public FileStreamResult downloadFile(long orderTrnHdrIntno)
        {
            var order = ctx.orderTrnHdrs.Find(orderTrnHdrIntno);
            StringBuilder sb = new StringBuilder();
            int line = 1;

            foreach(var scene in order.scenes.OrderBy(x => x.sceneNo))
            {
                foreach(var dialog in scene.dialogs.OrderBy(x => x.dialogNo))
                {
                    foreach(var subtitle in dialog.subtitles.OrderBy(x => x.subtitleNo))
                    {
                        sb.AppendFormat("{0}", line);
                        sb.AppendLine();
                        sb.AppendFormat("{0},1 --> {1},1", subtitle.startTimeCode, subtitle.endTimeCode);
                        sb.AppendLine();
                        sb.AppendFormat("<font size=\"36px\" color=\"white\">{0}</font>", subtitle.scentence);
                        sb.AppendLine();
                        line++;
                    }
                }
            }

            string fileName =  order.agreementWork.workName + " - " + order.episodeNo + ".srt";
            UTF8Encoding encoding = new UTF8Encoding();
            byte[] contentAsBytes = encoding.GetBytes(sb.ToString());
            var stream = new MemoryStream(contentAsBytes);


            //FileInfo info = new FileInfo(fileName);
            //if (!info.Exists)
            //{
            //    using (StreamWriter writer = info.CreateText())
            //    {
            //        writer.WriteLine(sb.ToString());

            //    }
            //}

            return File(stream, "text/plain", fileName);

        }


    }    
}