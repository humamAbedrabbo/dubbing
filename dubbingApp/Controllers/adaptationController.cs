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


            return View();
            
        }

        public ActionResult GetAdaptationWorks(bool isActive = true)
        {
            // Get list of episodes where adaptation is in progress
            var model = ctx.orderTrnDtls.Include(x => x.employee).Include(x => x.orderTrnHdr).Include(x => x.orderTrnHdr.agreementWork).Where(x => (x.activityType == "01" || x.activityType == "02") && x.status == isActive).ToList();


            if (User.IsInRole("EDITOR"))
            {
                var employee = ctx.employees.FirstOrDefault(x => x.email.ToUpper() == User.Identity.Name);
                if (employee == null)
                {
                    model.Clear();
                }
                else
                {
                    var userWorks = ctx.workPersonnels.Where(x => x.empIntno == employee.empIntno && x.status == true && (x.titleType == "04" || x.titleType == "05" || x.titleType == "06")).Select(x => x.workIntno).ToList();
                    model = model.Where(x => x.empIntno == employee.empIntno && userWorks.Contains(x.orderTrnHdr.workIntno) && (x.activityType == "01" || x.activityType == "02")).ToList();
                }
            }
            ViewBag.Status = isActive;
            return PartialView("_adaptationWorks", model);
        }

        public void RenumberScenesAndDialogs(long orderTrnHdrIntno)
        {
            var order = ctx.orderTrnHdrs.Find(orderTrnHdrIntno);
            short sceneNo = 0;
            short dlgNo = 0;
            short sNo = 0;
            long lastDlgId = 0;
            long lastSceneId = 0;
            var subtitles = ctx.subtitles.Include(x => x.dialog).Include(x => x.dialog.scene).Where(x => x.dialog.scene.orderTrnHdrIntno == orderTrnHdrIntno).OrderBy(x => x.startSecond).ThenBy(x => x.endSecond).ToList();
            foreach(var s in subtitles)
            {
                if(s.dialog.scene.sceneIntno == lastSceneId)
                {
                    s.dialog.scene.sceneNo = sceneNo;
                }
                else
                {
                    sceneNo += 1;
                    lastSceneId = s.dialog.scene.sceneIntno;
                    s.dialog.scene.sceneNo = sceneNo;
                    lastDlgId = 0;
                    dlgNo = 0;
                    sNo = 0;
                }

                if(s.dialog.dialogIntno == lastDlgId)
                {
                    s.dialog.dialogNo = dlgNo;
                }
                else
                {
                    dlgNo += 1;
                    lastDlgId = s.dialogIntno;
                    s.dialog.dialogNo = dlgNo;
                    sNo = 0;
                }
                sNo += 1;
                s.subtitleNo = sNo;
            }
            ctx.SaveChanges();
        }

        public ActionResult CompleteAdaptation(long orderTrnDtlIntno)
        {
            var order = ctx.orderTrnDtls.Find(orderTrnDtlIntno);
            order.status = false;
            ctx.SaveChanges();
            var assignments = order.orderTrnHdr.orderTrnDtls.Where(x =>x.activityType == order.activityType && x.status);

            if (assignments.Count() == 0 && !order.orderTrnHdr.endAdaptation.HasValue)
            {
                order.orderTrnHdr.endAdaptation = DateTime.Now;
                ctx.SaveChanges();
                RenumberScenesAndDialogs(order.orderTrnHdrIntno);
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
                .Where(x => x.dialog.scene.orderTrnHdrIntno == order.orderTrnHdrIntno).OrderBy(x => x.startSecond).ThenBy(x => x.endSecond)
                .ToList();

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

        public ActionResult GetCharacters(long orderTrnHdrIntno)
        {
            var order = ctx.orderTrnHdrs.Find(orderTrnHdrIntno);
            var list = order.agreementWork.workCharacters.Select(x => x.characterName).ToList();
            var list2 = ctx.dubbingSheetHdrs.Where(x => x.orderTrnHdrIntno == order.orderTrnHdrIntno).Select(x => x.characterName).ToList();
            list.AddRange(list2);
            HashSet<string> names = new HashSet<string>();
            foreach (var s in list)
                names.Add(s);

            return Json(names.ToList().OrderBy(x => x), JsonRequestBehavior.AllowGet);
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

        public string FixAllSubtitles()
        {
            foreach(var item in ctx.scenes)
            {
                item.startSecond = TimeConverter.StringToSeconds(item.startTimeCode);
                item.endSecond = TimeConverter.StringToSeconds(item.endTimeCode);
            }
            foreach (var item in ctx.dialogs)
            {
                item.startSecond = TimeConverter.StringToSeconds(item.startTimeCode);
                item.endSecond = TimeConverter.StringToSeconds(item.endTimeCode);
            }
            foreach (var item in ctx.subtitles)
            {
                item.startSecond = TimeConverter.StringToSeconds(item.startTimeCode);
                item.endSecond = TimeConverter.StringToSeconds(item.endTimeCode);
            }
            ctx.SaveChanges();

            return "OK";
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

        public void UpdateSubtitle(long id, long orderTrnHdrIntno, string name, string from, string to, string text)
        {
            var subtitle = ctx.subtitles.Find(id);
            if(subtitle.dubbingSheetHdr.characterName.ToUpper() != name.ToUpper())
            {
                var hdr = ctx.orderTrnHdrs.Find(orderTrnHdrIntno);
                // Find or add character
                var character = hdr.agreementWork.workCharacters.FirstOrDefault(x => x.characterName.ToUpper() == name.ToUpper());

                // Find or add dubbSheetHdr related to character
                var sheetHdr = hdr.dubbingSheetHdrs.FirstOrDefault(x => x.characterName.ToUpper() == name.ToUpper());
                if (sheetHdr == null)
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
                subtitle.dubbSheetHdrIntno = sheetHdr.dubbSheetHdrIntno;
            }

            subtitle.startTimeCode = from;
            subtitle.endTimeCode = to;
            subtitle.startSecond = TimeConverter.StringToSeconds(from);
            subtitle.endSecond = TimeConverter.StringToSeconds(to);
            subtitle.scentence = text;
            ctx.SaveChanges();

        }

        public void DeleteSubtitle(long id)
        {
            var subtitle = ctx.subtitles.Find(id);
            ctx.subtitles.Remove(subtitle);
            ctx.SaveChanges();

        }
        /***********************************************************************************************************
        *
        * OLD CODE FOR Edit1
        *             
       /************************************************************************************************************/

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