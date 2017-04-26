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
using System.Windows.Forms;

namespace dubbingApp.Controllers
{
    [Authorize(Roles = "ADMIN, GENERAL_MANAGER, EDITOR")]
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
            var model = ctx.orderTrnDtls.Include(x => x.employee).Include(x => x.orderTrnHdr).Include(x => x.orderTrnHdr.agreementWork).Where(x => x.activityType == "02" && x.status == isActive && x.orderTrnHdr.agreementWork.status == "01").ToList();

            if (User.IsInRole("EDITOR"))
            {
                var employee = ctx.employees.FirstOrDefault(x => x.email.ToUpper() == User.Identity.Name.ToUpper());
                if (employee == null)
                {
                    model.Clear();
                }
                else
                {
                    model = model.Where(x => x.empIntno == employee.empIntno).ToList();
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
            var subtitles = ctx.subtitles.Include(x => x.dialog).Include(x => x.dialog.scene).Where(x => x.dialog.scene.orderTrnHdrIntno == orderTrnHdrIntno).OrderBy(x => x.startSecond).ThenBy(x => x.startMillisecond).ThenBy(x => x.endSecond).ThenBy(x => x.endMillisecond).ToList(); //.ThenBy(x => x.endSecond)
            foreach (var s in subtitles)
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
                CleanSheetHdrsWithoutScenes(order.orderTrnHdrIntno);
                CleanEmptyDialogs(order.orderTrnHdrIntno);
                CleanEmptyScenes(order.orderTrnHdrIntno);
                RenumberScenesAndDialogs(order.orderTrnHdrIntno);
                //added by wael
                CreateSheetDtls(order.orderTrnHdrIntno);

                order.orderTrnHdr.endAdaptation = DateTime.Now;
                ctx.SaveChanges();
            }
            return RedirectToAction("GetAdaptationWorks", new { isActive = true });
        }

        public ActionResult EditScenesAndDialogs(long orderTrnHdrIntno)
        {
            
            CleanSheetHdrsWithoutScenes(orderTrnHdrIntno);
            CleanEmptyDialogs(orderTrnHdrIntno);
            CleanEmptyScenes(orderTrnHdrIntno);
            RenumberScenesAndDialogs(orderTrnHdrIntno);

            var order = ctx.orderTrnHdrs.Find(orderTrnHdrIntno);
            ViewBag.Episode = order.agreementWork.workName + " / " + order.episodeNo;

            var model = ctx.subtitles.Include(x => x.dialog)
                .Include(x => x.dialog.scene)
                .Include(x => x.dubbingSheetHdr)
                .Include(x => x.dubbingSheetHdr.workCharacter)
                .Where(x => x.dialog.scene.orderTrnHdrIntno == orderTrnHdrIntno).OrderBy(x => x.startSecond).ThenBy(x => x.startMillisecond).ThenBy(x => x.endSecond).ThenBy(x => x.endMillisecond)
                .ToList();
            ViewBag.Dialogs = model.Select(x => x.dialog).Distinct().OrderBy(x => x.scene.sceneNo).ThenBy(x => x.dialogNo).ToList();
            ViewBag.Scenes = model.Select(x => x.dialog.scene).Distinct().OrderBy(x => x.sceneNo).ToList();
            ViewBag.order = orderTrnHdrIntno;
            return View("EditScenesAndDialogs", model);
        }

        public ActionResult SceneDialogs(long sceneIntno)
        {
            var dialogs = ctx.scenes.Find(sceneIntno).dialogs.ToList();
            return Json(dialogs.Select(x => new { dialogIntno = x.dialogIntno, dialogNo = x.dialogNo }).ToList(), JsonRequestBehavior.AllowGet);
        }

