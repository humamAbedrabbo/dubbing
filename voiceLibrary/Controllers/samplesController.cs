using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.Entity;
using System.Web;
using System.Web.Mvc;
using dubbingModel;
using System.IO;
using voiceLibrary.Utils;

namespace voiceLibrary.Controllers
{
    public class samplesController : Controller
    {
        DUBBDBEntities ctx = new DUBBDBEntities();

        // GET: samples
        public ActionResult Index()
        {
            var actors = ctx.voiceActors.Where(x=>x.voiceActorIntno > 0).OrderBy(x => x.fullName.Trim()).ToList();
            return View(actors);
        }

        public ActionResult ActorSamples(long id)
        {
            var actor = ctx.voiceActors.Find(id);
            var model = ctx.audioSampleHdrs.Include(x => x.tagTemplateHdr).Where(x => x.voiceActorIntno == id);
            ViewBag.actorId = id;
            ViewBag.ActorName = actor.fullName;
            ViewBag.tagTemplates = ctx.tagTemplateHdrs.OrderBy(x=>x.Title).ToList();
            // ViewBag.tagTemplates = new SelectList(ctx.tagTemplateHdrs.OrderBy(x => x.Title).ToList(), "tagTemplateHdrIntno", "Title");
            return View(model);
        }

        [HttpPost]
        public ActionResult saveSample(long actorId, long tagTemplateHdrIntno, string title, string description, HttpPostedFileBase fileInput)
        {
            if (fileInput != null && fileInput.ContentLength > 0)
            {

                var fileName = Path.GetFileName(fileInput.FileName);
                var ext = Path.GetExtension(fileName);

                var sample = ctx.audioSampleHdrs.Create();
                sample.Title = title;
                sample.Description = description;
                sample.voiceActorIntno = actorId;
                sample.tagTemplateHdrIntno = tagTemplateHdrIntno;
                sample.SubmitDate = DateTime.Now;
                sample.Status = "01";
                sample.FileUrl = Server.MapPath(".");

                ctx.audioSampleHdrs.Add(sample);
                ctx.SaveChanges();




                var newFileName = sample.audioSampleHdrIntno.ToString() + "." + ext;

                IFileUploadService uploader = new LocalFileUploadStrategy();
                string url = uploader.UploadFile(fileInput, newFileName);
                sample.FileUrl = url;
                ctx.SaveChanges();


                var dtls = ctx.tagTemplateDtls.Where(x => x.tagTemplateHdrIntno == sample.tagTemplateHdrIntno);
                foreach(var dtl in dtls)
                {
                    var d = ctx.audioSampleDtls.Create();
                    d.audioSampleHdrIntno = sample.audioSampleHdrIntno;
                    d.tagId = dtl.tagId;
                    d.TagScore = 1;
                    d.Match = 1;
                    ctx.audioSampleDtls.Add(d);
                }
                ctx.SaveChanges();

            }

            return RedirectToAction("ActorSamples", new { id = actorId });

        }

        public ActionResult editSample(long id)
        {
            
            //var model1 = ctx.audioSampleHdrs.Include(x => x.audioSampleDtls).FirstOrDefault(x => x.audioSampleHdrIntno == id);
            var model = (from h in ctx.audioSampleHdrs
                         join d in ctx.audioSampleDtls on h.audioSampleHdrIntno equals d.audioSampleHdrIntno
                         join t in ctx.tags on d.tagId equals t.Id
                         select h
                        
                ).FirstOrDefault(x => x.audioSampleHdrIntno == id);
            ViewBag.tagTemplates = ctx.tagTemplateHdrs.OrderBy(x => x.Title).ToList();
            // ViewBag.tagTemplates = new SelectList(ctx.tagTemplateHdrs.OrderBy(x => x.Title).ToList(), "tagTemplateHdrIntno", "Title");
            return PartialView("editSample", model);
        }

        [HttpPost]
        public ActionResult saveSampleDtl(long audioSampleHdrIntno, string title, string description, long voiceActorIntno)
        {
            var model = ctx.audioSampleHdrs.Find(audioSampleHdrIntno);

            model.Title = title;
            model.Description = description;
            ctx.SaveChanges();

            return RedirectToAction("ActorSamples", new { id = voiceActorIntno });
        }

        public void saveScore(long id, int score)
        {
            var model = ctx.audioSampleDtls.Find(id);

            model.TagScore = score;
            ctx.SaveChanges();

            
        }
    }
}