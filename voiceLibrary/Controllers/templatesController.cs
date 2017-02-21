using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using dubbingModel;
using System.Data.Entity;

namespace voiceLibrary.Controllers
{
    public class templatesController : Controller
    {
        DUBBDBEntities ctx = new DUBBDBEntities();

        // GET: templates
        public ActionResult Index()
        {
            //ViewBag.TagNames = ctx.tags.OrderBy(x => x.Name).Select(x => x.Name).ToArray();
            return View();
        }
        public ActionResult templateList()
        {
            var model = ctx.tagTemplateHdrs.Include(x=>x.tagTemplateDtls);
            return PartialView("_templateList", model);
        }

        public ActionResult editTemplate(long id)
        {
            var model = ctx.tagTemplateHdrs.Include(x=>x.tagTemplateDtls).Include("tagTemplateDtls.tag").First(x=>x.tagTemplateHdrIntno == id);
            return PartialView("_editTemplate", model);
            
        }
        public ActionResult saveTemplate(long tagTemplateHdrIntno, string title, string desc, string text)
        {
            if(tagTemplateHdrIntno == 0)
            {
                var model = ctx.tagTemplateHdrs.Create();
                model.Title = title;
                model.Description = desc;
                model.Text = text;
                ctx.tagTemplateHdrs.Add(model);
                ctx.SaveChanges();
            }
            else
            {
                var model = ctx.tagTemplateHdrs.Find(tagTemplateHdrIntno);
                model.Title = title;
                model.Description = desc;
                model.Text = text;
            }
            ctx.SaveChanges();

            return RedirectToAction("templateList");
        }
        public ActionResult deleteTemplate(long id)
        {
            var model = ctx.tagTemplateHdrs.Find(id);
            ctx.tagTemplateHdrs.Remove(model);
            ctx.SaveChanges();
            return RedirectToAction("templateList");
        }

        public ActionResult addDetail(long id, string tagName, int minScore, int weight)
        {
            var tag = ctx.tags.FirstOrDefault(x => x.Name.ToUpper() == tagName.ToUpper());
            if(tag != null)
            {
                var dtl = ctx.tagTemplateDtls.Create();
                dtl.tagTemplateHdrIntno = id;
                dtl.tagId = tag.Id;
                dtl.MinScore = minScore;
                dtl.Weight = weight;
                ctx.tagTemplateDtls.Add(dtl);
                ctx.SaveChanges();
            }
            var hdr = ctx.tagTemplateHdrs.Find(id);
            return PartialView("_templateDtlList", hdr);
        }

        public ActionResult saveDetail(long id, string tagName, int minScore, int weight)
        {
            var tag = ctx.tags.FirstOrDefault(x => x.Name.ToUpper() == tagName.ToUpper());
            var dtl = ctx.tagTemplateDtls.Find(id); 
            if (tag != null)
            {
                dtl.tagId = tag.Id;
                dtl.MinScore = minScore;
                dtl.Weight = weight;
                ctx.SaveChanges();
            }
            var hdr = dtl.tagTemplateHdr;
            return PartialView("_templateDtlList", hdr);
        }
        public ActionResult deleteDetail(long id)
        {

            var dtl = ctx.tagTemplateDtls.Find(id);
            var hdr = dtl.tagTemplateHdr;
            ctx.tagTemplateDtls.Remove(dtl);
            ctx.SaveChanges();

            return PartialView("_templateDtlList", hdr);
        }
    }
}