        public void ChangeDialog(long orderTrnHdrIntno, long subtitleIntno, long dialogIntno, long sceneIntno)
        {
            var subtitle = ctx.subtitles.Find(subtitleIntno);
            if (sceneIntno == 0)
            {
                var scene = ctx.scenes.Create();
                scene.orderTrnHdrIntno = orderTrnHdrIntno;
                scene.sceneNo = 0;
                scene.isTaken = false;
                ctx.scenes.Add(scene);
                ctx.SaveChanges();

                var dlg = ctx.dialogs.Create();
                dlg.sceneIntno = scene.sceneIntno;
                dlg.dialogNo = 0;
                dlg.isTaken = false;
                ctx.dialogs.Add(dlg);
                ctx.SaveChanges();

                subtitle.dialogIntno = dlg.dialogIntno;
                ctx.SaveChanges();
            }
            else
            {
                if (dialogIntno == 0)
                {
                    var dlg = ctx.dialogs.Create();
                    dlg.sceneIntno = subtitle.dialog.sceneIntno;
                    dlg.dialogNo = 0;
                    dlg.isTaken = false;
                    ctx.dialogs.Add(dlg);
                    ctx.SaveChanges();
                    subtitle.dialogIntno = dlg.dialogIntno;
                    ctx.SaveChanges();
                }
                else
                {
                    subtitle.dialogIntno = dialogIntno;
                    ctx.SaveChanges();
                }
            }           
        }

        public ActionResult Edit(long? id)
        {
            var model = ctx.orderTrnHdrs.Include(x => x.agreementWork).First(x => x.orderTrnHdrIntno == id);

            return View(model);
        }

        public ActionResult Edit2(long? id, string fromTime = "00:00:00", string toTime = "00:00:00")
        {
            var hdr = ctx.orderTrnHdrs.Include(x => x.agreementWork).First(x => x.orderTrnHdrIntno == id);
            if(!hdr.startAdaptation.HasValue)
            {
                hdr.startAdaptation = DateTime.Now;
                ctx.SaveChanges();
            }
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
                .Where(x => x.dialog.scene.orderTrnHdrIntno == order.orderTrnHdrIntno).OrderBy(x => x.startSecond).ThenBy(x => x.startMillisecond).ThenBy(x => x.endSecond).ThenBy(x => x.endMillisecond)
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
                    StartMilli = s.startMillisecond,
                    EndMilli = s.endMillisecond,
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

        
        public void SubmitSubtitle(long orderTrnHdrIntno, string name, string from, string to, long fromM, long toM, string text, bool newScene = true, bool newDlg = true)
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
            subtitle.startMillisecond = fromM;
            subtitle.endMillisecond = toM;
            subtitle.subtitleNo = 0;
            subtitle.startSecond = TimeConverter.StringToSeconds(from);
            subtitle.endSecond = TimeConverter.StringToSeconds(to);
            subtitle.scentence = text;

            ctx.subtitles.Add(subtitle);
            ctx.SaveChanges();
        }

        public void UpdateSubtitle(long id, long orderTrnHdrIntno, string name, string from, string to, long fromM, long toM, string text)
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
            subtitle.startMillisecond = fromM;
            subtitle.endMillisecond = toM;
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

        private void CleanSheetHdrsWithoutScenes(long orderTrnHdrIntno)
        {
            var order = ctx.orderTrnHdrs.Find(orderTrnHdrIntno);
            var sheetHdrs = ctx.dubbingSheetHdrs.Include(x => x.subtitles).Where(x => x.orderTrnHdrIntno == orderTrnHdrIntno && x.subtitles.Count == 0);
            ctx.dubbingSheetHdrs.RemoveRange(sheetHdrs);
            ctx.SaveChanges();
        }

