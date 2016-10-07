using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;
using System.Web.Mvc;
using dubbingModel;

namespace voiceLibrary.Controllers
{
    public class TemplatesController : Controller
    {
        DUBBDBEntities ctx = new DUBBDBEntities();

        // GET: Templates
        public ActionResult Index()
        {
            var model = ctx.tagTemplateHdrs.Include(x => x.tagTemplateDtls).OrderBy(x => x.Title).ToList();
            return View(model);
        }
        public ActionResult GetTemplates()
        {
            var model = ctx.tagTemplateHdrs.Include(x => x.tagTemplateDtls).OrderBy(x => x.Title).ToList();
            return PartialView("_templates", model);
        }
        public ActionResult SaveTemplate(string title, string text, string description)
        {
            var temp = ctx.tagTemplateHdrs.Create();
            temp.Title = title;
            temp.Text = text;
            temp.Description = description;
            ctx.tagTemplateHdrs.Add(temp);
            ctx.SaveChanges();
            return GetTemplates();
        }
        public ActionResult SaveTemplateTag(long tagTemplateHdrIntno, string tagName, int minScore, int weight)
        {
            var dtl = ctx.tagTemplateDtls.Create();
            dtl.tagTemplateHdrIntno = tagTemplateHdrIntno;
            dtl.tagId = ctx.tags.First(x => x.Name.ToUpper() == tagName.ToUpper()).Id;
            dtl.MinScore = minScore;
            dtl.Weight = weight;
            ctx.tagTemplateDtls.Add(dtl);
            ctx.SaveChanges();
            return GetTemplates();
        }
    }
}