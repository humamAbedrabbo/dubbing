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
            return View();
        }
        public ActionResult templateList()
        {
            var model = ctx.tagTemplateHdrs.Include(x=>x.tagTemplateDtls);
            return PartialView("_templateList", model);
        }

        public ActionResult editTemplate(long id)
        {
            var model = ctx.tagTemplateHdrs.Find(id);
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
    }
}