        private void CleanEmptyDialogs(long orderTrnHdrIntno)
        {
            var order = ctx.orderTrnHdrs.Find(orderTrnHdrIntno);
            var dialogs = ctx.dialogs.Include(x => x.scene).Include(x => x.subtitles).Where(x => x.scene.orderTrnHdrIntno == orderTrnHdrIntno && x.subtitles.Count == 0);
            
            ctx.dialogs.RemoveRange(dialogs);
            ctx.SaveChanges();
        }
        private void CleanEmptyScenes(long orderTrnHdrIntno)
        {
            var order = ctx.orderTrnHdrs.Find(orderTrnHdrIntno);
            var scenes = ctx.scenes.Include(x => x.dialogs).Where(x => x.orderTrnHdrIntno == orderTrnHdrIntno && x.dialogs.Count == 0);

            ctx.scenes.RemoveRange(scenes);
            ctx.SaveChanges();
        }
        private void CreateSheetDtls(long orderTrnHdrIntno)
        {
            var model = ctx.dubbingSheetDtls;
            var x = ctx.subtitles.Include(b => b.dialog.scene).Where(b => b.dialog.scene.orderTrnHdrIntno == orderTrnHdrIntno)
                    .Select(b => new { dubbSheetHdrIntno = b.dubbSheetHdrIntno, sceneNo = b.dialog.scene.sceneNo}).Distinct().ToList();

            foreach (var hdr in x)
            {
                var z = ctx.dubbingSheetDtls.FirstOrDefault(b => b.dubbSheetHdrIntno == hdr.dubbSheetHdrIntno && b.sceneNo == hdr.sceneNo);
                if (z == null)
                {
                    dubbingSheetDtl dtl = new dubbingSheetDtl();
                    dtl.dubbSheetHdrIntno = hdr.dubbSheetHdrIntno;
                    dtl.orderTrnHdrIntno = orderTrnHdrIntno;
                    dtl.sceneNo = hdr.sceneNo;
                    dtl.isTaken = false;
                    model.Add(dtl);
                }
            }
            ctx.SaveChanges();
        }

