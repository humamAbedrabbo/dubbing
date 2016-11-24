using System;
using System.Collections.Generic;
using System.Linq;
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
            var model = ctx.voiceActors.Find(id).audioSampleHdrs;
            ViewBag.actorId = id;
            ViewBag.tagTemplates = ctx.tagTemplateHdrs.OrderBy(x=>x.Title).ToList();
            return View(model);
        }

        [HttpPost]
        public ActionResult saveSample(long actorId, long templateId, string title, string description, HttpPostedFileBase fileInput)
        {
            if (fileInput != null && fileInput.ContentLength > 0)
            {

                var fileName = Path.GetFileName(fileInput.FileName);
                var ext = Path.GetExtension(fileName);

                var sample = ctx.audioSampleHdrs.Create();
                sample.Title = title;
                sample.Description = description;
                sample.voiceActorIntno = actorId;
                sample.tagTemplateHdrIntno = templateId;
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


                }

            return RedirectToAction("ActorSamples", new { id = actorId });

        }


    }
}