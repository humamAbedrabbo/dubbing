using dubbingModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.Entity;

namespace voiceLibrary.Controllers
{
    public class TagsController : Controller
    {
        DUBBDBEntities ctx = new DUBBDBEntities();

        // GET: Tags
        public ActionResult Index()
        {
            var model = ctx.tagTypes.Include(x => x.tags).OrderBy(x => x.Name).ToList();
            return View(model);
        }

        public ActionResult addTag(int tagTypeId, string tagName)
        {
            var t = ctx.tags.Create();
            t.TagTypeId = tagTypeId;
            t.Name = tagName;
            ctx.tags.Add(t);
            ctx.SaveChanges();
            var model = ctx.tagTypes.Include(x => x.tags).OrderBy(x => x.Name).ToList();
            return PartialView("_Tags", model);
        }
    }
}