        [HttpPost]
        public ActionResult ImportFile(long order)
        {
            string savedFileName = "";
            foreach (string file in Request.Files)
            {
                HttpPostedFileBase hpf = Request.Files[file] as HttpPostedFileBase;
                if (hpf.ContentLength == 0)
                    continue;
                savedFileName = Path.Combine(
                   Server.MapPath("~/Content/Files"),
                   Path.GetFileName(hpf.FileName));
                hpf.SaveAs(savedFileName);
            }


            ImportManager im = new Utils.ImportManager();
            im.ImportFile(order, savedFileName);

            System.IO.File.Delete(savedFileName);
            return RedirectToAction("Index");
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
            //sb.AppendFormat("{0}", line);
            //sb.AppendFormat("{0}", @"{\rtf1\ansi\ansicpg1252\deff0\nouicompat\deflang14346{\fonttbl{\f0\fs20\fnil\fcharset0 Calibri;}} {\*\generator Riched20 10.0.10586}\viewkind4\uc1 \pard\sa200\sl276\slmult1\f0\fs22\lang10");
            //sb.AppendLine();
            //line++;
            var subtitles = ctx.subtitles.Include(XmlSiteMapProvider => XmlSiteMapProvider.dialog).Include(x => x.dialog.scene).Include(x => x.dialog.scene.orderTrnHdr)
                .Where(x => x.dialog.scene.orderTrnHdrIntno == orderTrnHdrIntno)
                .OrderBy(x => x.startSecond).ThenBy(x => x.startMillisecond).ThenBy(x => x.endSecond).ThenBy(x => x.endMillisecond)
                .ToList();
            foreach (var subtitle in subtitles)
            {
                sb.AppendFormat("{0}", line);
                sb.AppendLine();
                sb.AppendFormat("{0},{2} --> {1},{3}", subtitle.startTimeCode, subtitle.endTimeCode, subtitle.startMillisecond, subtitle.endMillisecond);
                sb.AppendLine();
                //sb.AppendFormat("<font size=\"36px\" color=\"white\">{0}</font>", subtitle.scentence);
                sb.AppendFormat("{0}", subtitle.scentence);
                sb.AppendLine();
                sb.AppendLine();
                line++;
            }
            //foreach (var scene in order.scenes.OrderBy(x => x.sceneNo))
            //{
            //    foreach(var dialog in scene.dialogs.OrderBy(x => x.dialogNo))
            //    {
            //        foreach(var subtitle in dialog.subtitles.OrderBy(x => x.startSecond).ThenBy(x => x.startMillisecond).ThenBy(x => x.endSecond).ThenBy(x=>x.endMillisecond))
            //        {
            //            sb.AppendFormat("{0}", line);
            //            sb.AppendLine();
            //            sb.AppendFormat("{0}.{2},1 --> {1}.{3},1", subtitle.startTimeCode, subtitle.endTimeCode, subtitle.startMillisecond, subtitle.endMillisecond);
            //            sb.AppendLine();
            //            //sb.AppendFormat("<font size=\"36px\" color=\"white\">{0}</font>", subtitle.scentence);
            //            sb.AppendFormat("{0}", subtitle.scentence);
            //            sb.AppendLine();
            //            sb.AppendLine();
            //            line++;
            //        }
            //    }
            //}



            //sb.AppendFormat("{0}", @"\par}");
            //sb.AppendLine();

            string fileName = order.agreementWork.workName + " - " + order.episodeNo + ".srt";
            UTF8Encoding encoding = new UTF8Encoding();
            byte[] contentAsBytes = encoding.GetBytes(sb.ToString());
            var stream = new MemoryStream(contentAsBytes);

            //added by wael
            //using (RichTextBox RTB = new RichTextBox())
            //{
            //    RTB.Rtf = sb.ToString();
            //    RTB.SaveFile(stream, RichTextBoxStreamType.RichText);
            //}

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

        public FileStreamResult downloadTemplateFile(long orderTrnHdrIntno)
        {
            var order = ctx.orderTrnHdrs.Find(orderTrnHdrIntno);
            var tempFile = Server.MapPath("~/Content/templates/adaptation-template.xlsx");
            byte[] contentAsBytes = System.IO.File.ReadAllBytes(tempFile);

            string fileName = orderTrnHdrIntno.ToString() + "-" + order.agreementWork.workName + " - " + order.episodeNo + ".xlsx";
            var stream = new MemoryStream(contentAsBytes);


            return File(stream, "text/plain", fileName);

        }

        //added by wael
        //to replace text in all subtitles belonging to an episode
        public ActionResult ReplaceText(long id, string findText, string replaceWith)
        {
            if (string.IsNullOrEmpty(findText) || string.IsNullOrEmpty(replaceWith))
                return Content("Please provide both Values!", "text/html");
            findText = findText.Trim();
            replaceWith = replaceWith.Trim();

            var model = ctx.subtitles.Include(b => b.dialog.scene).Where(b => b.dialog.scene.orderTrnHdrIntno == id && b.scentence.Contains(findText)).ToList();
            foreach(var item in model)
            {
                item.scentence = item.scentence.Replace(findText, replaceWith);
            }
            ctx.SaveChanges();
            return Content(model.Count() + " Occurances Were found and Successfully Replaced!", "text/html");
        }

        //added by wael
        public JsonResult popoulateCharactersCombo(long id)
        {
            var oi = ctx.orderTrnHdrs.Find(id);
            var model = ctx.workCharacters.Where(b => b.workIntno == oi.workIntno);
            SelectList cList = new SelectList(model, "workCharacterIntno", "characterName");
            return Json(cList);
        }

        public ActionResult RenameCharacter(long dubbSheetHdrIntno, string toCharacterName, long? toCharacterIntno, bool isSubtitle)
        {
            var model = ctx.dubbingSheetHdrs.Find(dubbSheetHdrIntno);
            string oldCharacterName = model.characterName.Trim();
            string newCharacterName = toCharacterName;

            if (toCharacterIntno.HasValue && string.IsNullOrEmpty(toCharacterName))
            {
                var c2 = ctx.workCharacters.Find(toCharacterIntno);
                model.workCharacterIntno = toCharacterIntno;
                model.characterName = c2.characterName;
                newCharacterName = c2.characterName;
            }
            else if (!toCharacterIntno.HasValue && !string.IsNullOrEmpty(toCharacterName))
            {
                model.workCharacterIntno = null;
                model.characterName = newCharacterName;
            }
            else
            {
                return Content("Failed to Rename Character! Only ONE Renaming Method SHOULD be given.", "text/html");
            }
            
            ctx.SaveChanges();

            if(isSubtitle)
            {
                var model1 = ctx.subtitles.Where(b => b.dubbSheetHdrIntno == dubbSheetHdrIntno && b.scentence.Contains(oldCharacterName)).ToList();
                foreach (var item in model1)
                {
                    item.scentence = item.scentence.Replace(oldCharacterName, newCharacterName.Trim());
                }
                ctx.SaveChanges();
                return Content("Character Successfully Renamed. " + model1.Count() + " Occurances Were found in Subtitles and Successfully Replaced!", "text/html");
            }
            else
                return Content("Character Successfully Renamed.", "text/html");
        }
    }    
}