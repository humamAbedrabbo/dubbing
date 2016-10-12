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
            var model = ctx.tags.Include(x => x.tagType);
            return View();
        }
    }
}