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
    }
}