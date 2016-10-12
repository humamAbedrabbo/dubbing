using dubbingModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;
using System.Web.Mvc;

namespace voiceLibrary.Controllers
{
    public class tagsController : Controller
    {
        DUBBDBEntities ctx = new DUBBDBEntities();

        // GET: tags
        public ActionResult Index()
        {
            
            ViewBag.tagTypes = ctx.tagTypes.OrderBy(x=>x.Name);
            return View();
        }

        public ActionResult tagList(int typeId = 0)
        {
            var model = ctx.tags.Where(x=>x.TagTypeId == typeId).OrderBy(x=>x.Name);

            return PartialView("_tagList", model);
        }

        public void saveTag(int tagId, string name)
        {
            var tag = ctx.tags.Find(tagId);
            tag.Name = name;
            ctx.SaveChanges();
        }

        public ActionResult deleteTag(int tagId, int typeId)
        {
            var tag = ctx.tags.Find(tagId);
            ctx.tags.Remove(tag);
            ctx.SaveChanges();
            return tagList(typeId);
        }
        public ActionResult saveNewTag(string name, int typeId)
        {
            var tag = ctx.tags.Create();
            tag.Name = name;
            tag.TagTypeId = typeId;
            ctx.tags.Add(tag);
            ctx.SaveChanges();
            return tagList(typeId);
        }
    }
}